using SVAuroraERP.Domain.Master;

namespace SVAuroraERP.Infrastructure.Repositories.Master
{
    public class DocumentTypeServiceRepository : IDocumentTypeServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public DocumentTypeServiceRepository(SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger,
                                            IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetDocumentType()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDocumentType.OrderBy(o => o.DocumentTypeCode).ToList();
                DataResponse.Count = resultdata.Count;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("DocumentType", ActionType.ListData, null, null, null, "DocumentTypeServiceRepository.GetDocumentType()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "DocumentTypeServiceRepository.GetDocumentType()");
            }

            return DataResponse;
        }
        public DataResponse GetByID(int DocumentTypeID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDocumentType.FirstOrDefault(w => w.DocumentTypeID == DocumentTypeID);
                if (resultdata == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                DataResponse.ID = DocumentTypeID;
                DataResponse.Message = Constants.RecordFound;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("DocumentType", ActionType.Select, DocumentTypeID.ToString(), DocumentTypeID, null, "DocumentTypeServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, DocumentTypeID, "DocumentTypeServiceRepository.GetByID()");
            }

            return DataResponse;
        }
        public DataResponse Save(DocumentType request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.DocumentType.FirstOrDefault(r => r.DocumentTypeCode == request.DocumentTypeCode);
                if (dataexists != null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = dataexists.DocumentTypeID;
                    DataResponse.Message = Constants.DataAlreadyExist;
                    return DataResponse;
                }
                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.DocumentType.Add(request);
                _dbcontext.SaveChanges();
                DataResponse.ID = request.DocumentTypeID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("DocumentType", ActionType.Insert, request.DocumentTypeID.ToString(), request, null, "DocumentTypeServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "DocumentTypeServiceRepository.Save()");
            }

            return DataResponse;
        }
        public DataResponse Update(DocumentType request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.DocumentType.FirstOrDefault(r => r.DocumentTypeID != request.DocumentTypeID && r.DocumentTypeName == request.DocumentTypeName);
                if (isFound != null)
                {
                    DataResponse.Error = false;
                    DataResponse.Success = true;
                    DataResponse.ID = isFound.DocumentTypeID;
                    DataResponse.Message = Constants.DataAlreadyExist;
                    return DataResponse;
                }
                var dataToUpdate = _dbcontext.DocumentType.FirstOrDefault(r => r.DocumentTypeID == request.DocumentTypeID);
                if (dataToUpdate == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                _auditLogger.SaveActionLog("DocumentType", ActionType.Update, request.DocumentTypeID.ToString(), request, dataToUpdate, "DocumentTypeServiceRepository.Update()");
                dataToUpdate.DocumentGroupID = request.DocumentGroupID;
                dataToUpdate.DocumentTypeCode = request.DocumentTypeCode;
                dataToUpdate.DocumentTypeName = request.DocumentTypeName;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                DataResponse.ID = dataToUpdate.DocumentTypeID;
                DataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "DocumentTypeServiceRepository.Update()");
            }

            return DataResponse;
        }
        public DataResponse Delete(int DocumentTypeID, int UserID, long LoginAuditID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {


                var dataToDelete = _dbcontext.DocumentType.FirstOrDefault(w => w.DocumentTypeID == DocumentTypeID);
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
                DataResponse.ID = dataToDelete.DocumentTypeID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("DocumentType", ActionType.Delete, DocumentTypeID.ToString(), null, dataToDelete, "DocumentTypeServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, DocumentTypeID, "DocumentTypeServiceRepository.Delete()");
            }

            return DataResponse;
        }
        public DataResponse GetDocumentTypeDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VDocumentType> query = _dbcontext.VDocumentType;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.DocumentGroupName.Contains(request.SearchValue) || d.DocumentTypeName.Contains(request.SearchValue)
                                                || d.DocumentTypeCode.Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VDocumentType.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.DocumentTypeID,
                                           w.DocumentGroupID,
                                           w.DocumentGroupName,
                                           w.DocumentTypeName,
                                           w.DocumentTypeCode,

                                           w.IsActive
                                       }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VDocumentType", ActionType.ListData, null, request, null, "DocumentTypeServiceRepository.GetDocumentTypeDataTable()");

                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "DocumentTypeServiceRepository.GetDocumentTypeDataTable()");
            }
            return response;
        }
    }
}

