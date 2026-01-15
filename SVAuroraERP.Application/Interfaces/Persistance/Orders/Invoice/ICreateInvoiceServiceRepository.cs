namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.Invoice
{
    public interface ICreateInvoiceServiceRepository
    {
        DataResponse SummaryForQCCompleted();        
        DataResponse GenerateInvoice(GenerateInvoiceRequest request);
        DataResponse GetListInvoiceTrans(InvoiceTransRequest request);
    }
}