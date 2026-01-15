using SVAuroraERP.Application.Interfaces.Persistance.Dealer;
using SVAuroraERP.Domain.Dealer;

namespace SVAuroraERP.WebUI.Pages.Dealer
{
    public class HolidayTypeModel : HSRPBasePageModel
    {
        private readonly IHolidayTypeServiceRepository _repository = null;
        private readonly ILogger<HolidayTypeModel> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.HolidayType; // ID for this specific page
        public HolidayTypeModel(IHolidayTypeServiceRepository respository,
                           ILogger<HolidayTypeModel> logger,
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
            Permissions = _permissionrepository.GetPagePermissions((int)HSRPLoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public JsonResult OnPostHolidayTypeToDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Validate sort column
                var validColumns = new[] { "TypeName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "TypeName";

                dataResponse = _repository.GetHolidayTypeToDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading HolidayType data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VHolidayType>()
                });
            }
        }
        public JsonResult OnPostSaveUpdateData([FromBody] HolidayType HolidayType)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                HolidayType.LastUpdatedBy = (int)HSRPLoggedUser.UserID;

                if (HolidayType.HolidayTypeID == 0)
                    resultdata = _repository.Save(HolidayType);
                else if (HolidayType.HolidayTypeID > 0)
                    resultdata = _repository.Update(HolidayType);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetHolidayTypeByID(int ID)
        {
            DataResponse resultdata = _repository.GetHolidayTypeByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse dataResponse = new DataResponse();

            try
            {
                dataResponse = _repository.Delete(ID, (int)HSRPLoggedUser.UserID, (long)HSRPLoggedUser.LoginAuditID);

                return new JsonResult(new { resultdata = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata = dataResponse });
            }
        }

    }
}

