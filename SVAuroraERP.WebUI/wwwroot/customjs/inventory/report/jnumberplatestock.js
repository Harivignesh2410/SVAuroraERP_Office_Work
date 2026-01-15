// NUMBER PLATE REPORT COLUMN DEFINITIONS
const npColumns = [
    { key: "Size", label: "SIZE" },
    { key: "Colour", label: "COLOUR" },
    { key: "BlankPlate", label: "BLANK PLATE" },
    { key: "HologramPlate", label: "HOLOGRAM PLATE" },
    { key: "LaserMarkingPlate", label: "LASERMARKING PLATE" },
    { key: "Packing", label: "PACKING" }
];

// Select All checkbox
$(document).on("change", "#chkSelectAll", function () {
    $(".col-select").prop("checked", $(this).is(":checked"));
    FilterBatchstockdata();
});

// Individual checkbox triggers Select All toggle
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
    $("#divSearchResultSummary").show();
    FilterBatchstockdata();
    pLoadingSetup(true);
});

// FILTER BUTTON
$("#btnFilter").on('click', function () {
    FilterBatchstockdata();
    return false;
});

// CLEAR FILTER
$("#btnClearFilter").on('click', function () {
    $("#ddlSize").val("0").change();
    $("#ddlColor").val("0").change();
    $("#txtFromDate").val('');
    $("#txtToDate").val('');

    $(".col-select").prop("checked", true);
    $("#chkSelectAll").prop("checked", true);

    FilterBatchstockdata();
    return false;
});

// MAIN FILTER CALL
function FilterBatchstockdata() {

    var FilterData = {
        SizeID: $("#ddlSize").val(),
        ColorID: $("#ddlColor").val(),
        BlankPlateID: BLANKPLATE,
        HologramPlateID: HOLOGRAMPLATE,
        LaserMarkingPlateID: LASERNOPLATE,
        SelectedColumns: getSelectedColumns()
    };


    GetBatchStockByFilter(FilterData);
}

// AJAX CALL
function GetBatchStockByFilter(FilterData) {
    $.ajax({
        url: GetBatchStockByFilterUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(FilterData),
        success: function (response) {
            const data = response?.resultdata?.Value || [];
            buildNumberPlateStockTable(data, FilterData.SelectedColumns);
        },
        error: function (xhr, status, error) {
            Swal.fire("Error", xhr.responseText || error, "error");
        }
    });
}

// BUILD TABLE
function buildNumberPlateStockTable(data, selectedColumns) {

    if (!selectedColumns || selectedColumns.length === 0)
        selectedColumns = npColumns.map(c => c.key);

    let thead = "<tr>";
    selectedColumns.forEach(key => {
        let col = npColumns.find(c => c.key === key);
        thead += `<th>${col.label}</th>`;
    });
    thead += "</tr>";

    // Remove NONE-values
    let cleanData = data.filter(x => x.SizeName !== "NONE" && x.ColorName !== "NONE");

    // Group by SIZE
    const grouped = {};
    cleanData.forEach(row => {
        if (!grouped[row.SizeName]) grouped[row.SizeName] = [];
        grouped[row.SizeName].push(row);
    });

    let tbody = "";

    for (const size in grouped) {
        const rows = grouped[size];
        const count = rows.length;

        let sizeWritten = false;

        rows.forEach(r => {
            tbody += "<tr>";

            selectedColumns.forEach(key => {

                if (key === "Size") {
                    if (!sizeWritten) {
                        tbody += `<td rowspan="${count}" class="fw-bold text-center" style="vertical-align: middle;">${r.SizeName}</td>`;
                        sizeWritten = true;
                    }
                }
                else if (key === "Colour")
                    tbody += `<td>${r.ColorName}</td>`;

                else if (key === "BlankPlate")
                    tbody += `<td class="text-end">${formatValue(r.BlankPlate, r.UnitName)}</td>`;

                else if (key === "HologramPlate")
                    tbody += `<td class="text-end">${formatValue(r.HologramPlate, r.UnitName)}</td>`;

                else if (key === "LaserMarkingPlate")
                    tbody += `<td class="text-end">${formatValue(r.LaserMarkingPlate, r.UnitName)}</td>`;

                else if (key === "Packing")
                    tbody += `<td class="text-end">${formatValue(r.Packing, r.UnitName)}</td>`;
            });

            tbody += "</tr>";
        });
    }

    let html = `
        <div class="table-responsive">
            <table class="table table-bordered table-hover" id="tblNumberPlateStock">
                <thead class="table-info text-center">${thead}</thead>
                <tbody>${tbody}</tbody>
            </table>
        </div>
    `;

    $("#divSearchResultSummary").html(html);
}

function formatValue(value, unit) {
    if (!value || value === 0) return "NIL";
    return `${value} ${unit || ""}`.trim();
}

// EXPORT BUTTON
$("#btnExport").on('click', function () {

    $.jGrowl("Please wait, exporting data...", { sticky: false, theme: 'warning', life: jGrowlLife });

    const filterObject = {
        SizeID: $("#ddlSize").val(),
        ColorID: $("#ddlColor").val(),
        BlankPlateID: BLANKPLATE,
        HologramPlateID: HOLOGRAMPLATE,
        LaserMarkingPlateID: LASERNOPLATE,
        SelectedColumns: getSelectedColumns()
    };


    $.ajax({
        url: ExportDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(filterObject),
        xhrFields: { responseType: 'blob' },
        success: function (data, status, xhr) {
            var blob = new Blob([data], { type: xhr.getResponseHeader("Content-Type") });

            var cd = xhr.getResponseHeader("Content-Disposition");
            var filename = cd && cd.indexOf("filename=") !== -1
                ? cd.split("filename=")[1].replace(/"/g, "")
                : `NumberPlateStock-${new Date().toISOString()}.xlsx`;

            var link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            link.download = filename;
            link.click();
        },
        error: function (xhr, status, error) {
            Swal.fire("Error", xhr.responseText || error, "error");
        }
    });

    return false;
});
