namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IHomeFitmentPincodeServiceRepository
    {
        DataResponse GetHomeFitmentPincode();
        DataResponse GetHomeFitmentPincodeByID(int ID);
        DataResponse GetDistrictByStateID(int StateID);
        DataResponse Save(HomeFitmentPincode HomeFitmentPincode);
        DataResponse Update(HomeFitmentPincode HomeFitmentPincode);
        DataResponse Delete(int HomeFitmentPincodeID, int UserID, long LoginAuditID);
        DataResponse GetHomeFitmentPincodeDataTableList(DataTableRequest request);
    }
}