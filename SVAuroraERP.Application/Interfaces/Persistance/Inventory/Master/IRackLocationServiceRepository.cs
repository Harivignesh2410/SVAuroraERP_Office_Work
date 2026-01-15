namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface IRackLocationServiceRepository
    {
        DataResponse GetRackLocation();
        DataResponse GetByID(int ID);
        DataResponse Save(RackLocation RackLocation);
        DataResponse Update(RackLocation RackLocation);
        DataResponse Delete(int RackLocationID, int UserID, long LoginAuditID);
        DataResponse SaveCapacity(List<RackLocationSizeCapacity> RackCapacity);
        DataResponse GetCapacityByLocaitonID(int ID);
        DataResponse GetRackLocationByComponentID(int ComponentID);
        DataResponse GetRackLocationByWareHouseID(int WareHouseID);
        DataResponse GetRackLocationDataTable(DataTableRequest request);
    }
}