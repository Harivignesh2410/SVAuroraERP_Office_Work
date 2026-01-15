namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface IOtherChargesServiceRepository
    {
        DataResponse GetOtherCharges();
        DataResponse GetByID(int ID);
        DataResponse Save(OtherCharges OtherCharges);
        DataResponse Update(OtherCharges OtherCharges);
        DataResponse Delete(int OtherChargesID, int UserID, long LoginAuditID);
        DataResponse GetOtherChargesDataTable(DataTableRequest request);
    }
}
