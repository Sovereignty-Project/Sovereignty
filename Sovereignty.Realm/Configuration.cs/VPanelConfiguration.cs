namespace Sovereignty.Realm.Configuration.cs;

public class VPanelConfiguration
{
    public const string SectionName = "VPanel";
    
    public string REDIS_HOST { get; set; } = String.Empty;
    public string DB_HOST { get; set; } = String.Empty;
    public string DB_DATABASE { get; set; } = String.Empty;
    public string DB_USER { get; set; } = String.Empty;
    public string DB_PASSWORD { get; set; } = String.Empty;
}