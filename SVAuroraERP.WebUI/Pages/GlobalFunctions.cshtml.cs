using Azure;
using Core.Logging.Models;
using SVAuroraERP.Domain.Authentication;

namespace SVAuroraERP.WebUI.Pages
{
    public class GlobalFunctionsModel : BasePageModel
    {
        private readonly IStateServiceRepository _staterepository;
        private readonly IHomeFitmentPincodeServiceRepository _homefitmentpincodeservicerepository;
        private readonly IPermissionServiceRepository _permissionServiceRepository;
        private readonly IHSRPUserServiceRepository _hSRPUserServiceRepository;
        private readonly IHSRPOrdersServiceRepository _hSRPOrdersServiceRepository;
        private readonly IVehiclePlateColorServiceRepository _vehiclePlateColorServiceRepository;
        private readonly IVehiclePlateSizeServiceRepository _vehiclePlateSizeServiceRepository;
        private readonly IHSRPLaserNoStockServiceRepository _hSRPLaserNoStockServiceRepository;
        private readonly IColorServiceRespository _colorServiceRespository; 
        private readonly ISizeServiceRepository _sizeServiceRepository;
        private readonly IRoleConfigurationServiceRepository _configrepository;
        private readonly ICreateJobCardServiceRepository _createJobCardServiceRepository;
        private readonly IUpdateOrderDataServiceRepository _updateOrderDataServiceRepository;    
        private readonly ICourierServiceRepository _courierServiceRepository;
        private readonly IHSRPPartNumberServiceRepository _hSRPPartNumberService;

        public GlobalFunctionsModel(IStateServiceRepository staterepository,
             IHomeFitmentPincodeServiceRepository homeFitmentPincodeServiceRepository,
             IPermissionServiceRepository permissionServiceRepository,
             IHSRPUserServiceRepository hSRPUserServiceRepository,
             IHSRPOrdersServiceRepository hSRPOrdersServiceRepository,
             IVehiclePlateColorServiceRepository vehiclePlateColorServiceRepository,
             IVehiclePlateSizeServiceRepository vehiclePlateSizeServiceRepository,
             IHSRPLaserNoStockServiceRepository hSRPLaserNoStockServiceRepository,
             IColorServiceRespository colorServiceRespository,
             ISizeServiceRepository sizeServiceRepository,
             IRoleConfigurationServiceRepository configrepository,    
             ICreateJobCardServiceRepository createJobCardServiceRepository,
             IUpdateOrderDataServiceRepository updateOrderDataServiceRepository,
             ICourierServiceRepository courierServiceRepository,
             IHSRPPartNumberServiceRepository hSRPPartNumberService)
        {

            _staterepository = staterepository;
            _homefitmentpincodeservicerepository = homeFitmentPincodeServiceRepository;
            _permissionServiceRepository = permissionServiceRepository;
            _hSRPUserServiceRepository = hSRPUserServiceRepository;
            _hSRPOrdersServiceRepository = hSRPOrdersServiceRepository;
            _vehiclePlateColorServiceRepository = vehiclePlateColorServiceRepository;
            _vehiclePlateSizeServiceRepository = vehiclePlateSizeServiceRepository;
            _hSRPLaserNoStockServiceRepository = hSRPLaserNoStockServiceRepository;
            _colorServiceRespository = colorServiceRespository;
            _sizeServiceRepository = sizeServiceRepository;
            _configrepository = configrepository;
            _createJobCardServiceRepository = createJobCardServiceRepository;
            _updateOrderDataServiceRepository = updateOrderDataServiceRepository;
            _courierServiceRepository = courierServiceRepository;
            _hSRPPartNumberService = hSRPPartNumberService;
        }
        public void OnGet()
        {
        }

