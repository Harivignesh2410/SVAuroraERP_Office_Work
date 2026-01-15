namespace SVAuroraERP.Domain.Orders.OrdersDelivery
{
    [Table("tGenerateDeliveryData")]
    public class GenerateDeliveryData
    {
        [Column("PK_GenerateDeliveryID"), Key] public int GenerateDeliveryID { get; set; }
        [Column("FK_DealerID")] public int? DealerID { get; set; }
        [Column("FK_ModeOfTransportID")] public int ModeOfTransportID { get; set; }
        [Column("FK_CourierID")] public int CourierID { get; set; }
        [Column("ConsignmentDetails")] public string? ConsignmentDetails { get; set; }
        [Column("CollectingPerson")] public string? CollectingPerson { get; set; }
        [Column("UploadImageUrl")] public string? UploadImageUrl { get; set; }
        [Column("ImageName")] public string? ImageName { get; set; }
        [Column("GenerateDate")] public DateOnly GenerateDate { get; set; }
        [Column("DeliveredDate")] public DateOnly DeliveredDate { get; set; }
        [Column("DocketNo")] public string? DocketNo { get; set; }
        [Column("IsDeleted")] public bool IsDeleted { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
    }
    public class GenerateDeliveryRequest
    {
        public int FK_DealerID { get; set; }
        public int? FK_ModeOfTransportID { get; set; }
        public int? FK_CourierID { get; set; }
        public string? ConsignmentDetails { get; set; }
        public string? CollectingPerson { get; set; }
        public string? UploadImageUrl { get; set; }
        public string? ImageName { get; set; }
        public DateOnly? DispatchDate { get; set; }
        public string? sDispatchDate { get; set; }
        public int LastUpdatedBy { get; set; }
        public string? OrderList { get; set; } // List of Order IDs
    }

    public class AcknowlegdeGenerateDeliveryRequest
    {
        public int GenerateDelieveryDataID { get; set; }

        public DateOnly? DeliveryDate { get; set; }
        public string? sDeliveryDate { get; set; }
        public string? OrderList { get; set; } // List of Order IDs
        public int LastUpdatedBy { get; set; }
    }
    [Table("V_GenerateDeliveryData")]
    public class VGenerateDeliveryData
    {
        [Column("PK_GenerateDeliveryID"), Key] public int GenerateDeliveryID { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("FK_ModeOfTransportID")] public int ModeOfTransportID { get; set; }
        [Column("ModeOfTransport")] public string? ModeOfTransport { get; set; }
        [Column("FK_CourierID")] public int CourierID { get; set; }
        [Column("ConsignmentDetails")] public string? ConsignmentDetails { get; set; }
        [Column("CollectingPerson")] public string? CollectingPerson { get; set; }
        [Column("UploadImageUrl")] public string? UploadImageUrl { get; set; }
        [Column("ImageName")] public string? ImageName { get; set; }
        [Column("GenerateDate")] public DateTime GenerateDate { get; set; }
        [Column("sGenerateDate")] public string? sGenerateDate { get; set; }
        [Column("CompanyName")] public string? CompanyName { get; set; }
        [Column("CourierName")] public string? CourierName { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("TotalOrders")] public int TotalOrders { get; set; }
        [Column("EmbossingStationName")] public string? EmbossingStationName { get; set; }
    }
    [Table("V_HSRPInvoiceForGenerateDelivery")]
    public class VHSRPInvoiceForGenerateDelivery
    {
        [Key][Column("PK_InvoiceTransID")] public int InvoiceTransID { get; set; }
        [Column("FK_InvoiceID")] public int InvoiceID { get; set; }
        [Column("FK_OrderID")] public int OrderID { get; set; }
        [Column("PK_HSRPOrderID")] public int HSRPOrderID { get; set; }
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
        [Column("ProcessDate")] public DateTime ProcessDate { get; set; }
        [Column("sProcessDate")] public string? sProcessDate { get; set; }
        [Column("PK_HSRPVehicleInfoID")] public int HSRPVehicleInfoID { get; set; }
        [Column("RegNo")] public string? RegNo { get; set; }
        [Column("RegDate")] public DateTime RegDate { get; set; }
        [Column("sRegDate")] public string? sRegDate { get; set; }
        [Column("ChasisNo")] public string? ChasisNo { get; set; }
        [Column("EngineNo")] public string? EngineNo { get; set; }
        [Column("PlateColor")] public string? PlateColor { get; set; }
        [Column("PartNo")] public string? PartNo { get; set; }
        [Column("FK_FrontLaserNoID")] public int FrontLaserNoID { get; set; }
        [Column("FrontLaserSerialNo")] public string? FrontLaserSerialNo { get; set; }
        [Column("FrontPlateSize")] public string? FrontPlateSize { get; set; }
        [Column("FK_RearLaserNoID")] public int RearLaserNoID { get; set; }
        [Column("RearLaserSerialNo")] public string? RearLaserSerialNo { get; set; }
        [Column("RearPlateSize")] public string? RearPlateSize { get; set; }
        [Column("Qty")] public decimal Qty { get; set; }
        [Column("Rate")] public decimal Rate { get; set; }
        [Column("GST")] public decimal GST { get; set; }
        [Column("Amount")] public decimal Amount { get; set; }
        [Column("LastUpdatedBy")] public int? LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
    }
    [Table("V_GenerateDeliveryDataForShipment")]
    public class VGenerateDeliveryDataForShipment
    {
        [Column("PK_GenerateDeliveryID"), Key] public int GenerateDeliveryID { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("FK_ModeOfTransportID")] public int ModeOfTransportID { get; set; }
        [Column("ModeOfTransport")] public string? ModeOfTransport { get; set; }
        [Column("FK_CourierID")] public int CourierID { get; set; }
        [Column("ConsignmentDetails")] public string? ConsignmentDetails { get; set; }
        [Column("CollectingPerson")] public string? CollectingPerson { get; set; }
        [Column("UploadImageUrl")] public string? UploadImageUrl { get; set; }
        [Column("ImageName")] public string? ImageName { get; set; }
        [Column("GenerateDate")] public DateTime GenerateDate { get; set; }
        [Column("sGenerateDate")] public string? sGenerateDate { get; set; }
        [Column("CompanyName")] public string? CompanyName { get; set; }
        [Column("CourierName")] public string? CourierName { get; set; }
        [Column("LastUpdatedBy")] public int LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("TotalOrders")] public int TotalOrders { get; set; }
        [Column("EmbossingStationName")] public string? EmbossingStationName { get; set; }
    }
}

