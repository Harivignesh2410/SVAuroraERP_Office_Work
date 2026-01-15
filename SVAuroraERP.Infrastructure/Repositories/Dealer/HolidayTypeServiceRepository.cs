namespace SVAuroraERP.Infrastructure.Repositories.Dealer
{
    public class HolidayTypeServiceRepository : IHolidayTypeServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<HolidayTypeServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public HolidayTypeServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<HolidayTypeServiceRepository> logger,
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
        public DataResponse GetHolidayType()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHolidayType.OrderBy(o => o.TypeName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VHolidayType", ActionType.ListData, null, null, null, "HolidayTypeServiceRepository.GetHolidayType()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HolidayTypeServiceRepository.GetHolidayType()");
            }

            return dataResponse;
        }
        
        public DataResponse GetHolidayTypeToDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHolidayType> query = _dbcontext.VHolidayType;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.TypeName ?? string.Empty).Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VHolidayType.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting
                var validColumns = new[] { "TypeName" };
                var sortColumn = validColumns.Contains(request.SortColumn) ? request.SortColumn : "TypeName";
                var sortDirection = request.SortDirection?.ToLower() == "desc" ? "desc" : "asc";
                
                query = query.OrderBy($"{sortColumn} {sortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                    .Select(w => new
                    {
                        w.HolidayTypeID,
                        w.TypeName,
                        w.IsActive,
                        w.LastUpdatedBy,
                        w.LastUpdatedDate,
                        w.LastUpdatedByName
                    }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords; // Filtered count (after search, before paging)
                _auditLogger.SaveActionLog("VHolidayType", ActionType.ListData, null, request, null, "HolidayTypeServiceRepository.GetHolidayTypeToDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "HolidayTypeServiceRepository.GetHolidayTypeToDataTable()");
            }
            return response;
        }
        
        public DataResponse GetHolidayTypeByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHolidayType.FirstOrDefault(w => w.HolidayTypeID == ID);
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
                _auditLogger.SaveActionLog("VHolidayType", ActionType.Select, ID.ToString(), ID, null, "HolidayTypeServiceRepository.GetHolidayTypeByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "HolidayTypeServiceRepository.GetHolidayTypeByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(HolidayType HolidayType)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.HolidayType.FirstOrDefault(r => r.TypeName == HolidayType.TypeName);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.HolidayTypeID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                HolidayType.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.HolidayType.Add(HolidayType);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("HolidayType", ActionType.Insert, HolidayType.HolidayTypeID.ToString(), HolidayType, null, "HolidayTypeServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HolidayType, "HolidayTypeServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(HolidayType HolidayType)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.HolidayType.FirstOrDefault(r => r.HolidayTypeID != HolidayType.HolidayTypeID && r.TypeName == HolidayType.TypeName);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.HolidayTypeID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.HolidayType.FirstOrDefault(r => r.HolidayTypeID == HolidayType.HolidayTypeID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("HolidayType", ActionType.Update, dataexists.HolidayTypeID.ToString(), HolidayType, dataexists, "HolidayTypeServiceRepository.Update()");
                dataexists.TypeName = HolidayType.TypeName;
                dataexists.IsActive = HolidayType.IsActive;
                dataexists.LastUpdatedBy = HolidayType.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.HolidayTypeID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HolidayType, "HolidayTypeServiceRepository.Update()");
            }


            return dataResponse;
        }
        public DataResponse Delete(int HolidayTypeID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.HolidayType.FirstOrDefault(w => w.HolidayTypeID == HolidayTypeID);
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

                dataResponse.ID = dataexists.HolidayTypeID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("HolidayType", ActionType.Delete, null, new { HolidayTypeID, UserID, LoginAuditID }, null, "HolidayTypeServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, new { HolidayTypeID, UserID, LoginAuditID }, "HolidayTypeServiceRepository.Delete()");
            }
            return dataResponse;
        }
    }
}