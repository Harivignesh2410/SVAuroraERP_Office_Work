namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IHSRPReplacementReasonServiceRepository
    {
        DataResponse GetHSRPReplacementReason();
        DataResponse GetHSRPReplacementReasonByID(int ID);
        DataResponse Save(HSRPReplacementReason HSRPReplacementReason);
        DataResponse Update(HSRPReplacementReason HSRPReplacementReason);
        DataResponse Delete(int HSRPReplacementReasonID, int UserID);
        DataResponse GetHSRPReplacementReasonDataTableList(DataTableRequest request);
    }
}
