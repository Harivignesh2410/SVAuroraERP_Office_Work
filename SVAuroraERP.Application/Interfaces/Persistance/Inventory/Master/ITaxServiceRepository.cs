namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface ITaxServiceRepository
    {
        DataResponse GetTax();
        DataResponse GetByID(int ID);
        DataResponse Save(Tax Tax);
        DataResponse Update(Tax Tax);
        DataResponse Delete(int TaxID, int UserID, long LoginAuditID);
        DataResponse GetTaxDataTable(DataTableRequest request);
    }
}