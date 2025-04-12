using System.Globalization;

namespace SV21T1020873.Shop
{
    public static class WebConfig
    {
        private static readonly IConfigurationRoot _configuration;

        static WebConfig()
        {
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
        }
        public static string ProductPhotoURL => _configuration["ProductPhotoURL"] ?? "";
    }

}
