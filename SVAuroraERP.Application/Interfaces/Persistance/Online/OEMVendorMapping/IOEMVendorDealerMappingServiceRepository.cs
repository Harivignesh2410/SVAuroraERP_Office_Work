namespace SVAuroraERP.Application.Interfaces.Persistance.Online.OEMVendorMapping
{
    public interface IOEMVendorDealerMappingServiceRepository
    {
        DataResponse GetOEMVendorDealerMapping();
        DataResponse GetOEMVendorDealerMappingByID(int ID);
        DataResponse Save(OEMVendorDealerMapping OEMVendorDealerMapping);
        DataResponse Update(OEMVendorDealerMapping OEMVendorDealerMapping);
        DataResponse Delete(int OEMVendorDealerMappingID, int UserID);
        DataResponse GetDealerByOEMID(int OEMID);
        //DataResponse GetEmbossingStationByDealerID(int DealerID);
        DataResponse GetEmbossingStationByDealerID();
        DataResponse GetVendorCodeByEmbossingStationID(int EmbossingStationID);
        DataResponse GetOEMVendorDealerMappingDataTableList(DataTableRequest request);
    }
}