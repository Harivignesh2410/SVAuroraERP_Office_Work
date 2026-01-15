namespace SVAuroraERP.Infrastructure.Repositories.Master
{
    public class MachineServiceRepository : IMachineServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;

        public MachineServiceRepository(SVAuroraERPDbContext dbcontext,
                                         IAuditLogger auditLogger,
                                          IErrorLoggerService errorLoggerService,
                                         ITransLogRespository transLogRespository)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
            _transLogRespository = transLogRespository;
        }

        public DataResponse GetMachineList()
        {
            DataResponse response = new DataResponse();
            try
            {
                var machines = _dbcontext.VMachine.OrderBy(o => o.MachineCode).ToList();
                response.Count = machines.Count;
                response.Value = machines;
                _auditLogger.SaveActionLog("Machine", ActionType.ListData, null, null, null, "MachineServiceRepository.GetMachineList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "MachineServiceRepository.GetMachineList()");
            }

            return response;
        }

        public DataResponse GetMachineByID(int MachineID)
        {
            DataResponse response = new DataResponse();
            try
            {
                var machine = _dbcontext.Machine.FirstOrDefault(w => w.MachineID == MachineID);
                if (machine == null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.ID = 0;
                    response.Message = Constants.NoRecordFound;
                }
                else
                {
                    response.ID = MachineID;
                    response.Message = Constants.RecordFound;
                    response.Value = machine;
                }
                _auditLogger.SaveActionLog("Machine", ActionType.Select, MachineID.ToString(), MachineID, null, "MachineServiceRepository.GetMachineByID()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, MachineID, "MachineServiceRepository.GetMachineByID()");
            }

            return response;
        }


        public DataResponse Save(Machine request)
        {
            DataResponse response = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Machine.FirstOrDefault(r => r.MachineCode == request.MachineCode);
                if (dataexists != null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.ID = dataexists.MachineID;
                    response.Message = Constants.DataAlreadyExist;
                    return response;
                }
                _dbcontext.Machine.Add(request);
                _dbcontext.SaveChanges();
                response.ID = request.MachineID;
                response.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Machine", ActionType.Insert, request.MachineID.ToString(), request, null, "MachineServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "MachineServiceRepository.Save()");
            }

            return response;
        }

        public DataResponse Update(Machine request)
        {
            DataResponse response = new DataResponse();
            try
            {
                var dataToUpdate = _dbcontext.Machine.FirstOrDefault(r => r.MachineID == request.MachineID);
                if (dataToUpdate == null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.Message = Constants.NoRecordFound;
                    return response;
                }
                _auditLogger.SaveActionLog("Machine", ActionType.Update, request.MachineID.ToString(), request, dataToUpdate, "MachineServiceRepository.Update()");
                dataToUpdate.MachineCode = request.MachineCode;
                dataToUpdate.MachineName = request.MachineName;
                dataToUpdate.Description = request.Description;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedBy = request.LastUpdatedBy;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;

                _dbcontext.SaveChanges();
                response.ID = request.MachineID;
                response.Message = Constants.SuccessMessage;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "MachineServiceRepository.Update()");
            }

            return response;
        }

        public DataResponse Delete(int MachineID, int UserID, long LoginAuditID)
        {
            DataResponse response = new DataResponse();
            try
            {
                var dataToUpdate = _dbcontext.Machine.FirstOrDefault(w => w.MachineID == MachineID);
                if (dataToUpdate == null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.Message = Constants.NoRecordFound;
                    return response;
                }

                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;
                dataToUpdate.LastUpdatedBy = UserID;
                dataToUpdate.IsDeleted = true;
                _dbcontext.SaveChanges();

                _transLogRespository.SaveLogTransaction(LoginAuditID, "tMachine", MachineID.ToString(), ActionType.Delete);
                response.ID = MachineID;
                response.Message = Constants.SuccessMessage;
                _auditLogger.SaveActionLog("Machine", ActionType.Delete, MachineID.ToString(), null, dataToUpdate, "MachineServiceRepository.Delete()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, MachineID, "MachineServiceRepository.Delete()");
            }

            return response;
        }


        public DataResponse GetMachineTypeList()
        {
            DataResponse response = new DataResponse();
            try
            {
                var machineTypes = _dbcontext.MachineType.OrderBy(o => o.MachineTypeName).ToList();
                response.Count = machineTypes.Count;
                response.Value = machineTypes;
                _auditLogger.SaveActionLog("MachineType", ActionType.ListData, null, machineTypes, null, "MachineServiceRepository.GetMachineTypeList()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "MachineServiceRepository.GetMachineTypeList()");
            }

            return response;
        }

        public DataResponse GetMachineDataTable(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VMachine> query = _dbcontext.VMachine;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.MachineTypeName.Contains(request.SearchValue) || d.MachineCode.Contains(request.SearchValue));
                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VMachine.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                       .Select(w => new
                                       {
                                           w.MachineID,
                                           w.MachineName,
                                           w.MachineCode,
                                           w.MachineTypeName,
                                           w.IsActive
                                       }).ToList();

                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("MachineType", ActionType.ListData, null, request, null, "MachineServiceRepository.GetMachineDataTable()");
                return response;
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, null, "MachineServiceRepository.GetMachineDataTable()");
            }

            return response;
        }
    }
}