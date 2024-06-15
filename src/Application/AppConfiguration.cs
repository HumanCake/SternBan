using Domain;
using FluentValidation;
using Infrastructure;
using MongoDB.Driver;

namespace Application;

public static class AppConfiguration
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

        services.AddSingleton<IMongoClient>(_ =>
        {
            var connectionString = "mongodb://localhost:27017";
            return new MongoClient(connectionString);
        });


        services.AddScoped<IDatabase, MongoDb>();

        services.AddScoped<IKanbanService, KanbanService>();

        services.AddScoped<IValidator<Board>, BoardValidator>();
    }

    public static void ConfigureMiddleware(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}