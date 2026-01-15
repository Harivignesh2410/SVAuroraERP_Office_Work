namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class HSRPPlateDimensionServiceRepository : IHSRPPlateDimensionServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<HSRPPlateDimensionServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public HSRPPlateDimensionServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<HSRPPlateDimensionServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetHSRPPlateDimension()
        {
            DataResponse response = new DataResponse();
            try
            {
                var district = _dbcontext.VHSRPPlateDimension.ToList();

                response.Count = district.Count;
                response.Value = district;
                _auditLogger.SaveActionLog("HSRPPlateDimension", ActionType.ListData, null, null,null, "HSRPPlateDimensionServiceRepository.GetHSRPPlateDimension()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "HSRPPlateDimensionServiceRepository.GetHSRPPlateDimension()");
            }
            return response;
        }
        public DataResponse GetHSRPPlateDimensionByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPPlateDimension.FirstOrDefault(w => w.HSRPPlateDimensionID == ID);
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
                _auditLogger.SaveActionLog("HSRPPlateDimension", ActionType.Select, ID.ToString(), ID, null, "HSRPPlateDimensionServiceRepository.GetHSRPPlateDimensionByID()");

            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "HSRPPlateDimensionServiceRepository.GetHSRPPlateDimensionByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(HSRPPlateDimension request)
        {

            DataResponse DataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.HSRPPlateDimension.FirstOrDefault(r => r.VehiclePlateSizeID == request.VehiclePlateSizeID
                                                                                     && r.VehiclePlateColorID == request.VehiclePlateColorID);
                if (dataexists != null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;
                    return DataResponse;
                }
                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.HSRPPlateDimension.Add(request);
                DataResponse.ID = request.HSRPPlateDimensionID;
                _dbcontext.SaveChanges();

                _auditLogger.SaveActionLog("HSRPPlateDimension", ActionType.Insert, request.HSRPPlateDimensionID.ToString(), request,null, "HSRPPlateDimensionServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "HSRPPlateDimensionServiceRepository.Save()");
            }

            return DataResponse;
        }
        public DataResponse Update(HSRPPlateDimension HSRPPlateDimension)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.HSRPPlateDimension.FirstOrDefault(r => r.HSRPPlateDimensionID != HSRPPlateDimension.HSRPPlateDimensionID && r.VehiclePlateColorID == HSRPPlateDimension.VehiclePlateColorID &&
                                                                                       r.VehiclePlateSizeID == HSRPPlateDimension.VehiclePlateSizeID);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.HSRPPlateDimensionID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.HSRPPlateDimension.FirstOrDefault(r => r.HSRPPlateDimensionID == HSRPPlateDimension.HSRPPlateDimensionID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("HSRPPlateDimension", ActionType.Update, dataexists.HSRPPlateDimensionID.ToString(), HSRPPlateDimension, dataexists, "HSRPPlateDimensionServiceRepository.Update()");
                dataexists.VehiclePlateSizeID = HSRPPlateDimension.VehiclePlateSizeID;
                dataexists.VehiclePlateColorID = HSRPPlateDimension.VehiclePlateColorID;
                dataexists.LastUpdatedBy = HSRPPlateDimension.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.HSRPPlateDimensionID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPPlateDimension, "HSRPPlateDimensionServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int HSRPPlateDimensionID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.HSRPPlateDimension.FirstOrDefault(w => w.HSRPPlateDimensionID == HSRPPlateDimensionID);
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

                dataResponse.ID = dataexists.HSRPPlateDimensionID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("HSRPPlateDimension", ActionType.Delete, null, HSRPPlateDimensionID,null, "HSRPPlateDimensionServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPPlateDimensionID, "HSRPPlateDimensionServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetHSRPPlateDimensionDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHSRPPlateDimension> query = _dbcontext.VHSRPPlateDimension;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.VehiclePlateSizeName ?? "").Contains(request.SearchValue)
                    || (d.VehiclePlateColorName ?? "").Contains(request.SearchValue)
                    || (d.Dimension ?? "").Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VHSRPPlateDimension.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.HSRPPlateDimensionID,
                                w.VehiclePlateSizeName,
                                w.VehiclePlateColorName,
                                w.Dimension
                            }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("HSRPPlateDimension", ActionType.Select, null, request, null, "HSRPPlateDimensionServiceRepository.GetHSRPPlateDimensionDataTableList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "HSRPPlateDimensionServiceRepository.GetHSRPPlateDimensionDataTableList()");
            }
            return response;
        }
    }
}