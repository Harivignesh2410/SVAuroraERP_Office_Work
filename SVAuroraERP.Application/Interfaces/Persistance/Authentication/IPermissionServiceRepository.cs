//Added on 2025/05/24 by HARIVIGNESH
using Core.Logging.Models;

namespace SVAuroraERP.Application.Interfaces.Persistance.Authentication
{
    public interface IPermissionServiceRepository
    {
        PagePermissions GetPagePermissions(int RoleID, int PageControlID);

        //Added on 2025.05.19
        void InsertPageAccessAuditLog(PageAccessAudit request);
    }
}
