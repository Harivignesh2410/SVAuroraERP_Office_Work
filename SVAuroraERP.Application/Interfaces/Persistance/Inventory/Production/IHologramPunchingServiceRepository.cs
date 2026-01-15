namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Production
{
    public interface IHologramPunchingServiceRepository
    {
        Tuple<bool, bool,bool> SaveHologramPunching(HologramPunching request);
        List<VStockRequest> GetHologramPunchingList();
        HologramPunching GetHologramPunchingByID(int StockRequestID);
        List<VStockRequest> GetStockRequestList(int id);
        List<VWareHouse> GetWarehouseList();
        List<VHydrolicPressureCompleted> GetHologramPunchingByWarehouseID(int id, int ComponentTypeID);
        HologramDataResponse GetHologramDetailsAsync(int backstockid, int stockrequestid);
        FullHologramDataResult GetHologramDetails(int backstockid, int stockrequestid);
        Tuple<bool, bool> DeleteHologramPunching(int HologramPunchingID);
        Tuple<bool, bool,bool> UpdateHologramPunching(HologramPunching request);
        Tuple<bool, bool> CompleteHologramPunching(int StockRequestID);
        List<VHologramPunchingCompleted> GetHologramPunchingCompleted();
        VHologramPunchingCompleted GetHologramPunchingByBatchstockID(int BatchStockID);


    }
}
