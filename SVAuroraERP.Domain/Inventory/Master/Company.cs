namespace SVAuroraERP.Domain.Master
{
    [Table("tCompany")]
    public class Company
    {
        [Column("PK_CompanyID"), Key] public int CompanyID { get; set; }
        [Column("CompanyName")] public string? CompanyName { get; set; }
        [Column("GSTNo")] public string? GSTNo { get; set; }
        [Column("AddressLine1")] public string? AddressLine1 { get; set; }
        [Column("AddressLine2")] public string? AddressLine2 { get; set; }
        [Column("City")] public string? City { get; set; }
        [Column("State")] public string? State { get; set; }
        [Column("Pincode")] public string? Pincode { get; set; }
        [Column("TelNo1")] public string? TelNo1 { get; set; }
        [Column("TelNo2")] public string? TelNo2 { get; set; }
        [Column("MobileNo")] public string? MobileNo { get; set; }
        [Column("Email")] public string? Email { get; set; }
        [Column("BankName")] public string? BankName { get; set; }
        [Column("BranchName")] public string? BranchName { get; set; }
        [Column("IFSCCode")] public string? IFSCCode { get; set; }
        [Column("AccountHolderName")] public string? AccountHolderName { get; set; }
        [Column("AccountType")] public byte AccountType { get; set; }
        [Column("AccountNo")] public string? AccountNo { get; set; }
        [Column("PANNo")] public string? PANNo { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_Company")]
    public class VCompany
    {
        [Column("PK_CompanyID"), Key] public int CompanyID { get; set; }
        [Column("CompanyName")] public string? CompanyName { get; set; }
        [Column("GSTNo")] public string? GSTNo { get; set; }
        [Column("AddressLine1")] public string? AddressLine1 { get; set; }
        [Column("AddressLine2")] public string? AddressLine2 { get; set; }
        [Column("City")] public string? City { get; set; }
        [Column("State")] public string? State { get; set; }
        [Column("Pincode")] public string? Pincode { get; set; }
        [Column("TelNo1")] public string? TelNo1 { get; set; }
        [Column("TelNo2")] public string? TelNo2 { get; set; }
        [Column("MobileNo")] public string? MobileNo { get; set; }
        [Column("Email")] public string? Email { get; set; }
        [Column("BankName")] public string? BankName { get; set; }
        [Column("BranchName")] public string? BranchName { get; set; }
        [Column("IFSCCode")] public string? IFSCCode { get; set; }
        [Column("AccountHolderName")] public string? AccountHolderName { get; set; }
        [Column("AccountType")] public byte AccountType { get; set; }
        [Column("AccountNo")] public string? AccountNo { get; set; }
        [Column("PANNo")] public string? PANNo { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}
