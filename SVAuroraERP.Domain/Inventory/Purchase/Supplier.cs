namespace SVAuroraERP.Domain.Purchase
{
    [Table("tSupplier")]

    public class Supplier
    {
        [Column("PK_SupplierID"), Key] public int SupplierID { get; set; }
        [Column("SupplierCode")] public string? SupplierCode { get; set; }
        [Column("SupplierName")] public string? SupplierName { get; set; }
        [Column("GSTNo")] public string? GSTNo { get; set; }
        [Column("AddressLine1")] public string? AddressLine1 { get; set; }
        [Column("AddressLine2")] public string? AddressLine2 { get; set; }
        [Column("City")] public string? City { get; set; }
        [Column("State")] public string? State { get; set; }
        [Column("Country")] public string? Country { get; set; }
        [Column("Pincode")] public string? Pincode { get; set; }
        [Column("TelNo1")] public string? TelNo1 { get; set; }
        [Column("TelNo2")] public string? TelNo2 { get; set; }
        [Column("MobileNo")] public string? MobileNo { get; set; }
        [Column("Email")] public string? Email { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public long LoginAuditID { get; set; }

    }

    [Table("V_Supplier")]

    public class VSupplier
    {
        [Column("PK_SupplierID"), Key] public int SupplierID { get; set; }
        [Column("SupplierCode")] public string? SupplierCode { get; set; }
        [Column("SupplierName")] public string? SupplierName { get; set; }
        [Column("GSTNo")] public string? GSTNo { get; set; }
        [Column("AddressLine1")] public string? AddressLine1 { get; set; }
        [Column("AddressLine2")] public string? AddressLine2 { get; set; }
        [Column("City")] public string? City { get; set; }
        [Column("State")] public string? State { get; set; }
        [Column("Country")] public string? Country { get; set; }
        [Column("Pincode")] public string? Pincode { get; set; }
        [Column("TelNo1")] public string? TelNo1 { get; set; }
        [Column("TelNo2")] public string? TelNo2 { get; set; }
        [Column("MobileNo")] public string? MobileNo { get; set; }
        [Column("Email")] public string? Email { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDateIST")] public DateTime LastUpdatedDateIST { get; set; }

    }
}
