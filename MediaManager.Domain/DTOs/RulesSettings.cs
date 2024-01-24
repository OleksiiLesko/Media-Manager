namespace MediaManager.Domain.DTOs
{
    /// <summary>
    /// Archiving rules.
    /// </summary>
    public class RulesSettings
    {
        public MediaTypeArchivingRule MediaTypeArchivingRule { get; set; }
        public CallDirectionArchivingRule CallDirectionArchivingRule { get; set; }
    }
}
