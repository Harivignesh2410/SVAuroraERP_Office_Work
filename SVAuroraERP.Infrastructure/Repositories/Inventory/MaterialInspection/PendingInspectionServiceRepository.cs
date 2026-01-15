// Added on 2025.01.21 by Harivignesh (US 44)
namespace SVAuroraERP.Infrastructure.Repositories.Inventory.MaterialInspection
{
    public class PendingInspectionServiceRepository : IPendingInspectionServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<PendingInspectionServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public PendingInspectionServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<PendingInspectionServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger, IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _auditLogger = auditLogger;
            _transLogRespository = transLogRespository;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetMaterialInwardList()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var Material = _dbcontext.VPendingInwardInspection.ToList();

                DataResponse.Count = Material.Count;
                DataResponse.Message = Constants.RecordFound;
                DataResponse.Value = Material;
                _auditLogger.SaveActionLog("VPendingInwardInspection", ActionType.ListData, null, null, null, "PendingInspectionServiceRepository.GetMaterialInwardList()");

                return DataResponse;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "PendingInspectionServiceRepository.GetMaterialInwardList()");
            }
            return DataResponse;
        }

        public DataResponse GetMaterialInwardListByID(int PurchaseEntryID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {


                var Material = _dbcontext.VPendingInwardInspection.FirstOrDefault(ws => ws.PurchaseEntryID == PurchaseEntryID);

                if (Material == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;
                }
                else
                {
                    var materialinwards = _dbcontext.VPendingInwardInspection
                                              .Where(w => w.PurchaseEntryID == PurchaseEntryID)
                                              .Select(v => new VPendingInwardInspection
                                              {
                                                  PendingInwardInspectionID = v.PendingInwardInspectionID,
                                                  PurchaseTransID = v.PurchaseTransID,
                                                  PurchaseEntryID = v.PurchaseEntryID,
                                                  ItemCode = v.ItemCode,
                                                  ItemName = v.ItemName,
                                                  HSNCode = v.HSNCode,
                                                  Quantity = v.Quantity,
                                                  UnitName = v.UnitName,
                                                  SizeName = v.SizeName,
                                                  ColorName = v.ColorName,
                                                  BatchNo = v.BatchNo,
                                                  BatchQuantity = v.BatchQuantity,
                                                  LastUpdatedBy = v.LastUpdatedBy,
                                                  LastUpdatedDate = v.LastUpdatedDate,
                                                  IsAutoBatch = v.IsAutoBatch,
                                                  SupplierName = v.SupplierName,
                                                  PurchaseInvoiceAmount = v.PurchaseInvoiceAmount,
                                                  sPurchaseInvoiceDate = v.sPurchaseInvoiceDate,
                                                  PurchaseInvoiceNo = v.PurchaseInvoiceNo,
                                                  PendingQuantity = v.PendingQuantity,
                                                  LessQuantity = v.LessQuantity,
                                                  ExcessQuantity = v.ExcessQuantity
                                              }).ToList();

                    DataResponse.ID = PurchaseEntryID;
                    DataResponse.Message = Constants.RecordFound;
                    DataResponse.Value = materialinwards;
                }
                _auditLogger.SaveActionLog("VPendingInwardInspection", ActionType.ListData, PurchaseEntryID.ToString(), PurchaseEntryID, null, "PendingInspectionServiceRepository.GetMaterialInwardListByID()");
                return DataResponse;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "PendingInspectionServiceRepository.GetMaterialInwardListByID()");
            }
            return DataResponse;
        }
        public Tuple<DataResponse> SaveMaterialInward(List<PendingInspection> request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                int SuccessCount = 0;
                if (request == null || request.Count == 0)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;
                    return Tuple.Create(DataResponse);
                }

                foreach (var Material in request)
                {
                    if (Material.StatusFlag != "")
                    {
                        int result = SavePendingInspection(Material);
                        if (result != 0)
                            SuccessCount++;
                    }
                }

                if (SuccessCount > 0)
                {
                    DataResponse.Message = Constants.SuccessMessage;
                }
                else
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.Message = Constants.DataAlreadyExist;
                }
                _auditLogger.SaveActionLog("PendingInspection", ActionType.Insert, null, request, null, "PendingInspectionServiceRepository.SaveMaterialInward()");
                return Tuple.Create(DataResponse);
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "PendingInspectionServiceRepository.SaveMaterialInward()");
                return Tuple.Create(DataResponse);
            }
        }
        public string GenerateNextBatchNumber()
        {
            string prefix = "B" + DateTime.Now.ToString("yyyyMM");
            string latestBatchNumber = GetLatestBatchNumberFromDB(prefix);

            int nextNumber = 1;
            try
            {
                if (!string.IsNullOrEmpty(latestBatchNumber))
                {
                    string lastNumberPart = latestBatchNumber.Substring(latestBatchNumber.Length - 5);
                    if (int.TryParse(lastNumberPart, out int lastNumber))
                    {
                        nextNumber = lastNumber + 1;
                    }
                }
                return $"{prefix}{nextNumber:D5}";
            }       
            catch (Exception ex)
            {
                _logger.LogError($"PendingInspectionServiceRepository.GenerateNextBatchNumber(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return $"{prefix}{nextNumber:D5}";
            }
}
        public string GetLatestBatchNumberFromDB(string prefix)
        {
            return _dbcontext.PendingInspection
                .Where(x => x.BatchNo.StartsWith(prefix))
                .OrderByDescending(x => x.BatchNo)
                .Select(x => x.BatchNo)
                .FirstOrDefault();
        }
        public void UpdatePurchaseStatus(int PurchaseTransID)
        {
            try
            {
                int? PurchaseEntryID = _dbcontext.PurchaseEntryTrans.FirstOrDefault(pt => pt.PurchaseTransID == PurchaseTransID).PurchaseEntryID;

                if (PurchaseEntryID != null)
                {
                    var totalPurchaseQuantity = _dbcontext.PurchaseEntryTrans.Where(pt => pt.PurchaseEntryID == PurchaseEntryID).Sum(pt => pt.Quantity);
                    //var PurchaseTransIDs = _dbcontext.PurchaseEntryTrans.Where(pt => pt.PurchaseEntryID == PurchaseEntryID).Select(s => s.PurchaseTransID).ToList();
                    var pendingInspectionQty = _dbcontext.VPendingInwardInspection.Where(mi => mi.PurchaseEntryID == PurchaseEntryID).Sum(mi => (decimal?)mi.BatchQuantity) ?? 0;

                    if (totalPurchaseQuantity > 0)
                    {
                        var purchaseEntry = _dbcontext.PurchaseEntry.FirstOrDefault(w => w.PurchaseEntryID == PurchaseEntryID);

                        if (pendingInspectionQty >= 0 && pendingInspectionQty < totalPurchaseQuantity)
                        {
                            if (purchaseEntry != null && purchaseEntry.PurchaseStatusID == 1) //Pending
                            {
                                purchaseEntry.PurchaseStatusID = 2; //In-Progress
                                _dbcontext.SaveChanges();
                            }
                            else if (purchaseEntry != null && purchaseEntry.PurchaseStatusID == 3) //Completed
                            {
                                purchaseEntry.PurchaseStatusID = 2; //In-Progress
                                _dbcontext.SaveChanges();
                            }
                            else
                            {
                                purchaseEntry.PurchaseStatusID = 1; //In-Progress
                                _dbcontext.SaveChanges();
                            }
                        }
                        else if (pendingInspectionQty > 0 && pendingInspectionQty == totalPurchaseQuantity)
                        {
                            if (purchaseEntry != null && purchaseEntry.PurchaseStatusID == 2) //In-Progress
                            {
                                purchaseEntry.PurchaseStatusID = 3;
                                _dbcontext.SaveChanges();
                            }
                            else if (purchaseEntry != null && purchaseEntry.PurchaseStatusID == 1) //Pending
                            {
                                purchaseEntry.PurchaseStatusID = 3;
                                _dbcontext.SaveChanges();
                            }
                        }
                    }
                }
                _auditLogger.SaveActionLog("PurchaseEntryTrans", ActionType.Update, null, null, null, "PendingInspectionServiceRepository.UpdatePurchaseStatus()");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _logger.LogError($"PendingInspectionServiceRepository.UpdatePurchaseStatus(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
        }
        public List<VPendingInwardInspection> GetPendingInspectionByFilter(SearchPendingInwardFilter searchFilter)
        {
            try
            {
                var query = _dbcontext.VPendingInwardInspection.AsQueryable();

                if (searchFilter.ItemID > 0) query = query.Where(o => o.ItemID == searchFilter.ItemID);
                if (searchFilter.ComponentTypeID > 0) query = query.Where(o => o.ComponentTypeID == searchFilter.ComponentTypeID);

                if (!string.IsNullOrEmpty(searchFilter.SearchInWord))
                {
                    string keyword = searchFilter.SearchInWord.ToLower(); // Convert to lowercase for case-insensitive search

                    query = query.Where(o =>

                        o.ItemID.ToString().Contains(keyword) ||
                        o.ItemName != null && o.ItemName.ToLower().Contains(keyword) ||
                        o.ItemCode != null && o.ItemCode.ToLower().Contains(keyword) ||
                         o.HSNCode != null && o.HSNCode.ToLower().Contains(keyword) ||
                          o.SizeName != null && o.SizeName.ToLower().Contains(keyword) ||
                          o.ColorName != null && o.ColorName.ToLower().Contains(keyword) ||
                          o.ComponentTypeName != null && o.ComponentTypeName.ToLower().Contains(keyword) ||
                          o.UnitName != null && o.UnitName.ToLower().Contains(keyword) ||
                          o.BatchNo != null && o.BatchNo.ToLower().Contains(keyword) ||
                        o.Quantity.ToString().Contains(keyword) ||
                          o.BatchQuantity.ToString().Contains(keyword)
                    );
                }
                _auditLogger.SaveActionLog("VPendingInwardInspection", ActionType.ListData, null, searchFilter, null, "PendingInspectionServiceRepository.GetPendingInwardByFilter()");
                return query.Where(w => w.PurchaseStatusID > 1).OrderBy(o => o.ItemName).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PendingInspectionServiceRepository.GetPendingInwardByFilter(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }
        public List<VPendingInwardInspection> GetCompletedPurchaseEntryByFilter(SearchPendingPurchase searchFilter)
        {
            try
            {
                var query = _dbcontext.VPendingInwardInspection.AsQueryable();

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
                _auditLogger.SaveActionLog("VPendingInwardInspection", ActionType.ListData, null, searchFilter, null, "PendingInspectionServiceRepository.GetPendingPurchaseEntryByFilter()");
                return query.Where(w => w.PurchaseStatusID > 1).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.GetPendingPurchaseEntryByFilter(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;

            }
        }
        public int SavePendingInspection(PendingInspection request)
        {
            int id = 0;
            try
            {
                using (var connection = (SqlConnection)_dbcontext.Database.GetDbConnection())
                {
                    using (var command = new SqlCommand("InsertOrUpdatePendingInwardInspection", connection))
                    {
                        command.CommandType = CommandType.;

                        command.Parameters.AddWithValue("@StatusFlag", request.StatusFlag);
                        command.Parameters.AddWithValue("@PendingInwardInspectionID", request.PendingInwardInspectionID);
                        command.Parameters.AddWithValue("@FK_PurchaseTransID", request.PurchaseTransID);
                        command.Parameters.AddWithValue("@BatchNo", request.BatchNo);
                        command.Parameters.AddWithValue("@BatchQuantity", request.BatchQuantity);
                        command.Parameters.AddWithValue("@IsDeleted", request.IsDeleted);
                        command.Parameters.AddWithValue("@LastUpdatedBy", request.LastUpdatedBy);
                        command.Parameters.AddWithValue("@IsAutoBatch", request.IsAutoBatch);
                        command.Parameters.AddWithValue("@LessQuantity", request.LessQuantity);
                        command.Parameters.AddWithValue("@ExcessQuantity", request.ExcessQuantity);
                        command.Parameters.AddWithValue("@PendingQuantity", request.PendingQuantity);

                        connection.Open();
                        id = command.ExecuteNonQuery();
                    }
                }
                _auditLogger.SaveActionLog("PendingInspection", ActionType.Insert, null, request, null, "PendingInspectionServiceRepository.SavePendingInspection()");
                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PendingInspectionServiceRepository.SavePendingInspection(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return id;
            }
        }
        public Tuple<bool, bool> Delete(int PendingInwardInspectionID, int UserID)
        {
            bool IsSuccess = false;
            bool doesTaxExist = false;
            try
            {
                var resultdata = _dbcontext.PendingInspection.FirstOrDefault(w => w.PendingInwardInspectionID == PendingInwardInspectionID);
                if (resultdata != null)
                {
                    resultdata.StatusFlag = "D";

                    //Validate Consumed Qty is zero (Modified on 2025.04.07)
                    var batchStockData = _dbcontext.VBatchStock.Where(w => w.BatchNo == resultdata.BatchNo).FirstOrDefault();
                    if (batchStockData != null && batchStockData.ConsumedQty == 0)
                    {
                        SavePendingInspection(resultdata);
                        IsSuccess = true;
                        doesTaxExist = true;
                    }
                }
                _auditLogger.SaveActionLog("PendingInspection", ActionType.Delete, PendingInwardInspectionID.ToString(), null, null, "PendingInspectionServiceRepository.Delete()");
                return Tuple.Create(IsSuccess, doesTaxExist);
            }
            catch (Exception ex)
            {
                _logger.LogError($"PendingInspectionServiceRepository.Delete(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return Tuple.Create(IsSuccess, doesTaxExist);
            }
        }
        public List<BatchStock> GetCompletedBatchStock(FilterForBatchStock FilterForBatchStock)
        {

            try
            {
                var query = _dbcontext.VBatchStock.AsQueryable();

                if (FilterForBatchStock.SizeID > 0) query = query.Where(o => o.SizeID == FilterForBatchStock.SizeID);
                if (FilterForBatchStock.ComponentTypeID > 0) query = query.Where(o => o.ComponentTypeID == FilterForBatchStock.ComponentTypeID);
                if (FilterForBatchStock.ColorID > 0) query = query.Where(o => o.ColorID == FilterForBatchStock.ColorID);
                if (FilterForBatchStock.WareHouseID > 0) query = query.Where(o => o.WareHouseID == FilterForBatchStock.WareHouseID);
                if (FilterForBatchStock.RackLocationID > 0) query = query.Where(o => o.RackLocationID == FilterForBatchStock.RackLocationID);
                if (!string.IsNullOrEmpty(FilterForBatchStock.SearchInWord))
                {
                    string keyword = FilterForBatchStock.SearchInWord.ToLower(); // Convert to lowercase for case-insensitive search

                    query = query.Where(o =>

                        o.ItemID.ToString().Contains(keyword) ||
                        o.ItemName != null && o.ItemName.ToLower().Contains(keyword) ||
                          o.SizeName != null && o.SizeName.ToLower().Contains(keyword) ||
                          o.ColorName != null && o.ColorName.ToLower().Contains(keyword) ||
                          o.ComponentTypeName != null && o.ComponentTypeName.ToLower().Contains(keyword) ||
                          o.BatchNo != null && o.BatchNo.ToLower().Contains(keyword) ||
                        o.ConsumedQty.ToString().Contains(keyword) ||
                         o.BatchQuantity.ToString().Contains(keyword) ||
                         o.BalanceQty.ToString().Contains(keyword) ||
                          o.BatchQuantity.ToString().Contains(keyword)
                    );
                }
                if (FilterForBatchStock.ReportTypeID == 1) query = query.Where(o => o.BalanceQty > 0);

                _auditLogger.SaveActionLog("VBatchStock", ActionType.ListData, null, FilterForBatchStock, null, "PendingInspectionServiceRepository.GetCompletedBatchStock()");
                return query.OrderBy(o => o.ComponentTypeName).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError($"PendingInspectionServiceRepository.GetCompletedBatchStock(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }


        }
        public List<BatchStock> GetComponenetStock(FilterForBatchStock FilterForBatchStock)
        {

            try
            {
                var query = _dbcontext.VBatchStock.AsQueryable();

                if (FilterForBatchStock.SizeID > 0) query = query.Where(o => o.SizeID == FilterForBatchStock.SizeID);
                if (FilterForBatchStock.ComponentTypeID > 0) query = query.Where(o => o.ComponentTypeID == FilterForBatchStock.ComponentTypeID);
                if (FilterForBatchStock.ColorID > 0) query = query.Where(o => o.ColorID == FilterForBatchStock.ColorID);
                if (!string.IsNullOrEmpty(FilterForBatchStock.SearchInWord))
                {
                    string keyword = FilterForBatchStock.SearchInWord.ToLower(); // Convert to lowercase for case-insensitive search

                    query = query.Where(o =>

                        o.ItemID.ToString().Contains(keyword) ||
                        o.ItemName != null && o.ItemName.ToLower().Contains(keyword) ||
                          o.SizeName != null && o.SizeName.ToLower().Contains(keyword) ||
                          o.ColorName != null && o.ColorName.ToLower().Contains(keyword) ||
                          o.ComponentTypeName != null && o.ComponentTypeName.ToLower().Contains(keyword) ||
                          o.BatchNo != null && o.BatchNo.ToLower().Contains(keyword) ||
                        o.ConsumedQty.ToString().Contains(keyword) ||
                         o.BatchQuantity.ToString().Contains(keyword) ||
                         o.BalanceQty.ToString().Contains(keyword) ||
                          o.BatchQuantity.ToString().Contains(keyword)
                    );
                }
                _auditLogger.SaveActionLog("VBatchStock", ActionType.ListData, null, FilterForBatchStock, null, "PendingInspectionServiceRepository.GetComponenetStock()");
                return query.OrderBy(o => o.ComponentTypeName).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError($"PendingInspectionServiceRepository.GetComponenetStock(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }


        }
        public DataResponse GetComponenetListdropdown()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var ComponenetType = _dbcontext.VComponentExceptType.OrderBy(o => o.ComponentTypeCode).ToList();

                DataResponse.Count = ComponenetType.Count;
                DataResponse.Message = Constants.RecordFound;
                DataResponse.Value = ComponenetType;
                _auditLogger.SaveActionLog("VComponentExceptType", ActionType.ListData, null, null, null,"PendingInspectionServiceRepository.GetComponenetListdropdown()");
            }
            catch (Exception ex)
            {
                string errorID = Guid.NewGuid().ToString();  // Generate Unique Error ID

                // Log error details into the central error database
                //await LogErrorAsync(errorID, ex, "SaveSurveyDetails", request);

                // Return a generic error message with the error tracking ID
                DataResponse.Error = true;
                DataResponse.Success = false;
                DataResponse.Message = $"An error occurred. Please contact support with Error ID: {errorID}";
            }
            return DataResponse;
        }
        public DataResponse GetNumberPlateStockReport(NumberPlateStockReportFilter request)
        {
            DataResponse DataResponse = new DataResponse();
            var list = new List<NumberPlateStockReportData>();

            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.GETNUMBERPLATESTOCKREPORT;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@SizeID", request.SizeID);
                        command.Parameters.AddWithValue("@ColorID", request.ColorID);
                        command.Parameters.AddWithValue("@BlankPlateID", request.BlankPlateID);
                        command.Parameters.AddWithValue("@HologramPlateID", request.HologramPlateID);
                        command.Parameters.AddWithValue("@LaserMarkingPlateID", request.LaserMarkingPlateID);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new NumberPlateStockReportData
                                {
                                    SizeName = reader["Size"]?.ToString(),
                                    ColorName = reader["Colour"]?.ToString(),
                                    UnitName = reader["UnitName"]?.ToString(),

                                    BlankPlate = reader.GetDecimal(reader.GetOrdinal("BlankPlate")),
                                    HologramPlate = reader.GetDecimal(reader.GetOrdinal("HologramPlate")),
                                    LaserMarkingPlate = reader.GetDecimal(reader.GetOrdinal("LaserMarkingPlate")),
                                    Packing = reader.GetDecimal(reader.GetOrdinal("Packing"))
                                });
                            }
                        }
                    }
                }

                DataResponse.Value = list;

                _auditLogger.SaveActionLog("NumberPlateStockReport", ActionType.ListData, null, null, null,
                    "PendingInspectionServiceRepository.GetNumberPlateStockReport()");

                return DataResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: {ex.Message} | Trace: {ex.StackTrace}");
                throw;
            }
        }

        public List<BatchStock> GetRawMaterialStockData(FilterRawMaterialData FilterForBatchStock)
        {
            try
            {
                var query = _dbcontext.VBatchStock.AsQueryable();

                if (FilterForBatchStock.SizeID > 0) query = query.Where(o => o.SizeID == FilterForBatchStock.SizeID);
                if (FilterForBatchStock.ComponentTypeID > 0) query = query.Where(o => o.ComponentTypeID == FilterForBatchStock.ComponentTypeID);
                if (FilterForBatchStock.ColorID > 0) query = query.Where(o => o.ColorID == FilterForBatchStock.ColorID);

                _auditLogger.SaveActionLog("VBatchStock", ActionType.ListData, null, FilterForBatchStock, null, "PendingInspectionServiceRepository.GetRawMaterialStockData()");
                return query.OrderBy(o => o.ComponentTypeName).ToList();

            }
            catch (Exception ex)
            {
                _logger.LogError($"PendingInspectionServiceRepository.GetRawMaterialStockData(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }


        }


    }
}