namespace SVAuroraERP.Domain.Orders.ManageOrder
{
    [Table("V_CreateJobCard")]
    public class VCreateJobCard
    {
        [Column("PK_HSRPOrderID"), Key] public int HSRPOrderID { get; set; }
        [Column("FK_OrderTypeID")] public byte? OrderTypeID { get; set; }
        [Column("OrderTypeName")] public string? OrderTypeName { get; set; }
        [Column("OrderNo")] public string? OrderNo { get; set; }
        [Column("OrderDate")] public DateTime? OrderDate { get; set; }
        [Column("sOrderDate")] public string? sOrderDate { get; set; }
        [Column("ssOrderDate")] public string? ssOrderDate { get; set; }
        [Column("DealerPONo")] public string? DealerPONo { get; set; }
        [Column("DealerSONo")] public string? DealerSONo { get; set; }
        [Column("FK_DealerID")] public int? DealerID { get; set; }
        [Column("Dealer")] public string? Dealer { get; set; }
        [Column("DealerCode")] public string? DealerCode { get; set; }
        [Column("DealerCity")] public string? DealerCity { get; set; }
        [Column("FK_OEMID")] public int? OEMID { get; set; }
        [Column("OEM")] public string? OEM { get; set; }
        [Column("OEMCode")] public string? OEMCode { get; set; }
        [Column("OEMCity")] public string? OEMCity { get; set; }
        [Column("FK_EmbossingStationID")] public int? EmbossingStationID { get; set; }
        [Column("EmbossingStation")] public string? EmbossingStation { get; set; }
        [Column("EmbossingStationCode")] public string? EmbossingStationCode { get; set; }
        [Column("EmbossingStationCity")] public string? EmbossingStationCity { get; set; }
        [Column("FK_OrderStatusID")] public byte? OrderStatusID { get; set; }
        [Column("Description")] public string? Description { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("IconCode")] public string? IconCode { get; set; }
        [Column("ProcessDate")] public DateTime? ProcessDate { get; set; }
        [Column("sProcessDate")] public string? sProcessDate { get; set; }
        [Column("PK_HSRPVehicleInfoID")] public int? HSRPVehicleInfoID { get; set; }
        [Column("FK_HSRPOrderID")] public int? HSRPOrderRefID { get; set; }
        [Column("RegNo")] public string? RegNo { get; set; }
        [Column("RegDate")] public DateTime? RegDate { get; set; }
        [Column("sRegDate")] public string? sRegDate { get; set; }
        [Column("ssRegDate")] public string? ssRegDate { get; set; }
        [Column("ChasisNo")] public string? ChasisNo { get; set; }
        [Column("EngineNo")] public string? EngineNo { get; set; }
        [Column("PlateColor")] public string? PlateColor { get; set; }
        [Column("LastUpdatedBy")] public int? LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("RearLaserSerialNo")] public string? RearLaserSerialNo { get; set; }
        [Column("FrontLaserSerialNo")] public string? FrontLaserSerialNo { get; set; }
        [Column("FrontPlateSize")] public string? FrontPlateSize { get; set; }
        [Column("RearPlateSize")] public string? RearPlateSize { get; set; }
        [Column("PartNo")] public string? PartNo { get; set; }
        [Column("FrontPlateDimension")] public string? FrontPlateDimension { get; set; }
        [Column("RearPlateDimension")] public string? RearPlateDimension { get; set; }
        [Column("FK_USERID")] public int? UserID { get; set; }
        //[Column("FK_InvoiceID")] public int? InvoiceID { get; set; }
        //[Column("InvoiceNo")] public string? InvoiceNo { get; set; }
        //[Column("InvoiceDate")] public DateOnly? InvoiceDate { get; set; }
        //[Column("sInvoiceDate")] public string? sInvoiceDate { get; set; }
        //[Column("InvoiceNetAmount")] public decimal? InvoiceNetAmount { get; set; }
    }
    public class CreateJobCardRequest : DataTableRequest
    {

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? sStartDate { get; set; }
        public string? sEndDate { get; set; }
        public byte? orderTypeID { get; set; }
        public int? DealerID { get; set; }
        public int? OEMID { get; set; }
        public int? EmbossingStationID { get; set; }
        public string? SearchText { get; set; }
    }
    [Table("tHSRPJobCard")]
    public class HSRPJobCard
    {
        [Column("PK_HSRPJobCardID"), Key] public int HSRPJobCardID { get; set; }
        [Column("JobCardNo")] public string? JobCardNo { get; set; }
        [Column("JobCardDate")] public DateTime JobCardDate { get; set; }
        [Column("FK_EmbossingStationID")] public int EmbossingStationID { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    [Table("tHSRPJobCardTrans")]
    public class HSRPJobCardTrans
    {
        [Key, Column("PK_HSRPJobCardTransID")] public int HSRPJobCardTransID { get; set; }
        [Column("FK_HSRPJobCardID")] public int HSRPJobCardID { get; set; }
        [Column("FK_OrderID")] public int OrderID { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    public class HSRPJobCardRequest
    {
        public string? OrderIds { get; set; }
        public int EmbossingID { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
    public class QualityProcessRequest
    {
        public int OrderID { get; set; }
        public int FrontLaserNoID { get; set; }
        public int RearLaserNoID { get; set; }
        public string? VerifiedFrontVehicleNo { get; set; }
        public string? VerifiedFrontLaserNo { get; set; }
        public string? FrontVehicleNoImageUrl { get; set; }
        public string? VerifiedRearVehicleNo { get; set; }
        public string? VerifiedRearLaserNo { get; set; }
        public string? RearVehicleNoImageUrl { get; set; }
        public int LastUpdatedBy { get; set; }
    }
    public class QCCompletedProcessRequest
    {
        public string? OrderIds { get; set; }
        public int LastUpdatedBy { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
    [Table("V_HSRPJobCard")]
    public class VHSRPJobCard
    {
        [Column("PK_HSRPJobCardID"), Key] public int HSRPJobCardID { get; set; }
        [Column("JobCardNo")] public string? JobCardNo { get; set; }
        [Column("JobCardDate")] public DateTime JobCardDate { get; set; }
        [Column("sJobCardDate")] public string? sJobCardDate { get; set; }
        [Column("FK_EmbossingStationID")] public int EmbossingStationID { get; set; }
        [Column("EmbossingName")] public string? EmbossingName { get; set; }
        [Column("EmbossingCity")] public string? EmbossingCity { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("Dealer")] public string? DealerName { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [NotMapped] public List<VHSRPJobCardTrans>? VHSRPJobCardTrans { get; set; }

        //Added on DK
        [Column("EmbossingCode")] public string? EmbossingCode { get; set; }
        [Column("JobCardTransCount")] public int? JobCardTransCount { get; set; }

    }
    [Table("V_HSRPJobCardTrans")]
    public class VHSRPJobCardTrans
    {
        [Column("PK_HSRPJobCardTransID"), Key] public int HSRPJobCardTransID { get; set; }
        [Column("FK_HSRPJobCardID")] public int HSRPJobCardID { get; set; }
        [Column("FK_OrderID")] public int OrderID { get; set; }
        [Column("FK_OrderTypeID")] public byte OrderTypeID { get; set; }
        [Column("OrderTypeName")] public string? OrderTypeName { get; set; }
        [Column("OrderNo")] public string? OrderNo { get; set; }
        [Column("OrderDate")] public DateTime OrderDate { get; set; }
        [Column("sOrderDate")] public string? sOrderDate { get; set; }
        [Column("DealerPONo")] public string? DealerPONo { get; set; }
        [Column("DealerSONo")] public string? DealerSONo { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("Dealer")] public string? Dealer { get; set; }
        [Column("DealerCode")] public string? DealerCode { get; set; }
        [Column("DealerCity")] public string? DealerCity { get; set; }
        [Column("FK_OEMID")] public int OEMID { get; set; }
        [Column("OEM")] public string? OEM { get; set; }
        [Column("OEMCode")] public string? OEMCode { get; set; }
        [Column("OEMCity")] public string? OEMCity { get; set; }
        [Column("FK_EmbossingStationID")] public int EmbossingStationID { get; set; }
        [Column("EmbossingStation")] public string? EmbossingStation { get; set; }
        [Column("EmbossingStationCode")] public string? EmbossingStationCode { get; set; }
        [Column("EmbossingStationCity")] public string? EmbossingStationCity { get; set; }
        [Column("FK_OrderStatusID")] public byte OrderStatusID { get; set; }
        [Column("ColorCode")] public string? ColorCode { get; set; }
        [Column("IconCode")] public string? IconCode { get; set; }
        [Column("Description")] public string? Description { get; set; }
        [Column("ProcessDate")] public DateTime? ProcessDate { get; set; }
        [Column("sProcessDate")] public string? sProcessDate { get; set; }
        [Column("PK_HSRPVehicleInfoID")] public int VehicleInfoID { get; set; }
        [Column("FK_HSRPOrderID")] public int HSRPOrderID { get; set; }
        [Column("RegNo")] public string? RegNo { get; set; }
        [Column("RegDate")] public DateTime? RegDate { get; set; }
        [Column("sRegDate")] public string? sRegDate { get; set; }
        [Column("ChasisNo")] public string? ChasisNo { get; set; }
        [Column("EngineNo")] public string? EngineNo { get; set; }
        [Column("PlateColor")] public string? PlateColor { get; set; }
        [Column("PartNo")] public string? PartNo { get; set; }
        [Column("FK_FrontLaserNoID")] public int FrontLaserNoID { get; set; }
        [Column("FrontLaserSerialNo")] public string? FrontLaserSerialNo { get; set; }
        [Column("FK_RearLaserNoID")] public int RearLaserNoID { get; set; }
        [Column("RearLaserSerialNo")] public string? RearLaserSerialNo { get; set; }
        [Column("SnapLockCount")] public byte SnapLockCount { get; set; }
        [Column("RivetsCount")] public byte RivetsCount { get; set; }
        [Column("Rate")] public decimal Rate { get; set; }
        [Column("CourierCharges")] public decimal CourierCharges { get; set; }
        [Column("GrossTotal")] public decimal GrossTotal { get; set; }
        [Column("TaxAmount")] public decimal TaxAmount { get; set; }
        [Column("NetAmount")] public decimal NetAmount { get; set; }
        [Column("FittedDate")] public DateTime? FittedDate { get; set; }
        [Column("sFittedDate")] public string? sFittedDate { get; set; }
        [Column("ReuploadDate")] public DateTime? ReuploadDate { get; set; }
        [Column("sReuploadDate")] public string? sReuploadDate { get; set; }
        [Column("ReuploadCount")] public byte ReuploadCount { get; set; }
        [Column("LastUpdatedDateIST")] public DateTime LastUpdatedDateIST { get; set; }
        [Column("RearPlateSize")] public string? RearPlateSize { get; set; }
        [Column("FrontPlateSize")] public string? FrontPlateSize { get; set; }

    }

    [Table("tLaserNoPlate")]
    public class LaserNoPlate
    {
        [Column("PK_LaserNoPlateID"), Key] public int LaserNoPlateID { get; set; }
        [Column("LaserSerialNo")] public string? LaserSerialNo { get; set; }
        [Column("FK_StatusID")] public byte? StatusID { get; set; }
        [Column("LastUpdateDate")] public DateTime? LastUpdateDate { get; set; }
    }
    public class CreateJobRequest : DataTableRequest
    {

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? sStartDate { get; set; }
        public string? sEndDate { get; set; }
        public int? DealerID { get; set; }
        public int? EmbossingStationID { get; set; }
        public string? SearchText { get; set; }
    }



    public class QualityProcessingRequest : DataTableRequest
    {

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? sStartDate { get; set; }
        public string? sEndDate { get; set; }
        public byte? orderTypeID { get; set; }
        public int? DealerID { get; set; }
        public int? OEMID { get; set; }
        public int? EmbossingStationID { get; set; }
        public string? SearchText { get; set; }
    }
}