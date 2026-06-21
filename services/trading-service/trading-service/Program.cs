using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using trading_service.Data;
using trading_service.DTOs;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──────────────────────────────────────────────────
builder.Services.AddDbContext<TradingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Redis Cache ───────────────────────────────────────────────
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// ── CORS ──────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:4000",
                "http://localhost:5173",
                "http://localhost:8080")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ── JWT Authentication (phải trùng Issuer / Audience / Key với identity-service) ──
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Key"];
if (string.IsNullOrWhiteSpace(secretKey))
    throw new InvalidOperationException("JwtSettings:Key is not configured.");

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
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        // Khớp cách identity-service phát hành claim (JwtTokenService)
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = JwtRegisteredClaimNames.Sub
    };
});

// ── Controllers ───────────────────────────────────────────────
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .SelectMany(e => e.Value.Errors.Select(x => x.ErrorMessage))
                .ToList();

            var result = new ApiResponse<object>
            {
                Success = false,
                Message = "Dữ liệu không hợp lệ: " + string.Join("; ", errors)
            };

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(result);
        };
    });

// ── Swagger ───────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Trading Service API", Version = "v1" });
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ── Code First: Auto-apply migrations on startup ─────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TradingDbContext>();
    db.Database.Migrate(); 
}

using (var scope = app.Services.CreateScope()) { var dbContext = scope.ServiceProvider.GetRequiredService<TradingDbContext>(); dbContext.Database.EnsureCreated(); }

app.Run();
