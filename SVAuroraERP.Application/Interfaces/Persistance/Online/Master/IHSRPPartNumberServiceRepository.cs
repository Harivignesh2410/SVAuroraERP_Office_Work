namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IHSRPPartNumberServiceRepository
    {
        DataResponse GetHSRPPartNumber();
        DataResponse GetHSRPPartNumberByID(int ID);
        DataResponse Save(HSRPPartNumber HSRPPartNumber);
        DataResponse Update(HSRPPartNumber HSRPPartNumber);
        DataResponse Delete(int HSRPPartNumberID, int UserID, long LoginAuditID);
        DataResponse GetHSRPPartNumberByOEMID(int OEMId);
        DataResponse GetHSRPPartNumberDataTableList(DataTableRequest request);
    }
}