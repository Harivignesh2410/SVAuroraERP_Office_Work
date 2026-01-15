using Azure;
using SVAuroraERP.Domain.Inventory.ScrapManagement;

namespace SVAuroraERP.Infrastructure.Repositories.Inventory.ScrapManagement
{
    public class ScrapEntryServiceRepository:IScrapEntryServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IScrapEntryTransServiceRepository _scrapEntryTransServiceRepository;
        public ScrapEntryServiceRepository(SVAuroraERPDbContext dbcontext,
                                              IAuditLogger auditLogger,
                                              IErrorLoggerService errorLoggerService,
                                              IScrapEntryTransServiceRepository scrapEntryTransServiceRepository)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
            _scrapEntryTransServiceRepository = scrapEntryTransServiceRepository;
        }

        public DataResponse GetScrapDataByComponentTypeID(ScrapDataParameter request)
        {
            DataResponse dataResponse = new DataResponse();

            List<ScrapData> scrapList = new List<ScrapData>();

            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.GETSCRAPDATABYCOMPONENTTYPEID;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@AluminumCoil", request.ALUMINUMCOILID);
                        command.Parameters.AddWithValue("@BLANKPLATE", request.BLANKPLATEID);
                        command.Parameters.AddWithValue("@HOLOGRAMPLATE", request.HOLOGRAMPLATEID);
                        command.Parameters.AddWithValue("@ScrapEntryID", request.SCRAPENTRYID);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                scrapList.Add(new ScrapData
                                {
                                    FK_ComponentTypeID = reader.GetInt32(reader.GetOrdinal("FK_ComponentTypeID")),
                                    ComponentTypeName = reader["ComponentTypeName"].ToString(),
                                    FK_SizeID = reader.GetInt32(reader.GetOrdinal("FK_SizeID")),
                                    SizeName = reader["SizeName"].ToString(),
                                    ProdWastageQty = reader.GetDecimal(reader.GetOrdinal("ProdWastageQty")),
                                    PerPlate = reader.IsDBNull(reader.GetOrdinal("PerPlate")) ? 0 : reader.GetDecimal(reader.GetOrdinal("PerPlate")),
                                    WastageQtyInKG = reader.IsDBNull(reader.GetOrdinal("WastageQtyInKG")) ? 0 : reader.GetDecimal(reader.GetOrdinal("WastageQtyInKG")),
                                    BalanceQty = reader.IsDBNull(reader.GetOrdinal("BalanceQty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("BalanceQty")),
                                    SoldQty = reader.IsDBNull(reader.GetOrdinal("SoldQty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("SoldQty")),
                                    TransSoldQty = reader.IsDBNull(reader.GetOrdinal("SoldQty")) ? 0 : reader.GetDecimal(reader.GetOrdinal("TransSoldQty")),
                                    ScrapEntryTransID = reader.GetInt32(reader.GetOrdinal("ScrapEntryTransID")),
                                });
                            }
                        }
                    }
                }

                dataResponse.Value = scrapList;
                dataResponse.Count = scrapList.Count;
                dataResponse.Message = Constants.RecordFound;

                // Log action for auditing
                _auditLogger.SaveActionLog("ScrapManagement", ActionType.Select,
                    $"{request.ALUMINUMCOILID},{request.BLANKPLATEID},{request.HOLOGRAMPLATEID}",
                    null, null, "ScrapManagementServiceRepository.GetScrapDataByComponentTypeID()");
            }
            catch (Exception ex)
            {
                dataResponse=_errorLoggerService.LogException(ex, new { request.ALUMINUMCOILID, request.BLANKPLATEID, request.HOLOGRAMPLATEID },
                    "ScrapManagementServiceRepository.GetScrapDataByComponentTypeID()");
            }
            return dataResponse;
        }
        public DataResponse GetScrapDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VScrapEntry> query = _dbcontext.VScrapEntry;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.ScrapEntryNo ?? "").Contains(request.SearchValue)||
                    (d.sScrapDate ?? "").Contains(request.SearchValue)
                    );
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VScrapEntry.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.ScrapEntryID,
                                           w.ScrapEntryNo,
                                           w.sScrapDate,
                                           w.TotalSoldQty,
                                           w.ComponentSizeList
                                       }).ToList();
                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VScrapEntry", ActionType.ListData, null, request, null, "ScrapEntryServiceRepository.GetScrapDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ScrapEntryServiceRepository.GetScrapDataTable()");
            }
            return response;
        }
        public DataResponse Save(ScrapEntry request)
        {
            int newId = 0;
            DataResponse response = new DataResponse();
            try
            {
                if (request == null)
                {
                    response.Message = "Empty request.";
                    return response;
                }

                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = new SqlCommand(Domain.StoredProcedure.INSERTSCRAPENTRY, connection)) 
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        SqlParameter pkParam = new SqlParameter("@PK_ScrapEntryID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(pkParam);

                        command.Parameters.Add(new SqlParameter("@ScrapDate", request.ScrapDate));
                        command.Parameters.Add(new SqlParameter("@TotalSoldQty", request.TotalSoldQty));
                        command.Parameters.Add(new SqlParameter("@LastUpdatedBy", request.LastUpdatedBy));
                      
                        command.ExecuteNonQuery();

                        newId = (int)pkParam.Value;
                    }
                }
                if (request.ScrapEntryTransList != null)
                {
                    foreach (var trans in request.ScrapEntryTransList)
                    {
                        trans.ScrapEntryID = newId;
                        _scrapEntryTransServiceRepository.SaveScrapEntryTransDetails(trans);
                    }
                }

                response.ID = newId;
                response.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("ScrapEntry", ActionType.Insert, null, request, null, "ScrapEntryServiceRepository.Save()");
            }

            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ScrapEntryServiceRepository.Save()");
            }
            return response;
        }
        public DataResponse Delete(int ScrapEntryID, int LastUpdatedBy)
        {
            DataResponse response = new DataResponse();

            try
            {
                var StockTransferByID = _dbcontext.VScrapEntry.Where(w => w.ScrapEntryID == ScrapEntryID).FirstOrDefault();
                if (StockTransferByID == null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.ID = 0;
                    response.Message = Constants.NoRecordFound;

                    return response;
                }

                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = new SqlCommand(Domain.StoredProcedure.DELETESCRAPENTRYDATA, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.Add(new SqlParameter("@PK_ScrapEntryID", ScrapEntryID));
                        command.Parameters.Add(new SqlParameter("@LastUpdatedBy", LastUpdatedBy));

                        command.ExecuteNonQuery();

                        response.Message = Constants.SuccessMessage;
                        response.ID = ScrapEntryID;
                    }
                }
                _auditLogger.SaveActionLog("VScrapEntry", ActionType.Delete, ScrapEntryID.ToString(), ScrapEntryID, null, "ScrapEntryServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, ScrapEntryID, "ScrapEntryServiceRepository.Delete()");
            }
            return response;
        }
        public DataResponse GetScrapEntryByID(int ScrapEntryID)
        {
            DataResponse response = new DataResponse();

            try
            {
                var StockTransferByID = _dbcontext.VScrapEntry.Where(w => w.ScrapEntryID == ScrapEntryID).FirstOrDefault();
                if (StockTransferByID == null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.ID = 0;
                    response.Message = Constants.NoRecordFound;

                    return response;
                }

               response.Value = StockTransferByID;
                response.Message = Constants.RecordFound;   
                response.ID = ScrapEntryID;
                response.Count = 1;

                _auditLogger.SaveActionLog("VScrapEntry", ActionType.Delete, ScrapEntryID.ToString(), ScrapEntryID, null, "ScrapEntryServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, ScrapEntryID, "ScrapEntryServiceRepository.Delete()");
            }
            return response;
        }

        //Added on 2025/11/22 by Harivignesh

        public DataResponse GetScrapStockData(ScrapDataFilterParameter request)
        {
            DataResponse dataResponse = new DataResponse();

            List<ScrapStockData> scrapList = new List<ScrapStockData>();

            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.GETSCRAPSTOCK;
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@AluminumCoil", request.AluminumCoil);
                        command.Parameters.AddWithValue("@BLANKPLATE", request.BlankPlate);
                        command.Parameters.AddWithValue("@HOLOGRAMPLATE", request.HologramPlate);
                        command.Parameters.AddWithValue("@SizeID", request.SizeID);
                        command.Parameters.AddWithValue("@ComponentTypeID", request.ComponentTypeID);

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                scrapList.Add(new ScrapStockData
                                {
                                    ComponentTypeName = reader["ComponentTypeName"].ToString(),
                                    SizeName = reader["SizeName"].ToString(),
                                    TotalScrap = reader.GetDecimal(reader.GetOrdinal("TotalScrap")),
                                    SoldQty = reader.GetDecimal(reader.GetOrdinal("SoldQty")),
                                    BalanceQty = reader.GetDecimal(reader.GetOrdinal("BalanceQty"))
                                });
                            }
                        }
                    }
                }

                dataResponse.Value = scrapList;
                dataResponse.Count = scrapList.Count;
                dataResponse.Message = Constants.RecordFound;

                // Log action for auditing
                _auditLogger.SaveActionLog("ScrapDataFilterParameter", ActionType.Select, null, request, null, "ScrapEntryServiceRepository.GetScrapStockData()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "ScrapEntryServiceRepository.GetScrapStockData()");
            }
            return dataResponse;
        }


    }
}
