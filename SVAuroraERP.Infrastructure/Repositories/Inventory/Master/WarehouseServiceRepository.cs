namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class WareHouseServiceRepository : IWareHouseServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<WareHouseServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public WareHouseServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<WareHouseServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetWareHouse()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VWareHouse.OrderBy(o => o.WareHouseCode).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("WareHouse", ActionType.ListData, null, null, null,"WareHouseServiceRepository.GetWareHouse()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "WareHouseServiceRepository.GetWareHouse()");
            }

            return dataResponse;
        }
        public DataResponse GetByID(int WareHouseID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VWareHouse.FirstOrDefault(w => w.WareHouseID == WareHouseID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataResponse.ID = WareHouseID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("WareHouse", ActionType.Select, WareHouseID.ToString(), WareHouseID, null, "WareHouseServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, WareHouseID, "WareHouseServiceRepository.GetByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(WareHouse request)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.WareHouse.FirstOrDefault(r => r.WareHouseCode == request.WareHouseCode);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.WareHouseID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.WareHouse.Add(request);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("WareHouse", ActionType.Insert, dataexists.WareHouseID.ToString(), request, null, "WareHouseServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "WareHouseServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(WareHouse request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.WareHouse.FirstOrDefault(r => r.WareHouseID != request.WareHouseID && r.WareHouseName == request.WareHouseName);
                if (isFound != null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = isFound.WareHouseID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.WareHouse.FirstOrDefault(r => r.WareHouseID == request.WareHouseID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("WareHouse", ActionType.Update, request.WareHouseID.ToString(), request, dataexists, "WareHouseServiceRepository.Update()");
                dataexists.WareHouseCode = request.WareHouseCode;
                dataexists.WareHouseName = request.WareHouseName;
                dataexists.IsActive = request.IsActive;
                dataexists.LastUpdatedBy = request.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.WareHouseID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "WareHouseServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int WareHouseID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.WareHouse.FirstOrDefault(w => w.WareHouseID == WareHouseID);
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

                dataResponse.ID = dataexists.WareHouseID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("WareHouse", ActionType.Delete, WareHouseID.ToString(), null, null, "WareHouseServiceRepository.Delete()");

            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, WareHouseID, "WareHouseServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetWareHouseDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();

            // Validate and sanitize inputs
            var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
            var skip = Math.Max(request.Start, 0);

            IQueryable<VWareHouse> query = _dbcontext.VWareHouse;

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchValue))
            {
                query = query.Where(d => d.WareHouseName.Contains(request.SearchValue) || d.WareHouseCode.Contains(request.SearchValue));
            }

            // Get TOTAL records in database (unfiltered)
            var totalRecords = _dbcontext.VWareHouse.Count();

            // Get FILTERED records count (same as total if no filter applied)
            var filteredRecords = query.Count();

            // Apply sorting 
            query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

            // Apply paging
            var pagedData = query.Skip(skip).Take(pageSize)
                                   .Select(w => new
                                   {
                                       w.WareHouseID,
                                       w.WareHouseName,
                                       w.WareHouseCode,
                                       w.IsActive
                                   }).ToList();

            response.Value = pagedData;
            response.recordsTotal = totalRecords;
            response.recordsFiltered = filteredRecords;
            _auditLogger.SaveActionLog("WareHouse", ActionType.ListData, null, request, null, "WareHouseServiceRepository.GetWareHouseDataTable()");
            return response;
        }
    }
}