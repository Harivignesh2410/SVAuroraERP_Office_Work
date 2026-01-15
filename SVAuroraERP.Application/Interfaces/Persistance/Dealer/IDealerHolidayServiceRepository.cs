using SVAuroraERP.Domain.Dealer;

namespace SVAuroraERP.Application.Interfaces.Persistance.Dealer
{
    public interface IDealerHolidayServiceRepository
    {
        DataResponse GetDealerHoliday();
        DataResponse GetDealerHolidayToDataTable(DealerHolidayDataTableRequest request);
        DataResponse GetDealerHolidayByID(int ID);
        DataResponse Save(DealerHoliday DealerHoliday, List<DealerHolidayType> DealerHolidayTypes);
        DataResponse Update(DealerHoliday DealerHoliday, List<DealerHolidayType> DealerHolidayTypes);
        DataResponse Delete(int DealerHolidayID, int UserID, long LoginAuditID);
    }
}

