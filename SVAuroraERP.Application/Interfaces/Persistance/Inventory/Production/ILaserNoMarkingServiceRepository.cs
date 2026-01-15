namespace SVAuroraERP.Application.Interfaces.Persistance.Production
{
    public interface ILaserNoMarkingServiceRepository
    {
        UpdateResult SaveLaserNoMarking(LaserNoMarking request);
        Tuple<bool, bool> SaveLaserNoConsumption(LaserNoConsumption request);
        List<VWareHouse> GetWarehouseList();
        List<VHologramPunchingCompleted> GetHologramPunchingByWarehouseID(int id, int ComponentTypeID);
        LaserDataResponse GetLaserNoAsync(int backstockid);
        UpdateResult UpdateLaserNoMarking(LaserNoMarking request);
        Tuple<bool, bool> DeleteLaserNoMarking(int LaserNoMarkingID);
        Tuple<bool, bool> CompleteLaserNoMarking(int BatchStockID);
        List<VLaserNoMarking> GetLaserNoMarkingCompleted();
        int GetLaserNoMarkingNxtNo();
    }
}
