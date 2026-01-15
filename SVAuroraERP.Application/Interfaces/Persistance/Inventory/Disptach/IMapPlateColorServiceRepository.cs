namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Disptach
{
    public interface IMapPlateColorServiceRepository
    {
        DataResponse GetMapPlateColor();
        DataResponse GetByID(int ID);
        DataResponse Save(MapPlateColor Color);
        DataResponse Update(MapPlateColor Color);
        DataResponse Delete(int ID, int UserID);
        DataResponse GetMapPlateDataTable(DataTableRequest request);
    }
}
