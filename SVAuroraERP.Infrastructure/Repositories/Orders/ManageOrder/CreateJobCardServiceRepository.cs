namespace SVAuroraERP.Infrastructure.Repositories.Orders.ManageOrder
{
    public class CreateJobCardServiceRepository : ICreateJobCardServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public CreateJobCardServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse GetCreateJobCard(CreateJobCardRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VCreateJobCard> query = _dbcontext.VCreateJobCard;
                var hsrpUser = _dbcontext.VHSRPUser
                         .FirstOrDefault(u => u.HSRPUserID == request.EmbossingStationID);

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.OrderNo.Contains(request.SearchValue) ||
                                          d.OrderTypeName.Contains(request.SearchValue) ||
                                          d.RegNo.Contains(request.SearchValue) ||
                                          d.Dealer.Contains(request.SearchValue) ||
                                          d.DealerCode.Contains(request.SearchValue) ||
                                          d.OEM.Contains(request.SearchValue) ||
                                          d.ChasisNo.Contains(request.SearchValue) ||
                                          d.EngineNo.Contains(request.SearchValue) ||
                                          d.RearLaserSerialNo.Contains(request.SearchValue) ||
                                          d.FrontLaserSerialNo.Contains(request.SearchValue) ||
                                          d.FrontPlateDimension.Contains(request.SearchValue) ||
                                          d.RearPlateDimension.Contains(request.SearchValue));
                }
                if (request.StartDate.HasValue) { query = query.Where(w => w.OrderDate >= request.StartDate); }
                if (request.EndDate.HasValue) { query = query.Where(w => w.OrderDate <= request.EndDate); }
                if (request.orderTypeID > 0) { query = query.Where(w => w.OrderTypeID == request.orderTypeID); }
                if (request.OEMID > 0) { query = query.Where(w => w.OEMID == request.OEMID); }
                if (request.DealerID > 0) { query = query.Where(w => w.DealerID == request.DealerID); }
                if (request.EmbossingStationID > 0) { query = query.Where(w => w.EmbossingStationID == request.EmbossingStationID); }
                if (!string.IsNullOrWhiteSpace(request.SearchText))
                {
                    query = query.Where(d => d.OrderNo.Contains(request.SearchValue) ||
                                             d.OrderTypeName.Contains(request.SearchValue)
                                             );
                }
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VCreateJobCard.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.HSRPOrderID,
                                                       w.OrderTypeID,
                                                       w.OrderTypeName,
                                                       w.OrderNo,
                                                       w.OrderDate,
                                                       w.sOrderDate,
                                                       w.ssOrderDate,
                                                       w.DealerPONo,
                                                       w.DealerSONo,
                                                       w.DealerID,
                                                       w.Dealer,
                                                       w.OEMID,
                                                       w.OEM,
                                                       w.EmbossingStationID,
                                                       w.EmbossingStation,
                                                       w.OrderStatusID,
                                                       w.Description,
                                                       w.ColorCode,
                                                       w.IconCode,
                                                       w.DealerCode,
                                                       w.OEMCode,
                                                       w.EmbossingStationCode,
                                                       w.DealerCity,
                                                       w.OEMCity,
                                                       w.EmbossingStationCity,
                                                       w.ProcessDate,
                                                       w.HSRPVehicleInfoID,
                                                       w.HSRPOrderRefID,
                                                       w.RegNo,
                                                       w.RegDate,
                                                       w.sRegDate,
                                                       w.ChasisNo,
                                                       w.EngineNo,
                                                       w.sProcessDate,
                                                       w.PlateColor,
                                                       w.RearLaserSerialNo,
                                                       w.FrontLaserSerialNo,
                                                       w.LastUpdatedBy,
                                                       w.LastUpdatedDate,
                                                       w.PartNo,
                                                       w.FrontPlateSize,
                                                       w.RearPlateSize,
                                                       w.ssRegDate,
                                                       //w.InvoiceNo,
                                                       //w.sInvoiceDate,
                                                       //w.InvoiceNetAmount,
                                                       w.FrontPlateDimension,
                                                       w.RearPlateDimension,
                                                       w.LastUpdatedByName
                                                   }).ToList();
                response.Value = pagedData.OrderByDescending(w => w.OrderNo);
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VCreateJobCard", ActionType.ListData, null, request, null, "CreateJobCardServiceRepository.GetCreateJobCard()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "CreateJobCardServiceRepository.GetCreateJobCard()");
            }
            return response;
        }
        public DataResponse Save(HSRPJobCardRequest model)
        {
            var response = new DataResponse();

            try
            {
                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                using var command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "InsertHSRPJobCard";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@OrderIds", model.OrderIds));
                command.Parameters.Add(new SqlParameter("@EmbossingID", model.EmbossingID));
                command.Parameters.Add(new SqlParameter("@LastUpdatedBy", model.LastUpdatedBy));

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        response.Success = Convert.ToBoolean(reader["Success"]);
                        response.Message = response.Success
                            ? "Job Card created successfully."
                            : reader["ErrorMessage"]?.ToString();

                        if (response.Success)
                        {
                            response.ID = Convert.ToInt32(reader["JobCardID"]);
                        }
                    }
                }
                _auditLogger.SaveActionLog("InsertHSRPJobCard", ActionType.ListData, null, model, null, "LaserNoAllocationServiceRepository.Save()");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response = _errorLoggerService.LogException(ex, model, "CreateJobCardServiceRepository.Save()");
            }

            return response;
        }
        public DataResponse GetHsrpJobcard(CreateJobRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                var pageSize = Math.Clamp(request.Length, 1, 100);
                var skip = Math.Max(request.Start, 0);

                IQueryable<VHSRPJobCard> query = _dbcontext.VHSRPJobCard;
                var hsrpUser = _dbcontext.VHSRPUser.FirstOrDefault(u => u.UserID == request.EmbossingStationID);

                //if (hsrpUser != null)
                //{
                //    // Replace the EmbossingStationID in the request with HSRPUserID
                //    request.EmbossingStationID = hsrpUser.HSRPUserID;

                //    // 🔹 Filter by actual EmbossingStationID from job card table
                //    query = query.Where(w => w.EmbossingStationID == request.EmbossingStationID);
                //}
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d =>
                        d.JobCardNo.Contains(request.SearchValue) ||
                        d.EmbossingName.Contains(request.SearchValue) ||
                        d.DealerName.Contains(request.SearchValue) ||
                        d.EmbossingCity.Contains(request.SearchValue)
                    );
                }

                if (request.StartDate.HasValue)
                    query = query.Where(w => w.JobCardDate >= request.StartDate);

                if (request.EndDate.HasValue)
                    query = query.Where(w => w.JobCardDate <= request.EndDate);

                if (request.DealerID > 0)
                  query = query.Where(w => w.DealerID == request.DealerID);

                if (request.EmbossingStationID > 0)
                    query = query.Where(w => w.EmbossingStationID == request.EmbossingStationID);

                var totalRecords = _dbcontext.VHSRPJobCard.Count();
                var filteredRecords = query.Count();

                query = query.OrderByDescending(q => q.JobCardNo);

                var jobcards = query.Skip(skip).Take(pageSize).ToList();

                foreach (var jobcard in jobcards)
                {
                    jobcard.VHSRPJobCardTrans = _dbcontext.VHSRPJobCardTrans
                        .Where(t => t.HSRPJobCardID == jobcard.HSRPJobCardID)
                        .ToList();
                }

                response.Value = jobcards;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VHSRPJobCard", ActionType.ListData, null, request, null, "CreateJobCardServiceRepository.GetHsrpJobcard()");

            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "CreateJobCardServiceRepository.GetHsrpJobcard()");
            }

            return response;
        }
        public DataResponse GetJobcardByID(int ID)
        {
            DataResponse response = new DataResponse();

            try
            {
                var jobCard = _dbcontext.VHSRPJobCard
                    .FirstOrDefault(w => w.HSRPJobCardID == ID);

                if (jobCard == null)
                {
                    response.Error = true;
                    response.Success = false;
                    response.ID = 0;
                    response.Message = Constants.NoRecordFound;
                    return response;
                }

                var transList = _dbcontext.VHSRPJobCardTrans
                    .Where(t => t.HSRPJobCardID == ID)
                    .ToList();

                // Attach it (assuming “Job” is not mapped in EF)
                jobCard.VHSRPJobCardTrans = transList;

                response.Error = false;
                response.Success = true;
                response.ID = ID;
                response.Value = jobCard;
                response.Message = Constants.SuccessMessage;

                _auditLogger.SaveActionLog("JobCard", ActionType.Select, ID.ToString(), ID, null, "JobCardServiceRepository.GetJobcardByID()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, ID, "JobCardServiceRepository.GetJobcardByID()");
            }

            return response;
        }
        public DataResponse GetLasserNo()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.LaserNoPlate.OrderBy(o => o.LaserNoPlateID).ToList();
                DataResponse.Count = resultdata.Count;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("tLaserNoPlate", ActionType.ListData, null, null,null, "JobCardServiceRepository.GetLasserNo()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "JobCardServiceRepository.GetLasserNo()");
            }
            return DataResponse;
        }
    }
}
