namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Purchase
{
    public class PurchaseEntryServiceRepository : IPurchaseEntryServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<PurchaseEntryServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IAuditLogger _auditLogger;
        private readonly IPurchaseEntryTransServiceRepository _purchasetranse;
        private readonly IPendingInspectionServiceRepository _pending;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IGlobalConfigServiceRepository _globalrepository = null;

        public PurchaseEntryServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<PurchaseEntryServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IPurchaseEntryTransServiceRepository purchasetranse,
                                     IPendingInspectionServiceRepository pending,
                                    IGlobalConfigServiceRepository globalrepository,
                                     IAuditLogger auditLogger, IErrorLoggerService errorLoggerService
                                     )
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _purchasetranse = purchasetranse;
            _pending = pending;
            _globalrepository = globalrepository;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public List<VPurchaseEntry> GetPurchaseEntry()
        {
            try
            {
                var globalconfig = _globalrepository.GetGlobalConfig().Result;
                var resultdata = _dbcontext.VPurchaseEntry.Take(globalconfig.RowLimitCount).ToList();
                _auditLogger.SaveActionLog("PurchaseEntry", ActionType.ListData, null, null);
                return resultdata;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.GetPurchaseEntry(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;

            }
        }
        public VPurchaseEntry GetByID(int PurchaseEntryID)
        {
            try
            {
                var PurchaseEntry = _dbcontext.VPurchaseEntry.FirstOrDefault(w => w.PurchaseEntryID == PurchaseEntryID);
                PurchaseEntry.PurchaseEntryTransList = _purchasetranse.GetPurchaseTransByID(PurchaseEntryID);
                _auditLogger.SaveActionLog("PurchaseEntry", ActionType.Select, PurchaseEntryID.ToString(), PurchaseEntryID, null, "PurchaseEntryServiceRepository.GetByID()");
                return PurchaseEntry;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.GetByID(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }
        public Tuple<bool, bool, int> Save(PurchaseEntry request)
        {
            bool IsSuccess = false;
            bool doesPurchaseEntryExist = false;
            int ID = 0;
            try
            {
                var dataexists = _dbcontext.PurchaseEntry.FirstOrDefault(r => r.PurchaseInvoiceNo == request.PurchaseInvoiceNo);
                if (dataexists == null)
                {
                    request.LastUpdatedDate = DateTime.UtcNow;
                    request.PurchaseStatusID = 1;
                    _dbcontext.PurchaseEntry.Add(request);
                    _dbcontext.SaveChanges();
                    ID = request.PurchaseEntryID;

                    foreach (PurchaseEntryTrans purchaseEntryTrans in request.PurchaseEntryTransList)
                    {
                        purchaseEntryTrans.PurchaseEntryID = request.PurchaseEntryID;
                    }

                    _purchasetranse.SavePurchaseTransDetails(request.PurchaseEntryTransList);
                    IsSuccess = true;
                    if (IsSuccess == true)
                    {
                        var purchaseorder = _dbcontext.PurchaseOrder.FirstOrDefault(r => r.PurchaseOrderID == request.PurchaseOrderID);
                        purchaseorder.PurchaseOrderStatusID = 2;
                        _dbcontext.SaveChanges();
                    }
                    _auditLogger.SaveActionLog("PurchaseEntry", ActionType.Insert, dataexists.PurchaseEntryID.ToString(), request);
                }
                else
                    doesPurchaseEntryExist = true;

            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.Save(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                IsSuccess = false;
            }
            return Tuple.Create(IsSuccess, doesPurchaseEntryExist, ID);
        }
        public Tuple<bool, bool, int> Update(PurchaseEntry request)
        {
            bool IsSuccess = false;
            bool doesUnitExist = false;
            int ID = 0;
            try
            {
                var isFound = _dbcontext.PurchaseEntry.FirstOrDefault(r => r.PurchaseEntryID != request.PurchaseEntryID && r.PurchaseInvoiceNo == request.PurchaseInvoiceNo);
                if (isFound != null)
                {
                    IsSuccess = false;
                    doesUnitExist = true;
                }
                var dataexists = _dbcontext.PurchaseEntry.FirstOrDefault(r => r.PurchaseEntryID == request.PurchaseEntryID);
                if (dataexists != null && !doesUnitExist)
                {
                    dataexists.PurchaseInvoiceNo = request.PurchaseInvoiceNo;
                    dataexists.PurchaseInvoiceDate = request.PurchaseInvoiceDate;
                    dataexists.PurchaseEntryID = request.PurchaseEntryID;
                    dataexists.GrossAmount = request.GrossAmount;
                    dataexists.RoundedOffPlus = request.RoundedOffPlus;
                    dataexists.RoundedOffMinus = request.RoundedOffMinus;
                    dataexists.OtherChargesID = request.OtherChargesID;
                    dataexists.OtherChargesAmount = request.OtherChargesAmount;
                    dataexists.TaxID1 = request.TaxID1;
                    dataexists.TaxPercentage1 = request.TaxPercentage1;
                    dataexists.TaxAmount1 = request.TaxAmount1;
                    dataexists.TaxID2 = request.TaxID2;
                    dataexists.TaxPercentage2 = request.TaxPercentage2;
                    dataexists.TaxAmount2 = request.TaxAmount2;
                    dataexists.TaxAmount = request.TaxAmount;
                    dataexists.PurchaseInvoiceAmount = request.PurchaseInvoiceAmount;
                    dataexists.Narration = request.Narration;
                    dataexists.LastUpdatedBy = request.LastUpdatedBy;
                    dataexists.LastUpdatedDate = DateTime.UtcNow;

                    ID = request.PurchaseEntryID;
                    _auditLogger.SaveActionLog("PurchaseEntry", ActionType.Insert, dataexists.PurchaseEntryID.ToString(), request);
                    _dbcontext.SaveChanges();
                    foreach (PurchaseEntryTrans purchaseEntryTrans in request.PurchaseEntryTransList)
                    {
                        purchaseEntryTrans.PurchaseEntryID = request.PurchaseEntryID;
                    }

                    _purchasetranse.SavePurchaseTransDetails(request.PurchaseEntryTransList);
                    IsSuccess = true;

                }
                else
                    doesUnitExist = false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.Update(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                IsSuccess = false;
            }
            return Tuple.Create(IsSuccess, doesUnitExist, ID);
        }
        public Tuple<bool, bool> Delete(int PurchaseEntryID)
        {
            bool IsSuccess = false;
            bool doesUnitExist = false;
            try
            {
                var dataexists = _dbcontext.PurchaseEntry.FirstOrDefault(w => w.PurchaseEntryID == PurchaseEntryID);
                if (dataexists != null)
                {
                    dataexists.LastUpdatedDate = DateTime.UtcNow;
                    dataexists.IsDeleted = true;
                    _dbcontext.SaveChanges();
                    IsSuccess = true;
                    doesUnitExist = true;
                    if (IsSuccess == true)
                    {
                        var purchaseorder = _dbcontext.PurchaseOrder.FirstOrDefault(r => r.PurchaseOrderID == dataexists.PurchaseOrderID);
                        purchaseorder.PurchaseOrderStatusID = 1;
                        _dbcontext.SaveChanges();
                    }
                }
                _auditLogger.SaveActionLog("PurchaseEntry", ActionType.Delete, null, PurchaseEntryID);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.Delete(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                IsSuccess = false;
            }
            return Tuple.Create(IsSuccess, doesUnitExist);
        }
        public List<VPurchaseEntry> GetPendingPurchaseEntryByFilter(SearchPendingPurchase searchFilter)
        {
            try
            {
                var query = _dbcontext.VPurchaseEntry.AsQueryable();

                if (searchFilter.SupplierID > 0) query = query.Where(o => o.SupplierID == searchFilter.SupplierID);
                if (searchFilter.ComponentTypeID > 0)
                {
                    query = query.Where(o =>
                        _dbcontext.VPurchaseEntryTrans.Any(t =>
                            t.PurchaseEntryID == o.PurchaseEntryID &&
                            t.ComponentTypeID == searchFilter.ComponentTypeID));
                }
                if (!string.IsNullOrEmpty(searchFilter.sStartDate) && !string.IsNullOrEmpty(searchFilter.sEndDate)) query = query.Where(o => o.PurchaseInvoiceDate >= searchFilter.StartDate
                                                                                           && o.PurchaseInvoiceDate <= searchFilter.EndDate);
                if (!string.IsNullOrEmpty(searchFilter.SearchInWord))
                {
                    string keyword = searchFilter.SearchInWord.ToLower(); // Convert to lowercase for case-insensitive search

                    query = query.Where(o =>
                        o.PurchaseInvoiceNo.ToLower().Contains(keyword) ||
                        o.PurchaseInvoiceDate.ToString().Contains(keyword) ||
                        o.sPurchaseInvoiceDate != null && o.sPurchaseInvoiceDate.ToLower().Contains(keyword) ||
                        o.SupplierID.ToString().Contains(keyword) ||
                        o.SupplierName != null && o.SupplierName.ToLower().Contains(keyword) ||
                        o.GrossAmount.ToString().Contains(keyword) ||
                        o.PurchaseInvoiceAmount.ToString().Contains(keyword) ||
                        o.Narration != null && o.Narration.ToLower().Contains(keyword) ||
                        o.Taxname1 != null && o.Taxname1.ToLower().Contains(keyword) ||
                        o.TaxName2 != null && o.TaxName2.ToLower().Contains(keyword) ||
                        o.LastUpdatedByName != null && o.LastUpdatedByName.ToLower().Contains(keyword) ||
                        o.LastUpdatedDateIST.HasValue && o.LastUpdatedDateIST.ToString().Contains(keyword) ||
                        o.LastUpdatedDate.ToString().Contains(keyword) ||
                        o.PurchaseOrderID.HasValue && o.PurchaseOrderID.ToString().Contains(keyword)
                    );
                }
                return query.Where(w => w.PurchaseStatusID != 3).ToList();
                        }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.GetPendingPurchaseEntryByFilter(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;

            }
        }
        public List<VPurchaseEntry> GetPurchaseEntryByFilter(SearchPurchaseEntryFilter searchFilter)
        {
            try
            {
                var query = _dbcontext.VPurchaseEntry.AsQueryable();

                if (searchFilter.SupplierID > 0) query = query.Where(o => o.SupplierID == searchFilter.SupplierID);

                if (!string.IsNullOrEmpty(searchFilter.sStartDate) && !string.IsNullOrEmpty(searchFilter.sEndDate)) query = query.Where(o => o.PurchaseInvoiceDate >= searchFilter.StartDate
                                                                                           && o.PurchaseInvoiceDate <= searchFilter.EndDate);
                if (!string.IsNullOrEmpty(searchFilter.SearchInWord))
                {
                    string keyword = searchFilter.SearchInWord.ToLower();

                    query = query.Where(o =>
                        o.PurchaseInvoiceNo.ToLower().Contains(keyword) ||
                        o.PurchaseInvoiceDate.ToString().Contains(keyword) ||
                        o.sPurchaseInvoiceDate != null && o.sPurchaseInvoiceDate.ToLower().Contains(keyword) ||
                        o.SupplierID.ToString().Contains(keyword) ||
                        o.SupplierName != null && o.SupplierName.ToLower().Contains(keyword) ||
                        o.GrossAmount.ToString().Contains(keyword) ||
                        o.PurchaseInvoiceAmount.ToString().Contains(keyword) ||
                        o.Narration != null && o.Narration.ToLower().Contains(keyword) ||
                        o.Taxname1 != null && o.Taxname1.ToLower().Contains(keyword) ||
                        o.TaxName2 != null && o.TaxName2.ToLower().Contains(keyword) ||
                        o.LastUpdatedByName != null && o.LastUpdatedByName.ToLower().Contains(keyword) ||
                        o.LastUpdatedDateIST.HasValue && o.LastUpdatedDateIST.ToString().Contains(keyword) ||
                        o.LastUpdatedDate.ToString().Contains(keyword) ||
                        o.PurchaseOrderID.HasValue && o.PurchaseOrderID.ToString().Contains(keyword)
                    );
                }
                return query.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.GetPurchaseEntryByFilter(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;

            }
        }
        public List<VPurchaseEntry> GetCompletedPurchaseEntryByFilter(SearchPendingPurchase searchFilter)
        {
            try
            {
                var query = _dbcontext.VPurchaseEntry.AsQueryable();

                if (searchFilter.SupplierID > 0) query = query.Where(o => o.SupplierID == searchFilter.SupplierID);
                if (searchFilter.ComponentTypeID > 0)
                {
                    query = query.Where(o =>
                        _dbcontext.VPurchaseEntryTrans.Any(t =>
                            t.PurchaseEntryID == o.PurchaseEntryID &&
                            t.ComponentTypeID == searchFilter.ComponentTypeID));
                }
                if (!string.IsNullOrEmpty(searchFilter.sStartDate) && !string.IsNullOrEmpty(searchFilter.sEndDate)) query = query.Where(o => o.PurchaseInvoiceDate >= searchFilter.StartDate
                                                                                           && o.PurchaseInvoiceDate <= searchFilter.EndDate);
                if (!string.IsNullOrEmpty(searchFilter.SearchInWord))
                {
                    string keyword = searchFilter.SearchInWord.ToLower(); // Convert to lowercase for case-insensitive search

                    query = query.Where(o =>
                        o.PurchaseInvoiceNo.ToLower().Contains(keyword) ||
                        o.PurchaseInvoiceDate.ToString().Contains(keyword) ||
                        o.sPurchaseInvoiceDate != null && o.sPurchaseInvoiceDate.ToLower().Contains(keyword) ||
                        o.SupplierID.ToString().Contains(keyword) ||
                        o.SupplierName != null && o.SupplierName.ToLower().Contains(keyword) ||
                        o.PurchaseInvoiceAmount.ToString().Contains(keyword) ||
                        o.LastUpdatedDate.ToString().Contains(keyword)
                    );
                }
                return query.Where(w => w.PurchaseStatusID == 3).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.GetPendingPurchaseEntryByFilter(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;

            }
        }
        public VPurchaseEntry GetMaterialInspectionByPurchaseEntryID(int PurchaseEntryID)
        {
            try
            {
                var PurchaseEntry = _dbcontext.VPurchaseEntry.FirstOrDefault(w => w.PurchaseEntryID == PurchaseEntryID);
                var response = _pending.GetMaterialInwardListByID(PurchaseEntryID);
                PurchaseEntry.PendingInwardInspectionList = response.Value as List<VPendingInwardInspection>;

                //PurchaseEntry.PendingInwardInspectionList = _pending.GetMaterialInwardListByID(PurchaseEntryID);
                return PurchaseEntry;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.GetByID(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }
        public DataResponse GetPurchaseEntryDataTable(DataTableRequest request)
        {

            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VPurchaseEntry> query = _dbcontext.VPurchaseEntry;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.PurchaseInvoiceNo ?? "").Contains(request.SearchValue)
                                            || (d.ComponentNames ?? "").Contains(request.SearchValue)
                                            || (d.SupplierName ?? "").Contains(request.SearchValue));
                }

                if (request.IsCustomFilterEnabled)
                {

                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VPurchaseEntry.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.PurchaseEntryID,
                                                       w.PurchaseInvoiceNo,
                                                       w.PurchaseInvoiceAmount,
                                                       w.SupplierName,
                                                       w.sPurchaseInvoiceDate,
                                                       w.TotalPcs,
                                                       w.TotalQuantity,
                                                       w.TotalItemTax,
                                                       w.GrossAmount,
                                                       w.TaxAmount,
                                                       w.PurchaseStatus
                                                   }).ToList();


                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;

                _auditLogger.SaveActionLog("VPurchaseEntry", ActionType.ListData, null, request, null, "PurchaseEntryServiceRepository.GetPurchaseEntryDataTable()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "PurchaseEntryServiceRepository.GetPurchaseEntryDataTable()");
            }
            return response;
        }
    }
}
