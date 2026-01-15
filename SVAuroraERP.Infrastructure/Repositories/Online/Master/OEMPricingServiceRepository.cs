namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class OEMPricingServiceRepository : IOEMPricingServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<OEMPricingServiceRepository> _logger;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public OEMPricingServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<OEMPricingServiceRepository> logger,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _auditLogger = auditLogger;
        }
        public DataResponse GetOEMPricing()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VOEMPricing.OrderBy(o => o.PartNumber).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VOEMPricing", ActionType.ListData, null, null,null, "OEMPricingServiceRepository.GetOEMPricing()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "OEMPricingServiceRepository.GetOEMPricing()");
            }

            return dataResponse;
        }
        public DataResponse GetOEMPricingByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VOEMPricing.FirstOrDefault(w => w.OEMPricingID == ID);
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
                _auditLogger.SaveActionLog("VOEMPricing", ActionType.Select, ID.ToString(), ID, null, "OEMPricingServiceRepository.GetOEMPricingByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "OEMPricingServiceRepository.GetOEMPricingByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(OEMPricing OEMPricing)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                //var dataexists = _dbcontext.OEMPricing.FirstOrDefault(r => r.OEMPricingName == OEMPricing.OEMPricingName);
                //if (dataexists != null)
                //{
                //    dataResponse.Error = true;
                //    dataResponse.Success = false;
                //    dataResponse.ID = dataexists.StateID;
                //    dataResponse.Message = Constants.DataAlreadyExist;
                //    return dataResponse;
                //}

                OEMPricing.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.OEMPricing.Add(OEMPricing);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("OEMPricing", ActionType.Insert, OEMPricing.OEMPricingID.ToString(), OEMPricing, null, "OEMPricingServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, OEMPricing, "OEMPricingServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(OEMPricing OEMPricing)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                //var isFound = _dbcontext.OEMPricing.FirstOrDefault(r => r.OEMPricingID != OEMPricing.OEMPricingID && r.OEMPricingName == OEMPricing.OEMPricingName);
                //if (isFound != null)
                //{
                //    dataResponse.Error = true;
                //    dataResponse.Success = false;
                //    dataResponse.ID = isFound.OEMPricingID;
                //    dataResponse.Message = Constants.DataAlreadyExist;
                //    return dataResponse;
                //}
                var dataexists = _dbcontext.OEMPricing.FirstOrDefault(r => r.OEMPricingID == OEMPricing.OEMPricingID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("OEMPricing", ActionType.Update, dataexists.OEMPricingID.ToString(), dataexists,OEMPricing, "OEMPricingServiceRepository.Update()");
                dataexists.HSRPPartNumberID = OEMPricing.HSRPPartNumberID;
                dataexists.VehiclePlateSizeFrontID = OEMPricing.VehiclePlateSizeFrontID;
                dataexists.VehiclePlateSizeRearID = OEMPricing.VehiclePlateSizeRearID;
                dataexists.Rivets = OEMPricing.Rivets;
                dataexists.SnapLock = OEMPricing.SnapLock;
                dataexists.Rate = OEMPricing.Rate;
                dataexists.CourierCharges = OEMPricing.CourierCharges;
                dataexists.TotalAmount = OEMPricing.TotalAmount;



                dataexists.LastUpdatedBy = OEMPricing.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.OEMPricingID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, OEMPricing, "OEMPricingServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int OEMPricingID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.OEMPricing.FirstOrDefault(w => w.OEMPricingID == OEMPricingID);
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

                dataResponse.ID = dataexists.OEMPricingID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("OEMPricing", ActionType.Delete, null, UserID,null, "OEMPricingServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, OEMPricingID, "OEMPricingServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetOEMPricingDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VOEMPricing> query = _dbcontext.VOEMPricing;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.OEMName).Contains(request.SearchValue)
                    || (d.PartNumber).Contains(request.SearchValue)
                    || (d.VehiclePlateSizeNameRear).Contains(request.SearchValue)
                    || (d.VehiclePlateSizeNameFront).Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VOEMPricing.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.OEMPricingID,
                                w.OEMName,
                                w.PartNumber,
                                w.VehiclePlateSizeNameFront,
                                w.VehiclePlateSizeNameRear,
                                w.Rate,
                                w.CourierCharges,
                                w.TotalAmount
                            }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("OEMPricing", ActionType.Select, null, request, null, "OEMPricingServiceRepository.GetOEMPricingDataTableList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "OEMPricingServiceRepository.GetOEMPricingDataTableList()");
            }
            return response;
        }
    }
}