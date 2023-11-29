using MediaManager.Domain.DTOs;

namespace MediaManager.ArchivingRuleManager
{
    /// <summary>
    /// Represents an archiving rules for call.
    /// </summary>
    public interface IArchivingRule
    {
        /// <summary>
        /// Priority of the rule.
        /// </summary>
        int Priority { get; }
        /// <summary>
        /// Applies rule for call recordings.
        /// </summary>
        /// <param name="callEvent"></param>
        /// <returns></returns>
        List<int> ApplyRule(List<Recording> recordings);
    }
}
