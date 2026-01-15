namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IDistrictServiceRepository
    {
        DataResponse GetDistrict();
        DataResponse GetDistrictByID(int ID);
        DataResponse Save(District District);
        DataResponse Update(District District);
        DataResponse Delete(int DistrictID, int UserID, long LoginAuditID);
        DataResponse GetDistrictList(DataTableRequest request);
    }
}