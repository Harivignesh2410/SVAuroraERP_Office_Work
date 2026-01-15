namespace SVAuroraERP.Infrastructure.Repositories.Authentication
{
    public class HsrpDashboardServiceRepository : IHsrpDashboardServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;
        public HsrpDashboardServiceRepository(IErrorLoggerService errorLoggerService,
                                            SVAuroraERPDbContext dbcontext,
                                            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public async Task<DataResponse> GetHsrpDashboardAsync(HsrpDashboardRequest request)
        {
            DataResponse response = new DataResponse();

            var dataResult = GetHsrpDashboardData(request);

            // OEM Orders (Status wise)
            var oemOrders = dataResult.OEMOrders?
                .ToList<HsrpDashboard>() ?? new List<HsrpDashboard>();

            // Online Orders (Status wise)
            var onlineOrders = dataResult.OnlineOrders?
                .ToList<HsrpDashboard>() ?? new List<HsrpDashboard>();

            // Summary Counts
            var summary = dataResult.SummaryCounts?
                .ToList<SummaryCount>() ?? new List<SummaryCount>();

            response.Value = new
            {
                OEMOrders = oemOrders,
                OnlineOrders = onlineOrders,
                Summary = summary
            };

            response.Count = summary.Sum(x => x.TotalOrders);
            response.Message = Constants.SuccessMessage;

            return response;
        }
        private HsrpDashboardDataSet GetHsrpDashboardData(HsrpDashboardRequest request)
        {
            var result = new HsrpDashboardDataSet();

            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                using (var command = new SqlCommand(StoredProcedure.GETHSRPDASHBOARD, connection))

                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@FK_UserID", request.UserID ?? 0);

                    var adapter = new SqlDataAdapter(command);
                    var dataSet = new DataSet();

                   adapter.Fill(dataSet);

                    if (dataSet.Tables.Count > 0)
                        result.OEMOrders = dataSet.Tables[0];

                    if (dataSet.Tables.Count > 1)
                        result.OnlineOrders = dataSet.Tables[1];

                    if (dataSet.Tables.Count > 2)
                        result.SummaryCounts = dataSet.Tables[2];
                }
            }
            catch (Exception ex)
            {
                _errorLoggerService.LogException(ex, request, "HsrpDashboardServiceRepository.GetHsrpDashboardData()");
            }

            return result;
        }

    }
}
