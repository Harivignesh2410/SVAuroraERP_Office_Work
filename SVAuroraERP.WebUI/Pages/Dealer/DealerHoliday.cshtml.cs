using SVAuroraERP.Application.Interfaces.Persistance.Dealer;
using SVAuroraERP.Application.Interfaces.Persistance.Online.Master;
using SVAuroraERP.Domain.Dealer;
using SVAuroraERP.Domain.Online.Master;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace SVAuroraERP.WebUI.Pages.Dealer
{
    public class DealerHolidayModel : HSRPBasePageModel
    {
        private readonly IDealerHolidayServiceRepository _repository = null;
        private readonly IHolidayTypeServiceRepository _holidayTypeRepository = null;
        private readonly IHSRPUserServiceRepository _hsrpUserRepository = null;
        private readonly ILogger<DealerHolidayModel> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.DealerHoliday; // ID for this specific page
        public DealerHolidayModel(IDealerHolidayServiceRepository repository,
                           IHolidayTypeServiceRepository holidayTypeRepository,
                           IHSRPUserServiceRepository hsrpUserRepository,
                           ILogger<DealerHolidayModel> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IPermissionServiceRepository permissionService)
        {
            _repository = repository;
            _holidayTypeRepository = holidayTypeRepository;
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
        public List<VHolidayType> HolidayTypeList { get; set; } = new List<VHolidayType>();

        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions((int)HSRPLoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;

            LoadOEMList();
            LoadHolidayTypeList();

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

        private void LoadHolidayTypeList()
        {
            var dataResponse = _holidayTypeRepository.GetHolidayType();
            HolidayTypeList.Clear();
            var holidayTypes = dataResponse.Value as List<VHolidayType>;
            if (holidayTypes != null)
            {
                HolidayTypeList = holidayTypes
                    .Where(w => w.IsActive)
                    .OrderBy(o => o.TypeName)
                    .ToList();
            }
        }

        public JsonResult OnPostDealerHolidayToDataTable([FromForm] DealerHolidayDataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Parse date strings to DateTime if they come as strings from form
                // Handle YYYY-MM-DD format (date-only, no timezone conversion)
                if (dataTableRequest.FromDate == null && !string.IsNullOrWhiteSpace(HttpContext.Request.Form["FromDate"].ToString()))
                {
                    var fromDateStr = HttpContext.Request.Form["FromDate"].ToString();
                    if (DateTime.TryParseExact(fromDateStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedFromDate))
                    {
                        // Ensure date-only (no time component) and Unspecified kind to avoid UTC conversion
                        dataTableRequest.FromDate = DateTime.SpecifyKind(parsedFromDate.Date, DateTimeKind.Unspecified);
                    }
                    else if (DateTime.TryParse(fromDateStr, out parsedFromDate))
                    {
                        // Fallback to regular parse
                        dataTableRequest.FromDate = DateTime.SpecifyKind(parsedFromDate.Date, DateTimeKind.Unspecified);
                    }
                }

                if (dataTableRequest.ToDate == null && !string.IsNullOrWhiteSpace(HttpContext.Request.Form["ToDate"].ToString()))
                {
                    var toDateStr = HttpContext.Request.Form["ToDate"].ToString();
                    if (DateTime.TryParseExact(toDateStr, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime parsedToDate))
                    {
                        // Ensure date-only (no time component) and Unspecified kind to avoid UTC conversion
                        dataTableRequest.ToDate = DateTime.SpecifyKind(parsedToDate.Date, DateTimeKind.Unspecified);
                    }
                    else if (DateTime.TryParse(toDateStr, out parsedToDate))
                    {
                        // Fallback to regular parse
                        dataTableRequest.ToDate = DateTime.SpecifyKind(parsedToDate.Date, DateTimeKind.Unspecified);
                    }
                }

                // Validate sort column
                var validColumns = new[] { "OEMName", "DealerName", "HolidayDate", "Reason" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "HolidayDate";

                dataResponse = _repository.GetDealerHolidayToDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading DealerHoliday data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<VDealerHoliday>()
                });
            }
        }

        public JsonResult OnPostSaveUpdateData([FromBody] DealerHolidaySaveRequest request)
        {
            DataResponse resultdata = null;

            try
            {
                var dealerHoliday = request.DealerHoliday;
                dealerHoliday.LastUpdatedBy = (int)HSRPLoggedUser.UserID;

                // Convert HolidayDate from string (DD/MM/YYYY) to DateTime if sHolidayDate is provided
                if (!string.IsNullOrWhiteSpace(request.sHolidayDate))
                {
                    var convertedDate = ConvertDate(request.sHolidayDate);
                    if (!convertedDate.HasValue)
                    {
                        return new JsonResult(new DataResponse
                        {
                            Success = false,
                            Error = true,
                            Message = "Invalid date format. Please use DD/MM/YYYY format."
                        });
                    }
                    dealerHoliday.HolidayDate = convertedDate.Value;
                }

                // Convert HolidayTypeIDs to DealerHolidayType list
                List<DealerHolidayType> dealerHolidayTypes = new List<DealerHolidayType>();
                if (request.HolidayTypeIDs != null && request.HolidayTypeIDs.Count > 0)
                {
                    foreach (var holidayTypeID in request.HolidayTypeIDs)
                    {
                        dealerHolidayTypes.Add(new DealerHolidayType
                        {
                            HolidayTypeID = holidayTypeID,
                            IsEnabled = true,
                            LastUpdatedBy = (int)HSRPLoggedUser.UserID
                        });
                    }
                }

                if (dealerHoliday.DealerHolidayID == 0)
                    resultdata = _repository.Save(dealerHoliday, dealerHolidayTypes);
                else if (dealerHoliday.DealerHolidayID > 0)
                    resultdata = _repository.Update(dealerHoliday, dealerHolidayTypes);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }

        public JsonResult OnGetDealerHolidayByID(int ID)
        {
            DataResponse resultdata = _repository.GetDealerHolidayByID(ID);

            return new JsonResult(resultdata);
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

        private DateTime? ConvertDate(string sdate)
        {
            DateTime? dtConvertedDate = null;
            if (DateTime.TryParseExact(sdate, Domain.Constants.DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDt))
            {
                dtConvertedDate = parsedDt;
            }
            return dtConvertedDate;
        }
    }

    public class DealerHolidaySaveRequest
    {
        public DealerHoliday DealerHoliday { get; set; } = new DealerHoliday();
        public List<int>? HolidayTypeIDs { get; set; }
        public string sHolidayDate { get; set; } = string.Empty; // DD/MM/YYYY format from frontend
    }
}

