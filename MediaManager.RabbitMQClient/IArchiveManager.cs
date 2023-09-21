using MediaManager.Common;
using MediaManager.Domain.DTOs;

namespace MediaManager.RabbitMQClient
{
    /// <summary>
    /// Represents an archiver  for archive files recieved from RabbitMQ
    /// </summary>
    public interface IArchiveManager
    {
        /// <summary>
        ///  Copies recording file  to a specified file path.
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        Task ArchiveCallEventAsync(CallEvent callEvent);
        /// <summary>
        /// Constructs the archive path for a given call event based on the configuration settings and call event details.
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        string GetArchivePath(CallEvent callEvent);
    }
}
