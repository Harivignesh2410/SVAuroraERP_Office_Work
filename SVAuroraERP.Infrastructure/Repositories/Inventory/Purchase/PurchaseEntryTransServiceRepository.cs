namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Purchase
{
    public class PurchaseEntryTransServiceRepository : IPurchaseEntryTransServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<PurchaseEntryTransServiceRepository> _logger;
        private readonly IAuditLogger _auditLogger;
        public PurchaseEntryTransServiceRepository(SVAuroraERPDbContext context,
                                                    ILogger<PurchaseEntryTransServiceRepository> logger,
                                                    IAuditLogger auditLogger)
        {
            _dbcontext = context;
            _logger = logger;
            _auditLogger = auditLogger;
        }

        public List<VPurchaseEntryTrans> GetPurchaseTrans()
        {
            try
            {
                _auditLogger.SaveActionLog("VPurchaseEntryTrans", ActionType.ListData, null, null);
                return _dbcontext.VPurchaseEntryTrans.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.GetPurchaseEntry(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;

            }
        }

        public List<VPurchaseEntryTrans> GetPurchaseTransByID(int PurchaseEntryID)
        {
            try
            {
                var purchaseEntryTrans = _dbcontext.VPurchaseEntryTrans
                                               .Where(w => w.PurchaseEntryID == PurchaseEntryID)
                                               .Select(v => new VPurchaseEntryTrans
                                               {
                                                   PurchaseTransID = v.PurchaseTransID,
                                                   PurchaseEntryID = v.PurchaseEntryID,
                                                   ItemID = v.ItemID,
                                                   ItemCode = v.ItemCode,
                                                   ItemName = v.ItemName,
                                                   HSNCode = v.HSNCode,
                                                   UnitName = v.UnitName,
                                                   Pcs = v.Pcs,
                                                   Quantity = v.Quantity,
                                                   Rate = v.Rate,
                                                   MaterialValue = v.MaterialValue,
                                                   OtherChargesID1 = v.OtherChargesID1,
                                                   OtherChargesDescription1 = v.OtherChargesDescription1,
                                                   OtherChargesIDAmount1 = v.OtherChargesIDAmount1,
                                                   OtherChargesID2 = v.OtherChargesID2,
                                                   OtherChargesDescription2 = v.OtherChargesDescription2,
                                                   OtherChargesIDAmount2 = v.OtherChargesIDAmount2,
                                                   OtherChargesID3 = v.OtherChargesID3,
                                                   OtherChargesDescription3 = v.OtherChargesDescription3,
                                                   OtherChargesIDAmount3 = v.OtherChargesIDAmount3,
                                                   OtherChargesAmount = v.OtherChargesAmount,
                                                   TaxableChargesAmount = v.TaxableChargesAmount,
                                                   TaxID1 = v.TaxID1,
                                                   TaxName1 = v.TaxName1,
                                                   TaxPercentage1 = v.TaxPercentage1,
                                                   TaxAmount1 = v.TaxAmount1,
                                                   TaxID2 = v.TaxID2,
                                                   TaxName2 = v.TaxName2,
                                                   TaxPercentage2 = v.TaxPercentage2,
                                                   TaxAmount2 = v.TaxAmount2,
                                                   TaxAmount = v.TaxAmount,
                                                   SubTotal = v.SubTotal,
                                                   LastUpdatedBy = v.LastUpdatedBy,
                                                   LastUpdatedDate = v.LastUpdatedDate,
                                                   SizeName = v.SizeName,
                                                   ColorName = v.ColorName,
                                                   ComponentTypeID = v.ComponentTypeID,
                                                   ComponentTypeName = v.ComponentTypeName
                                               }).ToList();
                _auditLogger.SaveActionLog("VPurchaseEntryTrans", ActionType.Select, PurchaseEntryID.ToString(), PurchaseEntryID, null, "PurchaseEntryServiceRepository.GetPurchaseTransByID()");
                return purchaseEntryTrans;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.GetPurchaseTransByID(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public Tuple<bool, bool> SavePurchaseTransDetails(List<PurchaseEntryTrans> request)
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

                foreach (var purchaseTrans in request)
                {
                    if (purchaseTrans.StatusFlag == "I") //Insert
                    {
                        if (Add(purchaseTrans)) SuccessCount++;
                    }
                    else if (purchaseTrans.StatusFlag == "U") //Update
                    {
                        if (Update(purchaseTrans)) SuccessCount++;
                    }
                    else if (purchaseTrans.StatusFlag == "D") //Delete
                    {
                        if (Delete(purchaseTrans.PurchaseTransID, purchaseTrans.LastUpdatedBy)) SuccessCount++;
                    }
                }

                if (SuccessCount > 0)
                {
                    IsSuccess = true;
                    doesSupplierExist = true;
                }
                _auditLogger.SaveActionLog("PurchaseEntryTrans", ActionType.Insert, null, request);
                return Tuple.Create(IsSuccess, doesSupplierExist);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.SavePurchaseTransDetails(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return Tuple.Create(IsSuccess, doesSupplierExist);
            }
        }

        public bool Add(PurchaseEntryTrans request)
        {
            bool IsSuccess = false;
            try
            {
                var CheckIfDataExists = _dbcontext.PurchaseEntryTrans.FirstOrDefault(w => w.PurchaseTransID == request.PurchaseTransID && w.PurchaseEntryID == request.PurchaseEntryID);

                if (CheckIfDataExists == null)
                {
                    request.LastUpdatedDate = DateTime.UtcNow;
                    _dbcontext.PurchaseEntryTrans.Add(request);
                    _dbcontext.SaveChanges();
                    IsSuccess = true;
                }
                _auditLogger.SaveActionLog("PurchaseEntryTrans", ActionType.Insert, CheckIfDataExists.PurchaseTransID.ToString(), request);

                return IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.Add(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return IsSuccess;
            }
        }

        public bool Update(PurchaseEntryTrans request)
        {
            bool IsSuccess = false;
            try
            {
                var CheckIfDataExists = _dbcontext.PurchaseEntryTrans.FirstOrDefault(w => w.PurchaseTransID == request.PurchaseTransID);
                if (CheckIfDataExists != null)
                {

                    CheckIfDataExists.ItemID = request.ItemID;
                    CheckIfDataExists.Pcs = request.Pcs;
                    CheckIfDataExists.Quantity = request.Quantity;
                    CheckIfDataExists.Rate = request.Rate;
                    CheckIfDataExists.MaterialValue = request.MaterialValue;
                    CheckIfDataExists.OtherChargesID1 = request.OtherChargesID1;
                    CheckIfDataExists.OtherChargesIDAmount1 = request.OtherChargesIDAmount1;
                    CheckIfDataExists.OtherChargesID2 = request.OtherChargesID2;
                    CheckIfDataExists.OtherChargesIDAmount2 = request.OtherChargesIDAmount2;
                    CheckIfDataExists.OtherChargesID3 = request.OtherChargesID3;
                    CheckIfDataExists.OtherChargesIDAmount3 = request.OtherChargesIDAmount3;
                    CheckIfDataExists.OtherChargesAmount = request.OtherChargesAmount;
                    CheckIfDataExists.TaxableChargesAmount = request.TaxableChargesAmount;
                    CheckIfDataExists.TaxID1 = request.TaxID1;
                    CheckIfDataExists.TaxPercentage1 = request.TaxPercentage1;
                    CheckIfDataExists.TaxAmount1 = request.TaxAmount1;
                    CheckIfDataExists.TaxID2 = request.TaxID2;
                    CheckIfDataExists.TaxPercentage2 = request.TaxPercentage2;
                    CheckIfDataExists.TaxAmount2 = request.TaxAmount2;
                    CheckIfDataExists.TaxAmount = request.TaxAmount;
                    CheckIfDataExists.SubTotal = request.SubTotal;
                    CheckIfDataExists.LastUpdatedBy = request.LastUpdatedBy;
                    CheckIfDataExists.LastUpdatedDate = DateTime.UtcNow;

                    _dbcontext.SaveChanges();
                    IsSuccess = true;
                }
                _auditLogger.SaveActionLog("PurchaseEntryTrans", ActionType.Insert, CheckIfDataExists.PurchaseTransID.ToString(), request);
                return IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.Update(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return IsSuccess;
            }
        }

        public bool Delete(int PurchaseTransID, int LastUpdatedBy)
        {
            bool IsSuccess = false;
            try
            {
                var entity = _dbcontext.PurchaseEntryTrans.FirstOrDefault(w => w.PurchaseTransID == PurchaseTransID);

                if (entity != null)
                {
                    entity.IsDeleted = true;
                    entity.LastUpdatedBy = LastUpdatedBy;

                    _dbcontext.SaveChanges();
                    IsSuccess = true;
                }
                _auditLogger.SaveActionLog("PurchaseEntryTrans", ActionType.Delete, null, PurchaseTransID);
                return IsSuccess;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.Delete(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return IsSuccess;
            }
        }
    }
}