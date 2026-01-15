namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface ISizeServiceRepository
    {
        DataResponse GetSize();
        DataResponse GetByID(int ID);
        DataResponse Save(Size Size);
        DataResponse Update(Size Size);
        DataResponse Delete(int SizeID, int UserID, long LoginAuditID);
        DataResponse GetSizeDataTable(DataTableRequest request);
    }
}