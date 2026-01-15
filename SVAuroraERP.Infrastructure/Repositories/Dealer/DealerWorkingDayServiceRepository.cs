namespace SVAuroraERP.Infrastructure.Repositories.Dealer
{
    public class DealerWorkingDayServiceRepository : IDealerWorkingDayServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<DealerWorkingDayServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public DealerWorkingDayServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<DealerWorkingDayServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IErrorLoggerService errorLoggerService,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
        }
        public DataResponse GetDealerWorkingDay()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDealerWorkingDay.OrderBy(o => o.DealerName).ThenBy(o => o.DayOfWeek).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VDealerWorkingDay", ActionType.ListData, null, null, null, "DealerWorkingDayServiceRepository.GetDealerWorkingDay()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "DealerWorkingDayServiceRepository.GetDealerWorkingDay()");
            }

            return dataResponse;
        }
        
        public DataResponse GetDealerWorkingDayToDataTable(DealerWorkingDayDataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                var pageSize = Math.Clamp(request.Length, 1, 100);
                var skip = Math.Max(request.Start, 0);

                IQueryable<VDealerWorkingDay> query = _dbcontext.VDealerWorkingDay;

                // Apply filters at database level
                if (request.OEMID.HasValue && request.OEMID.Value > 0) { query = query.Where(w => w.OEMID == request.OEMID.Value); }
                if (request.DealerID.HasValue && request.DealerID.Value > 0) { query = query.Where(w => w.DealerID == request.DealerID.Value); }

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d =>
                        (d.OEMName ?? string.Empty).Contains(request.SearchValue) ||
                        (d.DealerName ?? string.Empty).Contains(request.SearchValue) ||
                        (d.DealerCode ?? string.Empty).Contains(request.SearchValue) ||
                        (d.City ?? string.Empty).Contains(request.SearchValue));
                }

                // Materialize the query first before grouping
                var resultdata = query.ToList();

                var totalRecords = _dbcontext.VDealerWorkingDay.Count();
                var filteredRecords = resultdata.Count;

                // Group by dealer and create a single row per dealer with all 7 days (in-memory grouping)
                var groupedData = resultdata
                    .GroupBy(d => new { d.DealerID, d.DealerName, d.DealerCode, d.OEMID, d.OEMName, d.City })
                    .Select(g => new
                    {
                        DealerID = g.Key.DealerID,
                        DealerName = g.Key.DealerName ?? string.Empty,
                        DealerCode = g.Key.DealerCode ?? string.Empty,
                        OEMID = g.Key.OEMID,
                        OEMName = g.Key.OEMName ?? string.Empty,
                        City = g.Key.City ?? string.Empty,
                        Monday = g.FirstOrDefault(d => d.DayOfWeek == 1)?.IsWorking ?? false,
                        Tuesday = g.FirstOrDefault(d => d.DayOfWeek == 2)?.IsWorking ?? false,
                        Wednesday = g.FirstOrDefault(d => d.DayOfWeek == 3)?.IsWorking ?? false,
                        Thursday = g.FirstOrDefault(d => d.DayOfWeek == 4)?.IsWorking ?? false,
                        Friday = g.FirstOrDefault(d => d.DayOfWeek == 5)?.IsWorking ?? false,
                        Saturday = g.FirstOrDefault(d => d.DayOfWeek == 6)?.IsWorking ?? false,
                        Sunday = g.FirstOrDefault(d => d.DayOfWeek == 7)?.IsWorking ?? false,
                        WorkingDayID = g.FirstOrDefault() != null ? g.FirstOrDefault().WorkingDayID : 0
                    })
                    .ToList(); // Materialize before in-memory sorting

                // Apply sorting on in-memory collection
                var validColumns = new[] { "OEMName", "DealerName" };
                var sortColumn = validColumns.Contains(request.SortColumn) ? request.SortColumn : "DealerName";
                var sortDirection = request.SortDirection?.ToLower() == "desc" ? "desc" : "asc";

                IOrderedEnumerable<dynamic> sortedData;
                switch (sortColumn)
                {
                    case "OEMName": sortedData = sortDirection == "asc" ? groupedData.OrderBy(g => g.OEMName) : groupedData.OrderByDescending(g => g.OEMName); break;
                    case "DealerName": sortedData = sortDirection == "asc" ? groupedData.OrderBy(g => g.DealerName) : groupedData.OrderByDescending(g => g.DealerName); break;
                    default: sortedData = groupedData.OrderBy(g => g.DealerName); break;
                }

                var groupedCount = groupedData.Count;
                var pagedData = sortedData.Skip(skip).Take(pageSize).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = groupedCount; // Filtered count (after search, before paging)
                _auditLogger.SaveActionLog("VDealerWorkingDay", ActionType.ListData, null, request, null, "DealerWorkingDayServiceRepository.GetDealerWorkingDayToDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "DealerWorkingDayServiceRepository.GetDealerWorkingDayToDataTable()");
            }
            return response;
        }
        
        public DataResponse GetDealerWorkingDayByDealerID(int DealerID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDealerWorkingDay
                    .Where(w => w.DealerID == DealerID)
                    .OrderBy(o => o.DayOfWeek)
                    .ToList();
                    
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                dataResponse.ID = DealerID;
                dataResponse.Message = Constants.RecordFound;
                _auditLogger.SaveActionLog("VDealerWorkingDay", ActionType.Select, DealerID.ToString(), DealerID, null, "DealerWorkingDayServiceRepository.GetDealerWorkingDayByDealerID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, DealerID, "DealerWorkingDayServiceRepository.GetDealerWorkingDayByDealerID()");
            }

            return dataResponse;
        }
        public DataResponse SaveOrUpdate(int DealerID, List<DealerWorkingDay> DealerWorkingDays)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                // Get existing working days for this dealer
                var existingWorkingDays = _dbcontext.DealerWorkingDay
                    .Where(w => w.DealerID == DealerID && !w.IsDeleted)
                    .ToList();

                foreach (var workingDay in DealerWorkingDays)
                {
                    // Find if record already exists for this dealer and day
                    var existing = existingWorkingDays.FirstOrDefault(w => w.DayOfWeek == workingDay.DayOfWeek);

                    if (existing != null)
                    {
                        // Update existing record
                        existing.IsWorking = workingDay.IsWorking;
                        existing.LastUpdatedBy = workingDay.LastUpdatedBy;
                        existing.LastUpdatedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        // Create new record
                        workingDay.DealerID = DealerID;
                        workingDay.LastUpdatedDate = DateTime.UtcNow;
                        _dbcontext.DealerWorkingDay.Add(workingDay);
                    }
                }

                // Handle days that are not in the new list (should not happen with 7 days, but just in case)
                // This would mean unchecking a day - we'll soft delete it
                var newDayOfWeeks = DealerWorkingDays.Select(s => s.DayOfWeek).ToList();
                var daysToRemove = existingWorkingDays.Where(w => !newDayOfWeeks.Contains(w.DayOfWeek)).ToList();
                foreach (var toRemove in daysToRemove)
                {
                    toRemove.IsDeleted = true;
                    toRemove.LastUpdatedBy = DealerWorkingDays[0].LastUpdatedBy;
                    toRemove.LastUpdatedDate = DateTime.UtcNow;
                }

                _dbcontext.SaveChanges();

                _auditLogger.SaveActionLog("DealerWorkingDay", ActionType.Update, DealerID.ToString(), DealerWorkingDays, null, "DealerWorkingDayServiceRepository.SaveOrUpdate()");
                dataResponse.ID = DealerID;
                dataResponse.Message = Constants.SuccessMessage;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, DealerWorkingDays, "DealerWorkingDayServiceRepository.SaveOrUpdate()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int WorkingDayID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.DealerWorkingDay.FirstOrDefault(w => w.WorkingDayID == WorkingDayID);
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

                dataResponse.ID = dataexists.WorkingDayID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("DealerWorkingDay", ActionType.Delete, null, new { WorkingDayID, UserID, LoginAuditID }, null, "DealerWorkingDayServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, new { WorkingDayID, UserID, LoginAuditID }, "DealerWorkingDayServiceRepository.Delete()");
            }
            return dataResponse;
        }
        public DataResponse DeleteByDealerID(int DealerID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.DealerWorkingDay.Where(w => w.DealerID == DealerID && !w.IsDeleted).ToList();
                if (dataexists == null || dataexists.Count == 0)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }

                foreach (var item in dataexists)
                {
                    item.LastUpdatedDate = DateTime.UtcNow;
                    item.LastUpdatedBy = UserID;
                    item.IsDeleted = true;
                }
                _dbcontext.SaveChanges();

                dataResponse.ID = DealerID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("DealerWorkingDay", ActionType.Delete, null, new { DealerID, UserID, LoginAuditID }, null, "DealerWorkingDayServiceRepository.DeleteByDealerID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, new { DealerID, UserID, LoginAuditID }, "DealerWorkingDayServiceRepository.DeleteByDealerID()");
            }
            return dataResponse;
        }
    }
}

