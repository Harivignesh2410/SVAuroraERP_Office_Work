namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface ICancelledorDamagedPlateOrdersServiceRepository
    {
        DataResponse GetCancelledorDamagedPlateOrders(CancelledorDamagedPlateOrdersRequest request);
    }
}
