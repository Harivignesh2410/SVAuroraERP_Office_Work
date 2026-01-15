namespace SVAuroraERP.Application.Interfaces.Persistance.Online.OEMVendorMapping
{
    public interface IOEMVendorCodeMappingServiceRepository
    {
        DataResponse GetOEMVendorCodeMapping();
        DataResponse GetOEMVendorCodeMappingByID(int ID);
        DataResponse Save(OEMVendorCodeMapping OEMVendorCodeMapping);
        DataResponse Update(OEMVendorCodeMapping OEMVendorCodeMapping);
        DataResponse Delete(int OEMVendorCodeMappingID, int UserID);
        DataResponse GetOEMVendorCodeMappingDataTableList(DataTableRequest request);
    }
}