namespace SVAuroraERP.Infrastructure.Repositories.Online.OEMVendorMapping
{
    public class OEMVendorDealerMappingServiceRepository : IOEMVendorDealerMappingServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbContext;
        private readonly ILogger<OEMVendorDealerMappingServiceRepository> _logger;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public OEMVendorDealerMappingServiceRepository(SVAuroraERPDbContext dbContext,
                                     ILogger<OEMVendorDealerMappingServiceRepository> logger,
                                     IAuditLogger auditLogger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _auditLogger = auditLogger;
        }
        public DataResponse GetOEMVendorDealerMapping()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbContext.VOEMVendorDealerMapping.OrderBy(o => o.VendorCode).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VOEMVendorDealerMapping", ActionType.ListData, null, null,null, "OEMVendorDealerMappingServiceRepository.GetOEMVendorDealerMapping()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "OEMVendorDealerMappingServiceRepository.GetOEMVendorDealerMapping()");
            }

            return dataResponse;
        }
        public DataResponse GetOEMVendorDealerMappingByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbContext.VOEMVendorDealerMapping.FirstOrDefault(w => w.OEMVendorDealerMappingID == ID);
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
                _auditLogger.SaveActionLog("VOEMVendorDealerMapping", ActionType.Select, ID.ToString(), ID, null, "OEMVendorDealerMappingServiceRepository.GetOEMVendorDealerMappingByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "OEMVendorDealerMappingServiceRepository.GetOEMVendorDealerMappingByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(OEMVendorDealerMapping OEMVendorDealerMapping)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbContext.OEMVendorDealerMapping.FirstOrDefault(r => r.DealerID == OEMVendorDealerMapping.DealerID
                                                                                       && r.EmbossingStationID == OEMVendorDealerMapping.EmbossingStationID
                                                                                       && r.OEMVendorCodeMappingID == OEMVendorDealerMapping.OEMVendorCodeMappingID
                                                                                       );
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.OEMVendorDealerMappingID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                OEMVendorDealerMapping.LastUpdatedDate = DateTime.UtcNow;
                _dbContext.OEMVendorDealerMapping.Add(OEMVendorDealerMapping);
                _dbContext.SaveChanges();
                _auditLogger.SaveActionLog("OEMVendorDealerMapping", ActionType.Insert, null, OEMVendorDealerMapping, null, "OEMVendorDealerMappingServiceRepository.Save()");

            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, OEMVendorDealerMapping, "OEMVendorDealerMappingServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(OEMVendorDealerMapping OEMVendorDealerMapping)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbContext.OEMVendorDealerMapping.FirstOrDefault(r => r.OEMVendorDealerMappingID != OEMVendorDealerMapping.OEMVendorDealerMappingID
                                                                    && r.DealerID == OEMVendorDealerMapping.DealerID && r.EmbossingStationID == OEMVendorDealerMapping.EmbossingStationID
                                                                    && r.OEMVendorCodeMappingID == OEMVendorDealerMapping.OEMVendorCodeMappingID);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.OEMVendorDealerMappingID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbContext.OEMVendorDealerMapping.FirstOrDefault(r => r.OEMVendorDealerMappingID == OEMVendorDealerMapping.OEMVendorDealerMappingID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("OEMVendorDealerMapping", ActionType.Update, OEMVendorDealerMapping.OEMVendorDealerMappingID.ToString(), OEMVendorDealerMapping, dataexists, "VehiclePlateSizeServiceRepository.Update()");

                dataexists.DealerID = OEMVendorDealerMapping.DealerID;
                dataexists.OEMVendorCodeMappingID = OEMVendorDealerMapping.OEMVendorCodeMappingID;
                dataexists.EmbossingStationID = OEMVendorDealerMapping.EmbossingStationID;
                dataexists.IsActive = OEMVendorDealerMapping.IsActive;
                dataexists.LastUpdatedBy = OEMVendorDealerMapping.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbContext.SaveChanges();
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, OEMVendorDealerMapping, "VehiclePlateSizeServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int OEMVendorDealerMappingID, int UserID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbContext.OEMVendorDealerMapping.FirstOrDefault(w => w.OEMVendorDealerMappingID == OEMVendorDealerMappingID);
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

                dataResponse.ID = dataexists.OEMVendorDealerMappingID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("OEMVendorDealerMapping", ActionType.Delete, OEMVendorDealerMappingID.ToString(), new { OEMVendorDealerMappingID , UserID }, null, "VehiclePlateSizeServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, OEMVendorDealerMappingID, "VehiclePlateSizeServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetDealerByOEMID(int OEMID)
        {

            DataResponse response = new DataResponse();
            try
            {
                var Dealer = _dbContext.VHSRPUser.Where(w => w.OEMID == OEMID && w.IsActive == true && w.HSRPUserTypeID == 4).ToList();

                response.Count = Dealer.Count;
                response.Value = Dealer;
                response.ID = OEMID;
                _auditLogger.SaveActionLog("VHSRPUser", ActionType.ListData, null, null, null, "VehiclePlateSizeServiceRepository.GetDealerByOEMID()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, OEMID, "VehiclePlateSizeServiceRepository.GetDealerByOEMID()");
            }

            return response;
        }
        // public DataResponse GetEmbossingStationByDealerID(int DealerID)
        public DataResponse GetEmbossingStationByDealerID()
        {

            DataResponse response = new DataResponse();
            try
            {
                // var EmbossingStation = _dbContext.VHSRPUser.Where(w => w.HSRPUserID == DealerID && w.IsActive == true && w.HSRPUserTypeID == 2).ToList();
                var EmbossingStation = _dbContext.VHSRPUser.Where(w => w.IsActive == true && w.HSRPUserTypeID == 2).ToList();

                response.Count = EmbossingStation.Count;
                response.Value = EmbossingStation;
                //response.ID = DealerID;
                _auditLogger.SaveActionLog("VHSRPUser", ActionType.ListData, null, null,null, "VehiclePlateSizeServiceRepository.GetEmbossingStationByDealerID()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "VehiclePlateSizeServiceRepository.GetEmbossingStationByDealerID()");
            }

            return response;

        }
        public DataResponse GetVendorCodeByEmbossingStationID(int OEMID)
        {

            DataResponse response = new DataResponse();
            try
            {
                var EmbossingStation = _dbContext.VOEMVendorCodeMapping.Where(w => w.HSRPOEMID == OEMID).ToList();

                response.Count = EmbossingStation.Count;
                response.Value = EmbossingStation;
                response.ID = OEMID;
                _auditLogger.SaveActionLog("VOEMVendorCodeMapping", ActionType.Select, OEMID.ToString(), OEMID, null, "VehiclePlateSizeServiceRepository.GetVendorCodeByEmbossingStationID()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "VehiclePlateSizeServiceRepository.GetVendorCodeByEmbossingStationID()");
            }
            return response;
        }
        public DataResponse GetOEMVendorDealerMappingDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VOEMVendorDealerMapping> query = _dbContext.VOEMVendorDealerMapping;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.OEMName ?? "").Contains(request.SearchValue)
                    || (d.VendorCode ?? "").Contains(request.SearchValue)
                    ||(d.DealerName ?? "").Contains(request.SearchValue)
                    || (d.EmbossingStationName ?? "").Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbContext.VOEMVendorDealerMapping.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.OEMVendorDealerMappingID,
                                w.DealerName,
                                w.VendorCode,
                                w.OEMName,
                                w.EmbossingStationName,
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