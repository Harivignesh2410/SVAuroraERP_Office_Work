namespace SVAuroraERP.Infrastructure.Repositories.Orders.ManageOrder
{
    public class FixationImageReuploadServiceRepository : IFixationImageReuploadServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public FixationImageReuploadServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse GetFixationImageReuploadOrders(FixationImageReuploadRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VFixationImageReupload> query = _dbcontext.VFixationImageReupload;
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.OrderNo.Contains(request.SearchValue) ||
                                           d.OrderTypeName.Contains(request.SearchValue) ||
                                           d.RegNo.Contains(request.SearchValue) ||
                                           d.Dealer.Contains(request.SearchValue) ||
                                           d.DealerCode.Contains(request.SearchValue) ||
                                           d.OEM.Contains(request.SearchValue) ||
                                           d.ChasisNo.Contains(request.SearchValue) ||
                                           d.EngineNo.Contains(request.SearchValue) ||
                                           d.RearLaserSerialNo.Contains(request.SearchValue) ||
                                           d.FrontLaserSerialNo.Contains(request.SearchValue) ||
                                           d.FrontPlateDimension.Contains(request.SearchValue) ||
                                           d.RearPlateDimension.Contains(request.SearchValue));
                }
                if (request.StartDate.HasValue) { query = query.Where(w => w.OrderDate >= request.StartDate); }
                if (request.EndDate.HasValue) { query = query.Where(w => w.OrderDate <= request.EndDate); }
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
                var totalRecords = _dbcontext.VFixationImageReupload.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.HSRPVehiclePlateImageID,
                                                       w.HSRPOrderID,
                                                       w.FrontLaserNoURL,
                                                       w.RearLaserNoURL,
                                                       w.IsActive,
                                                       w.UploadedDate,
                                                       w.sUploadedDate,
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
                                                       w.FrontPlateDimension,
                                                       w.RearPlateDimension,
                                                       //w.InvoiceNo,
                                                       //w.sInvoiceDate,
                                                       //w.InvoiceNetAmount,
                                                       w.LastUpdatedByName
                                                   }).ToList();

                response.Value = pagedData.OrderByDescending(w => w.OrderNo);
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("FittedOrders", ActionType.ListData, null, request, null, "FittedOrdersServiceRepository.GetFittedOrders()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "FittedOrdersServiceRepository.GetFittedOrders()");
            }
            return response;
        }
        public DataResponse GetHsrporderByID(int HsrporderID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var HsrporderByID = _dbcontext.VHSRPVehiclePlateImage.FirstOrDefault(w => w.HSRPOrderID == HsrporderID);

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
                _auditLogger.SaveActionLog("VHSRPVehiclePlateImage", ActionType.Select, HsrporderID.ToString(), HsrporderID, null, "FittedOrdersServiceRepository.GetHsrporderByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "FittedOrdersServiceRepository.GetHsrporderByID()");
            }
            return dataResponse;
        }
        public DataResponse SaveHSRPPlateImage(HSRPVehiclePlateImage request)
        {
            DataResponse response = new DataResponse();

            try
            {
                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                using var command = new SqlCommand("UpdateHSRPOrderStatusToReuploadFittedOrder", connection);

                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add("@FK_OrderID", SqlDbType.Int)
                       .Value = request.OrderID;

                command.Parameters.Add("@FrontLaserNoURL", SqlDbType.VarChar)
                       .Value = string.IsNullOrEmpty(request.FrontLaserNoURL)
                                ? DBNull.Value
                                : request.FrontLaserNoURL;

                command.Parameters.Add("@RearLaserNoURL", SqlDbType.VarChar)
                       .Value = string.IsNullOrEmpty(request.RearLaserNoURL)
                                ? DBNull.Value
                                : request.RearLaserNoURL;

                command.Parameters.Add("@LastUpdatedBy", SqlDbType.Int)
                       .Value = request.LastUpdatedBy;

                connection.Open();
                command.ExecuteNonQuery();

                response.Success = true;
                response.Message = Constants.SuccessMessage;

                _auditLogger.SaveActionLog(
                    "HSRPVehiclePlateImage",
                    ActionType.Update,
                    request.OrderID.ToString(),
                    request,
                    null,
                    "FixationImageReuploadServiceRepository.SaveHSRPPlateImage()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(
                    ex,
                    request,
                    "FixationImageReuploadServiceRepository.SaveHSRPPlateImage()");
            }

            return response;
        }
        public DataResponse SummaryForImageReuploadData(int UserID)
        {
            DataResponse response = new DataResponse();

            try
            {
                // Get data from stored procedure
                var dataResult = HsrpLaserNoDataTable(UserID);

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
                _auditLogger.SaveActionLog("HsrpSummary", ActionType.ListData, null, UserID, null, "FixationImageReuploadServiceRepository.SummaryForImageReuploadData()");
            }
            catch (SqlException sqlEx)
            {
                response.Message = sqlEx.Message;
                response.ID = 0;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "FixationImageReuploadServiceRepository.SummaryForImageReuploadData()");
            }

            return response;
        }
        private HsrpLaserNoDataTable HsrpLaserNoDataTable(int UserID)
        {
            var dt = new HsrpLaserNoDataTable();

            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                using (var command = new SqlCommand(StoredProcedure.GETDEALERPENDINGSUMMARY, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FK_OrderStatusID", OrderStatus.FixationReUpload);
                    command.Parameters.AddWithValue("@UserID", UserID); // or DBNull.Value

                    var adapter = new SqlDataAdapter(command);
                    var dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    if (dataSet.Tables.Count > 0)
                        dt.dtLaserNoSummary = dataSet.Tables[0];
                }
                _auditLogger.SaveActionLog("dtLaserNoSummary", ActionType.ListData, null, UserID, null, "FixationReUploadedServiceRepository.HsrpLaserNoDataTable()");
            }
            catch (Exception ex)
            {
                _errorLoggerService.LogException(ex, null, "FixationReUploadedServiceRepository.HsrpLaserNoDataTable()");
            }

            return dt;
        }
    }
}