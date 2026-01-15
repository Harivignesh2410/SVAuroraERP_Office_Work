namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Purchase
{
    public interface IPurchaseEntryTransServiceRepository
    {
        List<VPurchaseEntryTrans> GetPurchaseTrans();
        List<VPurchaseEntryTrans> GetPurchaseTransByID(int PurchaseEntryID);
        Tuple<bool, bool> SavePurchaseTransDetails(List<PurchaseEntryTrans> request);
    }
}
