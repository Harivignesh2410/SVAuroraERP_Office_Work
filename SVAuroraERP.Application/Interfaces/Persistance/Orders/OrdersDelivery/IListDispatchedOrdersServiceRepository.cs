namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.OrdersDelivery
{
    public interface IListDispatchedOrdersServiceRepository
    {
        DataResponse GetListDispatchOrdersDetails(DataTableRequest dataTableRequest);
        DataResponse GetDispatchTransData(int ID);
    }
}
