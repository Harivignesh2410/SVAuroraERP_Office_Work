namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Disptach
{
    public interface IPackingTransServiceRepository
    {
        int AddPackingTrans(PackingTrans request);
        List<VPackingTrans> GetPackingTransByID(int PackingID);
    }
}