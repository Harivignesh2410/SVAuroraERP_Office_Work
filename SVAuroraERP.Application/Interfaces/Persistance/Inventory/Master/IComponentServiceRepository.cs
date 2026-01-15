namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface IComponentServiceRepository
    {
        DataResponse GetComponentList();
        DataResponse GetComponentByID(int ComponentTypeID);
        DataResponse Save(ComponentType request);
        DataResponse Update(ComponentType request);
        DataResponse Delete(int ComponentTypeID, int UserID);
        DataResponse GetComponentDataTable(DataTableRequest request);
    }
}
