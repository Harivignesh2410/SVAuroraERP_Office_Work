namespace SVAuroraERP.Infrastructure.Repositories.Orders.Import
{
    public class ImportOEMServiceRepository : IImportOEMServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        private readonly IErrorLoggerService _errorLoggerService;
        private readonly IAuditLogger _auditLogger;

        public ImportOEMServiceRepository(IErrorLoggerService errorLoggerService,
            SVAuroraERPDbContext dbcontext,
            IAuditLogger auditLogger)
        {
            _errorLoggerService = errorLoggerService;
            _dbcontext = dbcontext;
            _auditLogger = auditLogger;
        }
        public DataResponse Save(ImportOEM request)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                request.LastUpdatedDate = DateTime.UtcNow;
                _dbcontext.ImportOEM.Add(request);
                _dbcontext.SaveChanges();
                _auditLogger.SaveActionLog("ImportOEM", ActionType.Insert, null, request, null, "ImportOEMServiceRepository.Save()");

            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "ImportOEMServiceRepository.Save()");
            }

            return dataResponse;
        }
        public DataResponse GetOEMConfigData()
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var configdata = _dbcontext.OEMConfig.FirstOrDefault();
                dataResponse.Value = configdata;
                _auditLogger.SaveActionLog("OEMConfig", ActionType.ListData, null, null, null, "ImportOEMServiceRepository.GEtOEMConfigData()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, null, "ImportOEMServiceRepository.GEtOEMConfigData()");
            }

            return dataResponse;
        }
        public DataResponse GetImportOEMtoDataTable(ImportOEMFilter request)
        {
            DataResponse response = new DataResponse();
            try
            {
                // Validate and sanitize inputs
                var pageSize = Math.Clamp(request.Length, 1, 100); // Limit page size
                var skip = Math.Max(request.Start, 0);

                IQueryable<VOEMImport> query = _dbcontext.VOEMImport.OrderByDescending(w=>w.ImportOEMID);

                // Apply search filter if provided
                if (!string.IsNullOrWhiteSpace(request.SearchValue))
                {
                    query = query.Where(d => d.CompanyName.Contains(request.SearchValue) ||
                                             d.FileName.Contains(request.SearchValue) 
                                             );
                }
                if (request.StartDate.HasValue)
                    query = query.Where(w => (w.LastUpdatedDate) >= request.StartDate);

                if (request.EndDate.HasValue)
                    query = query.Where(w => w.LastUpdatedDate <= request.EndDate);

                if (request.OEMID > 0) { query=query.Where(w=>w.OEMID == request.OEMID); }
                // Get TOTAL records in database (unfiltered)
                var totalRecords = _dbcontext.VOEMImport.Count();

                // Get FILTERED records count (same as total if no filter applied)
                var filteredRecords = query.Count();

                // Apply sorting 
                query = query.OrderBy($"{request.SortColumn} {request.SortDirection}");

                // Apply paging
                var pagedData = query.Skip(skip).Take(pageSize)
                                                   .Select(w => new
                                                   {
                                                       w.FileName,
                                                       w.CompanyName,
                                                       w.DataRowCount,
                                                       w.InsertedCount,
                                                       w.RemovedCount,
                                                       w.ImportOEMID,
                                                       w.ImportedDate
                                                   }).ToList();


                response.Value = pagedData;
                response.recordsTotal = totalRecords;
                response.recordsFiltered = pagedData.Count;
                _auditLogger.SaveActionLog("VOEMImport", ActionType.ListData, null, request, null, "ImportOEMServiceRepository.GetImportOEMtoDataTable()");

            }
            catch (Exception ex)
            {
                response = _errorLoggerService.LogException(ex, request, "ImportOEMServiceRepository.GetImportOEMtoDataTable()");
            }

            return response;
        }

        public DataResponse ImportOEMData(ImportOEMDData request)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "dbo.ImportOEMData";
                        command.CommandType = CommandType.StoredProcedure;

                        // OUTPUT parameter
                        var fkoemImportIDParam = command.CreateParameter();
                        fkoemImportIDParam.ParameterName = "@FK_OEMImportID";
                        fkoemImportIDParam.Direction = ParameterDirection.Output;
                        fkoemImportIDParam.DbType = DbType.Int32;
                        command.Parameters.Add(fkoemImportIDParam);

                        // Normal parameters
                        command.Parameters.Add(new SqlParameter("@FK_OEMID", request.OEMID));
                        command.Parameters.Add(new SqlParameter("@FileName", request.FileName ?? (object)DBNull.Value));
                        command.Parameters.Add(new SqlParameter("@LastUpdatedBy", request.LastUpdatedBy));

                        // Build DataTable for TVP
                        var table = new DataTable();
                        table.Columns.Add("VendorCode", typeof(string));
                        table.Columns.Add("DealerCode", typeof(string));
                        table.Columns.Add("PONumber", typeof(string));
                        table.Columns.Add("SONumber", typeof(string));
                        table.Columns.Add("VehRegDate", typeof(string));
                        table.Columns.Add("PartNo", typeof(string));
                        table.Columns.Add("VehRegNo", typeof(string));
                        table.Columns.Add("PlateColor", typeof(string));
                        table.Columns.Add("OrderDate", typeof(string));
                        table.Columns.Add("EngineNo", typeof(string));
                        table.Columns.Add("ChassisNo", typeof(string));

                        foreach (var row in request.Exceldata)
                        {
                            table.Rows.Add(
                                row.VendorCode.Trim() ?? "",
                                row.DealerCode.Trim() ?? "",
                                row.PONumber.Trim() ?? "",
                                row.SONumber.Trim() ?? "",
                                row.VehRegDate.Trim() ?? "",
                                row.PartNo.Trim() ?? "",
                                row.VehRegNo.Trim() ?? "",
                                row.PlateColor.Trim() ?? "",
                                row.OrderDate.Trim() ?? "",
                                row.EngineNo.Trim() ?? "",
                                row.chassisNo.Trim() ?? ""   
                            );

                        }

                        var excelDataParam = command.CreateParameter();
                        excelDataParam.ParameterName = "@ExcelData";
                        excelDataParam.SqlDbType = SqlDbType.Structured;
                        excelDataParam.TypeName = "dbo.ExcelDataTableType"; // Must match SQL type name
                        excelDataParam.Value = table;
                        command.Parameters.Add(excelDataParam);

                        // Execute
                        command.ExecuteNonQuery();

                        dataResponse.ID = (int)fkoemImportIDParam.Value; 
                        dataResponse.Message = Constants.SuccessMessage;
                    }
                }

                _auditLogger.SaveActionLog("ImportOEMDData", ActionType.Insert, request.OEMID.ToString(),request,null,"ImportOEMServiceRepository.ImportOEMData()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, request, "ImportOEMServiceRepository.ImportOEMData()");
            }

            return dataResponse;
        }

        public DataResponse DeleteImportOEMData(int PK_OEMImportID, int LastUpdatedBy)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                using (var connection = new SqlConnection(_dbcontext.Database.GetConnectionString()))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = StoredProcedure.DELETEIMPORTEDDATA;
                        command.CommandType = CommandType.StoredProcedure;
                        
                        // Normal parameters
                        command.Parameters.Add(new SqlParameter("@PK_OEMImportID", PK_OEMImportID));
                        command.Parameters.Add(new SqlParameter("@LastUpdatedBy", LastUpdatedBy));

                        // Execute
                        command.ExecuteNonQuery();

                        dataResponse.ID = PK_OEMImportID;
                        dataResponse.Message = Constants.SuccessMessage;
                    }
                }

                _auditLogger.SaveActionLog( "ImportOEMDData",ActionType.Insert, PK_OEMImportID.ToString(), PK_OEMImportID,null,"ImportOEMServiceRepository.DeleteImportOEMData()" );
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, PK_OEMImportID, "ImportOEMServiceRepository.DeleteImportOEMData()");
            }

            return dataResponse;
        }
        public DataResponse GetImportDataByID(int PK_OEMImportID)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                var importData = _dbcontext.VOEMImport.FirstOrDefault(w => w.ImportOEMID == PK_OEMImportID);
                if (importData != null)
                {
                    var transdata = _dbcontext.VOEMImportTrans.Where(w => w.OEMImportID == PK_OEMImportID).ToList();
                    importData.VOEMImportTrans = transdata; 
                }

                dataResponse.Value = importData;

                _auditLogger.SaveActionLog("VOEMImport", ActionType.Select, PK_OEMImportID.ToString(), PK_OEMImportID, null, "ImportOEMServiceRepository.GetImportDataByID()");
            }
            catch (Exception ex)
            {
                dataResponse = _errorLoggerService.LogException(ex, PK_OEMImportID, "ImportOEMServiceRepository.GetImportDataByID()");
            }

            return dataResponse;
        }


    }
}