namespace SVAuroraERP.Infrastructure.Repositories.Orders.ManageOrder
{
    public class LaserNoAllocationServiceRepository : ILaserNoAllocationServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public LaserNoAllocationServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse GetLaserNoAllocation(ReadyforProcessingOrdersRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                var pageSize = Math.Clamp(request.Length, 1, 100);
                var skip = Math.Max(request.Start, 0);

                IQueryable<VGetReadyforProcessingOrders> query = _dbcontext.VGetReadyforProcessingOrders;

                //var hsrpUser = _dbcontext.VHSRPUser
                //          .FirstOrDefault(u => u.HSRPUserID == request.EmbossingStationID);

                //if (hsrpUser != null)
                //{
                //  //  request.EmbossingStationID = hsrpUser.UserID;

                    
                //}


                // 🔹 Apply filters
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.OrderNo.Contains(request.SearchValue) ||
                                             d.OrderTypeName.Contains(request.SearchValue)||
                                             d.RegNo.Contains(request.SearchValue) ||
                                             d.Dealer.Contains(request.SearchValue)||
                                             d.DealerCode.Contains(request.SearchValue) ||
                                             d.OEM.Contains(request.SearchValue)||
                                             d.ChasisNo.Contains(request.SearchValue)||
                                             d.EngineNo.Contains(request.SearchValue)||
                                             d.RearLaserSerialNo.Contains(request.SearchValue)||
                                             d.FrontLaserSerialNo.Contains(request.SearchValue)||
                                             d.FrontPlateDimension.Contains(request.SearchValue)||
                                             d.RearPlateDimension.Contains(request.SearchValue));
                }
                if (request.StartDate.HasValue) query = query.Where(w => w.OrderDate >= request.StartDate);
                if (request.EndDate.HasValue) query = query.Where(w => w.OrderDate <= request.EndDate);
                if (request.orderTypeID > 0) query = query.Where(w => w.OrderTypeID == request.orderTypeID);
                if (request.OEMID > 0) query = query.Where(w => w.OEMID == request.OEMID);
                if (request.DealerID > 0) query = query.Where(w => w.DealerID == request.DealerID);
                if (request.EmbossingStationID > 0) query = query.Where(w => w.EmbossingStationID == request.EmbossingStationID);

                // 🔹 Totals
                var totalRecords = _dbcontext.VGetReadyforProcessingOrders.Count();
                var filteredRecords = query.Count();

                // 🔹 Sorting + Paging
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");
                var pagedData = query.Skip(skip).Take(pageSize)
                                     .Select(w => new
                                     {
                                         w.HSRPOrderID,
                                         w.OrderTypeID,
                                         w.OrderTypeName,
                                         w.OrderNo,
                                         w.OrderDate,
                                         w.sOrderDate,
                                         w.ssOrderDate,
                                         w.DealerPONo,
                                         w.DealerSONo,
                                         w.DealerID,
                                         w.Dealer,
                                         w.OEMID,
                                         w.OEM,
                                         w.EmbossingStationID,
                                         w.EmbossingStation,
                                         w.OrderStatusID,
                                         w.Description,
                                         w.ColorCode,
                                         w.IconCode,
                                         w.DealerCode,
                                         w.OEMCode,
                                         w.EmbossingStationCode,
                                         w.DealerCity,
                                         w.OEMCity,
                                         w.EmbossingStationCity,
                                         w.ProcessDate,
                                         w.HSRPVehicleInfoID,
                                         w.HSRPOrderRefID,
                                         w.RegNo,
                                         w.RegDate,
                                         w.sRegDate,
                                         w.ChasisNo,
                                         w.EngineNo,
                                         w.sProcessDate,
                                         w.PlateColor,
                                         w.RearLaserSerialNo,
                                         w.FrontLaserSerialNo,
                                         w.LastUpdatedBy,
                                         w.LastUpdatedDate,
                                         w.PartNo,
                                         w.FrontPlateSize,
                                         w.RearPlateSize,
                                         w.ssRegDate,
                                         w.LastUpdatedByName
                                     }).ToList();

                // 🔹 Prepare response
                response.Value = pagedData.OrderByDescending(w => w.OrderNo); 
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;

                _auditLogger.SaveActionLog("VGetReadyforProcessingOrders", ActionType.ListData, null, request, null, "LaserNoAllocationServiceRepository.GetLaserNoAllocation()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "LaserNoAllocationServiceRepository.GetLaserNoAllocation()");
            }

            return response;
        }
        public DataResponse CheckAvailableOrderLaserNo(CheckAvailableOrderLaserNoRequest request)
        {
            DataResponse response = new DataResponse();

            try
            {
                if (request == null || string.IsNullOrWhiteSpace(request.OrderIds))
                    return new DataResponse { Message = "Empty request." };

                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                using var command = new SqlCommand(
                    StoredProcedure.CHECKAVAILABLEORDERLASERNO, connection);

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@FK_OrderID", request.OrderIds);

                connection.Open();

                using var reader = command.ExecuteReader();

                LaserAvailabilityResult result = new LaserAvailabilityResult();
                if (reader.Read())
                {
                    result.Summary = new LaserAvailabilitySummary
                    {
                        TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                        FrontLaserAvailable = reader.GetInt32(reader.GetOrdinal("FrontLaserAvailable")),
                        RearLaserAvailable = reader.GetInt32(reader.GetOrdinal("RearLaserAvailable")),
                        BothLaserAvailable = reader.GetInt32(reader.GetOrdinal("BothLaserAvailable")),
                        RejectedCount = reader.GetInt32(reader.GetOrdinal("RejectedCount")),
                        RejectedReasons = reader.IsDBNull(reader.GetOrdinal("RejectedReasons"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("RejectedReasons"))
                    };
                }
                if (reader.NextResult())
                {
                    while (reader.Read())
                    {
                        result.ValidOrderIds.Add(
                            reader.GetInt32(reader.GetOrdinal("FK_OrderID"))
                        );
                    }
                }

                response.Success = true;
                response.Value = result;
                response.Message = Constants.SuccessMessage;

                _auditLogger.SaveActionLog(
                    "Database",
                    ActionType.ListData,
                    null,
                    request,
                    null,
                    "LaserNoAllocationServiceRepository.CheckAvailableOrderLaserNo()");
            }
            catch (SqlException sqlEx)
            {
                response.Success = false;
                response.Message = sqlEx.Message;
                response.ID = 0;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(
                    ex,
                    request,
                    "LaserNoAllocationServiceRepository.CheckAvailableOrderLaserNo()");
            }

            return response;
        }

        public DataResponse Save(HSRPlaserStockRequest request)
        {
            DataResponse response = new DataResponse();

            try
            {
                if (request == null)
                    return new DataResponse { Message = "Empty request." };

                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                string infoMessage = string.Empty;

                //  Capture RAISERROR / PRINT messages
                connection.InfoMessage += (sender, e) =>
                {
                    infoMessage = e.Message;
                };
                connection.Open();

                using var command = new SqlCommand(StoredProcedure.ALLOCATEORDERLASERNO, connection);
                command.CommandType = CommandType.StoredProcedure;

                // OUTPUT parameter

                command.Parameters.AddWithValue("@FK_OrderID", request.OrderIds);
                command.Parameters.AddWithValue("@LastUpdatedBy", request.LastUpdatedBy);

                command.ExecuteNonQuery();
                response.Message = !string.IsNullOrEmpty(infoMessage) ? infoMessage : Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Database", ActionType.Insert, null, request, null, "LaserNoAllocationServiceRepository.Save()");

            }
            catch (SqlException sqlEx)  // catch DB errors
            {
                response.Message = sqlEx.Message;
                response.ID = 0;  // invalid
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "LaserNoAllocationServiceRepository.Save()");
            }

            return response;
        }
    }
}