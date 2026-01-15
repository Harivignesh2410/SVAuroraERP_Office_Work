namespace SVAuroraERP.WebUI.Pages.Authentication
{
    public class RoleConfigurationModel : BasePageModel
    {
        private readonly ILogger<RoleConfigurationModel> _logger;
        private readonly IAntiforgery _antiforgery;
        private readonly IAuthenticaionServiceRepository _authenicaterepository;
        private readonly IRoleConfigurationServiceRepository _repository;
        private const int PageControlID = (int)Common.Pages.RoleConfiguration; // ID for this specific page
        private readonly IPermissionServiceRepository _permissionrepository;
        public RoleConfigurationModel(ILogger<RoleConfigurationModel> logger,
                                      IAntiforgery antiforgery,
                                      IAuthenticaionServiceRepository authenticaterepository,
                                      IRoleConfigurationServiceRepository repository,
                                      IPermissionServiceRepository permissionService)
        {
            _logger = logger;
            _antiforgery = antiforgery;
            _authenicaterepository = authenticaterepository;
            _repository = repository;
            _permissionrepository = permissionService;
        }

        public string? AntiforgeryToken { get; private set; }
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }
        public List<SelectListItem> MenuList { get; set; } = new List<SelectListItem>();
        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadMenuList();
        }
        public void LoadMenuList()
        {
            MenuList.Clear();
            MenuList = _authenicaterepository.GetMenuControl()
                        .Select(s => new SelectListItem
                        {
                            Value = s.MenuControlID.ToString(),
                            Text = s.MenuDisplayName
                        }).ToList();

            MenuList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Menu--" });
        }

        public JsonResult OnGetPageControlList(int MenuID)
        {
            var dealers = _authenicaterepository.GetPageControl(MenuID)
                                  .OrderBy(d => d.OrdinalNo)
                                  .Select(d => new SelectListItem
                                  {
                                      Value = d.PageControlID.ToString(),
                                      Text = d.PageName
                                  }).ToList();

            return new JsonResult(dealers);
        }

        public JsonResult OnPostSaveUpdateData([FromBody] List<RoleConfiguration> roleData)
        {
            if (roleData == null || !roleData.Any())
            {
                return new JsonResult(new { success = false, message = "No data provided." });
            }

            try
            {
                foreach (var roleConfiguration in roleData)
                {
                    roleConfiguration.LastUpdatedBy = LoggedUser.UserID;
                    roleConfiguration.LastUpdatedDate = DateTime.UtcNow;
                }

                // Save data using repository
                var result = _repository.SaveChanges(roleData);

                //Return success and conflict information
                return new JsonResult(new
                {
                    success = result.Item1,
                    isExists = result.Item2,
                    message = result.Item1 ? "Updated successfully." : "Failed to Updated"
                });
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "Error occurred while saving role configurations.");

                // Return a user-friendly error message
                return new JsonResult(new { success = false, message = "An unexpected error occurred. Please try again." });
            }
        }

        //Added on 2024.12.15 by Sivakumar
        public JsonResult OnGetRoleConfigurationbyRoleID(int RoleID)
        {
            var result = _repository.GetRoleConfigurationByRoleID(RoleID);
            return new JsonResult(result);
        }

        public JsonResult OnGetMenuLayout() { return new JsonResult(_repository.GetMenuLayout()); }
    }
}