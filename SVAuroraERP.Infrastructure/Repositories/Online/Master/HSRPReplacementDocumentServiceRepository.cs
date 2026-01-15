namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class HSRPReplacementDocumentServiceRepository : IHSRPReplacementDocumentServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<HSRPReplacementDocumentServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public HSRPReplacementDocumentServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<HSRPReplacementDocumentServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                       IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetHSRPReplacementDocument()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPReplacementDocument.OrderBy(o => o.ReplacementReasonName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("HSRPReplacementDocument", ActionType.ListData, null, null,null, "HSRPReplacementDocumentServiceRepository.GetHSRPReplacementDocument()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HSRPReplacementDocumentServiceRepository.GetHSRPReplacementDocument()");
            }

            return dataResponse;
        }
        public DataResponse GetHSRPReplacementDocumentByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPReplacementDocument.FirstOrDefault(w => w.HSRPReplacementDocumentID == ID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataResponse.ID = ID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("HSRPReplacementDocument", ActionType.Select, ID.ToString(), ID, null, "HSRPReplacementDocumentServiceRepository.GetHSRPReplacementDocumentByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "HSRPReplacementDocumentServiceRepository.GetHSRPReplacementDocumentByID()");
            }
            return dataResponse;
        }
        public DataResponse Save(HSRPReplacementDocument HSRPReplacementDocument)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.HSRPReplacementDocument.FirstOrDefault(r => r.ReplacementDocumentName == HSRPReplacementDocument.ReplacementDocumentName);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.HSRPReplacementDocumentID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                HSRPReplacementDocument.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.HSRPReplacementDocument.Add(HSRPReplacementDocument);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("HSRPReplacementDocument", ActionType.Insert, HSRPReplacementDocument.HSRPReplacementDocumentID.ToString(), HSRPReplacementDocument,null, "HSRPReplacementDocumentServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPReplacementDocument, "HSRPReplacementDocumentServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(HSRPReplacementDocument HSRPReplacementDocument)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.HSRPReplacementDocument.FirstOrDefault(r => r.HSRPReplacementDocumentID != HSRPReplacementDocument.HSRPReplacementDocumentID && r.ReplacementDocumentName == HSRPReplacementDocument.ReplacementDocumentName);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.HSRPReplacementDocumentID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.HSRPReplacementDocument.FirstOrDefault(r => r.HSRPReplacementDocumentID == HSRPReplacementDocument.HSRPReplacementDocumentID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("HSRPReplacementDocument", ActionType.Update, dataexists.HSRPReplacementDocumentID.ToString(), HSRPReplacementDocument, dataexists, "HSRPReplacementDocumentServiceRepository.Update()");
                dataexists.ReplacementReasonID = HSRPReplacementDocument.ReplacementReasonID;
                dataexists.ReplacementDocumentName = HSRPReplacementDocument.ReplacementDocumentName;
                dataexists.Code = HSRPReplacementDocument.Code;
                dataexists.LastUpdatedBy = HSRPReplacementDocument.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.HSRPReplacementDocumentID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPReplacementDocument, "HSRPReplacementDocumentServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int HSRPReplacementDocumentID, int UserID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.HSRPReplacementDocument.FirstOrDefault(w => w.HSRPReplacementDocumentID == HSRPReplacementDocumentID);
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

                dataResponse.ID = dataexists.HSRPReplacementDocumentID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("HSRPReplacementDocument", ActionType.Delete, null, HSRPReplacementDocumentID,null, "HSRPReplacementDocumentServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HSRPReplacementDocumentID, "HSRPReplacementDocumentServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetHSRPReplacementDocumentDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHSRPReplacementDocument> query = _dbcontext.VHSRPReplacementDocument;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.ReplacementDocumentName ?? "").Contains(request.SearchValue)
                    || (d.Code ?? "").Contains(request.SearchValue)
                    || (d.ReplacementReasonName ?? "").Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VHSRPReplacementDocument.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.HSRPReplacementDocumentID,
                                w.ReplacementDocumentName,
                                w.ReplacementReasonName,
                                w.Code
                            }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("HSRPReplacementDocument", ActionType.Select, null, request, null, "HSRPReplacementDocumentServiceRepository.GetHSRPReplacementDocumentDataTableList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "HSRPReplacementDocumentServiceRepository.GetHSRPReplacementDocumentDataTableList()");
            }
            return response;
        }
    }
}