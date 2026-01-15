namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class CourierServiceRepository : ICourierServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public CourierServiceRepository(SVAuroraERPDbContext dbcontext,
                                     IAuditLogger auditLogger,
                                     IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetCourier()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VCourier.OrderBy(o => o.CourierName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Courier", ActionType.ListData, null, null, null, "CourierServiceRepository.GetCourier()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "CourierServiceRepository.GetCourier()");
            }

            return dataResponse;
        }
        public DataResponse GetByID(int CourierID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VCourier.FirstOrDefault(w => w.CourierID == CourierID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataResponse.ID = CourierID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Courier", ActionType.Select, CourierID.ToString(), CourierID, null, "CourierServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, CourierID, "CourierServiceRepository.GetByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(Courier request)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.Courier.FirstOrDefault(r => r.CourierName == request.CourierName);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.CourierID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.Courier.Add(request);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("Courier", ActionType.Insert, request.CourierID.ToString(), request, null, "CourierServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "CourierServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(Courier request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.Courier.FirstOrDefault(r => r.CourierID != request.CourierID && r.CourierName == request.CourierName);

                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.CourierID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataToUpdate = _dbcontext.Courier.FirstOrDefault(r => r.CourierID == request.CourierID);
                if (dataToUpdate == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("Courier", ActionType.Update, request.CourierID.ToString(), request, dataToUpdate, "CourierServiceRepository.Update()");
                dataToUpdate.CourierCode = request.CourierCode;
                dataToUpdate.CourierName = request.CourierName;
                dataToUpdate.Address = request.Address;
                dataToUpdate.ContactNo1 = request.ContactNo1;
                dataToUpdate.ContactNo2 = request.ContactNo2;
                dataToUpdate.Email = request.Email;
                dataToUpdate.TelNo = request.TelNo;
                dataToUpdate.TrackingURL = request.TrackingURL;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;

                _dbcontext.SaveChanges();
                dataResponse.ID = dataToUpdate.CourierID;
                dataResponse.Message = Constants.UpdatedSucessfully;


            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "CourierServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int CourierID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Courier.FirstOrDefault(w => w.CourierID == CourierID);
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

                dataResponse.ID = dataexists.CourierID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Courier", ActionType.Delete, CourierID.ToString(), null, dataexists, "CourierServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, CourierID, "CourierServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetCourierDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {

                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VCourier> query = _dbcontext.VCourier;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.CourierCode.Contains(request.SearchValue) || d.CourierName.Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VCourier.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.CourierID,
                                           w.CourierName,
                                           w.CourierCode,
                                           w.Address,
                                           w.ContactNo1,
                                           w.Email,
                                           w.TrackingURL,
                                           w.IsActive
                                       }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("Courier", ActionType.ListData, null, request, null, "CourierServiceRepository.GetCourierDataTable()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request,"CourierServiceRepository.GetCourierDataTable()");
            }
            return response;
        }
    }
}