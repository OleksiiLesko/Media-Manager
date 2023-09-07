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
    }
}
