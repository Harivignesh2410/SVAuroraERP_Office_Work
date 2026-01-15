namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IHSRPReplacementDocumentServiceRepository
    {
        DataResponse GetHSRPReplacementDocument();
        DataResponse GetHSRPReplacementDocumentByID(int ID);
        DataResponse Save(HSRPReplacementDocument HSRPReplacementDocument);
        DataResponse Update(HSRPReplacementDocument HSRPReplacementDocument);
        DataResponse Delete(int HSRPReplacementDocumentID, int UserID);
        DataResponse GetHSRPReplacementDocumentDataTableList(DataTableRequest request);
    }
}
