namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface ICategoryServiceRespository
    {
        DataResponse GetCategory();
        DataResponse GetByID(int ID);
        DataResponse Save(Category Category);
        DataResponse Update(Category Category);
        DataResponse Delete(int ID, int UserID, long LoginAuditID);
        DataResponse GetCategoryDataTable(DataTableRequest request);
    }
}