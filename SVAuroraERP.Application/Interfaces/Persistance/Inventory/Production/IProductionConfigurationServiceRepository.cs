namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Production
{
    public interface IProductionConfigurationServiceRepository
    {
        public List<VProductionConfiguration> GetProductionConfigurationList();
        public VProductionConfiguration GetProductionConfigurationByID(int ProductionConfigurationID);
        public Tuple<bool, bool> Save(ProductionConfiguration request);
        public Tuple<bool, bool> Update(ProductionConfiguration request);
        public Tuple<bool, bool> Delete(int ProductionConfigurationID, int UserID);
        public List<VProductionConfiguration> GetProductionConfigurationByProcessTypeID(int ProcessTypeID);
    }
}