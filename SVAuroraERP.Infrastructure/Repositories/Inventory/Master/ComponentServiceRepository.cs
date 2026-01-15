namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class ComponentServiceRepository : IComponentServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public ComponentServiceRepository(SVAuroraERPDbContext dbcontext,
                                     IAuditLogger auditLogger,
                                     IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetComponentList()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var ComponenetType = _dbcontext.VComponentType.OrderBy(o => o.ComponentTypeCode).ToList();

                DataResponse.Count = ComponenetType.Count;
                DataResponse.Message = Constants.RecordFound;
                DataResponse.Value = ComponenetType;
                _auditLogger.SaveActionLog("ComponentType", ActionType.ListData, null, null, null, "ComponentServiceRepository.GetComponentList()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "ComponentServiceRepository.GetComponentList()");
            }

            return DataResponse;
        }

        public DataResponse GetComponentByID(int ComponentTypeID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var Componenet = _dbcontext.VComponentType.FirstOrDefault(ws => ws.ComponentTypeID == ComponentTypeID);

                if (Componenet == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;
                }
                else
                {
                    var ComponenetType = _dbcontext.VComponentType.FirstOrDefault(w => w.ComponentTypeID == ComponentTypeID);

                    DataResponse.ID = ComponentTypeID;
                    DataResponse.Message = Constants.RecordFound;
                    DataResponse.Value = ComponenetType;
                }
                _auditLogger.SaveActionLog("ComponentType", ActionType.Select, ComponentTypeID.ToString(), ComponentTypeID, null, "ComponentServiceRepository.GetComponentByID()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, ComponentTypeID, "ComponentServiceRepository.GetComponentByID()");
            }

            return DataResponse;
        }
        public DataResponse Save(ComponentType request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var exisitingComponent = _dbcontext.ComponentType.FirstOrDefault(w => w.ComponentTypeName == request.ComponentTypeName);

                if (exisitingComponent != null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = exisitingComponent.ComponentTypeID;
                    DataResponse.Message = Constants.DataAlreadyExist;

                    return DataResponse;
                }
                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.ComponentType.Add(request);
                _dbcontext.SaveChanges();

                DataResponse.ID = 0;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("ComponentType", ActionType.Insert, request.ComponentTypeID.ToString(), request, null, "ComponentServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "ComponentServiceRepository.Save()");
            }

            return DataResponse;
        }
        public DataResponse Update(ComponentType request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {

                var isdataExists = _dbcontext.ComponentType.FirstOrDefault(w => w.ComponentTypeName == request.ComponentTypeName && w.ComponentTypeID != request.ComponentTypeID);

                if (isdataExists != null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = isdataExists.ComponentTypeID;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }

                var dataToUpdate = _dbcontext.ComponentType.FirstOrDefault(w => w.ComponentTypeID == request.ComponentTypeID);

                if (dataToUpdate == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                _auditLogger.SaveActionLog("ComponentType", ActionType.Update, request.ComponentTypeID.ToString(), request, dataToUpdate, "DesignationServiceRepository.Update()");
                dataToUpdate.ComponentTypeName = request.ComponentTypeName;
                dataToUpdate.ComponentTypeCode = request.ComponentTypeCode;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;

                _dbcontext.SaveChanges();

                DataResponse.ID = dataToUpdate.ComponentTypeID;
                DataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "ComponentServiceRepository.Update()");
            }

            return DataResponse;
        }
        public DataResponse Delete(int ComponentTypeID, int UserID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var ItemExists = _dbcontext.ComponentType.FirstOrDefault(w => w.ComponentTypeID == ComponentTypeID);

                if (ItemExists == null)
                {
                    DataResponse.Error = false;
                    DataResponse.Success = true;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }

                ItemExists.IsDeleted = true;
                ItemExists.LastUpdatedBy = UserID;

                _dbcontext.SaveChanges();

                DataResponse.ID = ItemExists.ComponentTypeID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("ComponentType", ActionType.Delete, ComponentTypeID.ToString(), null, ItemExists, "ComponentServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, ComponentTypeID, "ComponentServiceRepository.Delete()");
            }

            return DataResponse;
        }
        public DataResponse GetComponentDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VComponentType> query = _dbcontext.VComponentType;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.ComponentTypeCode.Contains(request.SearchValue) || d.ComponentTypeName.Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VComponentType.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.ComponentTypeID,
                                           w.ComponentTypeName,
                                           w.ComponentTypeCode,
                                           w.IsActive
                                       }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("ComponentType", ActionType.ListData, null, request, null, "ComponentServiceRepository.GetComponentDataTable()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ComponentServiceRepository.GetComponentDataTable()");
            }
            return response;
        }
    }
}
