using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using RobinTTY.PersonalFinanceDashboard.API.Tests.TestUtilities;

namespace RobinTTY.PersonalFinanceDashboard.API.Tests;

public class StartupTests
{
    [Fact]
    public void MissingGoCardlessKeys_StopTheStartup()
    {
        using var factory = new ApiFactoryWithoutGoCardlessKeys();

        var exception = Assert.Throws<OptionsValidationException>(factory.CreateClient);

        Assert.Contains("SecretId", exception.Message);
    }

    private class ApiFactoryWithoutGoCardlessKeys : ApiFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);
            builder.UseSetting("AppConfiguration:NordigenApiConfiguration:SecretId", "");
            builder.UseSetting("AppConfiguration:NordigenApiConfiguration:SecretKey", "");
        }
    }
}