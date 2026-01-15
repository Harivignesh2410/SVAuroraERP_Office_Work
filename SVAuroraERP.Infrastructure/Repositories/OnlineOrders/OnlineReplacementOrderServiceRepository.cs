using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SVAuroraERP.Application.Interfaces.Persistance.OnlineOrders;
using SVAuroraERP.Domain.OnlineOrders;

namespace SVAuroraERP.Infrastructure.Repositories.OnlineOrders
{
    public class OnlineReplacementOrderServiceRepository : IOnlineReplacementOrderServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        
        public OnlineReplacementOrderServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        
        public DataResponse GetReplacementOrderList(ReplacementOrderDTRequest request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VOnlineReplacementOrderDetails> query = _dbcontext.VOnlineReplacementOrderDetails;

                if (request.StartDate.HasValue) { query = query.Where(w => w.OrderDate >= request.StartDate); }
                if (request.EndDate.HasValue) { query = query.Where(w => w.OrderDate <= request.EndDate); }
                if (request.OEMID > 0) { query = query.Where(w => w.OEMID == request.OEMID); }
                if (request.DealerID > 0) { query = query.Where(w => w.DealerID == request.DealerID); }
              
                var totalRecords = query.Count();
                var filteredRecords = query.Count();
                
                // Validate sort column
                var sortColumn = string.IsNullOrWhiteSpace(request.SortColumn) ? "OrderNo" : request.SortColumn;
                var sortDirection = string.IsNullOrWhiteSpace(request.SortDirection) ? "asc" : request.SortDirection;
                
                // Validate column name against model properties
                var validProperties = typeof(VOnlineReplacementOrderDetails).GetProperties()
                    .Select(p => p.Name)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);

                if (validProperties.Contains(sortColumn))
                {
                    query = query.OrderBy($"{sortColumn} {sortDirection}");
                }
                else
                {
                    // Default fallback sort
                    query = query.OrderByDescending(w => w.OrderDate);
                }

                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.OnlineHSRPReplacementOrderID,
                                                       w.OrderNo,
                                                       sOrderDate = w.OrderDate.ToString("dd MMM, yyyy"),
                                                       w.OEMName,
                                                       w.VehicleNo,
                                                       w.VehicleClassName,
                                                       w.PlateTypeName,
                                                       w.PlateSizeName,
                                                       w.PlateColorName,
                                                       w.ReplacementReasonName,
                                                       w.DealerName,
                                                       w.CustomerName,
                                                       w.CustomerPhoneNo
                                                   }).ToList();

                response.Value = pagedData.OrderByDescending(w => w.OrderNo); ;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = filteredRecords;
                _auditLogger.SaveActionLog("VOnlineReplacementOrderDetails", ActionType.ListData, null, request, null, "OnlineReplacementOrderServiceRepository.GetReplacementOrderList()");

            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "OnlineReplacementOrderServiceRepository.GetReplacementOrderList()");
            }
            return response;
        }
        
        public DataResponse GetReplacementOrderByID(int replacementOrderId)
        {
            DataResponse response = new DataResponse();
            try
            {
                var resultdata = _dbcontext.VOnlineReplacementOrderDetails.FirstOrDefault(w => w.OnlineHSRPReplacementOrderID == replacementOrderId);

                if (resultdata != null)
                {
                    resultdata.sOrderDate = resultdata.OrderDate.ToString("dd MMM, yyyy");
                }

                response.Value = resultdata;

                _auditLogger.SaveActionLog("VOnlineReplacementOrderDetails", ActionType.ListData, null, replacementOrderId, null, "OnlineReplacementOrderServiceRepository.GetReplacementOrderByID()");

            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, ActionType.ListData, "OnlineReplacementOrderServiceRepository.GetReplacementOrderByID()");
            }
            return response;
        }

        public DataResponse ApproveReplacementOrder(ApproveReplacementOrderData request)
        {
            DataResponse response = new DataResponse();

            try
            {
                using var connection = new SqlConnection(
                    _dbcontext.Database.GetConnectionString());

                using var command = new SqlCommand("InsertReplacementOrderToHSRPOrder", connection)
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandTimeout = 60
                };

                command.Parameters.Add("@PK_OnlineHSRPReplacementOrderID", SqlDbType.Int)
                       .Value = request.OnlineHSRPReplacementOrderID;

                command.Parameters.Add("@EmbossingStationID", SqlDbType.Int)
                       .Value = request.EmbossingStationID;

                command.Parameters.Add("@LastUpdatedBy", SqlDbType.Int)
                       .Value = request.LastUpdatedBy;

                connection.Open();
                command.ExecuteNonQuery();

                response.Success = true;
                response.Message = "Replacement order approved successfully";

                _auditLogger.SaveActionLog(
                    "ApproveReplacementOrder()",
                    ActionType.Insert,
                    request.OnlineHSRPReplacementOrderID.ToString(),
                    request,
                    null,
                    "OnlineReplacementOrderServiceRepository"
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

