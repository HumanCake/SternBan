using Application;
using Microsoft.AspNetCore.Builder;

namespace ApplicationTests;

[TestFixture]
public class AppConfigurationTests
{
    [Test]
    public void ConfigureServices_And_ConfigureMiddleware_Should_NotThrowException()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        // Act & Assert
        Assert.DoesNotThrow(() =>
        {
            builder.Services.ConfigureServices(builder.Configuration);
            var app = builder.Build();
            app.ConfigureMiddleware();
        });
    }
}