using SVAuroraERP.Domain.Inventory.Master;

namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class DocumentGroupServiceRepository : IDocumentGroupServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public DocumentGroupServiceRepository(SVAuroraERPDbContext dbcontext,
                                     IAuditLogger auditLogger,
                                     IErrorLoggerService errorLoggerService
            )
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetDocumentGroup()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDocumentGroup.OrderBy(o => o.DocumentGroupCode).ToList();
                DataResponse.Count = resultdata.Count;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("DocumentGroup", ActionType.ListData, null, null, null, "DocumentGroupServiceRepository.GetDocumentGroup()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "DocumentGroupServiceRepository.GetDocumentGroup()");
            }

            return DataResponse;
        }

        public DataResponse GetByID(int DocumentGroupID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDocumentGroup.FirstOrDefault(w => w.DocumentGroupID == DocumentGroupID);
                if (resultdata == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                else
                {
                    DataResponse.ID = DocumentGroupID;
                    DataResponse.Message = Constants.RecordFound;
                    DataResponse.Value = resultdata;
                }
                _auditLogger.SaveActionLog("DocumentGroup", ActionType.Select, DocumentGroupID.ToString(), DocumentGroupID, null, "DocumentGroupServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, DocumentGroupID, "DocumentGroupServiceRepository.GetByID()");
            }

            return DataResponse;
        }
        public DataResponse Save(DocumentGroup request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.DocumentGroup.FirstOrDefault(r => r.DocumentGroupCode == request.DocumentGroupCode);
                if (dataexists != null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = dataexists.DocumentGroupID;
                    DataResponse.Message = Constants.DataAlreadyExist;
                    return DataResponse;
                }

                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.DocumentGroup.Add(request);
                _dbcontext.SaveChanges();
                DataResponse.ID = request.DocumentGroupID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("DocumentGroup", ActionType.Insert, request.DocumentGroupID.ToString(), request, null, "DocumentGroupServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "DocumentGroupServiceRepository.Save()");
            }

            return DataResponse;
        }
        public DataResponse Update(DocumentGroup request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.DocumentGroup.FirstOrDefault(r => r.DocumentGroupID != request.DocumentGroupID && r.DocumentGroupName == request.DocumentGroupName);
                if (isFound != null)
                {
                    DataResponse.Error = false;
                    DataResponse.Success = true;
                    DataResponse.ID = isFound.DocumentGroupID;
                    DataResponse.Message = Constants.DataAlreadyExist;
                    return DataResponse;
                }
                var dataToUpdate = _dbcontext.DocumentGroup.FirstOrDefault(r => r.DocumentGroupID == request.DocumentGroupID);
                if (dataToUpdate == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                _auditLogger.SaveActionLog("DocumentGroup", ActionType.Update, request.DocumentGroupID.ToString(), request, dataToUpdate, "DocumentGroupServiceRepository.Update()");
                dataToUpdate.DocumentGroupCode = request.DocumentGroupCode;
                dataToUpdate.DocumentGroupName = request.DocumentGroupName;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();

                DataResponse.ID = dataToUpdate.DocumentGroupID;
                DataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "DocumentGroupServiceRepository.Update()");
            }

            return DataResponse;
        }

        public DataResponse Delete(int DocumentGroupID, int UserID, long LoginAuditID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var dataToDelete = _dbcontext.DocumentGroup.FirstOrDefault(w => w.DocumentGroupID == DocumentGroupID);
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

                DataResponse.ID = dataToDelete.DocumentGroupID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("DocumentGroup", ActionType.Delete, DocumentGroupID.ToString(), null, dataToDelete, "DocumentGroupServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, DocumentGroupID, "DocumentGroupServiceRepository.Delete()");
            }

            return DataResponse;
        }
        public DataResponse GetDocumentGroupDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VDocumentGroup> query = _dbcontext.VDocumentGroup;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.DocumentGroupName.Contains(request.SearchValue) || d.DocumentGroupCode.Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VDocumentGroup.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.DocumentGroupID,
                                           w.DocumentGroupCode,
                                           w.DocumentGroupName,
                                           w.IsActive
                                       }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("DocumentGroup", ActionType.ListData, null, request, null, "DocumentGroupServiceRepository.GetDocumentGroupDataTable()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "DocumentGroupServiceRepository.GetDocumentGroupDataTable()");
            }
            return response;
        }
    }
}