var ConsumedData = [];
var HolgoramPunchingArr = [];
var StockRequestArr = [];
var BatchConsumedData = [];
var TabId = 1;
var HologramQty = [];
var gStockRequestArr = [];
var BatchStockArr = [];
var ProbableProductionQuantity = 0;
var HydrolicPressureArr = [];
var prodbalance = 0;

//  Utility Methods
function getMaxSNo() {
    if (BatchConsumedData.length === 0) return 0;
    return Math.max(...BatchConsumedData.map(item => parseInt(item.sNo) || 0));
}
function getValidatedFloat(selector) {
    let value = parseFloat($(selector).val());
    return isNaN(value) ? 0 : value;
}
function getTimeDifferenceInMinutes(startTime, endTime) {
    const today = new Date().toISOString().split('T')[0]; // e.g. "2025-05-30"
    const start = new Date(`${today}T${startTime}`);
    const end = new Date(`${today}T${endTime}`);
    return Math.floor((end - start) / 60000); // Convert ms to minutes
}
function parseTimeToMinutes(timeStr) {
    const [timePart, modifier] = timeStr.trim().split(" ");
    let [hours, minutes] = timePart.split(":").map(Number);

    if (modifier === "PM" && hours !== 12) hours += 12;
    if (modifier === "AM" && hours === 12) hours = 0;

    return hours * 60 + minutes;
}

// Initialization on Load
$(function () {
    pLoadingSetup(false);
    $("#divHologramPunching").hide();
    $("#divSearchPage").show();
    //initDatePicker(); // Run on page load
    $("#txtProductionDate").datetimepicker({
        format: 'DD/MM/YYYY', // or use 'HH:mm' for 24-hour format
        pickTime: false,
        useCurrent: true,
        minDate: moment('1951-01-01'),
        maxDate: moment()
    });
    $("#txtProductionDate").val(moment().format('DD/MM/YYYY'));
    $(".ClearDate").click(function () { var x = $(this).prev().attr('id'); $("#" + x).val(""); });
    $(".SelectDate").click(function () { var x = $(this).next().attr('id'); $("#" + x).focus(); });
    // FilterPurchaseEntry();
    GetHologramPunchingList(1);
    pLoadingSetup(true);
});
document.addEventListener("DOMContentLoaded", function () {
    $('a[data-bs-toggle="tab"]').on('shown.bs.tab', function (e) {
        var target = $(e.target).attr("href");
        TabId = 0;
        if (target === "#Approved") {
            TabId = 1;
            // GetStockRequest(1);
        } else if (target === "#InProgress") {
            TabId = 2;
            //GetStockRequest(2);
        }
        else if (target === "#HologramCompleted") {
            TabId = 3;
            //GetStockRequest(3);
        }
        else if (target === "#Completed") {
            TabId = 4;
            //GetStockRequest(3);
        }
        GetHologramPunchingList(1);
    });

});

