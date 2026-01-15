namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class VehiclePlateSizeMappingServiceRepository : IVehiclePlateSizeMappingServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<VehiclePlateSizeMappingServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
             private readonly IAuditLogger _auditLogger;
        public VehiclePlateSizeMappingServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<VehiclePlateSizeMappingServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetVehiclePlateSizeMapping()
        {

            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VVehiclePlateSizeMapping.OrderBy(o => o.VehicleClassName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VVehiclePlateSizeMapping", ActionType.ListData, null, null,null, "VehiclePlateSizeMappingServiceRepository.GetVehiclePlateSizeMapping()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "VehiclePlateSizeMappingServiceRepository.GetVehiclePlateSizeMapping()");
            }

            return dataResponse;
        }
        public DataResponse GetVehicleCategory()
        {
            DataResponse response = new DataResponse();
            try
            {
                var VehicleCategory = _dbcontext.VehicleCategory.ToList();

                response.Count = VehicleCategory.Count;
                response.Value = VehicleCategory;
                _auditLogger.SaveActionLog("VehicleCategory", ActionType.ListData, null, null,null, "VehiclePlateSizeMappingServiceRepository.GetVehicleCategory()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "VehiclePlateSizeMappingServiceRepository.GetVehicleCategory()");
            }
            return response;

        }
        public DataResponse GetVehicleType()
        {

            DataResponse response = new DataResponse();
            try
            {
                var VehicleType = _dbcontext.VehicleType.ToList();

                response.Count = VehicleType.Count;
                response.Value = VehicleType;
                _auditLogger.SaveActionLog("VehicleType", ActionType.ListData, null, null,null, "VehiclePlateSizeMappingServiceRepository.GetVehicleType()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "VehiclePlateSizeMappingServiceRepository.GetVehicleType()");
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
                _auditLogger.SaveActionLog("Fuel", ActionType.ListData, null, null,null, "VehiclePlateSizeMappingServiceRepository.GetFuel()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "VehiclePlateSizeMappingServiceRepository.GetFuel()");
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
                _auditLogger.SaveActionLog("VehiclePlateType", ActionType.ListData, null, null,null, "VehiclePlateSizeMappingServiceRepository.GetVehiclePlateType()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "VehiclePlateSizeMappingServiceRepository.GetVehiclePlateType()");
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
                _auditLogger.SaveActionLog("VOnlinePlatePrice", ActionType.Select, ID.ToString(), ID, null, "HSRPUserServiceRepository.GetOnlinePlatePriceByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "VehiclePlateSizeMappingServiceRepository.GetOnlinePlatePriceByID()");
            }

            return dataResponse;
        }
        public DataResponse GetVehiclePlateSizeMappingByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VVehiclePlateSizeMapping.FirstOrDefault(w => w.VehiclePlateSizeMappingID == ID);
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
                _auditLogger.SaveActionLog("VVehiclePlateSizeMapping", ActionType.Select, ID.ToString(), ID, null, "VehiclePlateSizeMappingServiceRepository.GetVehiclePlateSizeMappingByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "VehiclePlateSizeMappingServiceRepository.GetVehiclePlateSizeMappingByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(VehiclePlateSizeMapping request)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.VehiclePlateSizeMapping.FirstOrDefault(r => r.VehiclePlateColorID == request.VehiclePlateColorID &&
                                                                                        r.FuelID == request.FuelID &&
                                                                                        r.VehicleTypeID == request.VehicleTypeID &&
                                                                                        r.VehicleCategoryID == request.VehicleCategoryID &&
                                                                                        r.VehiclePlateSizeID == request.VehiclePlateSizeID &&
                                                                                        r.VehicleClassID == request.VehicleClassID &&
                                                                                        r.VehiclePlateTypeID == request.VehiclePlateTypeID);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.VehiclePlateSizeID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.VehiclePlateSizeMapping.Add(request);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("VehiclePlateSizeMapping", ActionType.Insert, request.VehiclePlateSizeID.ToString(), request, null, "VehiclePlateSizeMappingServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "VehiclePlateSizeMappingServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(VehiclePlateSizeMapping request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.VehiclePlateSizeMapping.FirstOrDefault(r => r.VehiclePlateColorID == request.VehiclePlateColorID &&
                                                                                        r.FuelID == request.FuelID &&
                                                                                        r.VehicleTypeID == request.VehicleTypeID &&
                                                                                        r.VehicleCategoryID == request.VehicleCategoryID &&
                                                                                        r.VehiclePlateSizeID == request.VehiclePlateSizeID &&
                                                                                        r.VehicleClassID == request.VehicleClassID &&
                                                                                        r.VehiclePlateTypeID == request.VehiclePlateTypeID &&
                                                                                        r.VehiclePlateSizeMappingID != request.VehiclePlateSizeMappingID);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.VehiclePlateSizeMappingID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.VehiclePlateSizeMapping.FirstOrDefault(r => r.VehiclePlateSizeMappingID == request.VehiclePlateSizeMappingID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("VehiclePlateSizeMapping", ActionType.Update, dataexists.VehiclePlateSizeMappingID.ToString(), request, dataexists, "VehiclePlateSizeMappingServiceRepository.Update()");
                dataexists.VehicleClassID = request.VehicleClassID;
                dataexists.VehiclePlateTypeID = request.VehiclePlateTypeID;
                dataexists.Description = request.Description;
                dataexists.VehiclePlateSizeID = request.VehiclePlateSizeID;
                dataexists.LastUpdatedBy = request.LastUpdatedBy;
                dataexists.VehicleCategoryID = request.VehicleCategoryID;
                dataexists.FuelID = request.FuelID;
                dataexists.VehiclePlateColorID = request.VehiclePlateColorID;
                dataexists.VehicleTypeID = request.VehicleTypeID;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.VehiclePlateSizeMappingID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "VehiclePlateSizeMappingServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int VehiclePlateSizeMappingID, int UserID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.VehiclePlateSizeMapping.FirstOrDefault(w => w.VehiclePlateSizeMappingID == VehiclePlateSizeMappingID);
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

                dataResponse.ID = dataexists.VehiclePlateSizeMappingID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("VehiclePlateSizeMapping", ActionType.Delete, null, new { VehiclePlateSizeMappingID , UserID },null, "VehiclePlateSizeMappingServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, new { VehiclePlateSizeMappingID, UserID }, "VehiclePlateSizeMappingServiceRepository.Delete()");
            }
            return dataResponse;
        }
        public DataResponse GetVehiclePlateSizeMappingDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VVehiclePlateSizeMapping> query = _dbcontext.VVehiclePlateSizeMapping;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.VehiclePlateColorName ?? "").Contains(request.SearchValue)
                    || (d.VehiclePlateSizeName ?? "").Contains(request.SearchValue)
                    || (d.VehicleTypeName ?? "").Contains(request.SearchValue)
                    || (d.VehicleCategoryName ?? "").Contains(request.SearchValue)
                    || (d.FuelName ?? "").Contains(request.SearchValue)
                    || (d.VehicleClassName ?? "").Contains(request.SearchValue)
                    || (d.Description ?? "").Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VVehiclePlateSizeMapping.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.VehiclePlateSizeMappingID,
                                w.VehicleTypeName,
                                w.VehicleCategoryName,
                                w.FuelName,
                                w.VehicleClassName,
                                w.VehiclePlateColorName,
                                w.VehiclePlateSizeName,
                                w.Description
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