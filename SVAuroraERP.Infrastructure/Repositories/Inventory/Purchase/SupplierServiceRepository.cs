using SVAuroraERP.Domain.Authentication;

namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Purchase
{
    public class SupplierServiceRepository : ISupplierServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<SupplierServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public SupplierServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<SupplierServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IErrorLoggerService errorLoggerService,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
        }


        public DataResponse GetSupplier()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VSupplier.OrderBy(o => o.SupplierName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VSupplier", ActionType.ListData, null, null, "SupplierServiceRepository.GetSupplier()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "SupplierServiceRepository.GetSupplier()");
            }
            return dataResponse;

        }
        public DataResponse GetByID(int SupplierID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VSupplier.FirstOrDefault(w => w.SupplierID == SupplierID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;
                }
                dataResponse.ID = SupplierID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VSupplier", ActionType.Select, SupplierID.ToString(), SupplierID, null, "SupplierServiceRepository.GetByID()");

            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, SupplierID, "SupplierServiceRepository.GetByID()");

            }
            return dataResponse;
        }
        public DataResponse Save(Supplier request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Supplier.FirstOrDefault(r => r.SupplierCode == request.SupplierCode);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.SupplierID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    request.LastUpdatedDate = DateTime.UtcNow;
                }
                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.Supplier.Add(request);
                _dbcontext.SaveChanges();

                _auditLogger.SaveActionLog("Supplier", ActionType.Insert, null, request, null, "SupplierServiceRepository.Save()");

            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "SupplierServiceRepository.Save()");
            }
            return dataResponse;
        }
        public DataResponse Update(Supplier request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.Supplier.FirstOrDefault(r => r.SupplierID != request.SupplierID && r.SupplierCode == request.SupplierCode && r.SupplierName == request.SupplierName);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.SupplierID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.Supplier.FirstOrDefault(r => r.SupplierID == request.SupplierID);
                if (dataexists == null)
                {

                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;
                    return dataResponse;
                }
                _auditLogger.SaveActionLog("Supplier", ActionType.Update, request.SupplierID.ToString(), request, dataexists, "SupplierServiceRepository.Update()");
                dataexists.SupplierCode = request.SupplierCode;
                dataexists.SupplierName = request.SupplierName;
                dataexists.GSTNo = request.GSTNo;
                dataexists.AddressLine1 = request.AddressLine1;
                dataexists.AddressLine2 = request.AddressLine2;
                dataexists.City = request.City;
                dataexists.State = request.State;
                dataexists.Country = request.Country;
                dataexists.Pincode = request.Pincode;
                dataexists.TelNo1 = request.TelNo1;
                dataexists.TelNo2 = request.TelNo2;
                dataexists.MobileNo = request.MobileNo;
                dataexists.Email = request.Email;
                dataexists.IsActive = request.IsActive;
                dataexists.LastUpdatedBy = request.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "SupplierServiceRepository.Update()");
            }
            return dataResponse;
        }
        public DataResponse Delete(int SupplierID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Supplier.FirstOrDefault(w => w.SupplierID == SupplierID);
                if (dataexists == null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                dataexists.IsDeleted = true;
                _dbcontext.SaveChanges();

                dataResponse.ID = dataexists.SupplierID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Supplier", ActionType.Delete, SupplierID.ToString(), null, null, "SupplierServiceRepository.Delete()");

            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, SupplierID, "SupplierServiceRepository.Delete()");
            }
            return dataResponse;
        }
        public DataResponse GetSupplierDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VSupplier> query = _dbcontext.VSupplier;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.SupplierCode ?? "").Contains(request.SearchValue)
                                            || (d.GSTNo ?? "").Contains(request.SearchValue)
                                            || (d.City ?? "").Contains(request.SearchValue)
                                            || (d.MobileNo ?? "").Contains(request.SearchValue)
                                            || (d.Email ?? "").Contains(request.SearchValue)
                                            || (d.SupplierName ?? "").Contains(request.SearchValue));
                }

                if (request.IsCustomFilterEnabled)
                {

                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VPurchaseEntry.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.SupplierID,
                                                       w.SupplierName,
                                                       w.SupplierCode,
                                                       w.Email,
                                                       w.MobileNo,
                                                       w.GSTNo,
                                                       w.City,
                                                       w.IsActive
                                                   }).ToList();
                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VSupplier", ActionType.ListData, null, null, "SupplierServiceRepository.GetSupplierDataTable()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "SupplierServiceRepository.GetSupplierDataTable()");
                return response;
            }
        }
    }
}

