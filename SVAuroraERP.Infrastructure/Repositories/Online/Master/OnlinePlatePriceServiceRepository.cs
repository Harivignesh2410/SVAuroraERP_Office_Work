namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class OnlinePlatePriceServiceRepository : IOnlinePlatePriceServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<OnlinePlatePriceServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public OnlinePlatePriceServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<OnlinePlatePriceServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetOnlinePlatePrice()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VOnlinePlatePrice.OrderBy(o => o.VehicleClassName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VOnlinePlatePrice", ActionType.ListData, null, null,null, "OnlinePlatePriceServiceRepository.GetOnlinePlatePrice()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "OnlinePlatePriceServiceRepository.GetOnlinePlatePrice()");
            }

            return dataResponse;
        }
        public DataResponse GetVehicleCategory()
        {
            DataResponse response = new DataResponse();
            try { 
            var VehicleCategory = _dbcontext.VehicleCategory.ToList();

            response.Count = VehicleCategory.Count;
            response.Value = VehicleCategory;
            _auditLogger.SaveActionLog("VehicleCategory", ActionType.ListData, null, null,null, "OnlinePlatePriceServiceRepository.GetVehicleCategory()");
            return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "OnlinePlatePriceServiceRepository.GetVehicleCategory()");
            }
            return response;
        }
        public DataResponse GetVehicleType()
        {

            DataResponse response = new DataResponse();
            try {
            var VehicleType = _dbcontext.VehicleType.ToList();

            response.Count = VehicleType.Count;
            response.Value = VehicleType;
            _auditLogger.SaveActionLog("VehicleType", ActionType.ListData, null, null,null, "OnlinePlatePriceServiceRepository.GetVehicleType()");
            return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "OnlinePlatePriceServiceRepository.GetVehicleType()");
            }
            return response;
        }
        public DataResponse GetFuel()
        {
            DataResponse response = new DataResponse();
            try
            {
                var Fuel = _dbcontext.Fuel.ToList();
                response.Count = Fuel.Count;
                response.Value = Fuel;
                _auditLogger.SaveActionLog("Fuel", ActionType.ListData, null, null,null, "OnlinePlatePriceServiceRepository.GetFuel()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "OnlinePlatePriceServiceRepository.GetFuel()");
            }
            return response;
        }
        public DataResponse GetVehiclePlateType()
        {
            DataResponse response = new DataResponse();
            try
            {
                var VehiclePlateType = _dbcontext.VehiclePlateType.ToList();
                response.Count = VehiclePlateType.Count;
                response.Value = VehiclePlateType;
                _auditLogger.SaveActionLog("VehiclePlateType", ActionType.ListData, null, null, null, "OnlinePlatePriceServiceRepository.GetVehiclePlateType()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "OnlinePlatePriceServiceRepository.GetVehiclePlateType()");
            }
            return response;
        }
        public DataResponse GetOnlinePlatePriceByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VOnlinePlatePrice.FirstOrDefault(w => w.OnlinePlatePriceID == ID);
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
                _auditLogger.SaveActionLog("VOnlinePlatePrice", ActionType.Select, ID.ToString(), ID, null, "OnlinePlatePriceServiceRepository.GetOnlinePlatePriceByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "OnlinePlatePriceServiceRepository.GetOnlinePlatePriceByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(OnlinePlatePrice request)
        {
            DataResponse DataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.OnlinePlatePrice.FirstOrDefault(r => r.Front == request.Front);
                if (dataexists != null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;
                    return DataResponse;
                }
                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.OnlinePlatePrice.Add(request);
                DataResponse.ID = request.OnlinePlatePriceID;
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("OnlinePlatePrice", ActionType.Insert, request.OnlinePlatePriceID.ToString(), request, null, "OnlinePlatePriceServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "OnlinePlatePriceServiceRepository.Save()");
            }

            return DataResponse;
        }
        public DataResponse Update(OnlinePlatePrice OnlinePlatePrice)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.OnlinePlatePrice.FirstOrDefault(r => r.OnlinePlatePriceID != OnlinePlatePrice.OnlinePlatePriceID && r.Front == OnlinePlatePrice.Front);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.OnlinePlatePriceID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.OnlinePlatePrice.FirstOrDefault(r => r.OnlinePlatePriceID == OnlinePlatePrice.OnlinePlatePriceID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("OnlinePlatePrice", ActionType.Update, dataexists.OnlinePlatePriceID.ToString(), OnlinePlatePrice, dataexists, "OnlinePlatePriceServiceRepository.Update()");
                dataexists.VehicleCategoryID = OnlinePlatePrice.VehicleCategoryID;
                dataexists.VehicleTypeID = OnlinePlatePrice.VehicleTypeID;
                dataexists.VehicleClassID = OnlinePlatePrice.VehicleClassID;
                dataexists.FuelID = OnlinePlatePrice.FuelID;
                dataexists.VehiclePlateColorID = OnlinePlatePrice.VehiclePlateColorID;
                dataexists.VehiclePlateTypeID = OnlinePlatePrice.VehiclePlateTypeID;
                dataexists.VehiclePlateSizeID = OnlinePlatePrice.VehiclePlateSizeID;
                dataexists.Front = OnlinePlatePrice.Front;
                dataexists.Rear = OnlinePlatePrice.Rear;
                dataexists.SnapLock = OnlinePlatePrice.SnapLock;
                dataexists.TLPSticker = OnlinePlatePrice.TLPSticker;
                dataexists.EmbossingFitmentCharges = OnlinePlatePrice.EmbossingFitmentCharges;
                dataexists.DealerFitmentCharges = OnlinePlatePrice.DealerFitmentCharges;
                dataexists.HomeFitmentCharges = OnlinePlatePrice.HomeFitmentCharges;
                dataexists.DealerCourierCharge = OnlinePlatePrice.DealerCourierCharge;
                dataexists.DealerLocationChangeCharge = OnlinePlatePrice.DealerLocationChangeCharge;
                dataexists.OtherCharges = OnlinePlatePrice.OtherCharges;
                dataexists.IsActive = OnlinePlatePrice.IsActive;
                dataexists.LastUpdatedBy = OnlinePlatePrice.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.OnlinePlatePriceID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, OnlinePlatePrice, "OnlinePlatePriceServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int OnlinePlatePriceID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.OnlinePlatePrice.FirstOrDefault(w => w.OnlinePlatePriceID == OnlinePlatePriceID);
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

                dataResponse.ID = dataexists.OnlinePlatePriceID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("OnlinePlatePrice", ActionType.Delete, null, OnlinePlatePriceID,null, "OnlinePlatePriceServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, OnlinePlatePriceID, "OnlinePlatePriceServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetSizeByPlateTypeID(int classID, int plateTypeID)
        {
            DataResponse response = new DataResponse();
            try
            {
                var result = _dbcontext.VVehiclePlateSizeMapping
                                       .Where(w => w.VehicleClassID == classID
                                                && w.VehiclePlateTypeID == plateTypeID)
                                       .ToList();

                response.Count = result.Count;
                response.Value = result;
                response.ID = plateTypeID;
                _auditLogger.SaveActionLog("VVehiclePlateSizeMapping", ActionType.Select, plateTypeID.ToString(), plateTypeID, null, "OnlinePlatePriceServiceRepository.GetSizeByClassAndPlateType()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex,plateTypeID,"OnlinePlatePriceServiceRepository.GetSizeByClassAndPlateType()");
            }
            return response;
        }
        public DataResponse GetPlateTypeByVehicleClassID(int ID)
        {
            DataResponse response = new DataResponse();
            try
            {
                var Result = _dbcontext.VVehiclePlateSizeMapping.Where(w => w.VehicleClassID == ID).ToList();
                response.Count = Result.Count;
                response.Value = Result;
                response.ID = ID;
                _auditLogger.SaveActionLog("VVehiclePlateSizeMapping", ActionType.Select, ID.ToString(), ID, null, "OnlinePlatePriceServiceRepository.GetPlateTypeByVehicleClassID()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, ID, "OnlinePlatePriceServiceRepository.GetPlateTypeByVehicleClassID()");
            }
            return response;
        }
        public DataResponse GetOnlinePlatePriceDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VOnlinePlatePrice> query = _dbcontext.VOnlinePlatePrice;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.VehicleClassName).Contains(request.SearchValue)
                    || (d.VehicleCategoryName).Contains(request.SearchValue)
                    || (d.VehiclePlateColorName).Contains(request.SearchValue)
                    || (d.VehiclePlateSizeName).Contains(request.SearchValue)
                      || (d.VehiclePlateTypeName).Contains(request.SearchValue)
                        || (d.FuelName).Contains(request.SearchValue)
                          || (d.VehicleCategoryName).Contains(request.SearchValue));
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
                                w.OnlinePlatePriceID,
                                w.VehicleCategoryName,
                                w.VehicleTypeName,
                                w.VehicleClassName,
                                w.FuelName,
                                w.VehiclePlateColorName,
                                w.VehiclePlateTypeName,
                                w.VehiclePlateSizeName,
                                w.IsActive
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