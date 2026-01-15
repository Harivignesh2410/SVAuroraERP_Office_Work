namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Dispatch
{
    public class MapPlateSizeServiceRepository : IMapPlateSizeServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public MapPlateSizeServiceRepository(SVAuroraERPDbContext dbcontext,
                                     IAuditLogger auditLogger,
                                     IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetMapPlateSize()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VMapPlateSize.OrderBy(o => o.SizeID).ToList();
                DataResponse.Count = resultdata.Count;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("MapPlateSize", ActionType.ListData, null, resultdata, null, "MapPlateSizeServiceRepository.GetMapPlateSize()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "MapPlateSizeServiceRepository.GetMapPlateSize()");
            }
            return DataResponse;
        }
        public DataResponse GetByID(int MapInventoryandHSRPSizeID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VMapPlateSize.FirstOrDefault(w => w.MapInventoryandHSRPSizeID == MapInventoryandHSRPSizeID);
                if (resultdata == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                DataResponse.ID = MapInventoryandHSRPSizeID;
                DataResponse.Message = Constants.RecordFound;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("MapPlateSize", ActionType.Select, MapInventoryandHSRPSizeID.ToString(), resultdata, null, "MapPlateSizeServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, MapInventoryandHSRPSizeID, "MapPlateSizeServiceRepository.GetByID()");
            }
            return DataResponse;
        }
        public DataResponse Save(MapPlateSize request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var existing = _dbcontext.VMapPlateSize.FirstOrDefault(r => r.SizeID == request.SizeID && r.HSRPPlateSizeID == request.HSRPPlateSizeID);

                if (existing != null)
                {
                    // Duplicate record found
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = existing.MapInventoryandHSRPSizeID;
                    dataResponse.Message = "Size or Plate Size already exists in another record.";
                    return dataResponse;
                }

                // Insert new record
                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.MapPlateSize.Add(request);
                _dbcontext.SaveChanges();

                dataResponse.Error = false;
                dataResponse.Success = true;
                dataResponse.ID = request.MapInventoryandHSRPSizeID;
                dataResponse.Message = Constants.SuccessMessage;

                _auditLogger.SaveActionLog("MapPlateSize", ActionType.Insert, request.MapInventoryandHSRPSizeID.ToString(), request, null, "MapPlateSizeServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "MapPlateSizeServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(MapPlateSize request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.VMapPlateSize.FirstOrDefault(r =>
                  r.MapInventoryandHSRPSizeID != request.MapInventoryandHSRPSizeID &&
                  (r.SizeID == request.SizeID && r.HSRPPlateSizeID == request.HSRPPlateSizeID)
              );

                if (isFound != null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = isFound.MapInventoryandHSRPSizeID;
                    DataResponse.Message = "Size or Plate Size already exists in another record.";
                    return DataResponse;
                }

                var dataToUpdate = _dbcontext.MapPlateSize.FirstOrDefault(r =>
                   r.MapInventoryandHSRPSizeID == request.MapInventoryandHSRPSizeID
               );

                if (dataToUpdate == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.ID = 0;
                    DataResponse.Message = Constants.NoRecordFound;

                    return DataResponse;
                }
                _auditLogger.SaveActionLog("MapPlateSize", ActionType.Update, request.MapInventoryandHSRPSizeID.ToString(), request, dataToUpdate, "MapPlateSizeServiceRepository.Update()");
                dataToUpdate.SizeID = request.SizeID;
                dataToUpdate.HSRPPlateSizeID = request.HSRPPlateSizeID;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();

                DataResponse.ID = dataToUpdate.MapInventoryandHSRPSizeID;
                DataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "MapPlateSizeServiceRepository.Update()");
            }

            return DataResponse;
        }
        public DataResponse Delete(int MapInventoryandHSRPSizeID, int UserID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.MapPlateSize.FirstOrDefault(w => w.MapInventoryandHSRPSizeID == MapInventoryandHSRPSizeID);
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

                DataResponse.ID = dataexists.MapInventoryandHSRPSizeID;
                DataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("MapPlateSize", ActionType.Delete, MapInventoryandHSRPSizeID.ToString(), null, dataexists, "MapPlateSizeServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, MapInventoryandHSRPSizeID, "MapPlateSizeServiceRepository.Delete()");
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

                IQueryable<VMapPlateSize> query = _dbcontext.VMapPlateSize;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.SizeName.Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VMapPlateSize.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.MapInventoryandHSRPSizeID,
                                           w.SizeName,
                                           w.VehiclePlateSizeName,
                                           w.IsActive
                                       }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("MapPlateSize", ActionType.ListData, null, request, null, "MapPlateSizeServiceRepository.GetMapPlateDataTable()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "MapPlateSizeServiceRepository.GetMapPlateDataTable()");
            }
            return response;
        }
    }
}
