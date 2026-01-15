namespace SVAuroraERP.WebUI.Pages.HR
{
    public class EmployeeModel : BasePageModel
    {
        private readonly IEmployeeServiceRepository _repository = null;
        private readonly IDesignationServiceRepository _designationrespository = null;
        private readonly ILogger<Employee> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.Employee; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;
        public EmployeeModel(IEmployeeServiceRepository respository,
                           ILogger<Employee> logger,
                           IAntiforgery antiforgery, IPermissionServiceRepository permissionService, IDesignationServiceRepository designationrespository)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
            _designationrespository = designationrespository;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> DesignationList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> BloodGroupList { get; set; } = new List<SelectListItem>();
        string DateFormat = "dd/MM/yyyy";
        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadDesignationList();
            LoadBloodGroupList();
        }
        public void LoadDesignationList()
        {
            DataResponse dataresponse = new DataResponse();
            DesignationList.Clear();
            dataresponse = _designationrespository.GetDesignation();
            DesignationList=((List<VDesignation>)dataresponse.Value)
                .OrderBy(o => o.DesignationName)
                .Select(s => new SelectListItem
                {
                    Value = s.DesignationID.ToString(),
                    Text = s.DesignationName
                }).ToList();

            DesignationList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Designation--" });
        }
        public void LoadBloodGroupList()
        {
            DataResponse dataResponse = new DataResponse();
            BloodGroupList.Clear();
            dataResponse = _repository.GetBloodGroupList();
            BloodGroupList=((List<BloodGroup>)dataResponse.Value)
                .OrderBy(o => o.Blood)
                .Select(s => new SelectListItem
                {
                    Value = s.BloodGroupID.ToString(),
                    Text = s.Blood
                }).ToList();

            BloodGroupList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Blood Group--" });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] Employee EmpData)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();
            try
            {
                EmpData.DOB = ConvertDate(EmpData.sDOB);
                EmpData.FatherDOB = ConvertDate(EmpData.sFatherDOB);
                EmpData.MotherDOB = ConvertDate(EmpData.sMotherDOB);
                EmpData.SpouseDOB = ConvertDate(EmpData.sSpouseDOB);
                EmpData.AnniversaryDate = ConvertDate(EmpData.sAnniversaryDate);
                EmpData.ChildOneDOB = ConvertDate(EmpData.sChildOneDOB);
                EmpData.ChildTwoDOB = ConvertDate(EmpData.sChildTwoDOB);
                EmpData.LastUpdatedBy = LoggedUser.UserID;
                EmpData.LastUpdatedDate = DateTime.UtcNow;
                EmpData.LoginAuditID = LoggedUser.LoginAuditID;

                if (EmpData.BloodGroupID == 0) EmpData.BloodGroupID = null;
                if (EmpData.EmergencyRelationshipContactID == 0) EmpData.EmergencyRelationshipContactID = null;

                if (EmpData.EmployeeID == 0)
                    resultdata = _repository.Save(EmpData);
                else if (EmpData.EmployeeID > 0)
                    resultdata = _repository.Update(EmpData);

                return new JsonResult(new { resultdata});
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata });
            }
        }
        private DateTime? ConvertDate(string sdate)
        {

            DateTime? dtConvertedDate = null;
            if (DateTime.TryParseExact(sdate, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDt))
            {
                dtConvertedDate = parsedDt;
            }

            return dtConvertedDate;
        }

        public JsonResult OnGetEmployeeByID(int EmployeeID)
        {
            DataResponse dataResponse = _repository.GetByID(EmployeeID);

            if (dataResponse.Value != null)
            {
                var data = (VEmployee)dataResponse.Value;

                // Format date fields
                data.sDOB = data.DOB?.ToString("yyyy-MM-dd");
                data.sFatherDOB = data.FatherDOB?.ToString(DateFormat);
                data.sMotherDOB = data.MotherDOB?.ToString(DateFormat);
                data.sSpouseDOB = data.SpouseDOB?.ToString(DateFormat);
                data.sAnniversaryDate = data.AnniversaryDate?.ToString(DateFormat);
                data.sChildOneDOB = data.ChildOneDOB?.ToString(DateFormat);
                data.sChildTwoDOB = data.ChildTwoDOB?.ToString(DateFormat);

                return new JsonResult(new { success = true, data });
            }
            else
            {
                return new JsonResult(new
                {
                    success = false,
                    resultdata = dataResponse
                });
            }
        }

        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
           DataResponse resultdata = new DataResponse();

            try
            {
                resultdata = _repository.Delete(ID);

                return new JsonResult(new { resultdata});
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata });
            }
        }
        public JsonResult OnPostEmployeeDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Validate sort column
                var validColumns = new[] { "DesignationName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "DesignationName";

                dataResponse = _repository.GetEmployeeDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Box data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VEmployee>()
                });
            }
        }

    }
}
