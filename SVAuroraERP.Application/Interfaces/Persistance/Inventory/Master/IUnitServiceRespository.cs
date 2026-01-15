namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface IUnitServiceRespository
    {
        DataResponse GetUnit();
        DataResponse GetByID(int ID);
        DataResponse Save(Unit Unit);
        DataResponse Update(Unit Unit);
        DataResponse Delete(int UnitID, int UserID, long LoginAuditID);
        DataResponse GetUnitDataTable(DataTableRequest request);
    }
}