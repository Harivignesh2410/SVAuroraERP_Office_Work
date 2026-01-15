namespace SVAuroraERP.Domain.Orders.ManageOrder
{
    [Table("V_HSRPOrder")]
    public class VHSRPOrder
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
        [Column("FK_FrontLaserNoID")] public int? FrontLaserNoID { get; set; }
        [Column("RearLaserSerialNo")] public string? RearLaserSerialNo { get; set; }
        [Column("FK_RearLaserNoID")] public int? RearLaserNoID { get; set; }
        [Column("FrontLaserSerialNo")] public string? FrontLaserSerialNo { get; set; }
        [Column("FrontPlateSize")] public string? FrontPlateSize { get; set; }
        [Column("RearPlateSize")] public string? RearPlateSize { get; set; }
        [Column("PartNo")] public string? PartNo { get; set; }
        [Column("FrontPlateDimension")] public string? FrontPlateDimension { get; set; }
        [Column("RearPlateDimension")] public string? RearPlateDimension { get; set; }
    }
    [Table("LkupOrderType")]
    public class OrderType
    {
        [Column("PK_OrderTypeID"), Key] public byte OrderTypeID { get; set; }
        [Column("OrderTypeName")] public string? OrderTypeName { get; set; }
        [Column("IsActive")] public bool IsActive { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
    }
    public enum OrderStatus
    {
        AllOrders = 0,
        ReadyForProcessing = 1,
        LaserNoAssigned = 2,
        QualityProcessing = 4,
        QCCompleted = 5,
        InvoiceGenerated = 6,
        DispatchedOrders = 7,
        Delivered = 8,
        JobCardGenerated = 3,
        VahanAPISubmitted = 9,
        RejectedQualityProcessing = 10,
        FixationReUpload = 13,
        FixationReUploaded = 14,
        FittedOrders = 11,
        CancelledOrders = 12
    }
    public class HsrpOrderRequest : DataTableRequest
    {

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? sStartDate { get; set; }
        public string? sEndDate { get; set; }
        public byte? orderTypeID { get; set; }
        public int? DealerID { get; set; }
        public int? OEMID { get; set; }
        public int? UserID { get; set; }
        public int? EmbossingStationID { get; set; }
        public string? SearchText { get; set; }
    }
    [Table("V_HSRPOrderSummary")]
    public class VHSRPOrderSummary
    {
        [Column("Dealer")] public string? Dealer { get; set; }
        [Column("DealerCode")] public string? DealerCode { get; set; }
        [Column("DealerCity")] public string? DealerCity { get; set; }
        [Column("OrderCount")] public int? OrderCount { get; set; }
    }

    public class HSRPOrderDataExport
    {
        public string? OrderNo { get; set; }
        public string? sOrderDate { get; set; }
        public string? DealerPONo { get; set; }
        public string? DealerSONo { get; set; }
        public string? Dealer { get; set; }
        public string? DealerCode { get; set; }
        public string? DealerCity { get; set; }
        public string? OEM { get; set; }
        public string? OEMCode { get; set; }
        public string? OEMCity { get; set; }
        public string? EmbossingStation { get; set; }
        public string? EmbossingStationCode { get; set; }
        public string? EmbossingStationCity { get; set; }
        public string? sProcessDate { get; set; }
        public string? RegNo { get; set; }
        public string? sRegDate { get; set; }
        public string? EngineNo { get; set; }
        public string? ChasisNo { get; set; }
        public string? PlateColor { get; set; }
    }
    public class OrderStatusHistoryDto
    {
        public int HSRPOrderStatusLogID { get; set; }
        public int OrderID { get; set; }
        public int OrderStatusID { get; set; }
        public string? Description { get; set; }
        public string? IconCode { get; set; }
        public string? LastUpdatedBy { get; set; }
        public string? LastUpdateDate { get; set; }
        public int? CompletedStatusID { get; set; }
        public int? NextPendingStatusID { get; set; }
        public string? NextPendingDescription { get; set; }
        public string? NextPendingIconCode { get; set; }
    }
    public class OrderInvoiceDetailsDto
    {
        public int InvoiceTransID { get; set; }
        public int InvoiceID { get; set; }
        public int OrderID { get; set; }
        public string? sInvoiceDate { get; set; }
        public string? InvoiceNo { get; set; }
        public string? Amount { get; set; }
    }
    public class OrderShipmentAndDeliveryDetailsDto
    {
        public int GenerateDeliveryTransID { get; set; }
        public int GenerateDeliveryID { get; set; }
        public int OrderID { get; set; }
        public string? CourierName { get; set; }
        public string? ModeOfTransport { get; set; }
        public string? ConsignmentDetails { get; set; }
        public string? CollectingPerson { get; set; }
        public string? UploadImageUrl { get; set; }
        public string? ShipmentDate { get; set; }
        public string? sDeliveredDate { get; set; }
        public string? DocketNo { get; set; }
    }
    public class SummaryFilterData
    {
        public int UserID { get; set; }
        public byte? OrderStatusID { get; set; }
    }
}