using Microsoft.Extensions.Configuration;
using System.IO;

namespace CYS.Repos
{
    public static class RepoHelper
    {
        public static string GetConnectionString()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            return configuration.GetConnectionString("DefaultConnection");
        }
    }
} 