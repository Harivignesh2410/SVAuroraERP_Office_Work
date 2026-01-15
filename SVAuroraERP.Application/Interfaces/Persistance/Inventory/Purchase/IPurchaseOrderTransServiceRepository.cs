namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Purchase
{
    public interface IPurchaseOrderTransServiceRepository
    {
        List<VPurchaseOrderTrans> GetPurchaseOrderTransList();
        List<VPurchaseOrderTrans>? GetPurchaseOrderTransListByID(int PurchaseOrderTransID);
        Tuple<bool, bool> SavePurchaseOrderTransDetails(List<PurchaseOrderTrans> request);
    }
}
