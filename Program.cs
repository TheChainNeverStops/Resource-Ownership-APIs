public class Program
    {
        public static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                 .AddJsonFile("appsettings.json")
                 .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true)
                 .Build();
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration)
                        .WriteTo.Console()
                        .WriteTo.File($"Logs/{DateTime.UtcNow:dd-MMM-yyyy}.txt")
                        .CreateLogger();

            Log.Information("Resource Ownership Api starting...");
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
