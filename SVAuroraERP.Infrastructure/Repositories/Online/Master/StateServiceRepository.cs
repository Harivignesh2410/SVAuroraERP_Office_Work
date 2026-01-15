namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class StateServiceRepository : IStateServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<StateServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public StateServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<StateServiceRepository> logger,
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
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VState.OrderBy(o => o.StateName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VState", ActionType.ListData, null, null,null, "StateServiceRepository.GetState()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "StateServiceRepository.GetState()");
            }

            return dataResponse;
        }
        public DataResponse GetStateByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VState.FirstOrDefault(w => w.StateID == ID);
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
                _auditLogger.SaveActionLog("VState", ActionType.Select, ID.ToString(), ID, null, "StateServiceRepository.GetStateByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "StateServiceRepository.GetStateByID()");
            }
            return dataResponse;
        }
        public DataResponse Save(State State)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.VState.FirstOrDefault(r => r.StateName == State.StateName);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.StateID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                State.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.State.Add(State);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("Box", ActionType.Insert, State.StateID.ToString(), State,null, "StateServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, State, "StateServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(State State)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.State.FirstOrDefault(r => r.StateID != State.StateID && r.StateName == State.StateName);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.StateID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.State.FirstOrDefault(r => r.StateID == State.StateID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("Box", ActionType.Update, dataexists.StateID.ToString(), State, dataexists, "StateServiceRepository.Update()");
                dataexists.StateName = State.StateName;
                dataexists.StateCode = State.StateCode;
                dataexists.IsActive = State.IsActive;
                dataexists.LastUpdatedBy = State.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.StateID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, State, "StateServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int StateID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.State.FirstOrDefault(w => w.StateID == StateID);
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

                dataResponse.ID = dataexists.StateID;
                dataResponse.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("State", ActionType.Delete, null, StateID,null, "StateServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, StateID, "StateServiceRepository.Delete()");
            }
            return dataResponse;
        }
        public DataResponse GetStateDataTableList(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VState> query = _dbcontext.VState;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => (d.StateName ?? "").Contains(request.SearchValue) || (d.StateCode ?? "").Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VState.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                            .Select(w => new
                            {
                                w.StateID,
                                w.StateCode,
                                w.StateName,
                                w.IsActive
                            }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("State", ActionType.Select, null, request, null, "StateServiceRepository.GetStateDataTableList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "StateServiceRepository.GetStateDataTableList()");
            }
            return response;
        }
    }
}