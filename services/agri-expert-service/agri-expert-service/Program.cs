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
using agri_expert_service.Data;
using agri_expert_service.DTOs.Requests;
using agri_expert_service.DTOs.Responses;
using agri_expert_service.Repositories.Interfaces;
using agri_expert_service.Repositories.Implements;
using agri_expert_service.Services.Interfaces;
using agri_expert_service.Services.Implements;

var builder = WebApplication.CreateBuilder(args);

// Load .env file
DotNetEnv.Env.Load();
DotNetEnv.Env.TraversePath().Load();

// ── CORS ──────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "http://localhost:4000",
                "http://localhost:5173",
                "http://localhost:8080",
                "https://localhost:7022")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── Database ──────────────────────────────────────────────────
var connectionString = builder.Configuration["AzureSql:ConnectionString"]
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Database connection string is not configured.");

builder.Services.AddDbContext<AgriDbContext>(options =>
    options.UseSqlServer(connectionString));

// ── DI: Repositories + Services ───────────────────────────────
builder.Services.AddScoped<IChemicalRepository, ChemicalRepository>();
builder.Services.AddScoped<IChemicalService, ChemicalService>();

// ── JWT Authentication ────────────────────────────────────────
// Dùng chung JwtSettings với identity-service
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey   = jwtSettings["Key"];

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
        IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!)),
        RoleClaimType            = ClaimTypes.Role,
        NameClaimType            = JwtRegisteredClaimNames.Sub
    };
});

// ── Controllers + Model Validation ───────────────────────────
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value!.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(x => x.ErrorMessage))
                .ToList();

            return new Microsoft.AspNetCore.Mvc.BadRequestObjectResult(
                new ApiResponse<object>
                {
                    Success = false,
                    Message = "Dữ liệu không hợp lệ: " + string.Join("; ", errors)
                });
        };
    });

// ── Swagger ───────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title       = "Agri-Expert Service API — Chemical Safety",
        Version     = "v1",
        Description = "Quản lý hóa chất nông nghiệp (UC20 - Manage Chemical Safety)"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name        = "Authorization",
        Type        = SecuritySchemeType.Http,
        Scheme      = "Bearer",
        BearerFormat = "JWT",
        In          = ParameterLocation.Header,
        Description = "Nhập JWT Token từ identity-service"
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
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    });
});

// ── Middleware Pipeline ───────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ── Auto-migrate on startup ───────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AgriDbContext>();
    db.Database.Migrate();
}

app.Run();
