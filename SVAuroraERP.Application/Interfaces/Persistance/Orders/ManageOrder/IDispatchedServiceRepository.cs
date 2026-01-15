namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface IDispatchedServiceRepository
    {
        DataResponse GetDispatched(DispatchedOrdersRequest request);
    }
}
