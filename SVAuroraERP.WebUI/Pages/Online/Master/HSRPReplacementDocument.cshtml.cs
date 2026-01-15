namespace SVAuroraERP.WebUI.Pages.Online.Master
{
    public class HSRPReplacementDocumentModel : BasePageModel
    {
        private readonly IHSRPReplacementDocumentServiceRepository _repository = null;
        private readonly ILogger<HSRPReplacementDocumentServiceRepository> _logger = null;
        private readonly IHSRPReplacementReasonServiceRepository _hSRPReplacementReasonServiceRepository;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.HSRPReplacementDocument; // ID for this specific page

        public HSRPReplacementDocumentModel(IHSRPReplacementDocumentServiceRepository respository,
                           ILogger<HSRPReplacementDocumentServiceRepository> logger,
                           IAntiforgery antiforgery,
                           IHSRPReplacementReasonServiceRepository hSRPReplacementReasonServiceRepository,
                           SessionService sessionService, IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _hSRPReplacementReasonServiceRepository = hSRPReplacementReasonServiceRepository;
            _permissionrepository = permissionService;
        } 
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> ReasonList { get; set; } = new List<SelectListItem>();
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
     
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadReasonList();

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadReasonList()
        {
            DataResponse dataResponse = new DataResponse();
            ReasonList.Clear();
            dataResponse = _hSRPReplacementReasonServiceRepository.GetHSRPReplacementReason();
            ReasonList = ((List<VHSRPReplacementReason>)dataResponse.Value)
                .OrderBy(o => o.ReplacementReasonName)
                .Select(s => new SelectListItem
                {
                    Value = s.HSRPReplacementReasonID.ToString(),
                    Text = s.ReplacementReasonName
                }).ToList();

            ReasonList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Reason --" });
        }
        public JsonResult OnGetHSRPReplacementDocumentList(int draw, int start, int length)
        {
            DataResponse dataResponse = null;
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = (_repository.GetHSRPReplacementDocument());
            var resultdata = ((List<VHSRPReplacementDocument>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.ReplacementReasonName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.Code ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VHSRPReplacementDocument, object> orderByFunc = orderCol switch
            {
                1 => d => d.Code,
                2 => d => d.ReplacementReasonName,
                3 => d => d.ReplacementDocumentName,
                _ => null  // No sorting for other columns
            };

            // Paginate the filtered data
            var paginatedData = filteredData.Skip(start).Take(length).ToList();

            // Return the JSON result
            return new JsonResult(new
            {
                draw = draw,
                recordsTotal = resultdata.Count,
                recordsFiltered = filteredData.Count,
                data = paginatedData
            });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] HSRPReplacementDocument HSRPReplacementDocument)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                HSRPReplacementDocument.LastUpdatedBy = LoggedUser.UserID;

                if (HSRPReplacementDocument.HSRPReplacementDocumentID == 0)
                    resultdata = _repository.Save(HSRPReplacementDocument);
                else if (HSRPReplacementDocument.HSRPReplacementDocumentID > 0)
                    resultdata = _repository.Update(HSRPReplacementDocument);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetHSRPReplacementDocumentByID(int ID)
        {
            DataResponse resultdata = _repository.GetHSRPReplacementDocumentByID(ID);

            return new JsonResult(resultdata);
        }
        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse dataResponse = new DataResponse();

            try
            {
                dataResponse = _repository.Delete(ID, LoggedUser.UserID);

                return new JsonResult(new { resultdata = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata = dataResponse });
            }
        }
        public JsonResult OnPostHSRPReplacementDocumentList([FromForm] DataTableRequest dataTableRequest)
        {
            try
            {
                // Validate sort column
                var validColumns = new[] { "ReplacementDocumentName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "ReplacementDocumentName";

                DataResponse dataResponse = new DataResponse();
                dataResponse = _repository.GetHSRPReplacementDocumentDataTableList(dataTableRequest);

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
                _logger.LogError(ex, "Error loading State data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VHSRPReplacementDocument>()
                });
            }
        }
    }
}