// SCRAP REPORT COLUMN SETTINGS
const scrapColumns = [
    { key: "Material", label: "MATERIAL NAME" },
    { key: "Size", label: "SIZE" },
    { key: "TotalScrap", label: "TOTAL SCRAP" },
    { key: "Sales", label: "SALES" },
    { key: "Available", label: "AVAILABLE SCRAP" }
];

// SELECT ALL CHECKBOX
$(document).on("change", "#chkSelectAll", function () {
    $(".col-select").prop("checked", $(this).is(":checked"));
    FilterBatchstockdata();
});

// INDIVIDUAL CHECKBOX TOGGLE
$(document).on("change", ".col-select", function () {
    $("#chkSelectAll").prop(
        $(".col-select").length === $(".col-select:checked").length
    );
    FilterBatchstockdata();
});

function getSelectedColumns() {
    return $(".col-select:checked")
        .map(function () { return $(this).val(); })
        .get();
}

$(function () {
    pLoadingSetup(false);
    $("#divSearchPage").show();
    $("#divScrapStockSummary").show();
    FilterBatchstockdata();
    pLoadingSetup(true);
});

// FILTER BUTTON
$("#btnFilter").on('click', function () {
    FilterBatchstockdata();
    return false;
});

// CLEAR
$("#btnClearFilter").on('click', function () {
    $("#ddlSize").val("0").change();
    $("#ddlComponentType").val("0").change();

    $(".col-select").prop("checked", true);
    $("#chkSelectAll").prop("checked", true);

    FilterBatchstockdata();
    return false;
});

function FilterBatchstockdata() {

    var FilterData = {
        SizeID: $('#ddlSize').val(),
        ComponentTypeID: $('#ddlComponentType').val(),

        AluminumCoil: AluminiumCoil,
        BlankPlate: BLANKPLATE,
        HologramPlate: HOLOGRAMPLATE,

        SelectedColumns: getSelectedColumns()
    };

    GetScrabStockByFilter(FilterData);
}

function GetScrabStockByFilter(FilterData) {
    $.ajax({
        url: GetScrabStockByFilterUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(FilterData),
        success: function (response) {
            buildScrapStockTable(response.data.Value, FilterData.SelectedColumns);
        },
        error: function (xhr, status, error) {
            Swal.fire("Error", xhr.responseText || error, "error");
        }
    });
}

function buildScrapStockTable(data, selectedColumns) {

    if (!selectedColumns || selectedColumns.length === 0)
        selectedColumns = scrapColumns.map(c => c.key);

    let thead = "<tr>";
    selectedColumns.forEach(key => {
        let col = scrapColumns.find(c => c.key === key);
        thead += `<th>${col.label}</th>`;
    });
    thead += "</tr>";

    let grouped = {};
    data.forEach(r => {
        if (!grouped[r.ComponentTypeName])
            grouped[r.ComponentTypeName] = [];
        grouped[r.ComponentTypeName].push(r);
    });

    let tbody = "";

    for (const material in grouped) {
        let rows = grouped[material];
        let count = rows.length;
        let materialWritten = false;

        rows.forEach(r => {
            tbody += "<tr>";

            selectedColumns.forEach(key => {

                if (key === "Material") {
                    if (!materialWritten) {
                        tbody += `<td rowspan="${count}" class="fw-bold text-center" style="vertical-align: middle;">${material}</td>`;
                        materialWritten = true;
                    }
                }

                else if (key === "Size") {
                    tbody += `<td class="text-center">${r.SizeName}</td>`;
                }

                else if (key === "TotalScrap") {
                    tbody += `<td class="text-end">${formatValue(r.TotalScrap)}</td>`;
                }

                else if (key === "Sales") {
                    tbody += `<td class="text-end">${formatValue(r.SoldQty)}</td>`;
                }

                else if (key === "Available") {
                    tbody += `<td class="text-end">${formatValue(r.BalanceQty)}</td>`;
                }
            });

            tbody += "</tr>";
        });
    }

    let html = `
    <div class="table-responsive">
        <table class="table table-bordered align-middle">
            <thead class="table-info text-center">${thead}</thead>
            <tbody>${tbody}</tbody>
        </table>
    </div>
    `;
    $("#divScrapStockSummary").html(html);
}

function formatValue(v) {
    if (!v || v === 0) return "NIL";
    return parseFloat(v).toFixed(2);
}

// EXPORT
$("#btnExport").on('click', function () {

    $.jGrowl("Please wait, exporting data...", { sticky: false, theme: 'warning', life: jGrowlLife });

    var FilterData = {
        SizeID: $('#ddlSize').val(),
        ComponentTypeID: $('#ddlComponentType').val(),
        AluminumCoil: AluminiumCoil,
        BlankPlate: BLANKPLATE,
        HologramPlate: HOLOGRAMPLATE,
        SelectedColumns: getSelectedColumns()
    };

    $.ajax({
        url: ExportDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(FilterData),
        xhrFields: { responseType: 'blob' },
        success: function (data, status, xhr) {

            var filename = "ScrapStockReport-" + new Date().toISOString().replace(/T/, '_').replace(/\..+/, '') + ".xlsx";

            var blob = new Blob([data], { type: xhr.getResponseHeader("Content-Type") });

            var link = document.createElement('a');
            link.href = window.URL.createObjectURL(blob);
            link.download = filename;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        },
        error: function (xhr, status, error) {
            Swal.fire("Error", xhr.responseText || error, "error");
        }
    });

    return false;
});
