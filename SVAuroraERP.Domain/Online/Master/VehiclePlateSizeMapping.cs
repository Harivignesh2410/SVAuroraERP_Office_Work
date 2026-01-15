namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tVehiclePlateSizeMapping")]
    public class VehiclePlateSizeMapping
    {
        [Column("PK_VehiclePlateSizeMappingID"), Key] public int VehiclePlateSizeMappingID { get; set; }
        [Column("FK_VehicleCategoryID")] public byte VehicleCategoryID { get; set; }
        [Column("FK_VehicleTypeID")] public byte VehicleTypeID { get; set; }
        [Column("FK_VehicleClassID")] public int VehicleClassID { get; set; }
        [Column("FK_FuelID")] public byte FuelID { get; set; }
        [Column("FK_VehiclePlateColorID")] public int VehiclePlateColorID { get; set; }
        [Column("FK_VehiclePlateTypeID")] public byte VehiclePlateTypeID { get; set; }
        [Column("FK_VehiclePlateSizeID")] public int VehiclePlateSizeID { get; set; }
        [Column("Description")] public string? Description { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_VehiclePlateSizeMapping")]
    public class VVehiclePlateSizeMapping
    {
        [Column("PK_VehiclePlateSizeMappingID"), Key] public int VehiclePlateSizeMappingID { get; set; }
        [Column("FK_VehiclePlateTypeID")] public byte VehiclePlateTypeID { get; set; }
        [Column("VehiclePlateType")] public string? VehiclePlateTypeName { get; set; }
        [Column("FK_VehicleClassID")] public int VehicleClassID { get; set; }
        [Column("VehicleClassName")] public string? VehicleClassName { get; set; }
        [Column("FK_VehiclePlateSizeID")] public int VehiclePlateSizeID { get; set; }
        [Column("VehiclePlateSizeName")] public string? VehiclePlateSizeName { get; set; }
        [Column("Description")] public string? Description { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("FK_VehicleCategoryID")] public byte VehicleCategoryID { get; set; }
        [Column("FK_VehicleTypeID")] public byte VehicleTypeID { get; set; }
        [Column("FK_FuelID")] public byte FuelID { get; set; }
        [Column("FK_VehiclePlateColorID")] public int VehiclePlateColorID { get; set; }
        [Column("FuelName")] public string? FuelName { get; set; }
        [Column("VehicleCategoryName")] public string? VehicleCategoryName { get; set; }
        [Column("VehicleTypeName")] public string? VehicleTypeName { get; set; }
        [Column("VehiclePlateColorName")] public string? VehiclePlateColorName { get; set; }




    }
}