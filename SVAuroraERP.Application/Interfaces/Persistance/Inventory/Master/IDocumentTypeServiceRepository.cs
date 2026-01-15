namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface IDocumentTypeServiceRepository
    {
        DataResponse GetDocumentType();
        DataResponse GetByID(int ID);
        DataResponse Save(DocumentType DocumentType);
        DataResponse Update(DocumentType DocumentType);
        DataResponse Delete(int DocumentTypeID, int UserID, long LoginAuditID);
        DataResponse GetDocumentTypeDataTable(DataTableRequest request);
    }
}
