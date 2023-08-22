using System.ServiceProcess;

namespace MediaManager.Service
{
    /// <summary>
    /// Provides extension method for running a host as a Windows service.
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Runs the specified host as a Windows service.
        /// </summary>
        /// <param name="host">The host to run as a service.</param>
        public static void RunAsService(this IHost host)
        {
            var hostService = new MediaManagerService(host);
            ServiceBase.Run(hostService);
        }
    }
}
