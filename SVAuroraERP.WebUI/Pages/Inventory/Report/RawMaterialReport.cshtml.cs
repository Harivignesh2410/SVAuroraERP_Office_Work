namespace SVAuroraERP.WebUI.Pages.Inventory.Report
{
    public class RawMaterialReportModel : BasePageModel
    {

        private readonly ILogger<RawMaterialReportModel> _logger = null;
        private readonly IAntiforgery _antiforgery;
        private readonly IComponentServiceRepository _componentrepository;
        private readonly ISizeServiceRepository _sizeservicerepository;
        private readonly IColorServiceRespository _colorServiceRespository;
        private const int PageControlID = (int)Common.Pages.RawMaterialReport;
        private readonly IPermissionServiceRepository _permissionrepository; 
        private readonly IPendingInspectionServiceRepository _materialRepository = null;

        public RawMaterialReportModel(
                           ILogger<RawMaterialReportModel> logger,
                           IAntiforgery antiforgery,
                           SessionService sessionService,
                           IComponentServiceRepository componentrepository,
                           ISizeServiceRepository sizeservicerepository,
                           IColorServiceRespository colorServiceRespository,
                           IPermissionServiceRepository permissionrepository,
                           IPendingInspectionServiceRepository materialRepository)
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

        public JsonResult OnPostBatchStockByFilter([FromBody] FilterRawMaterialData FilterForBatchStock)
        {
            var resultdata = _materialRepository.GetRawMaterialStockData(FilterForBatchStock);
            return new JsonResult(new { success = true, data = resultdata });
        }
        public IActionResult OnPostExportData([FromBody] FilterRawMaterialData filterData)
        {
            var list = _materialRepository.GetRawMaterialStockData(filterData);

            if (list == null || !list.Any())
                return BadRequest("No data to export.");

            var selectedCols = filterData.SelectedColumns ?? new List<string>();

            var groupedData = list
                .GroupBy(m => m.ComponentTypeName)
                .Select(mat => new
                {
                    Material = mat.Key,
                    Sizes = mat.GroupBy(s => s.SizeName)
                               .Select(size => new
                               {
                                   Size = size.Key,
                                   Color = size.First().ColorName,
                                   TotalInward = size.Sum(x => x.BatchQuantity),
                                   TotalConsumed = size.Sum(x => x.ConsumedQty),
                                   CurrentStock = size.Sum(x => x.BalanceQty),
                                   ProbableQty = size.Sum(x => x.ProbableProductionQuantity),
                                   UnitName = size.First().UnitName
                               }).ToList()
                }).ToList();

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Batch Stock Summary");

                int col = 1;

                if (selectedCols.Contains("Material")) ws.Cell(1, col++).Value = "MATERIAL NAME";
                if (selectedCols.Contains("Size")) ws.Cell(1, col++).Value = "SIZE";
                if (selectedCols.Contains("Color")) ws.Cell(1, col++).Value = "COLOUR";
                if (selectedCols.Contains("TotalInward")) ws.Cell(1, col++).Value = "TOTAL INWARD";
                if (selectedCols.Contains("TotalConsumed")) ws.Cell(1, col++).Value = "TOTAL CONSUMED";
                if (selectedCols.Contains("CurrentStock")) ws.Cell(1, col++).Value = "CURRENT STOCK";
                if (selectedCols.Contains("ProbableQty")) ws.Cell(1, col++).Value = "PROBABLE QTY";

                int row = 2;

                foreach (var mat in groupedData)
                {
                    foreach (var s in mat.Sizes)
                    {
                        int c = 1;

                        if (selectedCols.Contains("Material"))
                            ws.Cell(row, c++).Value = mat.Material;

                        if (selectedCols.Contains("Size"))
                            ws.Cell(row, c++).Value = s.Size;

                        if (selectedCols.Contains("Color"))
                            ws.Cell(row, c++).Value = s.Color;

                        if (selectedCols.Contains("TotalInward"))
                            ws.Cell(row, c++).Value = $"{s.TotalInward} {s.UnitName}";

                        if (selectedCols.Contains("TotalConsumed"))
                            ws.Cell(row, c++).Value = $"{s.TotalConsumed} {s.UnitName}";

                        if (selectedCols.Contains("CurrentStock"))
                            ws.Cell(row, c++).Value = $"{s.CurrentStock} {s.UnitName}";

                        if (selectedCols.Contains("ProbableQty"))
                            ws.Cell(row, c++).Value = $"{s.ProbableQty} {s.UnitName}";

                        row++;
                    }
                }

                ws.Columns().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    stream.Position = 0;

                    string excelName = $"BatchStockSummary-{DateTime.Now:yyyyMMddHHmmss}.xlsx";

                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        excelName);
                }
            }
        }


    }
}
