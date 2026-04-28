using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using identity_service.Data;
using identity_service.Services;

var builder = WebApplication.CreateBuilder(args);

// Setup CORS: Cho phép frontend/test page gọi API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:8080",   // Test HTML page
                "http://localhost:5240",   // API (http)
                "https://localhost:7022",  // API (https)
                "http://localhost:3000",   // Frontend (nếu có)
                "http://localhost:5173"    // Vite frontend (nếu có)
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add services to the container.

// Setup Database: Khai báo sử dụng Entity Framework Core với CSDL SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Setup custom services: Đăng ký JwtTokenService vào hệ thống (Dependency Injection) để sử dụng trong các Controller
builder.Services.AddScoped<JwtTokenService>();

// Setup Authentication (JWT): Cấu hình cơ chế xác thực bằng JWT (JSON Web Token)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Key"];

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Bật giao diện test API Swagger khi chạy ở môi trường Development
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Bật CORS (phải đặt trước Authentication/Authorization)
app.UseCors("AllowAll");

// IMPORTANT: app.UseAuthentication() (Kiểm tra token) bắt buộc phải nằm trước app.UseAuthorization() (Kiểm tra quyền hạn)
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
