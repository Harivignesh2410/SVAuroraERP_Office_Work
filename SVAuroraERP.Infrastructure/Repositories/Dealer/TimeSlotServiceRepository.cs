namespace SVAuroraERP.Infrastructure.Repositories.Dealer
{
    public class TimeSlotServiceRepository : ITimeSlotServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<TimeSlotServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public TimeSlotServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<TimeSlotServiceRepository> logger,
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
        public DataResponse GetTimeSlot()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VTimeSlot.OrderBy(o => o.StartTime).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VTimeSlot", ActionType.ListData, null, null, null, "TimeSlotServiceRepository.GetTimeSlot()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "TimeSlotServiceRepository.GetTimeSlot()");
            }

            return dataResponse;
        }
        
        public DataResponse GetTimeSlotToDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VTimeSlot> query = _dbcontext.VTimeSlot;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.SlotName ?? string.Empty).Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VTimeSlot.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting
                var validColumns = new[] { "SlotName", "StartTime", "EndTime" };
                var sortColumn = validColumns.Contains(request.SortColumn) ? request.SortColumn : "StartTime";
                var sortDirection = request.SortDirection?.ToLower() == "desc" ? "desc" : "asc";
                
                query = query.OrderBy($"{sortColumn} {sortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                    .Select(w => new
                    {
                        w.TimeSlotID,
                        w.SlotName,
                        w.StartTime,
                        w.EndTime,
                        w.IsActive,
                        w.LastUpdatedBy,
                        w.LastUpdatedDate,
                        w.LastUpdatedByName
                    }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords; // Filtered count (after search, before paging)
                _auditLogger.SaveActionLog("VTimeSlot", ActionType.ListData, null, request, null, "TimeSlotServiceRepository.GetTimeSlotToDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "TimeSlotServiceRepository.GetTimeSlotToDataTable()");
            }
            return response;
        }
        
        public DataResponse GetTimeSlotByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VTimeSlot.FirstOrDefault(w => w.TimeSlotID == ID);
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
                _auditLogger.SaveActionLog("VTimeSlot", ActionType.Select, ID.ToString(), ID, null, "TimeSlotServiceRepository.GetTimeSlotByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "TimeSlotServiceRepository.GetTimeSlotByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(TimeSlot TimeSlot)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.TimeSlot.FirstOrDefault(r => r.SlotName == TimeSlot.SlotName);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.TimeSlotID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                // Validate that StartTime < EndTime
                if (TimeSlot.StartTime >= TimeSlot.EndTime)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = "Start Time must be less than End Time";
                    return dataResponse;
                }

                TimeSlot.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.TimeSlot.Add(TimeSlot);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("TimeSlot", ActionType.Insert, TimeSlot.TimeSlotID.ToString(), TimeSlot, null, "TimeSlotServiceRepository.Save()");
                dataResponse.ID = TimeSlot.TimeSlotID;
                dataResponse.Message = Constants.SuccessMessage;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, TimeSlot, "TimeSlotServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(TimeSlot TimeSlot)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.TimeSlot.FirstOrDefault(r => r.TimeSlotID != TimeSlot.TimeSlotID && r.SlotName == TimeSlot.SlotName);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.TimeSlotID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                
                // Validate that StartTime < EndTime
                if (TimeSlot.StartTime >= TimeSlot.EndTime)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = "Start Time must be less than End Time";
                    return dataResponse;
                }
                
                var dataexists = _dbcontext.TimeSlot.FirstOrDefault(r => r.TimeSlotID == TimeSlot.TimeSlotID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("TimeSlot", ActionType.Update, dataexists.TimeSlotID.ToString(), TimeSlot, dataexists, "TimeSlotServiceRepository.Update()");
                dataexists.SlotName = TimeSlot.SlotName;
                dataexists.StartTime = TimeSlot.StartTime;
                dataexists.EndTime = TimeSlot.EndTime;
                dataexists.IsActive = TimeSlot.IsActive;
                dataexists.LastUpdatedBy = TimeSlot.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.TimeSlotID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, TimeSlot, "TimeSlotServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int TimeSlotID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.TimeSlot.FirstOrDefault(w => w.TimeSlotID == TimeSlotID);
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

                dataResponse.ID = dataexists.TimeSlotID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("TimeSlot", ActionType.Delete, null, new { TimeSlotID, UserID, LoginAuditID }, null, "TimeSlotServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, new { TimeSlotID, UserID, LoginAuditID }, "TimeSlotServiceRepository.Delete()");
            }
            return dataResponse;
        }
    }
}

