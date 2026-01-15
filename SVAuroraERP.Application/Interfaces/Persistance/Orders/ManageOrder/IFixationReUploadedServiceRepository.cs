namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.ManageOrder
{
    public interface IFixationReUploadedServiceRepository
    {
        DataResponse GetFixationReUploaded(FixationReUploadedRequest request);
        DataResponse SummaryForLaserNoAllocation(int userID);
        DataResponse UpdateVehiclePlateStatus(SaveFixationReUploadedRequest model);
        DataResponse GetVehicleImageData(int ID);
    }
}
