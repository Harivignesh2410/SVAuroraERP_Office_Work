namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IOnlinePlatePriceServiceRepository
    {
        DataResponse GetOnlinePlatePrice();
        DataResponse GetVehicleCategory();
        DataResponse GetVehicleType();
        DataResponse GetFuel();
        DataResponse GetVehiclePlateType();
        DataResponse GetOnlinePlatePriceByID(int ID);
        DataResponse Save(OnlinePlatePrice OnlinePlatePrice);
        DataResponse Update(OnlinePlatePrice OnlinePlatePrice);
        DataResponse Delete(int OnlinePlatePriceID, int UserID, long LoginAuditID);
        DataResponse GetSizeByPlateTypeID(int ID,int id);
        DataResponse GetPlateTypeByVehicleClassID(int ID);
        DataResponse GetOnlinePlatePriceDataTableList(DataTableRequest request);
    }
}