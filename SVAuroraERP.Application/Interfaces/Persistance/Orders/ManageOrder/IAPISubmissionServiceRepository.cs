
namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface IAPISubmissionServiceRepository
    {
        DataResponse GetAPISubmissionData(APISubmissionRequest request);
    }
}
