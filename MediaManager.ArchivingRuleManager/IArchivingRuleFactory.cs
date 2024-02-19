namespace MediaManager.ArchivingRuleManager
{
    /// <summary>
    /// Represents a factory for archiving rules.
    /// </summary>
    public interface IArchivingRuleFactory 
    {
        /// <summary>
        /// Creates all enabled archiving rules.
        /// </summary>
        /// <returns></returns>
        List<IArchivingRule> CreateAllEnabledArchivingRules();
    }
}
