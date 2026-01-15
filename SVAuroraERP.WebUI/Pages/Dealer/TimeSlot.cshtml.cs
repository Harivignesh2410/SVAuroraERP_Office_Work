using SVAuroraERP.Application.Interfaces.Persistance.Dealer;
using SVAuroraERP.Domain.Dealer;

namespace SVAuroraERP.WebUI.Pages.Dealer
{
    public class TimeSlotModel : HSRPBasePageModel
    {
        private readonly ITimeSlotServiceRepository _repository = null;
        private readonly ILogger<TimeSlotModel> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.TimeSlot; // ID for this specific page
        public TimeSlotModel(ITimeSlotServiceRepository repository,
                           ILogger<TimeSlotModel> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IPermissionServiceRepository permissionService)
        {
            _repository = repository;
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
        public JsonResult OnPostTimeSlotToDataTable([FromForm] DataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Validate sort column
                var validColumns = new[] { "SlotName", "StartTime", "EndTime" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "StartTime";

                dataResponse = _repository.GetTimeSlotToDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading TimeSlot data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VTimeSlot>()
                });
            }
        }
        public JsonResult OnPostSaveUpdateData([FromBody] TimeSlot TimeSlot)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                TimeSlot.LastUpdatedBy = (int)HSRPLoggedUser.UserID;

                if (TimeSlot.TimeSlotID == 0)
                    resultdata = _repository.Save(TimeSlot);
                else if (TimeSlot.TimeSlotID > 0)
                    resultdata = _repository.Update(TimeSlot);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetTimeSlotByID(int ID)
        {
            DataResponse resultdata = _repository.GetTimeSlotByID(ID);

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