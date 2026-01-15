var ConsumedData = [];
var BatchStockArr = [];
var prodbalance = 0;
var LaserMarkingArray = [];

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
$(function () {
    pLoadingSetup(false);
    $("#divLaserNoMarking").hide();
    $("#divSearchPage").show();
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
    GetWarehouseTabList();
    GetLaserNoMarkingCompleted();
    pLoadingSetup(true);
});
$('#divAddEditModal').on('shown.bs.modal', function () {
    $('#ddlRackLocation').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlMachineType').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlItem').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlEmployeee').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlComponentType').select2({ dropdownParent: $('#divHologramPunching'), width: '100%' });
    $('#ddlSize').select2({ dropdownParent: $('#divHologramPunching'), width: '100%' });
    $('#ddlColor').select2({ dropdownParent: $('#divHologramPunching'), width: '100%' });
});
function GetWarehouseTabList() {
    $.ajax({
        url: GetWarehouseTabListUrl,
        type: 'GET',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        success: function (response) {
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
            // Final HTML
            const html = `
                <div class="card" id="divWarehouse">
                    <div class="card-body ">
                        ${navTabs}
                        ${tabContent}
                    </div>
                </div>`;

            $("#divListingWarehouse").html(html);
            // Load data for the first tab
            if (response.data.length > 0) {
                GetHologramPunchingByWarehouseID(response.data[0].WareHouseID);
            }
            else {
                const html = `<div class="alert alert-info mt-2 text-center" role="alert">
                    No data available for Laser No Marking
                </div>`;
                $("#divListingWarehouse").html(html);
            }

        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) //console.log(error);
                Swal.fire({
                    title: "Error",
                    text: xhr.responseText,
                    icon: "error",
                    confirmButtonColor: "#556ee6"
                });
        }
    });
}
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
        data: { ID: warehouseId, ComponentTypeID: HOLOGRAMPLATE },
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
                                                      <span class="text-info fw-bold"> ${item.HologramFinishedQty}</span> | Consumed Plates: 
                                                      <span class="text-success fw-bold">${item.ProdConsumedQty}</span> | Wastage Plates: 
                                                      <span class="text-danger fw-bold">${item.ProdWastageQty}</span>
                                                    </p>
                                                </td>
                                                 <td style="padding: 0.3rem;">
                                                    <span class=" badge ${item.ColorCode}">${item.StockStatus}</span>
                                                </td>
                                                <td style="width: 90px; padding: 0.5rem;">
                                                    <div>
                                                        <button class="btn btn-sm btn-soft-primary" onclick="ProccedData('${item.OutputBatchStockID || 0}')">
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
        $(`#${containerId}`).html('<div class="alert alert-info" role="alert">No hologram punching data available for this warehouse.</div>');
    }
}
function ProccedData(id) {
    var stockrequestid = $('#hdnStockRequestID').val();
    prodbalanc = 0;
    $.ajax({
        url: GetHologramPuchingDetailsByBatchIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            $('#hdnBatchStockID').val(id);
            LaserMarkingArray = [];
            VHologramPunchingCompleted = [];
            if (response.data.VHologramPunchingCompleted.length > 0) BatchStockArr = response.data.VHologramPunchingCompleted;
            if (response.data.LaserNoMarking.length > 0) {
                LaserMarkingArray = response.data.LaserNoMarking;
            }
            DisplayLaserNoMarkingTransData();
            var data = response.data.VHologramPunchingCompleted[0];
            prodbalance = data.ProdBalanceQty;
            $("#divPendingStockRequestTrans").empty();
            $("#btnSaveProduction").show();
            $("#btnUpdateProduction").hide();
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
                     <td><i class="bx bx-buildings me-1 text-primary"></i>${data.HologramFinishedQty || ''} Pcs</td>
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

            $("#divLaserNoMarking").show();
            //$("#ddlItem").val(data.ItemID);
            $("#ddlComponentType").val(LASERNOPLATE).change();
            $("#ddlSize").val(data.SizeID).change();
            $("#ddlColor").val(data.ColorID).change();
            //GetHologramDetailsByID(id);

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
function BalanceQtyTable() {
    let colorCode = "bg-secondary bg-gradient text-white";
    $("#divProbableProduction").empty();

    let ProbableProduction = `    
        <table class="table table-sm align-middle" id="tblProbableProduction">
            <thead>
                <tr class="table-light">
                    <th class="${colorCode}">Component</th>
                    <th class="${colorCode} text-end"> Probable Production Quantity</th>
                </tr>
            </thead>
            <tbody>`;

    ProbableProduction += `
                <tr>
                    <td>Hologram Plates </td>
                    <td class="text-end text-danger"><b>${prodbalance} Pcs</b></td>
                </tr>`;

    ProbableProduction += `
            </tbody>
        </table>`;

    $("#divProbableProduction").append(ProbableProduction);
    return false;
}

$("#btnAddNew").on('click', function () {
    $('#btnSaveProduction').show();
    $('#btnUpdateProduction').hide();
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Production");
    ClearModuleFormFields();
    GetItemDropdownByFilter();
    BalanceQtyTable();
    //BalanceQtyTable();
    GetLaserNoMarking();
    return false;
});
function GetLaserNoMarking() {
    $.ajax({
        url:GetLaserNoMarkingUrl,
        type: 'GET',
        contentType: 'application/json',
        success: function (response) {
            if (response != null) {
                $('#txtStartingNo').val(response.data);
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
}


$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divLaserNoMarking").hide();
    $("#divSearchPage").show();
    GetWarehouseTabList();
    // GetHologramPunchingList(2);

    // Clear form fields
    ClearModuleFormFields();
    GetLaserNoMarkingCompleted();

    return false;
});
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
function ClearModuleFormFields() {
    // Clear validation feedback
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $('#ddlItem').val(0).trigger('change');
    $('#ddlMachineType').val(0).trigger('change');
    $('#ddlEmployeee').val(0).trigger('change');
    $('#ddlRackLocation').val(0).trigger('change');

    $('#txtstartTime').val('');
    $('#txtendTime').val('');
    $('#txtTotalTime').val('');
    $('#txtProductionDate').val(moment().format('DD/MM/YYYY'));
    $('#txtStartingNo').val('');
    $('#txtEndingNo').val('');
    $('#txtNoOfPlate').val('0.00');
    $('#txtRejectedPlate').val('0.00');
    $('#txtStartingLaserNo').val('CD0000000000');
    $('#txtEndingLaserNo').val('CD0000000000');

    $('#btnSaveProduction').show();
    $('#btnUpdateProduction').hide();

    return false;
}
function padLaserNo(value) {
    return 'CD' + value.toString().padStart(10, '0');
}
$('#txtEndingNo, #txtStartingNo').on('input', function () {
    var start = parseInt($('#txtStartingNo').val()) || 0;
    var end = parseInt($('#txtEndingNo').val()) || 0;

    if (end >= start && start !== 0) {
        var totalPlates = end - start + 1;
        $('#txtNoOfPlate').val(totalPlates);

        $('#txtStartingLaserNo').val(padLaserNo(start));
        $('#txtEndingLaserNo').val(padLaserNo(end));
    } else {
        $('#txtNoOfPlate').val('0.00');
        $('#txtStartingLaserNo').val('CD0000000000');
        $('#txtEndingLaserNo').val('CD0000000000');
    }
});
// Save or Update Hologram Punching Record
$("#btnSaveProduction,#btnUpdateProduction").on("click", function () {
    // Clear previous validation errors
    $(".invalid-feedback").remove();
    $(".form-control").removeClass("is-invalid");

    // IMPORTANT: Clear arrays at the beginning of each save operation
    ConsumedData = [];

    var LaserNoMarkingID = 0;
    if (this.id == "btnUpdateProduction" && $("#hdnLaserNoMarkingID").val() > 0) {
        LaserNoMarkingID = $("#hdnLaserNoMarkingID").val();
    }

    // Validation checks (keeping your existing validation)
    if ($('#ddlItem').val() == 0 || $('#ddlItem').val() == null) return markInvalid("#ddlItem", "Please select Item");
    if ($('#ddlMachineType').val() == 0 || $('#ddlMachineType').val() == null) return markInvalid("#ddlMachineType", "Please select Machine Type");
    if ($('#ddlEmployeee').val() == 0 || $('#ddlEmployeee').val() == null) return markInvalid("#ddlEmployeee", "Please select Operator");
    if ($('#txtstartTime').val().trim() === "") return markInvalid("#txtstartTime", "Please enter Start Time");
    if ($('#txtendTime').val().trim() === "") return markInvalid("#txtendTime", "Please enter End Time");
    if ($('#txtTotalTime').val().trim() === "") return markInvalid("#txtTotalTime", "Total Time Taken is required");
    if ($('#txtProductionDate').val().trim() === "") return markInvalid("#txtProductionDate", "Please select Production Date");
    if ($('#ddlRackLocation').val() == 0 || $('#ddlRackLocation').val() == null) return markInvalid("#ddlRackLocation", "Please select Rack Location");

    if (!$("#chkFullyReject").is(":checked")) {
        if ($('#txtStartingNo').val().trim() === "") return markInvalid("#txtStartingNo", "Please enter Starting No");
        if ($('#txtEndingNo').val().trim() === "") return markInvalid("#txtEndingNo", "Please enter Ending No");
        if ($('#txtNoOfPlate').val().trim() === "" || $('#txtNoOfPlate').val() == 0) return markInvalid("#txtNoOfPlate", "Plate count is required");
        if ($('#txtStartingLaserNo').val().trim() === "") return markInvalid("#txtStartingLaserNo", "Starting Laser No is required");
        if ($('#txtEndingLaserNo').val().trim() === "") return markInvalid("#txtEndingLaserNo", "Ending Laser No is required");

        const startingno = parseInt($('#txtStartingNo').val());
        const endingno = parseInt($('#txtEndingNo').val());

        if (startingno >= endingno) return markInvalid("#txtEndingNo", "Ending No Should be Greater than Starting No");
    }
    else {
        $('#txtStartingNo').val(0);
        $('#txtEndingNo').val(0);
        $('#txtNoOfPlate').val(0);
        $('#txtStartingLaserNo').val("");
        $('#txtStartingLaserNo').val("");
    }
   

    const startTimeStr = $('#txtstartTime').val();
    const endTimeStr = $('#txtendTime').val();
    const ProductionDate = $('#txtProductionDate').val();
    const totalMinutes = getTimeDifferenceInMinutes(startTimeStr, endTimeStr);

    if (totalMinutes <= 0 || isNaN(totalMinutes)) {
        return markInvalid("#txtTotalTime", "Invalid Start/End Time");
    }

        
        
   


    $('#txtTotalTime').val(totalMinutes + " minutes");


    var result = {
        LaserNoMarkingID: LaserNoMarkingID,
        OperatorID: parseInt($('#ddlEmployeee').val()),
        OperatorName: $('#ddlEmployeee option:selected').text(),
        sProductionDate: $('#txtProductionDate').val(),
        ItemID: parseInt($('#ddlItem').val()),
        ItemName: $('#ddlItem option:selected').text(),
        MachineID: parseInt($('#ddlMachineType').val()),
        MachineName: $('#ddlMachineType option:selected').text(),
        RackLocationID: parseInt($('#ddlRackLocation').val()),
        RackLocationName: $('#ddlRackLocation option:selected').text(),
        WareHouseName: $('#ddlRackLocation option:selected').attr("WareHouseName") || '',
        sStartTime: startTimeStr,
        sEndTime: endTimeStr,
        TotalTime: totalMinutes + " minutes",
        StartingNo: parseInt($('#txtStartingNo').val()),
        EndingNo: parseInt($('#txtEndingNo').val()),
        NoOfPlate: parseFloat($('#txtNoOfPlate').val()),
        RejectedPlate: parseFloat($('#txtRejectedPlate').val()) || 0,
        StartingLaserNo: $('#txtStartingLaserNo').val(),
        EndingLaserNo: $('#txtEndingLaserNo').val(),
        InputBatchStockID: $('#hdnBatchStockID').val(),
        LaserNoConsumption: null
    };


    var consumedQty = 0, wastageQty = 0, totalqty = 0, wastagePercentage = 0, balanceQty = 0;
    if (this.id == "btnSaveProduction") {
        // Blank Plate Consumption - Add null checks
        var batchStockId = parseInt($('#hdnBatchStockID').val());
        var data = BatchStockArr.find(item => item.OutputBatchStockID === batchStockId);

        consumedQty = result.NoOfPlate;
        wastageQty = result.RejectedPlate;
        totalqty = result.RejectedPlate + result.NoOfPlate;
        wastagePercentage = (result.RejectedPlate > 0 && data.HologramFinishedQty > 0) ? (result.RejectedPlate / data.HologramFinishedQty) * 100 : 0;
        balanceQty = (data.ProdBalanceQty > 0) ? (data.ProdBalanceQty - totalqty) : (data.HologramFinishedQty - totalqty);

        if (((result.NoOfPlate + result.RejectedPlate) > prodbalance) && prodbalance != 0) {
            return markInvalid("#txtEndingNo", "Production Quantity exceeded the limit!. Minmium Probable Qty: " + (prodbalance));
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
        var batchStockId = parseInt($('#hdnBatchStockID').val());
        var editabledata = LaserMarkingArray.find(item => item.LaserNoMarkingID == LaserNoMarkingID);

        //Blank Plate

        var oldProductionQty = parseFloat(editabledata.NoOfPlate) || 0;
        var oldRejectedPlateQty = parseFloat(editabledata.RejectedPlate) || 0;

        var newProductionQty = parseFloat($('#txtNoOfPlate').val()) || 0;
        var newRejectedPlateQty = parseFloat($('#txtRejectedPlate').val()) || 0;

        var diffProduction = newProductionQty - oldProductionQty;
        var diffRejectedPlateQty = newRejectedPlateQty - oldRejectedPlateQty;
            
        var batchStockId = parseInt($('#hdnBatchStockID').val());
        var data = BatchStockArr.find(item => item.OutputBatchStockID === batchStockId);

        var actualconsumed = data.ProdConsumedQty || 0;
        var Wastage = data.ProdWastageQty || 0;

        consumedQty = oldProductionQty + diffProduction;
        wastageQty = oldRejectedPlateQty + diffRejectedPlateQty;
        totalqty = diffProduction + diffRejectedPlateQty;
        wastagePercentage = (result.RejectedPlate > 0 && data.HologramFinishedQty > 0) ? (result.RejectedPlate / data.HologramFinishedQty) * 100 : 0;
        balanceQty = (data.ProdBalanceQty > 0) ? (data.ProdBalanceQty - totalqty) : (data.HologramFinishedQty - totalqty);

        if (((diffProduction + diffRejectedPlateQty) > prodbalance) && prodbalance != 0) {
            return markInvalid("#txtEndingNo", "Production Quantity exceeded the limit!. Minmium Probable Qty: " + (prodbalance));
        }

        ConsumedData.push({
            ActualConsumedQty: consumedQty,
            WastageQty: wastageQty,
            WastagePercentage: wastagePercentage,
            BalanceQty: balanceQty,
            BatchStockID: batchStockId
        });

    }

    result.LaserNoConsumption = ConsumedData[0];

    // Don't push to BatchConsumedData here - let the server response handle updates
  

    if (result.LaserNoConsumption.BalanceQty == 0) {
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
                SaveOrUpdateLaserNoMarking(result);
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
        SaveOrUpdateLaserNoMarking(result);
    }
});
function SaveOrUpdateLaserNoMarking(OutputData) {

        $.ajax({
            url: SaveLaserNoMarkingUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(OutputData),
            success: function (response) {
                if (ENABLE_VERBOSE_Logging) console.log(response);

                if (response.success && !response.isExists) {
                    ProccedData($('#hdnBatchStockID').val());

                    Swal.fire({ title: OutputData.LaserNoMarkingID == 0 ? "Saved!" : "Updated!", text: OutputData.HologramPunchingID == 0 ? SaveSuccessMessage : UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                    if (OutputData.LaserNoConsumption.BalanceQty == 0) {
                        //$("#divLaserNoMarking").hide();
                        //$("#divSearchPage").show();
                        //GetWarehouseTabList();

                        //// Clear form fields
                        //ClearModuleFormFields();
                        //GetLaserNoMarkingCompleted();
                        window.location = "/Inventory/Production/LaserNoMarking";
                    }
                    else {
                        $('#btnCloseProduction').click();
                    }

                }
                else if (!response.success && response.isExists) {
                    Swal.fire({ title: "Data already exists!", text: response.message || "A duplicate record was found.", icon: "warning", confirmButtonColor: "#556ee6" });
                }
                else if (!response.success && !response.isExists) {
                    Swal.fire({ title: "Error", text: response.message || SaveErrorMessage, icon: "error", confirmButtonColor: "#556ee6" });
                }
                else {
                    Swal.fire({ title: "Error", text: response.message || "Unexpected error occurred.", icon: "error", confirmButtonColor: "#556ee6" });
                }
            },
            error: function (xhr, status, error) {
                if (ENABLE_VERBOSE_Logging) console.log(error);
                Swal.fire({ title: "Error", text: xhr.responseText || error, icon: "error", confirmButtonColor: "#556ee6" });
            }
        });

    return false;
}
function DisplayLaserNoMarkingTransData() {
    $("#divLaserNoMarkingTrans").empty();
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
                    <th>Duration</th>
                    <th>Total Time</th>
                    <th>Machine </th>
                    <th>Operator</th>
                    <th>Item</th>
                    <th>Location</th>
                    <th>Starting/Ending No.</th>
                    <th>No of Plate</th>
                    <th>Rejected Qty</th>
                    <th>Status</th>`;

    tableContent += `<th>Action</th>`;

    tableContent += `</tr>
            </thead>
            <tbody>`;

    // Declare summary totals
    let totalNoOfPlate = 0;
    let totalRejectedPlate = 0;

    if (LaserMarkingArray.length != 0) {
        LaserMarkingArray.forEach((entry, index) => {
            tableContent += `
                <tr data-sno="${entry.sNo}">
                    <td>${index + 1}</td>
                     <td>${entry.BatchNo}</td>
                    <td>${entry.sProductionDate}</td>
                    <td>${entry.sStartTime}-<br>${entry.sEndTime}</td>
                    <td>${entry.TotalTime} Mins</td>
                    <td>${entry.MachineName}</td>
                    <td>${entry.OperatorName}</td>
                    <td>${entry.ItemName}</td>
                    <td>${entry.RackLocationName} /<br> ${entry.WareHouseName}</td>
                    <td>${entry.StartingLaserNo} -<br>${entry.EndingLaserNo}</td>
                     <td>${entry.NoOfPlate}</td>
                    <td>${entry.RejectedPlate}</td>
                    <td> <span class="badge ${entry.ColorCode}">${entry.BatchStockStatus}</span></td>`;
            if (entry.StatusID == 1) {
                tableContent += `
                    <td class='text-center'>
                     <a href="javascript:void(0);" onclick="EditProduction(${entry.LaserNoMarkingID})"  class="btn btn-sm btn-soft-info" data-bs-toggle="modal" data-bs-target="#divAddEditModal">
                            <i class="mdi mdi-pencil-outline"></i>
                        </a>
                        <a href="javascript:void(0);" onclick="DeleteProduction('${entry.LaserNoMarkingID}')" class="btn btn-sm btn-soft-danger" title="Click here to Delete Stock Request Item">
                            <i class="mdi-delete-outline align-middle"></i>
                        </a>
                    </td>`;
            }
            else {
                tableContent += `<td></td>`;
            }

            tableContent += `</tr>`;
            totalNoOfPlate += parseFloat(entry.NoOfPlate) || 0;
            totalRejectedPlate += parseFloat(entry.RejectedPlate) || 0;
        });
        // Update totals

        let sfootercolorcode = "bg-info bg-gradient";
        // Add summary row
        tableContent += `
            <tr class="table-dark fw-bold">
                <td colspan="10" class="${sfootercolorcode} text-end">Total</td>
                <td class="${sfootercolorcode}">${totalNoOfPlate} Pcs</td>
                <td class="${sfootercolorcode}">${totalRejectedPlate} Pcs</td>
                 `;
        tableContent += `<td class="${sfootercolorcode}"></td>`;
        tableContent += `<td class="${sfootercolorcode}"></td>`;
        tableContent += `</tr>`;
    } else {
        tableContent += `<tr><td colspan="14" class="text-center">No Records To Display</td></tr>`;
    }

    tableContent += `
            </tbody>
        </table>
    </div>`;

    $("#divLaserNoMarkingTrans").html(tableContent);
    $("#btnCloseProduction").click();
}

$("#chkFullyReject").change(function () {
    if (this.checked) {
        $("#txtStartingNo").prop("disabled", true);
        $("#txtEndingNo").prop("disabled", true);
        $("#txtRejectedPlate").prop("disabled", true);
        $("#txtRejectedPlate").val(prodbalance);
    } else {
        $("#txtStartingNo").prop("disabled", false);
        $("#txtEndingNo").prop("disabled", false);
        $("#txtRejectedPlate").prop("disabled", false);
        $("#txtRejectedPlate").val(0);
    }
});

function EditProduction(id) {

    ClearModuleFormFields();
    GetItemDropdownByFilter();
    $('#btnSaveProduction').hide();
    $('#btnUpdateProduction').show();

    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Edit Hologram Punching");

    var editabledata = LaserMarkingArray.find(item => item.LaserNoMarkingID == id);

    $("#hdnLaserNoMarkingID").val(editabledata.LaserNoMarkingID);
    $('#ddlEmployeee').val(editabledata.OperatorID).change();
    $('#txtProductionDate').val(editabledata.sProductionDate);
    $('#ddlItem').val(editabledata.ItemID).change();
    $('#ddlMachineType').val(editabledata.MachineID).change();
    $('#ddlRackLocation').val(editabledata.RackLocationID).change();
    $('#txtstartTime').val(editabledata.sStartTime);
    $('#txtendTime').val(editabledata.sEndTime);
    $('#txtTotalTime').val(editabledata.TotalTime);
    $('#txtStartingNo').val(editabledata.StartingNo);
    $('#txtEndingNo').val(editabledata.EndingNo);
    $('#txtNoOfPlate').val(editabledata.NoOfPlate);
    $('#txtRejectedPlate').val(editabledata.RejectedPlate);
    $('#txtStartingLaserNo').val(editabledata.StartingLaserNo);
    $('#txtEndingLaserNo').val(editabledata.EndingLaserNo);

    BalanceQtyTable();

    // $("#divRecordLog").show();
    $("#spnLastUpdatedBy").html("Last Updated By: " + editabledata.LastUpdatedByName);
    $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(editabledata.LastUpdatedDate));
    return false;

}

// Delete a marking record with confirmation
function DeleteProduction(id) {
    if (ENABLE_VERBOSE_Logging) console.log(id);

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
$("#btnPunching").on('click', function () {
    if (LaserMarkingArray.length <= 0) {
        $.jGrowl("Kindly Add Atleast One Item.", { sticky: false, theme: 'warning', life: jGrowlLife });
        return false
    }
    if (prodbalance != 0) {
        Swal.fire({
            title: "Stock Still Available!",
            text: `There are still ${prodbalance} Hologram plates remaining in stock. Please consume them before closing.`,
            icon: "warning",
            confirmButtonColor: "#556ee6"
        });
        return false;
    }
    var batchstockid = $("#hdnBatchStockID").val();

    CompleteLaserNoMarking(batchstockid);

    return false;
});
function CompleteLaserNoMarking(id) {
    if (ENABLE_VERBOSE_Logging) console.log(id);
    $.ajax({
        url: CompleteLaserNoMarkingUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(id),
        success: function (response) {
            if (response.success && !response.isExists) {
                Swal.fire({ title: "Completed!", text: "Completed SuccessFully", icon: "success", confirmButtonColor: "#556ee6" });
                $("#btnClose").click();
                
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
function GetLaserNoMarkingCompleted() {
    $.ajax({
        url: GetLaserNoMarkingCompletedUrl,
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
                                <th>Location</th>
                                <th>Starting/Ending No.</th>
                                <th>No of Plate</th>
                                <th>Rejected Qty</th>
                                <th>Status</th>`;
            tableContent += `</tr>
                        </thead>
                        <tbody>`;

            // Declare summary totals
            let totalNoOfPlate = 0;
            let totalRejectedPlate = 0;

            if (response.data.length != 0) {
                response.data.forEach((entry, index) => {
                    tableContent += `
                        <tr data-sno="${entry.sNo}">
                           <td>${index + 1}</td>
                     <td>${entry.BatchNo}</td>
                    <td>${entry.sProductionDate}</td>
                    <td>${entry.sStartTime}-<br>${entry.sEndTime}</td>
                    <td>${entry.TotalTime} Mins</td>
                    <td>${entry.MachineName}</td>
                    <td>${entry.OperatorName}</td>
                    <td>${entry.ItemName}</td>
                    <td>${entry.RackLocationName} /<br> ${entry.WareHouseName}</td>
                    <td>${entry.StartingLaserNo} -<br>${entry.EndingLaserNo}</td>
                     <td>${entry.NoOfPlate}</td>
                    <td>${entry.RejectedPlate}</td>
                    <td> <span class="badge ${entry.ColorCode}">${entry.BatchStockStatus}</span></td>
                         </tr>`;

                    totalNoOfPlate += parseFloat(entry.NoOfPlate) || 0;
                    totalRejectedPlate += parseFloat(entry.RejectedPlate) || 0;

                });
                // Update totals

                let sfootercolorcode = "bg-primary bg-gradient";
                // Add summary row
                tableContent += `
                        <tr class="table-dark fw-bold">
                            <td colspan="10" class="${sfootercolorcode} text-end">Total</td>
                            <td class="${sfootercolorcode}">${totalNoOfPlate} Pcs</td>
                            <td class="${sfootercolorcode}">${totalRejectedPlate} Pcs</td> `;

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

