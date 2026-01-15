namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IHSRPPlateDimensionServiceRepository
    {
        DataResponse GetHSRPPlateDimension();
        DataResponse GetHSRPPlateDimensionByID(int ID);
        DataResponse Save(HSRPPlateDimension HSRPPlateDimension);
        DataResponse Update(HSRPPlateDimension HSRPPlateDimension);
        DataResponse Delete(int HSRPPlateDimensionID, int UserID, long LoginAuditID);
        DataResponse GetHSRPPlateDimensionDataTableList(DataTableRequest request);
    }
}