$(function () {
    pLoadingSetup(false);

    $("#divSearchPage").show();
    $("#divSearchResultSummary").show();

    FilterPurchaseEntry();


    pLoadingSetup(true);
});
$("#btnFilter").on('click', function () {
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    FilterPurchaseEntry();

    return false;
});
function FilterPurchaseEntry() {
    var FilterData = new Object();

    FilterData.SizeID = $('#ddlSize').val();
    FilterData.ColorID = $('#ddlColor').val();
    FilterData.ComponentTypeID = $('#ddlComponentType').val();
    FilterData.SearchInWord = $('#txtSearchbox').val();
    FilterData.ReportTypeID = $('#ddlReportType').val();

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
            if (ENABLE_VERBOSE_Logging) //console.log(response);
            DisplayBatchStockData(response.data);
            $("btnFilter").hide();
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({
                title: "Error",
                text: error.responseText,
                icon: "error",
                confirmButtonColor: "#556ee6"
            });
        }
    });
    return false;
}

$("#btnClearFilter").on('click', function () {
    $('#txtSearchbox').val("");
    $("#ddlSize").val("0").change();
    $("#ddlColor").val("0").change();
    $("#ddlComponentType").val("0").change();
    FilterPurchaseEntry();
    return false;
});
function DisplayBatchStockData(Completeddata) {
    $("#divSearchResultSummary").empty();
    let accordionContent = `<div class="accordion accordion-flush" id="accordionComponentTypes">`;
    let sColorCode = "bg-info bg-gradient text-white";

    if (Completeddata.length !== 0) {
        let groupedData = {};

        // Group data by Component Type, Size, and Color
        Completeddata.forEach((entry) => {
            if (!groupedData[entry.ComponentTypeName]) {
                groupedData[entry.ComponentTypeName] = {};
            }
            if (!groupedData[entry.ComponentTypeName][entry.SizeName]) {
                groupedData[entry.ComponentTypeName][entry.SizeName] = {};
            }
            if (!groupedData[entry.ComponentTypeName][entry.SizeName][entry.ColorName]) {
                groupedData[entry.ComponentTypeName][entry.SizeName][entry.ColorName] = {
                    TotalQty: 0,
                    ConsumedQty: 0,
                    BalanceQty: 0
                };
            }

            // Aggregate quantities
            groupedData[entry.ComponentTypeName][entry.SizeName][entry.ColorName].TotalQty += entry.BatchQuantity;
            groupedData[entry.ComponentTypeName][entry.SizeName][entry.ColorName].ConsumedQty += entry.ConsumedQty;
            groupedData[entry.ComponentTypeName][entry.SizeName][entry.ColorName].BalanceQty += entry.BalanceQty;
        });

        // Generate accordion for each Component Type
        Object.keys(groupedData).forEach((component, index) => {
            let componentID = `component-${index}`;
            let expanded = index === 0 ? 'true' : 'false'; // First accordion is open by default

            accordionContent += `
                <div class="accordion-item">
                    <h2 class="accordion-header" id="heading${componentID}">
                        <button class="accordion-button ${expanded === 'true' ? '' : 'collapsed'}" type="button" data-bs-toggle="collapse" 
                            data-bs-target="#collapse${componentID}" aria-expanded="${expanded}" aria-controls="collapse${componentID}">
                            <strong>${component}</strong>
                        </button>
                    </h2>
                    <div id="collapse${componentID}" class="accordion-collapse collapse ${expanded === 'true' ? 'show' : ''}" 
                        aria-labelledby="heading${componentID}" data-bs-parent="#accordionComponentTypes">
                        <div class="accordion-body">
                            <div class="table-responsive">
                                <table class="table table-bordered table-lg">
                                    <thead>
                                        <tr class="table-info">
                                            <th class="${sColorCode}">Size</th>
                                            ${component.toUpperCase() === "ALUMINIUM COIL" ? "" : `<th class="${sColorCode}">Color</th>`}
                                            <th class="${sColorCode}">Total Quantity</th>
                                            <th class="${sColorCode}">Consumed Quantity</th>
                                            <th class="${sColorCode}">Balance Quantity</th>
                                        </tr>
                                    </thead>
                                    <tbody>`;

            let sizes = groupedData[component];

            if (component.toUpperCase() === "ALUMINIUM COIL") {
                // Aluminium Coil (Group only by Size)
                for (let size in sizes) {
                    let totalQty = 0, consumedQty = 0, balanceQty = 0;

                    // Sum up quantities across all colors
                    for (let color in sizes[size]) {
                        totalQty += sizes[size][color].TotalQty;
                        consumedQty += sizes[size][color].ConsumedQty;
                        balanceQty += sizes[size][color].BalanceQty;
                    }

                    accordionContent += `
                        <tr>
                            <td>${size}</td>
                            <td>${totalQty.toFixed(2)}</td>
                            <td>${consumedQty.toFixed(2)}</td>
                            <td>${balanceQty.toFixed(2)}</td>
                        </tr>`;
                }
            } else {
                // Other components (Group by Size and Color)
                for (let size in sizes) {
                    let firstColor = true;
                    for (let color in sizes[size]) {
                        accordionContent += `
                            <tr>
                                ${firstColor ? `<td rowspan="${Object.keys(sizes[size]).length}">${size}</td>` : ''}
                                <td>${color}</td>
                                <td>${sizes[size][color].TotalQty.toFixed(2)}</td>
                                <td>${sizes[size][color].ConsumedQty.toFixed(2)}</td>
                                <td>${sizes[size][color].BalanceQty.toFixed(2)}</td>
                            </tr>`;
                        firstColor = false;
                    }
                }
            }

            accordionContent += `</tbody></table></div></div></div></div>`;
        });
    } else {
        accordionContent += `<div class="text-center">No Batch Records To Display</div>`;
    }

    accordionContent += `</div>`;
    $("#divSearchResultSummary").html(accordionContent);


    $("#tblBatchStock").DataTable({
        "bAutoWidth": false,
        "bPaginate": false,
        "bFilter": true,
        "bSort": false,
        "order": [],
        "pagingType": "full_numbers"
    });


}
