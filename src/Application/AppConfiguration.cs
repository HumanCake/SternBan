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

        // Register IMongoClient with dependency injection
        services.AddSingleton<IMongoClient>(provider =>
        {
            // Replace "your_connection_string" and "your_database_name" with your actual MongoDB connection string and database name
            var connectionString = "mongodb://localhost:27017";

            // Create and return a new MongoClient instance
            return new MongoClient(connectionString);
        });


        services.AddScoped<IDatabase, MongoDb>();

        services.AddScoped<IKanbanService, KanbanService>();

        services.AddScoped<IBoardValidator, BoardValidator>();
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