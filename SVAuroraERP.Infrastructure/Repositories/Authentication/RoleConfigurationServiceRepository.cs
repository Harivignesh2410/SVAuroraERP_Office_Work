namespace SVAuroraERP.Infrastructure.Repositories.Authentication
{
    public class RoleConfigurationServiceRepository : IRoleConfigurationServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<RoleConfigurationServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        public RoleConfigurationServiceRepository(SVAuroraERPDbContext dbcontext,
                                                  ILogger<RoleConfigurationServiceRepository> logger,
                                                  ITransLogRespository transLogRespository)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
        }
        public List<VRoleConfiguration> GetRoleConfigurationByRoleID(int RoleID)
        {
            return _dbcontext.VRoleConfiguration
                   .Where(w => w.RoleID == RoleID)  // Fetch based on RoleID
                   .OrderBy(p => p.MenuName)        // Order by MenuName (or appropriate property)
                   .ToList();
        }
        public List<LkupMenuGroup>? GetMenuLayout()
        {
            var result = _dbcontext.VMenuLayout
                             .OrderBy(vc => vc.MenuGroupOrdinalNo)
                             .ThenBy(vc => vc.MenuOrdinalNo)
                             .ThenBy(vc => vc.PageOrdinalNo)
                             .Select(vc => new
                             {
                                 MenuGroupId = vc.MenuGroupID,
                                 MenuGroupName = vc.MenuGroupName,
                                 MenuGroupOrdinalNo = vc.MenuGroupOrdinalNo,
                                 MenuControlId = vc.MenuControlID,
                                 MenuControlName = vc.MenuName,
                                 MenuIcon = vc.MenuIcon,
                                 ModuleID = vc.ModuleID,
                                 MenuControlOrdinalNo = vc.MenuOrdinalNo,
                                 MenuControlDisplayName = vc.MenuDisplayName,
                                 PageControlId = vc.PageControlID,
                                 PageName = vc.PageName,
                                 PageURL = vc.PageURL,
                                 PageIcon = vc.PageIcon,
                                 PageOrdinalNo = vc.PageOrdinalNo
                             })
                             .ToList();

            var groupedResult = result
            .GroupBy(g => new { g.MenuGroupId, g.MenuGroupName, g.MenuGroupOrdinalNo })
            .Select(g => new LkupMenuGroup
            {
                MenuGroupID = g.Key.MenuGroupId,
                MenuGroupName = g.Key.MenuGroupName,
                OrdinalNo = g.Key.MenuGroupOrdinalNo,
                MenuControlList = g.GroupBy(mc => new { mc.MenuControlId, mc.MenuControlName, mc.MenuControlDisplayName, mc.MenuIcon, mc.ModuleID, mc.MenuControlOrdinalNo })
                                .Select(mc => new LkupMenuControl
                                {
                                    MenuControlID = mc.Key.MenuControlId,
                                    MenuName = mc.Key.MenuControlName,
                                    MenuDisplayName = mc.Key.MenuControlDisplayName,
                                    MenuGroupID = g.Key.MenuGroupId,
                                    MenuIcon = mc.Key.MenuIcon,
                                    ModuleID = mc.Key.ModuleID,
                                    OrdinalNo = mc.Key.MenuControlOrdinalNo, //Added Page Control Grouping on 2024.12.27
                                    PageControlList = mc.GroupBy(p => new { p.PageControlId, p.MenuControlId, p.PageName, p.PageIcon, p.PageURL, p.PageOrdinalNo })
                                    .Select(pc => new LkupPageControl
                                    {
                                        MenuControlID = pc.Key.MenuControlId,
                                        PageControlID = pc.Key.PageControlId,
                                        PageName = pc.Key.PageName,
                                        PageIcon = pc.Key.PageIcon,
                                        PageURL = pc.Key.PageURL,
                                        OrdinalNo = pc.Key.PageOrdinalNo
                                    }).ToList()
                                }).ToList()
            }).ToList();

            return groupedResult;
        }

        public Tuple<bool, bool> SaveChanges(List<RoleConfiguration> request)
        {
            bool IsSuccess = false;
            bool doesRoleConfigurationExist = false;
            int SuccessCount = 0;

            var RoleID = request.Distinct().Select(s => s.RoleID).FirstOrDefault();

            if (RoleID > 0)
            {
                var ExitingPageControlIDs = _dbcontext.RoleConfiguration.Where(w => w.RoleID == RoleID).Select(s => s.PageControlID).ToList();
                var tempControlIDs = request.Select(s => s.PageControlID).ToList();

                var missingPageControlIDs = ExitingPageControlIDs.Except(tempControlIDs).ToList();

                //Bulk Update
                _dbcontext.RoleConfiguration
                        .Where(rc => rc.RoleID == RoleID && missingPageControlIDs.Contains(rc.PageControlID))
                        .ExecuteUpdate(setters => setters
                            .SetProperty(rc => rc.IsAccess, false)
                            .SetProperty(rc => rc.IsAdd, false)
                            .SetProperty(rc => rc.IsEdit, false)
                            .SetProperty(rc => rc.IsDelete, false)
                            .SetProperty(rc => rc.IsView, false)
                            .SetProperty(rc => rc.IsExport, false));

                foreach (var roleconfiguration in request)
                {
                    var dataExists = _dbcontext.RoleConfiguration.Where(w => w.RoleID == RoleID && w.PageControlID == roleconfiguration.PageControlID).FirstOrDefault();

                    if (dataExists != null)
                    {
                        dataExists.IsAccess = roleconfiguration.IsAccess;
                        dataExists.IsAdd = roleconfiguration.IsAdd;
                        dataExists.IsEdit = roleconfiguration.IsEdit;
                        dataExists.IsDelete = roleconfiguration.IsDelete;
                        dataExists.IsView = roleconfiguration.IsView;
                        dataExists.IsExport = roleconfiguration.IsExport;
                        dataExists.LastUpdatedBy = roleconfiguration.LastUpdatedBy;
                        dataExists.LastUpdatedDate = roleconfiguration.LastUpdatedDate;

                        _dbcontext.SaveChanges();
                    }
                    else
                    {
                        _dbcontext.RoleConfiguration.Add(roleconfiguration);
                        _dbcontext.SaveChanges();
                    }
                    SuccessCount++;
                }
            }

            if (SuccessCount > 0) IsSuccess = true;

            return Tuple.Create(IsSuccess, doesRoleConfigurationExist);
        }

        public List<VRoleConfiguration> GetRoleConfigurationByUserID(int UserID)
        {
            var RoleID = _dbcontext.User.Where(w => w.UserID == UserID)
                                        .Select(w => w.RoleID)
                                        .FirstOrDefault();


            var resultdata = _dbcontext.VRoleConfiguration
                   .Where(w => w.RoleID == RoleID)  // Fetch based on RoleID
                   .OrderBy(p => p.MenuName)        // Order by MenuName (or appropriate property)
                   .ToList();

            return resultdata;
        }
    }
}