namespace SVAuroraERP.Infrastructure.Repositories.Orders.Invoice
{
    public class CreateInvoiceServiceRepository : ICreateInvoiceServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public CreateInvoiceServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse SummaryForQCCompleted()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.CreateInvoiceData.OrderBy(o => o.DealerPONo).ToList();
                DataResponse.Count = resultdata.Count;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("CreateInvoiceData", ActionType.ListData, null, null, null,"CreateInvoiceServiceRepository.SummaryForQCCompleted()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "CreateInvoiceServiceRepository.SummaryForQCCompleted()");
            }
            return DataResponse;
        }
        public DataResponse GenerateInvoice(GenerateInvoiceRequest request)
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

                using var command = new SqlCommand("dbo.GenerateInvoice", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // 🔹 Input parameters
                command.Parameters.AddWithValue("@DealerID", request.DealerID);
                command.Parameters.AddWithValue("@OrderID", request.OrderID);
                command.Parameters.AddWithValue("@LastUpdatedBy", request.LastUpdatedBy);

                // 🔹 Output parameter (important: add it to Parameters)
                var invoiceOutput = new SqlParameter("@InvoiceNo", SqlDbType.VarChar,-1)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(invoiceOutput);
                string invoiceNo = string.Empty;

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        invoiceNo = reader["InvoiceNo"].ToString();
                    }
                }

                // 🔹 If OUTPUT parameter has value, prefer that
                if (string.IsNullOrEmpty(invoiceNo) && invoiceOutput.Value != DBNull.Value)
                {
                    invoiceNo = invoiceOutput.Value.ToString();
                }
                response.Message = invoiceNo;
                _auditLogger.SaveActionLog("HSRPInvoice", ActionType.ListData, null, request, null, "CreateInvoiceServiceRepository.GenerateInvoice()");
            }
            catch (SqlException sqlEx)
            {
                response.Message = sqlEx.Message;
                response.ID = 0;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "CreateInvoiceServiceRepository.GenerateInvoice()");
            }

            return response;
        }

        public DataResponse GetListInvoiceTrans(InvoiceTransRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VQCCompleted> query = _dbcontext.VQCCompleted;
                var hsrpUser = _dbcontext.VHSRPUser.FirstOrDefault(u => u.UserID == request.EmbossingStationID);

                //if (hsrpUser != null)
                //{
                //    // Replace the EmbossingStationID in the request with HSRPUserID
                //    request.EmbossingStationID = hsrpUser.HSRPUserID;

                //    // 🔹 Filter by actual EmbossingStationID from job card table
                //    query = query.Where(w => w.EmbossingStationID == request.EmbossingStationID);
                //}
                if (request.DealerID != 0)
                {
                    query = query.Where(d => d.DealerID == request.DealerID);
                }
                if (request.DealerPONo!=null)
                {
                    query = query.Where(d => d.DealerPONo == request.DealerPONo);
                }
                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.OrderNo.Contains(request.SearchValue) ||
                                                d.OrderTypeName.Contains(request.SearchValue) ||
                                                d.RegNo.Contains(request.SearchValue) ||
                                                d.Dealer.Contains(request.SearchValue) ||
                                                d.OEM.Contains(request.SearchValue) ||
                                                d.ChasisNo.Contains(request.SearchValue) ||
                                                d.EngineNo.Contains(request.SearchValue) ||
                                                d.RearLaserSerialNo.Contains(request.SearchValue) ||
                                                d.FrontLaserSerialNo.Contains(request.SearchValue) ||
                                                d.FrontPlateDimension.Contains(request.SearchValue) ||
                                                d.RearPlateDimension.Contains(request.SearchValue));
                }
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VQCCompleted.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {   
                                                       w.Dealer,
                                                       w.HSRPOrderID,
                                                       w.RegNo,
                                                       w.FrontLaserSerialNo,
                                                       w.RearLaserSerialNo,
                                                       w.DealerID,
                                                       w.OrderNo,
                                                       w.DealerPONo,
                                                       w.DealerCode,
                                                       w.LastUpdatedBy,
                                                       w.LastUpdatedDate,
                                                       w.sOrderDate,
                                                       w.OrderDate,
                                                       w.PlateColor,    
                                                       w.FrontPlateDimension,
                                                       w.RearPlateDimension,
                                                       w.sRegDate,
                                                       w.RegDate
                                                   }).ToList();

                response.Value = pagedData.OrderByDescending(w => w.OrderNo);
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VQCCompleted", ActionType.ListData, null, request, null, "CreateInvoiceServiceRepository.GetListInvoiceTrans()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "CreateInvoiceServiceRepository.GetListInvoiceTrans()");
            }
            return response;
        }

    }
}