using engagement_service.Data;
using engagement_service.Repositories.Implements;
using engagement_service.Repositories.Interfaces;
using engagement_service.Services.Implements;
using engagement_service.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<EngagementDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();

// Register Services
builder.Services.AddScoped<IArticleService, ArticleService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Engagement Service API",
        Version = "v1",
        Description = "REST API for managing News/Articles in BioPestControl"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Auto-create DB and apply seed data on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EngagementDbContext>();
    dbContext.Database.EnsureCreated();
}

app.Run();
