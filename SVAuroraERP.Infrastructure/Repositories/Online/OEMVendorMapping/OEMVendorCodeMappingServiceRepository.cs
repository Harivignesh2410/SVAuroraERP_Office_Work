using Microsoft.EntityFrameworkCore;

namespace SVAuroraERP.Infrastructure.Repositories.Online.OEMVendorMapping
{
    public class OEMVendorCodeMappingServiceRepository : IOEMVendorCodeMappingServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbContext;
        private readonly ILogger<OEMVendorCodeMappingServiceRepository> _logger;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public OEMVendorCodeMappingServiceRepository(SVAuroraERPDbContext dbContext,
                                     ILogger<OEMVendorCodeMappingServiceRepository> logger,
                                     IAuditLogger auditLogger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _auditLogger = auditLogger;
        }
        public DataResponse GetOEMVendorCodeMapping()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbContext.VOEMVendorCodeMapping.OrderBy(o => o.VendorCode).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("MapPlateColor", ActionType.ListData, null, null,null, "OEMVendorCodeMappingServiceRepository.GetOEMVendorCodeMapping()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "OEMVendorCodeMappingServiceRepository.GetOEMVendorCodeMapping()");
            }

            return dataResponse;
        }
        public DataResponse GetOEMVendorCodeMappingByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbContext.VOEMVendorCodeMapping.FirstOrDefault(w => w.OEMVendorCodeMappingID == ID);
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
                _auditLogger.SaveActionLog("MapPlateColor", ActionType.Select,ID.ToString(), ID, null, "OEMVendorCodeMappingServiceRepository.GetOEMVendorCodeMappingByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "OEMVendorCodeMappingServiceRepository.GetOEMVendorCodeMappingByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(OEMVendorCodeMapping OEMVendorCodeMapping)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbContext.OEMVendorCodeMapping.FirstOrDefault(r => r.VendorCode == OEMVendorCodeMapping.VendorCode);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.OEMVendorCodeMappingID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                OEMVendorCodeMapping.LastUpdatedDate = DateTime.UtcNow;
                _dbContext.OEMVendorCodeMapping.Add(OEMVendorCodeMapping);
                _dbContext.SaveChanges();

                _auditLogger.SaveActionLog("OEMVendorCodeMapping", ActionType.Insert, null, OEMVendorCodeMapping, null, "OEMVendorCodeMappingServiceRepository.Save()");

            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, OEMVendorCodeMapping, "OEMVendorCodeMappingServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(OEMVendorCodeMapping OEMVendorCodeMapping)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbContext.OEMVendorCodeMapping.FirstOrDefault(r => r.OEMVendorCodeMappingID != OEMVendorCodeMapping.OEMVendorCodeMappingID
                                                                    && r.VendorCode == OEMVendorCodeMapping.VendorCode);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.OEMVendorCodeMappingID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbContext.OEMVendorCodeMapping.FirstOrDefault(r => r.OEMVendorCodeMappingID == OEMVendorCodeMapping.OEMVendorCodeMappingID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("OEMVendorCodeMapping", ActionType.Update, OEMVendorCodeMapping.OEMVendorCodeMappingID.ToString(), OEMVendorCodeMapping, dataexists, "VehiclePlateSizeServiceRepository.Update()");

                dataexists.HSRPOEMID = OEMVendorCodeMapping.HSRPOEMID;
                dataexists.VendorCode = OEMVendorCodeMapping.VendorCode;
                dataexists.DistrictID = OEMVendorCodeMapping.DistrictID;
                dataexists.IsActive = OEMVendorCodeMapping.IsActive;
                dataexists.LastUpdatedBy = OEMVendorCodeMapping.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbContext.SaveChanges();
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, OEMVendorCodeMapping, "VehiclePlateSizeServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int OEMVendorCodeMappingID, int UserID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbContext.OEMVendorCodeMapping.FirstOrDefault(w => w.OEMVendorCodeMappingID == OEMVendorCodeMappingID);
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
                _dbContext.SaveChanges();

                dataResponse.ID = dataexists.OEMVendorCodeMappingID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("OEMVendorCodeMapping", ActionType.Delete, OEMVendorCodeMappingID.ToString(), new { OEMVendorCodeMappingID , UserID }, null, "VehiclePlateSizeServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, new { OEMVendorCodeMappingID, UserID }, "VehiclePlateSizeServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetOEMVendorCodeMappingDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VOEMVendorCodeMapping> query = _dbContext.VOEMVendorCodeMapping;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.VendorCode ?? "").Contains(request.SearchValue)
                    || (d.StateName ?? "").Contains(request.SearchValue)
                     || (d.DistrictName ?? "").Contains(request.SearchValue)
                     || (d.OEMName ?? "").Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbContext.VOEMVendorCodeMapping.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.OEMVendorCodeMappingID,
                                w.CompanyName,
                                w.VendorCode,
                                w.StateName,
                                w.DistrictName,
                                w.IsActive
                            }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("State", ActionType.Select, null, request, null, "StateServiceRepository.GetStateDataTableList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "StateServiceRepository.GetStateDataTableList()");
            }
            return response;
        }
    }
}