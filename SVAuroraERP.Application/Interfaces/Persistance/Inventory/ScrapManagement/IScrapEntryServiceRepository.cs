//Added on 2025/10/30 by Harivignesh
using SVAuroraERP.Domain.Inventory.ScrapManagement;

namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.ScrapManagement
{
    public interface IScrapEntryServiceRepository
    {
        DataResponse GetScrapDataByComponentTypeID(ScrapDataParameter request);
        DataResponse GetScrapDataTable(DataTableRequest request);
        DataResponse Save(ScrapEntry ScrapEntry);
        //DataResponse Update(ScrapEntry ScrapEntry);
        DataResponse Delete(int ID, int UserID);
        DataResponse GetScrapEntryByID(int ID);
        DataResponse GetScrapStockData(ScrapDataFilterParameter request);
    }
}
