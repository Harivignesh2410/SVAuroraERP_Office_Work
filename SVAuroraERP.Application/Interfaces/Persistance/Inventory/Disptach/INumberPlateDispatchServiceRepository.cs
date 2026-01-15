//Added on 2025.04.29 by Harivignesh
namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Disptach
{
    public interface INumberPlateDispatchServiceRepository
    {
        public Tuple<bool, string> Save(NumberPlateDispatch Request);
        List<VNumberPlateDispatch> GetNumberPlateDispatchList();
        DataResponse Delete(int NumberPlateDispatchID, int LastUpdatedBy);
        List<VNumberPlateDispatchTrans> GetPackingByNumberPlateDispatchID1(int NumberPlateDispatchID);
        public VNumberPlateDispatch GetNumberPlateDispatchByID(int ID);
        public DataResponse InsertHSRPLaserStockTransID(int NumberPlateDispatchTransID, int lastUpdatedBy);
        List<VPacking> GetPackingByNumberPlateDispatchID(int NumberPlateDispatchID);

    }

}