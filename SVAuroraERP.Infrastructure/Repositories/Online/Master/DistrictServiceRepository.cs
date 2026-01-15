using Azure;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SVAuroraERP.Infrastructure.Repositories.Online.Master
{
    public class DistrictServiceRepository : IDistrictServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly ILogger<DistrictServiceRepository> _logger;
        private readonly ITransLogRespository _transLogRespository;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public DistrictServiceRepository(SVAuroraERPDbContext dbcontext,
                                     ILogger<DistrictServiceRepository> logger,
                                     ITransLogRespository transLogRespository,
                                      IAuditLogger auditLogger)
        {
            _dbcontext = dbcontext;
            _logger = logger;
            _transLogRespository = transLogRespository;
            _auditLogger = auditLogger;
        }
        public DataResponse GetDistrict()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDistrict.OrderBy(o => o.StateName).ToList();
                dataResponse.Count = resultdata.Count;
                dataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("District", ActionType.ListData, null, null,null, "DistrictServiceRepository.GetDistrict()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "DistrictServiceRepository.GetDistrict()");
            }

            return dataResponse;
        }
        public DataResponse GetDistrictByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VDistrict.FirstOrDefault(w => w.DistrictID == ID);
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
                _auditLogger.SaveActionLog("District", ActionType.Select, ID.ToString(), ID, null, "DistrictServiceRepository.GetDistrictByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, ID, "DistrictServiceRepository.GetDistrictByID()");
            }

            return dataResponse;
        }
        public DataResponse Save(District District)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var dataexists = _dbcontext.District.FirstOrDefault(r => r.DistrictName == District.DistrictName);
                if (dataexists != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = dataexists.DistrictID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }

                District.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.District.Add(District);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("District", ActionType.Insert, District.DistrictID.ToString(), District,null, "DistrictServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, District, "DistrictServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse Update(District District)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var isFound = _dbcontext.District.FirstOrDefault(r => r.DistrictID != District.DistrictID && r.DistrictName == District.DistrictName);
                if (isFound != null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = isFound.DistrictID;
                    dataResponse.Message = Constants.DataAlreadyExist;
                    return dataResponse;
                }
                var dataexists = _dbcontext.District.FirstOrDefault(r => r.DistrictID == District.DistrictID);
                if (dataexists == null)
                {
                    dataResponse.Error = true;
                    dataResponse.Success = false;
                    dataResponse.ID = 0;
                    dataResponse.Message = Constants.NoRecordFound;

                    return dataResponse;
                }
                _auditLogger.SaveActionLog("District", ActionType.Update, dataexists.DistrictID.ToString(), District, District, "DistrictServiceRepository.Update()");
                dataexists.StateID = District.StateID;
                dataexists.DistrictName = District.DistrictName;
                dataexists.DistrictCode = District.DistrictCode;
                dataexists.IsActive = District.IsActive;
                dataexists.LastUpdatedBy = District.LastUpdatedBy;
                dataexists.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.SaveChanges();
                dataResponse.ID = dataexists.StateID;
                dataResponse.Message = Constants.UpdatedSucessfully;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, District, "DistrictServiceRepository.Update()");
            }

            return dataResponse;
        }
        public DataResponse Delete(int DistrictID, int UserID, long LoginAuditID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var dataexists = _dbcontext.District.FirstOrDefault(w => w.DistrictID == DistrictID);
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
                _auditLogger.SaveActionLog("District", ActionType.Delete, DistrictID.ToString(),new { DistrictID, UserID, LoginAuditID });
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, DistrictID, "DistrictServiceRepository.Delete()");
            }

            return dataResponse;
        }
        public DataResponse GetDistrictList(DataTableRequest request)
        { 
            DataResponse response = new DataResponse();
            try
            {
            // Validate and sanitize inputs
            var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
            var skip = Math.Max(request.Start, 0);

            IQueryable<VDistrict> query = _dbcontext.VDistrict;

            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(request.SearchValue))
            {
            query = query.Where(d => (d.DistrictName ?? "").Contains(request.SearchValue)
            || (d.StateName ?? "").Contains(request.SearchValue)|| (d.DistrictCode ?? "").Contains(request.SearchValue));
            }

            // Get TOTAL records in database (unfiltered)
            var totalRecords = _dbcontext.VDistrict.Count();

            // Get FILTERED records count (same as total if no filter applied)
            var filteredRecords = query.Count();

            // Apply sorting 
            query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

            // Apply paging
            var pagedData = query.Skip(skip).Take(pageSize)
                        .Select(w => new
                        {
                            w.DistrictID,
                            w.DistrictName,
                            w.DistrictCode,
                            w.StateName,
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