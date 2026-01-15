//Added on 2025/04/18 by Harivignesh
namespace SVAuroraERP.Application.Interfaces.Persistance.Disptach
{
    public  interface IPackingServiceRepository
    {
       // List<VStockPacking> GetNumberPlateByFilter(PackingFilter searchFilter);
        List<AvailableLaserNoDto> GetAvailableLaserNos(PackingFilter searchFilter);
        Tuple<bool, int> Save(Packing Request);
        List<VPacking> GetPackingList();
        VPacking GetByID(int ID);
        Tuple<bool, bool> Delete(int PackingID, int LastUpdatedBy);
        List<VPacking> GetPackingListByStatus(int AllotedToID);
    }
}
