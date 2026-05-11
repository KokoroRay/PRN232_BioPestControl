using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Options;
using engagement_service.Data;
using engagement_service.DTOs;
using engagement_service.Services;
using System.Net;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ── Database (Code First) ─────────────────────────────────────
builder.Services.AddDbContext<EngagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<OrderServiceOptions>(builder.Configuration.GetSection(OrderServiceOptions.SectionName));
builder.Services.AddHttpClient(HttpOrderPurchaseEligibilityService.HttpClientName);
builder.Services.AddScoped<NoOpPurchaseEligibilityService>();
builder.Services.AddScoped<HttpOrderPurchaseEligibilityService>();
builder.Services.AddScoped<IPurchaseEligibilityService>(sp =>
{
    var orderOpts = sp.GetRequiredService<IOptions<OrderServiceOptions>>().Value;
    if (orderOpts.UseHttpValidation)
        return sp.GetRequiredService<HttpOrderPurchaseEligibilityService>();
    return sp.GetRequiredService<NoOpPurchaseEligibilityService>();
});

// ── CORS ──────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:5173",
                "http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ── JWT ───────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "DevOnly_ChangeMe_32chars_min____"))
    };
});

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value != null && e.Value.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(x => x.ErrorMessage))
                .ToList();

            var result = new ApiResponse<object>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ: " + string.Join("; ", errors)
            };

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(result);
        };
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Engagement Service API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT từ identity-service (roles: Customer, Staff, Admin)"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var errorMessage = app.Environment.IsDevelopment()
            ? feature?.Error.Message ?? "Lỗi không xác định"
            : "Đã xảy ra lỗi phía server. Vui lòng thử lại sau.";

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(
                new ApiResponse<object> { Success = false, Message = errorMessage },
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            )
        );
    });
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<EngagementDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();
