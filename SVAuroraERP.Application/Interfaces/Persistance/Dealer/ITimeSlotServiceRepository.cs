using SVAuroraERP.Domain.Dealer;

namespace SVAuroraERP.Application.Interfaces.Persistance.Dealer
{
    public interface ITimeSlotServiceRepository
    {
        DataResponse GetTimeSlot();
        DataResponse GetTimeSlotToDataTable(DataTableRequest request);
        DataResponse GetTimeSlotByID(int ID);
        DataResponse Save(TimeSlot TimeSlot);
        DataResponse Update(TimeSlot TimeSlot);
        DataResponse Delete(int TimeSlotID, int UserID, long LoginAuditID);
    }
}

