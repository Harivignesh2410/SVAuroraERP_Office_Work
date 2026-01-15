namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public   class HSRPReplacementReasonServiceRepository : IHSRPReplacementReasonServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<HSRPReplacementReasonServiceRepository> _logger;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public HSRPReplacementReasonServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<HSRPReplacementReasonServiceRepository> logger,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _auditLogger = auditLogger;
        }
        public DataResponse GetHSRPReplacementReason()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPReplacementReason.OrderBy(o => o.ReplacementReasonName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VHSRPReplacementReason", ActionType.ListData, null, null,null, "HSRPReplacementReasonServiceRepository.GetHSRPReplacementReason()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPReplacementReasonServiceRepository.GetHSRPReplacementReason()");
            }

            return dataResponse;
        }
        public DataResponse GetHSRPReplacementReasonByID(int ID)
        {

            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPReplacementReason.FirstOrDefault(w => w.HSRPReplacementReasonID == ID);
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
                _auditLogger.SaveActionLog("HSRPReplacementReason", ActionType.Select, ID.ToString(), ID, null, "HSRPReplacementReasonServiceRepository.GetHSRPReplacementReasonByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "HSRPReplacementReasonServiceRepository.GetHSRPReplacementReasonByID()");
            }
            return dataResponse;
        }
        public DataResponse Save(HSRPReplacementReason HSRPReplacementReason)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.HSRPReplacementReason.FirstOrDefault(r => r.ReplacementReasonName == HSRPReplacementReason.ReplacementReasonName);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.HSRPReplacementReasonID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                HSRPReplacementReason.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.HSRPReplacementReason.Add(HSRPReplacementReason);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("HSRPReplacementReason", ActionType.Insert, HSRPReplacementReason.HSRPReplacementReasonID.ToString(), HSRPReplacementReason,null, "HSRPReplacementReasonServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPReplacementReason, "HSRPReplacementReasonServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(HSRPReplacementReason HSRPReplacementReason)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.HSRPReplacementReason.FirstOrDefault(r => r.HSRPReplacementReasonID != HSRPReplacementReason.HSRPReplacementReasonID && r.ReplacementReasonName == HSRPReplacementReason.ReplacementReasonName);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.HSRPReplacementReasonID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.HSRPReplacementReason.FirstOrDefault(r => r.HSRPReplacementReasonID == HSRPReplacementReason.HSRPReplacementReasonID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("HSRPReplacementReason", ActionType.Update, dataexists.HSRPReplacementReasonID.ToString(), HSRPReplacementReason,null, "HSRPReplacementReasonServiceRepository.Update()");
                dataexists.ReplacementReasonName = HSRPReplacementReason.ReplacementReasonName;
                dataexists.Code = HSRPReplacementReason.Code;
                dataexists.IsActive = HSRPReplacementReason.IsActive;
                dataexists.LastUpdatedBy = HSRPReplacementReason.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.HSRPReplacementReasonID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPReplacementReason, "HSRPReplacementReasonServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int HSRPReplacementReasonID, int UserID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.HSRPReplacementReason.FirstOrDefault(w => w.HSRPReplacementReasonID == HSRPReplacementReasonID);
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

                dataResponse.ID = dataexists.HSRPReplacementReasonID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("HSRPReplacementReason", ActionType.Delete, null, HSRPReplacementReasonID,null, "HSRPReplacementReasonServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPReplacementReasonID, "HSRPReplacementReasonServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetHSRPReplacementReasonDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHSRPReplacementReason> query = _dbcontext.VHSRPReplacementReason;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.ReplacementReasonName ?? "").Contains(request.SearchValue)
                    || (d.Code ?? "").Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VHSRPReplacementReason.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.HSRPReplacementReasonID,
                                w.ReplacementReasonName,
                                w.Code,
                                w.IsActive
                            }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("HSRPReplacementReason", ActionType.Select, null, request, null, "HSRPReplacementReasonServiceRepository.GetHSRPReplacementReasonDataTableList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "HSRPReplacementReasonServiceRepository.GetHSRPReplacementReasonDataTableList()");
            }
            return response;
        }
    }
}