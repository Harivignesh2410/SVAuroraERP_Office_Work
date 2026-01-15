// Added on 2025.01.21 by Harivignesh (US 44)
namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.MaterialInspection
{
    public interface IPendingInspectionServiceRepository
    {
        DataResponse GetMaterialInwardList();
        DataResponse GetMaterialInwardListByID(int MaterialID);
        Tuple<DataResponse> SaveMaterialInward(List<PendingInspection> request);
        string GenerateNextBatchNumber();
        List<VPendingInwardInspection> GetPendingInspectionByFilter(SearchPendingInwardFilter searchFilter);
        List<VPendingInwardInspection> GetCompletedPurchaseEntryByFilter(SearchPendingPurchase searchFilter);
        Tuple<bool, bool> Delete(int PendingInwardInspectionID, int UserID);
        List<BatchStock> GetCompletedBatchStock(FilterForBatchStock FilterForBatchStock);
        List<BatchStock> GetComponenetStock(FilterForBatchStock FilterForBatchStock);
        DataResponse GetComponenetListdropdown();
        DataResponse GetNumberPlateStockReport(NumberPlateStockReportFilter request);

        List<BatchStock> GetRawMaterialStockData(FilterRawMaterialData FilterForBatchStock);


    }
}