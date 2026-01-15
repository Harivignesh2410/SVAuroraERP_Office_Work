namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class ColorServiceRepository : IColorServiceRespository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public ColorServiceRepository(SVAuroraERPDbContext dbcontext,
                                     IAuditLogger auditLogger,
                                     IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetColor()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VColor.OrderBy(o => o.ColorCode).ToList();
                DataResponse.Count = resultdata.Count;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Color", ActionType.ListData, null, null, null, "ColorServiceRepository.GetColor()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "ColorServiceRepository.GetColor()");
            }
            return DataResponse;
        }
        public DataResponse GetByID(int ColorID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VColor.FirstOrDefault(w => w.ColorID == ColorID);
                if (resultdata == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                DataResponse.ID = ColorID;
                DataResponse.Message = Constants.RecordFound;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Color", ActionType.Select, ColorID.ToString(), ColorID, null, "ColorServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, ColorID, "ColorServiceRepository.GetByID()");
            }
            return DataResponse;
        }
        public DataResponse Save(Color request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Color.FirstOrDefault(r => r.ColorCode == request.ColorCode || r.ColorName == request.ColorName);
                if (dataexists == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;
                    return DataResponse;
                }
                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.Color.Add(request);
                _dbcontext.SaveChanges();
                DataResponse.ID = request.ColorID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Color", ActionType.Insert, request.ColorID.ToString(), request, null, "ColorServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "ColorServiceRepository.Save()");
            }
            return DataResponse;
        }
        public DataResponse Update(Color request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.Color.FirstOrDefault(r => r.ColorID != request.ColorID && r.ColorName == request.ColorName || r.ColorCode == request.ColorCode);
                if (isFound != null)
                {
                    DataResponse.Error = false;
                    DataResponse.Success = true;
                    DataResponse.ID = isFound.ColorID;
                    DataResponse.Message = Constants.NoRecordFound;
                    return DataResponse;
                }
                var dataToUpdate = _dbcontext.Color.FirstOrDefault(r => r.ColorID == request.ColorID);
                if (dataToUpdate == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                _auditLogger.SaveActionLog("Color", ActionType.Update, request.ColorID.ToString(), request, dataToUpdate, "DesignationServiceRepository.Update()");
                dataToUpdate.ColorCode = request.ColorCode;
                dataToUpdate.ColorName = request.ColorName;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();

                DataResponse.ID = dataToUpdate.ColorID;
                DataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "ColorServiceRepository.Update()");
            }

            return DataResponse;
        }
        public DataResponse Delete(int ColorID, int UserID, long LoginAuditID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Color.FirstOrDefault(w => w.ColorID == ColorID);
                if (dataexists == null)
                {
                    DataResponse.Error = false;
                    DataResponse.Success = true;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                dataexists.LastUpdatedBy = UserID;
                dataexists.IsDeleted = true;
                _dbcontext.SaveChanges();

                DataResponse.ID = dataexists.ColorID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Color", ActionType.Delete, ColorID.ToString(), null, null, "ColorServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, ColorID, "ColorServiceRepository.Delete()");
            }

            return DataResponse;
        }
        public DataResponse GetColorDataTable(DataTableRequest request)
        {

            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VColor> query = _dbcontext.VColor;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.ColorName.Contains(request.SearchValue) || d.ColorCode.Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VColor.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.ColorID,
                                           w.ColorCode,
                                           w.ColorName,
                                           w.IsActive
                                       }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("Color", ActionType.ListData, null, request, null, "ColorServiceRepository.GetColorDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ColorServiceRepository.GetColorDataTable()");
            }
            return response;
        }
    }
}