using Microsoft.Extensions.Configuration;
using System.IO;

namespace ExcelTest.Env
{
    public class SystemConfig
    {
        public static string GetSettingset(string key)
        {
            var builder = new ConfigurationBuilder()
                .AddInMemoryCollection() // 将配置文件加载至缓存中
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();
            return configuration[key];
        }
    }
}