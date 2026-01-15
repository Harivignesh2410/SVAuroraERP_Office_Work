namespace SVAuroraERP.Domain.Orders.OrdersDelivery
{
    [Table("V_HSRPInvoiceByDealer")]
    public class VHSRPInvoiceByDealer
    {
        [Column("FK_DealerID")] public int DealerID { get; set; }
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
        [Column("TotalOrders")] public int TotalOrders { get; set; }
        [Column("ContactNo")] public string? ContactNo { get; set; }
        [Column("DealerPONo")] public string? DealerPONo { get; set; }
    }

}
