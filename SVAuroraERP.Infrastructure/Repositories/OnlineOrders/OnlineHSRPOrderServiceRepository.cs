using Org.BouncyCastle.Asn1.X509;
using System.Security.Claims;
using SVAuroraERP.Application.Interfaces.Persistance.OnlineOrders;
using SVAuroraERP.Domain.OnlineOrders;

namespace SVAuroraERP.Infrastructure.Repositories.OnlineOrders
{
    public class OnlineHSRPOrderServiceRepository : IOnlineHSRPOrderServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public OnlineHSRPOrderServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse GetOnlineOrderList(OnlineOrderDTRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VOnlineHSRPOrder> query = _dbcontext.VOnlineHSRPOrder;

                if (request.StartDate.HasValue) { query = query.Where(w => w.OrderDate >= request.StartDate); }
                if (request.EndDate.HasValue) { query = query.Where(w => w.OrderDate <= request.EndDate); }
                if (request.OEMID > 0) { query = query.Where(w => w.OEMID == request.OEMID); }
                if (request.DealerID > 0) { query = query.Where(w => w.DealerID == request.DealerID); }
              
                var totalRecords = _dbcontext.VOnlineHSRPOrder.Count();
                var filteredRecords = query.Count();
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.OnlineHSRPOrderID,
                                                       w.OrderNo,
                                                       w.sOrderDate,
                                                       w.OEMName,
                                                       w.VehicleNo,
                                                       w.VehicleClassName,
                                                       w.VehiclePlateType,
                                                       w.VehiclePlateSizeName,
                                                       w.VehiclePlateColorName,
                                                       w.FitmentTypeName,
                                                       w.DealerName,
                                                       w.OrderStatusName,
                                                       w.ColorCode
                                                   }).ToList();

                response.Value = pagedData.OrderByDescending(w=>w.OrderNo);
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VOnlineHSRPOrder", ActionType.ListData, null, request, null, "OnlineHSRPOrderServiceRepository.GetOnlineOrderList()");

            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "OnlineHSRPOrderServiceRepository.GetOnlineOrderList()");
            }
            return response;
        }
        public DataResponse GetOnlineOrderByHSRPOrderID(int OnlineHSRPOrderID)
        {
            DataResponse response = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VOnlineHSRPOrder.FirstOrDefault(w => w.OnlineHSRPOrderID == OnlineHSRPOrderID);

                response.Value = resultdata;

                _auditLogger.SaveActionLog("VOnlineHSRPOrder", ActionType.ListData, null, OnlineHSRPOrderID, null, "OnlineHSRPOrderServiceRepository.GetOnlineOrderByHSRPOrderID()");

            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, ActionType.ListData, "OnlineHSRPOrderServiceRepository.GetOnlineOrderByHSRPOrderID()");
            }
            return response;
        }

        public DataResponse ApproveOnlineOrders(Approvedata request)
        {
            DataResponse response = new DataResponse();

            try
            {
                using var connection = new SqlConnection(
                    _dbcontext.Database.GetConnectionString());

                using var command = new SqlCommand("InsertOnlineOrderToHSRPOrder", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 60
                };

                command.Parameters.Add("@PK_OnlineHSRPOrderID", SqlDbType.Int)
                       .Value = request.OnlineHSRPOrderID;

                command.Parameters.Add("@EmbossingStationID", SqlDbType.Int)
                       .Value = request.EmbossingStationID;

                command.Parameters.Add("@LastUpdatedBy", SqlDbType.Int)
                       .Value = request.LastUpdatedBy;

                connection.Open();
                command.ExecuteNonQuery();

                response.Success = true;
                response.Message = "Online order approved successfully";

                _auditLogger.SaveActionLog(
                    "ApproveOnlineOrders()",
                    ActionType.Insert,
                    request.OnlineHSRPOrderID.ToString(),
                    request,
                    null,
                    "OnlineHSRPOrderServiceRepository"
                );
            }
            catch (SqlException ex)
            {
                response.Success = false;
                response.Message = ex.Message;   
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Unexpected error occurred";
            }

            return response;
        }


    }
}
