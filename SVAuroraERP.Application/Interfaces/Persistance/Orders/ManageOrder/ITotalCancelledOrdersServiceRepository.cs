namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface ITotalCancelledOrdersServiceRepository
    {
        DataResponse GetTotalCancelledOrders(TotalCancelledOrdersRequest request);
    }
}
