namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class CategoryServiceRepository : ICategoryServiceRespository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public CategoryServiceRepository(SVAuroraERPDbContext dbcontext,
                                     IErrorLoggerService errorLoggerService,
                                     IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
        }
        public DataResponse GetCategory()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {

                var resultdata = _dbcontext.VCategory.OrderBy(o => o.CategoryCode).ToList();
                DataResponse.Count = resultdata.Count;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Category", ActionType.ListData, null, null, null, "CategoryServiceRepository.GetCategory()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "CategoryServiceRepository.GetCategory()");
            }
            return DataResponse;
        }
        public DataResponse GetByID(int CategoryID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VCategory.FirstOrDefault(w => w.CategoryID == CategoryID);
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
                    DataResponse.ID = CategoryID;
                    DataResponse.Message = Constants.RecordFound;
                    DataResponse.Value = resultdata;
                }
                _auditLogger.SaveActionLog("Category", ActionType.Select, CategoryID.ToString(), CategoryID, null, "CategoryServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, CategoryID, "CategoryServiceRepository.GetByID()");
            }
            return DataResponse;
        }
        public DataResponse Save(Category request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Category.FirstOrDefault(r => r.CategoryCode == request.CategoryCode);
                if (dataexists != null)
                {
                    DataResponse.Error = false;
                    DataResponse.Success = false;
                    DataResponse.ID = dataexists.CategoryID;
                    DataResponse.Message = Constants.DataAlreadyExist;
                    return DataResponse;
                }

                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.Category.Add(request);
                _dbcontext.SaveChanges();
                DataResponse.ID = request.CategoryID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Category", ActionType.Insert, request.CategoryID.ToString(), request, null, "CategoryServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "CategoryServiceRepository.Save()");
            }
            return DataResponse;
        }
        public DataResponse Update(Category request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.Category.FirstOrDefault(r => r.CategoryID != request.CategoryID && r.CategoryName == request.CategoryName);
                if (isFound != null)
                {
                    DataResponse.Error = false;
                    DataResponse.Success = true;
                    DataResponse.ID = isFound.CategoryID;
                    DataResponse.Message = Constants.DataAlreadyExist;
                    return DataResponse;
                }
                var dataToUpdate = _dbcontext.Category.FirstOrDefault(r => r.CategoryID == request.CategoryID);
                if (dataToUpdate == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                _auditLogger.SaveActionLog("Category", ActionType.Update, request.CategoryID.ToString(), request, dataToUpdate, "CategoryServiceRepository.Update()");
                dataToUpdate.CategoryCode = request.CategoryCode;
                dataToUpdate.CategoryName = request.CategoryName;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();

                DataResponse.ID = dataToUpdate.CategoryID;
                DataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "CategoryServiceRepository.Update()");
            }
            return DataResponse;
        }
        public DataResponse Delete(int CategoryID, int UserID, long LoginAuditID)
        {
            DataResponse DataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.Category.FirstOrDefault(w => w.CategoryID == CategoryID);
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

                DataResponse.ID = dataexists.CategoryID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Category", ActionType.Delete, CategoryID.ToString(), null, null, "CategoryServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, CategoryID, "CategoryServiceRepository.Delete()");
            }
            return DataResponse;
        }
        public DataResponse GetCategoryDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VCategory> query = _dbcontext.VCategory;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.CategoryName.Contains(request.SearchValue) || d.CategoryCode.Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VCategory.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.CategoryID,
                                           w.CategoryName,
                                           w.CategoryCode,
                                           w.IsActive
                                       }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("Category", ActionType.ListData, null, request, null, "CategoryServiceRepository.GetDesignationDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "CategoryServiceRepository.GetCategoryDataTable()");
            }
            return response;
        }
    }
}