namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class VehicleClassServiceRepository : IVehicleClassServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<VehicleClassServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public VehicleClassServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<VehicleClassServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetVehicleClass()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VVehicleClass.OrderBy(o => o.VehicleClassName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VVehicleClass", ActionType.ListData, null, null, null, "VehicleClassServiceRepository.GetVehicleClass()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "VehicleClassServiceRepository.GetVehicleClass()");
            }

            return dataResponse;
        }
        public DataResponse GetVehicleClassByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VVehicleClass.FirstOrDefault(w => w.VehicleClassID == ID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataResponse.ID = ID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VVehicleClass", ActionType.Select, ID.ToString(), ID, null, "VehicleClassServiceRepository.GetVehicleClassByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "VehicleClassServiceRepository.GetVehicleClassByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(VehicleClass VehicleClass)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.VehicleClass.FirstOrDefault(r => r.VehicleClassName == VehicleClass.VehicleClassName);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.VehicleClassID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                VehicleClass.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.VehicleClass.Add(VehicleClass);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("VehicleClass", ActionType.Insert, VehicleClass.VehicleClassID.ToString(), VehicleClass, null, "VehicleClassServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, VehicleClass, "VehicleClassServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(VehicleClass VehicleClass)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.VehicleClass.FirstOrDefault(r => r.VehicleClassID != VehicleClass.VehicleClassID && r.VehicleClassName == VehicleClass.VehicleClassName);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.VehicleClassID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.VehicleClass.FirstOrDefault(r => r.VehicleClassID == VehicleClass.VehicleClassID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("VehicleClass", ActionType.Update, dataexists.VehicleClassID.ToString(), VehicleClass, dataexists, "VehicleClassServiceRepository.Update()");
                dataexists.VehicleClassName = VehicleClass.VehicleClassName;
                dataexists.VehicleClassCode = VehicleClass.VehicleClassCode;
                dataexists.IsActive = VehicleClass.IsActive;
                dataexists.LastUpdatedBy = VehicleClass.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.VehicleClassID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, VehicleClass, "VehicleClassServiceRepository.Update()");
            }


            return dataResponse;
        }
        public DataResponse Delete(int VehicleClassID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.VehicleClass.FirstOrDefault(w => w.VehicleClassID == VehicleClassID);
                if (dataexists == null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }

                dataexists.LastUpdatedDate = DateTime.UtcNow;
                dataexists.LastUpdatedBy = UserID;
                dataexists.IsDeleted = true;
                _dbcontext.SaveChanges();

                dataResponse.ID = dataexists.VehicleClassID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("VehicleClass", ActionType.Delete, null, new { VehicleClassID, UserID, LoginAuditID }, null, "VehicleClassServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, new { VehicleClassID, UserID, LoginAuditID }, "VehicleClassServiceRepository.Delete()");
            }
            return dataResponse;
        }
        public DataResponse GetVehicleClassDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VVehicleClass> query = _dbcontext.VVehicleClass;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.VehicleClassName ?? "").Contains(request.SearchValue)
                    || (d.VehicleClassCode ?? "").Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VVehicleClass.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.VehicleClassID,
                                w.VehicleClassName,
                                w.VehicleClassCode,
                                w.IsActive
                            }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VehicleClass", ActionType.Select, null, request, null, "VehicleClassServiceRepository.GetVehicleClassDataTableList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "VehicleClassServiceRepository.GetVehicleClassDataTableList()");
            }
            return response;
        }
    }
}