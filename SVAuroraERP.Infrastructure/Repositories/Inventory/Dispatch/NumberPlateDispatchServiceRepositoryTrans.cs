namespace SVAuroraERP.Infrastructure.Repositories.Inventory.Dispatch
{
    public class NumberPlateDispatchServiceRepositoryTrans : INumberPlateDispatchServiceRepositoryTrans
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public NumberPlateDispatchServiceRepositoryTrans(SVAuroraERPDbContext dbcontext,
                                                            IAuditLogger auditLogger,
                                                            IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;

        }

        public int Save(NumberPlateDispatchTrans request)
        {
            int id = 0;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.INSERTNUMBERPLATEDISPATCHTRANS;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var fkPackingIDParam = command.CreateParameter();
                        fkPackingIDParam.ParameterName = "@FK_PackingID";
                        fkPackingIDParam.Value = request.PackingID;

                        var fkNumberPlateDispatchIDParam = command.CreateParameter();
                        fkNumberPlateDispatchIDParam.ParameterName = "@FK_NumberPlateDispatchID";
                        fkNumberPlateDispatchIDParam.Value = request.NumberPlateDispatchID;

                        command.Parameters.Add(fkPackingIDParam);
                        command.Parameters.Add(fkNumberPlateDispatchIDParam);

                        id = command.ExecuteNonQuery();
                    }
                }
                _auditLogger.SaveActionLog("NumberPlateDispatchTrans", ActionType.Insert, request.NumberPlateDispatchID.ToString(), request, null, "NumberPlateDispatchServiceRepositoryTrans.Save()");
                return id;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, request, "NumberPlateDispatchServiceRepositoryTrans.Save()");
            }

            return id;
        }
        public bool DeleteTrans(int NumberPlateDispatchID)
        {
            bool IsSuccess = false;
            int id = 0;
            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.DELETENUMBERPLATEDISPATCHTRANS;
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        var fkNumberPlateDispatchIDParam = command.CreateParameter();
                        fkNumberPlateDispatchIDParam.ParameterName = "@FK_NumberPlateDispatchID";
                        fkNumberPlateDispatchIDParam.Value = NumberPlateDispatchID;

                        command.Parameters.Add(fkNumberPlateDispatchIDParam);

                        id = command.ExecuteNonQuery();
                        if (id > 0)
                        {
                            IsSuccess = true;
                        }
                    }
                }
                _auditLogger.SaveActionLog("NumberPlateDispatchTrans", ActionType.Delete, NumberPlateDispatchID.ToString(), NumberPlateDispatchID, null, "NumberPlateDispatchServiceRepositoryTrans.DeleteTrans()");
                return IsSuccess;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "NumberPlateDispatchServiceRepositoryTrans.DeleteTrans()");
            }
            return IsSuccess;
        }

        public List<VNumberPlateDispatchTrans> GetNumberPlateDispatchTransByID(int NumberPlateDispatchID)
        {

            var resultdata = _dbcontext.VNumberPlateDispatchTrans.Where(w => w.NumberPlateDispatchID == NumberPlateDispatchID).ToList();
            try
            {
                foreach (var item in resultdata)
                {
                    item.PackingTrans = _dbcontext.VPackingTrans
                        .Where(w => w.PackingID == item.PackingID)
                        .ToList();
                }
                _auditLogger.SaveActionLog("VNumberPlateDispatchTrans", ActionType.ListData, NumberPlateDispatchID.ToString(), null, null, "NumberPlateDispatchServiceRepositoryTrans.GetNumberPlateDispatchTransByID()");
                return resultdata;
            }
            catch (Exception ex)
            {
                var response = _errorLoggerService.LogException(ex, null, "NumberPlateDispatchServiceRepositoryTrans.GetNumberPlateDispatchTransByID()");

                return resultdata;
            }
        }

    }
}
