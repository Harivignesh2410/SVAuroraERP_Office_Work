namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class BoxServiceRepository : IBoxServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public BoxServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<BoxServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                     IErrorLoggerService errorLoggerService,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
        }
        public DataResponse GetBox()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VBox.OrderBy(o => o.BoxName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Box", ActionType.ListData, null, null, null, "BoxServiceRepository.GetBox()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "BoxServiceRepository.GetBox()");
            }
            return dataResponse;
        }
        public DataResponse GetByID(int BoxID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VBox.FirstOrDefault(w => w.BoxID == BoxID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                dataResponse.ID = BoxID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Box", ActionType.Select, BoxID.ToString(), BoxID, null, "BoxServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, BoxID, "BoxServiceRepository.GetByID()");
            }
            return dataResponse;
        }
        public DataResponse Save(Box request)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.Box.FirstOrDefault(r => r.BoxName == request.BoxName);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.BoxID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.Box.Add(request);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("Box", ActionType.Insert, request.BoxID.ToString(), request, null, "BoxServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "BoxServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(Box request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.Box.FirstOrDefault(r => r.BoxID != request.BoxID && r.BoxName == request.BoxName);

                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.BoxID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataToUpdate = _dbcontext.Box.FirstOrDefault(r => r.BoxID == request.BoxID);
                if (dataToUpdate == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("MapPlateSize", ActionType.Update, request.BoxID.ToString(), request, dataToUpdate, "BoxServiceRepository.Update()");
                dataToUpdate.SizeID = request.SizeID;
                dataToUpdate.BoxName = request.BoxName;
                dataToUpdate.MaxCapacity = request.MaxCapacity;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;
                //Added on 2025.04.19 by Harivignesh
                dataToUpdate.InnerBoxCount = request.InnerBoxCount;
                dataToUpdate.InnerBoxQuantity = request.InnerBoxQuantity;

                _dbcontext.SaveChanges();
                dataResponse.ID = dataToUpdate.BoxID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "BoxServiceRepository.Update()");
            }
            return dataResponse;
        }

        public DataResponse Delete(int BoxID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataToDelete = _dbcontext.Box.FirstOrDefault(w => w.BoxID == BoxID);
                if (dataToDelete == null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }

                dataToDelete.LastUpdatedDate = DateTime.UtcNow;
                dataToDelete.LastUpdatedBy = UserID;
                dataToDelete.IsDeleted = true;
                _dbcontext.SaveChanges();

                dataResponse.ID = dataToDelete.BoxID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Box", ActionType.Delete, BoxID.ToString(), null, dataToDelete, "BoxServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, BoxID, "BoxServiceRepository.Delete()");
            }
            return dataResponse;
        }
        public DataResponse GetBoxtoDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VBox> query = _dbcontext.VBox;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.BoxName.Contains(request.SearchValue) ||
                                             d.SizeName.Contains(request.SearchValue));
                }

                if (request.IsCustomFilterEnabled)
                {

                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VBox.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.BoxName,
                                                       w.SizeName,
                                                       w.BoxID,
                                                       w.MaxCapacity,
                                                       w.InnerBoxCount,
                                                       w.InnerBoxQuantity,
                                                       w.IsActive
                                                   }).ToList();


                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = pagedData.Count;
                _auditLogger.SaveActionLog("Box", ActionType.ListData, null, request, null, "BoxServiceRepository.GetBoxtoDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "BoxServiceRepository.GetBoxtoDataTable()");
            }
            return response;
        }
    }
}
