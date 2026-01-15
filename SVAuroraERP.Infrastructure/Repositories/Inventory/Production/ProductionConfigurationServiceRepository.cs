namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Production
{
    public class ProductionConfigurationServiceRepository : IProductionConfigurationServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<ProductionConfigurationServiceRepository> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public ProductionConfigurationServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<ProductionConfigurationServiceRepository> logger,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _auditLogger = auditLogger;
        }
        public List<VProductionConfiguration> GetProductionConfigurationList()
        {
            try
            {
                _auditLogger.SaveActionLog("VProductionConfiguration", ActionType.ListData, null, null, null, "ProductionConfigurationServiceRepository.GetProductionConfigurationList()");
                return _dbcontext.VProductionConfiguration.OrderBy(o => o.ProcessTypeID).ToList();
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, null, "ProductionConfigurationServiceRepository.GetProductionConfigurationList()");
                return null;
            }
        }
        public VProductionConfiguration GetProductionConfigurationByID(int ProductionConfigurationID)
        {
            try
            {
                _auditLogger.SaveActionLog("VProductionConfiguration", ActionType.ListData, null, ProductionConfigurationID, null, "ProductionConfigurationServiceRepository.GetProductionConfigurationByID()");
                return _dbcontext.VProductionConfiguration.FirstOrDefault(w => w.ProductionConfigurationID == ProductionConfigurationID);
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, ProductionConfigurationID, "ProductionConfigurationServiceRepository.GetProductionConfigurationByID()");
                return null;
            }
        }
        public Tuple<bool, bool> Save(ProductionConfiguration request)
        {
            bool IsSuccess = false;
            bool doesProductionConfigurationExist = false;
            try
            {
                var dataexists = _dbcontext.ProductionConfiguration.FirstOrDefault(r => r.ProcessTypeID == request.ProcessTypeID && r.ComponentTypeID == request.ComponentTypeID);

                if (dataexists == null)
                {
                    request.LastUpdatedDate = DateTime.UtcNow;
                    _dbcontext.ProductionConfiguration.Add(request);
                    _dbcontext.SaveChanges();
                    IsSuccess = true;
                    _auditLogger.SaveActionLog("ProductionConfiguration", ActionType.ListData, null, request, null, "ProductionConfigurationServiceRepository.Save()");
                }
                else
                    doesProductionConfigurationExist = true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"ProductionConfigurationServiceRepository.Save(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                IsSuccess = false;
            }
            return Tuple.Create(IsSuccess, doesProductionConfigurationExist);

        }
        public Tuple<bool, bool> Update(ProductionConfiguration request)
        {
            bool IsSuccess = false;
            bool doesProductionConfigurationyExist = false;
            try
            {
                var isFound = _dbcontext.ProductionConfiguration.FirstOrDefault(r => r.ProductionConfigurationID != request.ProductionConfigurationID && r.ProcessTypeID == request.ProcessTypeID && r.ComponentTypeID == request.ComponentTypeID);
                if (isFound != null)
                {
                    IsSuccess = false;
                    doesProductionConfigurationyExist = true;

                    return Tuple.Create(IsSuccess, doesProductionConfigurationyExist);
                }

                var ExistingProductionConfiguration = _dbcontext.ProductionConfiguration.FirstOrDefault(r => r.ProductionConfigurationID == request.ProductionConfigurationID);
                _auditLogger.SaveActionLog("ProductionConfiguration", ActionType.Update, request.ProductionConfigurationID.ToString(), request, null, "ProductionConfigurationServiceRepository.Update()");
                if (ExistingProductionConfiguration != null)
                {
                    ExistingProductionConfiguration.ProcessTypeID = request.ProcessTypeID;
                    ExistingProductionConfiguration.ComponentTypeID = request.ComponentTypeID;
                    ExistingProductionConfiguration.LastUpdatedDate = DateTime.UtcNow;
                    ExistingProductionConfiguration.LastUpdatedBy = request.LastUpdatedBy;

                    _dbcontext.SaveChanges();
                    IsSuccess = true;
                }
                else
                    doesProductionConfigurationyExist = false;
             }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, request, "ProductionConfigurationServiceRepository.Update()");
                IsSuccess = false;
            }

            return Tuple.Create(IsSuccess, doesProductionConfigurationyExist);
        }
        public Tuple<bool, bool> Delete(int ProductionConfigurationID, int UserID)
        {
            bool IsSuccess = false;
            bool doesProductionConfigurationyExist = false;

            try
            {
                var dataexists = _dbcontext.ProductionConfiguration.FirstOrDefault(w => w.ProductionConfigurationID == ProductionConfigurationID);
                if (dataexists != null)
                {
                    dataexists.LastUpdatedDate = DateTime.UtcNow;
                    dataexists.LastUpdatedBy = UserID;
                    dataexists.IsDeleted = true;

                    IsSuccess = true;
                    doesProductionConfigurationyExist = true;

                    _dbcontext.SaveChanges();
                }
                _auditLogger.SaveActionLog("ProductionConfiguration", ActionType.Delete, null, ProductionConfigurationID, null, "ProductionConfigurationServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, ProductionConfigurationID, "ProductionConfigurationServiceRepository.Delete()");
                IsSuccess = false;
            }
            return Tuple.Create(IsSuccess, doesProductionConfigurationyExist);
        }
        public List<VProductionConfiguration> GetProductionConfigurationByProcessTypeID(int ProcessTypeID)
        {
            try
            {
                var resultdata = _dbcontext.VProductionConfiguration.Where(w => w.ProcessTypeID == ProcessTypeID).OrderBy(o => o.ComponentTypeName).ToList();

                _auditLogger.SaveActionLog("VProductionConfiguration", ActionType.ListData, null, ProcessTypeID, null, "ProductionConfigurationServiceRepository.GetProductionConfigurationByProcessTypeID()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var dataResponse = _errorLoggerService.LogException(ex, ProcessTypeID, "ProductionConfigurationServiceRepository.GetProductionConfigurationByProcessTypeID()");
                return null;
            }
        }
    }
}