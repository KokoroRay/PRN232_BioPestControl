using article_service.Data;
using article_service.Repositories.Implements;
using article_service.Repositories.Interfaces;
using article_service.Services.Implements;
using article_service.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Register MongoDB Context as Singleton (MongoClient is thread-safe)
builder.Services.AddSingleton<MongoDbContext>();

// Register Repositories
builder.Services.AddScoped<IArticleRepository, ArticleRepository>();

// Register Services
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IContactService, ContactService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Engagement Service API",
        Version = "v1",
        Description = "REST API for managing News/Articles in BioPestControl (MongoDB)"
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();
        await context.InitSeedDataAsync();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
