namespace SVAuroraERP.WebUI.Pages.Online.Master
{
    public class VehiclePlateImageModel : BasePageModel
    {
        private readonly IVehiclePlateImageServiceRepository _repository;
        private readonly IVehiclePlateSizeServiceRepository _vechiclepaltesizerepository;
        private readonly IVehiclePlateColorServiceRepository _vechiclepalteColourrepository;
        private readonly ILogger<VehiclePlateImage> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IPermissionServiceRepository _permissionrepository;
        private const int PageControlID = (int)Common.Pages.VehiclePlateImage; // ID for this specific page
        public VehiclePlateImageModel(IVehiclePlateImageServiceRepository respository,
                           ILogger<VehiclePlateImage> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IVehiclePlateColorServiceRepository vechiclepalteColourrepository,
                           IVehiclePlateSizeServiceRepository vechiclepaltesizerepository,
                             IWebHostEnvironment webHostEnvironment,
                             IPermissionServiceRepository permissionService)
        {
            _repository = respository;
            _logger = logger;
            _antiforgery = antiforgery;
            _vechiclepalteColourrepository = vechiclepalteColourrepository;
            _vechiclepaltesizerepository = vechiclepaltesizerepository;
            _webHostEnvironment = webHostEnvironment;
            _permissionrepository = permissionService;
        }
        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> VehiclePlateSizeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> VehiclePlateColorList { get; set; } = new List<SelectListItem>();

        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadVehiclePlateSizeList();
            LoadVehiclePlateColorList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadVehiclePlateSizeList()
        {
            DataResponse dataResponse = new DataResponse();
            VehiclePlateSizeList.Clear();
            dataResponse = _vechiclepaltesizerepository.GetVehiclePlateSize();
            VehiclePlateSizeList = ((List<VVehiclePlateSize>)dataResponse.Value)
                .OrderBy(o => o.VehiclePlateSizeName)
                .Select(s => new SelectListItem
                {
                    Value = s.VehiclePlateSizeID.ToString(),
                    Text = s.VehiclePlateSizeName
                }).ToList();

            VehiclePlateSizeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Vehicle Plate Size--" });
        }
        public void LoadVehiclePlateColorList()
        {
            DataResponse dataResponse = new DataResponse();
            VehiclePlateColorList.Clear();
            dataResponse = _vechiclepalteColourrepository.GetVehiclePlateColor();
            VehiclePlateColorList = ((List<VVehiclePlateColor>)dataResponse.Value)
                .OrderBy(o => o.VehiclePlateColorName)
                .Select(s => new SelectListItem
                {
                    Value = s.VehiclePlateColorID.ToString(),
                    Text = s.VehiclePlateColorName
                }).ToList();

            VehiclePlateColorList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Vehicle Plate Colour--" });
        }
        public JsonResult OnPostSaveUpdateData([FromBody] VehiclePlateImage VehiclePlateImage)
        {
            string message = string.Empty;
            DataResponse resultdata = null;

            try
            {
                VehiclePlateImage.LastUpdatedBy = LoggedUser.UserID;

                if (VehiclePlateImage.VehiclePlateImageID == 0)
                    resultdata = _repository.Save(VehiclePlateImage);
                else if (VehiclePlateImage.VehiclePlateImageID > 0)
                    resultdata = _repository.Update(VehiclePlateImage);

                return new JsonResult(resultdata);
            }
            catch (Exception ex)
            {
                return new JsonResult(resultdata);
            }
        }
        public JsonResult OnGetVehiclePlateImageList(int draw, int start, int length)
        {
            DataResponse dataResponse = null;
            int orderCol = -1;  // Default value
            string orderDir = "asc";  // Default direction

            // Get search value
            var searchValue = HttpContext.Request.Query["search[value]"].ToString();
            orderCol = Convert.ToInt32(HttpContext.Request.Query["order[0][column]"].ToString());
            orderDir = HttpContext.Request.Query["order[0][dir]"].ToString();

            dataResponse = (_repository.GetVehiclePlateImage());
            var resultdata = ((List<VVehiclePlateImage>)dataResponse.Value).ToList();

            // Filter based on search input
            var filteredData = string.IsNullOrWhiteSpace(searchValue)
                ? resultdata
                : resultdata.Where(d => d.VehiclePlateColorName.Contains(searchValue, StringComparison.OrdinalIgnoreCase) ||
                                     (d.VehiclePlateSizeName ?? string.Empty).Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();

            // Handle sorting based on the column index and direction
            Func<VVehiclePlateImage, object> orderByFunc = orderCol switch
            {
                1 => d => d.VehiclePlateColorName,
                2 => d => d.VehiclePlateSizeName,
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
        public async Task<JsonResult> OnPostUploadPlateImages(IFormFile FrontImage, IFormFile RearImage)
        {
            string frontPath = null;
            string rearPath = null;

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "vehicleplateimage");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            if (FrontImage != null)
            {
                string frontFileName = LoggedUser.UserID + "_Front_" + Guid.NewGuid().ToString() + Path.GetExtension(FrontImage.FileName);
                string frontFilePath = Path.Combine(uploadsFolder, frontFileName);
                using (var stream = new FileStream(frontFilePath, FileMode.Create))
                {
                    await FrontImage.CopyToAsync(stream);
                }
                 frontPath = "/uploads/vehicleplateImage/" + frontFileName;
            }

            if (RearImage != null)
            {
                string rearFileName = LoggedUser.UserID + "_Rear_" + Guid.NewGuid().ToString() + Path.GetExtension(RearImage.FileName);
                string rearFilePath = Path.Combine(uploadsFolder, rearFileName);
                using (var stream = new FileStream(rearFilePath, FileMode.Create))
                {
                    await RearImage.CopyToAsync(stream);
                }
                 rearPath = "/uploads/vehicleplateImage/" + rearFileName;
            }

            return new JsonResult(new
            {
                success = true,
                frontImagePath = frontPath,
                rearImagePath = rearPath
            });
        }
        public JsonResult OnGetVehiclePlateSizeMappingeByID(int ID)
        {
            DataResponse resultdata = _repository.GetVehiclePlateImageByID(ID);

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
    }
}
