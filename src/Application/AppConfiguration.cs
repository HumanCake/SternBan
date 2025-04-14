using Domain;
using FluentValidation;
using Infrastructure;
using Microsoft.OpenApi;
using MongoDB.Driver;

namespace Application;

public static class AppConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOpenApi(options =>
        {
            options.OpenApiVersion = OpenApiSpecVersion.OpenApi2_0;
        });
        
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

        services.AddSingleton<IMongoClient>(_ =>
        {
            // Check environment variable first (docker), fall back to appsettings.json
            var connectionString = Environment.GetEnvironmentVariable("Mongo:Url")
                ?? configuration.GetSection("Mongo:Url").Value;
            Console.WriteLine("This is the mongo connection stiring: " + connectionString);

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("MongoDB connection string is not configured.");
            }
            return new MongoClient(connectionString);
        });


        services.AddScoped<IDatabase, MongoDb>();

        services.AddScoped<IKanbanService, KanbanService>();

        services.AddScoped<IValidator<Board>, BoardValidator>();
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
    }

    public static void ConfigureMiddleware(this IApplicationBuilder app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("AllowAll");
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}