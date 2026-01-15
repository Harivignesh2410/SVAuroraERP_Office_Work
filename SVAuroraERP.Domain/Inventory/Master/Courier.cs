//Added on 2025.04.25 by Harivignesh
namespace SVAuroraERP.Domain.Master
{
    [Table("tCourier")]
    public class Courier
    {
        [Column("PK_CourierID"), Key] public int CourierID { get; set; }
        [Column("CourierCode")] public string? CourierCode { get; set; }
        [Column("CourierName")] public string? CourierName { get; set; }
        [Column("Address")] public string? Address { get; set; }
        [Column("ContactNo1")] public string? ContactNo1 { get; set; }
        [Column("ContactNo2")] public string? ContactNo2 { get; set; }
        [Column("Email")] public string? Email { get; set; }
        [Column("TelNo")] public string? TelNo { get; set; }
        [Column("TrackingURL")] public string? TrackingURL { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted"), JsonIgnore] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdateddate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_Courier")]
    public class VCourier
    {
        [Column("PK_CourierID"), Key] public int CourierID { get; set; }
        [Column("CourierCode")] public string? CourierCode { get; set; }
        [Column("CourierName")] public string? CourierName { get; set; }
        [Column("Address")] public string? Address { get; set; }
        [Column("ContactNo1")] public string? ContactNo1 { get; set; }
        [Column("ContactNo2")] public string? ContactNo2 { get; set; }
        [Column("Email")] public string? Email { get; set; }
        [Column("TelNo")] public string? TelNo { get; set; }
        [Column("TrackingURL")] public string? TrackingURL { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
}
