namespace SVAuroraERP.Infrastructure.Repositories.Authentication
{
    public class AppLauncherServiceRepository : IAppLauncherServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        public AppLauncherServiceRepository(SVAuroraERPDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public List<VApplauncher> GetByUserID(int UserID)
        {
            var resultdata = _dbcontext.VApplauncher.Where(w => w.UserID == UserID && w.IsActive == true).OrderBy(w => w.OrdinalNo).ToList();

            return resultdata;
        }
        public List<VRoleConfiguration> GetAppLauncherListByUserID(int UserID)
        {
            var RoleID = _dbcontext.User.Where(w => w.UserID == UserID)
                                        .Select(w => w.RoleID)
                                        .FirstOrDefault();

            var resultdata = _dbcontext.VRoleConfiguration
                   .Where(w => w.RoleID == RoleID)  // Fetch based on RoleID
                   .OrderBy(p => p.MenuName)        // Order by MenuName (or appropriate property)
                   .ToList();

            var AllPageControlIDs = resultdata.Select(s => s.PageControlID).ToList();

            var userAppLauncher = _dbcontext.VApplauncher.Where(w => w.UserID == UserID && w.IsActive).ToList();
            var AssignedPageControlIDs = userAppLauncher.Select(s => s.PageControlID).ToList();

            var pendingPageControlIDs = AllPageControlIDs.Except(AssignedPageControlIDs).ToList();

            //Return only the Page ControlIDs which are not already added to App Launcher Table
            var availablePageControlIDstoUser = resultdata.Where(w => pendingPageControlIDs.Contains(w.PageControlID))
                                                .OrderBy(o => o.ModuleOrdinalNo).ThenBy(o => o.MenuGroupOrdinalNo)
                                                .ThenBy(o => o.MenuOrdinalNo).ThenBy(o => o.PageOrdinalNo).ToList();

            return availablePageControlIDstoUser;
        }

        public Tuple<bool, bool> Save(List<AppLauncher> request)
        {
            bool IsSuccess = false;
            bool doesRoleConfigurationExist = false;
            int SuccessCount = 0;

            var UserID = request.Distinct().Select(s => s.UserID).FirstOrDefault();

            if (UserID > 0)
            {
                var ExitingPageControlIDs = _dbcontext.AppLauncher.Where(w => w.UserID == UserID && w.IsActive).Select(s => s.PageControlID).ToList();
                var tempControlIDs = request.Select(s => s.PageControlID).ToList();

                var missingPageControlIDs = ExitingPageControlIDs.Except(tempControlIDs).ToList();

                //Bulk Update
                _dbcontext.AppLauncher
                        .Where(rc => rc.UserID == UserID && missingPageControlIDs.Contains(rc.PageControlID))
                        .ExecuteUpdate(setters => setters
                            .SetProperty(rc => rc.IsActive, false)
                            .SetProperty(rc => rc.LastUpdatedDate, DateTime.UtcNow));

                foreach (var appLauncher in request)
                {
                    var dataExists = _dbcontext.AppLauncher.Where(w => w.UserID == UserID && w.PageControlID == appLauncher.PageControlID).FirstOrDefault();

                    if (dataExists != null)
                    {
                        dataExists.IsActive = true;
                        dataExists.OrdinalNo = appLauncher.OrdinalNo;
                        dataExists.LastUpdatedDate = DateTime.UtcNow;
                        _dbcontext.SaveChanges();
                    }
                    else
                    {
                        appLauncher.IsActive = true;
                        appLauncher.LastUpdatedDate = DateTime.UtcNow;
                        _dbcontext.AppLauncher.Add(appLauncher);
                        _dbcontext.SaveChanges();
                    }
                    SuccessCount++;
                }
            }

            if (SuccessCount > 0) IsSuccess = true;

            return Tuple.Create(IsSuccess, doesRoleConfigurationExist);
        }
    }
}