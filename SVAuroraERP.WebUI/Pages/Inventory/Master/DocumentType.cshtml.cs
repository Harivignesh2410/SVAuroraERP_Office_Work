namespace SVAuroraERP.WebUI.Pages.Inventory.Master
{
    public class DocumentTypeModel : BasePageModel
    {
        private readonly IDocumentTypeServiceRepository _repository = null;
        private readonly IDocumentGroupServiceRepository _grouprepository = null;
        private readonly ILogger<DocumentTypeModel> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private const int PageControlID = (int)Common.Pages.DocumentType; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;

        public DocumentTypeModel(IDocumentTypeServiceRepository respository,
                           ILogger<DocumentTypeModel> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IDocumentGroupServiceRepository grouprepository,
                            IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _grouprepository = grouprepository;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> DocumentGroupList { get; set; } = new List<SelectListItem>();
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
   
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadDocumentGroupList();

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadDocumentGroupList()
        {
            DataResponse dataResponse = new DataResponse();
            DocumentGroupList.Clear();
            dataResponse = _grouprepository.GetDocumentGroup();
            DocumentGroupList = ((List<VDocumentGroup>)dataResponse.Value)
                .OrderBy(o => o.DocumentGroupName)
                .Select(s => new SelectListItem
                {
                    Value = s.DocumentGroupID.ToString(),
                    Text = s.DocumentGroupName
                }).ToList();

            DocumentGroupList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Document Group--" });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] DocumentType DocumentTypeData)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();

            try
            {
                DocumentTypeData.LastUpdatedBy = LoggedUser.UserID;
                DocumentTypeData.LoginAuditID = LoggedUser.LoginAuditID;

                if (DocumentTypeData.DocumentTypeID == 0)
                    resultdata = _repository.Save(DocumentTypeData);
                else if (DocumentTypeData.DocumentTypeID > 0)
                    resultdata = _repository.Update(DocumentTypeData);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetDocumentTypeList(int draw, int start, int length)
        {
            DataResponse dataResponse = new DataResponse();
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

             dataResponse = _repository.GetDocumentType();
            var resultdata=((List<VDocumentType>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.DocumentTypeCode.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.DocumentTypeName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VDocumentType, object> orderByFunc = orderCol switch
            {
                1 => d => d.DocumentTypeCode,
                2 => d => d.DocumentTypeName,
                3 => d => d.DocumentGroupName,
                _ => null  // No sorting for other columns
            };

            if (orderByFunc != null)
            {
                filteredData = orderDir == "asc"
                    ? filteredData.OrderBy(orderByFunc).ToList()
                    : filteredData.OrderByDescending(orderByFunc).ToList();
            }

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

        public JsonResult OnGetDocumentTypeByID(int ID)
        {
            DataResponse dataResponse = new DataResponse();
            dataResponse = _repository.GetByID(ID);

            return new JsonResult(dataResponse.Value);
        }

        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
            string message = string.Empty;
            DataResponse resultdata = new DataResponse();

            try
            {
                resultdata = _repository.Delete(ID, LoggedUser.UserID, LoggedUser.LoginAuditID);

                return new JsonResult(new { result = resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { result = resultdata });
            }
        }
        public JsonResult OnPostDocumentTypeDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                // Validate sort column
                var validColumns = new[] { "DocumentTypeName", "DocumentTypeCode", "DocumentGroupName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "DocumentTypeName";

                dataResponse = _repository.GetDocumentTypeDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading Document Type data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VDocumentType>()
                });
            }
        }
    }
}