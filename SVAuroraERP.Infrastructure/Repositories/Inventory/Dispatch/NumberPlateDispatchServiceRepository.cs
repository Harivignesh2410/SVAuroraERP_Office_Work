//Added on 2025.04.29 by Harivignesh
namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Dispatch
{
    public class NumberPlateDispatchServiceRepository : INumberPlateDispatchServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly INumberPlateDispatchServiceRepositoryTrans _numberPlateDispatchServiceRepositoryTrans;
        private readonly ILogger<NumberPlateDispatchServiceRepository> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public NumberPlateDispatchServiceRepository(SVAuroraERPDbContext dbContext,
                                                    IAuditLogger auditLogger,
                                                    INumberPlateDispatchServiceRepositoryTrans numberPlateDispatchServiceRepositoryTrans,
                                                    ILogger<NumberPlateDispatchServiceRepository> logger,
                                                    IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbContext;
            _auditLogger = auditLogger;
            _numberPlateDispatchServiceRepositoryTrans = numberPlateDispatchServiceRepositoryTrans;
            _logger = logger;
            _errorLoggerService = errorLoggerService;
        }
        public Tuple<bool, string> Save(NumberPlateDispatch request)
        {
            bool IsSuccess = false;
            string? RequestNo = null;
            Tuple<int, string?> Result = null;

            try
            {
                Result = SaveNumberPlateDispatch(request);

                foreach (var numberPlateDispatchTrans in request.NumberPlateDispatchTrans)
                {
                    numberPlateDispatchTrans.NumberPlateDispatchID = Result.Item1;
                    _numberPlateDispatchServiceRepositoryTrans.Save(numberPlateDispatchTrans);
                }
                _auditLogger.SaveActionLog("NumberPlateDispatch", ActionType.Insert, null, request, null, "NumberPlateDispatchServiceRepository.Save()");
                IsSuccess = true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"NumberPlateDispatchServiceRepository.Save(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                IsSuccess = false;
            }

            return Tuple.Create(IsSuccess, Result.Item2);
        }
        public Tuple<int, string> SaveNumberPlateDispatch(NumberPlateDispatch request)
        {
            int id = 0;
            string DispatchNo = string.Empty;

            using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = StoredProcedure.INSERTNUMBERPLATEDISPATCH;
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    var pkNumberPlateDispatchIDParam = command.CreateParameter();
                    pkNumberPlateDispatchIDParam.ParameterName = "@PK_NumberPlateDispatchID";
                    pkNumberPlateDispatchIDParam.Direction = System.Data.ParameterDirection.Output;
                    pkNumberPlateDispatchIDParam.DbType = System.Data.DbType.Int32;
                    pkNumberPlateDispatchIDParam.Value = 0;

                    var DispatchDateParam = command.CreateParameter();
                    DispatchDateParam.ParameterName = "@DispatchDate";
                    DispatchDateParam.Value = request.DispatchDate;

                    var ModeofTransportIDParam = command.CreateParameter();
                    ModeofTransportIDParam.ParameterName = "@ModeofTransportID";
                    ModeofTransportIDParam.Value = request.ModeofTransportID;

                    var fkCourierIDParam = command.CreateParameter();
                    fkCourierIDParam.ParameterName = "@FK_CourierID";
                    fkCourierIDParam.Value = request.CourierID ?? (object)DBNull.Value;

                    var OwnVehicleDetailsParam = command.CreateParameter();
                    OwnVehicleDetailsParam.ParameterName = "@OwnVehicleDetails";
                    OwnVehicleDetailsParam.Value = request.OwnVehicleDetails ?? (object)DBNull.Value;

                    var DocketNoParam = command.CreateParameter();
                    DocketNoParam.ParameterName = "@DocketNo";
                    DocketNoParam.Value = request.DocketNo;

                    var DocketBookingDateParam = command.CreateParameter();
                    DocketBookingDateParam.ParameterName = "@DocketBookingDate";
                    DocketBookingDateParam.Value = request.DocketBookingDate;

                    var fkEmbossingStationIDParam = command.CreateParameter();
                    fkEmbossingStationIDParam.ParameterName = "@FK_EmbossingStationID";
                    fkEmbossingStationIDParam.Value = request.EmbossingStationID;

                    var lastUpdatedByParam = command.CreateParameter();
                    lastUpdatedByParam.ParameterName = "@LastUpdatedBy";
                    lastUpdatedByParam.Value = request.LastUpdatedBy;

                    command.Parameters.Add(pkNumberPlateDispatchIDParam);
                    command.Parameters.Add(DispatchDateParam);
                    command.Parameters.Add(ModeofTransportIDParam);
                    command.Parameters.Add(fkCourierIDParam);
                    command.Parameters.Add(OwnVehicleDetailsParam);
                    command.Parameters.Add(DocketNoParam);
                    command.Parameters.Add(DocketBookingDateParam);
                    command.Parameters.Add(fkEmbossingStationIDParam);
                    command.Parameters.Add(lastUpdatedByParam);

                    command.ExecuteNonQuery();
                    id = (int)pkNumberPlateDispatchIDParam.Value;
                    DispatchNo = _dbcontext.NumberPlateDispatch
                                               .Where(w => w.NumberPlateDispatchID == id)
                                               .Select(w => w.DispatchNo)
                                               .FirstOrDefault();
                }
            }
            _auditLogger.SaveActionLog("NumberPlateDispatch", ActionType.Insert, null, request, null, "NumberPlateDispatchServiceRepository.SaveNumberPlateDispatch()");
            return Tuple.Create(id, DispatchNo);
        }
        public List<VNumberPlateDispatch> GetNumberPlateDispatchList()
        {
            try
            {
                var resultdata = _dbcontext.VNumberPlateDispatch.OrderBy(o => o.DispatchNo).ToList();
                _auditLogger.SaveActionLog("NumberPlateDispatch", ActionType.ListData, null, resultdata, null, "NumberPlateDispatchServiceRepository.GetNumberPlateDispatchList()");
                return resultdata;
            }
            catch (Exception ex)
            {
                _logger.LogError($"NumberPlateDispatchServiceRepository.GetNumberPlateDispatchList(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }
        public DataResponse Delete(int NumberPlateDispatchID, int LastUpdatedBy)
        {
            DataResponse response = new DataResponse();
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.DELETENUMBERPLATEDISPATCH;
                        command.CommandType = CommandType.StoredProcedure;

                        // Parameters
                        command.Parameters.AddWithValue("@PK_NumberPlateDispatchID", NumberPlateDispatchID);
                        command.Parameters.AddWithValue("@LastUpdatedBy", LastUpdatedBy);

                        // Execute the stored procedure
                        command.ExecuteNonQuery();

                        response.Success = true;
                        response.Message = Constants.SuccessMessage;
                    }
                }

                _auditLogger.SaveActionLog("NumberPlateDispatch", ActionType.Delete, NumberPlateDispatchID.ToString(), null, null, "NumberPlateDispatchServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                _errorLoggerService.LogException(ex, NumberPlateDispatchID, "NumberPlateDispatchServiceRepository.Delete()");

            }
            return response;
        }
        public List<VNumberPlateDispatchTrans> GetPackingByNumberPlateDispatchID1(int NumberPlateDispatchID)
        {
            try
            {
                var result = new List<VNumberPlateDispatchTrans>();
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.GETPACKINGBYNUMBERPLATEDISPATCHID;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var param = command.CreateParameter();
                        param.ParameterName = "@FK_NumberPlateDispatchID";
                        param.Value = NumberPlateDispatchID;
                        command.Parameters.Add(param);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new VNumberPlateDispatchTrans
                                {
                                    NumberPlateDispatchTransID = reader.GetInt32(0),
                                    NumberPlateDispatchID = reader.GetInt32(1),
                                    PackingID = reader.GetInt32(2),
                                    ColorName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    SizeName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                                    PackingNo = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                    DispatchNo = reader.IsDBNull(6) ? "" : reader.GetString(6),
                                    DispatchDate = reader.IsDBNull(7) ? "" : reader.GetString(7), // string since nvarchar
                                  
                                });
                            }
                        }
                    }
                }

                _auditLogger.SaveActionLog("NumberPlateDispatchTrans", ActionType.Select, NumberPlateDispatchID.ToString(), NumberPlateDispatchID, null, "NumberPlateDispatchServiceRepository.GetPackingByNumberPlateDispatchID()");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetPackingByNumberPlateDispatchID Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }
        public VNumberPlateDispatch GetNumberPlateDispatchByID(int ID)
        {
            try
            {
                var resultdata = _dbcontext.VNumberPlateDispatch.FirstOrDefault(w => w.NumberPlateDispatchID == ID);
                resultdata.NumberPlateDispatchTrans = _numberPlateDispatchServiceRepositoryTrans.GetNumberPlateDispatchTransByID(ID);
                _auditLogger.SaveActionLog("NumberPlateDispatch", ActionType.Select, ID.ToString(), resultdata, null, "NumberPlateDispatchServiceRepository.GetByID()");
                return resultdata;
            }
            catch (Exception ex)
            {
                _logger.LogError($"NumberPlateDispatchServiceRepository.GetByID(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;

            }
        }
        public DataResponse InsertHSRPLaserStockTransID(int packingID, int lastUpdatedBy)
        {
            var response = new DataResponse();

            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.INSERTHSRPLASERSTOCKTRANSID; // constant name for SP
                        command.CommandType = CommandType.StoredProcedure;

                        // Parameters
                        command.Parameters.AddWithValue("@PackingID", packingID);
                        command.Parameters.AddWithValue("@LastUpdatedBy", lastUpdatedBy);

                        // Execute the stored procedure
                        command.ExecuteNonQuery();

                        response.Success = true;
                        response.Message = "HSRP Laser Stock inserted successfully.";
                    }
                }

                _auditLogger.SaveActionLog("tHSRPLaserNoStock", ActionType.Insert, packingID.ToString(), lastUpdatedBy, null, "HSRPLaserStockRepository.InsertHSRPLaserStockTransID()");

            }
            catch (SqlException ex)
            {
                response.Success = false;
                response.Message = $"SQL Error: {ex.Message}";
                _logger.LogError($"InsertHSRPLaserStockTransID SQL Error: {ex.Message}, StackTrace: {ex.StackTrace}");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Error: {ex.Message}";
                _logger.LogError($"InsertHSRPLaserStockTransID Error: {ex.Message}, StackTrace: {ex.StackTrace}");
            }

            return response;
        }
        public List<VPacking> GetPackingByNumberPlateDispatchID(int NumberPlateDispatchID)
        {
            try
            {
                var result = new List<VPacking>();
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {

                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.GETPACKINGBYNUMBERPLATEDISPATCHID;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var fkNumberPlateDispatchIDParam = command.CreateParameter();
                        fkNumberPlateDispatchIDParam.ParameterName = "@FK_NumberPlateDispatchID";
                        fkNumberPlateDispatchIDParam.Value = NumberPlateDispatchID;

                        command.Parameters.Add(fkNumberPlateDispatchIDParam);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(new VPacking
                                {
                                    PackingID = reader.GetInt32(0),
                                    PackingNo = reader.GetString(1),
                                    PackingDate = reader.GetString(2),
                                    BOXID = reader.GetInt32(3),
                                    BoxName = reader.GetString(4),
                                    ColorID = reader.GetInt32(5),
                                    ColorName = reader.GetString(6),
                                    LastUpdatedBy = reader.GetInt32(7),
                                    LastUpdatedDate = reader.GetDateTime(8),
                                    LastUpdatedByName = reader.GetString(9),
                                    BoxCount = reader.GetInt32(10),
                                    TotalQuantity = reader.GetDecimal(11),
                                    PcsPerBox = reader.GetDecimal(12),
                                    SizeName = reader.GetString(13),
                                    StatusID = reader.GetByte(14),
                                    StatusName = reader.GetString(15),
                                    ColorCode = reader.GetString(16)
                                }); 
                            }
                        }

                    }
                }
                _auditLogger.SaveActionLog("NumberPlateDispatch", ActionType.Select, NumberPlateDispatchID.ToString(), NumberPlateDispatchID, null, "PurchaseEntryServiceRepository.GetPackingByNumberPlateDispatchID()");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PurchaseEntryServiceRepository.GetPackingByNumberPlateDispatchID(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;

            }
        }

    }

}
