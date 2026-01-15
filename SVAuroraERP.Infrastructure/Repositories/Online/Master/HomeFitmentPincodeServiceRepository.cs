namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class HomeFitmentPincodeServiceRepository : IHomeFitmentPincodeServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<HomeFitmentPincodeServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public HomeFitmentPincodeServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<HomeFitmentPincodeServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                      IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetState()
        {
            DataResponse response = new DataResponse();
            try 
            {
                var district = _dbcontext.VState.ToList();

                response.Count = district.Count;
                response.Value = district;
                _auditLogger.SaveActionLog("State", ActionType.ListData, null, null,null, "HomeFitmentPincodeServiceRepository.GetState()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "HomeFitmentPincodeServiceRepository.GetState()");
            }
            return response;
        }
        public DataResponse GetDistrictByStateID(int StateID)
        {

            DataResponse response = new DataResponse();
            try 
            {
                var district = _dbcontext.VDistrict.Where(w => w.StateID == StateID).ToList();

                response.Count = district.Count;
                response.Value = district;
                response.ID = StateID;
                _auditLogger.SaveActionLog("District", ActionType.Select, StateID.ToString(), StateID, null,"HomeFitmentPincodeServiceRepository.GetDistrictByStateID()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, StateID, "HomeFitmentPincodeServiceRepository.GetDistrictByStateID()");
            }          
            return response;
        
        }
        public DataResponse GetHomeFitmentPincode()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHomeFitmentPincode.OrderBy(o => o.DistrictName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("HomeFitmentPincode", ActionType.ListData,null, null, null, "HomeFitmentPincodeServiceRepository.GetHomeFitmentPincode()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "HomeFitmentPincodeServiceRepository.GetHomeFitmentPincode()");
            }

            return dataResponse;
        }
        public DataResponse GetHomeFitmentPincodeByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHomeFitmentPincode.FirstOrDefault(w => w.HomeFitmentPincodeID == ID);
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
                _auditLogger.SaveActionLog("HomeFitmentPincode", ActionType.Select, ID.ToString(),ID, null, "BoxServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "HomeFitmentPincodeServiceRepository.GetHomeFitmentPincodeByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(HomeFitmentPincode HomeFitmentPincode)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.HomeFitmentPincode.FirstOrDefault(r => r.Pincode == HomeFitmentPincode.Pincode);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.HomeFitmentPincodeID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                HomeFitmentPincode.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.HomeFitmentPincode.Add(HomeFitmentPincode);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("HomeFitmentPincode", ActionType.Insert, HomeFitmentPincode.HomeFitmentPincodeID.ToString(), HomeFitmentPincode,null, "HomeFitmentPincodeServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HomeFitmentPincode, "HomeFitmentPincodeServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(HomeFitmentPincode HomeFitmentPincode)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.HomeFitmentPincode.FirstOrDefault(r => r.HomeFitmentPincodeID != HomeFitmentPincode.HomeFitmentPincodeID && r.Pincode == HomeFitmentPincode.Pincode);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.HomeFitmentPincodeID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.HomeFitmentPincode.FirstOrDefault(r => r.HomeFitmentPincodeID == HomeFitmentPincode.HomeFitmentPincodeID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("HomeFitmentPincode", ActionType.Update, dataexists.HomeFitmentPincodeID.ToString(), HomeFitmentPincode, dataexists, "HomeFitmentPincodeServiceRepository.Update()");
                dataexists.DistrictID = HomeFitmentPincode.DistrictID;
                dataexists.Location = HomeFitmentPincode.Location;
                dataexists.Pincode = HomeFitmentPincode.Pincode;
                dataexists.IsActive = HomeFitmentPincode.IsActive;
                dataexists.LastUpdatedBy = HomeFitmentPincode.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.HomeFitmentPincodeID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HomeFitmentPincode, "HomeFitmentPincodeServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int HomeFitmentPincodeID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.HomeFitmentPincode.FirstOrDefault(w => w.HomeFitmentPincodeID == HomeFitmentPincodeID);
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

                dataResponse.ID = dataexists.DistrictID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("HomeFitmentPincode", ActionType.Delete, null, HomeFitmentPincodeID,null, "HomeFitmentPincodeServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, HomeFitmentPincodeID, "HomeFitmentPincodeServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetHomeFitmentPincodeDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHomeFitmentPincode> query = _dbcontext.VHomeFitmentPincode;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.DistrictName ?? "").Contains(request.SearchValue)
                    || (d.StateName ?? "").Contains(request.SearchValue)
                    || (d.Location ?? "").Contains(request.SearchValue) || (d.Location ?? "").Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VHomeFitmentPincode.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.HomeFitmentPincodeID,
                                w.DistrictName,
                                w.StateName,
                                w.Location,
                                w.Pincode,
                                w.IsActive
                            }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("District", ActionType.Select, null, request, null, "DistrictServiceRepository.GetDistrictList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "DistrictServiceRepository.GetDistrictList()");
            }
            return response;
        }
    }
}