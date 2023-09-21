using MediaManager.Common;
using MediaManager.Domain.DTOs;

namespace MediaManager.Repositories
{
    /// <summary>
    /// Configuration for work with database
    /// </summary>
    public interface IRepository
    {
        /// <summary>
        /// Works with database
        /// </summary>
        /// <param name="callEvent"></param>
        void SaveCallToDatabase(CallEvent callEvent);
        /// <summary>
        /// Sets status archiving of recording 
        /// </summary>
        /// <param name="callEvent"></param>
        void SetArchivingStatus(CallEvent callEvent, string archivingFilePath, ArchivingStatus archivingStatus);
    }
}
