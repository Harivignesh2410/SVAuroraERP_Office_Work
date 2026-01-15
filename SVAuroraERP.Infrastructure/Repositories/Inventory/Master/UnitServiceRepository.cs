namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class UnitServiceRepository : IUnitServiceRespository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<UnitServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public UnitServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<UnitServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetUnit()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VUnit.OrderBy(o => o.UnitCode).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Unit", ActionType.ListData, null, null,null, "UnitServiceRepository.GetUnit()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "UnitServiceRepository.GetUnit()");
            }

            return dataResponse;
        }
        public DataResponse GetByID(int UnitID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VUnit.FirstOrDefault(w => w.UnitID == UnitID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataResponse.ID = UnitID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Unit", ActionType.Select, UnitID.ToString(), UnitID, null, "TaxserviceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, UnitID, "UnitServiceRepository.GetByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(Unit request)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.Unit.FirstOrDefault(r => r.UnitCode == request.UnitCode);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.UnitID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.Unit.Add(request);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("Unit", ActionType.Insert, dataexists.UnitID.ToString(), request, null, "UnitServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "UnitServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(Unit request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.Unit.FirstOrDefault(r => r.UnitID != request.UnitID && r.UnitName == request.UnitName);
                if (isFound != null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = isFound.UnitID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.Unit.FirstOrDefault(r => r.UnitID == request.UnitID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("Unit", ActionType.Update, request.UnitID.ToString(), request, dataexists, "UnitServiceRepository.Update()");
                dataexists.UnitCode = request.UnitCode;
                dataexists.UnitName = request.UnitName;
                dataexists.IsActive = request.IsActive;
                dataexists.LastUpdatedBy = request.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.UnitID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "UnitServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int UnitID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Unit.FirstOrDefault(w => w.UnitID == UnitID);
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

                dataResponse.ID = dataexists.UnitID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Unit", ActionType.Delete, UnitID.ToString(), null, null, "UnitServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, UnitID, "UnitServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetUnitDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();

            // Validate and sanitize inputs
            var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
            var skip = Math.Max(request.Start, 0);

            IQueryable<VUnit> query = _dbcontext.VUnit;

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchValue))
            {
                query = query.Where(d => d.UnitName.Contains(request.SearchValue) || d.UnitCode.Contains(request.SearchValue));
            }

            // Get TOTAL records in database (unfiltered)
            var totalRecords = _dbcontext.VUnit.Count();

            // Get FILTERED records count (same as total if no filter applied)
            var filteredRecords = query.Count();

            // Apply sorting 
            query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

            // Apply paging
            var pagedData = query.Skip(skip).Take(pageSize)
                                   .Select(w => new
                                   {
                                       w.UnitID,
                                       w.UnitName,
                                       w.UnitCode,
                                       w.IsActive
                                   }).ToList();

            response.Value = pagedData;
            response.recordsTotal = totalRecords;
            response.recordsFiltered = filteredRecords;
            _auditLogger.SaveActionLog("Unit", ActionType.ListData, null, request, null, "UnitServiceRepository.GetUnitDataTable()");
            return response;
        }
    }
}