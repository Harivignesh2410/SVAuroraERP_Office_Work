namespace SVAuroraERP.Infrastructure.Repositories.Orders.ManageOrder
{
    public class QualityProcessingServiceRepository : IQualityProcessingServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public QualityProcessingServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse GetQualityProcessing(QualityProcessingRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VJobCardGenerated> query = _dbcontext.VJobCardGenerated;
                var hsrpUser = _dbcontext.VHSRPUser
                                        .FirstOrDefault(u => u.UserID == request.EmbossingStationID);
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
                var totalRecords = _dbcontext.VJobCardGenerated.Count();

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
                                                       w.LastUpdatedByName,
                                                       w.FrontPlateDimension,
                                                       w.RearPlateDimension
                                                   }).ToList();

                response.Value = pagedData.OrderByDescending(w => w.OrderNo);
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VJobCardGenerated", ActionType.ListData, null, request, null, "QualityProcessingServiceRepository.GetQualityProcessing()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "QualityProcessingServiceRepository.GetQualityProcessing()");
            }

            return response;
        }
        public DataResponse Save(QualityProcessRequest request)
        {
            var response = new DataResponse();

            try
            {
                // Check if the service already exists

                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                using var command = connection.CreateCommand();
                connection.Open();

                command.CommandText = "UpdateHSRPOrderStatusToQualityProcessing";
                command.CommandType = CommandType.StoredProcedure;

                var pkqualitycheckid = command.CreateParameter();
                pkqualitycheckid.ParameterName = "@PK_QualityCheckID";
                pkqualitycheckid.Direction = System.Data.ParameterDirection.Output;
                pkqualitycheckid.DbType = System.Data.DbType.Int32;
                pkqualitycheckid.Value = 0;

                command.Parameters.Add(@pkqualitycheckid);
                command.Parameters.Add(new SqlParameter("@FK_OrderID", request.OrderID));
                command.Parameters.Add(new SqlParameter("@LastUpdatedBy", request.LastUpdatedBy));
                command.Parameters.Add(new SqlParameter("@FK_FrontLaserNoID", request.FrontLaserNoID));
                command.Parameters.Add(new SqlParameter("@FK_RearLaserNoID", request.RearLaserNoID));
                command.Parameters.Add(new SqlParameter("@VerifiedFrontVehicleNo", request.VerifiedFrontVehicleNo ?? (object)DBNull.Value));    
                command.Parameters.Add(new SqlParameter("@VerifiedFrontLaserNo", request.VerifiedFrontLaserNo ?? (object)DBNull.Value));    
                command.Parameters.Add(new SqlParameter("@FrontVehicleNoImageUrl", request.FrontVehicleNoImageUrl ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@VerifiedRearVehicleNo", request.VerifiedRearVehicleNo ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@VerifiedRearLaserNo", request.VerifiedRearLaserNo ?? (object)DBNull.Value));
                command.Parameters.Add(new SqlParameter("@RearVehicleNoImageUrl", request.RearVehicleNoImageUrl ?? (object)DBNull.Value));
                

                using (var reader = command.ExecuteReader())
                {
                    int id = (int)pkqualitycheckid.Value;

                    if (id == -1)
                    {
                        response.Success = false;
                        response.Error=true;
                        response.Message = Constants.DataAlreadyExist;
                        return response;
                    }

                    if (id > 0)
                    {
                        response.Success = true;
                        response.Message = Constants.SuccessMessage;
                        response.ID = id;
                    }
                }

                _auditLogger.SaveActionLog("QualityProcessRequest", ActionType.Insert, response.ID.ToString(), request, null, "QualityProcessingServiceRepository.Save()");

            }
            catch (Exception ex)
            {
                 response = _errorLoggerService.LogException(ex, request, "QualityProcessingServiceRepository.Save()");
            }

            return response;
        }
        public DataResponse Reject(int LaserNoPlateID,int LastUpdatedBy)
        {
            var response = new DataResponse();

            try
            {
                // Check if the service already exists

                using var connection = new SqlConnection(_dbcontext.Database.GetConnectionString());
                using var command = connection.CreateCommand();
                connection.Open();

                command.CommandText = StoredProcedure.REJECTHSRPLASERNOPLATE;
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@FK_LaserNoPlateID", LaserNoPlateID));
                command.Parameters.Add(new SqlParameter("@LastUpdatedBy", LastUpdatedBy));

                command.ExecuteNonQuery();

                _auditLogger.SaveActionLog("QualityProcessRequest", ActionType.Delete, LaserNoPlateID.ToString(), null, null, "QualityProcessingServiceRepository.Reject()");

            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, LaserNoPlateID, "QualityProcessingServiceRepository.Reject()");
            }

            return response;
        }
    }
}
