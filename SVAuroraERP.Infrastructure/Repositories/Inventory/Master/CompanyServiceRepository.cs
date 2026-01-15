namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Master
{
    public class CompanyServiceRepository : ICompanyServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public CompanyServiceRepository(SVAuroraERPDbContext dbcontext,
                                     IAuditLogger auditLogger,
                                     IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;            
        }

        public DataResponse dataResponse { get; private set; }

        public DataResponse GetCompany()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VCompany.FirstOrDefault();
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("Company", ActionType.ListData, null, null,null, "CompanyServiceRepository.GetCompany()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "CompanyServiceRepository.GetCompany()");
            }

            return DataResponse;
        }
        public DataResponse Save(Company request)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                Company company = new Company();

                var recordCount = _dbcontext.Company.Count();

                if (recordCount > 0)
                {
                    var dataToSave = _dbcontext.Company.FirstOrDefault(w => w.CompanyID == request.CompanyID);

                    if (dataToSave != null)
                    {
                        dataToSave.CompanyName = request.CompanyName;
                        dataToSave.GSTNo = request.GSTNo;
                        dataToSave.PANNo = request.PANNo;
                        dataToSave.AddressLine1 = request.AddressLine1;
                        dataToSave.AddressLine2 = request.AddressLine2;
                        dataToSave.City = request.City;
                        dataToSave.TelNo1 = request.TelNo1;
                        dataToSave.TelNo2 = request.TelNo2;
                        dataToSave.State = request.State;
                        dataToSave.Pincode = request.Pincode;
                        dataToSave.MobileNo = request.MobileNo;
                        dataToSave.Email = request.Email;
                        dataToSave.BankName = request.BankName;
                        dataToSave.BranchName = request.BranchName;
                        dataToSave.IFSCCode = request.IFSCCode;
                        dataToSave.AccountHolderName = request.AccountHolderName;
                        dataToSave.AccountType = request.AccountType;
                        dataToSave.LastUpdatedBy = request.LastUpdatedBy;
                        dataToSave.LastUpdatedDate = DateTime.UtcNow;

                        _dbcontext.SaveChanges();
                        DataResponse.ID = request.CompanyID;
                        DataResponse.Message = Constants.SuccessMessage;
                    }
                }
                else
                {
                    request.LastUpdatedDate = DateTime.UtcNow;
                    _dbcontext.Company.Add(request);
                    _dbcontext.SaveChanges();
                    DataResponse.ID = request.CompanyID;
                    DataResponse.Message = Constants.SuccessMessage;
                }
                _auditLogger.SaveActionLog("Company", ActionType.Insert, request.CompanyID.ToString(), request, null, "CompanyServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, request, "CompanyServiceRepository.Save()");
            }

            return DataResponse;
        }
    }
}