        public JsonResult OnGetStateList()
        {
            var response = _staterepository.GetState();

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetDistrictByStateID(int StateID)
        {
            var response = _homefitmentpincodeservicerepository.GetDistrictByStateID(StateID);

            return new JsonResult(new { result = response });
        }
        public void OnPostInsertPageAccessAuditLog([FromBody] PageAccessAudit request)
        {
            if (request.PageControlID <= 0) request.PageControlID = null;

            request.AccessDate = DateTime.UtcNow;
            request.LoginAuditID = LoggedUser.LoginAuditID;

            _permissionServiceRepository.InsertPageAccessAuditLog(request);
        }
        public JsonResult OnGetOEMList()
        {
            var response = _hSRPUserServiceRepository.GetOEM();

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetEmbossingStationList()
        {
            var response = _hSRPUserServiceRepository.GetEmbossingStation();

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetDealerList()
        {
            var response = _hSRPUserServiceRepository.GetDealer();

            return new JsonResult(new { result = response });
        }

        public JsonResult OnGetDealerListByOEMID(int oemID)
        {
            var response = _hSRPUserServiceRepository.GetDealerByOEMIDForFilter(oemID);

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetHsrpOrderTypeList()
        {
            var response = _hSRPOrdersServiceRepository.GetOrderType();

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetLaserNoStockStatusList()
        {
            var response = _hSRPLaserNoStockServiceRepository.GetHSRPLaserNoStatus();

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetVehiclePlateSizeList()
        {
            var response = _vehiclePlateSizeServiceRepository.GetVehiclePlateSize();

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetVehiclePlateColorList()
        {
            var response = _vehiclePlateColorServiceRepository.GetVehiclePlateColor();

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetColor()
        {
            var response = _colorServiceRespository.GetColor();

            return new JsonResult(new { result = response });
        }

        public JsonResult OnGetApplicationList()
        {
            var response = _hSRPUserServiceRepository.GetApplication();

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetSize()
        {
            var response = _sizeServiceRepository.GetSize();

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetLasserNo()
        {
            var response = _createJobCardServiceRepository.GetLasserNo();

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetCourier()
        {
            var response = _courierServiceRepository.GetCourier();

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetGlobalRoleIDByPageID(int PageID)
        {
            var response = _hSRPUserServiceRepository.GetRoleIDByPageID(PageID);

            return new JsonResult(new { result = response });
        }

        public JsonResult OnGetGlobalPageControlByRoleID(int RoleID)
        {
            var pageList = _configrepository.GetRoleConfigurationByRoleID(RoleID)?
              .Where(w => w.IsAccess)
              .GroupBy(g => new { g.PageControlID, g.PageName })
              .Select(g => g.First())
              .OrderBy(d => d.PageName)
              .Select(d => new SelectListItem
              {
                  Value = d.PageControlID.ToString(),
                  Text = d.PageName
              })
              .ToList() ?? new List<SelectListItem>();


            return new JsonResult(pageList);
        }
        public JsonResult OnGetRectificaionReason()
        {
            var response = _updateOrderDataServiceRepository.GetRectificationReason();

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetOrderStatusTimeline(int orderId)
        {
            var response = _hSRPOrdersServiceRepository.GetOrderStatusTimeline(orderId);

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetOrderInvoiceDetails(int orderId)
        {
            var response = _hSRPOrdersServiceRepository.GetInvoiceDetails(orderId);

            return new JsonResult(new { result = response });
        }
        public JsonResult OnGetShipmentAndDeliveryDetails(int orderId)     
        {
            var response = _hSRPOrdersServiceRepository.GetShipmentAndDeliveryDetails(orderId);

            return new JsonResult(new { result = response });
        }

        public JsonResult OnGetOEMListByEmbossingStation(int USERID)
        {
            DataResponse dataResponse = new DataResponse();

            if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.EmbossingStation)
            {
                dataResponse = _hSRPUserServiceRepository
                                    .GetOEMByEmbossingStation(USERID);
            }
            else
            {
                dataResponse = _hSRPUserServiceRepository.GetOEM();
            }
            return new JsonResult(new
            {   success = !dataResponse.Error,
                message = dataResponse.Message,
                result = dataResponse.Value
            });
        }
        public JsonResult OnGetEmbossingStationByUser(int USERID)
        {
            DataResponse dataResponse = new DataResponse();

            if (HSRPLoggedUser.HSRPUserTypeID == (byte)Common.HSRPUserType.EmbossingStation)
            {
                dataResponse = _hSRPUserServiceRepository.GetEmbossingStationByUser(USERID);
            }

            else
            {
                dataResponse = _hSRPUserServiceRepository.GetEmbossingStation();
            }
            return new JsonResult(new
            {
                success = !dataResponse.Error,
                message = dataResponse.Message,
                result = dataResponse.Value
            });
        }

        public JsonResult OnGetPartNumberByOEM(int OEMID)
        {
            var result = _hSRPPartNumberService.GetHSRPPartNumberByOEMID(OEMID);
            return new JsonResult(new { result });
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

        public JsonResult OnGetEmbossingStationByHSRPOnlineOrderID(int OnlineOrderID)
        {
            DataResponse dataResponse = new DataResponse();

                dataResponse = _hSRPUserServiceRepository.GetEmbossingStationByHSRPOnlineOrderID(OnlineOrderID);
            return new JsonResult(new
            {
                success = !dataResponse.Error,
                message = dataResponse.Message,
                result = dataResponse.Value
            });
        }
    }
}