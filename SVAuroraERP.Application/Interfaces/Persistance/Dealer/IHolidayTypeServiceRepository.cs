using SVAuroraERP.Domain.Dealer;

namespace SVAuroraERP.Application.Interfaces.Persistance.Dealer
{
    public interface IHolidayTypeServiceRepository
    {
        DataResponse GetHolidayType();
        DataResponse GetHolidayTypeToDataTable(DataTableRequest request);
        DataResponse GetHolidayTypeByID(int ID);
        DataResponse Save(HolidayType HolidayType);
        DataResponse Update(HolidayType HolidayType);
        DataResponse Delete(int HolidayTypeID, int UserID, long LoginAuditID);
    }
}

