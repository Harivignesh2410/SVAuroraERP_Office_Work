//Added on 2025/05/26 by Harivignesh
namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Production
{
    public interface IProductionCalculationServiceRepository
    {
        DataResponse GetProductionCalculation();
        DataResponse GetByID(int ID);
        DataResponse Save(ProductionCalculation ProductionCalculation);
        DataResponse Update(ProductionCalculation ProductionCalculation);
        DataResponse Delete(int ProductionCalculationID, int UserID, long LoginAuditID);
        DataResponse GetProductionCalculationtoDataTable(DataTableRequest request);
    }
}
