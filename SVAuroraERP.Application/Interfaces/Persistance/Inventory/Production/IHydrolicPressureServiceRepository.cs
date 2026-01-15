//Added on 2025.05.31  by Harivignesh
namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Production
{
    public interface IHydrolicPressureServiceRepository
    {
        List<VStockRequest> GetStockRequestList(int id);
        Tuple<bool, bool> SaveHydrolicPressure(HydrolicPressure request);
        Tuple<bool, bool> UpdateHydrolicPressure(HydrolicPressure request);
        FullHydraulicDataResult GetHydraulicDetails(int stockRequestID);
        HydraulicDataResponse GetHydraulicDetailsAsync(int stockRequestId);
        Tuple<bool, bool> DeleteHydrolicPressure(int HydrolicPressureID);
        Tuple<bool, bool> CompleteHydrolicPressure(int StockRequestID);
        HydrolicPressureBatchStock GetHydrolicPressureByID(int BatchStockID);
    }
}
