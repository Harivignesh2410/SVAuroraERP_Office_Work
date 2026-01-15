using SVAuroraERP.Domain.Dealer;

namespace SVAuroraERP.Application.Interfaces.Persistance.Dealer
{
    public interface IDealerWorkingDayServiceRepository
    {
        DataResponse GetDealerWorkingDay();
        DataResponse GetDealerWorkingDayToDataTable(DealerWorkingDayDataTableRequest request);
        DataResponse GetDealerWorkingDayByDealerID(int DealerID);
        DataResponse SaveOrUpdate(int DealerID, List<DealerWorkingDay> DealerWorkingDays);
        DataResponse Delete(int WorkingDayID, int UserID, long LoginAuditID);
        DataResponse DeleteByDealerID(int DealerID, int UserID, long LoginAuditID);
    }
}

