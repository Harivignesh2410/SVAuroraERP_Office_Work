namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Purchase
{
    public class PurchaseOrderServiceRepository : IPurchaseOrderServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<PurchaseOrderServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IPurchaseOrderTransServiceRepository _purchasetranse;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IGlobalConfigServiceRepository _globalrepository = null;
        public PurchaseOrderServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<PurchaseOrderServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IPurchaseOrderTransServiceRepository purchasetranse,
                                      IGlobalConfigServiceRepository globalrepository,
                                      IAuditLogger auditLogger,
                                      IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _purchasetranse = purchasetranse;
            _globalrepository = globalrepository;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetPurchaseOrder()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var globalconfig = _globalrepository.GetGlobalConfig().Result;
                var Material = _dbcontext.VPurchaseOrder.Take(globalconfig.RowLimitCount).ToList();

                DataResponse.Count = Material.Count;
                DataResponse.Message = Constants.RecordFound;
                DataResponse.Value = Material;
                _auditLogger.SaveActionLog("VPurchaseOrder", ActionType.ListData, null, null, "PurchaseOrderServiceRepository.GetPurchaseOrder()");

                return DataResponse;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "PurchaseOrderServiceRepository.GetPurchaseOrder()");
            }
            return DataResponse;
        }
        public DataResponse GetPurchaseOrderByID(int PurchaseOrderID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var PurchaseOrder = _dbcontext.VPurchaseOrder.FirstOrDefault(w => w.PurchaseOrderID == PurchaseOrderID);
                if (PurchaseOrder != null) PurchaseOrder.PurchaseOrderTransList = _purchasetranse.GetPurchaseOrderTransListByID(PurchaseOrderID);

                DataResponse.ID = PurchaseOrderID;
                DataResponse.Message = Constants.RecordFound;
                DataResponse.Value = PurchaseOrder;             
                _auditLogger.SaveActionLog("VPurchaseOrder", ActionType.ListData, null, null, "PurchaseOrderServiceRepository.GetPurchaseOrderByID()");
                return DataResponse;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "PurchaseOrderServiceRepository.GetPurchaseOrderByID()");
            }
            return DataResponse;
        }
        public DataResponse Save(PurchaseOrder request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var exisitingComponent = _dbcontext.PurchaseOrder.FirstOrDefault(w => w.PurchaseOrderNo == request.PurchaseOrderNo);

                if (exisitingComponent != null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = exisitingComponent.PurchaseOrderID;
                    DataResponse.Message = Constants.DataAlreadyExist;

                    return DataResponse;
                }
                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.PurchaseOrder.Add(request);
                _dbcontext.SaveChanges();

                foreach (PurchaseOrderTrans purchaseOrderTrans in request.PurchaseOrderTransList)
                {
                    purchaseOrderTrans.PurchaseOrderID = request.PurchaseOrderID;
                }

                _purchasetranse.SavePurchaseOrderTransDetails(request.PurchaseOrderTransList);

                DataResponse.ID = 0;
                DataResponse.Message = Constants.SuccessMessage;     
                _auditLogger.SaveActionLog("PurchaseOrder", ActionType.Insert, null, request, null, "PurchaseOrderServiceRepository.Save()");
                return DataResponse;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "PurchaseOrderServiceRepository.Save()");
            }

            return DataResponse;
        }

        public DataResponse Update(PurchaseOrder request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var isdataExists = _dbcontext.PurchaseOrder.FirstOrDefault(w => w.PurchaseOrderID != request.PurchaseOrderID && w.PurchaseOrderNo == request.PurchaseOrderNo);

                if (isdataExists != null)
                {
                    DataResponse.Error = false;
                    DataResponse.Success = true;
                    DataResponse.ID = isdataExists.PurchaseOrderID;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }

                var datatoUpdate = _dbcontext.PurchaseOrder.FirstOrDefault(w => w.PurchaseOrderID == request.PurchaseOrderID);

                if (datatoUpdate == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                _auditLogger.SaveActionLog("PurchaseOrder", ActionType.Update, request.PurchaseOrderID.ToString(), request, datatoUpdate, "PurchaseOrderServiceRepository.Update()");

                datatoUpdate.SupplierID = request.SupplierID;
                datatoUpdate.PurchaseOrderNo = request.PurchaseOrderNo;
                datatoUpdate.PurchaseOrderDate = request.PurchaseOrderDate;
                datatoUpdate.PurchaseOrderValue = request.PurchaseOrderValue;
                datatoUpdate.LastUpdatedBy = request.LastUpdatedBy;
                datatoUpdate.LastUpdatedDate = DateTime.UtcNow;

                _dbcontext.SaveChanges();

                foreach (PurchaseOrderTrans purchaseOrderTrans in request.PurchaseOrderTransList)
                {
                    purchaseOrderTrans.PurchaseOrderID = request.PurchaseOrderID;
                }

                _purchasetranse.SavePurchaseOrderTransDetails(request.PurchaseOrderTransList);

                DataResponse.ID = datatoUpdate.PurchaseOrderID;
                DataResponse.Message = Constants.UpdatedSucessfully;

                return DataResponse;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "PurchaseOrderServiceRepository.Update()");
            }

            return DataResponse;
        }
        public DataResponse Delete(int PurchaseOrderID, int UserID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var ItemExists = _dbcontext.PurchaseOrder.FirstOrDefault(w => w.PurchaseOrderID == PurchaseOrderID);

                if (ItemExists == null)
                {
                    DataResponse.Error = false;
                    DataResponse.Success = true;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }

                ItemExists.IsDeleted = true;
                ItemExists.LastUpdatedBy = UserID;

                _dbcontext.SaveChanges();

                DataResponse.ID = ItemExists.PurchaseOrderID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("PurchaseOrder", ActionType.Delete, PurchaseOrderID.ToString(), null, null, "PurchaseOrderServiceRepository.Delete()");
                return DataResponse;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, PurchaseOrderID, "PurchaseOrderServiceRepository.Delete()");
            }

            return DataResponse;
        }
        public DataResponse GetPurchaseOrderDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VPurchaseOrder> query = _dbcontext.VPurchaseOrder;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.PurchaseOrderNo ?? "").Contains(request.SearchValue)
                                            || (d.sPurchaseOrderDate ?? "").Contains(request.SearchValue)
                                            || (d.SupplierName ?? "").Contains(request.SearchValue)
                                            || (d.PurchaseOrderStatus ?? "").Contains(request.SearchValue)
                                            || (d.SupplierName ?? "").Contains(request.SearchValue));
                }

                if (request.IsCustomFilterEnabled)
                {

                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VPurchaseOrder.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.PurchaseOrderID,
                                                       w.PurchaseOrderNo,
                                                       w.sPurchaseOrderDate,
                                                       w.SupplierName,
                                                       w.PurchaseOrderStatusID,
                                                       w.PurchaseOrderStatus,
                                                       w.ColorCode,
                                                       w.PurchaseOrderValue
                                                   }).ToList();


                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VPurchaseOrder", ActionType.ListData, null, request, null, "PurchaseOrderServiceRepository.GetPurchaseOrderDataTable()");

                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "PurchaseOrderServiceRepository.GetPurchaseOrderDataTable()");
            }
            return response;
        }
    }
}
