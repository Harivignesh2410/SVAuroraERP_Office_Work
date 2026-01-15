using SVAuroraERP.Domain.Online.Master;

namespace SVAuroraERP.Infrastructure.Repositories.Dealer
{
    public class DealerHolidayServiceRepository : IDealerHolidayServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<DealerHolidayServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public DealerHolidayServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<DealerHolidayServiceRepository> logger,
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
        public DataResponse GetDealerHoliday()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDealerHoliday.OrderByDescending(o => o.HolidayDate).ToList();
                
                // Get HolidayTypes for each DealerHoliday
                foreach (var holiday in resultdata)
                {
                    var holidayTypes = _dbcontext.DealerHolidayType
                        .Where(w => w.DealerHolidayID == holiday.DealerHolidayID && !w.IsDeleted && w.IsEnabled)
                        .Join(_dbcontext.VHolidayType,
                            dht => dht.HolidayTypeID,
                            ht => ht.HolidayTypeID,
                            (dht, ht) => ht.TypeName)
                        .ToList();
                    holiday.HolidayTypes = string.Join(", ", holidayTypes);
                    
                    holiday.HolidayTypeIDs = _dbcontext.DealerHolidayType
                        .Where(w => w.DealerHolidayID == holiday.DealerHolidayID && !w.IsDeleted && w.IsEnabled)
                        .Select(s => s.HolidayTypeID)
                        .ToList();
                }
                
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VDealerHoliday", ActionType.ListData, null, null, null, "DealerHolidayServiceRepository.GetDealerHoliday()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "DealerHolidayServiceRepository.GetDealerHoliday()");
            }

            return dataResponse;
        }
        
        public DataResponse GetDealerHolidayToDataTable(DealerHolidayDataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VDealerHoliday> query = _dbcontext.VDealerHoliday;

                // Join with VHSRPUser to get OEM information if not in view
                var queryWithOEM = from holiday in query
                                   join dealer in _dbcontext.VHSRPUser on holiday.DealerID equals dealer.HSRPUserID
                                   select new
                                   {
                                       Holiday = holiday,
                                       OEMID = dealer.OEMID,
                                       OEMName = dealer.OEMName ?? string.Empty,
                                       DealerCode = dealer.HSRPUserCode ?? string.Empty,
                                       City = dealer.City ?? string.Empty
                                   };

                // Apply filters at database level
                if (request.OEMID.HasValue && request.OEMID.Value > 0)
                {
                    queryWithOEM = queryWithOEM.Where(w => w.OEMID == request.OEMID.Value);
                }

                if (request.DealerID.HasValue && request.DealerID.Value > 0)
                {
                    queryWithOEM = queryWithOEM.Where(w => w.Holiday.DealerID == request.DealerID.Value);
                }

                if (request.FromDate.HasValue)
                {
                    queryWithOEM = queryWithOEM.Where(w => w.Holiday.HolidayDate.Date >= request.FromDate.Value.Date);
                }

                if (request.ToDate.HasValue)
                {
                    queryWithOEM = queryWithOEM.Where(w => w.Holiday.HolidayDate.Date <= request.ToDate.Value.Date);
                }

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    queryWithOEM = queryWithOEM.Where(d => 
                        (d.OEMName ?? string.Empty).Contains(request.SearchValue) ||
                        (d.Holiday.DealerName ?? string.Empty).Contains(request.SearchValue) ||
                        (d.DealerCode ?? string.Empty).Contains(request.SearchValue) ||
                        (d.City ?? string.Empty).Contains(request.SearchValue) ||
                        (d.Holiday.Reason ?? string.Empty).Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VDealerHoliday.Count();

                // Get FILTERED records count
                var filteredRecords = queryWithOEM.Count();

                // Materialize and get HolidayTypes
                var materializedData = queryWithOEM.ToList();
                
                foreach (var item in materializedData)
                {
                    var holidayTypes = _dbcontext.DealerHolidayType
                        .Where(w => w.DealerHolidayID == item.Holiday.DealerHolidayID && !w.IsDeleted && w.IsEnabled)
                        .Join(_dbcontext.VHolidayType,
                            dht => dht.HolidayTypeID,
                            ht => ht.HolidayTypeID,
                            (dht, ht) => ht.TypeName)
                        .ToList();
                    item.Holiday.HolidayTypes = string.Join(", ", holidayTypes);
                    item.Holiday.OEMID = item.OEMID;
                    item.Holiday.OEMName = item.OEMName;
                    if (string.IsNullOrWhiteSpace(item.Holiday.sHolidayDate))
                    {
                        item.Holiday.sHolidayDate = item.Holiday.HolidayDate.ToString("dd/MM/yyyy");
                    }
                }

                // Apply sorting
                var validColumns = new[] { "OEMName", "DealerName", "HolidayDate", "Reason" };
                var sortColumn = validColumns.Contains(request.SortColumn) ? request.SortColumn : "HolidayDate";
                var sortDirection = request.SortDirection?.ToLower() == "desc" ? "desc" : "asc";
                
                IEnumerable<dynamic> sortedData;
                switch (sortColumn)
                {
                    case "OEMName":
                        sortedData = sortDirection == "asc" 
                            ? materializedData.OrderBy(g => g.OEMName)
                            : materializedData.OrderByDescending(g => g.OEMName);
                        break;
                    case "DealerName":
                        sortedData = sortDirection == "asc" 
                            ? materializedData.OrderBy(g => g.Holiday.DealerName)
                            : materializedData.OrderByDescending(g => g.Holiday.DealerName);
                        break;
                    case "HolidayDate":
                        sortedData = sortDirection == "asc" 
                            ? materializedData.OrderBy(g => g.Holiday.HolidayDate)
                            : materializedData.OrderByDescending(g => g.Holiday.HolidayDate);
                        break;
                    case "Reason":
                        sortedData = sortDirection == "asc" 
                            ? materializedData.OrderBy(g => g.Holiday.Reason ?? string.Empty)
                            : materializedData.OrderByDescending(g => g.Holiday.Reason ?? string.Empty);
                        break;
                    default:
                        sortedData = materializedData.OrderByDescending(g => g.Holiday.HolidayDate);
                        break;
                }

                // Apply paging
                var pagedData = sortedData.Skip(skip).Take(pageSize)
                    .Select(s => new
                    {
                        s.Holiday.DealerHolidayID,
                        s.Holiday.DealerID,
                        DealerName = s.Holiday.DealerName ?? string.Empty,
                        DealerCode = s.DealerCode,
                        City = s.City,
                        OEMID = s.OEMID,
                        OEMName = s.OEMName,
                        HolidayDate = s.Holiday.HolidayDate,
                        sHolidayDate = s.Holiday.sHolidayDate,
                        Reason = s.Holiday.Reason ?? string.Empty,
                        HolidayTypes = s.Holiday.HolidayTypes ?? string.Empty,
                        IsFullDay = s.Holiday.IsFullDay,
                        LastUpdatedBy = s.Holiday.LastUpdatedBy,
                        LastUpdatedDate = s.Holiday.LastUpdatedDate,
                        LastUpdatedByName = s.Holiday.LastUpdatedByName ?? string.Empty
                    })
                    .ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VDealerHoliday", ActionType.ListData, null, request, null, "DealerHolidayServiceRepository.GetDealerHolidayToDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "DealerHolidayServiceRepository.GetDealerHolidayToDataTable()");
            }
            return response;
        }
        
        public DataResponse GetDealerHolidayByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDealerHoliday.FirstOrDefault(w => w.DealerHolidayID == ID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                
                // Get OEM information from dealer
                var dealer = _dbcontext.VHSRPUser.FirstOrDefault(d => d.HSRPUserID == resultdata.DealerID);
                if (dealer != null)
                {
                    resultdata.OEMID = dealer.OEMID;
                    resultdata.OEMName = dealer.OEMName ?? string.Empty;
                    if (string.IsNullOrWhiteSpace(resultdata.DealerCode))
                    {
                        resultdata.DealerCode = dealer.HSRPUserCode ?? string.Empty;
                    }
                    if (string.IsNullOrWhiteSpace(resultdata.City))
                    {
                        resultdata.City = dealer.City ?? string.Empty;
                    }
                }
                
                // Format sHolidayDate if not set
                if (string.IsNullOrWhiteSpace(resultdata.sHolidayDate))
                {
                    resultdata.sHolidayDate = resultdata.HolidayDate.ToString("dd/MM/yyyy");
                }
                
                // Get HolidayTypeIDs
                resultdata.HolidayTypeIDs = _dbcontext.DealerHolidayType
                    .Where(w => w.DealerHolidayID == ID && !w.IsDeleted && w.IsEnabled)
                    .Select(s => s.HolidayTypeID)
                    .ToList();
                
                dataResponse.ID = ID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VDealerHoliday", ActionType.Select, ID.ToString(), ID, null, "DealerHolidayServiceRepository.GetDealerHolidayByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "DealerHolidayServiceRepository.GetDealerHolidayByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(DealerHoliday DealerHoliday, List<DealerHolidayType> DealerHolidayTypes)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                // Check if holiday already exists for this dealer and date
                var dataexists = _dbcontext.DealerHoliday.FirstOrDefault(r => r.DealerID == DealerHoliday.DealerID && r.HolidayDate.Date == DealerHoliday.HolidayDate.Date);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.DealerHolidayID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                DealerHoliday.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.DealerHoliday.Add(DealerHoliday);
                _dbcontext.SaveChanges();

                // Add HolidayTypes
                if (DealerHolidayTypes != null && DealerHolidayTypes.Count > 0)
                {
                    foreach (var holidayType in DealerHolidayTypes)
                    {
                        holidayType.DealerHolidayID = DealerHoliday.DealerHolidayID;
                        holidayType.IsEnabled = true;
                        holidayType.LastUpdatedDate = DateTime.UtcNow;
                        _dbcontext.DealerHolidayType.Add(holidayType);
                    }
                    _dbcontext.SaveChanges();
                }

                _auditLogger.SaveActionLog("DealerHoliday", ActionType.Insert, DealerHoliday.DealerHolidayID.ToString(), DealerHoliday, null, "DealerHolidayServiceRepository.Save()");
                dataResponse.ID = DealerHoliday.DealerHolidayID;
                dataResponse.Message = Constants.SuccessMessage;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, DealerHoliday, "DealerHolidayServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(DealerHoliday DealerHoliday, List<DealerHolidayType> DealerHolidayTypes)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Check if holiday already exists for this dealer and date (excluding current record)
                var isFound = _dbcontext.DealerHoliday.FirstOrDefault(r => r.DealerHolidayID != DealerHoliday.DealerHolidayID && r.DealerID == DealerHoliday.DealerID && r.HolidayDate.Date == DealerHoliday.HolidayDate.Date);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.DealerHolidayID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                
                var dataexists = _dbcontext.DealerHoliday.FirstOrDefault(r => r.DealerHolidayID == DealerHoliday.DealerHolidayID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("DealerHoliday", ActionType.Update, dataexists.DealerHolidayID.ToString(), DealerHoliday, dataexists, "DealerHolidayServiceRepository.Update()");
                
                dataexists.DealerID = DealerHoliday.DealerID;
                dataexists.HolidayDate = DealerHoliday.HolidayDate;
                dataexists.Reason = DealerHoliday.Reason;
                dataexists.IsFullDay = DealerHoliday.IsFullDay;
                dataexists.LastUpdatedBy = DealerHoliday.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();

                // Delete existing HolidayTypes (soft delete)
                var existingHolidayTypes = _dbcontext.DealerHolidayType
                    .Where(w => w.DealerHolidayID == DealerHoliday.DealerHolidayID && !w.IsDeleted)
                    .ToList();
                foreach (var existing in existingHolidayTypes)
                {
                    existing.IsDeleted = true;
                    existing.LastUpdatedBy = DealerHoliday.LastUpdatedBy;
                    existing.LastUpdatedDate = DateTime.UtcNow;
                }
                _dbcontext.SaveChanges();

                // Add new HolidayTypes
                if (DealerHolidayTypes != null && DealerHolidayTypes.Count > 0)
                {
                    foreach (var holidayType in DealerHolidayTypes)
                    {
                        holidayType.DealerHolidayID = DealerHoliday.DealerHolidayID;
                        holidayType.IsEnabled = true;
                        holidayType.LastUpdatedDate = DateTime.UtcNow;
                        _dbcontext.DealerHolidayType.Add(holidayType);
                    }
                    _dbcontext.SaveChanges();
                }
                
                dataResponse.ID = dataexists.DealerHolidayID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, DealerHoliday, "DealerHolidayServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int DealerHolidayID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.DealerHoliday.FirstOrDefault(w => w.DealerHolidayID == DealerHolidayID);
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

                // Soft delete related HolidayTypes
                var holidayTypes = _dbcontext.DealerHolidayType
                    .Where(w => w.DealerHolidayID == DealerHolidayID && !w.IsDeleted)
                    .ToList();
                foreach (var holidayType in holidayTypes)
                {
                    holidayType.IsDeleted = true;
                    holidayType.LastUpdatedBy = UserID;
                    holidayType.LastUpdatedDate = DateTime.UtcNow;
                }
                _dbcontext.SaveChanges();

                dataResponse.ID = dataexists.DealerHolidayID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("DealerHoliday", ActionType.Delete, null, new { DealerHolidayID, UserID, LoginAuditID }, null, "DealerHolidayServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, new { DealerHolidayID, UserID, LoginAuditID }, "DealerHolidayServiceRepository.Delete()");
            }
            return dataResponse;
        }
    }
}

