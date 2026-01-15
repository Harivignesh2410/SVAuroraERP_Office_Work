namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class SizeServiceRepository : ISizeServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<SizeServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public SizeServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<SizeServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger
                                     )
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetSize()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VSize.OrderBy(o => o.SizeCode).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Size", ActionType.ListData, null, null,null, "SizeServiceRepository.GetSize()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "SizeServiceRepository.GetSize()");
            }

            return dataResponse;
        }
        public DataResponse GetByID(int SizeID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VSize.FirstOrDefault(w => w.SizeID == SizeID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataResponse.ID = SizeID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Size", ActionType.Select, SizeID.ToString(), SizeID, null, "SizeServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, SizeID, "SizeServiceRepository.GetByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(Size request)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.Size.FirstOrDefault(r => r.SizeCode == request.SizeCode);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.SizeID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;

                }

                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.Size.Add(request);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("Size", ActionType.Insert, dataexists.SizeID.ToString(), request, null, "SizeServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "SizeServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(Size request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.Size.FirstOrDefault(r => r.SizeID != request.SizeID && r.SizeName == request.SizeName);
                if (isFound != null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = isFound.SizeID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.Size.FirstOrDefault(r => r.SizeID == request.SizeID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("Size", ActionType.Update, request.SizeID.ToString(), request, dataexists, "SizeServiceRepository.Update()");
                dataexists.SizeCode = request.SizeCode;
                dataexists.SizeName = request.SizeName;
                dataexists.IsActive = request.IsActive;
                dataexists.LastUpdatedBy = request.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.SizeID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "SizeServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int SizeID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Size.FirstOrDefault(w => w.SizeID == SizeID);
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

                dataResponse.ID = dataexists.SizeID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Size", ActionType.Delete, SizeID.ToString(), null, null, "SizeServiceRepository.Delete()");

            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, SizeID, "SizeServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetSizeDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();

            // Validate and sanitize inputs
            var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
            var skip = Math.Max(request.Start, 0);

            IQueryable<VSize> query = _dbcontext.VSize;

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchValue))
            {
                query = query.Where(d => d.SizeName.Contains(request.SearchValue) || d.SizeCode.Contains(request.SearchValue));
            }

            // Get TOTAL records in database (unfiltered)
            var totalRecords = _dbcontext.VSize.Count();

            // Get FILTERED records count (same as total if no filter applied)
            var filteredRecords = query.Count();

            // Apply sorting 
            query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

            // Apply paging
            var pagedData = query.Skip(skip).Take(pageSize)
                                   .Select(w => new
                                   {
                                       w.SizeID,
                                       w.SizeName,
                                       w.SizeCode,
                                       w.IsActive
                                   }).ToList();

            response.Value = pagedData;
            response.recordsTotal = totalRecords;
            response.recordsFiltered = filteredRecords;
            _auditLogger.SaveActionLog("Size", ActionType.ListData, null, request, null, "SizeServiceRepository.GetSizeDataTable()");
            return response;
        }
    }
}