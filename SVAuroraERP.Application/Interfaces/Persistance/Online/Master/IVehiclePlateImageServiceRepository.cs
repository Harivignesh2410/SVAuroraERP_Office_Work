namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IVehiclePlateImageServiceRepository
    {
        DataResponse GetVehiclePlateImage();
        DataResponse GetVehiclePlateImageByID(int ID);
        DataResponse Save(VehiclePlateImage VehiclePlateImage);
        DataResponse Update(VehiclePlateImage VehiclePlateImage);
        DataResponse Delete(int VehiclePlateImageID, int UserID);
    }
}