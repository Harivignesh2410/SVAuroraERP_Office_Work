namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Production
{
    public interface IStockRequestTransServiceRepository
    {
        List<VStockRequestTrans> GetStockRequestTransByID(int StockRequestID);
        Tuple<bool, bool> SaveStockRequestTrans(List<StockRequestTrans> request);
        public int AddStockRequestTrans(StockRequestTrans request);
        public int DeleteStockRequestTrans(int StockRequestTransID);
    }
}
