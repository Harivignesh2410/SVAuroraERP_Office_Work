namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface IRejectedQualityProcessingServiceRepository 
    {
        DataResponse GetRejectedQualityProcessing(RejectedQualityProcessingRequest request);
    }
}
