namespace SVAuroraERP.Infrastructure.Repositories.Dealer
{
    public class DealerSlotConfigServiceRepository : IDealerSlotConfigServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<DealerSlotConfigServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public DealerSlotConfigServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<DealerSlotConfigServiceRepository> logger,
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
        public DataResponse GetDealerSlotConfig()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDealerSlotConfig.OrderBy(o => o.DealerName).ThenByDescending(o => o.SlotDate).ThenBy(o => o.StartTime).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VDealerSlotConfig", ActionType.ListData, null, null, null, "DealerSlotConfigServiceRepository.GetDealerSlotConfig()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "DealerSlotConfigServiceRepository.GetDealerSlotConfig()");
            }

            return dataResponse;
        }
        
        public DataResponse GetDealerSlotConfigToDataTable(DealerSlotConfigDataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VDealerSlotConfig> query = _dbcontext.VDealerSlotConfig;

                // Apply filters at database level
                if (request.OEMID.HasValue && request.OEMID.Value > 0)
                {
                    query = query.Where(w => w.OEMID == request.OEMID.Value);
                }

                if (request.DealerID.HasValue && request.DealerID.Value > 0)
                {
                    query = query.Where(w => w.DealerID == request.DealerID.Value);
                }

                if (request.FromDate.HasValue)
                {
                    query = query.Where(w => w.SlotDate.Date >= request.FromDate.Value.Date);
                }

                if (request.ToDate.HasValue)
                {
                    query = query.Where(w => w.SlotDate.Date <= request.ToDate.Value.Date);
                }

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => 
                        (d.OEMName ?? string.Empty).Contains(request.SearchValue) ||
                        (d.DealerName ?? string.Empty).Contains(request.SearchValue) ||
                        (d.DealerCode ?? string.Empty).Contains(request.SearchValue) ||
                        (d.City ?? string.Empty).Contains(request.SearchValue) ||
                        (d.sSlotDate ?? string.Empty).Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VDealerSlotConfig.Count();

                // Get FILTERED records count (after filters but before grouping)
                var filteredRecords = query.Count();

                // Group by DealerID and SlotDate - materialize first to avoid EF issues
                var groupedData = query
                    .GroupBy(d => new { d.DealerID, d.SlotDate.Date, d.DealerName, d.DealerCode, d.City, d.OEMID, d.OEMName })
                    .Select(g => new
                    {
                        DealerID = g.Key.DealerID,
                        DealerName = g.Key.DealerName ?? string.Empty,
                        DealerCode = g.Key.DealerCode ?? string.Empty,
                        City = g.Key.City ?? string.Empty,
                        OEMID = g.Key.OEMID,
                        OEMName = g.Key.OEMName ?? string.Empty,
                        SlotDate = g.Key.Date,
                        sSlotDate = g.FirstOrDefault() != null ? (g.FirstOrDefault().sSlotDate ?? g.Key.Date.ToString("dd/MM/yyyy")) : g.Key.Date.ToString("dd/MM/yyyy"),
                        TotalTimeSlots = g.Count(),
                        TotalCapacity = g.Sum(s => s.MaxCapacity),
                        ActiveTimeSlots = g.Count(s => s.IsActive),
                        ConfigID = g.FirstOrDefault() != null ? g.FirstOrDefault().ConfigID : 0,
                        IsActive = g.All(s => s.IsActive)
                    })
                    .ToList();

                // Apply sorting on in-memory collection
                var validColumns = new[] { "OEMName", "DealerName", "SlotDate", "TotalCapacity" };
                var sortColumn = validColumns.Contains(request.SortColumn) ? request.SortColumn : "SlotDate";
                var sortDirection = request.SortDirection?.ToLower() == "desc" ? "desc" : "asc";
                
                IEnumerable<dynamic> sortedData;
                switch (sortColumn)
                {
                    case "OEMName":
                        sortedData = sortDirection == "asc" 
                            ? groupedData.OrderBy(g => g.OEMName)
                            : groupedData.OrderByDescending(g => g.OEMName);
                        break;
                    case "DealerName":
                        sortedData = sortDirection == "asc" 
                            ? groupedData.OrderBy(g => g.DealerName)
                            : groupedData.OrderByDescending(g => g.DealerName);
                        break;
                    case "SlotDate":
                        sortedData = sortDirection == "asc" 
                            ? groupedData.OrderBy(g => g.SlotDate)
                            : groupedData.OrderByDescending(g => g.SlotDate);
                        break;
                    case "TotalCapacity":
                        sortedData = sortDirection == "asc" 
                            ? groupedData.OrderBy(g => g.TotalCapacity)
                            : groupedData.OrderByDescending(g => g.TotalCapacity);
                        break;
                    default:
                        sortedData = groupedData.OrderBy(g => g.SlotDate);
                        break;
                }

                // Get total grouped records count
                var groupedCount = groupedData.Count;

                // Apply paging
                var pagedData = sortedData.Skip(skip).Take(pageSize).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords; // Total records before grouping
                response.recordsFiltered = groupedCount; // Filtered grouped records count
                _auditLogger.SaveActionLog("VDealerSlotConfig", ActionType.ListData, null, request, null, "DealerSlotConfigServiceRepository.GetDealerSlotConfigToDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "DealerSlotConfigServiceRepository.GetDealerSlotConfigToDataTable()");
            }
            return response;
        }
        
        public DataResponse GetDealerSlotConfig(int? OEMID, int? DealerID, DateTime? FromDate, DateTime? ToDate)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                IQueryable<VDealerSlotConfig> query = _dbcontext.VDealerSlotConfig;
                
                // Apply filters at database level
                if (OEMID.HasValue && OEMID.Value > 0)
                {
                    query = query.Where(w => w.OEMID == OEMID.Value);
                }
                
                if (DealerID.HasValue && DealerID.Value > 0)
                {
                    query = query.Where(w => w.DealerID == DealerID.Value);
                }
                
                if (FromDate.HasValue)
                {
                    query = query.Where(w => w.SlotDate.Date >= FromDate.Value.Date);
                }
                
                if (ToDate.HasValue)
                {
                    query = query.Where(w => w.SlotDate.Date <= ToDate.Value.Date);
                }
                
                // Apply ordering and materialize
                var resultdata = query.OrderBy(o => o.DealerName).ThenByDescending(o => o.SlotDate).ThenBy(o => o.StartTime).ToList();
                
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VDealerSlotConfig", ActionType.ListData, null, null, null, "DealerSlotConfigServiceRepository.GetDealerSlotConfig()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "DealerSlotConfigServiceRepository.GetDealerSlotConfig()");
            }

            return dataResponse;
        }
        public DataResponse GetDealerSlotConfigByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDealerSlotConfig.FirstOrDefault(w => w.ConfigID == ID);
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
                _auditLogger.SaveActionLog("VDealerSlotConfig", ActionType.Select, ID.ToString(), ID, null, "DealerSlotConfigServiceRepository.GetDealerSlotConfigByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "DealerSlotConfigServiceRepository.GetDealerSlotConfigByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(DealerSlotConfig DealerSlotConfig)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                // Check if configuration already exists for this dealer, date, and time slot
                var dataexists = _dbcontext.DealerSlotConfig.FirstOrDefault(r => 
                    r.DealerID == DealerSlotConfig.DealerID && 
                    r.SlotDate.Date == DealerSlotConfig.SlotDate.Date && 
                    r.TimeSlotID == DealerSlotConfig.TimeSlotID &&
                    !r.IsDeleted);
                    
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.ConfigID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                // Validate MaxCapacity > 0
                if (DealerSlotConfig.MaxCapacity <= 0)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = "Max Capacity must be greater than 0";
                    return dataResponse;
                }

                DealerSlotConfig.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.DealerSlotConfig.Add(DealerSlotConfig);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("DealerSlotConfig", ActionType.Insert, DealerSlotConfig.ConfigID.ToString(), DealerSlotConfig, null, "DealerSlotConfigServiceRepository.Save()");
                dataResponse.ID = DealerSlotConfig.ConfigID;
                dataResponse.Message = Constants.SuccessMessage;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, DealerSlotConfig, "DealerSlotConfigServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(DealerSlotConfig DealerSlotConfig)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Check if configuration already exists for this dealer, date, and time slot (excluding current record)
                var isFound = _dbcontext.DealerSlotConfig.FirstOrDefault(r => 
                    r.ConfigID != DealerSlotConfig.ConfigID &&
                    r.DealerID == DealerSlotConfig.DealerID && 
                    r.SlotDate.Date == DealerSlotConfig.SlotDate.Date && 
                    r.TimeSlotID == DealerSlotConfig.TimeSlotID &&
                    !r.IsDeleted);
                    
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.ConfigID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                
                // Validate MaxCapacity > 0
                if (DealerSlotConfig.MaxCapacity <= 0)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = "Max Capacity must be greater than 0";
                    return dataResponse;
                }
                
                var dataexists = _dbcontext.DealerSlotConfig.FirstOrDefault(r => r.ConfigID == DealerSlotConfig.ConfigID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("DealerSlotConfig", ActionType.Update, dataexists.ConfigID.ToString(), DealerSlotConfig, dataexists, "DealerSlotConfigServiceRepository.Update()");
                dataexists.DealerID = DealerSlotConfig.DealerID;
                dataexists.TimeSlotID = DealerSlotConfig.TimeSlotID;
                dataexists.SlotDate = DealerSlotConfig.SlotDate;
                dataexists.MaxCapacity = DealerSlotConfig.MaxCapacity;
                dataexists.IsActive = DealerSlotConfig.IsActive;
                dataexists.LastUpdatedBy = DealerSlotConfig.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.ConfigID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, DealerSlotConfig, "DealerSlotConfigServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int ConfigID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.DealerSlotConfig.FirstOrDefault(w => w.ConfigID == ConfigID);
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

                dataResponse.ID = dataexists.ConfigID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("DealerSlotConfig", ActionType.Delete, null, new { ConfigID, UserID, LoginAuditID }, null, "DealerSlotConfigServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, new { ConfigID, UserID, LoginAuditID }, "DealerSlotConfigServiceRepository.Delete()");
            }
            return dataResponse;
        }
    }
}

