
namespace SVAuroraERP.Infrastructure.Repositories.Orders.ManageOrder
{
    public class FixationReUploadedServiceRepository : IFixationReUploadedServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public FixationReUploadedServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse GetFixationReUploaded(FixationReUploadedRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VFixationReUploaded> query = _dbcontext.VFixationReUploaded;
                //var hsrpUser = _dbcontext.VHSRPUser
                //         .FirstOrDefault(u => u.HSRPUserID == request.EmbossingStationID);

                //if (hsrpUser != null)
                //{
                //    request.EmbossingStationID = hsrpUser.UserID;

                //    query = query.Where(w => w.EmbossingStationID == request.EmbossingStationID);
                //}
                // Apply search filter if provided
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
                var totalRecords = _dbcontext.VFixationReUploaded.Count();

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
                _auditLogger.SaveActionLog("VehiclePlateImage", ActionType.ListData, null, request, null, "VehiclePlateImageServiceRepository.GetVehiclePlateImage()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "VehiclePlateImageServiceRepository.GetVehiclePlateImage()");
            }
            return response;
        }
        public DataResponse SummaryForLaserNoAllocation(int UserID)
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
                _auditLogger.SaveActionLog("HsrpSummary", ActionType.ListData, null, UserID, null, "VehiclePlateImageServiceRepository.SummaryForLaserNoAllocation()");
            }
            catch (SqlException sqlEx)
            {
                response.Message = sqlEx.Message;
                response.ID = 0;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "FixationReUploadedsServiceRepository.SummaryForFixationReUploadeds()");
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
                    command.Parameters.AddWithValue("@FK_OrderStatusID", OrderStatus.FixationReUploaded);
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
        public DataResponse UpdateVehiclePlateStatus(SaveFixationReUploadedRequest model)
        {
            var response = new DataResponse();

            try
            {
                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                using var command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "UpdateHSRPVehicleImageStatus";
                command.CommandType = CommandType.StoredProcedure;
                int statusId = (int)(model.IsSubmit
                                             ? OrderStatus.VahanAPISubmitted
                                             : OrderStatus.FixationReUpload);
                command.Parameters.Add(new SqlParameter("@OrderID", model.HSRPOrderID));
                command.Parameters.Add(new SqlParameter("@StatusID", statusId));
                command.Parameters.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        response.Success = Convert.ToBoolean(reader["Success"]);
                        response.Message = response.Success
                            ? "Status updated successfully."
                            : reader["ErrorMessage"]?.ToString();

                        if (response.Success)
                        {
                            response.ID = Convert.ToInt32(reader["OrderID"]);
                        }
                    }
                }

                _auditLogger.SaveActionLog("UpdateVehiclePlateStatus", ActionType.ListData, null, model, null, "VehiclePlateRepository.UpdateVehiclePlateStatus()");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response = _errorLoggerService.LogException(ex, model, "VehiclePlateRepository.UpdateVehiclePlateStatus()");
            }

            return response;
        }
        public DataResponse GetVehicleImageData(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VFixationReUploaded.FirstOrDefault(w => w.HSRPOrderID == ID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataResponse.ID = ID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VehicleImage", ActionType.Select, ID.ToString(), ID, null, "FixationReUploadedServiceRepository.GetVehicleImageData()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "FixationReUploadedServiceRepository.GetVehicleImageData()");
            }

            return dataResponse;
        }
    }
}
