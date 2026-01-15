namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Dispatch
{

    public class PackingTransServiceRepository : IPackingTransServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly ILogger<IPackingTransServiceRepository> _logger;
        private readonly IAuditLogger _auditLogger;
        public PackingTransServiceRepository(SVAuroraERPDbContext dbContext,
                                            IErrorLoggerService errorLoggerService,
                                            IAuditLogger auditLogger,
                                            ILogger<IPackingTransServiceRepository> logger)
        {
            _dbcontext = dbContext;
            _errorLoggerService = errorLoggerService;
            _auditLogger = auditLogger;
            _logger = logger;
        }
        public int AddPackingTrans(PackingTrans request)
        {
            int id = 0;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.INSERTPACKINGTRANS;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var fkPackingIDParam = command.CreateParameter();
                        fkPackingIDParam.ParameterName = "@FK_PackingID";
                        fkPackingIDParam.Value = request.PackingID;

                        var StartingLaserNoParam = command.CreateParameter();
                        StartingLaserNoParam.ParameterName = "@StartingLaserNo";
                        StartingLaserNoParam.Value = request.StartingLaserNo;

                        var EndingLaserNoParam = command.CreateParameter();
                        EndingLaserNoParam.ParameterName = "@EndingLaserNo";
                        EndingLaserNoParam.Value = request.EndingLaserNo;

                        var quantityParam = command.CreateParameter();
                        quantityParam.ParameterName = "@Quantity";
                        quantityParam.Value = request.Quantity;

                        var LaserNoPrefixParam = command.CreateParameter();
                        LaserNoPrefixParam.ParameterName = "@LaserNoPrefix";
                        LaserNoPrefixParam.Value = request.LaserNoPrefix;

                        command.Parameters.Add(fkPackingIDParam);
                        command.Parameters.Add(StartingLaserNoParam);
                        command.Parameters.Add(EndingLaserNoParam);
                        command.Parameters.Add(quantityParam);
                        command.Parameters.Add(LaserNoPrefixParam);

                        id = command.ExecuteNonQuery();
                    }
                }
                _auditLogger.SaveActionLog("PackingTrans", ActionType.Insert, request.PackingID.ToString(), request, null, "PackingTransServiceRepository.AddPackingTrans()");

                return id;
            }
            catch (Exception ex)
            {
                _logger.LogError($"PackingTransServiceRepository.AddPackingTrans(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return id;
            }
        }
        public List<VPackingTrans> GetPackingTransByID(int PackingID)
        {
            try
            {
                var resultdata = _dbcontext.VPackingTrans.Where(w => w.PackingID == PackingID).ToList();
                _auditLogger.SaveActionLog("VPackingTrans", ActionType.ListData, PackingID.ToString(), resultdata, null, "StockReportServiceRepository.GetPackingTransByID()");
                return resultdata;
            }
            catch (Exception ex)
            {
                _logger.LogError($"StockReportServiceRepository.GetPackingTransByID(). Error: {ex.Message}, StackTrace: {ex.StackTrace}");
                return null;
            }
        }
    }
}
