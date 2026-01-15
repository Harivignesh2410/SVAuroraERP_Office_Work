namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IVehiclePlateSizeMappingServiceRepository
    {
        DataResponse GetVehiclePlateSizeMapping();
        DataResponse GetVehicleCategory();
        DataResponse GetVehicleType();
        DataResponse GetFuel();
        DataResponse GetVehiclePlateType();
        DataResponse GetOnlinePlatePriceByID(int ID);
        DataResponse GetVehiclePlateSizeMappingByID(int ID);
        DataResponse Save(VehiclePlateSizeMapping VehiclePlateSizeMapping);
        DataResponse Update(VehiclePlateSizeMapping VehiclePlateSizeMapping);
        DataResponse Delete(int VehiclePlateSizeMappingID, int UserID);
        DataResponse GetVehiclePlateSizeMappingDataTableList(DataTableRequest request);
    }
}
