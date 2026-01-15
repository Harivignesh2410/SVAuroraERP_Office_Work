namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IVehiclePlateColorServiceRepository
    {
        DataResponse GetVehiclePlateColor();
        DataResponse GetVehiclePlateColorByID(int ID);
        DataResponse Save(VehiclePlateColor VehiclePlateColor);
        DataResponse Update(VehiclePlateColor VehiclePlateColor);
        DataResponse Delete(int VehiclePlateColorID, int UserID, long LoginAuditID);
        DataResponse GetVehiclePlateColorDataTableList(DataTableRequest request);
    }
}