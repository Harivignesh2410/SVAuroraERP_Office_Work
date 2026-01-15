namespace SVAuroraERP.Domain
{
    [Table("tGlobalConfig")]
    public class GlobalConfig
    {
        public int RowLimitCount { get; set; }
        public string EncryptionKey { get; set; } = string.Empty;
        public string ReportApiURL { get; set; } = string.Empty;

        //Added on 2025.11.03
        public string ReportAPIKey { get; set; }=string.Empty;
    }
}