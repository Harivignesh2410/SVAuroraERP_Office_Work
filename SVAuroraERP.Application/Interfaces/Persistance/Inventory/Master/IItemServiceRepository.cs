namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface IItemServiceRepository
    {
        DataResponse GetItem();
        DataResponse GetByID(int ID);
        DataResponse Save(Item Item);
        DataResponse Update(Item Item);
        DataResponse Delete(int ItemID, int UserID, long LoginAuditID);
        DataResponse GetItemDataTable(DataTableRequest request);

        //Added on 2025.01.05 by Sivakumar
        DataResponse GetItemCategory();
        DataResponse GetItemByFilter(BatchStockFilter request);
    }
}