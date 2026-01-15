namespace SVAuroraERP.Infrastructure.Repositories.Orders.ManageOrder
{
    internal class APISubmissionServiceRepository :IAPISubmissionServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public APISubmissionServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse GetAPISubmissionData(APISubmissionRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<APISubmittedOrders> query = _dbcontext.APISubmittedOrders;
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
                                             d.OEM.Contains(request.SearchValue) ||
                                             d.DealerCode.Contains(request.SearchValue) ||
                                             d.OEMCode.Contains(request.SearchValue) ||
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
                var totalRecords = _dbcontext.APISubmittedOrders.Count();

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

                response.Value = pagedData.OrderByDescending(w => w.OrderNo); ;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("APISubmittedOrders", ActionType.ListData, null, request, null, "APISubmissionServiceRepository.GetAPISubmissionData()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "APISubmissionServiceRepository.GetAPISubmissionData()");
            }
            return response;
        }
    }
}