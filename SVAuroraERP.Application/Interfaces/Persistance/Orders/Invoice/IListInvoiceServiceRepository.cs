namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.Invoice
{
    public interface IListInvoiceServiceRepository
    {
        DataResponse GetListInvoice(ListInvoiceRequest dataTableRequest);
        DataResponse GetListInvoiceTrans(HSRPInvoiceTransRequest request);
        DataResponse GetExportInvoiceList(ExportInvoiceRequest request);
        DataResponse GetExportInvoiceExcel(ExportInvoiceRequest request);
    }
}