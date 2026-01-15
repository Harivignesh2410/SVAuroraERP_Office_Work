using SVAuroraERP.Domain.Authentication;

namespace SVAuroraERP.Infrastructure.Repositories.Orders.ManageOrder
{
    public class UpdateOrderDataServiceRepository : IUpdateOrderDataServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;

        public UpdateOrderDataServiceRepository(
            IErrorLoggerService errorLoggerService,
            SVAuroraERPDbContext dbcontext,
            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }

        public DataResponse GetRectificationReason()
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.LkupHSRPOrderRectificationReason.ToList();

                DataResponse.Count = resultdata.Count;
                DataResponse.Value = resultdata;

                _auditLogger.SaveActionLog("LkupHSRPOrderRectificationReason", ActionType.ListData, null, null, null,"UpdateOrderDataServiceRepository.GetRectificationReason()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, null, "UpdateOrderDataServiceRepository.GetRectificationReason()");
            }
            return DataResponse;
        }

        public DataResponse GetUpdateOrder(DataTableRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                var pageSize = Math.Clamp(request.Length, 1, 100);
                var skip = Math.Max(request.Start, 0);

                IQueryable<VCreateJobCard> query = _dbcontext.VCreateJobCard;

                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.OrderNo.Contains(request.SearchValue) ||
                                           d.OrderTypeName.Contains(request.SearchValue) ||
                                           d.RegNo.Contains(request.SearchValue) ||
                                           d.Dealer.Contains(request.SearchValue) ||
                                           d.OEM.Contains(request.SearchValue) ||
                                           d.ChasisNo.Contains(request.SearchValue) ||
                                           d.EngineNo.Contains(request.SearchValue) ||
                                           d.RearLaserSerialNo.Contains(request.SearchValue) ||
                                           d.FrontLaserSerialNo.Contains(request.SearchValue) ||
                                           d.FrontPlateDimension.Contains(request.SearchValue) ||
                                           d.RearPlateDimension.Contains(request.SearchValue));
                }

                var totalRecords = _dbcontext.VCreateJobCard.Count();
                var filteredRecords = query.Count();

                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                var pagedData = query.Skip(skip).Take(pageSize)
                    .Select(w => new
                    {
                        w.HSRPOrderID,
                        w.OrderNo,
                        w.OrderDate,
                        w.sOrderDate,
                        w.DealerPONo,
                        w.DealerSONo,
                        w.RegNo,
                        w.RegDate,
                        w.sRegDate,
                        w.ChasisNo,
                        w.EngineNo,
                        w.FrontLaserSerialNo,
                        w.RearLaserSerialNo,
                        w.PartNo,
                        w.PlateColor,
                        w.FrontPlateDimension,
                        w.RearPlateDimension,
                        w.LastUpdatedBy,
                        w.LastUpdatedDate,
                        w.LastUpdatedByName
                    }).ToList();

                response.Value = pagedData.OrderByDescending(w => w.OrderNo);
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VCreateJobCard", ActionType.ListData, null, request, null, "UpdateOrderDataServiceRepository.GetUpdateOrder()");
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "UpdateOrderDataServiceRepository.GetUpdateOrder()");
            }
            return response;
        }

        public DataResponse GetByID(int ID)
        {
            DataResponse DataResponse = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VHSRPOrder.FirstOrDefault(w => w.HSRPOrderID == ID);
                if (resultdata == null)
                {
                    DataResponse.Error = true;
                    DataResponse.Success = false;
                    DataResponse.Message = Constants.NoRecordFound;
                    return DataResponse;
                }

                DataResponse.ID = ID;
                DataResponse.Success = true;
                DataResponse.Message = Constants.RecordFound;
                DataResponse.Value = resultdata;
                _auditLogger.SaveActionLog("VCreateJobCard", ActionType.Select, ID.ToString(), ID, null, "UpdateOrderDataServiceRepository.GetByID()");
            }
            catch (Exception ex)
            {
                DataResponse = _errorLoggerService.LogException(ex, ID, "UpdateOrderDataServiceRepository.GetByID()");
            }
            return DataResponse;
        }
        public DataResponse SaveLaserNoForOrder(LaserNoUpdateRequest request)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.UPDATELASERNOFORORDER; 
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.AddWithValue("@FK_NewFrontLaserNoID", request.FrontLaserNoPlateID);
                        command.Parameters.AddWithValue("@FK_NewRearLaserNoID", request.RearLaserNoPlateID);
                        command.Parameters.AddWithValue("@FK_OrderID", request.HSRPOrderID);
                        command.Parameters.AddWithValue("@LastUpdatedBy", request.LastUpdatedBy);
                        command.Parameters.AddWithValue("@OrderDate", request.OrderDate);
                        command.Parameters.AddWithValue("@EngineNumber", request.EngineNumber);
                        command.Parameters.AddWithValue("@ChassisNumber", request.ChassisNumber);


                        command.ExecuteNonQuery();
                        dataResponse.Success = true;
                        dataResponse.Message = "Laser number updated successfully.";
                    }
                }

                _auditLogger.SaveActionLog("LaserNoUpdate", ActionType.Update, request.ToString(), request, null, "UpdateOrderDataServiceRepository.SaveLaserNoForOrder()");
            }
            catch (Exception ex)
            {
                dataResponse=_errorLoggerService.LogException(ex, request, "UpdateOrderDataServiceRepository.SaveLaserNoForOrder()");
            }

            return dataResponse;
        }

        public DataResponse GetLaserNoByPartNo(string PartNo)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                LaserNoDataResponse object1 = new LaserNoDataResponse();

                var dataResult = GetLaserNo(PartNo);

                _auditLogger.SaveActionLog("LaserNoDataResponse", ActionType.ListData, null, PartNo, null, "UpdateOrderDataServiceRepository.GetLaserNoByPartNo()");
                object1.FrontLaserNoData = dataResult.FrontLaserNoData?.ToList<LaserNoData>() ?? new List<LaserNoData>();
                object1.RearLaserNoData = dataResult.RearLaserNoData?.ToList<LaserNoData>() ?? new List<LaserNoData>();

                dataResponse.Value = object1;
                dataResponse.Message = Constants.RecordFound;
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "UpdateOrderDataServiceRepository.GetLaserNoByPartNo()");
            }
            return dataResponse;
        }

        public FullLaserNoDataResult GetLaserNo(string PartNo)
        {
            try
            {
                var result = new FullLaserNoDataResult();

                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                using (var command = new SqlCommand("GetAvailableLaserNosByPartNo", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@PartNo", PartNo);

                    var adapter = new SqlDataAdapter(command);
                    var dataSet = new DataSet();
                    adapter.Fill(dataSet);

                    if (dataSet.Tables.Count > 0) result.FrontLaserNoData = dataSet.Tables[0];
                    if (dataSet.Tables.Count > 1) result.RearLaserNoData = dataSet.Tables[1];

                }

                _auditLogger.SaveActionLog("FullLaserNoDataResult", ActionType.ListData, null, PartNo, null, "UpdateOrderDataServiceRepository.GetLaserNoByPartNo()");
                return result;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "UpdateOrderDataServiceRepository.GetLaserNoByPartNo()");
                return new FullLaserNoDataResult();
            }
        }

        public DataResponse SaveRectification(RectifyLaserPlate request)
        {
            DataResponse response = new DataResponse();

            try
            {
                if (request.FrontLaserNoPlateID == 0) request.FrontLaserNoPlateID = null;
                if (request.RearLaserNoPlateID == 0) request.RearLaserNoPlateID = null;
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = new SqlCommand(StoredProcedure.INSERTRECTIFYLASERPLATE, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        var pkParam = new SqlParameter("@PK_RectifyLaserPlateID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.Output
                        };
                        command.Parameters.Add(pkParam);

                        command.Parameters.AddWithValue("@FK_HSRPOrderID", request.HSRPOrderID);
                        command.Parameters.AddWithValue("@FK_HSRPOrderRectificationReasonID", request.HSRPOrderRectificationReasonID);
                        command.Parameters.AddWithValue("@FK_FrontLaserNoPlateID", request.FrontLaserNoPlateID);
                        command.Parameters.AddWithValue("@FK_RearLaserNoPlateID", request.RearLaserNoPlateID);
                        command.Parameters.AddWithValue("@Remarks", request.Remarks ?? string.Empty);
                        command.Parameters.AddWithValue("@LastUpdatedBy", request.LastUpdatedBy);

                        command.ExecuteNonQuery();

                        request.RectifyLaserPlateID = Convert.ToInt32(pkParam.Value);
                    }
                }

                if(request.RectifyLaserPlateID==0)
                {
                    response.Success = false;
                    response.Error = true;

                    return response;
                }
                response.Success = true;
                response.Message = "Rectification saved successfully.";
                response.ID = request.RectifyLaserPlateID;

                _auditLogger.SaveActionLog(
                    "RectifyLaserPlate",
                    ActionType.Insert,
                    request.RectifyLaserPlateID.ToString(),
                    request.RectifyLaserPlateID,
                    request,
                    "UpdateOrderDataServiceRepository.SaveRectification()"
                );
            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(
                    ex,
                    request,
                    "UpdateOrderDataServiceRepository.SaveRectification()"
                );
            }

            return response;
        }

    }
}