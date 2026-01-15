//Added on 2025.10.13 by Harivignesh
namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface IHSRPLaserNoStockServiceRepository
    {
        DataResponse GetLaserNoStockDataTable(HSRPLaserNoStockFilterData request);
        DataResponse GetHSRPLaserNoStatus();
        DataResponse GetLaserStockSummary(HSRPLaserNoStockFilterData request);
        DataResponse GetHSRPLaserNoStockLogByID(int HSRPLaserNoStockID);
    }
}