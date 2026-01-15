namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface ICourierServiceRepository
    {
        DataResponse GetCourier();
        DataResponse GetByID(int ID);
        DataResponse Save(Courier Courier);
        DataResponse Update(Courier Courier);
        DataResponse Delete(int CourierID, int UserID, long LoginAuditID);
        DataResponse GetCourierDataTable(DataTableRequest request);
    }
}
