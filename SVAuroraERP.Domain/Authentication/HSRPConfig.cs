namespace SVAuroraERP.Domain.Authentication
{
    [Table("tHSRPRoleConfig")]
    public class HSRPRoleConfig
    {
        [Column("SuperAdminRoleID")] public int? SuperAdminRoleID { get; set; }
        [Column("AdminRoleID")] public int? AdminRoleID { get; set; }
        [Column("EmbossingStationRoleID")] public int? EmbossingStationRoleID { get; set; }
        [Column("OEMRoleID")] public int OEMRoleID { get; set; }
        [Column("DealerRoleID")] public int? DealerRoleID { get; set; }
        [Column("DealerSubUserID")] public int? DealerSubUserID { get; set; }
        [Column("EmbossingSubUserID")] public int? EmbossingSubUserID { get; set; }
        [Column("OEMSubUserID")] public int? OEMSubUserID { get; set; }
        [Column("LastUpdatedBy")] public int? LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
    }

    [Table("V_OEMConfig")]
    public class VOEMConfig
    {
        [Column("TVSOEMID")] public int TVSOEMID { get; set; }
        [Column("SaravanaEngOEMID")] public int SaravanaEngOEMID { get; set; }
        [Column("EroyceMotorsOEMID")] public int EroyceMotorsOEMID { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    [Table("V_HSRPRoleConfig")]
    public class VHSRPRoleConfig
    {
        [Column("SuperAdminRoleID")] public int SuperAdminRoleID { get; set; }
        [Column("AdminRoleID")] public int AdminRoleID { get; set; }
        [Column("EmbossingStationRoleID")] public int EmbossingStationRoleID { get; set; }
        [Column("OEMRoleID")] public int OEMRoleID { get; set; }
        [Column("DealerRoleID")] public int DealerRoleID { get; set; }
        [Column("DealerSubUserID")] public int DealerSubUserID { get; set; }
        [Column("EmbossingSubUserID")] public int EmbossingSubUserID { get; set; }
        [Column("OEMSubUserID")] public int OEMSubUserID { get; set; }



        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    public class HSRPConfigResponse
    {
        public List<VOEMConfig> OEMConfigList { get; set; } = new();
        public List<VHSRPRoleConfig> RoleConfigList { get; set; } = new();
    }

}
