using SVAuroraERP.Domain.OnlineOrders;

namespace SVAuroraERP.Application.Interfaces.Persistance.OnlineOrders
{
    public interface IOnlineHSRPOrderServiceRepository
    {
        DataResponse GetOnlineOrderList(OnlineOrderDTRequest dataTableRequest);
        DataResponse GetOnlineOrderByHSRPOrderID(int hsrpOrderId);
        DataResponse ApproveOnlineOrders(Approvedata request);
    }
}
