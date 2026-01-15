namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Purchase
{
    public interface IPurchaseOrderServiceRepository
    {
        DataResponse GetPurchaseOrder();
        DataResponse GetPurchaseOrderByID(int PurchaseOrderID);
        DataResponse Save(PurchaseOrder request);
        DataResponse Update(PurchaseOrder request);
        DataResponse Delete(int PurchaseOrderID, int UserID);
        DataResponse GetPurchaseOrderDataTable(DataTableRequest request);
    }
}