namespace SVAuroraERP.Domain.Orders.Invoice
{
    [Table("V_HSRPInvoice")]
    public class VHSRPInvoice
    {
        [Column("PK_InvoiceID")] public int InvoiceID { get; set; }
        [Column("InvoiceNo")] public string? InvoiceNo { get; set; }
        [Column("InvoiceDate")] public DateTime InvoiceDate { get; set; }
        [Column("OrderCount")] public int OrderCount { get; set; }
        [Column("sInvoiceDate")] public string? sInvoiceDate { get; set; }
        [Column("FK_DealerID")] public int DealerID { get; set; }
        [Column("FK_OEMID")] public int OEMID { get; set; }
        [Column("Dealer")] public string? Dealer { get; set; }
        [Column("DealerCode")] public string? DealerCode { get; set; }
        [Column("Address1")] public string? Address1 { get; set; }
        [Column("Address2")] public string? Address2 { get; set; }
        [Column("City")] public string? City { get; set; }
        [Column("Pincode")] public string? Pincode { get; set; }
        [Column("DistrictName")] public string? DistrictName { get; set; }
        [Column("StateName")] public string? StateName { get; set; }
        [Column("DeliveryAddress1")] public string? DeliveryAddress1 { get; set; }
        [Column("DeliveryAddress2")] public string? DeliveryAddress2 { get; set; }
        [Column("DeliveryCity")] public string? DeliveryCity { get; set; }
        [Column("DeliveryPincode")] public string? DeliveryPincode { get; set; }
        [Column("DeliveryDistrictName")] public string? DeliveryDistrictName { get; set; }
        [Column("DeliveryStateName")] public string? DeliveryStateName { get; set; }
        [Column("LastUpdatedBy")] public int? LastUpdatedBy { get; set; }
        [Column("LastUpdatedDate")] public DateTime? LastUpdatedDate { get; set; }
        [Column("LastUpdatedByName")] public string? LastUpdatedByName { get; set; }
        [Column("NetAmount")] public decimal NetAmount { get; set; }
        [Column("PK_HSRPUserID")] public int HSRPUserID { get; set; }
        [Column("FrontPlateDimension")] public string? FrontPlateDimension { get; set; }
        [Column("RearPlateDimension")] public string? RearPlateDimension { get; set; }
        [Column("RegDate")] public DateTime RegDate { get; set; }
        [Column("sRegDate")] public string? sRegDate { get; set; }
    }

    [Table("V_HSRPInvoiceTrans")]
    public class VHSRPInvoiceTrans
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
        [Column("FrontPlateDimension")] public string? FrontPlateDimension { get; set; }
        [Column("RearPlateDimension")] public string? RearPlateDimension { get; set; }

        //Added on 2025.11.08
        [Column("FK_OrderStatusID")] public byte OrderStatusID { get; set; }
    }
    public class InvoiceTransRequest : DataTableRequest
    {
        public int DealerID { get; set; }
        public int EmbossingStationID { get; set; }
        public string? DealerPONo { get; set; }
    }
    public class HSRPDispatchRequest : DataTableRequest
    {
        public int DealerID { get; set; }
    }
    public class HSRPInvoiceTransRequest : DataTableRequest
    {
        public int InvoiceID { get; set; }
    }
    public class HSRPInvoiceTransByDealerRequest : DataTableRequest
    {
        public int DealerID { get; set; }
    }
    [Table("V_ExportInvoiceList")]
    public class VExportInvoiceList
    {
        [Key][Column("PK_InvoiceTransID")] public int InvoiceTransID { get; set; }
        [Column("FK_InvoiceID")] public int InvoiceID { get; set; }
        [Column("FK_OrderID")] public int OrderID { get; set; }
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
        [Column("sInvoiceDate")] public string? sInvoiceDate { get; set; }
        [Column("InvoiceNo")] public string? InvoiceNo { get; set; }
        [Column("InvoiceDate")] public DateTime? InvoiceDate { get; set; }
    }
    public class ExportInvoiceRequest : DataTableRequest
    {

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? sStartDate { get; set; }
        public string? sEndDate { get; set; }
        public int? DealerID { get; set; }
        public int? OEMID { get; set; }
        public int? EmbossingStationID { get; set; }
    }
    public class ListInvoiceRequest : DataTableRequest
    {

        public int? HsrpUserID { get; set; }
        public DateTime? StartDate { get; set; }
        public string? sStartDate { get; set; }
        public DateTime? EndDate { get; set; }       
        public string? sEndDate { get; set; }
        public int? DealerID { get; set; }
        public int? OEMID { get; set; }

    }
}
