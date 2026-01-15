namespace SVAuroraERP.Domain.Orders.Import
{
    [Table("tOEMImportTrans")]
    public class ImportOEMTrans
    {
        [Column("PK_OEMImportTransID"), Key] public int OEMImportTransID { get; set; }
        [Column("FK_OEMImportID")] public int OEMImportID { get; set; }
        [Column("FK_ImportStatusID")] public int ImportStatusID { get; set; }
        [Column("DealerCode")] public string? DealerCode { get; set; }
        [Column("PoNo")] public string? PoNo { get; set; }
        [Column("SoNo")] public string? SoNo { get; set; }
        [Column("VehicleRegistrationDate")] public string? VehicleRegistrationDate { get; set; }
        [Column("PartNo")] public string? PartNo { get; set; }
        [Column("VehicleRegistrationNo")] public string? VehicleRegistrationNo { get; set; }
        [Column("PlateColor")] public string? PlateColor { get; set; }
        [Column("OrderDate")] public string? OrderDate { get; set; }
        [Column("ChassisNo")] public string? ChassisNo { get; set; }
        [Column("EngineNo")] public string? EngineNo { get; set; }
        [Column("VendorCode")] public string? VendorCode { get; set; }
        [Column("FK_OrderID")] public int OrderID { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    [Table("V_OEMImportTrans")]
    public class VOEMImportTrans
    {
        [Column("PK_OEMImportTransID"), Key] public int OEMImportTransID { get; set; }
        [Column("FK_OEMImportID")] public int OEMImportID { get; set; }
        [Column("FK_ImportStatusID")] public byte ImportStatusID { get; set; }
        [Column("ImportStatus")] public string? ImportStatus { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("DealerCode")] public string? DealerCode { get; set; }
        [Column("PoNo")] public string? PoNo { get; set; }
        [Column("SoNo")] public string? SoNo { get; set; }
        [Column("VehicleRegistrationDate")] public string? VehicleRegistrationDate { get; set; }
        [Column("PartNo")] public string? PartNo { get; set; }
        [Column("VehicleRegistrationNo")] public string? VehicleRegistrationNo { get; set; }
        [Column("PlateColor")] public string? PlateColor { get; set; }
        [Column("OrderDate")] public string? OrderDate { get; set; }
        [Column("ChassisNo")] public string? ChassisNo { get; set; }
        [Column("EngineNo")] public string? EngineNo { get; set; }
        [Column("VendorCode")] public string? VendorCode { get; set; }
        [Column("FK_OrderID")] public int OrderID { get; set; }
        [Column("OrderNo")] public string? OrderNo { get; set; }
        [Column("DealerName")] public string? DealerName { get; set; }
    }
}
