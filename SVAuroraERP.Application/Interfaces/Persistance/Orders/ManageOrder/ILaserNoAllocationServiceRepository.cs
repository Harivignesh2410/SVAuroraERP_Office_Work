namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface ILaserNoAllocationServiceRepository
    {
        DataResponse GetLaserNoAllocation(ReadyforProcessingOrdersRequest request);
        DataResponse Save(HSRPlaserStockRequest request);
        DataResponse CheckAvailableOrderLaserNo(CheckAvailableOrderLaserNoRequest request);
    }
}