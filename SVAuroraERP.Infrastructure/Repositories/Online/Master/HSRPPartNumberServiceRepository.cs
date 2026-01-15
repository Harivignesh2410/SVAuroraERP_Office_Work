namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class HSRPPartNumberServiceRepository : IHSRPPartNumberServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<HSRPPartNumberServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public HSRPPartNumberServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<HSRPPartNumberServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetHSRPPartNumber()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPPartNumber.OrderBy(o => o.PartNumber).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("HSRPPartNumber", ActionType.ListData, null, null,null, "HSRPPartNumberServiceRepository.GetHSRPPartNumber()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPPartNumberServiceRepository.GetHSRPPartNumber()");
            }

            return dataResponse;
        }
        public DataResponse GetHSRPPartNumberByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPPartNumber.FirstOrDefault(w => w.HSRPPartNumberID == ID);
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
                _auditLogger.SaveActionLog("HSRPPartNumber", ActionType.Select, ID.ToString(), ID, null, "HSRPPartNumberServiceRepository.GetHSRPPartNumberByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "HSRPPartNumberServiceRepository.GetHSRPPartNumberByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(HSRPPartNumber HSRPPartNumber)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.HSRPPartNumber.FirstOrDefault(r => r.PartNumber == HSRPPartNumber.PartNumber);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.HSRPPartNumberID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                HSRPPartNumber.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.HSRPPartNumber.Add(HSRPPartNumber);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("HSRPPartNumber", ActionType.Insert, HSRPPartNumber.HSRPPartNumberID.ToString(), HSRPPartNumber,null, "HSRPPartNumberServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPPartNumber, "HSRPPartNumberServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(HSRPPartNumber HSRPPartNumber)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.HSRPPartNumber.FirstOrDefault(r => r.HSRPPartNumberID != HSRPPartNumber.HSRPPartNumberID && r.PartNumber == HSRPPartNumber.PartNumber);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.HSRPPartNumberID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.HSRPPartNumber.FirstOrDefault(r => r.HSRPPartNumberID == HSRPPartNumber.HSRPPartNumberID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("HSRPPartNumber", ActionType.Update, dataexists.HSRPPartNumberID.ToString(), HSRPPartNumber, dataexists, "HSRPPartNumberServiceRepository.Update()");
                dataexists.PartNumber = HSRPPartNumber.PartNumber;
                dataexists.OEMID = HSRPPartNumber.OEMID;
                dataexists.IsActive = HSRPPartNumber.IsActive;
                dataexists.LastUpdatedBy = HSRPPartNumber.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.HSRPPartNumberID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPPartNumber, "HSRPPartNumberServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int HSRPPartNumberID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.HSRPPartNumber.FirstOrDefault(w => w.HSRPPartNumberID == HSRPPartNumberID);
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

                dataResponse.ID = dataexists.HSRPPartNumberID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("HSRPPartNumber", ActionType.Delete, null, HSRPPartNumberID, new { HSRPPartNumberID , UserID , LoginAuditID }, "HSRPPartNumberServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPPartNumberID, "HSRPPartNumberServiceRepository.Delete()");
            }
            return dataResponse;
        }
        public DataResponse GetHSRPPartNumberByOEMID(int OEMId)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPPartNumber.Where(w => w.OEMID == OEMId).ToList();
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VHSRPPartNumber", ActionType.Select, OEMId.ToString(), OEMId, null, "HSRPPartNumberServiceRepository.GetHSRPPartNumberByOEMID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, OEMId, "HSRPPartNumberServiceRepository.GetHSRPPartNumberByOEMID()");
            }

            return dataResponse;
        }
        public DataResponse GetHSRPPartNumberDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHSRPPartNumber> query = _dbcontext.VHSRPPartNumber;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.PartNumber ?? "").Contains(request.SearchValue)
                    || (d.OEMName ?? "").Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VHSRPPartNumber.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.HSRPPartNumberID,
                                w.PartNumber,
                                w.OEMName,
                                w.IsActive
                            }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("District", ActionType.Select, null, request, null, "DistrictServiceRepository.GetDistrictList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "DistrictServiceRepository.GetDistrictList()");
            }
            return response;
        }
    }
}