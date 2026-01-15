namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Production
{
    public interface IProcessTypeServiceRepository
    {
        List<VProcessType> GetProcessTypeList();
        VProcessType GetByID(int ID);
        Tuple<bool, bool> Update(ProcessType ProcessType);
    }
}