using Google;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace MediaManager.Service
{
    /// <summary>
    /// Represents the Windows service 
    /// </summary>
    public class ManagerService : ServiceBase
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<ManagerService> _logger;
        /// <summary>
        /// Host
        /// </summary>
        private readonly IHost _host;
        /// <summary>
        /// Cancellation Token Source
        /// </summary>
        private readonly CancellationTokenSource _stopHostToken;

        /// <summary>
        /// Settings for library service with host 
        /// </summary>
        /// <param name="host"></param>
        public ManagerService(IHost host)
        {
            ILoggerFactory? loggerFactory = (ILoggerFactory?)host.Services.GetService(typeof(ILoggerFactory));
            _logger = loggerFactory?.CreateLogger<ManagerService>() ?? NullLogger<ManagerService>.Instance;
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
