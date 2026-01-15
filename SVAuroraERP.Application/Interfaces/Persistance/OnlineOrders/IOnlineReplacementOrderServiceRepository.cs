using SVAuroraERP.Domain.OnlineOrders;

namespace SVAuroraERP.Application.Interfaces.Persistance.OnlineOrders
{
    public interface IOnlineReplacementOrderServiceRepository
    {
        DataResponse GetReplacementOrderList(ReplacementOrderDTRequest dataTableRequest);
        DataResponse GetReplacementOrderByID(int replacementOrderId);
        DataResponse ApproveReplacementOrder(ApproveReplacementOrderData request);
    }
}

