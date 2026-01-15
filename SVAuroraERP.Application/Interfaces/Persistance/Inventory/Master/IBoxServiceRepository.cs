//Added on 2025/04/17 by Harivignesh
namespace SVAuroraERP.Application.Interfaces.Persistance.Inventory.Master
{
    public interface IBoxServiceRepository
    {
        DataResponse GetBox();
        DataResponse GetByID(int ID);
        DataResponse Save(Box Box);
        DataResponse Update(Box Box);
        DataResponse Delete(int BoxID, int UserID, long LoginAuditID);
        DataResponse GetBoxtoDataTable(DataTableRequest request);
    }
}
