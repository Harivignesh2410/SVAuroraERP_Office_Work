namespace SVAuroraERP.Domain.Orders.ManageOrder
{
    [Table("V_CreateInvoiceData")]
    public class CreateInvoiceData
    {
        [Column("Dealer")] public string? Dealer { get; set; }
        [Column("DealerCode")] public string? DealerCode { get; set; }
        [Column("DealerCity")] public string? DealerCity { get; set; }
        [Column("DealerPONo")] public string? DealerPONo { get; set; }
        [Column("FK_DealerID")] public int? DealerID { get; set; }
        [Column("LastOrderDate")] public DateTime? LastOrderDate { get; set; }
        [Column("sLastOrderDate")] public string? sLastOrderDate { get; set; }
        [Column("TotalOrders")] public int TotalOrders { get; set; }
        [Column("FK_OrderTypeID")] public byte? OrderTypeID { get; set; }
    }

    public class GenerateInvoiceRequest
    {
        public int? DealerID { get; set; }
        public string? OrderID { get; set; }
        public int LastUpdatedBy { get; set; }
    }
}
