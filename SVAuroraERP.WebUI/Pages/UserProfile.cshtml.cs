using SVAuroraERP.Domain.Inventory.Master;

namespace SVAuroraERP.WebUI.Pages
{
    public class UserProfileModel : BasePageModel
    {
        private readonly IAppLauncherServiceRepository _repository = null;
        private readonly IRoleConfigurationServiceRepository _roleconfig = null;
        private readonly ILogger<Color> logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IUserServiceRepository _userServiceRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserProfileModel(IAppLauncherServiceRepository respository,
                          ILogger<Color> _logger,
                          IAntiforgery antiforgery,
                          SessionService sessionService,
                          IRoleConfigurationServiceRepository roleconfig,
                          IUserServiceRepository userServiceRepository,
                          IWebHostEnvironment webHostEnvironment
            )
        {
            _repository = respository;
            logger = _logger;
            _antiforgery = antiforgery;
            _roleconfig = roleconfig;
            _userServiceRepository = userServiceRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public string? AntiforgeryToken { get; private set; }
        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
        }
        public JsonResult OnGetRoleConfigurationByRoleID()
        {
            var resultdata = _repository.GetAppLauncherListByUserID(LoggedUser.UserID);

            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnPostSaveAppLauncher([FromBody] List<AppLauncher> AppLauncher)
        {
            string message = string.Empty;
            Tuple<bool, bool> resultdata = null;

            try
            {
                foreach(var Updated in AppLauncher)
                {
                    Updated.UserID = LoggedUser.UserID;
                }
                resultdata = _repository.Save(AppLauncher);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
        public JsonResult OnGetAppLauncherByUserID()
        {
            var resultdata = _repository.GetByUserID(LoggedUser.UserID);

            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnPostSaveChangePassword([FromBody]ChangePassword changePassword)
        {
            string message = string.Empty;
            Task<DataResponse> resultdata = null;

            try
            {
                changePassword.UserID = LoggedUser.UserID;               
                resultdata = _userServiceRepository.ChangePassword(changePassword);

                return new JsonResult(new { resultdata.Result });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata.Result });
            }
        }
        public JsonResult OnPostUpdateUserName([FromBody] User user)
        {
            string message = string.Empty;
            Task<DataResponse> resultdata = null;

            try
            {
                user.UserID = LoggedUser.UserID;
                resultdata = _userServiceRepository.UpdateUserName(user);

                return new JsonResult(new { resultdata.Result });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { resultdata.Result });
            }
        }
        public JsonResult OnGetUserProfile()
        {
            var resultdata = _userServiceRepository.GetUserProfile(LoggedUser.UserID);

            return new JsonResult(new { success = true, data = resultdata.Result });
        }
        public async Task<JsonResult> OnGetUserByID()
        {
            var resultdata = await _userServiceRepository.GetUserByID(LoggedUser.UserID);

            return new JsonResult(new { success = true, data = resultdata });
        }
        public JsonResult OnGetPageControlList(int RoleID)
        {
            var pageList = _roleconfig.GetRoleConfigurationByRoleID(RoleID)?
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

            pageList.Insert(0, new SelectListItem
            {
                Value = "0",
                Text = "--Select Page--"
            });

            return new JsonResult(pageList);
        }

        public async Task<JsonResult> OnPostUploadProfile(IFormFile profileImage)
        {
            if (profileImage == null || profileImage.Length == 0)
            {
                return new JsonResult(new { success = false, message = "No file uploaded" });
            }

            // Set the upload folder in wwwroot/images
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads","userprofile");
            string uniqueFileName = LoggedUser.UserID + "_" + Guid.NewGuid().ToString() + Path.GetExtension(profileImage.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Ensure the images folder exists
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await profileImage.CopyToAsync(fileStream);
            }

            // Store file path in DB (Example: "/images/uniqueFileName.jpg")
            string dbFilePath = "/uploads/userprofile/" + uniqueFileName;
               
            return new JsonResult(new { success = true, filePath = dbFilePath });
        }
        public JsonResult OnPostUploadUserProfile([FromBody] string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return new JsonResult(new { success = false, message = "Path is required." });
            }

            try
            {
                DataResponse resultdata = _userServiceRepository.UploadProfilePicture(LoggedUser.UserID, path);
                return new JsonResult(new { success = true, result = resultdata });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = "An error occurred while updating profile.", error = ex.Message });
            }
        }
    }
}