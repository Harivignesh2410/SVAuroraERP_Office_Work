namespace SVAuroraERP.Infrastructure.Repositories.Authentication
{
    public class PermissionServiceRepository : IPermissionServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbContext;
        private readonly IAuditLogger _auditlogger;
        public PermissionServiceRepository(SVAuroraERPDbContext dbContext,IAuditLogger auditLogger )
        {
            _dbContext = dbContext;
            _auditlogger = auditLogger;
        }

        public PagePermissions GetPagePermissions(int RoleID, int PageControlID)
        {
            var permission = _dbContext.VRoleConfiguration.Where(rc => rc.RoleID == RoleID
                                && rc.PageControlID == PageControlID).FirstOrDefault();

            return permission == null
               ? new PagePermissions()
               : new PagePermissions
               {
                   HasAccess = permission.IsAccess ? true : false,
                   HasAdd = permission.IsAdd ? true : false,
                   HasEdit = permission.IsEdit ? true : false,
                   HasDelete = permission.IsDelete ? true : false,
                   HasView = permission.IsView ? true : false,
                   HasExport = permission.IsExport ? true : false
               };
        }

        public void InsertPageAccessAuditLog(PageAccessAudit request)
        {
            _auditlogger.InsertPageAccessAuditLog(request);
        }
    }
}