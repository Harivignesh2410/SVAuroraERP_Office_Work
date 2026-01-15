
namespace SVAuroraERP.Domain.Online.OEMVendorMapping
{
    [Table("tOEMVendorDealerMapping")]
    public class OEMVendorDealerMapping
    {
        [Column("PK_OEMVendorDealerMappingID"), Key] public int OEMVendorDealerMappingID { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("FK_EmbossingStationID")] public int EmbossingStationID { get; set; }
        [Column("FK_OEMVendorCodeMappingID")] public int OEMVendorCodeMappingID { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    [Table("V_OEMVendorDealerMapping")]
    public class VOEMVendorDealerMapping
    {
        [Column("PK_OEMVendorDealerMappingID"), Key] public int OEMVendorDealerMappingID { get; set; }
        [Column("FK_HSRPOEMID")] public int HSRPOEMID { get; set; }
        [Column("OEMCompanyName")] public string? OEMName { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("DealerName")] public string? DealerName { get; set; }
        [Column("FK_EmbossingStationID")] public int EmbossingStationID { get; set; }
        [Column("EmbossingStationName")] public string? EmbossingStationName { get; set; }
        [Column("FK_OEMVendorCodeMappingID")] public int OEMVendorCodeMappingID { get; set; }
        [Column("VendorCode")] public string? VendorCode { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}
