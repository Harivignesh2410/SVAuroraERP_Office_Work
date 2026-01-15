namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Disptach
{
    public interface IMapPlateSizeServiceRepository
    {
        DataResponse GetMapPlateSize();
        DataResponse GetByID(int ID);
        DataResponse Save(MapPlateSize Size);
        DataResponse Update(MapPlateSize Size);
        DataResponse Delete(int ID, int UserID);
        DataResponse GetMapPlateDataTable(DataTableRequest request);
    }
}
