namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Production
{
    public interface IPendingApprovalFilterServiceRepository
    {
        List<VStockRequest> GetPendingApprovalByFilter(PendingApprovalFilter searchFilter);
        Tuple<bool, bool> ApproveorRejectStockRequest(ApprovalRequest request);
        List<VStockRequest> GetProductionInwardByFilter(PendingApprovalFilter searchFilter);
    }
}