using SVAuroraERP.Application.Interfaces.Persistance.Dealer;
using SVAuroraERP.Application.Interfaces.Persistance.Online.Master;
using SVAuroraERP.Domain.Dealer;
using SVAuroraERP.Domain.Online.Master;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace SVAuroraERP.WebUI.Pages.Dealer
{
    public class DealerSlotConfigModel : HSRPBasePageModel
    {
        private readonly IDealerSlotConfigServiceRepository _repository = null;
        private readonly ITimeSlotServiceRepository _timeSlotRepository = null;
        private readonly IHSRPUserServiceRepository _hsrpUserRepository = null;
        private readonly IDealerWorkingDayServiceRepository _dealerWorkingDayRepository = null;
        private readonly IDealerHolidayServiceRepository _dealerHolidayRepository = null;
        private readonly ILogger<DealerSlotConfigModel> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.DealerSlotConfig; // ID for this specific page
        public DealerSlotConfigModel(IDealerSlotConfigServiceRepository repository,
                           ITimeSlotServiceRepository timeSlotRepository,
                           IHSRPUserServiceRepository hsrpUserRepository,
                           IDealerWorkingDayServiceRepository dealerWorkingDayRepository,
                           IDealerHolidayServiceRepository dealerHolidayRepository,
                           ILogger<DealerSlotConfigModel> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IPermissionServiceRepository permissionService)
        {
            _repository = repository;
            _timeSlotRepository = timeSlotRepository;
            _hsrpUserRepository = hsrpUserRepository;
            _dealerWorkingDayRepository = dealerWorkingDayRepository;
            _dealerHolidayRepository = dealerHolidayRepository;
            _logger = logger;
            _antiforgery = antiforgery;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> DealerList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> OEMList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> TimeSlotList { get; set; } = new List<SelectListItem>();
        
        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions((int)HSRPLoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            
            LoadOEMList();
            LoadDealerList();
            LoadTimeSlotList();
      
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
            var dealerList = new List<SelectListItem>();
            
            try
            {
                var dataResponse = _hsrpUserRepository.GetDealerByOEMIDForFilter(OEMID);
                var dealers = dataResponse?.Value as List<VHSRPUser>;
                
                if (dealers != null && dealers.Any())
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
            }
            catch (Exception ex)
            {
                // Log error if needed
            }
            
            dealerList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Dealer--" });
            
            return new JsonResult(new { result = new { Value = dealerList } });
        }
        
        private void LoadTimeSlotList()
        {
            var dataResponse = _timeSlotRepository.GetTimeSlot();
            TimeSlotList.Clear();
            var timeSlots = dataResponse.Value as List<VTimeSlot>;
            if (timeSlots != null)
            {
                TimeSlotList = timeSlots
                    .Where(w => w.IsActive)
                    .OrderBy(o => o.StartTime)
                    .Select(s => new SelectListItem
                    {
                        Value = s.TimeSlotID.ToString(),
                        Text = s.SlotName
                    }).ToList();
            }
            TimeSlotList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Time Slot--" });
        }
        
        
        public JsonResult OnPostDealerSlotConfigToDataTable([FromForm] DealerSlotConfigDataTableRequest dataTableRequest)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                // Parse date strings to DateTime if they come as strings from form
                if (dataTableRequest.FromDate == null && !string.IsNullOrWhiteSpace(HttpContext.Request.Form["FromDate"].ToString()))
                {
                    if (DateTime.TryParse(HttpContext.Request.Form["FromDate"].ToString(), out DateTime parsedFromDate))
                    {
                        dataTableRequest.FromDate = parsedFromDate;
                    }
                }
                
                if (dataTableRequest.ToDate == null && !string.IsNullOrWhiteSpace(HttpContext.Request.Form["ToDate"].ToString()))
                {
                    if (DateTime.TryParse(HttpContext.Request.Form["ToDate"].ToString(), out DateTime parsedToDate))
                    {
                        dataTableRequest.ToDate = parsedToDate;
                    }
                }
                
                // Validate sort column
                var validColumns = new[] { "OEMName", "DealerName", "SlotDate", "TotalCapacity" };
                dataTableRequest.SortColumn = validColumns.Contains(dataTableRequest.SortColumn) ? dataTableRequest.SortColumn : "SlotDate";

                dataResponse = _repository.GetDealerSlotConfigToDataTable(dataTableRequest);

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
                _logger.LogError(ex, "Error loading DealerSlotConfig data");

                return new JsonResult(new
                {
                    draw = dataTableRequest.Draw,
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = new List<DealerSlotConfigGroupedDto>()
                });
            }
        }
        
        public JsonResult OnPostSaveUpdateData([FromBody] DealerSlotConfigSaveRequest request)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                // Validate request
                if (request == null)
                {
                    return new JsonResult(new DataResponse
                    {
                        Success = false,
                        Error = true,
                        Message = "Invalid request data."
                    });
                }
                
                if (request.TimeSlotConfigs == null || !request.TimeSlotConfigs.Any())
                {
                    return new JsonResult(new DataResponse
                    {
                        Success = false,
                        Error = true,
                        Message = "At least one time slot configuration is required."
                    });
                }
                
                if (request.DealerID <= 0)
                {
                    return new JsonResult(new DataResponse
                    {
                        Success = false,
                        Error = true,
                        Message = "Please select a valid dealer."
                    });
                }

                // Convert sSlotDate to DateTime
                var slotDate = ConvertDate(request.sSlotDate);
                if (!slotDate.HasValue)
                {
                    return new JsonResult(new DataResponse
                    {
                        Success = false,
                        Error = true,
                        Message = "Invalid date format. Please use DD/MM/YYYY format."
                    });
                }

                // Validate date is not a non-working day or holiday
                var validationResult = ValidateSlotDate(request.DealerID, slotDate.Value);
                if (!validationResult.IsValid)
                {
                    return new JsonResult(new DataResponse
                    {
                        Success = false,
                        Error = true,
                        Message = validationResult.Message
                    });
                }

                // Convert sOriginalSlotDate to DateTime if provided
                DateTime? originalSlotDate = null;
                if (!string.IsNullOrWhiteSpace(request.sOriginalSlotDate))
                {
                    originalSlotDate = ConvertDate(request.sOriginalSlotDate);
                }

                // If date has changed during edit, delete configurations for the original date
                if (originalSlotDate.HasValue && 
                    originalSlotDate.Value.Date != slotDate.Value.Date)
                {
                    var originalConfigsResponse = _repository.GetDealerSlotConfig();
                    var originalConfigs = new List<VDealerSlotConfig>();
                    if (originalConfigsResponse?.Value is List<VDealerSlotConfig> allOriginalConfigs)
                    {
                        originalConfigs = allOriginalConfigs
                            .Where(c => c.DealerID == request.DealerID && c.SlotDate.Date == originalSlotDate.Value.Date)
                            .ToList();
                    }
                    
                    // Delete all configurations for the original date
                    foreach (var originalConfig in originalConfigs)
                    {
                        _repository.Delete(originalConfig.ConfigID, (int)HSRPLoggedUser.UserID, (long)HSRPLoggedUser.LoginAuditID);
                    }
                }

                // Get existing configurations for this dealer and new date
                var existingConfigsResponse = _repository.GetDealerSlotConfig();
                var existingConfigs = new List<VDealerSlotConfig>();
                if (existingConfigsResponse?.Value is List<VDealerSlotConfig> allConfigs)
                {
                    existingConfigs = allConfigs
                        .Where(c => c.DealerID == request.DealerID && c.SlotDate.Date == slotDate.Value.Date)
                        .ToList();
                }

                // Process each time slot configuration
                foreach (var slotConfig in request.TimeSlotConfigs)
                {
                    // Find existing config for this time slot
                    var existingConfig = existingConfigs.FirstOrDefault(e => e.TimeSlotID == slotConfig.TimeSlotID);
                    
                    var dealerSlotConfig = new DealerSlotConfig
                    {
                        ConfigID = existingConfig != null ? existingConfig.ConfigID : 0,
                        DealerID = request.DealerID,
                        TimeSlotID = slotConfig.TimeSlotID,
                        SlotDate = slotDate.Value,
                        MaxCapacity = slotConfig.MaxCapacity,
                        IsActive = slotConfig.IsActive,
                        LastUpdatedBy = (int)HSRPLoggedUser.UserID
                    };

                    if (dealerSlotConfig.ConfigID == 0)
                    {
                        // Create new configuration
                        resultdata = _repository.Save(dealerSlotConfig);
                        if (!resultdata.Success)
                        {
                            return new JsonResult(resultdata);
                        }
                    }
                    else
                    {
                        // Update existing configuration
                        resultdata = _repository.Update(dealerSlotConfig);
                        if (!resultdata.Success)
                        {
                            return new JsonResult(resultdata);
                        }
                    }
                }

                // Delete configurations for time slots that are no longer in the request
                var requestedTimeSlotIDs = request.TimeSlotConfigs.Select(t => t.TimeSlotID).ToList();
                var configsToDelete = existingConfigs.Where(e => !requestedTimeSlotIDs.Contains(e.TimeSlotID)).ToList();
                
                foreach (var configToDelete in configsToDelete)
                {
                    _repository.Delete(configToDelete.ConfigID, (int)HSRPLoggedUser.UserID, (long)HSRPLoggedUser.LoginAuditID);
                }

                return new JsonResult(new DataResponse
                {
                    Success = true,
                    Error = false,
                    Message = Constants.SuccessMessage
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new DataResponse
                {
                    Success = false,
                    Error = true,
                    Message = ex.Message
                });
            }
        }

        public JsonResult OnGetValidateSlotDate(int DealerID, string sSlotDate)
        {
            var slotDate = ConvertDate(sSlotDate);
            if (!slotDate.HasValue)
            {
                return new JsonResult(new { IsValid = false, Message = "Invalid date format. Please use DD/MM/YYYY format." });
            }
            var validationResult = ValidateSlotDate(DealerID, slotDate.Value);
            return new JsonResult(new { IsValid = validationResult.IsValid, Message = validationResult.Message });
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

        private (bool IsValid, string Message) ValidateSlotDate(int DealerID, DateTime SlotDate)
        {
            try
            {
                // Check if date is a non-working day
                var workingDaysResponse = _dealerWorkingDayRepository?.GetDealerWorkingDayByDealerID(DealerID);
                if (workingDaysResponse?.Value is List<VDealerWorkingDay> workingDays && workingDays.Any())
                {
                    var dayOfWeek = (byte)((int)SlotDate.DayOfWeek == 0 ? 7 : (int)SlotDate.DayOfWeek); // Convert Sunday from 0 to 7
                    var workingDay = workingDays.FirstOrDefault(w => w.DayOfWeek == dayOfWeek);
                    
                    if (workingDay == null || !workingDay.IsWorking)
                    {
                        return (false, $"Selected date ({SlotDate:dd-MMM-yyyy}) is a non-working day for this dealer.");
                    }
                }

                // Check if date is a holiday
                var holidaysResponse = _dealerHolidayRepository?.GetDealerHoliday();
                if (holidaysResponse?.Value is List<VDealerHoliday> holidays && holidays.Any())
                {
                    var holiday = holidays.FirstOrDefault(h => h.DealerID == DealerID && h.HolidayDate.Date == SlotDate.Date);
                    if (holiday != null)
                    {
                        return (false, $"Selected date ({SlotDate:dd-MMM-yyyy}) is a holiday for this dealer. Reason: {holiday.Reason ?? "Holiday"}");
                    }
                }

                return (true, string.Empty);
            }
            catch (Exception ex)
            {
                // If validation fails due to error, allow it but log the error
                return (true, string.Empty);
            }
        }
        
        public JsonResult OnGetDealerSlotConfigByID(int ID)
        {
            DataResponse resultdata = _repository.GetDealerSlotConfigByID(ID);
            
            // Format sSlotDate if it's not already populated from the view
            if (resultdata.Value is VDealerSlotConfig config)
            {
                if (string.IsNullOrWhiteSpace(config.sSlotDate))
                {
                    config.sSlotDate = config.SlotDate.ToString(Domain.Constants.DateFormat, CultureInfo.InvariantCulture);
                }
            }

            return new JsonResult(resultdata);
        }
        
        public JsonResult OnGetDealerSlotConfigByDealerAndDate(int DealerID, string sSlotDate)
        {
            DataResponse dataResponse = new DataResponse();
            try
            {
                var slotDate = ConvertDate(sSlotDate);
                if (!slotDate.HasValue)
                {
                    dataResponse.Success = false;
                    dataResponse.Error = true;
                    dataResponse.Message = "Invalid date format.";
                    return new JsonResult(dataResponse);
                }

                var allConfigs = _repository.GetDealerSlotConfig();
                var configsForDate = new List<VDealerSlotConfig>();
                
                if (allConfigs?.Value is List<VDealerSlotConfig> allConfigsList)
                {
                    configsForDate = allConfigsList
                        .Where(c => c.DealerID == DealerID && c.SlotDate.Date == slotDate.Value.Date)
                        .OrderBy(c => c.StartTime)
                        .ToList();
                }
                
                dataResponse.Value = configsForDate;
                dataResponse.Count = configsForDate.Count;
                dataResponse.Success = true;
            }
            catch (Exception ex)
            {
                dataResponse.Success = false;
                dataResponse.Error = true;
                dataResponse.Message = ex.Message;
            }
            
            return new JsonResult(dataResponse);
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
    
    public class DealerSlotConfigSaveRequest
    {
        public int ConfigID { get; set; }
        public int DealerID { get; set; }
        public string sSlotDate { get; set; } = string.Empty;
        public string? sOriginalSlotDate { get; set; } // Original date when editing (null for new records)
        public List<TimeSlotConfigDto> TimeSlotConfigs { get; set; } = new List<TimeSlotConfigDto>();
    }
    
    public class TimeSlotConfigDto
    {
        public int ConfigID { get; set; }
        public int TimeSlotID { get; set; }
        public int MaxCapacity { get; set; }
        public bool IsActive { get; set; }
    }
    
    public class DealerSlotConfigGroupedDto
    {
        public int DealerID { get; set; }
        public string? DealerName { get; set; }
        public string? DealerCode { get; set; }
        public string? City { get; set; }
        public int OEMID { get; set; }
        public string? OEMName { get; set; }
        public DateTime SlotDate { get; set; }
        public string sSlotDate { get; set; } = string.Empty;
        public int TotalTimeSlots { get; set; }
        public int TotalCapacity { get; set; }
        public int ActiveTimeSlots { get; set; }
        public int ConfigID { get; set; } // First ConfigID for action buttons
        public bool IsActive { get; set; } // Overall status (if all are active)
    }
}

