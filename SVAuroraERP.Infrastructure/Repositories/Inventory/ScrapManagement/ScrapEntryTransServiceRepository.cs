using SVAuroraERP.Domain.Inventory.ScrapManagement;

namespace SVAuroraERP.Infrastructure.Repositories.Inventory.ScrapManagement
{
    public class ScrapEntryTransServiceRepository: IScrapEntryTransServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IAuditLogger _auditLogger;
        private readonly IErrorLoggerService _errorLoggerService;
        public ScrapEntryTransServiceRepository(SVAuroraERPDbContext dbcontext,
                                              IAuditLogger auditLogger,
                                              IErrorLoggerService errorLoggerService)
        {
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
            _errorLoggerService = errorLoggerService;
        }

        public DataResponse SaveScrapEntryTransDetails(ScrapEntryTrans request)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = new SqlCommand(Domain.StoredProcedure.INSERTORUPDATESCRAPENTRYTRANS, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        var pkParam = new SqlParameter("@PK_ScrapEntryTransID", SqlDbType.Int)
                        {
                            Direction = ParameterDirection.InputOutput,
                            Value = request.ScrapEntryTransID > 0 ? request.ScrapEntryTransID : 0
                        };
                        command.Parameters.Add(pkParam);

                        // Input parameters
                        command.Parameters.AddWithValue("@FK_ScrapEntryID", request.ScrapEntryID);
                        command.Parameters.AddWithValue("@FK_ComponentTypeID", request.ComponentTypeID);
                        command.Parameters.AddWithValue("@FK_SizeID", request.SizeID);
                        command.Parameters.AddWithValue("@SoldQty", request.SoldQty);
                        command.Parameters.AddWithValue("@LastUpdatedBy", request.LastUpdatedBy);

                        // Execute SP
                        command.ExecuteNonQuery();

                        // Get the new or updated ID from the SP
                        int returnedId = Convert.ToInt32(pkParam.Value);
                        dataResponse.ID = returnedId;
                        dataResponse.Success = true;
                        dataResponse.Error = false;

                        if (request.ScrapEntryTransID > 0)
                        {
                            dataResponse.Message = "Scrap Entry Transaction updated successfully.";
                        }
                        else
                        {
                            dataResponse.Message = "Scrap Entry Transaction created successfully.";
                        }

                        // ✅ Log action type dynamically
                        var actionType = request.ScrapEntryTransID > 0 ? ActionType.Update : ActionType.Insert;
                        _auditLogger.SaveActionLog(
                            "ScrapEntryTrans",
                            actionType,
                            null,
                            request,
                            null,
                            "ScrapEntryTransServiceRepository.SaveScrapEntryTransDetails()"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(
                    ex,
                    request,
                    "ScrapEntryTransServiceRepository.SaveScrapEntryTransDetails()"
                );
            }

            return dataResponse;
        }

    }
}
