using SVAuroraERP.Application.Interfaces.Persistance.Dealer;
using SVAuroraERP.Application.Interfaces.Persistance.Online.Master;
using SVAuroraERP.Domain.Dealer;
using SVAuroraERP.Domain.Online.Master;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace SVAuroraERP.WebUI.Pages.Dealer
{
    public class DealerCalendarModel : HSRPBasePageModel
    {
        private readonly IDealerSlotConfigServiceRepository _repository = null;
        private readonly IHSRPUserServiceRepository _hsrpUserRepository = null;
        private readonly ILogger<DealerCalendarModel> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.DealerSlotConfig; // Reuse same permission as DealerSlotConfig
        public DealerCalendarModel(IDealerSlotConfigServiceRepository repository,
                           IHSRPUserServiceRepository hsrpUserRepository,
                           ILogger<DealerCalendarModel> logger,
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
        public List<SelectListItem> OEMList { get; set; } = new List<SelectListItem>();

        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken;
            Permissions = _permissionrepository.GetPagePermissions((int)HSRPLoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;

            LoadOEMList();

            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

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
            OEMList.Insert(0, new SelectListItem { Value = "0", Text = "--All OEM--" });
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
            dealerList.Insert(0, new SelectListItem { Value = "0", Text = "--All Dealers--" });
            return new JsonResult(new { result = new { Value = dealerList } });
        }

        public JsonResult OnGetCalendarData(int? OEMID, int? DealerID, DateTime? FromDate, DateTime? ToDate)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Get all slot configs (not grouped)
                var allConfigsResponse = _repository.GetDealerSlotConfig();
                
                if (allConfigsResponse?.Value is List<VDealerSlotConfig> allConfigs)
                {
                    // Apply filters
                    var filteredConfigs = allConfigs.AsQueryable();
                    
                    if (OEMID.HasValue && OEMID.Value > 0)
                    {
                        filteredConfigs = filteredConfigs.Where(c => c.OEMID == OEMID.Value);
                    }
                    
                    if (DealerID.HasValue && DealerID.Value > 0)
                    {
                        filteredConfigs = filteredConfigs.Where(c => c.DealerID == DealerID.Value);
                    }
                    
                    if (FromDate.HasValue)
                    {
                        filteredConfigs = filteredConfigs.Where(c => c.SlotDate.Date >= FromDate.Value.Date);
                    }
                    
                    if (ToDate.HasValue)
                    {
                        filteredConfigs = filteredConfigs.Where(c => c.SlotDate.Date <= ToDate.Value.Date);
                    }
                    
                    var calendarEvents = new List<object>();
                    
                    foreach (var config in filteredConfigs.ToList())
                    {
                        try
                        {
                            // Create start and end datetime
                            var startDateTime = config.SlotDate.Date.Add(config.StartTime);
                            var endDateTime = config.SlotDate.Date.Add(config.EndTime);
                            
                            var title = $"{(string.IsNullOrEmpty(config.DealerCode) ? "" : config.DealerCode + " - ")}{config.DealerName ?? ""}";
                            var body = $"Slot: {config.SlotName ?? ""} | Capacity: {config.MaxCapacity}";
                            
                            calendarEvents.Add(new
                            {
                                id = config.ConfigID.ToString(),
                                calendarId = "slot-config",
                                title = title,
                                body = body,
                                start = startDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                                end = endDateTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                                category = "time",
                                dueDateClass = "",
                                color = config.IsActive ? "#03bd9e" : "#ff5656",
                                bgColor = config.IsActive ? "#03bd9e" : "#ff5656",
                                borderColor = config.IsActive ? "#03bd9e" : "#ff5656",
                                raw = new
                                {
                                    ConfigID = config.ConfigID,
                                    DealerID = config.DealerID,
                                    DealerName = config.DealerName,
                                    DealerCode = config.DealerCode,
                                    SlotName = config.SlotName,
                                    MaxCapacity = config.MaxCapacity,
                                    IsActive = config.IsActive
                                }
                            });
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing slot config for calendar");
                            continue;
                        }
                    }
                    
                    dataResponse.Value = calendarEvents;
                    dataResponse.Success = true;
                }
                else
                {
                    dataResponse.Value = new List<object>();
                    dataResponse.Success = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading calendar data");
                dataResponse.Success = false;
                dataResponse.Error = true;
                dataResponse.Message = "Error loading calendar data.";
            }
            
            return new JsonResult(dataResponse);
        }
    }
}

