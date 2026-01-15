namespace SVAuroraERP.WebUI.Pages.Inventory.ScrapManagement
{
    public class ScrapEntryModel : BasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.ScrapEntry;
        private readonly IPermissionServiceRepository _permissionrepository;
        private readonly IScrapEntryServiceRepository _repository;
        private readonly ILogger<ScrapEntryModel> _logger;

        public ScrapEntryModel(IAntiforgery antiforgery,
                               IPermissionServiceRepository permissionrepository,
                               IScrapEntryServiceRepository repository,
                               ILogger<ScrapEntryModel> logger)
        {
            _antiforgery = antiforgery;
            _permissionrepository = permissionrepository;
            _repository = repository;
            _logger = logger;
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
        public JsonResult OnPostScrapEntryList([FromForm] DataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "ScrapEntryNo" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "ScrapEntryNo";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetScrapDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Scrap Entry data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VScrapEntry>()
                });
            }
        }
        public JsonResult OnPostAvailableScrapStock([FromBody] ScrapDataParameter request)
        {
            string message = string.Empty;
            DataResponse dataResponse = new DataResponse();
            try
            {
                dataResponse = _repository.GetScrapDataByComponentTypeID(request);
                return new JsonResult(new { result = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
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
        public JsonResult OnPostSaveUpdateData([FromBody] ScrapEntry ScrapEntry)
        {
            string message = string.Empty;
            DataResponse dataResponse = new DataResponse();

            try
            {
                ScrapEntry.ScrapDate = ConvertDateonly(ScrapEntry.sScrapDate);
                ScrapEntry.LastUpdatedBy = LoggedUser.UserID;

                foreach (ScrapEntryTrans scrapEntryTrans in ScrapEntry.ScrapEntryTransList)
                {
                    scrapEntryTrans.LastUpdatedBy = LoggedUser.UserID;
                }
              
                    dataResponse = _repository.Save(ScrapEntry);
               

                return new JsonResult(new { dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        public JsonResult OnPostDeleteData([FromBody] int id)
        {
            DataResponse dataResponse = new DataResponse();
            string message = string.Empty;

            try
            {
                dataResponse = _repository.Delete(id, LoggedUser.UserID);
                return new JsonResult(new { result = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { dataResponse });
            }
        }

        public JsonResult OnGetScrapEntryByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            dataResponse = _repository.GetScrapEntryByID(ID);

            return new JsonResult(new { result = dataResponse });
        }

    }
}
