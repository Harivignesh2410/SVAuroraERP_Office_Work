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

    var rackLocationValue = $('#ddlRackLocation').val();
    FilterData.RackLocationID = rackLocationValue ? rackLocationValue : "0";

    FilterData.WareHouseID = $('#ddlWarehouse').val();

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
function DisplayBatchStockData(Completeddata) {

    $("#divSearchResultSummary").empty();

    let sColorCode = "bg-info bg-gradient text-white";
    let tableContent = "";

    let totalConsumedQuantity = 0;
    let totalBalanceQuantity = 0;
    let totalBatchQuantity = 0;
    let totalProdConsumedQty = 0;
    let totalProdWastageQty = 0;
    let totalProdBalanceQty = 0;

    tableContent += `
        <table class="table table-sm w-100" id="tblBatchStock">
            <thead>
                <tr>
                    <th class="${sColorCode}">S.No</th>
                    <th class="${sColorCode}">Batch No</th>
                    <th class="${sColorCode}">Item</th>
                    <th class="${sColorCode}">Component Type</th>
                    <th class="${sColorCode}">Colour</th>
                    <th class="${sColorCode}">Size</th>
                    <th class="${sColorCode}">Rack Location</th>
                    <th class="${sColorCode}">WareHouse</th>
                    <th class="${sColorCode} text-end">Batch<br>Qty</th>
                    <th class="${sColorCode} text-end">Consumed<br>Qty</th>
                    <th class="${sColorCode} text-end">Balance<br>Qty</th>
                    <th class="${sColorCode} text-end">Production Consumed<br>Qty</th>
                    <th class="${sColorCode} text-end">Production Wastage<br>Qty</th>
                    <th class="${sColorCode} text-end">Production Balance<br>Qty</th>
                </tr>
            </thead>
            <tbody>
    `;

    if (Completeddata && Completeddata.length > 0) {

        Completeddata.forEach((entry, index) => {

            totalBatchQuantity += entry.BatchQuantity;
            totalConsumedQuantity += entry.ConsumedQty;
            totalBalanceQuantity += entry.BalanceQty;
            totalProdConsumedQty += entry.ProdConsumedQty;
            totalProdWastageQty += entry.ProdWastageQty;
            totalProdBalanceQty += entry.ProdBalanceQty;

            tableContent += `
                <tr>
                    <td>${index + 1}</td>
                    <td>${entry.BatchNo}</td>
                    <td>${entry.ItemName}</td>
                    <td>${entry.ComponentTypeName}</td>
                    <td>${entry.ColorName}</td>
                    <td>${entry.SizeName}</td>
                    <td>${entry.RackLocationName}</td>
                    <td>${entry.WareHouseName}</td>
                    <td class="text-end">${entry.BatchQuantity.toFixed(2)} ${entry.UnitName}</td>
                    <td class="text-end">${entry.ConsumedQty.toFixed(2)} ${entry.UnitName}</td>
                    <td class="text-end">${entry.BalanceQty.toFixed(2)} ${entry.UnitName}</td>
                    <td class="text-end">${entry.ProdConsumedQty.toFixed(2)} ${entry.UnitName}</td>
                    <td class="text-end">${entry.ProdWastageQty.toFixed(2)} ${entry.UnitName}</td>
                    <td class="text-end">${entry.ProdBalanceQty.toFixed(2)} ${entry.UnitName}</td>
                </tr>
            `;
        });
    }

    tableContent += `
            </tbody>
            <tfoot>
                <tr class="table-info fw-bold">
                    <th colspan="8" class="text-end">TOTAL</th>
                    <th class="text-end">${totalBatchQuantity.toFixed(2)}</th>
                    <th class="text-end">${totalConsumedQuantity.toFixed(2)}</th>
                    <th class="text-end">${totalBalanceQuantity.toFixed(2)}</th>
                    <th class="text-end">${totalProdConsumedQty.toFixed(2)}</th>
                    <th class="text-end">${totalProdWastageQty.toFixed(2)}</th>
                    <th class="text-end">${totalProdBalanceQty.toFixed(2)}</th>
                </tr>
            </tfoot>
        </table>
    `;

    $("#divSearchResultSummary").html(tableContent);

    if ($.fn.DataTable.isDataTable('#tblBatchStock')) {
        $('#tblBatchStock').DataTable().destroy();
    }

    $("#tblBatchStock").DataTable({
        scrollY: 550,          
        scrollCollapse: true,
        paging: false,
        searching: true,
        ordering: false,
        info: false,
        autoWidth: false
    });
}

$("#btnClearFilter").on('click', function () {
    $('#txtSearchbox').val("");
    $("#ddlSize").val("0").change();
    $("#ddlColor").val("0").change();
    $("#ddlComponentType").val("0").change();
    FilterPurchaseEntry();
    return false;
});
$("#ddlWarehouse").on('change', function () {

    WareHouseID = parseInt($('#ddlWarehouse').val());

    GetRackLocationByWareHouseID(WareHouseID);
    return false;
});
function GetRackLocationByWareHouseID(WareHouseID) {
    $.ajax({
        url: GetRackLocationByWareHouseIDUrl,
        type: 'GET',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: { WareHouseID: WareHouseID },
        success: function (response) {
            $("#ddlRackLocation").empty();
            $("#ddlRackLocation").append("<option value='0'>--Select--</option>");

            $.each(response.data.Value, function (i, result) {
                $("#ddlRackLocation").append("<option value='" + result.RackLocationID + "'>" + result.RackLocationName + "</option>");
            });
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
}

