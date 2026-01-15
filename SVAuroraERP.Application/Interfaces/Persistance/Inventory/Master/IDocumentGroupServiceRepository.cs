namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface IDocumentGroupServiceRepository
    {
        DataResponse GetDocumentGroup();
        DataResponse GetByID(int ID);
        DataResponse Save(DocumentGroup DocumentGroup);
        DataResponse Update(DocumentGroup DocumentGroup);
        DataResponse Delete(int DocumentGroupID, int UserID, long LoginAuditID);
        DataResponse GetDocumentGroupDataTable(DataTableRequest request);
    }
}
