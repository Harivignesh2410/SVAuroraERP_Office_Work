namespace SVAuroraERP.WebUI.Pages.Inventory.Dispatch
{
    public class DeliveryAcknowledgementModel : BasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private readonly INumberPlateDispatchServiceRepository _numberPlateDispatchServiceRepository;
        private readonly ILogger<DeliveryAcknowledgementModel> logger = null;
        private const int PageControlID = (int)Common.Pages.DeliveryAcknowledgement; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;

        public DeliveryAcknowledgementModel(IAntiforgery antiforgery,
                            INumberPlateDispatchServiceRepository numberPlateDispatchServiceRepository,
                         IPermissionServiceRepository permissionService,
                          ILogger<DeliveryAcknowledgementModel> _logger)
        {
            _antiforgery = antiforgery;
            _numberPlateDispatchServiceRepository = numberPlateDispatchServiceRepository;
            _permissionrepository = permissionService;
            logger = _logger;
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
        public JsonResult OnGetPackingListByID(int ID)
        {
            var resultdata = _numberPlateDispatchServiceRepository.GetPackingByNumberPlateDispatchID(ID);

            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnGetNumberPlateDispatchByID(int ID)
        {
            var resultdata = _numberPlateDispatchServiceRepository.GetNumberPlateDispatchByID(ID);

            return new JsonResult(new { success = true, data = resultdata });
        }

        public JsonResult OnGetAcknowledgeInnerBox(int packingID)
        {
            try
            {
                if (packingID <= 0)
                {
                    return new JsonResult(new { success = false, message = "Invalid Packing ID" });
                }

                var LastupdatedBy = LoggedUser.UserID;

                // Directly pass the integer to the repository (assuming the repository method supports int input)
                var resultdata = _numberPlateDispatchServiceRepository.InsertHSRPLaserStockTransID(packingID, LastupdatedBy);
               
                return new JsonResult(new { success = true, data = resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }



    }
}
