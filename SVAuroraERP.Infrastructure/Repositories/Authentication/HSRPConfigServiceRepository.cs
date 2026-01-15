namespace SVAuroraERP.Infrastructure.Repositories.Authentication
{
    public class HSRPConfigServiceRepository : IHSRPConfigServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public HSRPConfigServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<HSRPConfigServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IErrorLoggerService errorLoggerService,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
        }
        public DataResponse GetHSRPUser()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPUser.OrderBy(o => o.HSRPUserCode).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPConfigServiceRepository.GetHSRPUser()");
            }
            _auditLogger.SaveActionLog("VHSRPUser", ActionType.ListData, null, null);
            return dataResponse;
        }
        public DataResponse GetRole()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VRole.OrderBy(o => o.RoleName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPConfigServiceRepository.GetRole()");
            }
            _auditLogger.SaveActionLog("VRole", ActionType.ListData, null, null);
            return dataResponse;
        }

        public DataResponse GetHSRPConfig()
        {
            var dataResponse = new DataResponse();
            try
            {
                var oemConfig = _dbcontext.VOEMConfig
                    .OrderBy(o => o.TVSOEMID)
                    .ToList();

                var roleConfig = _dbcontext.VHSRPRoleConfig
                    .OrderBy(r => r.SuperAdminRoleID)
                    .ToList();

                var result = new HSRPConfigResponse
                {
                    OEMConfigList = oemConfig,
                    RoleConfigList = roleConfig
                };

                dataResponse.Count = oemConfig.Count + roleConfig.Count;
                dataResponse.Value = result;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPConfigServiceRepository.GetHSRPConfig()");
            }

            _auditLogger.SaveActionLog("HSRPConfig", ActionType.ListData, null, null);
            return dataResponse;
        }

        public DataResponse Save(OEMConfig model)
        {
            var response = new DataResponse();
            try
            {
                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                using var command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "SaveOEMConfig";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@TVSOEMID", model.TVSOEMID));
                command.Parameters.Add(new SqlParameter("@SaravanaEngOEMID", model.SaravanaEngOEMID));
                command.Parameters.Add(new SqlParameter("@EroyceMotorsOEMID", model.EroyceMotorsOEMID));
                command.Parameters.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));

                command.ExecuteNonQuery();

                response.Success = true;
                response.Message = "OEM Config saved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            _auditLogger.SaveActionLog("SaveOEMConfig", ActionType.ListData, null, null);

            return response;
        }
        public DataResponse SaveRole(HSRPRoleConfig model)
        {
            var response = new DataResponse();
            try
            {
                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                using var command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "SaveHSRPRoleConfig";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@SuperAdminRoleID", model.SuperAdminRoleID));
                command.Parameters.Add(new SqlParameter("@AdminRoleID", model.AdminRoleID));
                command.Parameters.Add(new SqlParameter("@EmbossingStationRoleID", model.EmbossingStationRoleID));
                command.Parameters.Add(new SqlParameter("@OEMRoleID", model.OEMRoleID));
                command.Parameters.Add(new SqlParameter("@DealerRoleID", model.DealerRoleID));
                command.Parameters.Add(new SqlParameter("@DealerSubUserID", model.DealerSubUserID));
                command.Parameters.Add(new SqlParameter("@EmbossingSubUserID", model.EmbossingSubUserID));
                command.Parameters.Add(new SqlParameter("@OEMSubUserID", model.OEMSubUserID));


                command.Parameters.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));

                command.ExecuteNonQuery();

                response.Success = true;
                response.Message = "OEM Config saved successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            _auditLogger.SaveActionLog("SaveHSRPRoleConfig", ActionType.ListData, null, null);

            return response;
        }


    }
}
