namespace SVAuroraERP.Infrastructure.Repositories.Master
{
    public class TaxserviceRepository : ITaxServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<TaxserviceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public TaxserviceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<TaxserviceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IAuditLogger auditLogger
                                     )
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetTax()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VTax.OrderBy(o => o.TaxCode).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Tax", ActionType.ListData, null, null,null, "TaxserviceRepository.GetTax()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "TaxserviceRepository.GetTax()");
            }

            return dataResponse;
        }
        public DataResponse GetByID(int TaxID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VTax.FirstOrDefault(w => w.TaxID == TaxID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataResponse.ID = TaxID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Tax", ActionType.Select, TaxID.ToString(), TaxID, null, "TaxserviceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, TaxID, "TaxserviceRepository.GetByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(Tax request)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.Tax.FirstOrDefault(r => r.TaxCode == request.TaxCode);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.TaxID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.Tax.Add(request);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("Tax", ActionType.Insert, dataexists.TaxID.ToString(), request, null, "TaxserviceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "TaxserviceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(Tax request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.Tax.FirstOrDefault(r => r.TaxID != request.TaxID && r.TaxName == request.TaxName && r.TaxPercentage == request.TaxPercentage);

                if (isFound != null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = isFound.TaxID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.Tax.FirstOrDefault(r => r.TaxID == request.TaxID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("Tax", ActionType.Update, request.TaxID.ToString(), request, dataexists, "TaxserviceRepository.Update()");
                dataexists.TaxCode = request.TaxCode;
                dataexists.TaxName = request.TaxName;
                dataexists.TaxPercentage = request.TaxPercentage;
                dataexists.IsActive = request.IsActive;
                dataexists.LastUpdatedBy = request.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;

                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.TaxID;
                dataResponse.Message = Constants.UpdatedSucessfully;


            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "TaxserviceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int TaxID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Tax.FirstOrDefault(w => w.TaxID == TaxID);
                if (dataexists == null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }

                dataexists.LastUpdatedDate = DateTime.UtcNow;
                dataexists.LastUpdatedBy = UserID;
                dataexists.IsDeleted = true;
                _dbcontext.SaveChanges();

                dataResponse.ID = dataexists.TaxID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Tax", ActionType.Delete, TaxID.ToString(), null, null, "TaxserviceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, TaxID, "TaxserviceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetTaxDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();

            // Validate and sanitize inputs
            var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
            var skip = Math.Max(request.Start, 0);

            IQueryable<VTax> query = _dbcontext.VTax;

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchValue))
            {
                query = query.Where(d => d.TaxName.Contains(request.SearchValue)|| d.TaxCode.Contains(request.SearchValue));
            }

            // Get TOTAL records in database (unfiltered)
            var totalRecords = _dbcontext.VTax.Count();

            // Get FILTERED records count (same as total if no filter applied)
            var filteredRecords = query.Count();

            // Apply sorting 
            query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

            // Apply paging
            var pagedData = query.Skip(skip).Take(pageSize)
                                   .Select(w => new
                                   {
                                       w.TaxID,
                                       w.TaxName,
                                       w.TaxPercentage,
                                       w.TaxCode,
                                       w.IsActive
                                   }).ToList();

            response.Value = pagedData;
            response.recordsTotal = totalRecords;
            response.recordsFiltered = filteredRecords;
            _auditLogger.SaveActionLog("Tax", ActionType.ListData, null, request, null, "TaxserviceRepository.GetTaxDataTable()");
            return response;
        }
    }
}