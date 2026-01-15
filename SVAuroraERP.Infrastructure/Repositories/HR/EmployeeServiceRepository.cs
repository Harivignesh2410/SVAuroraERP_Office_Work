namespace SVAuroraERP.Infrastructure.Repositories.HR
{
    public class EmployeeServiceRepository : IEmployeeServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public EmployeeServiceRepository(SVAuroraERPDbContext dbcontext,
                                     IAuditLogger auditLogger,
                                     IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }
        public DataResponse GetEmployee()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VEmployee.OrderBy(o => o.EmployeeCode).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Employee", ActionType.ListData, null, null, null, "EmployeeServiceRepository.GetEmployee()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "EmployeeServiceRepository.GetEmployee()");
            }
            return dataResponse;
        }
        public DataResponse GetByID(int EmployeeID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VEmployee.FirstOrDefault(w => w.EmployeeID == EmployeeID);
                if (resultdata == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;
                    return dataResponse;
                }
                dataResponse.ID = EmployeeID;
                dataResponse.Message = Constants.RecordFound;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Employee", ActionType.Select, EmployeeID.ToString(), EmployeeID, null, "EmployeeServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, EmployeeID, "EmployeeServiceRepository.GetByID()");

            }
            return dataResponse;
        }
        public DataResponse Save(Employee request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
              
                var dataexists = _dbcontext.Employee.FirstOrDefault(r => r.EmployeeCode == request.EmployeeCode);
                if (dataexists != null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.EmployeeID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    request.LastUpdatedDate = DateTime.UtcNow;
                }

                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.Employee.Add(request);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("Employee", ActionType.Insert, null, request, null, "EmployeeServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "EmployeeServiceRepository.Save()");
            }
            return dataResponse;
        }
        public DataResponse Update(Employee request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.Employee.FirstOrDefault(r => r.EmployeeID != request.EmployeeID && r.FirstName == request.FirstName);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.EmployeeID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataToUpdate = _dbcontext.Employee.FirstOrDefault(r => r.EmployeeID == request.EmployeeID);
                if (dataToUpdate == null)
                {

                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;
                    return dataResponse;
                }
                _auditLogger.SaveActionLog("Employee", ActionType.Update, request.EmployeeID.ToString(), request, dataToUpdate, "EmployeeServiceRepository.Update()");
                dataToUpdate.EmployeeCode = request.EmployeeCode;
                dataToUpdate.EmployeeTypeID = request.EmployeeTypeID;
                dataToUpdate.Gender = request.Gender;
                dataToUpdate.FirstName = request.FirstName;
                dataToUpdate.MiddleName = request.MiddleName;
                dataToUpdate.SurName = request.SurName;
                dataToUpdate.AddressLine1 = request.AddressLine1;
                dataToUpdate.AddressLine2 = request.AddressLine2;
                dataToUpdate.City = request.City;
                dataToUpdate.State = request.State;
                dataToUpdate.Zipcode = request.Zipcode;
                dataToUpdate.TelNo1 = request.TelNo1;
                dataToUpdate.TelNo2 = request.TelNo2;
                dataToUpdate.MobileNo = request.MobileNo;
                dataToUpdate.Email = request.Email;
                dataToUpdate.PlaceofBirth = request.PlaceofBirth;
                dataToUpdate.EmergencyRelationshipContactID = request.EmergencyRelationshipContactID;
                dataToUpdate.EmergencyContactName = request.EmergencyContactName;
                dataToUpdate.EmergencyContactNo = request.EmergencyContactNo;
                dataToUpdate.DOB = request.DOB;
                dataToUpdate.FatherName = request.FatherName;
                dataToUpdate.FatherDOB = request.FatherDOB;
                dataToUpdate.MotherName = request.MotherName;
                dataToUpdate.MotherDOB = request.MotherDOB;
                dataToUpdate.MaritalStatus = request.MaritalStatus;
                dataToUpdate.SpouseName = request.SpouseName;
                dataToUpdate.SpouseDOB = request.SpouseDOB;
                dataToUpdate.AnniversaryDate = request.AnniversaryDate;
                dataToUpdate.ChildOneName = request.ChildOneName;
                dataToUpdate.ChildOneDOB = request.ChildOneDOB;
                dataToUpdate.ChildTwoName = request.ChildTwoName;
                dataToUpdate.ChildTwoDOB = request.ChildTwoDOB;
                dataToUpdate.IsActive = request.IsActive;
                dataToUpdate.LastUpdatedDate = DateTime.UtcNow;

                _dbcontext.SaveChanges();
                dataResponse.ID = dataToUpdate.EmployeeID;
                dataResponse.Message = Constants.UpdatedSucessfully;

            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "EmployeeServiceRepository.Update()");
            }
            return dataResponse;
        }
        public DataResponse Delete(int EmployeeID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.Employee.FirstOrDefault(w => w.EmployeeID == EmployeeID);
                if (dataexists == null)
                {
                    dataResponse.Error = false;
                    dataResponse.Success = true;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;
                    _auditLogger.SaveActionLog("Employee", ActionType.Delete, EmployeeID.ToString(), null, null, "EmployeeServiceRepository.Delete()");
                    return dataResponse;
                }
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                dataexists.IsDeleted = true;
                _dbcontext.SaveChanges();

                dataResponse.ID = dataexists.DesignationID;
                dataResponse.Message = Constants.SuccessMessage;
                
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, EmployeeID, "EmployeeServiceRepository.Delete()");
            }
            return dataResponse;
        }
        public DataResponse GetBloodGroupList()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.BloodGroup.OrderBy(w => w.BloodGroupID).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("BloodGroup", ActionType.ListData, null, null, null, "EmployeeServiceRepository.GetBloodGroupList()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "EmployeeServiceRepository.GetBloodGroupList()");
            }
            return dataResponse;
        }
        public DataResponse GetEmployeeDataTable(DataTableRequest request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {

                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VEmployee> query = _dbcontext.VEmployee;

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.DesignationName.Contains(request.SearchValue) ||
                                             d.EmployeeCode.Contains(request.SearchValue) || d.FirstName.Contains(request.SearchValue) ||
                                             d.City.Contains(request.SearchValue));
                }

                if (request.IsCustomFilterEnabled)
                {

                }

                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VEmployee.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.DesignationID,
                                                       w.EmployeeID,
                                                       w.DesignationName,
                                                       w.EmployeeCode,
                                                       w.FirstName,
                                                       w.City,
                                                       w.MobileNo,
                                                       w.Email,
                                                       w.IsActive
                                                   }).ToList();


                dataResponse.Value = pagedData;
                dataResponse.recordsTotal = totalRecords;
                dataResponse.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("Employee", ActionType.ListData, null, request, null, "EmployeeServiceRepository.GetEmployeeDataTable()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "EmployeeServiceRepository.GetEmployeeDataTable()");
            }
            return dataResponse;
        }
    }
}