//Load Stock Requests into tabs (Hologram Request, Hologram Production, Production Completed)
function GetHologramPunchingList(type) {
    $.ajax({
        url: GetStockRequestUrl,
        type: 'GET',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: { ID: TabId },
        success: function (response) {
            StockRequestArr = response.data;
            if (type == 1) {


                if (TabId == 1) {
                    $("#btnAddNew").show();
                    $("#btnCompleted").show();
                    GetHologramSummary();
                    DisplayPendingforapprovalData(response.data, "divApprovedList", "tabApproved", "tblSRApproved");
                }
                else if (TabId == 2) {
                    $("#btnAddNew").show();
                    $("#btnCompleted").show();
                    GetHologramSummary();
                    DisplayPendingforapprovalData(response.data, "divInProgressList", "tabInProgress", "tblSRInProgress");
                }
                else if (TabId == 3) {
                    $("#btnAddNew").hide();
                    $("#btnCompleted").hide();
                    DisplayPendingforapprovalData(response.data, "divHologramCompletedList", "tabHologramCompleted", "tblSRHologramCompleted");
                    //GetHologramCompleted();
                }
                else if (TabId == 4) {
                    $("#btnAddNew").hide();
                    $("#btnCompleted").hide();
                    GetHologramPunchingCompleted();
                }
            }
            else if (type == 2) {
                var stId = $("#hdnStockRequestID").val();
                if (StockRequestArr.length > 0) {
                    var hologramdata = StockRequestArr.find(item => item.StockRequestID == stId);
                    $("#divHologramSummaryAlert").replaceWith(GetHologramAlertCard(hologramdata.VStockRequestTrans[0]));
                }
            }
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
function GetHologramSummary() {
    $("#divHologramSummary").empty();
    let content = `<div class="row justify-content-end">`; // Aligns content to right
    var totalHologram = 0;
    StockRequestArr.forEach((item) => {
        var hologramtotal = item.VStockRequestTrans[0].ProbableProdConsumedQty + item.VStockRequestTrans[0].ProdWastageQty
        if (item.VStockRequestTrans[0].BatchQuantity != hologramtotal)
            if (item.VStockRequestTrans[0].ProbableProductionQuantity == 0)
                totalHologram += item.VStockRequestTrans[0].Quantity;
            else
                totalHologram += item.VStockRequestTrans[0].ProbableProductionQuantity;
    });

    content += `
    <div class="col-auto">
        <div class="card mini-stats-wid shadow-sm" style="padding: 10px; min-width: 200px;">
            <div class="card-body p-2">
                <div class="text-center">
                    <span class="fs-6 text-muted">Total No. of Hologram</span>
                    <h3 class="text-danger fw-bold mb-0">${totalHologram} Pcs</h>
                </div>
            </div>
        </div>
    </div>`;

    content += `</div>`;
    $("#divHologramSummary").html(content);
}

// Show data in respective tab views
function DisplayPendingforapprovalData(dataList, divName, tabTitleId, tableId) {
    gStockRequestArr = dataList;

    // Destroy existing DataTable if it exists
    if ($.fn.DataTable.isDataTable('#' + tableId)) {
        $('#' + tableId).DataTable().destroy();
    }

    // Clear previous content
    $("#" + divName).empty();

    const hideProduction = tabTitleId === "tabHologramCompleted";
    const hideConsumed = tabTitleId !== "tabHologramCompleted";

    // Build the table HTML
    let tableContent = `<div class="table-responsive">
        <table class="table table-striped align-middle" id="${tableId}">
            <thead>
                <tr class="table-light">
                    <th>S No.</th>
                    <th>Request No</th>
                    <th>Date</th>
                    <th>Process Type</th>
                    <th>Requested By</th>
                    <th>Approved</th>
                    <th>Approved Dt</th>
                    <th class="${hideProduction ? 'd-none' : ''}">Probable Production Quantity</th>
                    <th class="${hideConsumed ? 'd-none' : ''}">Consumed Quantity</th>
                    <th>Status</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>`;

    if (dataList.length > 0) {
        dataList.forEach((entry, index) => {

            const finalqty = entry.VStockRequestTrans?.[0]?.ProbableProductionQuantity;

            const probableQty = (finalqty && finalqty !== 0)
                ? finalqty
                : entry.VStockRequestTrans?.[0]?.ConsumedQty ?? 0;

            //const probableQty = entry.VStockRequestTrans?.[0]?.ProbableProductionQuantity ?? 0;
            const consumedQty = entry.VStockRequestTrans?.[0]?.ConsumedQty ?? 0;

            tableContent += `
                <tr data-sno="${entry.sNo}">
                    <td>${index + 1}</td>
                    <td>${entry.RequestNo}</td>
                    <td>${entry.sRequestDate}</td>
                    <td>${entry.ProcessTypeName}</td>
                    <td>${entry.RequestedByName}</td>
                    <td>${entry.ApprovedByName}</td>
                    <td>${entry.sApprovedDate}</td>
                    <td class="${hideProduction ? 'd-none' : ''}">${probableQty} Pcs</td>
                    <td class="${hideConsumed ? 'd-none' : ''}">${consumedQty} Pcs</td>
                    <td><span class="badge ${entry.ColorCode}">${entry.StockRequestStatus}</span></td>
                    <td>
                        ${!hideProduction ? `
                        <button type="button" onclick="GetWarehouseTabList(${entry.StockRequestID})" class="btn btn-sm btn-outline-warning">
                            <i class="bx bx-package me-2"></i>Hologram Punching
                        </button>` : ''}
                    </td>
                </tr>`;
        });
    }

    tableContent += `</tbody></table></div>`;
    $("#" + divName).html(tableContent);

    // Update tab title count
    if (tabTitleId) {
        $("#" + tabTitleId).text(function (_, oldText) {
            return oldText.replace(/\(\d+\)/, '') + ` (${dataList.length})`;
        });
    }

    // Initialize DataTable
    $("#" + tableId).DataTable({
        bAutoWidth: false,
        bPaginate: false,
        bFilter: true,
        bSort: false,
        order: [],
        pagingType: "full_numbers"
    });
}

// Get and render warehouse-wise tab layout
function GetWarehouseTabList(id) {
    $.ajax({
        url: GetWarehouseTabListUrl,
        type: 'GET',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        success: function (response) {
            if (response.data.length === 0) {
                Swal.fire({
                    title: "Out of Stock!",
                    text: "Blank Plates are currently not available in our warehouse!",
                    icon: "warning",
                    confirmButtonColor: "#556ee6"
                });
                $("#divHologramPunching").hide();
                $("#divHologramPunching").hide();
                $("#divHologramSummaryAlert").hide();
                $("#divListingWarehouse").hide();

                $("#divSearchPage").show();

                GetHologramPunchingList(1);
                return false;
            }

            // IMPORTANT: Ensure proper div visibility at the start
            $("#divSearchPage").hide();
            $("#divHologramPunching").hide();
            $("#divHologramSummaryAlert").hide();
            $("#divListingWarehouse").show();

            $('#hdnStockRequestID').val(id);
            HologramQty = [];

            // Get Hologram Quantity
            const quantity = StockRequestArr.find(item => item.StockRequestID === id);

            if (quantity?.VStockRequestTrans?.[0]) {
                HologramQty.push(quantity.VStockRequestTrans[0]);
            }

            $("#divListingWarehouse").empty();

            if (!response || response.length === 0) return;

            // Build tab headers
            let navTabs = `<ul class="nav nav-pills nav-justified" role="tablist">`;
            let tabContent = `<div class="tab-content p-3 text-muted ">`;

            response.data.forEach((tab, index) => {
                const isActive = index === 0 ? "active" : "";
                const isSelected = index === 0 ? "true" : "false";
                const tabId = `warehouse_${tab.WareHouseID}`;

                navTabs += `
                    <li class="nav-item waves-effect waves-light" role="presentation">
                        <a class="nav-link ${isActive}" data-bs-toggle="tab" href="#${tabId}" role="tab" aria-selected="${isSelected}" data-warehouse-id="${tab.WareHouseID}">
                            <span class="d-block d-sm-none"><i class="fas fa-circle"></i></span>
                            <span class="d-none d-sm-block" id="tab${tab.WareHouseID}">${tab.WareHouseName}</span>
                        </a>
                    </li>`;

                tabContent += `
                    <div class="tab-pane ${isActive}" id="${tabId}" role="tabpanel">
                        <div id="div_${tab.WareHouseID}List"></div>
                    </div>`;
            });

            navTabs += `</ul>`;
            tabContent += `</div>`;

            const hologramAlert = GetHologramAlertCard(HologramQty[0]);

            // Footer with Close button
            const footerHtml = `
                <div class="card-footer  mt-3">
                    <button type="button" class="btn btn-outline-danger waves-effect" id="btnWarehouseClose">
                        <i class="fas fa-window-close font-size-16 me-2"></i>Close
                    </button>
                </div>`;

            // Final HTML
            const html = `
                ${hologramAlert}
                <div class="card" id="divWarehouse">
                    <div class="card-body ">
                        ${navTabs}
                        ${tabContent}
                    </div>
                    ${footerHtml}
                </div>`;

            $("#divListingWarehouse").html(html);

            // Load data for the first tab
            if (response.data.length > 0) {
                GetHologramPunchingByWarehouseID(response.data[0].WareHouseID);
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({
                title: "Error",
                text: xhr.responseText,
                icon: "error",
                confirmButtonColor: "#556ee6"
            });
        }
    });
}
function GetHologramAlertCard(data) {

    var hologramqtydata = data.ProbableProdConsumedQty + data.ProdWastageQty

    if (!data) return '';
    if (data.BatchQuantity != hologramqtydata) {
        ProbableProductionQuantity = data.ProbableProductionQuantity == 0
            ? data.Quantity
            : data.ProbableProductionQuantity
    }
    //else {
    //    $("#btnWarehouseClose").click();
    //    Swal.fire({
    //        title: "Out of Stock!",
    //        text: "Hologram are currently not available, Kindly Request the Stock!",
    //        icon: "warning",
    //        confirmButtonColor: "#556ee6"
    //    });

    //    return false;
    //}


    return `
        <div id="divHologramSummaryAlert" class="container-fluid mt-2">
            <div class="row text-center">
                <!-- Stock Request No -->
                <div class="col-lg-3 col-md-6 mb-2">
                    <div class="card shadow-sm">
                        <div class="card-body">
                            <p class="text-muted mb-1">Stock Request No.</p>
                            <h4 class="font-size-14">
                                <i class="bx bx-copy-alt me-1 text-primary"></i>${data.RequestNo}
                            </h4>
                        </div>
                    </div>
                </div>
                               
                <!-- Balance Hologram Qty -->
                <div class="col-lg-3 col-md-6 mb-2">
                    <div class="card shadow-sm">
                        <div class="card-body">
                            <p class="text-muted mb-1">Balance Hologram Qty</p> 
                            <h4 class="font-size-14 text-success">
                               <i class="bx bx-bookmark me-1 text-primary"></i>
                                <b>${ProbableProductionQuantity} Pcs</b>
                            </h4>
                        </div>
                    </div>
                </div>

                <!-- Consumed Hologram Qty -->
                <div class="col-lg-3 col-md-6 mb-2">
                    <div class="card shadow-sm">
                        <div class="card-body">
                            <p class="text-muted mb-1">Consumed Hologram Qty</p>
                            <h4 class="font-size-14 text-danger">
                                <i class="bx bx-bookmark me-1 text-primary"></i><b>${Math.round(data.ProbableProdConsumedQty)} Pcs</b>
                            </h4>
                        </div>
                    </div>
                </div>
                 <!-- Wastage Qty -->
                <div class="col-lg-3 col-md-6 mb-2">
                    <div class="card shadow-sm">
                        <div class="card-body">
                            <p class="text-muted mb-1">Wastage Qty</p>
                            <h4 class="font-size-14">
                                <i class="bx bx bx-trash me-1 text-primary"></i>${Math.round(data.ProdWastageQty)}
                            </h4>
                        </div>
                    </div>
                </div>
            </div>
        </div>`;
}

$(document).on('click', '#btnWarehouseClose', function () {
    $("#divListingWarehouse").hide();
    $("#divSearchPage").show();
    clearGlobalState();
    GetHologramPunchingList(1);
    // GetWarehouseTabList($("#hdnStockRequestID").val());
    return false;
});
// Tab switch handler
$(document).on('shown.bs.tab', '#divListingWarehouse a[data-bs-toggle="tab"]', function (e) {
    const warehouseId = $(e.target).data('warehouse-id');
    if (warehouseId) {
        GetHologramPunchingByWarehouseID(warehouseId);
    }
});

// Fetch Hologram Punching Data by Warehouse ID and render as cards
function GetHologramPunchingByWarehouseID(warehouseId) {
    $.ajax({
        url: GetHologramPunchingByWarehouseIDUrl,
        type: 'GET',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: { ID: warehouseId, ComponentTypeID: BLANKPLATE },
        success: function (response) {
            if (response && response.data) {
                HolgoramPunchingArr = response.data;
                DisplayHologramPunchingData(response.data, warehouseId);
            } else {
                $(`#div_${warehouseId}List`).html('<p class="text-muted">No hologram punching data available.</p>');
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({
                title: "Error",
                text: xhr.responseText,
                icon: "error",
                confirmButtonColor: "#556ee6"
            });
        }
    });
    return false;
}
function DisplayHologramPunchingData(dataList, warehouseId) {
    var stockRequestID = $("#hdnStockRequestID").val();
    const containerId = `div_${warehouseId}List`;

    $(`#${containerId}`).empty();

    if (dataList && dataList.length > 0) {
        // Group data by RackLocationID
        var groupedData = {};
        dataList.forEach(function (item) {
            var rackLocationId = item.RackLocationID || 0;
            if (!groupedData[rackLocationId]) {
                groupedData[rackLocationId] = {
                    RackLocationName: item.RackLocationName || 'N/A',
                    RackLocationID: rackLocationId,
                    items: []
                };
            }
            groupedData[rackLocationId].items.push(item);
        });

        // Summary counts
        var totalRackLocations = Object.keys(groupedData).length;
        var totalRecords = dataList.length;

        // Append summary alert
        var summaryHTML = `
            <div class="alert alert-success alert-dismissible fade show mb-3" role="alert">
                We have found <b>${totalRackLocations}</b> rack location(s) with <b>${totalRecords}</b> hologram punching record(s) for you!
            </div>
        `;
        $(`#${containerId}`).append(summaryHTML);

        var cardHTML = "<div class='row g-3'>";

        Object.keys(groupedData).forEach(function (rackLocationId) {
            var rackData = groupedData[rackLocationId];
            var items = rackData.items;

            cardHTML += `                   
                <div class="col-xl-4 col-lg-6 col-md-6">
                    <div class="card border border-primary" style="height: 300px; width: 100%;">
                        <div class="card-body p-2 d-flex flex-column" style="height: 100%;">
                            <div class="d-flex justify-content-between align-items-center mb-2">
                                <h5 class="card-title mb-0" style="font-size: 1rem;">
                                    Rack Location: 
                                    <span class="text-primary">
                                        ${rackData.RackLocationName}
                                    </span>
                                </h5>
                                <span class="badge bg-info">${items.length} Item(s)</span>
                            </div>  
                            
                            <div class="table-responsive flex-grow-1" style="overflow: hidden;">
                                <div data-simplebar style="height: 100%;">
                                    <table class="table table-nowrap align-middle table-hover mb-0">
                                        <tbody>`;

            items.forEach(function (item) {
                cardHTML += `
                                            <tr>
                                                <td style="padding: 0.5rem;">
                                                    <h5 class="text-truncate font-size-14 mb-1">
                                                        <a href="javascript: void(0);" class="text-dark fw-semibold">
                                                            ${item.ItemName || 'N/A'}
                                                        </a>
                                                        
                                                    </h5>
                                                    <p class="text-muted mb-0" style="font-size: 0.7rem;">
                                                        Batch: ${item.BatchNo || 'N/A'} | Date: ${item.sProductionDate || 'N/A'}  
                                                    </p>
                                                   <p class="text-muted mb-0" style="font-size: 0.7rem;">No. of Plates:
                                                      <span class="text-info fw-bold"> ${item.ProductionQty}</span> | Consumed Plates: 
                                                      <span class="text-success fw-bold">${item.ProdConsumedQty}</span>
                                                      | Wastage Plates:
                                                      <span class="text-danger fw-bold">${item.ProdWastageQty}</span>
                                                    </p>
                                                </td>
                                                 <td style="padding: 0.3rem;">
                                                    <span class=" badge ${item.ColorCode}">${item.Status}</span>
                                                </td>
                                                <td style="width: 90px; padding: 0.5rem;">
                                                    <div>
                                                        <button class="btn btn-sm btn-soft-primary" onclick="ProccedData('${item.BatchStockID || 0}')">
                                                            <i class="bx bx-log-in-circle"></i> Proceed
                                                        </button>
                                                    </div>
                                                </td>
                                            </tr>`;
            });

            cardHTML += `
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>`;
        });

        cardHTML += "</div>";
        $(`#${containerId}`).append(cardHTML);
    } else {
        //$(`#${containerId}`).html('<div class="alert alert-info" role="alert">No hologram punching data available for this warehouse.</div>');
        Swal.fire({
            title: "Out of Stock!",
            text: "Blank Plates are currently not available in our warehouse!",
            icon: "warning",
            confirmButtonColor: "#556ee6"
        });
        $("#btnWarehouseClose").click();
        return false;
    }
}

// Proceed to Punching input screen with selected batch info
function ProccedData(id) {
    var _StockRequestID = parseInt($('#hdnStockRequestID').val());
    //console.log(_StockRequestID)
    prodbalance = 0;

    // GetHydrolicPressureByIDUrl = `/Inventory/Production/HologramPunching?handler=DataListStockRequestID&ID=${id}&StockRequestID=${_StockRequestID}`;
    $.ajax({
        url: GetHydrolicPressureByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            $('#divHologramPunching').show();
            $('#hdnBatchStockID').val(id);
            var data = response.data;
            HydrolicPressureArr = data;
            prodbalance = data.BSProdBalanceQty;
            $("#divPendingStockRequestTrans").empty();
            var headerDetails = `
            <table class="table align-middle" id="tblSearchResult">
                <thead>
                    <tr>
                        <th>Batch No.</th>
                        <th>Production Date</th>   
                        <th>Warehouse Name</th>
                        <th>Rack Location</th>
                        <th>Item</th>
                        <th>Production Quantity</th>
                        <th>Actual Consumed Quantity</th>
                        <th>Wastage  Quantity</th>
                        <th>Balance  Quantity</th>
                    </tr>
                </thead>
                <tbody>
                <tr>
                    <td><i class="bx bx-copy-alt me-1 text-primary"></i>${data.BatchNo || ''}</td>
                    <td><i class="bx bx-calendar-check me-1 text-primary"></i>${data.sProductionDate || ''}</td>
                    <td><i class="bx bx-home me-1 text-primary"></i>${data.WareHouseName || ''}</td>
                    <td><i class="fas fa-box me-1 text-primary"></i>${data.RackLocationName || ''}</td>
                    <td><i class="bx bx-package me-1 text-primary"></i>${data.ItemName || ''}</td>
                     <td><i class="bx bx-buildings me-1 text-primary"></i>${data.ProductionQty || ''} Pcs</td>
                      <td><i class="bx bx-buildings me-1 text-primary"></i>${data.ProdConsumedQty || '0'} Pcs</td>
                      <td><i class="bx bx-buildings me-1 text-primary"></i>${data.ProdWastageQty || '0'} Pcs</td>
                      <td><i class="bx bx-buildings me-1 text-primary"></i>${prodbalance || '0'} Pcs</td>
                 <tr>
            <tbody>
        </table>
     `;

            $("#divPendingStockRequestTrans").append(headerDetails);

            $("#divWarehouse").hide();
            $("#divSearchPage").hide();

            $("#divHologramPunching").show();
            $("#divHologramSummaryAlert").show();
            //$("#ddlItem").val(data.ItemID);
            $("#ddlComponentType").val(HOLOGRAMPLATE).change();
            $("#ddlSize").val(data.SizeID).change();
            $("#ddlColor").val(data.ColorID).change();
            GetHologramDetailsByID(id);

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
function GetHologramDetailsByID(id) {
    var _StockRequestID = parseInt($('#hdnStockRequestID').val());
    $.ajax({
        url: GetHologramDetailsByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id, StockRequestID: _StockRequestID },
        success: function (response) {
            if (!response || !response.data) {
                //console.error("Invalid response format");
                return;
            }

            // Clear and update global arrays with fresh data
            if (response.data.BatchStock.length > 0) { BatchStockArr = []; BatchStockArr = response.data.BatchStock }
            HolgoramPunchingArr = response.data.HologramPunching || [];
            if (response.data.StockRequests.length > 0) {
                response.data.StockRequests[0].VStockRequestTrans = response.data.VStockRequestTrans
                StockRequestArr = response.data.StockRequests || []

            };

            if (response.data.VStockRequestTrans && response.data.VStockRequestTrans.length > 0) {
                HologramQty = [];
                HologramQty.push(response.data.VStockRequestTrans[0]);
                $("#divHologramSummaryAlert").replaceWith(GetHologramAlertCard(HologramQty[0]));
            }
            BalanceQtyTable();
            DisplayHologramPunchingTransData(HolgoramPunchingArr);


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

//Add New Punching Entry
$("#btnAddNew").on('click', function () {
    $('#btnSaveProduction').show();
    $('#btnUpdateProduction').hide();
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Production");
    ClearModuleFormFields();
    GetItemDropdownByFilter();
    //BalanceQtyTable();
    return false;
});
function BalanceQtyTable() {
    let colorCode = "bg-secondary bg-gradient text-white";
    $("#divProbableProduction").empty();
    let hologramQty = parseFloat(ProbableProductionQuantity) || 0;
    let batchStockId = parseInt($('#hdnBatchStockID').val());
    let data = BatchStockArr.find(item => item.BatchStockID === batchStockId);
    let blankPlateQty = parseFloat(data?.ProdBalanceQty) || 0;

    let minQty = Math.min(hologramQty, blankPlateQty);

    let hologramClass = (hologramQty === minQty) ? "text-danger fw-bold" : "";
    let blankPlateClass = (blankPlateQty === minQty) ? "text-danger fw-bold" : "";

    // Table rendering
    let ProbableProduction = `    
    <table class="table table-sm align-middle" id="tblProbableProduction">
        <thead>
            <tr class="table-light">
                <th class="${colorCode}">Component</th>
                <th class="${colorCode} text-end">Probable Production Quantity</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td>Hologram</td>
                <td class="text-end ${hologramClass}">${hologramQty} Pcs</td>
            </tr>
            <tr>
                <td>Blank Plate</td>
                <td class="text-end ${blankPlateClass}">${blankPlateQty} Pcs</td>
            </tr>
        </tbody>
    </table>`;

    $("#divProbableProduction").append(ProbableProduction);

    return false;
}

$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divHologramPunching").hide();
    $("#divWarehouse").show();
    var stockrequestid = parseInt($("#hdnStockRequestID").val());
    GetWarehouseTabList(stockrequestid);
    // GetHologramPunchingList(2);

    // Clear form fields
    ClearModuleFormFields();

    return false;
});


// Load dropdown data for Item and Rack Location
function GetItemDropdownByFilter() {
    var filterdata = new Object();
    filterdata.ComponentTypeID = parseInt($('#ddlComponentType').val());
    filterdata.SizeID = parseInt($('#ddlSize').val());
    filterdata.ColorID = parseInt($("#ddlColor").val());

    $.ajax({
        url: GetItemDropdownByFilterUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(filterdata),
        async: false,
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);

            $("#ddlItem").empty();
            $("#ddlItem").append("<option value='0'>--Select--</option>");

            $.each(response.data.Value, function (i, result) {
                $("#ddlItem").append("<option value='" + result.ItemID + "'>" + result.ItemName + "</option>");
                $('#ddlItem').val(result.ItemID).change();//for the filtered value 
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
    GetRackLocationDropdownByFilter(filterdata.ComponentTypeID);
}
function GetRackLocationDropdownByFilter(ComponentTypeID) {
    $.ajax({
        url: GetRackLocationDropdownByFilterUrl,
        type: 'GET',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: { ComponentTypeID: ComponentTypeID },
        async: false,
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            $("#ddlRackLocation").empty();
            $("#ddlRackLocation").append("<option value='0'>--Select--</option>");

            $.each(response.data.Value, function (i, result) {
                $("#ddlRackLocation").append(
                    "<option value='" + result.RackLocationID + "' WareHouseName='" + result.WareHouseName + "'>" +
                    result.RackLocationName + " (" + result.WareHouseName + ")" +
                    "</option>"
                );
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

// Form Reset and Select2 Bindings
$('#divAddEditModal').on('shown.bs.modal', function () {
    $('#ddlRackLocation').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlMachineType').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlItem').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlEmployeee').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlComponentType').select2({ dropdownParent: $('#divHologramPunching'), width: '100%' });
    $('#ddlSize').select2({ dropdownParent: $('#divHologramPunching'), width: '100%' });
    $('#ddlColor').select2({ dropdownParent: $('#divHologramPunching'), width: '100%' });
});
function ClearModuleFormFields() {
    //  $("#hdnSNo").val("0");

    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $('#ddlEmployeee').val(0);
    // $('#txtProductionDate').val('');
    $('#ddlMachineType').val(0);
    $('#ddlRackLocation').val(0);
    $('#txtHologramFinishedQty').val('');
    $('#txtHologramWastageQty').val('');
    $('#txtRejectedPlateQty').val('');
    $('#txtstartTime').val('');
    $('#txtendTime').val('');
    $('#txtTotalTime').val('');
    $("#txtProductionDate").val(moment().format('DD/MM/YYYY'));

    $("#btnSaveItem").show();

    return false;
}

// Auto-calculate total time from start/end
$("#txtstartTime, #txtendTime").on('change', function () {
    var starttime = $('#txtstartTime').val(); // e.g. "09:30"
    var endtime = $('#txtendTime').val();     // e.g. "12:15"

    if (starttime && endtime) {
        var startParts = starttime.split(':');
        var endParts = endtime.split(':');

        var startMinutes = parseInt(startParts[0]) * 60 + parseInt(startParts[1]);
        var endMinutes = parseInt(endParts[0]) * 60 + parseInt(endParts[1]);

        var totalMinutes = endMinutes - startMinutes;

        if (totalMinutes < 0) {
            $.jGrowl("End time must be after start time.", { sticky: false, theme: 'warning', life: jGrowlLife });
            $('#txtTotalTime').val('');
        } else {
            $('#txtTotalTime').val(totalMinutes + " minutes");
        }
    }
});

// Save or Update Hologram Punching Record
$("#btnSaveProduction,#btnUpdateProduction").on("click", function () {
    // Clear previous validation errors
    $(".invalid-feedback").remove();
    $(".form-control").removeClass("is-invalid");

    // IMPORTANT: Clear arrays at the beginning of each save operation
    ConsumedData = [];

    var HologramPunchingID = 0;
    if (this.id == "btnUpdateProduction" && $("#hdnHologramPunchingID").val() > 0) {
        HologramPunchingID = $("#hdnHologramPunchingID").val();
    }

    // Validation checks (keeping your existing validation)
    if ($('#ddlMachineType').val() == 0) return markInvalid("#ddlMachineType", "Please select Machine Type");
    if ($('#ddlEmployeee').val() == 0) return markInvalid("#ddlEmployeee", "Please select Operator");
    if (!$('#txtProductionDate').val()) return markInvalid("#txtProductionDate", "Please select Production Date");
    if ($('#ddlItem').val() == 0) return markInvalid("#ddlItem", "Please select Item");
    if ($('#ddlRackLocation').val() == 0) return markInvalid("#ddlRackLocation", "Please select Rack Location");
    if (!$('#txtHologramFinishedQty').val()) return markInvalid("#txtHologramFinishedQty", "Please enter Hologram Finished Quantity");
    if (!$('#txtstartTime').val()) return markInvalid("#txtstartTime", "Please enter Start Time");
    if (!$('#txtendTime').val()) return markInvalid("#txtendTime", "Please enter End Time");

    const startTimeStr = $('#txtstartTime').val();
    const endTimeStr = $('#txtendTime').val();
    const ProductionDate = $('#txtProductionDate').val();
    const totalMinutes = getTimeDifferenceInMinutes(startTimeStr, endTimeStr);

    if (totalMinutes <= 0 || isNaN(totalMinutes)) {
        return markInvalid("#txtTotalTime", "Invalid Start/End Time");
    }

    $('#txtTotalTime').val(totalMinutes + " minutes");

    const maxSNo = getMaxSNo();
    var result = {
        sNo: maxSNo + 1,
        HologramPunchingID: HologramPunchingID,
        OperatorID: parseInt($('#ddlEmployeee').val()),
        OperatorName: $('#ddlEmployeee option:selected').text(),
        sProductionDate: $('#txtProductionDate').val(),
        HologramPlateID: parseInt($('#ddlItem').val()),
        ItemName: $('#ddlItem option:selected').text(),
        MachineID: parseInt($('#ddlMachineType').val()),
        MachineName: $('#ddlMachineType option:selected').text(),
        RackLocationID: parseInt($('#ddlRackLocation').val()),
        RackLocationName: $('#ddlRackLocation option:selected').text(),
        WareHouseName: $('#ddlRackLocation option:selected').attr("WareHouseName") || '',
        HologramFinishedQty: parseInt($('#txtHologramFinishedQty').val()) || 0,
        HologramWastageQty: parseInt($('#txtHologramWastageQty').val()) || 0,
        RejectedPlateQty: parseFloat($('#txtRejectedPlateQty').val()) || 0,
        sStartTime: startTimeStr,
        sEndTime: endTimeStr,
        TotalTime: totalMinutes + " minutes",
        StockRequestID: $('#hdnStockRequestID').val(),
        InputBatchStockID: $('#hdnBatchStockID').val(),
        HologramConsumption: null
    };

    // Qutantity Validation 
    var minProbableQty = 0;

    var batchStockId = parseInt($('#hdnBatchStockID').val());
    var ProdBalanceQty = BatchStockArr.find(item => item.BatchStockID === batchStockId).ProdBalanceQty;

    let totalProductionQty = 0, totalWastageQty = 0, totalRejectedQty = 0;

    $.each(HolgoramPunchingArr, function (index, item) {
        if (item.HologramPunchingID != $("#hdnHologramPunchingID").val()) {
            totalProductionQty += item.HologramFinishedQty || 0;
            totalWastageQty += item.HologramWastageQty || 0;
            totalRejectedQty += item.RejectedPlateQty
        }
    });

    var editabledata = HolgoramPunchingArr.find(item => item.HologramPunchingID == parseInt($("#hdnHologramPunchingID").val()));

    if (ProbableProductionQuantity > ProdBalanceQty) {
        minProbableQty = ProdBalanceQty;
        if ($("#hdnHologramPunchingID").val() > 0 && editabledata != null) {
            minProbableQty = minProbableQty + editabledata.HologramFinishedQty + editabledata.HologramWastageQty;
        }
        
    }
    else if (ProbableProductionQuantity < ProdBalanceQty) {
        minProbableQty = ProbableProductionQuantity;
        if ($("#hdnHologramPunchingID").val() > 0 && editabledata != null) {
            minProbableQty = minProbableQty + editabledata.HologramFinishedQty + editabledata.RejectedPlateQty;
        }
    }

    //if ($("#hdnHologramPunchingID").val() > 0) {
    //    minProbableQty = minProbableQty + editabledata.HologramFinishedQty
    //}

    //if (((result.HologramFinishedQty + result.HologramWastageQty) > minProbableQty || (result.HologramFinishedQty + result.RejectedPlateQty) > minProbableQty) && minProbableQty != 0) {
    //    return markInvalid("#txtProductionQty", "Production Quantity exceeded the limit!. Minmium Probable Qty: " + (minProbableQty));
    //}

    var consumedQty = 0, wastageQty = 0, totalqty = 0, wastagePercentage = 0, balanceQty = 0;
    if (this.id == "btnSaveProduction") {
        // Blank Plate Consumption - Add null checks
        var batchStockId = parseInt($('#hdnBatchStockID').val());
        var data = BatchStockArr.find(item => item.BatchStockID === batchStockId);

        consumedQty = result.HologramFinishedQty;
        wastageQty = result.RejectedPlateQty;
        totalqty = result.RejectedPlateQty + result.HologramFinishedQty;
        wastagePercentage = (result.RejectedPlateQty > 0 && data.ProductionQty > 0) ? (result.RejectedPlateQty / data.ProductionQty) * 100 : 0;
        balanceQty = (data.ProductionQty > 0) ? (data.ProdBalanceQty - totalqty) : (data.ProdBalanceQty - totalqty);

        if (result.HologramFinishedQty + result.RejectedPlateQty > ProdBalanceQty) {
            return markInvalid("#txtProductionQty",
                "Production Quantity exceeded the Blank Plate limit! Max Allowed: " + ProdBalanceQty);
        }


        ConsumedData.push({
            ActualConsumedQty: consumedQty,
            WastageQty: wastageQty,
            WastagePercentage: wastagePercentage,
            BalanceQty: balanceQty,
            BatchStockID: batchStockId
        });

        // Hologram - Add safety checks
        var stockrequestdata = gStockRequestArr.find((item) => item.StockRequestID == $('#hdnStockRequestID').val());

        var totalHologramqty = result.HologramWastageQty + result.HologramFinishedQty + HologramQty[0].ProbableProdConsumedQty + HologramQty[0].ProdWastageQty;
        var batchStockId = stockrequestdata.VStockRequestTrans[0].BatchStockID;

        consumedQty = result.HologramFinishedQty;
        wastageQty = result.HologramWastageQty;
        totalqty = result.HologramWastageQty + result.HologramFinishedQty;
        wastagePercentage = (result.HologramWastageQty > 0 && HologramQty[0].ProbableProductionQuantity > 0) ? (result.HologramWastageQty / HologramQty[0].ProbableProductionQuantity) * 100 : 0;
        balanceQty = HologramQty[0].ConsumedQty - totalHologramqty;

        if (result.HologramFinishedQty + result.HologramWastageQty > ProbableProductionQuantity) {
            return markInvalid("#txtProductionQty",
                "Production Quantity exceeded the Hologram limit! Max Allowed: " + ProbableProductionQuantity);
        }
        ConsumedData.push({
            ActualConsumedQty: consumedQty,
            WastageQty: wastageQty,
            WastagePercentage: wastagePercentage,
            BalanceQty: balanceQty,
            BatchStockID: batchStockId
        });
    }
    else if (this.id == "btnUpdateProduction") {
        var editabledata = HolgoramPunchingArr.find(item => item.HologramPunchingID == HologramPunchingID);

        //Blank Plate

        var oldProductionQty = parseFloat(editabledata.HologramFinishedQty) || 0;
        var oldRejectedPlateQty = parseFloat(editabledata.RejectedPlateQty) || 0;

        var newProductionQty = parseFloat($('#txtHologramFinishedQty').val()) || 0;
        var newRejectedPlateQty = parseFloat($('#txtRejectedPlateQty').val()) || 0;

        var diffProduction = newProductionQty - oldProductionQty;
        var diffRejectedPlateQty = newRejectedPlateQty - oldRejectedPlateQty;

        var batchStockId = parseInt($('#hdnBatchStockID').val());
        var data = BatchStockArr.find(item => item.BatchStockID === batchStockId);

        var actualconsumed = data.ProdConsumedQty || 0;
        var Wastage = data.ProdWastageQty || 0;

        consumedQty = oldProductionQty + diffProduction;
        wastageQty = oldRejectedPlateQty + diffRejectedPlateQty;
        totalqty = diffProduction + diffRejectedPlateQty;
        wastagePercentage = (result.RejectedPlateQty > 0 && data.ProductionQty > 0) ? (result.RejectedPlateQty / data.ProductionQty) * 100 : 0;
        balanceQty = (data.ConsumedQty > 0) ? (data.ConsumedQty - (totalqty + actualconsumed + Wastage)) : 0;

        if (diffProduction + diffRejectedPlateQty > ProdBalanceQty) {
            return markInvalid("#txtProductionQty",
                "Production Quantity exceeded the Blank Plate limit! Max Allowed: " + ProdBalanceQty);
        }

        ConsumedData.push({
            ActualConsumedQty: consumedQty,
            WastageQty: wastageQty,
            WastagePercentage: wastagePercentage,
            BalanceQty: balanceQty,
            BatchStockID: batchStockId
        });
        //Hologram

        var oldProductionQty = parseFloat(editabledata.HologramFinishedQty) || 0;
        var oldWastageQty = parseFloat(editabledata.HologramWastageQty) || 0;

        var newProductionQty = parseFloat($('#txtHologramFinishedQty').val()) || 0;
        var newWastageQty = parseFloat($('#txtHologramWastageQty').val()) || 0;

        var data = HydrolicPressureArr;

        var diffProduction = newProductionQty - oldProductionQty;
        var diffWastage = newWastageQty - oldWastageQty;
               
        var stockrequestdata = HologramQty.find((item) => item.StockRequestID == $('#hdnStockRequestID').val());
        var batchStockId = stockrequestdata.BatchStockID;

        var actualconsumed = stockrequestdata.ProbableProdConsumedQty || 0;
        var Wastage = stockrequestdata.ProdWastageQty || 0;

        consumedQty = oldProductionQty + diffProduction;
        wastageQty = oldWastageQty + diffWastage;
        totalqty = consumedQty + wastageQty;
        wastagePercentage = (result.HologramWastageQty > 0 && HologramQty[0].ProbableProductionQuantity > 0) ? (result.HologramWastageQty / HologramQty[0].ProbableProductionQuantity) * 100 : 0;
        balanceQty = HologramQty[0].ConsumedQty - totalqty;

        if (diffProduction + diffWastage > ProbableProductionQuantity) {
            return markInvalid("#txtProductionQty",
                "Production Quantity exceeded the Hologram limit! Max Allowed: " + ProbableProductionQuantity);
        }
        ConsumedData.push({
            ActualConsumedQty: consumedQty,
            WastageQty: wastageQty,
            WastagePercentage: wastagePercentage,
            BalanceQty: balanceQty,
            BatchStockID: batchStockId
        });
    }

    result.HologramConsumption = ConsumedData;

    if (result.HologramConsumption[0].BalanceQty == 0) {
        Swal.fire({
            title: "Stock will be Automatically completed!",
            text: "Balance quantity is zero. Do you want to proceed?",
            icon: "question",
            showCancelButton: true,
            confirmButtonText: "Yes, proceed",
            cancelButtonText: "No, cancel",
            customClass: {
                confirmButton: "btn btn-success mt-2",
                cancelButton: "btn btn-danger ms-2 mt-2"
            },
            buttonsStyling: false
        }).then(function (data) {
            if (data.value) {
                // Proceed with AJAX call if user confirms
                UpdateHologramPunching(result);
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                Swal.fire({
                    title: "Cancelled",
                    text: "Operation has been cancelled.",
                    icon: "error",
                    confirmButtonColor: "#556ee6"
                });
                return false;
            }
        });
    } else {
        // Directly process if BalanceQty != 0
        UpdateHologramPunching(result);
    }
});
function UpdateHologramPunching(OutputData) {
    $.ajax({
        url: UpdateHologramPunchingUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(OutputData),
        success: function (response) {
            if (response.success && !response.isExists) {

                ProccedData($('#hdnBatchStockID').val());
                if (OutputData.HologramPunchingID == 0)
                    Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                else
                    Swal.fire({ title: "Update!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });


                if (response.ProductionCompleted) {
                    //console.log("Production Completed");
                    //$('#btnCloseProduction').click();
                    //$('#btnClose').click();
                    //$('#btnWarehouseClose').click();
                    // GetHologramPunchingList(1);

                    $("#divHologramPunching").hide();
                    $("#divHologramSummaryAlert").hide();
                    $("#divSearchPage").show();
                    window.location = "/Inventory/Production/HologramPunching";
                }
                else if (OutputData.HologramConsumption[0].BalanceQty == 0)
                {
                      $("#divHologramPunching").hide();
                $("#divHologramSummaryAlert").hide();
                $("#divSearchPage").hide();

                // Reload warehouse list
                var stockrequestid = parseInt($("#hdnStockRequestID").val());
                GetWarehouseTabList(stockrequestid);
                }
                else {
                    $('#btnCloseProduction').click();
                }
            }
            else if (!response.success && response.isExists) {
                Swal.fire({
                    title: "Data already exists!",
                    text: "",
                    icon: "warning",
                    confirmButtonColor: "#556ee6"
                });
            }
            else if (!response.success && !response.isExists) {
                Swal.fire({
                    title: "Error",
                    text: SaveErrorMessage,
                    icon: "error",
                    confirmButtonColor: "#556ee6"
                });
            }
            else {
                Swal.fire({
                    title: "Error",
                    text: response.message,
                    icon: "error",
                    confirmButtonColor: "#556ee6"
                });
            }
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
function clearGlobalState() {
    HolgoramPunchingArr = [];
    BatchConsumedData = [];
    ConsumedData = [];
    StockRequestArr = [];
    HologramQty = [];
}

// Show saved punching transaction records
function DisplayHologramPunchingTransData(HologramPunching) {
    $("#divHologramPunchingTrans").empty();
    let tableContent = `<div class="alert alert-info mt-1" role="alert">
        <i class="bx bx-archive-in me-3"></i>Production Output
    </div>`;
    tableContent += '<div class="table-responsive">';
    tableContent += `
        <table class="table table-sm  align-middle" id="tblSRApproved">
            <thead>
                <tr class="table-light">
                    <th>S No.</th>
                     <th>Batch No.</th>
                    <th>Date</th>
                    <th>Start Time</th>
                    <th>End Time</th>
                    <th>Total Time</th>
                    <th>Machine </th>
                    <th>Operator</th>
                    <th>Item</th>
                    <th>WareHouse</th>
                    <th>RackLocation</th>
                    <th>Hologram Finished Qty</th>
                    <th>Wastage Qty</th>
                    <th>Rejected Plate Qty</th>
                    <th>Status</th>`;
    if (TabId < 3) {
        tableContent += `<th>Action</th>`;
    }
    tableContent += `</tr>
            </thead>
            <tbody>`;

    // Declare summary totals
    let totalHologramFinishedQty = 0;
    let totalHologramWastageQty = 0;
    let totalOtherWastageQty = 0;

    if (HologramPunching.length != 0) {
        HologramPunching.forEach((entry, index) => {
            tableContent += `
                <tr data-sno="${entry.sNo}">
                    <td>${index + 1}</td>
                     <td>${entry.BatchNo}</td>
                    <td>${entry.sProductionDate}</td>
                    <td>${entry.sStartTime}</td>
                    <td>${entry.sEndTime}</td>
                    <td>${entry.TotalTime} Mins</td>
                    <td>${entry.MachineName}</td>
                    <td>${entry.OperatorName}</td>
                    <td>${entry.ItemName}</td>
                    <td>${entry.WareHouseName}</td>
                    <td>${entry.RackLocationName}</td>
                    <td>${entry.HologramFinishedQty} Pcs</td>
                    <td>${entry.HologramWastageQty} Pcs</td>
                    <td>${entry.RejectedPlateQty} Pcs</td>
                    <td> <span class="badge ${entry.ColorCode}">${entry.StockStatus}</span></td>`;
            if (TabId < 3 && entry.StatusID <= 2) {
                tableContent += `
                    <td class='text-center'>
                     <a href="javascript:void(0);" onclick="EditProduction(${entry.HologramPunchingID})"  class="btn btn-sm btn-soft-info" data-bs-toggle="modal" data-bs-target="#divAddEditModal">
                            <i class="mdi mdi-pencil-outline"></i>
                        </a>
                        <a href="javascript:void(0);" onclick="DeleteProduction('${entry.HologramPunchingID}')" class="btn btn-sm btn-soft-danger" title="Click here to Delete Stock Request Item">
                            <i class="mdi-delete-outline align-middle"></i>
                        </a>
                    </td>`;
            }
            tableContent += `</tr>`;
            totalHologramFinishedQty += parseFloat(entry.HologramFinishedQty) || 0;
            totalHologramWastageQty += parseFloat(entry.HologramWastageQty) || 0;
            totalOtherWastageQty += parseFloat(entry.RejectedPlateQty) || 0;
        });
        // Update totals

        let sfootercolorcode = "bg-info bg-gradient";
        // Add summary row
        tableContent += `
            <tr class="table-dark fw-bold">
                <td colspan="11" class="${sfootercolorcode} text-end">Total</td>
                <td class="${sfootercolorcode}">${totalHologramFinishedQty} Pcs</td>
                <td class="${sfootercolorcode}">${totalHologramWastageQty} Pcs</td>
                <td class="${sfootercolorcode}">${totalOtherWastageQty} Pcs</td> `;
        if (TabId < 3) {
            tableContent += `<td class="${sfootercolorcode}"></td>`;
            tableContent += `<td class="${sfootercolorcode}"></td>`;
        }
        tableContent += `</tr>`;
    } else {
        tableContent += `<tr><td colspan="14" class="text-center">No Records To Display</td></tr>`;
    }

    tableContent += `
            </tbody>
        </table>
    </div>`;

    $("#divHologramPunchingTrans").html(tableContent);
    $("#btnCloseProduction").click();
}

// Delete a punching record with confirmation
function DeleteProduction(id) {
    if (ENABLE_VERBOSE_Logging) //console.log(id);

    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "question",
        showCancelButton: !0,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        customClass: { confirmButton: "btn btn-success mt-2", cancelButton: "btn btn-danger ms-2 mt-2" },
        buttonsStyling: !1,
    }).then(function (t) {
        t.value
            ? ConfirmDelete(id)
            : t.dismiss === Swal.DismissReason.cancel && Swal.fire({ title: "Cancelled", text: "Your data is safe :)", icon: "error" });
    });

    return false;
}
function ConfirmDelete(id) {
    $.ajax({
        url: DeleteDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(id),
        success: function (response) {
            if (response.success && !response.isExists) {
                Swal.fire({ title: "Deleted!", text: DeleteSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                GetHologramPunchingList(2);
                ProccedData($('#hdnBatchStockID').val());
            }
            else
                Swal.fire({ title: "Error", text: DeleteErrorMessage, icon: "warning", confirmButtonColor: "#556ee6" });
        },
        error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
    return false;
}

// Edit an existing punching entry
function EditProduction(id) {

    ClearModuleFormFields();
    GetItemDropdownByFilter();
    $('#btnSaveProduction').hide();
    $('#btnUpdateProduction').show();

    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Edit Hologram Punching");

    var editabledata = HolgoramPunchingArr.find(item => item.HologramPunchingID == id);

    $("#hdnHologramPunchingID").val(editabledata.HologramPunchingID);
    $('#ddlEmployeee').val(editabledata.OperatorID).change();
    $('#txtProductionDate').val(editabledata.sProductionDate);
    $('#ddlItem').val(editabledata.HologramPlateID).change();
    $('#ddlMachineType').val(editabledata.MachineID).change();
    $('#ddlRackLocation').val(editabledata.RackLocationID).change();
    $('#txtHologramFinishedQty').val(editabledata.HologramFinishedQty)
    $('#txtHologramWastageQty').val(editabledata.HologramWastageQty)
    $('#txtRejectedPlateQty').val(editabledata.RejectedPlateQty)
    $('#txtstartTime').val(editabledata.sStartTime)
    $('#txtendTime').val(editabledata.sEndTime)
    $('#txtTotalTime').val(editabledata.TotalTime)

    BalanceQtyTable();

    // $("#divRecordLog").show();
    $("#spnLastUpdatedBy").html("Last Updated By: " + editabledata.LastUpdatedByName);
    $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(editabledata.LastUpdatedDate));
    return false;

}

// Mark punching batch as completed
$("#btnCompleted").on('click', function () {
    if (HolgoramPunchingArr.length <= 0) {
        $.jGrowl("Kindly Add Atleast One Item.", { sticky: false, theme: 'warning', life: jGrowlLife });
        return false
    }
    if (prodbalance != 0) {
        Swal.fire({
            title: "Stock Still Available!",
            text: `There are still ${prodbalance} blank plates remaining in stock. Please consume them before closing.`,
            icon: "warning",
            confirmButtonColor: "#556ee6"
        });
        return false;
    }
    var batchstockid = HydrolicPressureArr.BatchStockID;

    CompleteHologramPunching(batchstockid);

    return false;
});
function CompleteHologramPunching(id) {
    if (ENABLE_VERBOSE_Logging) console.log(id);
    $.ajax({
        url: CompleteHologramPunchingUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(id),
        success: function (response) {
            if (response.success && !response.isExists) {
                Swal.fire({ title: "Completed!", text: "Completed SuccessFully", icon: "success", confirmButtonColor: "#556ee6" });
                $("#divHologramPunching").hide();
                $("#divHologramPunching").hide();
                $("#divHologramSummaryAlert").hide();

                $("#divSearchPage").show();

                GetHologramPunchingList(1);
            }
            else
                Swal.fire({ title: "Error", text: DeleteErrorMessage, icon: "warning", confirmButtonColor: "#556ee6" });
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
function GetHologramPunchingCompleted() {
    $.ajax({
        url: GetHologramPunchingCompletedUrl,
        type: 'GET',
        contentType: 'application/json',
        success: function (response) {

            $("#divCompletedList").empty();

            let tableContent = '<div class="table-responsive">';
            tableContent += `
                    <table class="table  align-middle" id="tblSRApproved">
                        <thead>
                            <tr class="table-light">
                                <th>S No.</th>
                                 <th>Batch No.</th>
                                <th>Date</th>
                                <th>Duration</th>
                                <th>Total Time</th>
                                <th>Machine </th>
                                <th>Operator</th>
                                <th>Item</th>
                                <th>WareHouse</th>
                                <th>RackLocation</th>
                                <th>Hologram Finished Qty</th>
                                <th>Wastage Qty</th>
                                <th>Rejected Plate Qty</th>
                                <th>Status</th>`;
            if (TabId < 3) {
                tableContent += `<th>Action</th>`;
            }
            tableContent += `</tr>
                        </thead>
                        <tbody>`;

            // Declare summary totals
            let totalHologramFinishedQty = 0;
            let totalHologramWastageQty = 0;
            let totalOtherWastageQty = 0;

            if (response.data.length != 0) {
                response.data.forEach((entry, index) => {
                    tableContent += `
                        <tr data-sno="${entry.sNo}">
                            <td>${index + 1}</td>
                             <td>${entry.BatchNo}</td>
                            <td>${entry.sProductionDate}</td>
                            <td>${entry.sStartTime}-${entry.sEndTime}</td>
                            <td>${entry.TotalTime} Mins</td>
                            <td>${entry.MachineName}</td>
                            <td>${entry.OperatorName}</td>
                            <td>${entry.ItemName}</td>
                            <td>${entry.WareHouseName}</td>
                            <td>${entry.RackLocationName}</td>
                            <td>${entry.HologramFinishedQty} Pcs</td>
                            <td>${entry.HologramWastageQty} Pcs</td>
                            <td>${entry.RejectedPlateQty} Pcs</td>
                            <td> <span class="badge ${entry.ColorCode}">${entry.StockStatus}</span></td>
                         </tr>`;

                    totalHologramFinishedQty += parseFloat(entry.HologramFinishedQty) || 0;
                    totalHologramWastageQty += parseFloat(entry.HologramWastageQty) || 0;
                    totalOtherWastageQty += parseFloat(entry.RejectedPlateQty) || 0;
                });
                // Update totals

                let sfootercolorcode = "bg-primary bg-gradient";
                // Add summary row
                tableContent += `
                        <tr class="table-dark fw-bold">
                            <td colspan="10" class="${sfootercolorcode} text-end">Total</td>
                            <td class="${sfootercolorcode}">${totalHologramFinishedQty} Pcs</td>
                            <td class="${sfootercolorcode}">${totalHologramWastageQty} Pcs</td>
                            <td class="${sfootercolorcode}">${totalOtherWastageQty} Pcs</td> `;

                tableContent += `<td class="${sfootercolorcode}"></td>`;


                tableContent += `</tr>`;
            } else {
                tableContent += `<tr><td colspan="13" class="text-center">No Records To Display</td></tr>`;
            }

            tableContent += `
                        </tbody>
                    </table>
                </div>`;

            $("#divCompletedList").html(tableContent);
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

