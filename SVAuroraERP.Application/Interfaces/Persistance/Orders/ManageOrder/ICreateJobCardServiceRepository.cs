using SVAuroraERP.Domain.Orders.ManageOrder;

namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface ICreateJobCardServiceRepository
    {
        DataResponse GetCreateJobCard(CreateJobCardRequest request);
        DataResponse Save(HSRPJobCardRequest request);
        DataResponse GetHsrpJobcard(CreateJobRequest request);
        DataResponse GetJobcardByID(int ID);
        DataResponse GetLasserNo();        
    }
}