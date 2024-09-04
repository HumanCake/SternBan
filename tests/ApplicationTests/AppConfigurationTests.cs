using Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace ApplicationTests;

[TestFixture]
public class AppConfigurationTests
{
    [Test]
    public void ConfigureServices_Should_NotThrowException()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        var services = new ServiceCollection();

        // Act & Assert
        Assert.DoesNotThrow(() => services.ConfigureServices(builder.Configuration));
    }

    [Test]
    public void ConfigureMiddleware_Should_NotThrowException()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();

        builder.Services.ConfigureServices(builder.Configuration);

        var app = builder.Build();

        app.ConfigureMiddleware();

        // Act & Assert
        Assert.DoesNotThrow(() => app.ConfigureMiddleware());
    }
}