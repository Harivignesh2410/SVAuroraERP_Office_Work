namespace SVAuroraERP.Domain.Inventory.Purchase
{
    public class SearchPendingPurchase
    {
        public int SupplierID { get; set; }
        public int ComponentTypeID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? sStartDate { get; set; }
        public string? sEndDate { get; set; }
        public string? SearchInWord { get; set; }
    }
}