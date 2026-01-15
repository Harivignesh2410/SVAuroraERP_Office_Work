namespace SVAuroraERP.Infrastructure.Repositories.Orders.ManageOrder
{
    public class HSRPLaserNoStockServiceRepository: IHSRPLaserNoStockServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public HSRPLaserNoStockServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse GetLaserNoStockDataTable(HSRPLaserNoStockFilterData request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize pagination
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit max 100 per page
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHSrpLaserNoStock> query = _dbcontext.VHSrpLaserNoStock;

                //Apply filters based on request parameters
                if (request.EmbossingStationID.HasValue && request.EmbossingStationID.Value > 0)
                    query = query.Where(o => o.EmbossingStationID == request.EmbossingStationID);

                if (request.SizeID.HasValue && request.SizeID.Value > 0)
                    query = query.Where(o => o.SizeID == request.SizeID);

                if (request.ColorID.HasValue && request.ColorID.Value > 0)
                    query = query.Where(o => o.ColorID == request.ColorID);

                if (request.StockStatusID.HasValue && request.StockStatusID.Value > 0)
                    query = query.Where(o => o.StockStatusID == request.StockStatusID);

                if (!string.IsNullOrWhiteSpace(request.sStatingDate) &&
                DateTime.TryParse(request.sStatingDate, out DateTime startDate))
                {
                    string start = startDate.ToString("yyyy-MM-dd");
                    query = query.Where(o => string.Compare(o.StockInsertedDate, start) >= 0);
                }

                if (!string.IsNullOrWhiteSpace(request.sEndingDate) &&
                    DateTime.TryParse(request.sEndingDate, out DateTime endDate))
                {
                    string end = endDate.ToString("yyyy-MM-dd");
                    query = query.Where(o => string.Compare(o.StockInsertedDate, end) <= 0);
                }


                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    string search = request.SearchValue.Trim();
                    query = query.Where(d =>
                        (d.DispatchNo ?? "").Contains(search) ||
                        (d.LaserNoStatus ?? "").Contains(search) ||
                        (d.SerialNo ?? "").Contains(search) ||
                        (d.Dimension ?? "").Contains(search) ||
                        (d.EmbossingStationName ?? "").Contains(search)
                    );
                }

                var totalRecords = _dbcontext.VHSrpLaserNoStock.Count();
                var filteredRecords = query.Count();

                
                var sortColumn = !string.IsNullOrEmpty(request.SortColumn) ? request.SortColumn : "HSRPLaserNoStockID";
                var sortDirection = !string.IsNullOrEmpty(request.SortDirection) ? request.SortDirection : "desc";
                query = query.OrderBy($"{sortColumn} {sortDirection}");

                var pagedData = query.Skip(skip).Take(pageSize)
                    .Select(w => new
                    {
                        w.HSRPLaserNoStockID,
                        w.DispatchNo,
                        w.LaserNoStatus,
                        w.ColorCode,
                        w.SerialNo,
                        w.EmbossingStationName,
                        w.Dimension,
                        w.StockInsertedDate,
                        w.LastUpdatedDate
                    }).ToList();

               
                response.Value = pagedData.OrderByDescending(w => w.DispatchNo); 
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;

                _auditLogger.SaveActionLog("VHSrpLaserNoStock", ActionType.ListData, null, request, null, "HSRPLaserNoStockServiceRepository.GetLaserNoStockDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "HSRPLaserNoStockServiceRepository.GetLaserNoStockDataTable()");
            }

            return response;
        }
        public DataResponse GetHSRPLaserNoStatus()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.HSrpLaserNoStatus.ToList();

                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;

                _auditLogger.SaveActionLog("HSrpLaserNoStatus", ActionType.ListData, null, null, null, "HSRPLaserNoStockServiceRepository.GetHSRPLaserNoStatus()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPLaserNoStockServiceRepository.GetHSRPLaserNoStatus()");
            }

            return dataResponse;
        }
        public DataResponse GetLaserStockSummary(HSRPLaserNoStockFilterData request)
        {
            DataResponse resultdata=new DataResponse();
            try
            {
                var query = _dbcontext.VHSrpLaserNoStock.AsQueryable();

                if (request.EmbossingStationID > 0) query = query.Where(o => o.EmbossingStationID == request.EmbossingStationID);
                if (request.SizeID > 0) query = query.Where(o => o.SizeID == request.SizeID);
                if (request.ColorID > 0) query = query.Where(o => o.ColorID == request.ColorID);

                if (!string.IsNullOrWhiteSpace(request.sStatingDate) &&
                DateTime.TryParse(request.sStatingDate, out DateTime startDate))
                {
                    string start = startDate.ToString("yyyy-MM-dd");
                    query = query.Where(o => string.Compare(o.StockInsertedDate, start) >= 0);
                }

                if (!string.IsNullOrWhiteSpace(request.sEndingDate) &&
                    DateTime.TryParse(request.sEndingDate, out DateTime endDate))
                {
                    string end = endDate.ToString("yyyy-MM-dd");
                    query = query.Where(o => string.Compare(o.StockInsertedDate, end) <= 0);
                }

                resultdata.Value = query.ToList();
                _auditLogger.SaveActionLog("VHSrpLaserNoStock", ActionType.ListData, null, request, null, "HSRPLaserNoStockServiceRepository.GetLaserStockSummary()");
            }
            catch (Exception ex)
            {
                resultdata = _errorLoggerService.LogException(ex, null, "HSRPLaserNoStockServiceRepository.GetLaserStockSummary()");
            }
            return resultdata;
        }
        public DataResponse GetHSRPLaserNoStockLogByID(int HSRPLaserNoStockID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPLaserNoStockLog.Where(w=>w.HSRPLaserNoStockID== HSRPLaserNoStockID).ToList();

                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;

                _auditLogger.SaveActionLog("VHSRPLaserNoStockLog", ActionType.ListData, null, HSRPLaserNoStockID, null, "HSRPLaserNoStockServiceRepository.GetHSRPLaserNoStockLogByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPLaserNoStockServiceRepository.GetHSRPLaserNoStockLogByID()");
            }

            return dataResponse;
        }
    }
}
