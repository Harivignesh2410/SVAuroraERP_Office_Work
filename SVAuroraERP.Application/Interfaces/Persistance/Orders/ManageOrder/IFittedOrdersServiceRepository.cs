namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface IFittedOrdersServiceRepository
    {
        DataResponse GetFittedOrder(FittedOrderRequest request);
        DataResponse UpdateVehiclePlateStatus(SaveFittedOrderRequest model);
        DataResponse GetVehicleImageData(int ID);
    }
}
