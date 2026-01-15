namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class VehiclePlateColorServiceRepository : IVehiclePlateColorServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<VehiclePlateColorServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public VehiclePlateColorServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<VehiclePlateColorServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetVehiclePlateColor()
        {

            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VVehiclePlateColor.OrderBy(o => o.VehiclePlateColorName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VehiclePlateColor", ActionType.ListData, null, null,null, "VehiclePlateColorServiceRepository.GetVehiclePlateColor()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "VehiclePlateColorServiceRepository.GetVehiclePlateColor()");
            }

            return dataResponse;
        }
        public DataResponse GetVehiclePlateColorByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VVehiclePlateColor.FirstOrDefault(w => w.VehiclePlateColorID == ID);
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
                _auditLogger.SaveActionLog("VVehiclePlateColor", ActionType.Select, ID.ToString(), ID, null, "VehiclePlateColorServiceRepository.GetVehiclePlateColorByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "VehiclePlateColorServiceRepository.GetVehiclePlateColorByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(VehiclePlateColor VehiclePlateColor)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.VehiclePlateColor.FirstOrDefault(r => r.VehiclePlateColorName == VehiclePlateColor.VehiclePlateColorName);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.VehiclePlateColorID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                VehiclePlateColor.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.VehiclePlateColor.Add(VehiclePlateColor);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("VehiclePlateColor", ActionType.Insert, VehiclePlateColor.VehiclePlateColorID.ToString(), VehiclePlateColor,null, "VehiclePlateColorServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, VehiclePlateColor, "VehiclePlateColorServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(VehiclePlateColor VehiclePlateColor)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.VehiclePlateColor.FirstOrDefault(r => r.VehiclePlateColorID != VehiclePlateColor.VehiclePlateColorID && r.VehiclePlateColorName  == VehiclePlateColor.VehiclePlateColorName);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.VehiclePlateColorID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.VehiclePlateColor.FirstOrDefault(r => r.VehiclePlateColorID == VehiclePlateColor.VehiclePlateColorID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("VehiclePlateColor", ActionType.Update, dataexists.VehiclePlateColorID.ToString(), VehiclePlateColor, dataexists, "VehiclePlateColorServiceRepository.Update()");
                dataexists.VehiclePlateColorName = VehiclePlateColor.VehiclePlateColorName;
                dataexists.VehiclePlateColorCode = VehiclePlateColor.VehiclePlateColorCode;
                dataexists.IsActive = VehiclePlateColor.IsActive;
                dataexists.LastUpdatedBy = VehiclePlateColor.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.VehiclePlateColorID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, VehiclePlateColor, "VehiclePlateColorServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int VehiclePlateColorID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.VehiclePlateColor.FirstOrDefault(w => w.VehiclePlateColorID == VehiclePlateColorID);
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

                dataResponse.ID = dataexists.VehiclePlateColorID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("VehiclePlateColor", ActionType.Delete, null, VehiclePlateColorID,null, "VehiclePlateColorServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, VehiclePlateColorID, "VehiclePlateColorServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetVehiclePlateColorDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VVehiclePlateColor> query = _dbcontext.VVehiclePlateColor;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.VehiclePlateColorName ?? "").Contains(request.SearchValue)
                    || (d.VehiclePlateColorCode ?? "").Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VVehiclePlateColor.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.VehiclePlateColorID,
                                w.VehiclePlateColorName,
                                w.VehiclePlateColorCode,
                                w.IsActive
                            }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VehiclePlateColor", ActionType.Select, null, request, null, "VehiclePlateColorServiceRepository.GetVehiclePlateColorDataTableList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "VehiclePlateColorServiceRepository.GetVehiclePlateColorDataTableList()");
            }
            return response;
        }
    }
}