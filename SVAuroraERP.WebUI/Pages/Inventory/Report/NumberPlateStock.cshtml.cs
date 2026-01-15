namespace SVAuroraERP.WebUI.Pages.Inventory.Report
{
    public class NumberPlateStockModel : BasePageModel
    {
        private readonly ILogger<RawMaterialReportModel> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly ISizeServiceRepository _sizeservicerepository;
        private readonly IColorServiceRespository _colorServiceRespository;
        private const int PageControlID = (int)Common.Pages.NumberPlateStock;
        private readonly IPermissionServiceRepository _permissionrepository;
        private readonly IPendingInspectionServiceRepository _materialRepository = null;

        public NumberPlateStockModel(
                           ILogger<RawMaterialReportModel> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           ISizeServiceRepository sizeservicerepository,
                           IColorServiceRespository colorServiceRespository,
                           IPermissionServiceRepository permissionrepository,
                           IPendingInspectionServiceRepository materialRepository)
        {
            _logger = logger;
            _antiforgery = antiforgery;
            _sizeservicerepository = sizeservicerepository;
            _colorServiceRespository = colorServiceRespository;
            _permissionrepository = permissionrepository;
            _materialRepository = materialRepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> SizeList { get; set; } = new List<SelectListItem>();
        public List<SelectListItem> ColorList { get; set; } = new List<SelectListItem>();
        public PagePermissions Permissions { get; set; }
        public byte CurrentPageControlID { get; set; }

        public IActionResult OnGet()
        {
            var tokens = _antiforgery.GetAndStoreTokens(HttpContext);
            AntiforgeryToken = tokens.RequestToken; // Store the token to use in the view
            Permissions = _permissionrepository.GetPagePermissions(LoggedUser.RoleID, PageControlID);
            CurrentPageControlID = PageControlID;
            LoadSizeList();
            LoadColorList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadSizeList()
        {
            DataResponse dataResponse = null;
            SizeList.Clear();
            dataResponse = _sizeservicerepository.GetSize();
            SizeList = ((List<VSize>)dataResponse.Value)
                            .OrderBy(o => o.SizeName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.SizeID.ToString(),
                                 Text = s.SizeName
                             }).ToList();

            SizeList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Size--" });
        }
        public void LoadColorList()
        {
            DataResponse dataResponse = null;
            ColorList.Clear();
            dataResponse = _colorServiceRespository.GetColor();
            ColorList = ((List<VColor>)dataResponse.Value).
                            OrderBy(o => o.ColorName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.ColorID.ToString(),
                                 Text = s.ColorName
                             }).ToList();

            ColorList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Color--" });
        }

        public JsonResult OnPostBatchStockByFilter([FromBody] NumberPlateStockReportFilter NumberPlateStockReportFilter)
        {
            DataResponse resultdata = _materialRepository.GetNumberPlateStockReport(NumberPlateStockReportFilter);
            return new JsonResult(new { resultdata });
        }
        public IActionResult OnPostExportData([FromBody] NumberPlateStockReportFilter filterData)
        {
            var response = _materialRepository.GetNumberPlateStockReport(filterData);
            var list = response.Value as List<NumberPlateStockReportData>;

            if (list == null || !list.Any())
                return BadRequest("No data to export.");

            var selected = filterData.SelectedColumns ?? new List<string>();
            if (!selected.Any())
                selected = new List<string> { "Size", "Colour", "BlankPlate", "HologramPlate", "LaserMarkingPlate", "Packing" };

            list = list
                .Where(x => x.SizeName != "NONE" && x.ColorName != "NONE")
                .OrderBy(x => x.SizeName)
                .ThenBy(x => x.ColorName)
                .ToList();

            var grouped = list.GroupBy(x => x.SizeName).ToList();

            using (var wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("Number Plate Stock");

                int col = 1;
                if (selected.Contains("Size")) ws.Cell(1, col++).Value = "SIZE";
                if (selected.Contains("Colour")) ws.Cell(1, col++).Value = "COLOUR";
                if (selected.Contains("BlankPlate")) ws.Cell(1, col++).Value = "BLANK PLATE";
                if (selected.Contains("HologramPlate")) ws.Cell(1, col++).Value = "HOLOGRAM PLATE";
                if (selected.Contains("LaserMarkingPlate")) ws.Cell(1, col++).Value = "LASERMARKING PLATE";
                if (selected.Contains("Packing")) ws.Cell(1, col++).Value = "PACKING";

                int row = 2;

                foreach (var g in grouped)
                {
                    int rowspan = g.Count();
                    bool merged = false;

                    foreach (var item in g)
                    {
                        int c = 1;

                        if (selected.Contains("Size"))
                        {
                            if (!merged)
                            {
                                ws.Range(row, c, row + rowspan - 1, c).Merge().Value = item.SizeName;
                                merged = true;
                            }
                            c++;
                        }
                        if (selected.Contains("Colour"))
                            ws.Cell(row, c++).Value = item.ColorName;

                        if (selected.Contains("BlankPlate"))
                            ws.Cell(row, c++).Value = item.BlankPlate == 0 ? "NIL" : $"{item.BlankPlate} {item.UnitName}";

                        if (selected.Contains("HologramPlate"))
                            ws.Cell(row, c++).Value = item.HologramPlate == 0 ? "NIL" : $"{item.HologramPlate} {item.UnitName}";

                        if (selected.Contains("LaserMarkingPlate"))
                            ws.Cell(row, c++).Value = item.LaserMarkingPlate == 0 ? "NIL" : $"{item.LaserMarkingPlate} {item.UnitName}";

                        if (selected.Contains("Packing"))
                            ws.Cell(row, c++).Value = item.Packing == 0 ? "NIL" : $"{item.Packing} {item.UnitName}";

                        row++;
                    }
                }

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    stream.Position = 0;

                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"NumberPlateStock-{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                    );
                }
            }
        }




    }
}
