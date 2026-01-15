namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class ItemServiceRepository : IItemServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public ItemServiceRepository(SVAuroraERPDbContext dbcontext, 
                                            IAuditLogger auditLogger,
                                            IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetItem()
        {
            DataResponse response = new DataResponse();
            try
            {
                var items = _dbcontext.VItem.OrderBy(o => o.ItemCode).AsNoTracking().ToList();
                response.Count = items.Count;
                response.Value = items;
                _auditLogger.SaveActionLog("item", ActionType.ListData, null, null, null,"ItemServiceRepository.GetItem()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "ItemServiceRepository.GetItem()");
            }

            return response;
        }
        public DataResponse GetByID(int ItemID)
        {
            DataResponse response = new DataResponse();
            try
            {
                var item = _dbcontext.VItem.FirstOrDefault(w => w.ItemID == ItemID);
                if (item == null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.ID = 0;
                    response.Message = Constants.NoRecordFound;
                }
                else
                {
                    response.ID = ItemID;
                    response.Message = Constants.RecordFound;
                    response.Value = item;
                }
                _auditLogger.SaveActionLog("Item", ActionType.Select, ItemID.ToString(), ItemID, null, "ItemServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, ItemID, "ItemServiceRepository.GetByID()");
            }

            return response;
        }
        public DataResponse Save(Item request)
        {
            DataResponse response = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Item.FirstOrDefault(r => r.ItemCode == request.ItemCode);
                if (dataexists != null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.ID = dataexists.ItemID;
                    response.Message = Constants.DataAlreadyExist;
                    return response;
                }
                _dbcontext.Item.Add(request);
                _dbcontext.SaveChanges();
                response.ID = request.ItemID;
                response.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Item", ActionType.Insert, request.ItemID.ToString(), request, null, "ItemServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ItemServiceRepository.Save()");
            }

            return response;
        }
        public DataResponse Update(Item request)
        {
            DataResponse response = new DataResponse();
            try
            {   
                var isFound = _dbcontext.Item.FirstOrDefault(r => r.ItemID != request.ItemID && r.ItemName == request.ItemName && r.ItemCode == request.ItemCode);
                if (isFound != null)
                {
                    response.Error = false;
                    response.Success = true;
                    response.ID = isFound.ItemID;
                    response.Message = Constants.DataAlreadyExist;
                    return response;
                }
                var dataToUpdate = _dbcontext.Item.FirstOrDefault(r => r.ItemID == request.ItemID);
                if (dataToUpdate == null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.ID = 0;
                    response.Message = Constants.NoRecordFound;
                    return response;
                }
                _auditLogger.SaveActionLog("Item", ActionType.Update, request.ItemID.ToString(), request, dataToUpdate, "ItemServiceRepository.Update()");
                dataToUpdate.ItemCode = request.ItemCode;
                dataToUpdate.ItemName = request.ItemName;
                dataToUpdate.HSNCode = request.HSNCode;
                dataToUpdate.IsStockRequired = request.IsStockRequired;
                dataToUpdate.UnitID = request.UnitID;
                dataToUpdate.Description = request.Description;
                dataToUpdate.Price = request.Price;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;

                // Added on 2025/06/17 by Harivignesh
                dataToUpdate.ComponentTypeID = request.ComponentTypeID;
                dataToUpdate.ColorID = request.ColorID;
                dataToUpdate.SizeID = request.SizeID;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.ItemCategoryID = request.ItemCategoryID;

                _dbcontext.SaveChanges();
                response.ID = dataToUpdate.ItemID;
                response.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ItemServiceRepository.Update()");
            }

            return response;
        }
        public DataResponse Delete(int ItemID, int UserID, long LoginAuditID)
        {
            DataResponse response = new DataResponse();
            try
            {
                var DataToDelete = _dbcontext.Item.FirstOrDefault(i => i.ItemID == ItemID);
                if (DataToDelete == null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.Message = Constants.NoRecordFound;
                    return response;
                }
                DataToDelete.IsDeleted = true;
                DataToDelete.LastUpdatedBy = UserID;
                DataToDelete.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                response.Message = Constants.SuccessMessage;
                response.ID = ItemID;
                _auditLogger.SaveActionLog("Item", ActionType.Delete, ItemID.ToString(), null, DataToDelete, "ItemServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, ItemID, "ItemServiceRepository.Delete()");
            }

            return response;
        }
        //Added on 2025.01.05 by Sivakumar
        public DataResponse GetItemCategory()
        {
            DataResponse response = new DataResponse();
            try
            {
                var items = _dbcontext.LkupItemCategory.ToList();
                response.Count = items.Count;
                response.Value = items;
                _auditLogger.SaveActionLog("LkupItemCategory", ActionType.ListData, null, items, null, "ItemServiceRepository.GetItemCategory()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "ItemServiceRepository.GetItemCategory()");
            }

            return response;
        }
        public DataResponse GetItemByFilter(BatchStockFilter request)
        {
            DataResponse response = new DataResponse();
            try
            {
                var query = _dbcontext.VItem.AsQueryable();
                if (request.ComponentTypeID > 0) query = query.Where(w => w.ComponentTypeID == request.ComponentTypeID);
                if (request.SizeID > 0) query = query.Where(w => w.SizeID == request.SizeID);
                if (request.ColorID > 0) query = query.Where(w => w.ColorID == request.ColorID);
                //return query.ToList();
                response.Value = query.ToList();
                _auditLogger.SaveActionLog("Item", ActionType.ListData, null, request, null, "ItemServiceRepository.GetItemByFilter()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ItemServiceRepository.GetItemByFilter()");
            }

            return response;
        }
        public DataResponse GetItemDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VItem> query = _dbcontext.VItem;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.ItemCategoryName.Contains(request.SearchValue) || d.ItemCode.Contains(request.SearchValue)
                                                || d.ItemName.Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VItem.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.ItemID,
                                           w.ItemCategoryID,
                                           w.HSNCode,
                                           w.Price,
                                           w.UnitName,
                                           w.ColorName,
                                           w.SizeName,
                                           w.ComponentTypeName,
                                           w.IsStockRequired,
                                           w.ItemCategoryName,
                                           w.ItemCode,
                                           w.ItemName,
                                           w.IsActive
                                       }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("item", ActionType.ListData, null, request,null, "ItemServiceRepository.GetItemDataTable()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ItemServiceRepository.GetItemDataTable()");
            }

            return response;
        }
    }
}