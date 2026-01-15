namespace SVAuroraERP.WebUI.Pages.HSRPDashboard
{
    public class HSRPDashboardModel : HSRPBasePageModel
    {
        private readonly IHsrpDashboardServiceRepository _repository;
        private readonly ILogger<HSRPDashboardModel> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionRepository;
        private const int PageControlID = (int)Common.Pages.HSRPDashboard;

        public HSRPDashboardModel(
            IHsrpDashboardServiceRepository repository,
            ILogger<HSRPDashboardModel> logger,
            IAntiforgery antiforgery,
            SessionService sessionService,
            IPermissionServiceRepository permissionRepository)
        {
            _repository = repository;
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionRepository = permissionRepository;
        }

        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken;
            Permissions = _permissionRepository.GetPagePermissions((int)HSRPLoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
        }
        public async Task<JsonResult> OnPostHsrpDashboardAsync([FromForm] HsrpDashboardRequest dataTableRequest)
        {
            try
            {
                dataTableRequest.UserID = HSRPLoggedUser.UserID;
                var dataResponse = await _repository.GetHsrpDashboardAsync(dataTableRequest);
                return new JsonResult(new
                {
                    dataResponse.recordsTotal,
                    dataResponse.recordsFiltered,
                    data = dataResponse.Value
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading Hsrp Dashboard");
                return new JsonResult(new
                {
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<HsrpDashboard>()
                });
            }
        }
        public IActionResult OnGetRedirect(int OrderStatusID)
        {
            if (HSRPLoggedUser == null)
                return RedirectToPage("/Account/Login");

            if (!Enum.TryParse<OrderStatus>(OrderStatusID.ToString(), out var status))
                return RedirectToPage("/AccessDenied");

            bool isEmbossingStation = HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.EmbossingStation;
            bool isAdmin = HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.Admin;

            // Centralized routing map
            var routeMap = new Dictionary<OrderStatus, Func<IActionResult>>
    {
        {
            OrderStatus.ReadyForProcessing, () => (isEmbossingStation || isAdmin) ? RedirectToPage("/Orders/ManageOrder/LaserNoAllocation")
                                                 : RedirectToPage("/Orders/ViewOrders/ViewLaserNoAllocation")

        },
        {
            OrderStatus.LaserNoAssigned, () => (isEmbossingStation || isAdmin)  ? RedirectToPage("/Orders/ManageOrder/CreateJobCard")
                                                 : RedirectToPage("/Orders/ViewOrders/ViewCreateJobCard")
        },
        {
            OrderStatus.JobCardGenerated, () => (isEmbossingStation || isAdmin)  ? RedirectToPage("/Orders/ManageOrder/QualityProcessing")
                                                                            : RedirectToPage("/Orders/ViewOrders/ViewQualityProcessing")
        },
        {
            OrderStatus.QualityProcessing, () =>(isEmbossingStation || isAdmin)  ?  RedirectToPage("/Orders/ManageOrder/QCCompleted")
                                                                             :  RedirectToPage("/Orders/ViewOrders/ViewQCCompleted")
        },
        {
            OrderStatus.QCCompleted, () =>(isEmbossingStation || isAdmin)  ?  RedirectToPage("/Orders/Invoice/CreateInvoice")
                                                                             :  RedirectToPage("/AccessDenied")
        },
        {
            OrderStatus.InvoiceGenerated, () =>(isEmbossingStation || isAdmin)  ?  RedirectToPage("/Orders/ManageOrder/Dispatched")
                                                                             :  RedirectToPage("/Orders/ViewOrders/ViewDispatched")
        },
        {
            OrderStatus.DispatchedOrders, () =>(isEmbossingStation || isAdmin)  ?  RedirectToPage("/Orders/OrdersDelivery/GenerateDeliveryData")
                                                                             :  RedirectToPage("/AccessDenied")
        },
        {
            OrderStatus.Delivered, () =>(isEmbossingStation || isAdmin)  ?  RedirectToPage("/Orders/ManageOrder/DeliveryAcknowledgement")
                                                                             :  RedirectToPage("/Orders/ViewOrders/ViewDeliveryAcknowledgement")
        },
        {
            OrderStatus.VahanAPISubmitted, () =>(isEmbossingStation || isAdmin)  ?  RedirectToPage("/Orders/ManageOrder/APISubmittedOrders")
                                                                             :  RedirectToPage("/Orders/ViewOrders/ViewAPISubmittedOrders")
        },
        {
            OrderStatus.RejectedQualityProcessing, () =>(isEmbossingStation || isAdmin)  ?  RedirectToPage("/Orders/ManageOrder/RejectedQualityProcessing")
                                                                             :  RedirectToPage("/Orders/ViewOrders/ViewRejectedQualityProcessing")
        },
        {
            OrderStatus.FittedOrders, () =>(isEmbossingStation || isAdmin)  ?  RedirectToPage("/Orders/ManageOrder/FittedOrders")
                                                                             :  RedirectToPage("/Orders/ViewOrders/ViewFittedOrders")
        },
        {
            OrderStatus.CancelledOrders, () =>(isEmbossingStation || isAdmin)  ?  RedirectToPage("/Orders/ManageOrder/CancelledorDamagedPlateOrders")
                                                                             :  RedirectToPage("/Orders/ViewOrders/ViewCancelledorDamagedPlateOrders")
        },
        {
            OrderStatus.FixationReUpload, () =>(isEmbossingStation || isAdmin)  ?  RedirectToPage("/Orders/ManageOrder/FixationImageReupload")
                                                                             :  RedirectToPage("/Orders/ViewOrders/ViewFixationImageReupload")
        },
        {
            OrderStatus.FixationReUploaded, () =>(isEmbossingStation || isAdmin)  ?  RedirectToPage("/Orders/ManageOrder/FixationReUploaded")
                                                                             :  RedirectToPage("/Orders/ViewOrders/ViewFixationReUploaded")
        },
    };


            if (routeMap.TryGetValue(status, out var redirect))
                return redirect();
            return RedirectToPage("/AccessDenied");
        }
    }
}