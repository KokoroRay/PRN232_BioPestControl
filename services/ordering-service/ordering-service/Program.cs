using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ordering_service.Data;
using ordering_service.DTOs;
using System.Net;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────
builder.Services.AddDbContext<OrderingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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

// ── JWT Authentication ────────────────────────────────────────
// Dùng CÙNG JwtSettings với identity-service để xác thực token
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer           = true,
        ValidateAudience         = true,
        ValidateLifetime         = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer              = jwtSettings["Issuer"],
        ValidAudience            = jwtSettings["Audience"],
        IssuerSigningKey         = new SymmetricSecurityKey(
                                       Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
    };
});

// ── Controllers ───────────────────────────────────────────────
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

// ── Swagger với hỗ trợ Bearer Token ──────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering Service API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Type        = SecuritySchemeType.Http,
        Scheme      = "Bearer",
        BearerFormat = "JWT",
        In          = ParameterLocation.Header,
        Description = "Nhập JWT Token từ identity-service để xác thực"
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

// ═══════════════════════════════════════════════════════════════
var app = builder.Build();

// ── Global Exception Handler ──────────────────────────────────
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode  = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var feature      = context.Features.Get<IExceptionHandlerFeature>();
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

// ── Middleware Pipeline ───────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();   // Phải trước UseAuthorization
app.UseAuthorization();

app.MapControllers();

// ── Code First: Auto-apply migrations on startup ─────────────
// Migrate() sẽ tạo DB nếu chưa có VÀ apply tất cả pending migrations
// Đây là chuẩn Code First thay vì EnsureCreated()
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderingDbContext>();
    db.Database.Migrate();
}

app.Run();
