using Microsoft.Extensions.Configuration;
using System.IO;

public static class ConfigurationHelper
{
    public static IConfiguration Configuration { get; }

    static ConfigurationHelper()
    {
        var basePath = Directory.GetParent(AppContext.BaseDirectory)?.FullName;

        Configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }

    public static string GetConnectionString(string name)
    {
        return Configuration.GetConnectionString(name);
    }
}