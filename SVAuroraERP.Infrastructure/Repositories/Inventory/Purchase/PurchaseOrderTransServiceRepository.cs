namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Purchase
{
    public class PurchaseOrderTransServiceRepository : IPurchaseOrderTransServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<PurchaseOrderTransServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IAuditLogger _auditLogger;

        public PurchaseOrderTransServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<PurchaseOrderTransServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public List<VPurchaseOrderTrans> GetPurchaseOrderTransList()
        {
            try
            {
                _auditLogger.SaveActionLog("VPurchaseOrderTrans", ActionType.ListData, null, null, "PurchaseOrderTransServiceRepository.GetPurchaseOrderTransList()");
                return _dbcontext.VPurchaseOrderTrans.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseOrderTransServiceRepository.GetPurchaseOrderTransList(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;

            }
        }
        public List<VPurchaseOrderTrans>? GetPurchaseOrderTransListByID(int PurchaseOrderID)
        {
            var purchaseOrderTrans = _dbcontext.VPurchaseOrderTrans.Where(w => w.PurchaseOrderID == PurchaseOrderID).ToList();
            return purchaseOrderTrans;
        }
        public Tuple<bool, bool> SavePurchaseOrderTransDetails(List<PurchaseOrderTrans> request)
        {
            bool IsSuccess = false;
            bool doesSupplierExist = false;

            int SuccessCount = 0;
            try
            {
                if (request == null || request.Count == 0)
                {
                    return Tuple.Create(IsSuccess, doesSupplierExist);
                }

                foreach (var purchaseOrderTrans in request)
                {
                    if (purchaseOrderTrans.StatusFlag == "I") //Insert
                    {
                        if (Add(purchaseOrderTrans)) SuccessCount++;
                    }
                    else if (purchaseOrderTrans.StatusFlag == "U") //Update
                    {
                        if (Update(purchaseOrderTrans)) SuccessCount++;
                    }
                    else if (purchaseOrderTrans.StatusFlag == "D") //Delete
                    {
                        if (Delete(purchaseOrderTrans.PurchaseOrderTransID, purchaseOrderTrans.LastUpdatedBy)) SuccessCount++;
                    }
                }

                if (SuccessCount > 0)
                {
                    IsSuccess = true;
                    doesSupplierExist = true;
                }
                _auditLogger.SaveActionLog("PurchaseOrderTrans", ActionType.Insert, null, request, null, "PurchaseOrderTransServiceRepository.SavePurchaseOrderTransDetails()");

                return Tuple.Create(IsSuccess, doesSupplierExist);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseOrderTransServiceRepository.SavePurchaseOrderTransDetails(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;

            }
        }
        public bool Add(PurchaseOrderTrans request)
        {
            bool IsSuccess = false;
            try
            {
                var CheckIfDataExists = _dbcontext.PurchaseOrderTrans.FirstOrDefault(w => w.PurchaseOrderTransID == request.PurchaseOrderTransID
                                                                                               && w.PurchaseOrderID == request.PurchaseOrderID);

                if (CheckIfDataExists == null)
                {
                    request.LastUpdatedDate = DateTime.UtcNow;
                    _dbcontext.PurchaseOrderTrans.Add(request);
                    _dbcontext.SaveChanges();
                    IsSuccess = true;
                }
                _auditLogger.SaveActionLog("PurchaseOrderTrans", ActionType.Insert, null, request, null, "PurchaseOrderTransServiceRepository.Add()");

                return IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseOrderTransServiceRepository.Add(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return IsSuccess;

            }
        }
        public bool Update(PurchaseOrderTrans request)
        {
            bool IsSuccess = false;
            try
            {
                var CheckIfDataExists = _dbcontext.PurchaseOrderTrans.FirstOrDefault(w => w.PurchaseOrderTransID == request.PurchaseOrderTransID);
                if (CheckIfDataExists != null)
                {

                    CheckIfDataExists.PurchaseOrderTransID = request.PurchaseOrderTransID;
                    CheckIfDataExists.PurchaseOrderID = request.PurchaseOrderID;
                    CheckIfDataExists.ItemID = request.ItemID;
                    CheckIfDataExists.Quantity = request.Quantity;
                    CheckIfDataExists.LastUpdatedBy = request.LastUpdatedBy;
                    CheckIfDataExists.LastUpdatedDate = DateTime.UtcNow;

                    _dbcontext.SaveChanges();
                    IsSuccess = true;
                }
                _auditLogger.SaveActionLog("PurchaseOrderTrans", ActionType.Update, request.PurchaseOrderID.ToString(), request, CheckIfDataExists, "PurchaseOrderTransServiceRepository.Update()");

                return IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseOrderTransServiceRepository.Update(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return IsSuccess;

            }
        }
        public bool Delete(int PurchaseOrderTransID, int LastUpdatedBy)
        {
            bool IsSuccess = false;
            try
            {
                var entity = _dbcontext.PurchaseOrderTrans.FirstOrDefault(w => w.PurchaseOrderTransID == PurchaseOrderTransID);

                if (entity != null)
                {
                    entity.IsDeleted = true;
                    entity.LastUpdatedBy = LastUpdatedBy;

                    _dbcontext.SaveChanges();
                    IsSuccess = true;
                }
                _auditLogger.SaveActionLog("PurchaseOrderTrans", ActionType.Delete, PurchaseOrderTransID.ToString(), null, null, "PurchaseOrderTransServiceRepository.Delete()");

                return IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseOrderTransServiceRepository.Delete(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return IsSuccess;
            }

        }
    }
}