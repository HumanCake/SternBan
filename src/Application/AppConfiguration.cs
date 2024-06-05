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