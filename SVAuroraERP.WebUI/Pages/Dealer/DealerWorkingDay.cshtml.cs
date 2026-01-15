using SVAuroraERP.Application.Interfaces.Persistance.Dealer;
using SVAuroraERP.Application.Interfaces.Persistance.Online.Master;
using SVAuroraERP.Domain.Dealer;
using SVAuroraERP.Domain.Online.Master;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SVAuroraERP.WebUI.Pages.Dealer
{
    public class DealerWorkingDayModel : HSRPBasePageModel
    {
        private readonly IDealerWorkingDayServiceRepository _repository = null;
        private readonly IHSRPUserServiceRepository _hsrpUserRepository = null;
        private readonly ILogger<DealerWorkingDayModel> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.DealerWorkingDay; // ID for this specific page
        public DealerWorkingDayModel(IDealerWorkingDayServiceRepository repository,
                           IHSRPUserServiceRepository hsrpUserRepository,
                           ILogger<DealerWorkingDayModel> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IPermissionServiceRepository permissionService)
        {
            _repository = repository;
            _hsrpUserRepository = hsrpUserRepository;
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> DealerList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> OEMList { get; set; } = new List<SelectListItem>();

        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions((int)HSRPLoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;

            LoadOEMList();
            LoadDealerList();

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }

        private void LoadOEMList()
        {
            var dataResponse = _hsrpUserRepository.GetOEM();
            OEMList.Clear();
            var oems = dataResponse.Value as List<VHSRPUser>;
            if (oems != null)
            {
                OEMList = oems
                    .Where(w => w.IsActive)
                    .OrderBy(o => o.CompanyName)
                    .Select(s => new SelectListItem
                    {
                        Value = s.HSRPUserID.ToString(),
                        Text = s.CompanyName
                    }).ToList();
            }
            OEMList.Insert(0, new SelectListItem { Value = "0", Text = "--Select OEM--" });
        }

        private void LoadDealerList()
        {
            DealerList.Clear();
            DealerList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Dealer--" });
        }

        public JsonResult OnGetDealersByOEMID(int OEMID)
        {
            var dataResponse = _hsrpUserRepository.GetDealerByOEMIDForFilter(OEMID);
            List<SelectListItem> dealerList = new List<SelectListItem>();
            if (dataResponse.Value is List<VHSRPUser> dealers && dealers.Any())
            {
                dealerList = dealers
                    .Where(w => w.IsActive && w.IsDealerEnabledOnline)
                    .OrderBy(o => o.CompanyName)
                    .Select(s => new SelectListItem
                    {
                        Value = s.HSRPUserID.ToString(),
                        Text = (s.HSRPUserCode ?? "") + " - " + (s.CompanyName ?? "") + " (" + (s.City ?? "") + ")"
                    }).ToList();
            }
            dealerList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Dealer--" });
            return new JsonResult(new { result = new { Value = dealerList } });
        }

        public JsonResult OnPostDealerWorkingDayToDataTable([FromForm] DealerWorkingDayDataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Validate sort column
                var validColumns = new[] { "OEMName", "DealerName" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "DealerName";

                dataResponse = _repository.GetDealerWorkingDayToDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading DealerWorkingDay data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<DealerWorkingDayGroupedDto>()
                });
            }
        }

        public JsonResult OnGetDealerWorkingDayByDealerID(int DealerID)
        {
            DataResponse resultdata = _repository.GetDealerWorkingDayByDealerID(DealerID);

            return new JsonResult(resultdata);
        }

        public JsonResult OnPostSaveUpdateData([FromBody] DealerWorkingDaySaveRequest request)
        {
            DataResponse resultdata = null;

            try
            {
                // Create list of DealerWorkingDay for all 7 days
                List<DealerWorkingDay> dealerWorkingDays = new List<DealerWorkingDay>();

                // DayOfWeek: 1=Monday, 2=Tuesday, 3=Wednesday, 4=Thursday, 5=Friday, 6=Saturday, 7=Sunday
                for (byte dayOfWeek = 1; dayOfWeek <= 7; dayOfWeek++)
                {
                    dealerWorkingDays.Add(new DealerWorkingDay
                    {
                        DayOfWeek = dayOfWeek,
                        IsWorking = request.WorkingDays.Contains(dayOfWeek),
                        LastUpdatedBy = (int)HSRPLoggedUser.UserID
                    });
                }

                resultdata = _repository.SaveOrUpdate(request.DealerID, dealerWorkingDays);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }

        public JsonResult OnPostDeleteData([FromBody] int ID)
        {
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

        public JsonResult OnPostDeleteDataByDealer([FromBody] int DealerID)
        {
            DataResponse dataResponse = new DataResponse();

            try
            {
                dataResponse = _repository.DeleteByDealerID(DealerID, (int)HSRPLoggedUser.UserID, (long)HSRPLoggedUser.LoginAuditID);

                return new JsonResult(new { resultdata = dataResponse });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata = dataResponse });
            }
        }
    }

    public class DealerWorkingDaySaveRequest
    {
        public int DealerID { get; set; }
        public List<byte> WorkingDays { get; set; } = new List<byte>(); // List of DayOfWeek (1-7) that are working days
    }

    public class DealerWorkingDayGroupedDto
    {
        public int DealerID { get; set; }
        public string DealerName { get; set; } = string.Empty;
        public string DealerCode { get; set; } = string.Empty;
        public int OEMID { get; set; }
        public string OEMName { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool Monday { get; set; }
        public bool Tuesday { get; set; }
        public bool Wednesday { get; set; }
        public bool Thursday { get; set; }
        public bool Friday { get; set; }
        public bool Saturday { get; set; }
        public bool Sunday { get; set; }
        public int WorkingDayID { get; set; }
    }
}

