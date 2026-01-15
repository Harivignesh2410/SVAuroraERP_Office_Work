namespace SVAuroraERP.Application.Interfaces.Persistance.Production
{
    public interface IStockRequestServiceRepository
    {
        List<VStockRequest> GetStockRequest();
        VStockRequest GetByID(int ID);
        Tuple<bool, string?> Save(StockRequest StockRequest);
        Tuple<bool, bool> Update(StockRequest StockRequest);
        Tuple<bool, bool> Delete(int StockRequestID, int UserID);
        List<BatchStock> GetBatchStockByFilter(BatchStockFilter request);
    }
}