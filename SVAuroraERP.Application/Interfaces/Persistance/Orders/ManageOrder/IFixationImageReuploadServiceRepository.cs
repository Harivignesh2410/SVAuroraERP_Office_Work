namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface IFixationImageReuploadServiceRepository
    {
        DataResponse GetFixationImageReuploadOrders(FixationImageReuploadRequest request);
        DataResponse GetHsrporderByID(int HsrporderID);
        DataResponse SaveHSRPPlateImage(HSRPVehiclePlateImage request);
        DataResponse SummaryForImageReuploadData(int UserID);
    }
}