namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tVehiclePlateImage")]
    public class VehiclePlateImage
    {
        [Column("PK_VehiclePlateImageID"), Key] public int VehiclePlateImageID { get; set; }
        [Column("FK_VehiclePlateSizeID")] public int VehiclePlateSizeID { get; set; }
        [Column("FK_VehiclePlateColorID")] public int VehiclePlateColorID { get; set; }
        [Column("FrontImageURL")] public string? FrontImageURL { get; set; }
        [Column("RearImageURL")] public string? RearImageURL { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    } 
    [Table("V_VehiclePlateImage")]
    public class VVehiclePlateImage
    {
        [Column("PK_VehiclePlateImageID"), Key] public int VehiclePlateImageID { get; set; }
        [Column("FK_VehiclePlateSizeID")] public int VehiclePlateSizeID { get; set; }
        [Column("VehiclePlateSizeName")] public string? VehiclePlateSizeName { get; set; }
        [Column("FK_VehiclePlateColorID")] public int VehiclePlateColorID { get; set; }
        [Column("VehiclePlateColorName")] public string? VehiclePlateColorName { get; set; }
        [Column("FrontImageURL")] public string? FrontImageURL { get; set; }
        [Column("RearImageURL")] public string? RearImageURL { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        
    }
}
