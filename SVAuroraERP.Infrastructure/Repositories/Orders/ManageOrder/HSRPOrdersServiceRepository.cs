namespace SVAuroraERP.Infrastructure.Repositories.Orders.ManageOrder
{
    public class HSRPOrdersServiceRepository : IHSRPOrdersServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public HSRPOrdersServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse GetHsrporder(HsrpOrderRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHSRPOrder> query = _dbcontext.VHSRPOrder;

            
                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.OrderNo.Contains(request.SearchValue) ||
                                           d.OrderTypeName.Contains(request.SearchValue) ||
                                           d.RegNo.Contains(request.SearchValue) ||
                                           d.Dealer.Contains(request.SearchValue) ||
                                           d.DealerCity.Contains(request.SearchValue) ||
                                           d.OEM.Contains(request.SearchValue) ||
                                           d.OEMCity.Contains(request.SearchValue) ||                                        
                                           d.EmbossingStation.Contains(request.SearchValue) ||
                                           d.EmbossingStationCity.Contains(request.SearchValue)||
                                           d.DealerCode.Contains(request.SearchValue) ||
                                           d.OEMCode.Contains(request.SearchValue) ||
                                           d.EmbossingStationCode.Contains(request.SearchValue)
                                      );
                }
                if(request.StartDate.HasValue) { query = query.Where(w => w.OrderDate >= request.StartDate); }
                if(request.EndDate.HasValue) { query = query.Where(w => w.OrderDate <= request.EndDate); }
                if (request.orderTypeID > 0) { query = query.Where(w => w.OrderTypeID == request.orderTypeID); }
                if (request.OEMID > 0) { query = query.Where(w => w.OEMID == request.OEMID); }
                if (request.DealerID > 0) { query = query.Where(w => w.DealerID == request.DealerID); }
                if (request.EmbossingStationID > 0) { query = query.Where(w => w.EmbossingStationID == request.EmbossingStationID); }
                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    query = query.Where(d => d.OrderNo.Contains(request.SearchValue) ||
                                             d.OrderTypeName.Contains(request.SearchValue)
                                             );
                }
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VHSRPOrder.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
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
                                                       //w.InvoiceNo,
                                                       //w.sInvoiceDate,
                                                       //w.InvoiceNetAmount,
                                                       w.LastUpdatedByName
                                                   }).ToList();
                _auditLogger.SaveActionLog("VHSRPOrder", ActionType.ListData, null, request, null, "HSRPOrdersServiceRepository.GetHsrporder()");
                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered =filteredRecords;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "HSRPOrdersServiceRepository.GetHsrporder()");
            }

            return response;
        }
        public DataResponse GetHsrporderForExport(HsrpOrderRequest request)
        {
            DataResponse response = new DataResponse();

            try
            {
                IQueryable<VHSRPOrder> query = _dbcontext.VHSRPOrder.AsQueryable();

                // 🔍 Search filters
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    var search = request.SearchValue.Trim();
                    query = query.Where(d =>
                        d.OrderNo.Contains(search) ||
                        d.OrderTypeName.Contains(search) ||
                        d.Dealer.Contains(search) ||
                        d.OEM.Contains(search) ||
                        d.EmbossingStation.Contains(search) ||
                        d.RegNo.Contains(search));
                }

                //  Date filters
                if (request.StartDate.HasValue)
                    query = query.Where(w => w.OrderDate >= request.StartDate.Value);

                if (request.EndDate.HasValue)
                    query = query.Where(w => w.OrderDate <= request.EndDate.Value);

                //  Other filters
                if (request.orderTypeID > 0)
                    query = query.Where(w => w.OrderTypeID == request.orderTypeID);

                if (request.OEMID > 0)
                    query = query.Where(w => w.OEMID == request.OEMID);

                if (request.DealerID > 0)
                    query = query.Where(w => w.DealerID == request.DealerID);

                if (request.EmbossingStationID > 0)
                    query = query.Where(w => w.EmbossingStationID == request.EmbossingStationID);

                // Optional SearchText filter
                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    var searchText = request.SearchText.Trim();
                    query = query.Where(d =>
                        d.OrderNo.Contains(searchText) ||
                        d.OrderTypeName.Contains(searchText));
                }

                //  Counts
                var sortColumn = string.IsNullOrWhiteSpace(request.SortColumn) ? "OrderDate" : request.SortColumn;
                var sortDirection = string.IsNullOrWhiteSpace(request.SortDirection) ? "desc" : request.SortDirection;

                // Validate column name against model properties
                var validProperties = typeof(VHSRPOrder).GetProperties()
                    .Select(p => p.Name)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (validProperties.Contains(sortColumn))
                {
                    query = query.OrderBy($"{sortColumn} {sortDirection}");
                }
                else
                {
                    // Default fallback sort
                    query = query.OrderByDescending(w => w.OrderDate);
                }

                // Get full filtered data (NO paging here)
                var exportData = query
                    .Select(w => new HSRPOrderDataExport
                    {
                        OrderNo = w.OrderNo,
                        sOrderDate = w.sOrderDate,
                        DealerPONo = w.DealerPONo,
                        DealerSONo = w.DealerSONo,
                        Dealer = w.Dealer,
                        DealerCode = w.DealerCode,
                        DealerCity = w.DealerCity,
                        OEM = w.OEM,
                        OEMCode = w.OEMCode,
                        OEMCity = w.OEMCity,
                        EmbossingStation = w.EmbossingStation,
                        EmbossingStationCode = w.EmbossingStationCode,
                        EmbossingStationCity = w.EmbossingStationCity,
                        sProcessDate = w.sProcessDate,
                        RegNo = w.RegNo,
                        sRegDate = w.sRegDate,
                        EngineNo = w.EngineNo,
                        ChasisNo = w.ChasisNo,
                        PlateColor = w.PlateColor
                    })
                    .ToList();
                response.Value = exportData.OrderByDescending(w => w.OrderNo); ;
                _auditLogger.SaveActionLog("VHSRPOrder", ActionType.ExportExcel, null, request, null, "HSRPOrdersServiceRepository.GetHsrporderForExport()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "HSRPOrdersServiceRepository.GetHsrporderForExport()");
            }

            return response;
        }


        public DataResponse SummaryOrdersByStatusID(SummaryFilterData filterData)
        {
            DataResponse response = new DataResponse();

            try
            {
                // Get data from stored procedure
                var dataResult = HsrpLaserNoDataTable(filterData);

                // Convert DataTable to strongly typed list
                var HsrpLaserNoDataList = dataResult.dtLaserNoSummary?.ToList<VHsrpLaserNoDataTable>()
                                         ?? new List<VHsrpLaserNoDataTable>();

                // Wrap inside custom object
                var laserDataResponse = new HSRPLaserDataResponse
                {
                    lstLaserNoSummary = HsrpLaserNoDataList
                };

                // Assign to DataResponse
                response.Value = laserDataResponse;
                response.Count = HsrpLaserNoDataList.Count;
                response.Message = Constants.SuccessMessage;
                response.ID = 1; // success
                _auditLogger.SaveActionLog("HsrpSummary", ActionType.ListData, null, filterData, null, "HSRPOrdersServiceRepository.SummaryForAllOrders()");
            }
            catch (SqlException sqlEx)
            {
                response.Message = sqlEx.Message;
                response.ID = 0;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "HSRPOrdersServiceRepository.SummaryForAllOrders()");
            }

            return response;
        }
        private HsrpLaserNoDataTable HsrpLaserNoDataTable(SummaryFilterData filterData)
        {
            var dt = new HsrpLaserNoDataTable();

            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                using (var command = new SqlCommand(StoredProcedure.GETDEALERPENDINGSUMMARY, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FK_OrderStatusID", filterData.OrderStatusID);
                    command.Parameters.AddWithValue("@UserID", filterData.UserID); // or DBNull.Value

                    var adapter = new SqlDataAdapter(command);
                    var dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    if (dataSet.Tables.Count > 0)
                        dt.dtLaserNoSummary = dataSet.Tables[0];
                }
                _auditLogger.SaveActionLog("dtLaserNoSummary", ActionType.ListData, null, filterData, null, "HSRPOrdersServiceRepository.HsrpLaserNoDataTable()");
            }
            catch (Exception ex)
            {
                _errorLoggerService.LogException(ex, null, "HSRPOrdersServiceRepository.HsrpLaserNoDataTable()");
            }

            return dt;
        }
        public DataResponse GetOrderType()
        {
            DataResponse response = new DataResponse();
            try
            {
                var ordertype = _dbcontext.OrderType.Where(o => o.IsActive == true).ToList();
                response.Value = ordertype;
                response.Message= Constants.SuccessMessage;
                _auditLogger.SaveActionLog("OrderType", ActionType.ListData, null, null, null, "HSRPOrdersServiceRepository.GetOrderType()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "HSRPOrdersServiceRepository.GetOrderType()");
            }
            return response;
        }
        public DataResponse GetHsrporderByID(int HsrporderID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var HsrporderByID = _dbcontext.VHSRPOrder.FirstOrDefault(w => w.HSRPOrderID == HsrporderID);

                if (HsrporderByID == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;
                    return dataResponse;
                }
                dataResponse.ID = HsrporderID;
                dataResponse.Message = Constants.SuccessMessage;
                dataResponse.Value = HsrporderByID;
                _auditLogger.SaveActionLog("VHSRPOrder", ActionType.Select, HsrporderID.ToString(), HsrporderID, null, "HSRPOrdersServiceRepository.GetHsrporderByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPOrdersServiceRepository.GetHsrporderByID()");
            }

            return dataResponse;
        }
        public DataResponse GetOrderStatusTimeline(int orderId)
        {
            DataResponse response = new DataResponse();
            List<OrderStatusHistoryDto> list = new List<OrderStatusHistoryDto>();

            try
            {
                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                string infoMessage = string.Empty;
                connection.InfoMessage += (sender, e) =>
                {
                    infoMessage = e.Message;
                };

                connection.Open();

                using var command = new SqlCommand("GetOrderStatusHistory", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@OrderID", orderId);

                using var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new OrderStatusHistoryDto
                    {
                        HSRPOrderStatusLogID = Convert.ToInt32(dr["PK_HSRPOrderStatusLogID"]),
                        OrderID = Convert.ToInt32(dr["FK_OrderID"]),
                        OrderStatusID = Convert.ToInt32(dr["FK_OrderStatusLogID"]),
                        Description = dr["Description"]?.ToString(),
                        IconCode = dr["IconCode"]?.ToString(),
                        LastUpdatedBy = dr["LastUpdatedBy"]?.ToString(),
                        LastUpdateDate = dr["LastUpdateDate"]?.ToString(),

                        // ⭐ NEW dynamic fields from SPROC
                        CompletedStatusID = dr["CompletedStatusID"] != DBNull.Value ? Convert.ToInt32(dr["CompletedStatusID"]) : null,
                        NextPendingStatusID = dr["NextPendingStatusID"] != DBNull.Value ? Convert.ToInt32(dr["NextPendingStatusID"]) : null,
                        NextPendingDescription = dr["NextPendingDescription"]?.ToString(),
                        NextPendingIconCode = dr["NextPendingIconCode"]?.ToString()
                    });
                }
                response.Success = true;
                response.Value = list;
                response.Message = !string.IsNullOrEmpty(infoMessage)
                                    ? infoMessage
                                    : "Order status timeline loaded.";
                _auditLogger.SaveActionLog("HSRPOrder", ActionType.ListData, orderId.ToString(), orderId, null, "HSRPOrdersServiceRepository.GetOrderStatusTimeline()");
            }
            catch (SqlException sqlEx)
            {
                response.Error = false;
                response.Message = sqlEx.Message;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, orderId, "HSRPOrdersServiceRepository.GetOrderStatusTimeline()");
            }

            return response;
        }
        public DataResponse GetInvoiceDetails(int orderId)
        {
            DataResponse response = new DataResponse();
            List<OrderInvoiceDetailsDto> list = new List<OrderInvoiceDetailsDto>();

            try
            {
                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                string infoMessage = string.Empty;
                connection.InfoMessage += (sender, e) =>
                {
                    infoMessage = e.Message;
                };

                connection.Open();

                using var command = new SqlCommand("GetOrderInvoiceDetails", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@OrderID", orderId);

                using var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new OrderInvoiceDetailsDto
                    {
                        InvoiceTransID = Convert.ToInt32(dr["PK_InvoiceTransID"]),
                        InvoiceID = Convert.ToInt32(dr["FK_InvoiceID"]),
                        OrderID = Convert.ToInt32(dr["FK_OrderID"]),
                        sInvoiceDate = dr["sInvoiceDate"]?.ToString(),
                        InvoiceNo = dr["InvoiceNo"]?.ToString(),
                        Amount = dr["Amount"]?.ToString(),
                    });
                }
                response.Success = true;
                response.Value = list;
                response.Message = !string.IsNullOrEmpty(infoMessage)
                                    ? infoMessage
                                    : "Order Details Loaded";
                _auditLogger.SaveActionLog("HSRPOrder", ActionType.ListData, orderId.ToString(), orderId, null, "HSRPOrdersServiceRepository.GetInvoiceDetails()");
            }
            catch (SqlException sqlEx)
            {
                response.Error = false;
                response.Message = sqlEx.Message;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, orderId, "HSRPOrdersServiceRepository.GetInvoiceDetails()");
            }

            return response;
        }
        public DataResponse GetShipmentAndDeliveryDetails(int orderId)
        {
            DataResponse response = new DataResponse();
            List<OrderShipmentAndDeliveryDetailsDto> list = new List<OrderShipmentAndDeliveryDetailsDto>();

            try
            {
                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                string infoMessage = string.Empty;
                connection.InfoMessage += (sender, e) =>
                {
                    infoMessage = e.Message;
                };

                connection.Open();

                using var command = new SqlCommand("GetShipmentAndDeliveryDetails", connection);
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.AddWithValue("@OrderID", orderId);

                using var dr = command.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new OrderShipmentAndDeliveryDetailsDto
                    {
                        GenerateDeliveryTransID = Convert.ToInt32(dr["PK_GenerateDeliveryTransID"]),
                        GenerateDeliveryID = Convert.ToInt32(dr["FK_GenerateDeliveryID"]),
                        OrderID = Convert.ToInt32(dr["FK_OrderID"]),
                        ModeOfTransport = dr["ModeOfTransport"]?.ToString(),
                        CourierName = dr["CourierName"]?.ToString(),
                        ConsignmentDetails = dr["ConsignmentDetails"]?.ToString(),
                        CollectingPerson = dr["CollectingPerson"]?.ToString(),
                        UploadImageUrl = dr["UploadImageUrl"]?.ToString(),
                        ShipmentDate = dr["ShipmentDate"]?.ToString(),
                        sDeliveredDate = dr["sDeliveredDate"]?.ToString(),
                        DocketNo = dr["DocketNo"]?.ToString(),
                    });
                }
                response.Success = true;
                response.Value = list;
                response.Message = !string.IsNullOrEmpty(infoMessage)
                                    ? infoMessage
                                    : "Order Shipment And Delivery Details Loaded";
                _auditLogger.SaveActionLog("HSRPOrder", ActionType.ListData, orderId.ToString(), orderId, null, "HSRPOrdersServiceRepository.GetShipmentAndDeliveryDetails()");
            }
            catch (SqlException sqlEx)
            {
                response.Error = false;
                response.Message = sqlEx.Message;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, orderId, "HSRPOrdersServiceRepository.GetShipmentAndDeliveryDetails()");
            }
            return response;
        }
    }
}