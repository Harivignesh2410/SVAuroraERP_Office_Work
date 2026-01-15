$(function () {
    pLoadingSetup(false);

    $("#divSearchPage").show();
    $("#divSearchResultSummary").show();

    // Select All Event
    $("#chkSelectAll").on("change", function () {
        $(".col-select").prop("checked", $(this).is(":checked"));
        FilterBatchstockdata();
    });

    // Individual column checkbox
    $(".col-select").on("change", function () {
        let allChecked = $(".col-select").length === $(".col-select:checked").length;
        $("#chkSelectAll").prop("checked", allChecked);
        FilterBatchstockdata();
    });

    FilterBatchstockdata();

    pLoadingSetup(true);
});

$("#btnFilter").on('click', function () {
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    FilterBatchstockdata();

    return false;
});

$("#btnClearFilter").on('click', function () {
    $("#ddlSize").val("0").change();
    $("#ddlColor").val("0").change();
    $("#ddlComponentType").val("0").change();

    // Reset columns
    $(".col-select").prop("checked", true);
    $("#chkSelectAll").prop("checked", true);

    FilterBatchstockdata();
    return false;
});

// GET SELECTED COLUMNS
function getSelectedColumns() {
    let cols = [];
    $(".col-select:checked").each(function () {
        cols.push($(this).val());
    });
    return cols;
}

function FilterBatchstockdata() {
    var FilterData = {};

    FilterData.SizeID = $('#ddlSize').val();
    FilterData.ColorID = $('#ddlColor').val();
    FilterData.ComponentTypeID = $('#ddlComponentType').val();
    FilterData.SelectedColumns = getSelectedColumns();

    GetBatchStockByFilter(FilterData);
}

function GetBatchStockByFilter(FilterData) {
    $.ajax({
        url: GetBatchStockByFilterUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(FilterData),
        success: function (response) {
            buildGroupedTable(response.data);
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: "Error",
                text: error.responseText,
                icon: "error",
                confirmButtonColor: "#556ee6"
            });
        }
    });
}

function buildGroupedTable(data) {

    let selected = getSelectedColumns();

    let html = `
        <div class="table-responsive">
        <table class="table table-bordered table-striped table-hover align-middle" id="tblBatchStock">
            <thead class="table-info">
                <tr>`;

    if (selected.includes("Material")) html += `<th>MATERIAL NAME</th>`;
    if (selected.includes("Size")) html += `<th>SIZE</th>`;
    if (selected.includes("Color")) html += `<th>COLOUR</th>`;
    if (selected.includes("TotalInward")) html += `<th class="text-end">TOTAL INWARD</th>`;
    if (selected.includes("TotalConsumed")) html += `<th class="text-end">TOTAL CONSUMED</th>`;
    if (selected.includes("CurrentStock")) html += `<th class="text-end">CURRENT STOCK</th>`;
    if (selected.includes("ProbableQty")) html += `<th class="text-end">PROBABLE QTY</th>`;

    html += `</tr></thead><tbody>`;

    if (data.length > 0) {

        const grouped = {};

        data.forEach(row => {
            const material = row.ComponentTypeName;
            const size = row.SizeName;

            if (!grouped[material]) grouped[material] = {};
            if (!grouped[material][size]) grouped[material][size] = [];

            grouped[material][size].push(row);
        });

        for (const material in grouped) {

            const sizeGroup = grouped[material];
            const rowCount = Object.values(sizeGroup).length;
            let materialRendered = false;

            for (const size in sizeGroup) {

                const rows = sizeGroup[size];

                let totalInward = 0;
                let totalConsumed = 0;
                let currentStock = 0;
                let probableQty = 0;

                let unitName = rows[0].UnitName ?? "";
                let colorName = rows[0].ColorName ?? "-";

                html += `<tr>`;

                if (selected.includes("Material") && !materialRendered) {
                    html += `<td rowspan="${rowCount}" class="fw-bold">${material}</td>`;
                    materialRendered = true;
                }

                if (selected.includes("Size"))
                    html += `<td>${size}</td>`;

                if (selected.includes("Color"))
                    html += `<td>${colorName}</td>`;

                if (selected.includes("TotalInward"))
                    html += `<td class="text-end">${totalInward.toFixed(2)} ${unitName}</td>`;

                if (selected.includes("TotalConsumed"))
                    html += `<td class="text-end">${totalConsumed.toFixed(2)} ${unitName}</td>`;

                if (selected.includes("CurrentStock"))
                    html += `<td class="text-end">${currentStock.toFixed(2)} ${unitName}</td>`;

                if (selected.includes("ProbableQty"))
                    html += `<td class="text-end">${probableQty.toFixed(2)} ${unitName}</td>`;

                html += `</tr>`;
            }
        }


    } else {
        html += `<tr><td colspan="7" class="text-center">No data to Display</td></tr>`;
    }

    html += `</tbody></table></div>`;

    $("#divSearchResultSummary").html(html);
}


$("#btnExport").on('click', function () {

    $.jGrowl("Please wait, exporting data...", {
        sticky: false,
        theme: 'warning',
        life: jGrowlLife
    });

    const filterObject = {
        SizeID: $("#ddlSize").val() || 0,
        ColorID: $("#ddlColor").val() || 0,
        ComponentTypeID: $("#ddlComponentType").val() || 0,
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

            var filename = "RawMaterialStockReport-" +
                new Date().toISOString().replace(/T/, '_').replace(/\..+/, '') +
                ".xlsx";

            var blob = new Blob([data], {
                type: xhr.getResponseHeader("Content-Type")
            });

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
