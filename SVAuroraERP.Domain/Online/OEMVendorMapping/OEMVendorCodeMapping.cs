
namespace SVAuroraERP.Domain.Online.OEMVendorMapping
{
    [Table("tOEMVendorCodeMapping")]
    public class OEMVendorCodeMapping
    {
        [Column("PK_OEMVendorCodeMappingID"),Key] public int OEMVendorCodeMappingID { get; set; }
        [Column("FK_HSRPOEMID")] public int HSRPOEMID { get; set; }
        [Column("VendorCode")] public string? VendorCode { get; set; }
        [Column("FK_DistrictID")] public int DistrictID { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_OEMVendorCodeMapping")]
    public class VOEMVendorCodeMapping
    {
        [Column("PK_OEMVendorCodeMappingID"),Key] public int OEMVendorCodeMappingID { get; set; }
        [Column("FK_HSRPOEMID")] public int HSRPOEMID { get; set; }
        [Column("OEMCompanyName")] public string? OEMCompanyName { get; set; }
        [Column("CompanyName")] public string? CompanyName { get; set; }
        [Column("OEMName")] public string? OEMName { get; set; }
        [Column("VendorCode")] public string? VendorCode { get; set; }
        [Column("FK_DistrictID")] public int DistrictID { get; set; }
        [Column("FK_StateID")] public int StateID { get; set; }
        [Column("StateName")] public string? StateName { get; set; }
        [Column("DistrictName")] public string? DistrictName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}