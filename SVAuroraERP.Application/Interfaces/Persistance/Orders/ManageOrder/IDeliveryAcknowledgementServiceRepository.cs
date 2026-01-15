namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface IDeliveryAcknowledgementServiceRepository
    {
        DataResponse GetDeliveryAcknowledgementOrders(DeliveryAcknowledgementOrdersRequest request);
        DataResponse SaveHSRPPlateImage(HSRPVehiclePlateImage request);
    }
}