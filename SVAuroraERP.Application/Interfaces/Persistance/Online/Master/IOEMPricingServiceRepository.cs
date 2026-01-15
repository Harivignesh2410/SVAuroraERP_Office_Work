namespace SVAuroraERP.Application.Interfaces.Persistance.Online.Master
{
    public interface IOEMPricingServiceRepository
    {
        DataResponse GetOEMPricing();
        DataResponse GetOEMPricingByID(int ID);
        DataResponse Save(OEMPricing OEMPricing);
        DataResponse Update(OEMPricing OEMPricing);
        DataResponse Delete(int OEMPricingID, int UserID, long LoginAuditID);
        DataResponse GetOEMPricingDataTableList(DataTableRequest request);
    }
}
