namespace SVAuroraERP.WebUI.Pages.Inventory.Production
{
    public class OldHologramModel : BasePageModel
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IHologramPunchingServiceRepository _repository;
        private readonly IEmployeeServiceRepository _emprepo;
        private readonly IMachineServiceRepository _machinerepo;

        public OldHologramModel(IAntiforgery antiforgery,
                                     IHologramPunchingServiceRepository repository,
                                     IEmployeeServiceRepository emprepo,
                                     IMachineServiceRepository machinerepo)
        {
            _antiforgery = antiforgery;
            _repository = repository;
            _emprepo = emprepo;
            _machinerepo = machinerepo;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> MachineTypeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> EmployeeList { get; set; } = new List<SelectListItem>();
        public void OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken;
            LoadMachineTypeList();
            LoadEmployeeList();
        }
        public JsonResult OnGetHologramPunchingList()
        {
            var resultdata = _repository.GetHologramPunchingList();

            return new JsonResult(new { success = true, data = resultdata });
        }
        public void LoadMachineTypeList()
        {
            DataResponse dataResponse = null;
            MachineTypeList.Clear();
            dataResponse = _machinerepo.GetMachineList();
            MachineTypeList = ((List<VMachine>)dataResponse.Value).Where(w => w.MachineTypeID == 1)
                .Select(s => new SelectListItem
                {
                    Value = s.MachineTypeID.ToString(),
                    Text = s.MachineName,
                }).ToList();

            MachineTypeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Machine Type --" });
        }

        public void LoadEmployeeList()
        {
            DataResponse dataResponse = new DataResponse();
            EmployeeList.Clear();
            dataResponse = _emprepo.GetEmployee();
            EmployeeList = ((List<VEmployee>)dataResponse.Value)
                .Select(s => new SelectListItem
                {
                    Value = s.EmployeeID.ToString(),
                    Text = s.FirstName
                }).ToList();

            EmployeeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Operator--" });
        }
        public JsonResult OnPostUpdateHologramPunching([FromBody] HologramPunching HologramPunching)
        {
            string message = string.Empty;
            Tuple<bool, bool,bool> resultdata = null;
            try
            {
                HologramPunching.LastUpdatedBy = LoggedUser.UserID;

                resultdata = _repository.SaveHologramPunching(HologramPunching);

                return new JsonResult(new { success = resultdata.Item1, isExists = resultdata.Item2 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
