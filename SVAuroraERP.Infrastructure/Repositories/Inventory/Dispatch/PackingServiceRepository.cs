namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Dispatch
{
    public class PackingServiceRepository : IPackingServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<IPackingServiceRepository> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly IPackingTransServiceRepository _transServiceRepository;
        private readonly IErrorLoggerService _errorLoggerService;
        public PackingServiceRepository(SVAuroraERPDbContext dbcontext,
                                        ILogger<IPackingServiceRepository> logger,
                                        IAuditLogger auditLogger,
                                        IPackingTransServiceRepository transServiceRepository,
                                        IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _auditLogger = auditLogger;
            _transServiceRepository = transServiceRepository;
            _errorLoggerService = errorLoggerService;
        }
        //public List<VStockPacking> GetNumberPlateByFilter(PackingFilter searchFilter)
        //{
        //    try
        //    {
        //        var query = _dbcontext.VStockPacking.AsQueryable();

        //        if (searchFilter.BoxID > 0) query = query.Where(o => o.BOXID == searchFilter.BoxID);
        //        if (searchFilter.SizeID > 0) query = query.Where(o => o.SizeID == searchFilter.SizeID);
        //        if (searchFilter.ColorID > 0) query = query.Where(o => o.BOXID == searchFilter.ColorID);


        //        return query.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;

        //    }
        //}
        public List<AvailableLaserNoDto> GetAvailableLaserNos(PackingFilter request)
        {
            try
            {
                var result = new List<AvailableLaserNoDto>();

                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.GETAVAILABLELASERNOS;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var fkSizeIDParam = command.CreateParameter();
                        fkSizeIDParam.ParameterName = "@FK_SizeID";
                        fkSizeIDParam.Value = request.SizeID;

                        var fkColorIDParam = command.CreateParameter();
                        fkColorIDParam.ParameterName = "@FK_ColorID";
                        fkColorIDParam.Value = request.ColorID;

                        command.Parameters.Add(fkSizeIDParam);
                        command.Parameters.Add(fkColorIDParam);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new AvailableLaserNoDto
                                {
                                    BatchStockID = reader.GetInt32(0),
                                    StartingNo = reader.GetInt32(1),
                                    EndingNo = reader.GetInt32(2),
                                    PlateCount = reader.GetInt32(3),
                                    BatchNo = reader.GetString(4),
                                    ItemName = reader.GetString(5),
                                    StartLaserNo = reader.GetString(6),
                                    EndLaserNo = reader.GetString(7)
                                });
                            }
                        }
                    }
                }
                _auditLogger.SaveActionLog("AvailableLaserNoDto", ActionType.ListData, null, request, null, "StockReportServiceRepository.GetAvailableLaserNos()");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"StockReportServiceRepository.GetAvailableLaserNos(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }

        public Tuple<bool, int> Save(Packing request)
        {
            bool IsSuccess = false;
            string? RequestNo = null;
            int Result = 0;

            try
            {
                Result = SavePacking(request);

                foreach (var packingtrans in request.PackingTrans)
                {
                    packingtrans.PackingID = Result;
                    _transServiceRepository.AddPackingTrans(packingtrans);
                }
                _auditLogger.SaveActionLog("Packing", ActionType.Insert, Result.ToString(), request, null, "StockReportServiceRepository.Save()");
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"StockReportServiceRepository.Save(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                IsSuccess = false;
            }

            return Tuple.Create(IsSuccess, Result);
        }
        public int SavePacking(Packing request)
        {
            int id = 0;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.INSERTPACKING;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var pkPackingIDParam = command.CreateParameter();
                        pkPackingIDParam.ParameterName = "@PK_PackingID";
                        pkPackingIDParam.Direction = System.Data.ParameterDirection.Output;
                        pkPackingIDParam.DbType = System.Data.DbType.Int32;
                        pkPackingIDParam.Value = 0;

                        var PackingDateParam = command.CreateParameter();
                        PackingDateParam.ParameterName = "@PackingDate";
                        PackingDateParam.Value = request.PackingDate;

                        var fkboxIDParam = command.CreateParameter();
                        fkboxIDParam.ParameterName = "@FK_BoxID";
                        fkboxIDParam.Value = request.BOXID;

                        var fkColoridParam = command.CreateParameter();
                        fkColoridParam.ParameterName = "@FK_ColorID";
                        fkColoridParam.Value = request.ColorID;

                        var lastUpdatedByParam = command.CreateParameter();
                        lastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                        lastUpdatedByParam.Value = request.LastUpdatedBy;

                        var fkAllotedToIDParam = command.CreateParameter();
                        fkAllotedToIDParam.ParameterName = "@FK_AllotedToID";
                        fkAllotedToIDParam.Value = request.AllotedToID;


                        command.Parameters.Add(pkPackingIDParam);
                        command.Parameters.Add(fkboxIDParam);
                        command.Parameters.Add(fkColoridParam);
                        command.Parameters.Add(PackingDateParam);
                        command.Parameters.Add(lastUpdatedByParam);
                        command.Parameters.Add(fkAllotedToIDParam);


                        command.ExecuteNonQuery();
                        id = (int)pkPackingIDParam.Value;
                    }
                }
                _auditLogger.SaveActionLog("Packing", ActionType.Insert, request.AllotedToID.ToString(), request, null, "PackingServiceRepository.SavePacking()");
                return id;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "PackingServiceRepository.SavePacking()");
            }
            return id;
        }
        public List<VPacking> GetPackingList()
        {
            try
            {
                var resultdata = _dbcontext.VPacking.OrderBy(o => o.PackingNo).ToList();
                _auditLogger.SaveActionLog("Packing", ActionType.ListData, null, resultdata, null, "PackingServiceRepository.GetPackingList()");
                return resultdata;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PackingServiceRepository.GetPackingList(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }
        public VPacking GetByID(int ID)
        {
            try
            {
                var resultdata = _dbcontext.VPacking.FirstOrDefault(w => w.PackingID == ID);
                resultdata.PackingTrans = _transServiceRepository.GetPackingTransByID(ID);
                _auditLogger.SaveActionLog("Packing", ActionType.Select, ID.ToString(), resultdata, null, "StockReportServiceRepository.GetByID()");
                return resultdata;
            }
            catch (Exception ex)
            {
                _logger.LogError($"StockReportServiceRepository.GetByID(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;

            }
        }
        public Tuple<bool, bool> Delete(int PackingID,int LastUpdatedBy)
        {
            bool IsSuccess = false;
            bool doesUnitExist = false;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.DELETEPACKINGTRANS;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var PackingDateParam = command.CreateParameter();
                        PackingDateParam.ParameterName = "@FK_PackingID";
                        PackingDateParam.Value = PackingID;

                        var lastUpdatedByParam = command.CreateParameter();
                        lastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                        lastUpdatedByParam.Value = LastUpdatedBy;

                        command.Parameters.Add(PackingDateParam);
                        command.Parameters.Add(lastUpdatedByParam);


                        command.ExecuteNonQuery();
                         IsSuccess = true;
                         doesUnitExist = true;
                    }
                }

                _auditLogger.SaveActionLog("Packing", ActionType.Delete, PackingID.ToString(), null, null, "PurchaseEntryServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.Delete(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                IsSuccess = false;
            }
            return Tuple.Create(IsSuccess, doesUnitExist);
        }
        public List<VPacking> GetPackingListByStatus(int AllotedToID)
        {
            try
            {
                var resultdata = _dbcontext.VPacking.Where(o => o.StatusID == 1 && o.AllotedToID== AllotedToID).ToList();
                _auditLogger.SaveActionLog("Packing", ActionType.ListData, null, resultdata, null, "PackingServiceRepository.GetPackingListByStatus()");
                return resultdata;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PackingServiceRepository.GetPackingListByStatus(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }
}
