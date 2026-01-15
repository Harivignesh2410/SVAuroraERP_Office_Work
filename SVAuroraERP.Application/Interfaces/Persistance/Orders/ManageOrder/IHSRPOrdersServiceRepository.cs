namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface IHSRPOrdersServiceRepository
    {
        DataResponse GetHsrporder(HsrpOrderRequest request);
        DataResponse GetOrderType();
        DataResponse SummaryOrdersByStatusID(SummaryFilterData filterData);
        DataResponse GetHsrporderByID(int HsrporderID);
        DataResponse GetHsrporderForExport(HsrpOrderRequest request);
        DataResponse GetOrderStatusTimeline(int orderId);
        DataResponse GetInvoiceDetails(int orderId);
        DataResponse GetShipmentAndDeliveryDetails(int orderId);
    }
}