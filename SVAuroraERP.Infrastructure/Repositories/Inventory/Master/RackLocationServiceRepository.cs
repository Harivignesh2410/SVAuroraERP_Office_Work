namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class RackLocationServiceRepository : IRackLocationServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public RackLocationServiceRepository(SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger,
                                          IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;          
        }
        public DataResponse GetRackLocation()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VRackLocation.OrderBy(o => o.RackLocationCode).ToList();
                DataResponse.Count = resultdata.Count;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("RackLocation", ActionType.ListData, null, null, null, "RackLocationServiceRepository.GetRackLocation()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "RackLocationServiceRepository.GetRackLocation()");
            }

            return DataResponse;
        }
        public DataResponse GetByID(int RackLocationID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var RackLocation = _dbcontext.VRackLocation.FirstOrDefault(w => w.RackLocationID == RackLocationID);
                DataResponse = GetCapacityByLocaitonID(RackLocationID);
                RackLocation.RackLocationSizeCapacity = ((List<RackLocationSizeCapacity>)DataResponse.Value).ToList();
                if (RackLocation == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }

                DataResponse.ID = RackLocationID;
                DataResponse.Message = Constants.RecordFound;
                DataResponse.Value = RackLocation;
                _auditLogger.SaveActionLog("RackLocation", ActionType.Select, RackLocationID.ToString(), RackLocationID, null, "RackLocationServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, RackLocationID, "RackLocationServiceRepository.GetByID()");
            }

            return DataResponse;
        }
        public DataResponse Save(RackLocation request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.RackLocation.FirstOrDefault(r => r.WareHouseID == request.WareHouseID && r.RackLocationName == request.RackLocationName);
                if (dataexists != null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = dataexists.RackLocationID;
                    DataResponse.Message = Constants.DataAlreadyExist;
                    return DataResponse;
                }

                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.RackLocation.Add(request);
                _dbcontext.SaveChanges();
                DataResponse.ID = request.RackLocationID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("RackLocation", ActionType.Insert, dataexists.RackLocationID.ToString(), request, null, "RackLocationServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "RackLocationServiceRepository.Save()");
            }

            return DataResponse;
        }
        public DataResponse Update(RackLocation request)
        {
            DataResponse DataResponse = new DataResponse();

            try
            {
                var isFound = _dbcontext.RackLocation.FirstOrDefault(r => r.RackLocationID != request.RackLocationID && r.RackLocationName == request.RackLocationName && r.WareHouseID == request.WareHouseID);
                if (isFound != null)
                {
                    DataResponse.Error = false;
                    DataResponse.Success = true;
                    DataResponse.ID = isFound.RackLocationID;
                    DataResponse.Message = Constants.DataAlreadyExist;
                    return DataResponse;
                }
                var dataToUpdate = _dbcontext.RackLocation.FirstOrDefault(r => r.RackLocationID == request.RackLocationID);
                if (dataToUpdate == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                _auditLogger.SaveActionLog("RackLocation", ActionType.Update, request.RackLocationID.ToString(), request, dataToUpdate, "RackLocationServiceRepository.Update()");
                dataToUpdate.WareHouseID = request.WareHouseID;
                dataToUpdate.RackLocationCode = request.RackLocationCode;
                dataToUpdate.RackLocationName = request.RackLocationName;
                dataToUpdate.ComponentTypeID = request.ComponentTypeID;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                DataResponse.ID = dataToUpdate.RackLocationID;
                DataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "RackLocationServiceRepository.Update()");
            }

            return DataResponse;
        }
        public DataResponse Delete(int RackLocationID, int UserID, long LoginAuditID)
        {
            DataResponse DataResponse = new DataResponse();

            try
            {
                var dataToDelete = _dbcontext.RackLocation.FirstOrDefault(w => w.RackLocationID == RackLocationID);
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

                DeletCapacity(RackLocationID, UserID);
                DataResponse.ID = dataToDelete.RackLocationID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("RackLocation", ActionType.Delete, RackLocationID.ToString(), RackLocationID, null, "RackLocationServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, RackLocationID, "RackLocationServiceRepository.Delete()");
            }

            return DataResponse;
        }
        public DataResponse GetCapacityByLocaitonID(int RackLocationID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.RackLocationSizeCapacity.Where(w => w.RackLocationID == RackLocationID).ToList();
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
                    DataResponse.ID = RackLocationID;
                    DataResponse.Message = Constants.RecordFound;
                    DataResponse.Value = resultdata;
                }
                _auditLogger.SaveActionLog("RackLocationSizeCapacity", ActionType.Select, RackLocationID.ToString(), RackLocationID, null, "RackLocationServiceRepository.GetCapacityByLocaitonID()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, RackLocationID, "RackLocationServiceRepository.GetCapacityByLocaitonID()");
            }

            return DataResponse;
        }
        public DataResponse SaveCapacity(List<RackLocationSizeCapacity> RackCapacity)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                int SuccessCount = 0;
                if (RackCapacity == null || RackCapacity.Count == 0)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    return DataResponse;
                }

                foreach (var rackcapacity in RackCapacity)
                {
                    if (rackcapacity.StatusFlag == "I") //Insert
                    {
                        if (AddCapacity(rackcapacity)) SuccessCount++;
                    }
                    else if (rackcapacity.StatusFlag == "U") //Update
                    {
                        if (UpdateCapacity(rackcapacity)) SuccessCount++;
                    }
                    else if (rackcapacity.StatusFlag == "D") //Delete
                    {
                        if (DeletCapacity(rackcapacity.RackLocationSizeCapacityID, rackcapacity.LastUpdatedBy)) SuccessCount++;
                    }
                }

                if (SuccessCount > 0)
                {
                    DataResponse.Message = Constants.SuccessMessage;
                }
                _auditLogger.SaveActionLog("RackLocationSizeCapacity", ActionType.Insert, null, RackCapacity, null, "RackLocationServiceRepository.SaveCapacity()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, RackCapacity, "RackLocationServiceRepository.SaveCapacity()");
            }

            return DataResponse;
        }
        public bool AddCapacity(RackLocationSizeCapacity request)
        {
            try
            {
                bool IsSuccess = false;

                var CheckIfDataExists = _dbcontext.RackLocationSizeCapacity.FirstOrDefault(w => w.RackLocationID == request.RackLocationID && w.SizeID == request.SizeID);

                if (CheckIfDataExists == null)
                {
                    request.LastUpdatedDate = DateTime.UtcNow;
                    _dbcontext.RackLocationSizeCapacity.Add(request);
                    _dbcontext.SaveChanges();
                    IsSuccess = true;
                }
                else if (CheckIfDataExists != null)
                {
                    CheckIfDataExists.RackLocationID = request.RackLocationID;
                    CheckIfDataExists.Capacity = request.Capacity;
                    CheckIfDataExists.SizeID = request.SizeID;
                    CheckIfDataExists.LastUpdatedBy = request.LastUpdatedBy;
                    CheckIfDataExists.LastUpdatedDate = DateTime.UtcNow;
                    _dbcontext.SaveChanges();
                    IsSuccess = true;
                }
                _auditLogger.SaveActionLog("RackLocationSizeCapacity", ActionType.Insert, null, request, null, "RackLocationServiceRepository.SaveCapacity()");
                return IsSuccess;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool UpdateCapacity(RackLocationSizeCapacity request)
        {
            bool IsSuccess = false;
            var dataToUpdate = _dbcontext.RackLocationSizeCapacity.FirstOrDefault(w => w.RackLocationSizeCapacityID != request.RackLocationSizeCapacityID && w.RackLocationID == request.RackLocationID && w.SizeID == request.SizeID);
            if (dataToUpdate != null)
            {

                dataToUpdate.RackLocationID = request.RackLocationID;
                dataToUpdate.Capacity = request.Capacity;
                dataToUpdate.SizeID = request.SizeID;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                IsSuccess = true;
            }
            _auditLogger.SaveActionLog("RackLocationSizeCapacity", ActionType.Update, request.RackLocationSizeCapacityID.ToString(), request, dataToUpdate, "RackLocationServiceRepository.UpdateCapacity()");
            return IsSuccess;
        }

        public bool DeletCapacity(int RackLocationID, int LastUpdatedBy)
        {
            bool IsSuccess = false;

            var entity = _dbcontext.RackLocationSizeCapacity.Where(w => w.RackLocationID == RackLocationID).ToList();

            if (entity != null)
            {
                foreach (var rackcapacity in entity)
                {
                    rackcapacity.IsDeleted = true;
                    rackcapacity.LastUpdatedBy = LastUpdatedBy;
                }
                _dbcontext.SaveChanges();
                IsSuccess = true;
            }
            _auditLogger.SaveActionLog("RackLocationSizeCapacity", ActionType.Delete, RackLocationID.ToString(), null, null, "RackLocationServiceRepository.DeletCapacity()");
            return IsSuccess;
        }
        public DataResponse GetRackLocationByComponentID(int ComponentID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VRackLocation.Where(o => o.ComponentTypeID == ComponentID).ToList();
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
                    DataResponse.ID = ComponentID;
                    DataResponse.Message = Constants.RecordFound;
                    DataResponse.Value = resultdata;
                }
                _auditLogger.SaveActionLog("RackLocation", ActionType.Select, ComponentID.ToString(), ComponentID, null, "RackLocationServiceRepository.GetRackLocationByComponentID()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, ComponentID, "RackLocationServiceRepository.GetRackLocationByComponentID()");
            }

            return DataResponse;
        }
        public DataResponse GetRackLocationByWareHouseID(int WareHouseID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VRackLocation.Where(w => w.WareHouseID == WareHouseID).ToList();
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
                    DataResponse.ID = WareHouseID;
                    DataResponse.Message = Constants.RecordFound;
                    DataResponse.Value = resultdata;
                }
                _auditLogger.SaveActionLog("RackLocation", ActionType.Select, WareHouseID.ToString(), WareHouseID, null, "RackLocationServiceRepository.GetRackLocationByWareHouseID()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, WareHouseID, "RackLocationServiceRepository.GetRackLocationByWareHouseID()");
            }

            return DataResponse;
        }
        public DataResponse GetRackLocationDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VRackLocation> query = _dbcontext.VRackLocation;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.RackLocationCode.Contains(request.SearchValue) || d.RackLocationName.Contains(request.SearchValue) ||
                                              d.WareHouseName.Contains(request.SearchValue) || d.ComponentTypeName.Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VRackLocation.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.RackLocationID,
                                           w.RackLocationCode,
                                           w.RackLocationName,
                                           w.WareHouseName,
                                           w.ComponentTypeName,
                                           w.IsActive
                                       }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("RackLocation", ActionType.ListData, null, request, null, "RackLocationServiceRepository.GetRackLocationDataTable()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "RackLocationServiceRepository.GetRackLocationDataTable()");
            }

            return response;
        }
    }
}