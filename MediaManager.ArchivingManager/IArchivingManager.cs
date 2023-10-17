using MediaManager.Domain.DTOs;

namespace MediaManager.ArchivingEventManager
{
    /// <summary>
    /// Represents an archiving manager for archiving event and sent status to RabbitMQ.
    /// </summary>
    public interface IArchivingManager
    {
        /// <summary>
        /// Handle archiving event for sent status to RabbitMQ
        /// </summary>
        /// <param name="callEvent"></param>
        byte[] HandleArchivingEvent(CallEvent callEvent);
    }
}