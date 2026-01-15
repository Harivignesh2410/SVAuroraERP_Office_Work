namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tHomeFitmentPincode")]
    public class HomeFitmentPincode
    {
        [Column("PK_HomeFitmentPincodeID"), Key] public int HomeFitmentPincodeID { get; set; }
        [Column("FK_DistrictID")] public int DistrictID { get; set; }
        [Column("Location")] public string? Location { get; set; }
        [Column("Pincode")] public string? Pincode { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_HomeFitmentPincode")]
    public class VHomeFitmentPincode
    {
        [Column("PK_HomeFitmentPincodeID"), Key] public int HomeFitmentPincodeID { get; set; }
        [Column("FK_StateID")] public int StateID { get; set; }
        [Column("StateName")] public string? StateName { get; set; }
        [Column("FK_DistrictID")] public int DistrictID { get; set; }
        [Column("DistrictName")] public string? DistrictName { get; set; }
        [Column("Location")] public string? Location { get; set; }
        [Column("Pincode")] public string? Pincode { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
}