namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IVehiclePlateSizeServiceRepository
    {
        DataResponse GetVehiclePlateSize();
        DataResponse GetVehiclePlateSizeByID(int ID);
        DataResponse Save(VehiclePlateSize VehiclePlateSize);
        DataResponse Update(VehiclePlateSize VehiclePlateSize);
        DataResponse Delete(int VehiclePlateSizeID, int UserID);
        DataResponse GetVehiclePlateSizeDataTableList(DataTableRequest request);
    }
}