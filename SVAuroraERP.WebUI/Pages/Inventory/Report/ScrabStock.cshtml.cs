namespace SVAuroraERP.WebUI.Pages.Inventory.Report
{
    public class ScrabStockModel : BasePageModel
    {
        private readonly ILogger<RawMaterialReportModel> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IComponentServiceRepository _componentrepository;
        private readonly ISizeServiceRepository _sizeservicerepository;
        private readonly IColorServiceRespository _colorServiceRespository;
        private const int PageControlID = (int)Common.Pages.RawMaterialReport;
        private readonly IPermissionServiceRepository _permissionrepository;
        private readonly IScrapEntryServiceRepository _materialRepository = null;

        public ScrabStockModel(
                           ILogger<RawMaterialReportModel> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IComponentServiceRepository componentrepository,
                           ISizeServiceRepository sizeservicerepository,
                           IColorServiceRespository colorServiceRespository,
                           IPermissionServiceRepository permissionrepository,
                           IScrapEntryServiceRepository materialRepository)
        {
            _logger = logger;
            _antiforgery = antiforgery;
            _componentrepository = componentrepository;
            _sizeservicerepository = sizeservicerepository;
            _colorServiceRespository = colorServiceRespository;
            _permissionrepository = permissionrepository;
            _materialRepository = materialRepository;
        }
        public string? AntiforgeryToken { get; private set; }
        public List<SelectListItem> ComponentList { get; set; } = new List<SelectListItem>();
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
            LoadComponentList();
            LoadSizeList();
            LoadColorList();
            //Added on 2025.10.28
            if (!Permissions.HasAccess) return RedirectToPage("/AccessDenied");

            // Normal logic here
            return Page();
        }
        public void LoadComponentList()
        {
            DataResponse dataResponse = null;
            ComponentList.Clear();
            dataResponse = _componentrepository.GetComponentList();
            ComponentList = ((List<VComponentType>)dataResponse.Value)
                            .OrderBy(o => o.ComponentTypeName)
                             .Select(s => new SelectListItem
                             {
                                 Value = s.ComponentTypeID.ToString(),
                                 Text = s.ComponentTypeName
                             }).ToList();

            ComponentList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Component--" });
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

        public JsonResult OnPostScrabStockByFilter([FromBody] ScrapDataFilterParameter FilterForBatchStock)
        {
            var resultdata = _materialRepository.GetScrapStockData(FilterForBatchStock);
            return new JsonResult(new { success = true, data = resultdata });
        }
        public IActionResult OnPostExportData([FromBody] ScrapDataFilterParameter filterData)
        {
            DataResponse response = _materialRepository.GetScrapStockData(filterData);
            var data = response.Value as List<ScrapStockData>;

            if (data == null || !data.Any())
                return BadRequest("No data to export.");

            var selected = filterData.SelectedColumns ?? new List<string>();

            if (!selected.Any())
            {
                selected = new List<string> { "Material", "Size", "TotalScrap", "Sales", "Available" };
            }

            var grouped = data
                .GroupBy(x => x.ComponentTypeName)
                .OrderBy(g => g.Key)
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Scrap Stock");

                int col = 1;

                if (selected.Contains("Material")) ws.Cell(1, col++).Value = "MATERIAL NAME";
                if (selected.Contains("Size")) ws.Cell(1, col++).Value = "SIZE";
                if (selected.Contains("TotalScrap")) ws.Cell(1, col++).Value = "TOTAL SCRAP";
                if (selected.Contains("Sales")) ws.Cell(1, col++).Value = "SALES";
                if (selected.Contains("Available")) ws.Cell(1, col++).Value = "AVAILABLE SCRAP";

                var header = ws.Range(1, 1, 1, col - 1);
                header.Style.Font.Bold = true;
                header.Style.Fill.BackgroundColor = XLColor.LightGray;
                header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                header.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                header.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                int row = 2;

                foreach (var group in grouped)
                {
                    var material = group.Key;
                    var rows = group.OrderBy(x => x.SizeName).ToList();
                    int rowCount = rows.Count;

                    bool materialMerged = false;

                    foreach (var item in rows)
                    {
                        int c = 1;

                        if (selected.Contains("Material"))
                        {
                            if (!materialMerged)
                            {
                                ws.Range(row, c, row + rowCount - 1, c).Merge().Value = material;

                                ws.Cell(row, c).Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                                ws.Cell(row, c).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                                ws.Cell(row, c).Style.Font.Bold = true;

                                materialMerged = true;
                            }

                            c++;
                        }

                        if (selected.Contains("Size"))
                            ws.Cell(row, c++).Value = item.SizeName;

                        if (selected.Contains("TotalScrap"))
                            ws.Cell(row, c++).Value = item.TotalScrap == 0 ? "NIL" : item.TotalScrap.ToString("0.##");

                        if (selected.Contains("Sales"))
                            ws.Cell(row, c++).Value = item.SoldQty == 0 ? "NIL" : item.SoldQty.ToString("0.##");

                        if (selected.Contains("Available"))
                            ws.Cell(row, c++).Value = item.BalanceQty == 0 ? "NIL" : item.BalanceQty.ToString("0.##");

                        row++;
                    }
                }

                ws.Columns().AdjustToContents();
                ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    string fileName = $"ScrapStockReport-{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    return File(
                        stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName
                    );
                }
            }
        }




    }
}
