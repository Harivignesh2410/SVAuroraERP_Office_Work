namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface IUpdateOrderDataServiceRepository
    {
        DataResponse GetRectificationReason();
        DataResponse GetUpdateOrder(DataTableRequest request);
        DataResponse GetByID(int ID);
        DataResponse SaveLaserNoForOrder(LaserNoUpdateRequest request);
        DataResponse GetLaserNoByPartNo(string PartNo);
        DataResponse SaveRectification(RectifyLaserPlate request);
    }
}