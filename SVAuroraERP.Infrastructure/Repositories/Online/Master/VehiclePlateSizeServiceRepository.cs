namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class VehiclePlateSizeServiceRepository : IVehiclePlateSizeServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<VehiclePlateSizeServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public VehiclePlateSizeServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<VehiclePlateSizeServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetVehiclePlateSize()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VVehiclePlateSize.OrderBy(o => o.VehiclePlateSizeName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VVehiclePlateSize", ActionType.ListData, null, null,null, "VehiclePlateSizeServiceRepository.GetVehiclePlateSize()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "VehiclePlateSizeServiceRepository.GetVehiclePlateSize()");
            }

            return dataResponse;
        }
        public DataResponse GetVehiclePlateSizeByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VVehiclePlateSize.FirstOrDefault(w => w.VehiclePlateSizeID == ID);
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
                _auditLogger.SaveActionLog("VVehiclePlateSize", ActionType.Select, ID.ToString(), ID, null, "VehiclePlateSizeServiceRepository.GetVehiclePlateSizeByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "VehiclePlateSizeServiceRepository.GetVehiclePlateSizeByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(VehiclePlateSize VehiclePlateSize)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.VehiclePlateSize.FirstOrDefault(r => r.VehiclePlateSizeName == VehiclePlateSize.VehiclePlateSizeName);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.VehiclePlateSizeID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                VehiclePlateSize.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.VehiclePlateSize.Add(VehiclePlateSize);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("VehiclePlateSize", ActionType.Insert, VehiclePlateSize.VehiclePlateSizeID.ToString(), VehiclePlateSize,null, "VehiclePlateSizeServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, VehiclePlateSize, "VehiclePlateSizeServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(VehiclePlateSize VehiclePlateSize)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.VehiclePlateSize.FirstOrDefault(r => r.VehiclePlateSizeID != VehiclePlateSize.VehiclePlateSizeID && r.VehiclePlateSizeName == VehiclePlateSize.VehiclePlateSizeName);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.VehiclePlateSizeID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.VehiclePlateSize.FirstOrDefault(r => r.VehiclePlateSizeID == VehiclePlateSize.VehiclePlateSizeID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("VehiclePlateSize", ActionType.Update, dataexists.VehiclePlateSizeID.ToString(), VehiclePlateSize, dataexists, "VehiclePlateSizeServiceRepository.Update()");
                dataexists.VehiclePlateSizeName = VehiclePlateSize.VehiclePlateSizeName;
                dataexists.VehiclePlateSizeCode = VehiclePlateSize.VehiclePlateSizeCode;
                dataexists.IsActive = VehiclePlateSize.IsActive;
                dataexists.LastUpdatedBy = VehiclePlateSize.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, VehiclePlateSize, "VehiclePlateSizeServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int VehiclePlateSizeID, int UserID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.VehiclePlateSize.FirstOrDefault(w => w.VehiclePlateSizeID == VehiclePlateSizeID);
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

                dataResponse.ID = dataexists.VehiclePlateSizeID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("VehiclePlateSize", ActionType.Delete, null, VehiclePlateSizeID,null, "VehiclePlateSizeServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, VehiclePlateSizeID, "VehiclePlateSizeServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetVehiclePlateSizeDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VVehiclePlateSize> query = _dbcontext.VVehiclePlateSize;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.VehiclePlateSizeName ?? "").Contains(request.SearchValue)
                    || (d.VehiclePlateSizeCode ?? "").Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VVehiclePlateSizeMapping.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.VehiclePlateSizeID,
                                w.VehiclePlateSizeCode,
                                w.VehiclePlateSizeName,                               
                                w.IsActive
                            }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VehiclePlateColor", ActionType.Select, null, request, null, "VehiclePlateColorServiceRepository.GetVehiclePlateColorDataTableList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "VehiclePlateColorServiceRepository.GetVehiclePlateColorDataTableList()");
            }
            return response;
        }
    }
}