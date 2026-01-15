namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface IColorServiceRespository
    {
        DataResponse GetColor();
        DataResponse GetByID(int ID);
        DataResponse Save(Color Color);
        DataResponse Update(Color Color);
        DataResponse Delete(int ID, int UserID, long LoginAuditID);
        DataResponse GetColorDataTable(DataTableRequest request);
    }
}