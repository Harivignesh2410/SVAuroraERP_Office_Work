//Added on 2025.09.27 by Harivignesh
namespace SVAuroraERP.Application.Interfaces.Persistance.Orders.Import
{
    public interface IImportOEMServiceRepository
    {
        DataResponse Save(ImportOEM request);
        DataResponse GetOEMConfigData();
        DataResponse GetImportOEMtoDataTable(ImportOEMFilter request);
        DataResponse ImportOEMData(ImportOEMDData request);
        DataResponse DeleteImportOEMData(int PK_OEMImportID, int LastUpdatedBy);
        DataResponse GetImportDataByID(int PK_OEMImportID);
    }
}