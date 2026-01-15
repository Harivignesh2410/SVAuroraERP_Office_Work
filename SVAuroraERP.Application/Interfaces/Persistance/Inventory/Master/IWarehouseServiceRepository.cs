namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface IWareHouseServiceRepository
    {
        DataResponse GetWareHouse();
        DataResponse GetByID(int ID);
        DataResponse Save(WareHouse WareHouse);
        DataResponse Update(WareHouse WareHouse);
        DataResponse Delete(int WareHouseID, int UserID, long LoginAuditID);
        DataResponse GetWareHouseDataTable(DataTableRequest request);
    }
}