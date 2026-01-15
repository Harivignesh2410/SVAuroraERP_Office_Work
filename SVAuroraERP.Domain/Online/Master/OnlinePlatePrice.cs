namespace SVAuroraERP.Domain.Online.Master
{
    [Table("tOnlinePlatePrice")]
    public class OnlinePlatePrice
    {
        [Column("PK_OnlinePlatePriceID"), Key] public int OnlinePlatePriceID { get; set; }
        [Column("FK_VehicleCategoryID")] public byte VehicleCategoryID { get; set; }
        [Column("FK_VehicleTypeID")] public byte VehicleTypeID { get; set; }
        [Column("FK_VehicleClassID")] public int VehicleClassID { get; set; }
        [Column("FK_FuelID")] public byte FuelID { get; set; }
        [Column("FK_VehiclePlateColorID")] public int VehiclePlateColorID { get; set; }
        [Column("FK_VehiclePlateTypeID")] public byte VehiclePlateTypeID { get; set; }
        [Column("FK_VehiclePlateSizeID")] public int VehiclePlateSizeID { get; set; }
        [Column("Front")] public decimal Front { get; set; }
        [Column("Rear")] public decimal Rear { get; set; }
        [Column("SnapLock")] public decimal SnapLock { get; set; }
        [Column("TLPSticker")] public decimal TLPSticker { get; set; }
        [Column("EmbossingFitmentCharges")] public decimal EmbossingFitmentCharges { get; set; }
        [Column("DealerFitmentCharges")] public decimal DealerFitmentCharges { get; set; }
        [Column("HomeFitmentCharges")] public decimal HomeFitmentCharges { get; set; }
        [Column("DealerCourierCharge")] public decimal DealerCourierCharge { get; set; }
        [Column("DealerLocationChangeCharge")] public decimal DealerLocationChangeCharge { get; set; }
        [Column("OtherCharges")] public decimal OtherCharges { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_OnlinePlatePrice")]
    public class VOnlinePlatePrice
    {
        [Column("PK_OnlinePlatePriceID"), Key] public int OnlinePlatePriceID { get; set; }
        [Column("FK_VehicleCategoryID")] public byte VehicleCategoryID { get; set; }
        [Column("VehicleCategoryName")] public string? VehicleCategoryName { get; set; }
        [Column("FK_VehicleTypeID")] public byte VehicleTypeID { get; set; }
        [Column("VehicleTypeName")] public string? VehicleTypeName { get; set; }
        [Column("FK_VehicleClassID")] public int VehicleClassID { get; set; }
        [Column("VehicleClassCode")] public string? VehicleClassCode { get; set; }
        [Column("VehicleClassName")] public string? VehicleClassName { get; set; }
        [Column("FK_FuelID")] public byte FuelID { get; set; }
        [Column("FuelName")] public string? FuelName { get; set; }
        [Column("FK_VehiclePlateColorID")] public int VehiclePlateColorID { get; set; }
        [Column("VehiclePlateColorName")] public string? VehiclePlateColorName { get; set; }
        [Column("VehiclePlateColorCode")] public string? VehiclePlateColorCode { get; set; }
        [Column("FK_VehiclePlateTypeID")] public byte VehiclePlateTypeID { get; set; }
        [Column("VehiclePlateType")] public string? VehiclePlateTypeName { get; set; }
        [Column("FK_VehiclePlateSizeID")] public int VehiclePlateSizeID { get; set; }
        [Column("VehiclePlateSizeName")] public string? VehiclePlateSizeName { get; set; }
        [Column("VehiclePlateSizeCode")] public string? VehiclePlateSizeCode { get; set; }
        [Column("Front")] public decimal Front { get; set; }
        [Column("Rear")] public decimal Rear { get; set; }
        [Column("SnapLock")] public decimal SnapLock { get; set; }
        [Column("TLPSticker")] public decimal TLPSticker { get; set; }
        [Column("EmbossingFitmentCharges")] public decimal EmbossingFitmentCharges { get; set; }
        [Column("DealerFitmentCharges")] public decimal DealerFitmentCharges { get; set; }
        [Column("HomeFitmentCharges")] public decimal HomeFitmentCharges { get; set; }
        [Column("DealerCourierCharge")] public decimal DealerCourierCharge { get; set; }
        [Column("DealerLocationChangeCharge")] public decimal DealerLocationChangeCharge { get; set; }
        [Column("OtherCharges")] public decimal OtherCharges { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
    }
    [Table("LkupVehicleCategory")]
    public class VehicleCategory
    {
        [Column("PK_VehicleCategoryID"), Key] public byte VehicleCategoryID { get; set; }
        [Column("VehicleCategoryName")] public string? VehicleCategoryName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
    }
    [Table("LkupVehiclePlateType")]
    public class VehiclePlateType
    {
        [Column("PK_VehiclePlateTypeID"), Key] public byte VehiclePlateTypeID { get; set; }
        [Column("VehiclePlateType")] public string? VehiclePlateTypeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
    }
    [Table("LkupVehicleType")]
    public class VehicleType
    {
        [Column("PK_VehicleTypeID"),Key] public byte VehicleTypeID { get; set; }
        [Column("VehicleTypeName")] public string? VehicleTypeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("LkupFuel")]
    public class Fuel
    {
        [Column("PK_FuelID"),Key] public byte FuelID { get; set; }
        [Column("FuelName")] public string? FuelName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
}