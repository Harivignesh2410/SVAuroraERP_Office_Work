namespace SVAuroraERP.Domain
{
    //Added on 2025.07.04
    public class ApplicationSettings
    {
        public string? ProjectCode { get; set; }
        public string? AppName { get; set; }
        public string? AppEdition { get; set; }
        public string? AppVersion { get; set; }
        public DateTime BuildDate { get; set; }
        public string AppCompanyName { get; set; } = string.Empty;
    }
}