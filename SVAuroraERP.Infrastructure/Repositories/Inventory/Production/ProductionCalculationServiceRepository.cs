namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Production
{
    public class ProductionCalculationServiceRepository : IProductionCalculationServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<ProductionCalculationServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public ProductionCalculationServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<ProductionCalculationServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger,
                                     IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetProductionCalculation()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VProductionCalculation.OrderBy(o => o.ComponentTypeID).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VProductionCalculation", ActionType.ListData, null, null, null, "ProductionCalculationServiceRepository.GetProductionCalculation()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "ProductionCalculationServiceRepository.GetProductionCalculation()");
            }
              return dataResponse;
        }
        public DataResponse GetByID(int ProductionCalculationID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VProductionCalculation.FirstOrDefault(w => w.ProductionCalculationID == ProductionCalculationID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataResponse.ID = ProductionCalculationID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VProductionCalculation", ActionType.ListData, null, ProductionCalculationID, null, "ProductionCalculationServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ProductionCalculationID, "ProductionCalculationServiceRepository.GetByID()");
            }
             return dataResponse;
        }
        public DataResponse Save(ProductionCalculation request)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.ProductionCalculation.FirstOrDefault(r => r.ComponentTypeID == request.ComponentTypeID && r.SizeID==request.SizeID);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.ProductionCalculationID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.ProductionCalculation.Add(request);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("ProductionCalculation", ActionType.Insert, request.ProductionCalculationID.ToString(), request, null, "ProductionCalculationServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "ProductionCalculationServiceRepository.Save()");
            }
             return dataResponse;
        }
        public DataResponse Update(ProductionCalculation request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.ProductionCalculation.FirstOrDefault(r => r.ProductionCalculationID != request.ProductionCalculationID 
                                                                && r.ComponentTypeID == request.ComponentTypeID && r.SizeID == request.SizeID);

                if (isFound != null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = isFound.ProductionCalculationID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.ProductionCalculation.FirstOrDefault(r => r.ProductionCalculationID == request.ProductionCalculationID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("ProductionCalculation", ActionType.Update, request.ProductionCalculationID.ToString(), request, null, "ProductionCalculationServiceRepository.Update()");
                dataexists.ComponentTypeID = request.ComponentTypeID;
                dataexists.SizeID = request.SizeID;
                dataexists.UnitID = request.UnitID;
                dataexists.QuantityForOneUnit = request.QuantityForOneUnit;
                dataexists.ProductionQuantity = request.ProductionQuantity;
                dataexists.PerPlate = request.PerPlate;
                dataexists.IsActive = request.IsActive;
                dataexists.LastUpdatedBy = request.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;

                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.ProductionCalculationID;
                dataResponse.Message = Constants.UpdatedSucessfully;


            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "ProductionCalculationServiceRepository.Update()");
            }
             return dataResponse;
        }
        public DataResponse Delete(int ProductionCalculationID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.ProductionCalculation.FirstOrDefault(w => w.ProductionCalculationID == ProductionCalculationID);
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

                dataResponse.ID = dataexists.ProductionCalculationID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("ProductionCalculation", ActionType.Delete, null, ProductionCalculationID, null, "ProductionCalculationServiceRepository.Update()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ProductionCalculationID, "ProductionCalculationServiceRepository.Update()");
            }
            return dataResponse;
        }
        public DataResponse GetProductionCalculationtoDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VProductionCalculation> query = _dbcontext.VProductionCalculation;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.ComponentTypeName.Contains(request.SearchValue) ||
                                             d.SizeName.Contains(request.SearchValue) ||
                                              d.UnitName.Contains(request.SearchValue));
                }

                if (request.IsCustomFilterEnabled)
                {

                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VProductionCalculation.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.ProductionCalculationID,
                                                       w.SizeName,
                                                       w.ComponentTypeName,
                                                       w.UnitName,
                                                       w.QuantityForOneUnit,
                                                       w.ProductionQuantity,
                                                       w.PerPlate,
                                                       w.IsActive
                                                   }).ToList();


                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;

                _auditLogger.SaveActionLog("VProductionCalculation", ActionType.ListData, null, request,null, "ProductionCalculationServiceRepository.GetProductionCalculationtoDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ProductionCalculationServiceRepository.GetProductionCalculationtoDataTable()");
            }
            return response;
        }
    }
}
