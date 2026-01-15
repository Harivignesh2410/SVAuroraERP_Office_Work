namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface IQualityProcessingServiceRepository
    {
        DataResponse GetQualityProcessing(QualityProcessingRequest request);
        DataResponse Save(QualityProcessRequest request);
        DataResponse Reject(int LaserNoPlateID, int LastUpdatedBy);
    }
}