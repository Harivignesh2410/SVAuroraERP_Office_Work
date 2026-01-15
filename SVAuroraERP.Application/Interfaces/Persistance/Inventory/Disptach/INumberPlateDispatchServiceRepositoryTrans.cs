//Added on 2025.04.29 by Harivignesh
namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Disptach
{
    public interface INumberPlateDispatchServiceRepositoryTrans
    {
        int Save(NumberPlateDispatchTrans request);
        bool DeleteTrans(int PackingID);
        public List<VNumberPlateDispatchTrans> GetNumberPlateDispatchTransByID(int NumberPlateDispatchID);

    }
}
