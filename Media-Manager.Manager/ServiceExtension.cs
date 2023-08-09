using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Media_Manager.Manager
{
    /// <summary>
    /// Class for run project as service
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Run media-manager service 
        /// </summary>
        /// <param name="host"></param>
        public static void RunAsService(this IHost host)
        {
            var hostService = new ManagerService(host);
            ServiceBase.Run(hostService);
        }
    }
}
