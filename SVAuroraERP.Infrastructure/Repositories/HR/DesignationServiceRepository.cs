namespace SVAuroraERP.Infrastructure.Repositories.HR
{
    public class DesignationServiceRepository : IDesignationServiceRepository
    {

        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public DesignationServiceRepository(SVAuroraERPDbContext dbcontext,
                                     IAuditLogger auditLogger,
                                     IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetDesignation()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDesignation.OrderBy(o => o.DesignationName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Designation", ActionType.ListData, null, null, null, "DesignationServiceRepository.GetDesignation()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "DesignationServiceRepository.GetDesignation()");
            }
            return dataResponse;
        }
        public DataResponse GetByID(int DesignationID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDesignation.FirstOrDefault(w => w.DesignationID == DesignationID);
                if (resultdata != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;
                }
                dataResponse.ID = DesignationID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Designation", ActionType.Select, DesignationID.ToString(), DesignationID, null, "DesignationServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, DesignationID, "DesignationServiceRepository.GetByID()");

            }
            return dataResponse;
        }
        public DataResponse Save(Designation request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Designation.FirstOrDefault(r => r.DesignationName == request.DesignationName);
                if (dataexists != null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.DesignationID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    request.LastUpdatedDate = DateTime.UtcNow;
                }
                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.Designation.Add(request);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("Designation", ActionType.Insert, null, request, null, "DesignationServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "DesignationServiceRepository.Save()");
            }
            return dataResponse;
        }
        public DataResponse Update(Designation request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.Designation.FirstOrDefault(r => r.DesignationID != request.DesignationID && r.DesignationName == request.DesignationName);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.DesignationID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataToUpdate = _dbcontext.Designation.FirstOrDefault(r => r.DesignationID == request.DesignationID);
                if (dataToUpdate == null)
                {

                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;
                    return dataResponse;
                }
                _auditLogger.SaveActionLog("Designation", ActionType.Update, request.DesignationID.ToString(), request, dataToUpdate, "DesignationServiceRepository.Update()");
                dataToUpdate.DesignationName = request.DesignationName;
                dataToUpdate.Description = request.Description;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;

                _dbcontext.SaveChanges();
                dataResponse.ID = dataToUpdate.DesignationID;
                dataResponse.Message = Constants.UpdatedSucessfully;

            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "DesignationServiceRepository.Update()");
            }
            return dataResponse;
        }
        public DataResponse Delete(int DesignationID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Designation.FirstOrDefault(w => w.DesignationID == DesignationID);
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

                dataResponse.ID = dataexists.DesignationID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Designation", ActionType.Delete, DesignationID.ToString(), null, null, "DesignationServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, DesignationID, "DesignationServiceRepository.Delete()");
            }
            return dataResponse;
        }
        public DataResponse GetDesignationDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VDesignation> query = _dbcontext.VDesignation;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.DesignationName.Contains(request.SearchValue) ||
                                             d.Description.Contains(request.SearchValue));
                }

                if (request.IsCustomFilterEnabled)
                {

                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VDesignation.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.DesignationID,
                                                       w.DesignationName,
                                                       w.Description,
                                                       w.IsActive
                                                   }).ToList();


                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("Designation", ActionType.ListData, null, request, null, "DesignationServiceRepository.GetDesignationDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "DesignationServiceRepository.GetDesignationDataTable()");
            }

            return response;
        }

    }
}
