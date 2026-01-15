namespace SVAuroraERP.Infrastructure.Repositories.Authentication
{
    public class RoleServiceRepository : IRoleServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<RoleServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public RoleServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<RoleServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger,
                                     IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }

        public  List<LkupModule> GetModuleList()
        {
            return  _dbcontext.LkupModule.ToList();
        }

        public  List<RoleModule> GetRoleModuleByID(int RoleID)
        {
            return  _dbcontext.RoleModule.Where(w => w.RoleID == RoleID).ToList();
        }

        public  List<VRole> GetList()
        {
            var roleList = new List<VRole>();
            roleList =  _dbcontext.VRole.ToList();

            return roleList;
        }

        public  VRole? GetByID(int ID)
        {
            var roleData =  _dbcontext.VRole.FirstOrDefault(w => w.RoleID == ID);
            return roleData;
        }

        public  Tuple<bool, bool> Save(Role request)
        {
            bool IsSuccess = false;
            bool doesRoleExist = false;

            try
            {
                // Assuming _context is your database context
                var existingRole = _dbcontext.Role.FirstOrDefault(r => r.RoleName == request.RoleName);

                if (existingRole == null) //Role does not exist, proceed to save
                {
                    request.LastUpdateDate = DateTime.UtcNow;
                     _dbcontext.Role.Add(request);
                     _dbcontext.SaveChanges();
                    IsSuccess = true;

                    if (request.RoleModuleIDs.Count > 0) SaveRoleModule(request.RoleModuleIDs, request);

                    //_transLogRespository.SaveLogTransaction(request.LoginAuditID, "tRole", request.RoleID.ToString(), Constants.ActionType.INSERT);
                }
                else
                    doesRoleExist = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"RoleServiceRepository.SaveRole(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                IsSuccess = false;
            }

            return Tuple.Create(IsSuccess, doesRoleExist);
        }

        public  Tuple<bool, bool> Update(Role request)
        {
            bool IsSuccess = false;
            bool doesRoleExist = false;

            try
            {
                var isFound =  _dbcontext.Role.FirstOrDefault(r => r.RoleID != request.RoleID && r.RoleName == request.RoleName);
                if (isFound != null)
                {
                    doesRoleExist = true;
                    IsSuccess = false;
                }

                var existingRole =  _dbcontext.Role.FirstOrDefault(r => r.RoleID == request.RoleID);
                if (existingRole != null & !doesRoleExist)
                {
                    existingRole.RoleName = request.RoleName;
                    existingRole.Description = request.Description;
                    existingRole.IsActive = request.IsActive;
                    existingRole.LastUpdatedBy = request.LastUpdatedBy;
                    existingRole.LastUpdateDate = DateTime.UtcNow;
                    existingRole.ApplicationID = request.ApplicationID;

                    // Update other properties as necessary
                    _dbcontext.SaveChanges();

                    if (request.RoleModuleIDs.Count > 0) SaveRoleModule(request.RoleModuleIDs, request);

                    doesRoleExist = false;
                    IsSuccess = true;

                    //_transLogRespository.SaveLogTransaction(request.LoginAuditID, "tRole", request.RoleID.ToString(), Constants.ActionType.UPDATE);
                }
                else
                {
                    doesRoleExist = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RoleServiceRepository.UpdateRole().RoleID: {request.RoleID}. Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                IsSuccess = false;
            }

            return Tuple.Create(IsSuccess, doesRoleExist);
        }

        public  Tuple<bool, bool> Delete(int ID, int UserID, long LoginAuditID)
        {
            bool IsSuccess = false;
            bool doesRoleExist = false;

            try
            {
                var existingRole =  _dbcontext.Role.FirstOrDefault(r => r.RoleID == ID);
                if (existingRole != null)
                {
                    existingRole.LastUpdateDate = DateTime.UtcNow;
                    existingRole.LastUpdatedBy = UserID;
                    existingRole.IsDeleted = true;

                     _dbcontext.SaveChanges();

                    doesRoleExist = true;
                    IsSuccess = true;

                  //  _transLogRespository.SaveLogTransaction(LoginAuditID, "tRole", ID.ToString(), ActionType.Delete);
                }
                else
                {
                    doesRoleExist = false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"RoleServiceRepository.DeleteRole().RoleID: {ID}. Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                IsSuccess = false;
            }

            return Tuple.Create(IsSuccess, doesRoleExist);
        }

        private void SaveRoleModule(List<byte>? RoleModuleIDs, Role request)
        {
            string moduleIdList = string.Join(",", RoleModuleIDs);

            using (var connection = _dbcontext.Database.GetDbConnection())
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SaveRoleModule";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var RoleIDParam = command.CreateParameter();
                    RoleIDParam.ParameterName = "@FK_RoleID";
                    RoleIDParam.Value = request.RoleID;

                    var RoleModuleIDsParam = command.CreateParameter();
                    RoleModuleIDsParam.ParameterName = "@RoleModuleIDs";
                    RoleModuleIDsParam.Value = moduleIdList;

                    var LastUpdatedByParam = command.CreateParameter();
                    LastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                    LastUpdatedByParam.Value = request.LastUpdatedBy;

                    command.Parameters.Add(RoleIDParam);
                    command.Parameters.Add(RoleModuleIDsParam);
                    command.Parameters.Add(LastUpdatedByParam);

                    command.ExecuteNonQuery();
                }

                connection.Dispose();
            }
        }
        public List<VRole> GetRoleByApplicationID(int ApplicationID)
        {
            return _dbcontext.VRole.Where(w=>w.ApplicationID==ApplicationID).ToList();
        }
        public List<LkupModule> GetModuleListByApplicationID(int ApplicationID)
        {
            return _dbcontext.LkupModule.Where(w=>w.ApplicationID== ApplicationID).ToList();
        }
    }
}