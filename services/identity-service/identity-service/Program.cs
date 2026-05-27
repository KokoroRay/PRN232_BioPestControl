using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json;
using identity_service.Data;
using identity_service.DTOs;
using identity_service.DTOs.Requests;
using identity_service.DTOs.Responses;
using identity_service.Repositories.Interfaces;
using identity_service.Repositories.Implements;
using identity_service.Services.Interfaces;
using identity_service.Services.Implements;

var builder = WebApplication.CreateBuilder(args);

// Setup CORS: Cho phép frontend/test page gọi API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:8080",
                "http://localhost:5240",
                "https://localhost:7022",
                "http://localhost:3000",
                "http://localhost:4000",
                "http://localhost:5173"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.

// Setup Database: Khai báo sử dụng Entity Framework Core với CSDL SQL Server
var azureSqlConnectionString = builder.Configuration["AzureSql:ConnectionString"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(azureSqlConnectionString))
{
    throw new InvalidOperationException("Azure SQL connection string is not configured.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(azureSqlConnectionString));

// Setup custom services: Đăng ký JwtTokenService vào hệ thống (Dependency Injection) để sử dụng trong các Controller
builder.Services.AddScoped<JwtTokenService>();

// ── Staff Management + IAM ──────────────────────────────────────────────────
// DI Registration
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IStaffService, StaffService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPasswordResetService, PasswordResetService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

// Setup Authentication (JWT): Cấu hình cơ chế xác thực bằng JWT (JSON Web Token)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Key"];

if (string.IsNullOrWhiteSpace(secretKey))
{
    throw new InvalidOperationException("JwtSettings:Key is not configured.");
}

builder.Services.AddAuthentication(options =>
{
    // Cài đặt mặc định là dùng JWT Bearer cho bước xác thực (Authentication)
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,                  // Kiểm tra Issuer (Người phát hành Token) có đúng không
        ValidateAudience = true,                // Kiểm tra Audience (Nơi Token được dùng) có đúng không
        ValidateLifetime = true,                // Kiểm tra Token còn hạn sử dụng hay không
        ValidateIssuerSigningKey = true,        // Kiểm tra chữ ký bảo mật (chống giả mạo Token)
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        // Chỉ định claim nào trong JWT sẽ được dùng cho Role và Name
        // Mặc định .NET map sai khi dùng chuẩn JWT "role" → phải khai báo rõ
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = JwtRegisteredClaimNames.Sub
    };
});

builder.Services.AddControllers();

// Cấu hình giao diện Swagger (Tài liệu API) để hỗ trợ nhập JWT Token
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity Service API", Version = "v1" });

    // Thêm nút "Authorize" vào Swagger để nhập Token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT Token của bạn để kết nối"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ============================================================
// Global Exception Handler — Bắt mọi lỗi không mong muốn
// Trả về 500 dưới dạng ApiResponse thay vì lộ stack trace
// ============================================================
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode  = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
        var errorMessage = app.Environment.IsDevelopment()
            ? errorFeature?.Error.Message ?? "Lỗi không xác định"   // Dev: hiện message cụ thể
            : "Đã xảy ra lỗi phía server. Vui lòng thử lại sau.";   // Prod: ẩn chi tiết lỗi

        var response = new ApiResponse<object>
        {
            Success = false,
            Message = errorMessage
        };

        await context.Response.WriteAsync(
            JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
        );
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Bật giao diện test API Swagger khi chạy ở môi trường Development
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

// Bật CORS (phải đặt trước Authentication/Authorization)
app.UseCors("AllowAll");

// IMPORTANT: app.UseAuthentication() (Kiểm tra token) bắt buộc phải nằm trước app.UseAuthorization() (Kiểm tra quyền hạn)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
