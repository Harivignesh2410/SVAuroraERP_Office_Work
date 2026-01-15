$(function () {
    pLoadingSetup(false);
    //GetPurchasePendingData();

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

    FilterData.ItemID = $('#ddlItem').val();
    FilterData.ComponentTypeID = $('#ddlComponentType').val();
    FilterData.SearchInWord = $('#txtSearchbox').val();

    GetPendingInspectionByFilter(FilterData);
}
function GetPendingInspectionByFilter(FilterData) {
    $.ajax({
        url: GetPendingInspectionByFilterUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(FilterData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
            DisplayPendingInspectionData(response.data);
            $("btnFilter").hide();
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) //console.log(error);
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
    $("#ddlItem").val("0").change();
    $("#ddlComponentType").val("0").change();
    FilterPurchaseEntry();
    return false;
});
function DisplayPendingInspectionData(pendingdata) {
    const groupedData = groupByComponentType(pendingdata);
    $("#divSearchResultSummary").empty();

    let accordionContent = `
        <div class="accordion accordion-flush" id="accordionComponentTypes">`;

    Object.keys(groupedData).forEach((componentType, index) => {
        let componentID = `component-${index}`;
        let expanded = index === 0 ? 'true' : 'false'; // First accordion opens by default

        let totalQuantity = 0;
        let totalBatchQuantity = 0;

        accordionContent += `
            <div class="accordion-item">
                <h2 class="accordion-header" id="heading${componentID}">
                    <button class="accordion-button ${expanded === 'true' ? '' : 'collapsed'}" type="button" data-bs-toggle="collapse" 
                        data-bs-target="#collapse${componentID}" aria-expanded="${expanded}" aria-controls="collapse${componentID}">
                        <strong>${componentType}</strong>
                    </button>
                </h2>
                <div id="collapse${componentID}" class="accordion-collapse collapse ${expanded === 'true' ? 'show' : ''}" 
                    aria-labelledby="heading${componentID}" data-bs-parent="#accordionComponentTypes">
                    <div class="accordion-body">
                        <div class="table-responsive">
                            <table class="table table-striped align-center">
                                <thead class="table-light">
                                    <tr class="table-info">
                                        <th>S.No</th>
                                        <th>Code</th>
                                        <th>Item</th>
                                        <th>HSN Code</th>
                                        <th>Colour</th>
                                        <th>Size</th>
                                        <th>Quantity</th>
                                        <th>Units</th>
                                        <th>Batch No</th>
                                        <th class='text-end'>Batch Quantity</th>
                                    </tr>
                                </thead>
                                <tbody>`;

        if (groupedData[componentType].length !== 0) {
            groupedData[componentType].forEach((entry, index) => {
                totalQuantity += entry.Quantity;
                totalBatchQuantity += entry.BatchQuantity;

                accordionContent += `
                                    <tr>
                                        <td>${index + 1}</td>
                                        <td>${entry.ItemCode}</td>
                                        <td>${entry.ItemName}</td>
                                        <td>${entry.HSNCode}</td>
                                        <td>${entry.ColorName}</td>
                                        <td>${entry.SizeName}</td>
                                        <td>${entry.Quantity.toFixed(2)}</td>
                                        <td>${entry.UnitName}</td>
                                        <td>${entry.BatchNo}</td>
                                        <td class='text-end'>${entry.BatchQuantity.toFixed(2)}</td>
                                    </tr>`;
            });

            // Add Summary Row
            accordionContent += `
                                    <tr class="table-warning fw-bold">
                                        <td colspan="6" class="text-center">Total Summary</td>
                                        <td>${totalQuantity.toFixed(2)}</td>
                                        <td></td>
                                        <td></td>
                                        <td class='text-end'>${totalBatchQuantity.toFixed(2)}</td>
                                    </tr>`;
        } else {
            accordionContent += `<tr><td colspan="10" class="text-center">No Batch Records To Display</td></tr>`;
        }

        accordionContent += `
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            </div>`;
    });

    accordionContent += `</div>`;

    $("#divSearchResultSummary").html(accordionContent);
}
function groupByComponentType(data) {
    return data.reduce((acc, item) => {
        const key = item.ComponentTypeName;
        if (!acc[key]) {
            acc[key] = [];
        }
        acc[key].push(item);
        return acc;
    }, {});
}