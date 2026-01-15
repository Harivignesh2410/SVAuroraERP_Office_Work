namespace SVAuroraERP.Domain.Orders.Import
{
    [Table("tOEMImport")]
    public class ImportOEM
    {
        [Column("PK_ImportOEMID"), Key] public int ImportOEMID { get; set; }
        [Column("FK_OEMID")] public int OEMID { get; set; }
        [Column("FileName")] public string? FileName { get; set; }
        [Column("DataRowCount")] public int DataRowCount { get; set; }
        [Column("InsertedCount")] public int InsertedCount { get; set; }
        [Column("RemovedCount")] public int RemovedCount { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }

    [Table("V_OEMImport")]
    public class VOEMImport
    {
        [Column("PK_ImportOEMID"), Key] public int ImportOEMID { get; set; }
        [Column("FK_OEMID")] public int OEMID { get; set; }
        [Column("CompanyName")] public string? CompanyName { get; set; }
        [Column("FileName")] public string? FileName { get; set; }
        [Column("DataRowCount")] public int DataRowCount { get; set; }
        [Column("InsertedCount")] public int InsertedCount { get; set; }
        [Column("RemovedCount")] public int RemovedCount { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
        [NotMapped] public List<VOEMImportTrans>? VOEMImportTrans { get; set; }
        [Column("ImportedDate")] public string? ImportedDate { get; set; }
    }

    [Table("tOEMConfig")]
    public class OEMConfig
    {
        [Column("TVSOEMID")] public int TVSOEMID { get; set; }
        [Column("SaravanaEngOEMID")] public int SaravanaEngOEMID { get; set; }
        [Column("EroyceMotorsOEMID")] public int EroyceMotorsOEMID { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_OEMImportValidate")]
    public class VOEMImportValidate
    {
        [Column("PK_HSRPPartNumberID"), Key] public int HSRPPartNumberID { get; set; }
        [Column("FK_OEMID")] public int OEMID { get; set; }
        [Column("CompanyName")] public string? CompanyName { get; set; }
        [Column("PartNumber")] public string? PartNumber { get; set; }
        [Column("FrontVehiclePlateSize")] public string? FrontVehiclePlateSize { get; set; }
        [Column("RearVehiclePlateSize")] public string? RearVehiclePlateSize { get; set; }
        [Column("Rivets")] public byte? Rivets { get; set; }
        [Column("SnapLock")] public byte? SnapLock { get; set; }
        [Column("Rate")] public byte? Rate { get; set; }
        [Column("CourierCharges")] public byte? CourierCharges { get; set; }
        [Column("TotalAmount")] public byte? TotalAmount { get; set; }
    }
    public class ImportOEMFilter : DataTableRequest
    {
        public int OEMID { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? sStartDate { get; set; }
        public string? sEndDate { get; set; }
    }

    public class ImportOEMDData
    {
        public int OEMID { get; set; }
        public string? FileName { get; set; }
        public int LastUpdatedBy { get; set; }
        public List<Exceldata> Exceldata { get; set; }
    }
    public class Exceldata
    {
        public string? VendorCode { get; set; }
        public string? DealerCode { get; set; }
        public string? PONumber { get; set; }
        public string? SONumber { get; set; }
        public string? VehRegDate { get; set; }
        public string? PartNo { get; set; }
        public string? VehRegNo { get; set; }
        public string? PlateColor { get; set; }
        public string? OrderDate { get; set; }
        public string? EngineNo { get; set; }
        public string? chassisNo { get; set; }
    }


}
