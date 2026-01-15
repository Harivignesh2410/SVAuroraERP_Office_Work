namespace SVAuroraERP.Domain.Orders.ManageOrder
{
    [Table("V_GetReadyforProcessingOrders")]
    public class VGetReadyforProcessingOrders
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
    }

    [Table("V_JobCardGenerated")]
    public class VJobCardGenerated
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
}