namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Purchase
{
    public interface ISupplierServiceRepository
    {
        DataResponse GetSupplier();
        DataResponse GetByID(int ID);
        DataResponse Save(Supplier Size);
        DataResponse Update(Supplier Size);
        DataResponse Delete(int ID);
        DataResponse GetSupplierDataTable(DataTableRequest request);
    }
}
