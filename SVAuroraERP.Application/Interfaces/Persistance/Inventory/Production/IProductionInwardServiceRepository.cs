namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Production
{
    public interface IProductionInwardServiceRepository
    {
        Tuple<bool, bool> SaveProductionInward(ProductionInward request);
        Tuple<bool, bool> UpdateProductionInward(ProductionInward request);
        Tuple<bool, bool> DeleteProductionInward(int ProductionInwardID);
    }
}
