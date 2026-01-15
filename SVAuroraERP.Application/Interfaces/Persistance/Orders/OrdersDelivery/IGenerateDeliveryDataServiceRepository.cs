namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.OrdersDelivery
{
    public interface IGenerateDeliveryDataServiceRepository
    {
        DataResponse GetListInvoice(DataTableRequest dataTableRequest);
        DataResponse GetListInvoiceTrans(HSRPInvoiceTransByDealerRequest request);
        DataResponse GetHSRPInvoiceTransByDealer(int dealerId);
        DataResponse SaveGenerateDeliveryData(GenerateDeliveryRequest request);
        DataResponse GetDispatchDetails(DataTableRequest dataTableRequest);
        DataResponse GetDispatchData(int GetDeliveryID);
        DataResponse GetListDispatchDataTrans(int GenerateDeliveryID);
        DataResponse AcknowledgeGenerateDeliveryData(AcknowlegdeGenerateDeliveryRequest request);
    }
}