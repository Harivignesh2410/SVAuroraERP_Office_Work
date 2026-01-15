using Azure;

namespace SVAuroraERP.Infrastructure.Repositories.Orders.Invoice
{
    public class ListInvoiceServiceRepository : IListInvoiceServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public ListInvoiceServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse GetListInvoice(ListInvoiceRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHSRPInvoice> query = _dbcontext.VHSRPInvoice;
                var hsrpUser = _dbcontext.VHSRPUser.FirstOrDefault(u => u.HSRPUserID == request.HsrpUserID);

                //if (hsrpUser != null)
                //{
                //    // Replace the EmbossingStationID in the request with HSRPUserID
                //    request.HsrpUserID = hsrpUser.HSRPUserID;

                //    // 🔹 Filter by actual EmbossingStationID from job card table
                //    query = query.Where(w => w.HSRPUserID == request.HsrpUserID);
                //}
                if (request.StartDate.HasValue) { query = query.Where(w => w.InvoiceDate >= request.StartDate); }
                if (request.EndDate.HasValue) { query = query.Where(w => w.InvoiceDate <= request.EndDate); }
                if (request.OEMID > 0) { query = query.Where(w => w.OEMID == request.OEMID); }
                if (request.DealerID > 0) { query = query.Where(w => w.DealerID == request.DealerID); }
                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.DealerCode.Contains(request.SearchValue) ||
                                             d.Dealer.Contains(request.SearchValue) ||
                                             d.FrontPlateDimension.Contains(request.SearchValue) ||
                                             d.RearPlateDimension.Contains(request.SearchValue)||
                                             d.InvoiceNo.Contains(request.SearchValue) ||
                                             d.City.Contains(request.SearchValue)
                                             );
              
                }
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VHSRPInvoice.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.InvoiceID,
                                                       w.InvoiceNo,
                                                       w.OrderCount,
                                                       w.InvoiceDate,
                                                       w.sInvoiceDate,
                                                       w.DealerID,
                                                       w.Dealer,
                                                       w.OEMID,
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
                                                       w.LastUpdatedBy,
                                                       w.LastUpdatedDate,
                                                       w.LastUpdatedByName,
                                                       w.NetAmount
                                                   }).ToList();

                response.Value = pagedData.OrderByDescending(w => w.InvoiceNo);
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VHSRPInvoice", ActionType.ListData, null, request, null, "ListInvoiceServiceRepository.GetListInvoice()");

            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ListInvoiceServiceRepository.GetListInvoice()");
            }
            return response;
        }
        public DataResponse GetListInvoiceTrans(HSRPInvoiceTransRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHSRPInvoiceTrans> query = _dbcontext.VHSRPInvoiceTrans;

                if (request.InvoiceID != 0)
                {
                    query = query.Where(d => d.InvoiceID == request.InvoiceID);
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
                                                       w.FrontLaserSerialNo,
                                                       w.RearLaserSerialNo,
                                                       w.DealerID,
                                                       w.OrderNo,
                                                       w.DealerCode,
                                                       w.PlateColor,
                                                       w.sOrderDate,
                                                       w.OrderDate,
                                                       w.FrontPlateSize,
                                                       w.RearPlateSize,
                                                       w.LastUpdatedBy,
                                                       w.LastUpdatedDate,
                                                       w.FrontPlateDimension,
                                                       w.RearPlateDimension,
                                                       w.RegDate,
                                                   }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VHSRPInvoiceTrans", ActionType.ListData, null, request, null, "ListInvoiceServiceRepository.GetListInvoiceTrans()");

            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ListInvoiceServiceRepository.GetListInvoiceTrans()");
            }
            return response;
        }

        public DataResponse GetExportInvoiceList(ExportInvoiceRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VExportInvoiceList> query = _dbcontext.VExportInvoiceList;
                //var hsrpUser = _dbcontext.VHSRPUser.FirstOrDefault(u => u.HSRPUserID == request.HsrpUserID);

                //if (hsrpUser != null)
                //{
                //    // Replace the EmbossingStationID in the request with HSRPUserID
                //    request.HsrpUserID = hsrpUser.HSRPUserID;

                //    // 🔹 Filter by actual EmbossingStationID from job card table
                //    query = query.Where(w => w.HSRPUserID == request.HsrpUserID);
                //}
                // Apply search filter if provided
                if (request.StartDate.HasValue) { query = query.Where(w => w.InvoiceDate >= request.StartDate); }
                if (request.EndDate.HasValue) { query = query.Where(w => w.InvoiceDate <= request.EndDate); }
                if (request.OEMID > 0) { query = query.Where(w => w.OEMID == request.OEMID); }
                if (request.DealerID > 0) { query = query.Where(w => w.DealerID == request.DealerID); }

                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.DealerCode.Contains(request.SearchValue) ||
                                             d.InvoiceNo.Contains(request.SearchValue) ||
                                             d.Dealer.Contains(request.SearchValue) ||
                                             d.OEM.Contains(request.SearchValue) ||
                                             d.PartNo.Contains(request.SearchValue) ||
                                             d.DealerPONo.Contains(request.SearchValue) ||
                                             d.RegNo.Contains(request.SearchValue)
                                             );
                }
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VExportInvoiceList.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.InvoiceID,
                                                       w.InvoiceNo,
                                                       w.sInvoiceDate,
                                                       w.Dealer,
                                                       w.DealerPONo,
                                                       w.PartNo,
                                                       w.Qty,
                                                       w.RegNo,
                                                       w.FrontLaserSerialNo,
                                                       w.RearLaserSerialNo,
                                                       w.PlateColor,
                                                       w.OEM,
                                                       w.DealerCode
                                                   }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VHSRPInvoice", ActionType.ListData, null, request, null, "ListInvoiceServiceRepository.GetListInvoice()");

            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ListInvoiceServiceRepository.GetListInvoice()");
            }
            return response;
        }

        public DataResponse GetExportInvoiceExcel(ExportInvoiceRequest request)
        {
            DataResponse response = new DataResponse();

            try
            {
                var query = _dbcontext.VExportInvoiceList.AsQueryable();

                if (request.StartDate.HasValue) { query = query.Where(w => w.InvoiceDate >= request.StartDate); }
                if (request.EndDate.HasValue) { query = query.Where(w => w.InvoiceDate <= request.EndDate); }
                if (request.OEMID > 0) { query = query.Where(w => w.OEMID == request.OEMID); }
                if (request.DealerID > 0) { query = query.Where(w => w.DealerID == request.DealerID); }

                _auditLogger.SaveActionLog("VExportInvoiceList", ActionType.ListData, null, request, null, "ListInvoiceServiceRepository.GetExportInvoiceExcel()");
                response.Value= query.ToList();
                response.Count = query.Count();

            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ListInvoiceServiceRepository.GetExportInvoiceExcel()");
            }
            return response;

        }
    }
}