namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Dispatch
{
    public class MapPlateColorServiceRepository : IMapPlateColorServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public MapPlateColorServiceRepository(SVAuroraERPDbContext dbcontext,
                                     IAuditLogger auditLogger,
                                     IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetMapPlateColor()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VMapPlateColor.OrderBy(o => o.ColorID).ToList();
                DataResponse.Count = resultdata.Count;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("MapPlateColor", ActionType.ListData, null, resultdata, null,"MapPlateColorServiceRepository.GetMapPlateColor()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "MapPlateColorServiceRepository.GetMapPlateColor()");
            }
            return DataResponse;
        }
        public DataResponse GetByID(int MapInventoryandHSRPColorID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VMapPlateColor.FirstOrDefault(w => w.MapInventoryandHSRPColorID == MapInventoryandHSRPColorID);
                if (resultdata == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                DataResponse.ID = MapInventoryandHSRPColorID;
                DataResponse.Message = Constants.RecordFound;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("MapPlateColor", ActionType.Select, MapInventoryandHSRPColorID.ToString(), resultdata, null, "MapPlateColorServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, MapInventoryandHSRPColorID, "MapPlateColorServiceRepository.GetByID()");
            }
            return DataResponse;
        }
        public DataResponse Save(MapPlateColor request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Check if either ColorID or HSRPPlateColorID already exists
                var existing = _dbcontext.VMapPlateColor.FirstOrDefault(r =>
                    r.ColorID == request.ColorID && r.HSRPPlateColorID == request.HSRPPlateColorID
                );

                if (existing != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = existing.MapInventoryandHSRPColorID;
                    dataResponse.Message = "Color or Plate Color already exists in another record.";
                    return dataResponse;
                }

                // Insert new record
                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.MapPlateColor.Add(request);
                _dbcontext.SaveChanges();

                dataResponse.Error = false;
                dataResponse.Success = true;
                dataResponse.ID = request.MapInventoryandHSRPColorID;
                dataResponse.Message = Constants.SuccessMessage;

                _auditLogger.SaveActionLog("MapPlateColor", ActionType.Insert, request.MapInventoryandHSRPColorID.ToString(), request, null, "MapPlateColorServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "MapPlateColorServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(MapPlateColor request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.VMapPlateColor.FirstOrDefault(r =>
                    r.MapInventoryandHSRPColorID != request.MapInventoryandHSRPColorID &&
                    (r.ColorID == request.ColorID && r.HSRPPlateColorID == request.HSRPPlateColorID)
                );

                if (isFound != null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = isFound.MapInventoryandHSRPColorID;
                    DataResponse.Message = "Color or Plate Color already exists in another record.";
                    return DataResponse;
                }

                // Get the current record to update
                var dataToUpdate = _dbcontext.MapPlateColor.FirstOrDefault(r =>
                    r.MapInventoryandHSRPColorID == request.MapInventoryandHSRPColorID
                );

                if (dataToUpdate == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;
                    return DataResponse;
                }

                _auditLogger.SaveActionLog("MapPlateColor", ActionType.Update, request.MapInventoryandHSRPColorID.ToString(), request, dataToUpdate, "MapPlateColorServiceRepository.Update()");

                dataToUpdate.ColorID = request.ColorID;
                dataToUpdate.HSRPPlateColorID = request.HSRPPlateColorID;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;

                _dbcontext.SaveChanges();

                DataResponse.ID = dataToUpdate.MapInventoryandHSRPColorID;
                DataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "MapPlateColorServiceRepository.Update()");
            }

            return DataResponse;
        }
        public DataResponse Delete(int MapInventoryandHSRPColorID, int UserID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.MapPlateColor.FirstOrDefault(w => w.MapInventoryandHSRPColorID == MapInventoryandHSRPColorID);
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

                DataResponse.ID = dataexists.MapInventoryandHSRPColorID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("MapPlateColor", ActionType.Delete, MapInventoryandHSRPColorID.ToString(), null, dataexists, "MapPlateColorServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, MapInventoryandHSRPColorID, "MapPlateColorServiceRepository.Delete()");
            }

            return DataResponse;
        }
        public DataResponse GetMapPlateDataTable(DataTableRequest request)
        {

            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VMapPlateColor> query = _dbcontext.VMapPlateColor;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.ColorName.Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VMapPlateColor.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.MapInventoryandHSRPColorID,                                    
                                           w.ColorName,
                                           w.VehiclePlateColorName,
                                           w.IsActive
                                       }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("MapPlateColor", ActionType.ListData, null, request, null, "MapPlateColorServiceRepository.GetMapPlateDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "MapPlateColorServiceRepository.GetMapPlateDataTable()");
            }
            return response;
        }

    }
}
