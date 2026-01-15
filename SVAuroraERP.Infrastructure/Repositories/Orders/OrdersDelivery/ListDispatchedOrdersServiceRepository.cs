namespace SVAuroraERP.Infrastructure.Repositories.Orders.OrdersDelivery
{
    public class ListDispatchedOrdersServiceRepository : IListDispatchedOrdersServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public ListDispatchedOrdersServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse GetListDispatchOrdersDetails(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VListDispatchOrder> query = _dbcontext.VListDispatchOrder;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.CompanyName.Contains(request.SearchValue) ||
                                             d.CollectingPerson.Contains(request.SearchValue) ||
                                             d.ConsignmentDetails.Contains(request.SearchValue)
                                             );
                }
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VListDispatchOrder.Count();

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
                _auditLogger.SaveActionLog("VListDispatchOrderTrans", ActionType.ListData, null, null, null, "ListDispatchedOrdersServiceRepository.GetListDispatchOrdersDetails()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ListDispatchedOrdersServiceRepository.GetListDispatchOrdersDetails()");
            }
            return response;
        }
        public DataResponse GetDispatchTransData(int GetDeliveryID)
        {
            DataResponse response = new DataResponse();
            try
            {
                var query = _dbcontext.VListDispatchOrderTrans.AsQueryable();

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
                        x.LastUpdatedBy,
                        x.LastUpdatedDate,
                        x.RegNo,
                        x.FrontLaserSerialNo,
                        x.RearLaserSerialNo,
                        x.DeliveredDate,
                        x.sRegDate,
                        x.RegDate,
                        x.FrontPlateDimension,
                        x.RearPlateDimension
                    })
                    .ToList();

                response.Value = resultData;
                response.Count = resultData.Count;

                _auditLogger.SaveActionLog("VListDispatchOrderTrans", ActionType.ListData, GetDeliveryID.ToString(), null, null, "ListDispatchedOrdersServiceRepository.GetDispatchTransData()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, GetDeliveryID, "ListDispatchedOrdersServiceRepository.GetDispatchTransData()");
            }
            return response;
        }

    }
}
