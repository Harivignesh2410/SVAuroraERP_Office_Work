using static SVAuroraERP.Domain.Common;

namespace SVAuroraERP.Infrastructure.Repositories.Orders.OrdersDelivery
{
    public class GenerateDeliveryDataServiceRepository : IGenerateDeliveryDataServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public GenerateDeliveryDataServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse GetListInvoice(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHSRPInvoiceByDealer> query = _dbcontext.VHSRPInvoiceByDealer;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.DealerCode.Contains(request.SearchValue) ||
                                             d.Dealer.Contains(request.SearchValue)
                                             );
                }
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VHSRPInvoiceByDealer.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {

                                                       w.DealerID,
                                                       w.Dealer,
                                                       w.DealerCode,
                                                       w.Address1,
                                                       w.Address2,
                                                       w.City,
                                                       w.Pincode,
                                                       w.DistrictName,
                                                       w.StateName,
                                                       w.DeliveryAddress1,
                                                       w.DeliveryAddress2,
                                                       w.DeliveryCity,
                                                       w.DeliveryPincode,
                                                       w.DeliveryStateName,
                                                       w.DeliveryDistrictName,
                                                       w.TotalOrders,
                                                       w.ContactNo,
                                                       w.DealerPONo
                                                   }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VHSRPInvoiceByDealer", ActionType.ListData, null, request, null, "GenerateDeliveryDataServiceRepository.GetListInvoice()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "GenerateDeliveryDataServiceRepository.GetListInvoice()");
            }
            return response;
        }
        public DataResponse GetListInvoiceTrans(HSRPInvoiceTransByDealerRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHSRPInvoiceForGenerateDelivery> query = _dbcontext.VHSRPInvoiceForGenerateDelivery;

                if (request.DealerID != 0)
                {
                    query = query.Where(d => d.DealerID == request.DealerID);
                }
                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.RegNo.Contains(request.SearchValue) ||
                                             d.FrontLaserSerialNo.Contains(request.SearchValue)
                                             );
                }
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VHSRPInvoiceTrans.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.InvoiceTransID,
                                                       w.InvoiceID,
                                                       w.Dealer,
                                                       w.RegNo,
                                                       w.OrderDate,                                                     
                                                       w.FrontPlateSize,
                                                       w.RearPlateSize,
                                                       w.FrontLaserSerialNo,
                                                       w.RearLaserSerialNo,
                                                       w.DealerID,
                                                       w.OrderNo,
                                                       w.DealerCode,
                                                       w.LastUpdatedBy,
                                                       w.LastUpdatedDate
                                                   }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VHSRPInvoiceForGenerateDelivery", ActionType.ListData, null, request, null, "GenerateDeliveryDataServiceRepository.GetListInvoiceTrans()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "GenerateDeliveryDataServiceRepository.GetListInvoiceTrans()");
            }
            return response;
        }
        public DataResponse GetHSRPInvoiceTransByDealer(int dealerId)
        {
            DataResponse response = new DataResponse();
            try
            {
                var query = _dbcontext.VHSRPInvoiceTrans.AsQueryable();

                // Filter only if DealerID is provided
                if (dealerId != 0)
                {
                    query = query.Where(x => x.DealerID == dealerId);
                    query = query.Where(x => x.OrderStatusID == (byte)HSRPOrderStatus.InvoiceGenerated);
                }

                var resultData = query
                    .OrderBy(o => o.InvoiceTransID)
                    .Select(x => new
                    {
                        x.InvoiceTransID,
                        x.OrderID,
                        x.DealerID,
                        x.Dealer,
                        x.DealerCode,
                        x.OrderNo,
                        x.RegNo,
                        x.FrontLaserSerialNo,
                        x.RearLaserSerialNo,
                        x.LastUpdatedBy,
                        x.LastUpdatedDate,
                        x.FrontPlateDimension,
                        x.RearPlateDimension,
                        x.sRegDate,
                        x.RegDate,
                        x.OrderDate,
                        x.sOrderDate
                    })
                    .ToList();

                response.Value = resultData;
                response.Count = resultData.Count;

                _auditLogger.SaveActionLog("VHSRPInvoiceTrans", ActionType.ListData, dealerId.ToString(), null, null, "GenerateDeliveryDataServiceRepository.GetHSRPInvoiceTransByDealer()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, dealerId, "GenerateDeliveryDataServiceRepository.GetHSRPInvoiceTransByDealer()");
            }
            return response;
        }
        public DataResponse SaveGenerateDeliveryData(GenerateDeliveryRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                connection.InfoMessage += (sender, e) =>
                {
                    response.Message = e.Message; // captures RAISERROR or PRINT
                };
                connection.Open();

                using var command = new SqlCommand("dbo.SaveGenerateDeliveryData", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // 🔹 Required parameter
                command.Parameters.AddWithValue("@FK_DealerID", request.FK_DealerID);

                // 🔹 Optional parameters — save as NULL when empty or 0
                command.Parameters.AddWithValue("@FK_ModeOfTransportID",
                    request.FK_ModeOfTransportID > 0 ? (object)request.FK_ModeOfTransportID : DBNull.Value);

                command.Parameters.AddWithValue("@FK_CourierID",
                    request.FK_CourierID > 0 ? (object)request.FK_CourierID : DBNull.Value);

                command.Parameters.AddWithValue("@ConsignmentDetails",
                    !string.IsNullOrWhiteSpace(request.ConsignmentDetails) ? request.ConsignmentDetails : DBNull.Value);

                command.Parameters.AddWithValue("@CollectingPerson",
                    !string.IsNullOrWhiteSpace(request.CollectingPerson) ? request.CollectingPerson : DBNull.Value);

                command.Parameters.AddWithValue("@DispatchDate",
                    request.DispatchDate.HasValue ? (object)request.DispatchDate.Value : DBNull.Value);

                command.Parameters.AddWithValue("@UploadImageUrl",
                    !string.IsNullOrWhiteSpace(request.UploadImageUrl) ? request.UploadImageUrl : DBNull.Value);

                command.Parameters.AddWithValue("@LastUpdatedBy",
                    request.LastUpdatedBy > 0 ? (object)request.LastUpdatedBy : DBNull.Value);

                command.Parameters.AddWithValue("@UploadImageName",
                    !string.IsNullOrWhiteSpace(request.ImageName) ? request.ImageName : DBNull.Value);
                command.Parameters.AddWithValue("@OrderList",
              !string.IsNullOrWhiteSpace(request.OrderList) ? request.OrderList : DBNull.Value);

                // 🔹 Execute and handle response
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        response.ID = reader["Status"] != DBNull.Value ? Convert.ToInt32(reader["Status"]) : 0;
                        response.Message = reader["Message"]?.ToString() ?? "Operation completed";
                    }
                    else
                    {
                        response.ID = 1;
                        response.Message = "Delivery data saved successfully";
                    }
                }
                _auditLogger.SaveActionLog("VHSRPInvoiceTrans", ActionType.Insert, request.FK_DealerID.ToString(), request, null, "GenerateDeliveryDataServiceRepository.SaveGenerateDeliveryData()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "GenerateDeliveryDataServiceRepository.SaveGenerateDeliveryData()");
            }

            return response;
        }
        public DataResponse GetDispatchDetails(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VGenerateDeliveryData> query = _dbcontext.VGenerateDeliveryData;
                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.CompanyName.Contains(request.SearchValue) ||
                                             d.CollectingPerson.Contains(request.SearchValue) ||
                                             d.ConsignmentDetails.Contains(request.SearchValue)
                                             );
                }
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VGenerateDeliveryData.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {

                                                       w.DealerID,
                                                       w.GenerateDeliveryID,
                                                       w.CompanyName,
                                                       w.ModeOfTransport,
                                                       w.ModeOfTransportID,
                                                       w.CourierID,
                                                       w.CourierName,
                                                       w.ConsignmentDetails,
                                                       w.CollectingPerson,
                                                       w.GenerateDate,
                                                       w.sGenerateDate,
                                                       w.ImageName,
                                                       w.UploadImageUrl,
                                                       w.TotalOrders,

                                                       w.EmbossingStationName,
                                                   }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VHSRPInvoiceTrans", ActionType.ListData, null, request, null, "GenerateDeliveryDataServiceRepository.GetDispatchDetails()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "GenerateDeliveryDataServiceRepository.GetDispatchDetails()");
            }
            return response;
        }
        public DataResponse GetDispatchData(int GetDeliveryID)
        {
            DataResponse response = new DataResponse();
            try
            {
                var query = _dbcontext.VGenerateDeliveryTrans.AsQueryable();

                // Filter only if DealerID is provided
                if (GetDeliveryID != 0)
                {
                    query = query.Where(x => x.GenerateDeliveryID == GetDeliveryID);
                }

                var resultData = query
                    .OrderBy(o => o.GenerateDeliveryTransID)
                    .Select(x => new
                    {
                        x.GenerateDeliveryTransID,
                        x.GenerateDeliveryID,
                        x.Dealer,
                        x.DealerCode,
                        x.OrderNo,
                        x.DealerPONo,
                        x.OrderDate,
                        x.sOrderDate,
                        x.DealerSONo,
                        x.RegNo,
                        x.sRegDate,
                        x.RearLaserSerialNo,
                        x.FrontLaserSerialNo,
                        x.RearPlateSize,
                        x.FrontPlateSize,
                        x.PlateColor,
                        x.LastUpdatedBy,
                        x.LastUpdatedDate
                    })
                    .ToList();

                response.Value = resultData;
                response.Count = resultData.Count;

                _auditLogger.SaveActionLog("VHSRPInvoiceTrans", ActionType.ListData, GetDeliveryID.ToString(), null, null, "GenerateDeliveryDataServiceRepository.GetDispatchData()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, GetDeliveryID, "GenerateDeliveryDataServiceRepository.GetDispatchData()");
            }
            return response;
        }

        public DataResponse GetListDispatchDataTrans(int GenerateDeliveryID)
        {
            DataResponse response = new DataResponse();
            try
            {
                var query = _dbcontext.VAcknowledgeDispatchedOrders.AsQueryable();

                // Filter only if DealerID is provided
                if (GenerateDeliveryID != 0)
                {
                    query = query.Where(x => x.GenerateDeliveryID == GenerateDeliveryID);
                }

                var resultData = query
                    .OrderBy(o => o.OrderID)
                    .Select(w => new
                    {
                        w.GenerateDeliveryTransID,
                        w.GenerateDeliveryID,
                        w.OrderID,
                        w.DealerPONo,
                        w.OrderNo,
                        w.DealerSONo,
                        w.Dealer,
                        w.DealerCode,
                        w.DealerID,
                        w.sOrderDate,
                        w.OrderDate,
                        w.LastUpdatedBy,
                        w.LastUpdatedDate,
                        w.FrontLaserSerialNo,
                        w.RearLaserSerialNo,
                        w.RegNo,
                        w.sRegDate,
                        w.RegDate,
                        w.PlateColor,
                        w.FrontPlateSize,
                        w.RearPlateSize,
                        w.sGenerateDate,
                        w.GenerateDate,
                        w.FrontPlateDimension,
                        w.RearPlateDimension
                    })
                    .ToList();

                response.Value = resultData;
                response.Count = resultData.Count;

                _auditLogger.SaveActionLog("VHSRPInvoiceTrans", ActionType.ListData, GenerateDeliveryID.ToString(), null, null, "GenerateDeliveryDataServiceRepository.GetListDispatchDataTrans()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, GenerateDeliveryID, "GenerateDeliveryDataServiceRepository.GetListDispatchDataTrans()");
            }
            return response;
        }
        public DataResponse AcknowledgeGenerateDeliveryData(AcknowlegdeGenerateDeliveryRequest request)
        {
            DataResponse response = new DataResponse();

            try
            {
                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                connection.Open();

                using var command = new SqlCommand("dbo.AcknowledgeGenerateDeliveryData", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@GeneratedDeliveryDataID", request.GenerateDelieveryDataID);

                command.Parameters.AddWithValue("@DeliveryDate",
                    request.DeliveryDate.HasValue ? (object)request.DeliveryDate.Value : DBNull.Value);

                command.Parameters.AddWithValue("@LastUpdatedBy", request.LastUpdatedBy);

                command.Parameters.AddWithValue("@OrderList", request.OrderList ?? "");

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        response.ID = Convert.ToInt32(reader["Status"]);
                        response.Message = reader["Message"]?.ToString();
                    }
                }
                _auditLogger.SaveActionLog("AcknowlegdeDelivery", ActionType.ListData, request.GenerateDelieveryDataID.ToString(), request, null, "GenerateDeliveryDataServiceRepository.AcknowledgeGenerateDeliveryData()");
            }
            catch (Exception ex)
            {
                response.ID = 0;
                response.Message = ex.Message;
                response = _errorLoggerService.LogException(ex, request, "GenerateDeliveryDataServiceRepository.AcknowledgeGenerateDeliveryData()");
            }
            return response;
        }
    }
}
