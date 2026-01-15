namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface IQCCompletedServiceRepository
    {
        DataResponse GetQCCompleted(QCCompletedJobRequest request);
        DataResponse Save(QCCompletedProcessRequest request);
    }
}