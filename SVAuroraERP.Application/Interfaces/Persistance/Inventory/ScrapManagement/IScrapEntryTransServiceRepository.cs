//Added on 2025/10/30 by Harivignesh
using SVAuroraERP.Domain.Inventory.ScrapManagement;

namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.ScrapManagement
{
    public interface IScrapEntryTransServiceRepository
    {
        DataResponse SaveScrapEntryTransDetails(ScrapEntryTrans request);
    }
}
