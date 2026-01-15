namespace SVAuroraERP.WebUI.Pages.Orders.Import
{
    public class ImportOEMDataModel : HSRPBasePageModel
    {
        private readonly IHSRPUserServiceRepository _staterepository = null;
        private readonly ILogger<DistrictServiceRepository> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IImportOEMServiceRepository _importOEMServiceRepository;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.ImportOEMData; // ID for this specific page
        public ImportOEMDataModel(
            IHSRPUserServiceRepository stateRepository,
                           ILogger<DistrictServiceRepository> logger,
                           IAntiforgery antiforgery,
                                 IPermissionServiceRepository permissionService,
                           IImportOEMServiceRepository importOEMServiceRepository
                           )
        {
            _staterepository = stateRepository;
            _logger = logger;
            _antiforgery = antiforgery;
            _importOEMServiceRepository = importOEMServiceRepository;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<string> ValidationErrors { get; set; }
        public int LoggedUserID { get; set; }

        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions((int)HSRPLoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");
            LoggedUserID = (int)HSRPLoggedUser.HSRPUserID;

            if (HSRPLoggedUser.HSRPUserTypeID != (byte)Common.HSRPUserType.EmbossingStation)
            {
                return RedirectToPage("/Orders/ViewOrders/ViewAllOrder");
            }

            // Normal logic here
            return Page();
        }
        public static DateOnly? ConvertDateonly(string? sdate)
        {
            if (string.IsNullOrWhiteSpace(sdate))
                return null;

            string[] formats = {
                                    "dd/MM/yyyy",    // expected from frontend
                                    "yyyy-MM-dd",    // fallback for ISO
                                    "d MMM, yyyy"    // if flatpickr default used
                                };

            foreach (var format in formats)
            {
                if (DateOnly.TryParseExact(sdate, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedDate))
                {
                    return parsedDate;
                }
            }

            return null;
        }
        public JsonResult OnPostImportDataListData([FromForm] ImportOEMFilter dataTableRequest)
        {
            try
            {
                dataTableRequest.StartDate = ConvertDateonly(dataTableRequest.sStartDate)?.ToDateTime(TimeOnly.MinValue);
                dataTableRequest.EndDate = ConvertDateonly(dataTableRequest.sEndDate)?.ToDateTime(TimeOnly.MinValue);
                // Validate sort column
                var validColumns = new[] { "CompanyName", "FileName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "CompanyName";

                DataResponse dataResponse = new DataResponse();

                if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.OEM)
                {
                    dataTableRequest.OEMID = HSRPLoggedUser.HSRPUserID;
                }

                dataResponse = _importOEMServiceRepository.GetImportOEMtoDataTable(dataTableRequest);

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = dataResponse.recordsTotal,
                    recordsFiltered = dataResponse.recordsFiltered,
                    data = dataResponse.Value
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Vendor data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VOEMImport>()
                });
            }
        }
        public async Task<JsonResult> OnPostOEMImportDataFromExcel(IFormFile ImportExcelFile, int OEMID)
        {
            DataResponse importexceldata = new DataResponse();
            ImportOEMDData importOEMRecord = new ImportOEMDData
            {
                OEMID = OEMID,
                FileName = ImportExcelFile?.FileName,
                LastUpdatedBy = (int)HSRPLoggedUser.UserID,
                Exceldata = new List<Exceldata>()
            };

            DataResponse configdata = _importOEMServiceRepository.GetOEMConfigData();
            var oemConfig = configdata.Value as OEMConfig;

            int OEM_TVSID = oemConfig.TVSOEMID;
            int OEM_SARAVANAENGINEERINGWORKSID = oemConfig.SaravanaEngOEMID;
            int EROYCEMOTORSINDIA = oemConfig.EroyceMotorsOEMID;

            if (ImportExcelFile == null || ImportExcelFile.Length == 0)
            {
                importexceldata.Success = false;
                importexceldata.Message = "Please select an Excel file.";
                return new JsonResult(new { result = false, data = importexceldata });
            }

            string extension = Path.GetExtension(ImportExcelFile.FileName).ToLower();
            if (!(extension == ".xls" || extension == ".xlsx"))
            {
                importexceldata.Success = false;
                importexceldata.Message = "Please upload a valid Excel file (.xls or .xlsx)";
                return new JsonResult(new { result = false, data = importexceldata });
            }

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            DataTable dtexcelRecords = new DataTable();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await ImportExcelFile.CopyToAsync(stream);
                    stream.Position = 0;

                    IExcelDataReader reader = extension == ".xls"
                        ? ExcelReaderFactory.CreateBinaryReader(stream)
                        : ExcelReaderFactory.CreateOpenXmlReader(stream);

                    var dsexcelRecords = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });
                    reader.Close();

                    if (dsexcelRecords.Tables.Count > 0 && dsexcelRecords.Tables[0].Rows.Count > 0)
                        dtexcelRecords = dsexcelRecords.Tables[0];
                }

                if (dtexcelRecords == null || dtexcelRecords.Rows.Count == 0)
                {
                    importexceldata.Success = false;
                    importexceldata.Message = "No data found in Excel file.";
                    return new JsonResult(new { result = false, data = importexceldata });
                }

                // OEM column definitions
                string TVSExcelColumnName = "VENDOR CODE,DEALER CODE,TVSM PO NO,TVSM SO NO,VEH REG DATE,PART NO,HSRP VEH REG,COLOUR,ORDER DATE,FRAME NO,ENGINE NO";
                string SaravanaEngExcelColumnName = "vendor_code,dealer_code,po_no,veh_reg_date,part_no,veh_reg_no,plate_color,order_dt,chassis_no,engine_no";
                string EROYCEMOTORSINDIAExcelColumnName = "vendor_code,dealer_code,po_no,veh_reg_date,part_no,veh_reg_no,plate_color,order_dt,chassis_no,engine_no";

                int insertedCount = 0;

                // Validate and process OEM-specific data
                if (OEMID == OEM_TVSID && ValidateColumns(dtexcelRecords, TVSExcelColumnName))
                {
                    importOEMRecord.Exceldata = ProcessTVSData(dtexcelRecords);
                    insertedCount = importOEMRecord.Exceldata.Count;
                }
                else if (OEMID == OEM_SARAVANAENGINEERINGWORKSID && ValidateColumns(dtexcelRecords, SaravanaEngExcelColumnName))
                {
                    importOEMRecord.Exceldata = ProcessSaravanaData(dtexcelRecords);
                    insertedCount = importOEMRecord.Exceldata.Count;
                }
                else if (OEMID == EROYCEMOTORSINDIA && ValidateColumns(dtexcelRecords, EROYCEMOTORSINDIAExcelColumnName))
                {
                    importOEMRecord.Exceldata = ProcessERoyceData(dtexcelRecords);
                    insertedCount = importOEMRecord.Exceldata.Count;
                }
                else
                {
                    string oemName = OEMID == OEM_TVSID ? "TVS" :
                                   OEMID == OEM_SARAVANAENGINEERINGWORKSID ? "Saravana Engineering Works" :
                                   "E Royce Motors";

                    importexceldata.Success = false;
                    importexceldata.Message = $"Excel file doesn't have the required columns for {oemName}.";
                    return new JsonResult(new { result = false, data = importexceldata });
                }

                importexceldata = _importOEMServiceRepository.ImportOEMData(importOEMRecord);
                return new JsonResult(new { result = importexceldata.Success, data = importexceldata });

            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
        }

        private List<Exceldata> ProcessTVSData(DataTable dt)
        {
            var list = new List<Exceldata>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Exceldata
                {
                    VendorCode = row["VENDOR CODE"]?.ToString(),
                    DealerCode = row["DEALER CODE"]?.ToString(),
                    PONumber = row["TVSM PO NO"]?.ToString(),
                    SONumber = row["TVSM SO NO"]?.ToString(),
                    VehRegDate = row["VEH REG DATE"]?.ToString(),
                    PartNo = row["PART NO"]?.ToString(),
                    VehRegNo = row["HSRP VEH REG"]?.ToString(),
                    PlateColor = row["COLOUR"]?.ToString(),
                    OrderDate = row["ORDER DATE"]?.ToString(),
                    chassisNo = row["FRAME NO"]?.ToString(),
                    EngineNo = row["ENGINE NO"]?.ToString()
                });
            }
            return list;
        }

        private List<Exceldata> ProcessSaravanaData(DataTable dt)
        {
            var list = new List<Exceldata>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Exceldata
                {
                    VendorCode = row["vendor_code"]?.ToString(),
                    DealerCode = row["dealer_code"]?.ToString(),
                    PONumber = row["po_no"]?.ToString(),
                    VehRegDate = row["veh_reg_date"]?.ToString(),
                    PartNo = row["part_no"]?.ToString(),
                    VehRegNo = row["veh_reg_no"]?.ToString(),
                    PlateColor = row["plate_color"]?.ToString(),
                    OrderDate = row["order_dt"]?.ToString(),
                    chassisNo = row["chassis_no"]?.ToString(),
                    EngineNo = row["engine_no"]?.ToString()
                });
            }
            return list;
        }

        private List<Exceldata> ProcessERoyceData(DataTable dt)
        {
            var list = new List<Exceldata>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new Exceldata
                {
                    VendorCode = row["vendor_code"]?.ToString(),
                    DealerCode = row["dealer_code"]?.ToString(),
                    PONumber = row["po_no"]?.ToString(),
                    VehRegDate = row["veh_reg_date"]?.ToString(),
                    PartNo = row["part_no"]?.ToString(),
                    VehRegNo = row["veh_reg_no"]?.ToString(),
                    PlateColor = row["plate_color"]?.ToString(),
                    OrderDate = row["order_dt"]?.ToString(),
                    chassisNo = row["chassis_no"]?.ToString(),
                    EngineNo = row["engine_no"]?.ToString()
                });
            }
            return list;
        }

        private bool ValidateColumns(DataTable excelData, string requiredColumns)
        {
            if (excelData == null || excelData.Columns.Count == 0)
                return false;

            List<string> requiredColumnsList = requiredColumns.Split(',')
                .Select(col => col.Trim().ToUpper())
                .ToList();

            List<string> actualColumns = excelData.Columns.Cast<DataColumn>()
                .Select(col => col.ColumnName.Trim().ToUpper())
                .ToList();

            foreach (string requiredCol in requiredColumnsList)
            {
                if (!actualColumns.Contains(requiredCol))
                {
                    return false;
                }
            }

            return true;
        }

        public JsonResult OnPostDeleteData([FromBody] int PK_OEMImportID)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                if (PK_OEMImportID <= 0)
                {
                    dataResponse.Success = false;
                    dataResponse.Message = "Invalid Request";
                    return new JsonResult(new { result = false, data = dataResponse });
                }
                dataResponse = _importOEMServiceRepository.DeleteImportOEMData(PK_OEMImportID, HSRPLoggedUser.HSRPUserID);
                return new JsonResult(new { result = dataResponse });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Import OEM data");
                dataResponse.Success = false;
                dataResponse.Message = "An error occurred while processing your request.";
                return new JsonResult(new { result = dataResponse });
            }
        }
        public JsonResult OnGetImportDataByID(int ID)
        {
            var resultSet = _importOEMServiceRepository.GetImportDataByID(ID);

            return new JsonResult(new
            {
                success = true,
                data = resultSet
            });
        }
    }
}
