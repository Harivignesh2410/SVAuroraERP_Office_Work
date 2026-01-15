using SVAuroraERP.Domain.Inventory.Master;

namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class OtherChargesServiceRepository : IOtherChargesServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public OtherChargesServiceRepository(SVAuroraERPDbContext dbcontext,
                                         IAuditLogger auditLogger,
                                          IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetOtherCharges()
        {
            DataResponse response = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VOtherCharges.ToList();
                if (resultdata == null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.Message = Constants.NoRecordFound;
                    return response;
                }
                response.Count = resultdata.Count;
                response.Message = Constants.SuccessMessage;
                response.Value = resultdata;
                _auditLogger.SaveActionLog("OtherCharges", ActionType.ListData, null, null, null, "OtherChargesServiceRepository.GetOtherCharges()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "OtherChargesServiceRepository.GetOtherCharges()");
            }

            return response;
        }
        public DataResponse GetByID(int OtherChargesID)
        {
            DataResponse response = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VOtherCharges.FirstOrDefault(w => w.OtherChargesID == OtherChargesID);
                if (resultdata == null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.ID = 0;
                    response.Message = Constants.NoRecordFound;

                    return response;
                }
                else
                {
                    response.ID = OtherChargesID;
                    response.Message = Constants.RecordFound;
                    response.Value = resultdata;
                }
                _auditLogger.SaveActionLog("OtherCharges", ActionType.Select, OtherChargesID.ToString(), resultdata, null, "OtherChargesServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, OtherChargesID, "OtherChargesServiceRepository.GetByID()");
            }

            return response;
        }
        public DataResponse Save(OtherCharges request)
        {
            DataResponse response = new DataResponse();
            try
            {
                var dataexists = _dbcontext.OtherCharges.FirstOrDefault(r => r.OtherChargesDescription == request.OtherChargesDescription);
                if (dataexists != null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.ID = dataexists.OtherChargesID;
                    response.Message = Constants.DataAlreadyExist;
                    return response;
                }
                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.OtherCharges.Add(request);
                _dbcontext.SaveChanges();
                response.ID = request.OtherChargesID;
                response.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("OtherCharges", ActionType.Insert, request.OtherChargesID.ToString(), request, null, "OtherChargesServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "OtherChargesServiceRepository.Save()");
            }

            return response;
        }
        public DataResponse Update(OtherCharges request)
        {
            DataResponse response = new DataResponse();
            try
            {
                var isFound = _dbcontext.OtherCharges.FirstOrDefault(r => r.OtherChargesID != request.OtherChargesID && r.OtherChargesDescription == request.OtherChargesDescription);
                if (isFound != null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.ID = isFound.OtherChargesID;
                    response.Message = Constants.DataAlreadyExist;
                    return response;
                }
                var dataToUpdate = _dbcontext.OtherCharges.FirstOrDefault(r => r.OtherChargesID == request.OtherChargesID);
                if (dataToUpdate == null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.ID = 0;
                    response.Message = Constants.NoRecordFound;

                    return response;
                }
                _auditLogger.SaveActionLog("OtherCharges", ActionType.Update, request.OtherChargesID.ToString(), request, dataToUpdate, "OtherChargesServiceRepository.Update()");
                dataToUpdate.OtherChargesDescription = request.OtherChargesDescription;
                dataToUpdate.Type = request.Type;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                response.ID = dataToUpdate.OtherChargesID;
                response.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "OtherChargesServiceRepository.Update()");
            }

            return response;
        }
        public DataResponse Delete(int OtherChargesID, int UserID, long LoginAuditID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var dataToDelete = _dbcontext.OtherCharges.FirstOrDefault(w => w.OtherChargesID == OtherChargesID);
                if (dataToDelete == null)
                {
                    DataResponse.Error = false;
                    DataResponse.Success = true;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                dataToDelete.LastUpdatedDate = DateTime.UtcNow;
                dataToDelete.LastUpdatedBy = UserID;
                dataToDelete.IsDeleted = true;
                _dbcontext.SaveChanges();

                DataResponse.ID = dataToDelete.OtherChargesID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("OtherCharges", ActionType.Delete, OtherChargesID.ToString(), null, dataToDelete, "OtherChargesServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, OtherChargesID, "OtherChargesServiceRepository.Delete()");
            }

            return DataResponse;
        }
        public DataResponse GetOtherChargesDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VOtherCharges> query = _dbcontext.VOtherCharges;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.OtherChargesDescription.Contains(request.SearchValue) || d.TypeName.Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VOtherCharges.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.OtherChargesID,
                                           w.OtherChargesDescription,
                                           w.TypeName,
                                           w.IsActive
                                       }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("OtherCharges", ActionType.ListData, null, request, null, "OtherChargesServiceRepository.GetOtherChargesDataTable()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "OtherChargesServiceRepository.GetOtherChargesDataTable()");
            }

            return response;
        }
    }
}
