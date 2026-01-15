using System.Security.Cryptography;

namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IHSRPUserServiceRepository
    {
        DataResponse GetAdmin();
        DataResponse GetEmbossingStation();
        DataResponse GetOEM();
        DataResponse GetOEMSubDealer();
        DataResponse GetDealerByOEMID(int OEMID);
        DataResponse GetDealer();
        DataResponse GetSubDealer();
        DataResponse GetEmbossingStationSubUser();
        DataResponse GetHSRPUserByID(int ID);
        DataResponse Save(HSRPUser HSRPUser);
        DataResponse Update(HSRPUser HSRPUser);
        DataResponse Delete(int HSRPUserID, int UserID, long LoginAuditID);
        DataResponse GetApplication();
        DataResponse GetRoleIDByPageID(int PageID);
        DataResponse GetHSRPUserByUserID(int UserID);
        DataResponse GetDealerListByOEM(OEMDataTableRequest request);
        DataResponse GetDealerByOEMIDForFilter(int oemID);
        DataResponse GetOEMByEmbossingStation(int EmbossingStationID);
        DataResponse GetEmbossingStationByUser(int ID);
        DataResponse GetEmbossingStationByHSRPOnlineOrderID(int ID);
        DataResponse GetHSRPUserDataTableList(HSRPUserRequest request);
    }
}