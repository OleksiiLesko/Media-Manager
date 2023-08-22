using Microsoft.Extensions.Logging.Abstractions;
using System.ServiceProcess;

namespace MediaManager.Service
{
    /// <summary>
    /// Represents a service that running as a windows service
    /// </summary>
    public class MediaManagerService : ServiceBase
    {
        private readonly ILogger<MediaManagerService> _logger;
 
        private readonly IHost _host;

        private readonly CancellationTokenSource _stopHostToken;

        /// <summary>
        ///Initializes a new instance of the MediaManagerService
        /// </summary>
        /// <param name="host"></param>
        public MediaManagerService(IHost host)
        {
            ILoggerFactory? loggerFactory = (ILoggerFactory?)host.Services.GetService(typeof(ILoggerFactory));
            _logger = loggerFactory?.CreateLogger<MediaManagerService>() ?? NullLogger<MediaManagerService>.Instance;
            _host = host;
            _stopHostToken = new CancellationTokenSource();
        }

        /// <summary>
        /// Starts the Windows service.
        /// </summary>
        protected override void OnStart(string[] args)
        {
            _logger.LogInformation("MediaManager service is starting");

            Task.Run(async () =>
            {
                try
                {
                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await _host.RunAsync(_stopHostToken.Token);
                }
                catch (Exception e)
                {
                    _logger.LogCritical(e, "Host builder init is failed");
                }
            });

            _logger.LogInformation("MediaManager service is started");
        }


        /// <summary>
        /// Stops the Windows service
        /// </summary>
        protected override void OnStop()
        {
            _stopHostToken.Cancel();
            base.OnStop();

            _logger.LogInformation("MediaManager service is  stopped");
        }
    }
}
