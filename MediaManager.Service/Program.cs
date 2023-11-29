using MediaManager.ArchivingEventManager;
using MediaManager.ArchivingRuleManager;
using MediaManager.Domain.DTOs;
using MediaManager.RabbitMQClient;
using MediaManager.Repositories;
using MediaManager.Worker;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using System.Diagnostics;
using System.Reflection;

namespace MediaManager.Service
{
    public class Program
    {
        /// <summary>
        /// Argument name for console 
        /// </summary>
        private const string CONSOLE_ARG_NAME = "--console";
        /// <summary>
        /// Starting method 
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            var pathToContentRoot = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Directory.SetCurrentDirectory(pathToContentRoot!);
            try
            {
                ConfigureLogger();

                IHost host = CreateHostBuilder(args).Build();
                var isService = !Debugger.IsAttached && !args.ToList().Contains(CONSOLE_ARG_NAME);
                if (isService)
                {
                    Log.Information("Running as a service");
                    host.RunAsService();
                }
                else
                {
                    Log.Information("Running as a console");
                    host.Run();
                }

            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
        /// <summary>
        /// Configure Logger
        /// </summary>
        private static void ConfigureLogger()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
        }
        /// <summary>
        /// Create host
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appsettings.json", optional: true);
                })
                  .ConfigureServices((hostContext, services) =>
                  {
                      var configuration = hostContext.Configuration;
                      services.AddHostedService<MediaManagerWorker>();
                      services.AddSingleton<IRabbitMQService, RabbitMQService>();
                      services.AddSingleton<IFileManager, FileManager>();
                      services.AddSingleton<IRepository, Repository>();
                      services.AddSingleton<IArchivingManager, ArchivingManager>();
                      services.AddSingleton<ArchivingRuleManagerService>();
                      services.AddSingleton<List<IArchivingRule>>();
                      services.Configure<RulesSettings>(configuration.GetSection("Rules"));
                  }).UseSerilog();
        }
    }
}