namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Purchase
{
    public interface IPurchaseEntryServiceRepository
    {
        List<VPurchaseEntry> GetPurchaseEntry();
        VPurchaseEntry GetByID(int ID);
        Tuple<bool, bool, int> Save(PurchaseEntry PurchaseEntry);
        Tuple<bool, bool, int> Update(PurchaseEntry PurchaseEntry);
        Tuple<bool, bool> Delete(int ID);
        List<VPurchaseEntry> GetPendingPurchaseEntryByFilter(SearchPendingPurchase searchFilter);
        List<VPurchaseEntry> GetPurchaseEntryByFilter(SearchPurchaseEntryFilter searchFilter);
        List<VPurchaseEntry> GetCompletedPurchaseEntryByFilter(SearchPendingPurchase searchFilter);
        VPurchaseEntry GetMaterialInspectionByPurchaseEntryID(int PurchaseEntryID);
        DataResponse GetPurchaseEntryDataTable(DataTableRequest request);
    }
}