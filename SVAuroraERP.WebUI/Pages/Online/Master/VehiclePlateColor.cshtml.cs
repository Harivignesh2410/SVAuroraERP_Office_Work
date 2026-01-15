namespace SVAuroraERP.WebUI.Pages.Online.Master
{
    public class VehiclePlateColorModel : BasePageModel
    {
        private readonly IVehiclePlateColorServiceRepository _repository = null;
        private readonly ILogger<VehiclePlateColor> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.VehiclePlateColor; // ID for this specific page

        public VehiclePlateColorModel(IVehiclePlateColorServiceRepository respository,
                           ILogger<VehiclePlateColor> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view          
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public JsonResult OnPostSaveUpdateData([FromBody] VehiclePlateColor VehiclePlateColor)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                VehiclePlateColor.LastUpdatedBy = LoggedUser.UserID;

                if (VehiclePlateColor.VehiclePlateColorID == 0)
                    resultdata = _repository.Save(VehiclePlateColor);
                else if (VehiclePlateColor.VehiclePlateColorID > 0)
                    resultdata = _repository.Update(VehiclePlateColor);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetVehiclePlateColourByID(int ID)
        {
            DataResponse resultdata = _repository.GetVehiclePlateColorByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse dataResponse = new DataResponse();

            try
            {
                dataResponse = _repository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(new { resultdata = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata = dataResponse });
            }
        }
        public JsonResult OnPostVehiclePlateColourList([FromForm] DataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "VehiclePlateColorName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "VehiclePlateColorName";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetVehiclePlateColorDataTableList(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Vehicle Plate Color data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VVehiclePlateColor>()
                });
            }
        }
    }
}