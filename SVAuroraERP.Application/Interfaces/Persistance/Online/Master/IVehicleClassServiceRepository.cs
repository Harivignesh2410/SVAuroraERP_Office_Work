//Added on 2025.05.05 by Harivignesh (US-49)
namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IVehicleClassServiceRepository
    {
        DataResponse GetVehicleClass();
        DataResponse GetVehicleClassByID(int ID);
        DataResponse Save(VehicleClass VehicleClass);
        DataResponse Update(VehicleClass VehicleClass);
        DataResponse Delete(int VehicleClassID, int UserID, long LoginAuditID);
        DataResponse GetVehicleClassDataTableList(DataTableRequest request);
    }
}