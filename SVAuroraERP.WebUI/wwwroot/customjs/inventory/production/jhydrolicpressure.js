var ConsumedData = [];
var BatchConsumedData = [];
var StockRequestArray = [];
var gStockRequestTransData = [];
var TabId = 1;
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


$(function () {
    pLoadingSetup(false);
    $("#divPendingStockRequest").hide();
    $("#divSearchPage").show();
    $("#btnAddNew").show();
    $("#btnSaveProduction").show();
    //calendar
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
    GetStockRequest(1);
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
        } else if (target === "#Completed") {
            TabId = 3;
            //GetStockRequest(3);
        }
        GetStockRequest();
    });

});
function GetStockRequest() {
    $.ajax({
        url: GetStockRequestUrl,
        type: 'GET',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: { ID: TabId },
        success: function (response) {
            if (TabId == 1) {
                $("#btnAddNew").show();
                $("#btnCompleted").show();
                DisplayPendingforapprovalData(response.data, "divApprovedList", "tabApproved", "tblSRApproved");
            }
            else if (TabId == 2) {
                $("#btnAddNew").show();
                $("#btnCompleted").show();
                DisplayPendingforapprovalData(response.data, "divInProgressList", "tabInProgress", "tblSRInProgress");
            }
            else if (TabId == 3) {
                $("#btnAddNew").hide();
                $("#btnCompleted").hide();
                DisplayPendingforapprovalData(response.data, "divCompletedList", "tabCompleted", "tblSRCompleted");
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
$("#btnClearFilter").on('click', function () {
    $('#txtStartDate').val("");
    $('#txtEndDate').val("");
    $('#txtSearchbox').val("");
    $("#ddlProcessType").val("0").change();
    FilterPurchaseEntry();
    return false;
});
function DisplayPendingforapprovalData(dataList, divName, tabTitleId, tableId) {
    // Destroy old DataTable if it exists
    if ($.fn.DataTable.isDataTable('#' + tableId)) {
        $('#' + tableId).DataTable().destroy();
    }

    // Build table
    $("#" + divName).empty();
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
                    <th>Status</th>
                    <th></th>
                </tr>
            </thead>
            <tbody>`;

    if (dataList.length !== 0) {
        dataList.forEach((entry, index) => {
            tableContent += `
                <tr data-sno="${entry.sNo}">
                    <td>${index + 1}</td>
                    <td>${entry.RequestNo}</td>
                    <td>${entry.sRequestDate}</td>
                    <td>${entry.ProcessTypeName}</td>
                    <td>${entry.RequestedByName}</td>
                    <td>${entry.ApprovedByName}</td>
                    <td>${entry.sApprovedDate}</td>
                    <td><span class="badge ${entry.ColorCode}">${entry.StockRequestStatus}</span></td>
                    <td>
                        <button type="button" onclick="EditData(${entry.StockRequestID})" class="btn btn-sm btn-outline-warning">
                            <i class="bx bx-package me-2"></i>Hydraulic Pressure
                        </button>
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
        "bAutoWidth": false,
        "bPaginate": false,
        "bFilter": true,
        "bSort": false,
        "order": [],
        "pagingType": "full_numbers"
    });
}
function EditData(id, tabId) {
    if (ENABLE_VERBOSE_Logging) //console.log(id);
    stockRequestId = 0;
    BatchConsumedData = [];
    gStockRequestTransData = [];

    $.ajax({
        url: GetStockRequestDetailsByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            if (!response || !response.data) {
                console.error("Invalid response format");
                return;
            }
            var stockRequest = response.data.StockRequest[0];
            var stockTransList = response.data.StockRequestTrans;
            var hydraulicConsumption = response.data.HydrolicConsumption || [];

            stockRequest.VStockRequestTrans = stockTransList;

            stockRequest.HydrolicConsumption = hydraulicConsumption;

            response.data.HydrolicPressure.forEach((entry) => {
                BatchConsumedData.push(entry);
            });

            $("#hdnStockRequestID").val(id);
            $("#divPendingStockRequest").show();
            $("#divSearchPage").hide();

            $("#ddlComponentType").val(stockRequest.OutputComponentTypeID).change();

            if (stockTransList && stockTransList.length > 0) {
                $("#ddlSize").val(stockTransList[0].SizeID).change();

                if (stockTransList[0].ColorName != "NONE")
                    $("#ddlColor").val(stockTransList[0].ColorID).change();
                else
                    $("#ddlColor").val(stockTransList[1].ColorID).change();
            }

            $("#ddlComponentType").prop("disabled", true);
            $("#ddlSize").prop("disabled", true);
            $("#ddlColor").prop("disabled", true);
            GetItemDropdownByFilter();
            //if (stockRequest.ProcessTypeID > 1) {
            //    $("#ddlColor").prop("disabled", true);
            //}

            // 🔹 Clear input fields
            $('#txtExpectedProductQty').val("");
            $('#txtActualProductQty').val("");
            $('#ddlEmployeee').val("0").change();
            $("#ddlRackLocation").val("0").change();

            GetRackLocationDropdownByFilter(parseInt($('#ddlComponentType').val()));
            if (stockRequest.VStockRequestTrans && stockRequest.VStockRequestTrans.length !== 0) {
                stockRequest.VStockRequestTrans.forEach((entry, index) => {
                    var StockRequestData = new Object();
                    StockRequestData.StockRequestTransID = entry.StockRequestTransID;
                    StockRequestData.ComponentTypeID = entry.ComponentTypeID; 
                    StockRequestData.ComponentTypeName = entry.ComponentTypeName;
                    StockRequestData.ItemName = entry.ItemName;
                    StockRequestData.SizeName = entry.SizeName;
                    StockRequestData.ColorName = entry.ColorName;
                    StockRequestData.UnitName = entry.UnitName;
                    StockRequestData.BatchNo = entry.BatchNo;
                    StockRequestData.ProbableProductionQty = parseFloat(entry.ProbableProductionQuantity);
                    StockRequestData.ApprovedQty = parseFloat(entry.Quantity);
                    StockRequestData.ActualConsumedQty = 0;
                    StockRequestData.ProductionPlates = 0;
                    StockRequestData.WastageQty = 0;
                    StockRequestData.WastagePercentage = 0;
                    StockRequestData.BalanceQty = StockRequestData.ApprovedQty;
                    StockRequestData.BalanceProbableQty = StockRequestData.ProbableProductionQty;
                    StockRequestData.PerPlate = entry.PerPlate;
                    StockRequestData.ProductionQuantity = entry.ProductionQuantity;
                    StockRequestData.Quantity = entry.Quantity;
                   
                    gStockRequestTransData.push(StockRequestData);
                });
            }
            RenderPendingstrockRequestDetails(stockRequest);
            DisplayHydrolicPressureTransData();
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
}
function RenderPendingstrockRequestDetails(data) {
    StockRequestArray = data;
    $("#divPendingStockRequestTrans").empty();

    var headerDetails = `
        <div class="row g-2 task-dates">
            <div class="col-lg-2 col-md-4 col-sm-6 col-12">
                <p class="text-muted mb-2">Request No.</p>
                <h5 class="font-size-14"><i class="bx bx-copy-alt me-1 text-primary"></i>${data.RequestNo}</h5>
            </div>
            <div class="col-lg-2 col-md-4 col-sm-6 col-12">
                <p class="text-muted mb-2">Request Date</p>
                <h5 class="font-size-14"><i class="bx bx-calendar-check me-1 text-primary"></i>${data.sRequestDate}</h5>
            </div>
            <div class="col-lg-2 col-md-4 col-sm-6 col-12">
                <p class="text-muted mb-2">Process Type</p>
                <h5 class="font-size-14"><i class="bx bx-chevrons-right me-1 text-primary"></i>${data.ProcessTypeName}</h5>
            </div>
            <div class="col-lg-2 col-md-4 col-sm-6 col-12">
                <p class="text-muted mb-2">Request By</p>
                <h5 class="font-size-14"><i class="bx bx-user me-1 text-primary"></i>${data.RequestedByName}</h5>
            </div>
            <div class="col-lg-2 col-md-4 col-sm-6 col-12">
                <p class="text-muted mb-2">Approved By</p>
                <h5 class="font-size-14"><i class="bx bx-user me-1 text-primary"></i>${data.ApprovedByName}</h5>
            </div>
             <div class="col-lg-2 col-md-4 col-sm-6 col-12">
                <p class="text-muted mb-2">Approved Date</p>
                <h5 class="font-size-14"><i class="bx bx-calendar me-1 text-primary"></i>${ISTtoLocalTime(data.ApprovedDate)}</h5>
            </div>
        </div>
    `;
    let colorCode = "bg-secondary bg-gradient text-white";
    let tableContent = '<div class="table-responsive mt-4">';

    tableContent += `
        <table class="table table-striped align-middle" id="tblSearchResult">
            <thead>
                <tr class="table-light">
                    <th class="${colorCode}">Component</th>
                    <th class="${colorCode}">Item</th>
                    <th class="${colorCode}">Size</th>
                    <th class="${colorCode}">Colour</th>
                    <th class="${colorCode}">Batch No</th>
                    <th class='${colorCode} text-end'>Probable No of Production Quantity</th>
                    <th class='${colorCode} text-end'>Approved Qty</th>        
                    <th class='${colorCode} text-end'>Actual</br>Consumed Qty</th>
                    <th class='${colorCode} text-end'>Production Plates</th>
                    <th class='${colorCode} text-end'>Wastage Qty</th>
                    <th class='${colorCode} text-end'>Wastage %</th>
                    <th class='${colorCode} text-end'>Balance Qty</th>
                    <th class='${colorCode} text-end'>Balance Propable Quantity</th>
                </tr>
            </thead>
            <tbody>`;

    gStockRequestTransData.forEach((entry, index) => {
        tableContent += `
                <tr>                  
                    <td>${entry.ComponentTypeName || ''}</td>
                    <td>${entry.ItemName || ''}</td>
                    <td>${entry.SizeName || ''}</td>
                    <td>${entry.ColorName || ''}</td>
                    <td>${entry.BatchNo || ''}</td>
                   <td class='text-end'>${Math.round(entry.ProbableProductionQty || 0)} Pcs</td>
                     <th class='text-end'>${entry.ApprovedQty.toFixed(4)} ${entry.UnitName}</th>  
                     <td>
                        <div class="input-group">
                          <input
                            id="txtActualConsumed_${entry.StockRequestTransID}"  
                            type="text" 
                            class="form-control text-end"
                            readonly
                            placeholder="0.00 ${entry.UnitName || 'Kg'}" 
                            disabled
                            data-approvedqty="${entry.ApprovedQty.toFixed(4)}"
                            value="0.00 ${entry.UnitName || 'KG'}">
                        </div>
                      </td>
                      <td>
                        <div class="input-group">
                          <input
                            id="txtConsumedPcs_${entry.StockRequestTransID}" 
                            type="text" 
                            disabled 
                            class="form-control text-end" 
                            readonly 
                            placeholder="0.00 Pcs"
                             value="0.00 Pcs">
                        </div>
                      </td>
                      <td>
                        <div class="input-group">
                          <input 
                            id="txtWastageQuantity_${entry.StockRequestTransID}" 
                            type="text"  
                            disabled 
                            class="form-control text-end" 
                            readonly 
                            placeholder="0.00 ${entry.UnitName || 'Kg'}"
                            value="0.00 ${entry.UnitName || 'KG'}">
                        </div>
                      </td>

                      <td>
                        <div class="input-group">
                          <input 
                            id="txtWastagePer_${entry.StockRequestTransID}" 
                            type="text" 
                            disabled 
                            class="form-control text-end" 
                            readonly 
                            placeholder="0.00"
                            value="0.00 %">
                        </div>
                      </td>

                      <td>
                        <div class="input-group">
                          <input 
                            id="txtBalanceQty_${entry.StockRequestTransID}" 
                            type="text" 
                            disabled 
                            class="form-control text-end" 
                            readonly 
                            placeholder="0.00 ${entry.UnitName || 'Kg'}"
                            value="0.00 ${entry.UnitName || 'KG'}">
                        </div>
                      </td>

                      <td>
                        <div class="input-group">
                          <input
                            id="txtBalancePcs_${entry.StockRequestTransID}" 
                            type="text" 
                            disabled 
                            class="form-control text-end" 
                            readonly 
                            placeholder="${entry.ProbableProductionQty} ${'Pcs'}"
                            value="${entry.ProbableProductionQty} ${'Pcs'}">
                        </div>
                      </td>
                </tr>`;
    });

    tableContent += `
                </tbody>
            </table>
        </div>`;

    $("#divPendingStockRequestTrans").append(headerDetails + tableContent);
    ////=================================================================================Table for the Remaining Probable Prodcution


    $(".decimal").inputmask("decimal", { digits: 2, radixPoint: "." });
}

//========================================================Modal Popup
$("#btnAddNew").on('click', function () {
    $('#btnSaveProduction').show();
    $('#btnUpdateProduction').hide();
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Production");
    BalanceQtyTable();
    ClearModuleFormFields();
    $('#divRecordLog').hide();
    return false;
});
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

    if (StockRequestArray.VStockRequestTrans && StockRequestArray.VStockRequestTrans.length !== 0) {
        let minBalance = Math.min(
            ...StockRequestArray.VStockRequestTrans.map(entry => {
                let val = $(`#txtBalancePcs_${entry.StockRequestTransID}`).val() || "0";
                return parseFloat(val.split(" ")[0]) || 0;
            })
        );

        StockRequestArray.VStockRequestTrans.forEach((entry) => {
            let balanceVal = $(`#txtBalancePcs_${entry.StockRequestTransID}`).val() || "0";
            let balanceNum = parseFloat(balanceVal.split(" ")[0]) || 0;
            let dangerClass = balanceNum === minBalance ? "text-danger fw-bold" : "";

            ProbableProduction += `
            <tr>
                <td>${entry.ComponentTypeName || ''}</td>
                <td class="text-end ${dangerClass}">${balanceVal}</td>
            </tr>`;
        });
    }


    ProbableProduction += `
            </tbody>
        </table>`;

    $("#divProbableProduction").append(ProbableProduction);
    return false;
}
function ClearModuleFormFields()     {
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $('#ddlEmployeee').val(0);
  //  $('#txtProductionDate').val('');
    $('#ddlRackLocation').val(0);
    $('#txtProductionQty').val('');
    $('#txtWastageQty').val('');
    $('#txtOtherWastageQty').val('');
    $('#txtstartTime').val('');
    $('#txtendTime').val('');
    $('#txtTotalTime').val('');

    $("#btnSaveItem").show();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();
    $("#txtProductionDate").val(moment().format('DD/MM/YYYY'));

    return false;
}
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

$("#btnSaveProduction,#btnUpdateProduction").on("click", function () {
    // Clear previous validation errors
    $(".invalid-feedback").remove();
    $(".form-control").removeClass("is-invalid");
    ConsumedData = [];
    var HydrolicPressureID = 0;

    if (this.id == "btnUpdateProduction" && $("#hdnHydrolicPressureID").val() > 0)
        HydrolicPressureID = $("#hdnHydrolicPressureID").val();

    // Basic validation
    if ($('#ddlEmployeee').val() == 0) return markInvalid("#ddlEmployeee", "Please select Operator");
    if (!$('#txtProductionDate').val()) return markInvalid("#txtProductionDate", "Please select Production Date");
    if ($('#ddlItem').val() == 0) return markInvalid("#ddlItem", "Please select Item");
    if ($('#ddlRackLocation').val() == 0) return markInvalid("#ddlRackLocation", "Please select Rack Location");
    if (!$('#txtProductionQty').val()) return markInvalid("#txtProductionQty", "Please enter Production Quantity");
    if (!$('#txtstartTime').val()) return markInvalid("#txtstartTime", "Please enter Start Time");
    if (!$('#txtendTime').val()) return markInvalid("#txtendTime", "Please enter End Time");

    const startTimeStr = $('#txtstartTime').val();
    const endTimeStr = $('#txtendTime').val();
    const ProductionDate = $('#txtProductionDate').val();
    const totalMinutes = getTimeDifferenceInMinutes(startTimeStr, endTimeStr);

    if (totalMinutes <= 0 || isNaN(totalMinutes)) {
        return markInvalid("#txtTotalTime", "Invalid Start/End Time");
    }

    const newStartMin = parseTimeToMinutes(startTimeStr);
    const newEndMin = parseTimeToMinutes(endTimeStr);

    const hasConflict = BatchConsumedData.some(item => {
        if (this.id === "btnUpdateProduction" && item.HydrolicPressureID == HydrolicPressureID) {
            return false;
        }

        if (item.sProductionDate !== ProductionDate) return false;

        const existingStartMin = parseTimeToMinutes(item.sStartTime);
        const existingEndMin = parseTimeToMinutes(item.sEndTime);

        if (item.sStartTime === startTimeStr || item.sEndTime === endTimeStr) {
            return true;
        }

        return newStartMin < existingEndMin && newEndMin > existingStartMin;
    });

    if (hasConflict) {
        return markInvalid("#txtstartTime", "Start/End time overlaps or already exists in previous records.");
    }

    $('#txtTotalTime').val(totalMinutes + " minutes");

    //Validate Least Production Qty to make sure doesn't exceed
    //let minProbableQty = parseFloat(Math.min(...gStockRequestTransData.map(e => e.ProbableProductionQty)));

    let minItem = gStockRequestTransData.reduce((min, current) => {
        return current.BalanceProbableQty < min.BalanceProbableQty ? current : min;
    }, gStockRequestTransData[0]);

    let minProbableQty = parseFloat(minItem.ProbableProductionQty);
    let minstockRequestTransID = minItem.StockRequestTransID; // or whatever the property name is

    let InputProductionQty = parseFloat($('#txtProductionQty').val()) || 0;
    let InputWastageQty = parseFloat($('#txtWastageQty').val()) || 0;
    let OtherWastageQty = parseFloat($('#txtOtherWastageQty').val()) || 0;
    var InputOtherWastageQty = 0;

    let totalProductionQty = 0, totalWastageQty = 0; totalOtherWastageQty = 0;

    $.each(BatchConsumedData, function (index, item) {
        if (item.HydrolicPressureID != $("#hdnHydrolicPressureID").val()) {
            totalProductionQty += item.ProductionQty || 0;
            totalWastageQty += item.WastageQty || 0;
            totalOtherWastageQty += item.OtherWastageQty || 0;
        }
    });

    const maxSNo = getMaxSNo();
    var result = {
        sNo: maxSNo + 1,
        HydrolicPressureID: HydrolicPressureID,
        OperatorID: parseInt($('#ddlEmployeee').val()),
        OperatorName: $('#ddlEmployeee option:selected').text(),
        sProductionDate: $('#txtProductionDate').val(),
        ItemID: parseInt($('#ddlItem').val()),
        ItemName: $('#ddlItem option:selected').text(),
        RackLocationID: parseInt($('#ddlRackLocation').val()),
        RackLocationName: $('#ddlRackLocation option:selected').text(),
        WareHouseName: $('#ddlRackLocation option:selected').attr("WareHouseName") || '',
        ProductionQty: parseInt($('#txtProductionQty').val()) || 0,
        WastageQty: parseInt($('#txtWastageQty').val()) || 0,
        OtherWastageQty: parseFloat($('#txtOtherWastageQty').val()) || 0,
        sStartTime: startTimeStr,
        sEndTime: endTimeStr,
        TotalTime: totalMinutes + " minutes",
        StockRequestID: $('#hdnStockRequestID').val(),
        HydrolicConsumption: null
    };

    let isValid = true;

    //gStockRequestTransData.forEach((arraydata) => {
    //    var stockRequestTransID = arraydata.StockRequestTransID;
    //    var PerPlate = arraydata.PerPlate;
    //    var approvedQty = arraydata.ApprovedQty || 0;
    //    var actualconsumed = parseFloat($("#txtActualConsumed_" + stockRequestTransID).val()) || 0;
    //    var Wastage = parseFloat($("#txtWastageQuantity_" + stockRequestTransID).val()) || 0;
    //    var ProductionQuantity = parseFloat(arraydata.ProductionQuantity) || 0;
    //    var UnitName = arraydata.UnitName;
    //    var consumedQty = 0, wastageQty = 0, totalqty = 0, wastagePercentage = 0, balanceQty = 0;

    //    if (this.id == "btnSaveProduction") {
    //        // consumedQty = PerPlate * result.ProductionQty + actualconsumed;
    //        consumedQty = PerPlate * result.ProductionQty;
    //        //wastageQty = PerPlate * result.WastageQty + result.OtherWastageQty; => The Other wastage only calculate for the Aluminium coil alone not for all the componenet as per the discussion on 08/09/2025

    //        // Only include OtherWastageQty if ComponentTypeID is ALUMINIUMCOIL
    //        if (arraydata.ComponentTypeID == AluminiumCoil) {
    //            wastageQty = PerPlate * result.WastageQty + result.OtherWastageQty;
    //        } else {
    //            wastageQty = PerPlate * result.WastageQty;
    //        }
    //        totalqty = consumedQty + wastageQty + actualconsumed;
    //        wastagePercentage = (wastageQty > 0 && approvedQty > 0) ? (wastageQty / approvedQty) * 100 : 0;
    //        balanceQty = (approvedQty > 0) ? (approvedQty - totalqty) : 0;
    //        if (minstockRequestTransID == stockRequestTransID) {
    //            InputOtherWastageQty = OtherWastageQty / PerPlate;
    //            if (totalOtherWastageQty > 0) Math.round( totalOtherWastageQty= totalOtherWastageQty / PerPlate);
    //        }
    //    }
    //    else if (this.id == "btnUpdateProduction") {
    //        var editabledata = BatchConsumedData.find(item => item.HydrolicPressureID == HydrolicPressureID);

    //        var oldProductionQty = parseFloat(editabledata.ProductionQty) || 0;
    //        var oldWastageQty = parseFloat(editabledata.WastageQty) || 0;
    //        var oldOtherWastageQty = parseFloat(editabledata.OtherWastageQty) || 0;

    //        var newProductionQty = parseFloat($('#txtProductionQty').val()) || 0;
    //        var newWastageQty = parseFloat($('#txtWastageQty').val()) || 0;
    //        var newOtherWastageQty = parseFloat($('#txtOtherWastageQty').val()) || 0;

    //        var diffProduction = newProductionQty - oldProductionQty;
    //        var diffWastage = newWastageQty - oldWastageQty;
    //        var diffOtherWastage = newOtherWastageQty - oldOtherWastageQty;

    //        consumedQty = actualconsumed + (diffProduction * PerPlate);
    //        if (arraydata.ComponentTypeID == AluminiumCoil) {
    //            wastageQty = Wastage + (diffWastage * PerPlate) + diffOtherWastage;
    //        }
    //        else {
    //            wastageQty = Wastage + (diffWastage * PerPlate);
    //        }
    //        totalqty = consumedQty + wastageQty;

    //        wastagePercentage = (wastageQty > 0 && approvedQty > 0) ? (wastageQty / approvedQty) * 100 : 0;
    //        balanceQty = (approvedQty > 0) ? (approvedQty - totalqty) : 0;
    //        if (minstockRequestTransID == stockRequestTransID) {
    //            InputOtherWastageQty = OtherWastageQty / PerPlate;
    //            if (totalOtherWastageQty > 0) Math.round(totalOtherWastageQty = totalOtherWastageQty / PerPlate);
    //        }
    //    }

    //    //$("#txtWastagePer_" + stockRequestTransID).val(wastagePercentage.toFixed(4) + " %");
    //    //$("#txtBalanceQty_" + stockRequestTransID).val(balanceQty.toFixed(4) + " " + UnitName);
    //    //$("#txtActualConsumed_" + stockRequestTransID).val(consumedQty.toFixed(4) + " " + UnitName);
    //    //$("#txtWastageQuantity_" + stockRequestTransID).val(wastageQty.toFixed(4) + " " + UnitName);
    //    //$("#txtBalancePcs_" + stockRequestTransID).val(Math.round(balanceQty / PerPlate) + " Pcs");
    //    //$("#txtConsumedPcs_" + stockRequestTransID).val(Math.round(consumedQty / PerPlate) + " Pcs");

    //    var dataObj = {
    //        ActualConsumedQty: consumedQty,
    //        WastageQty: wastageQty,
    //        WastagePercentage: wastagePercentage,
    //        BalanceQty: balanceQty,
    //        StockRequestTransID: stockRequestTransID
    //    };

    //    if (arraydata.ComponentTypeID == AluminiumCoil) {
    //        if ((InputProductionQty + InputWastageQty + InputOtherWastageQty) > minProbableQty - (totalProductionQty + totalWastageQty + totalOtherWastageQty)) {
    //            return markInvalid("#txtProductionQty", "Production Quantity exceeded the Aluminium Coil limit! Max Allowed: " + Math.round((minProbableQty - (totalProductionQty + totalWastageQty + totalOtherWastageQty))));
    //        }
    //        return false;
    //    }
    //    else {
    //        if ((InputProductionQty + InputWastageQty ) > minProbableQty - (totalProductionQty + totalWastageQty )) {
    //            return markInvalid("#txtProductionQty", "Production Quantity exceeded the RRS limit! Max Allowed" + Math.round((minProbableQty - (totalProductionQty + totalWastageQty))));
    //        }
    //        return false;
    //    }

    //    ConsumedData.push(dataObj);
    //});

    //if ((InputProductionQty + InputWastageQty  + InputOtherWastageQty ) > minProbableQty - (totalProductionQty + totalWastageQty + totalOtherWastageQty)) {
    //    return markInvalid("#txtProductionQty", "Production Quantity exceeded the limit!. Minmium Probable Qty: " + Math.round((minProbableQty - (totalProductionQty + totalWastageQty + totalOtherWastageQty))));
    //}

    for (let i = 0; i < gStockRequestTransData.length; i++) {
        const arraydata = gStockRequestTransData[i];
        var stockRequestTransID = arraydata.StockRequestTransID;
        var PerPlate = arraydata.PerPlate;
        var approvedQty = arraydata.ApprovedQty || 0;
        var actualconsumed = parseFloat($("#txtActualConsumed_" + stockRequestTransID).val()) || 0;
        var Wastage = parseFloat($("#txtWastageQuantity_" + stockRequestTransID).val()) || 0;
        var ProductionQuantity = parseFloat(arraydata.ProductionQuantity) || 0;
        var UnitName = arraydata.UnitName;
        var consumedQty = 0, wastageQty = 0, totalqty = 0, wastagePercentage = 0, balanceQty = 0;

        if (this.id == "btnSaveProduction") {
            consumedQty = PerPlate * result.ProductionQty;

            if (arraydata.ComponentTypeID === AluminiumCoil) {
                wastageQty = PerPlate * result.WastageQty + result.OtherWastageQty;
            } else {
                wastageQty = PerPlate * result.WastageQty;
            }

            totalqty = consumedQty + wastageQty + actualconsumed;
            wastagePercentage = (wastageQty > 0 && approvedQty > 0) ? (wastageQty / approvedQty) * 100 : 0;
            balanceQty = (approvedQty > 0) ? (approvedQty - totalqty) : 0;

            if (minstockRequestTransID == stockRequestTransID) {
                InputOtherWastageQty = OtherWastageQty / PerPlate;
                if (totalOtherWastageQty > 0) {
                    totalOtherWastageQty = Math.round(totalOtherWastageQty / PerPlate);
                }
            }
        } else if (this.id == "btnUpdateProduction") {
            var editabledata = BatchConsumedData.find(item => item.HydrolicPressureID == HydrolicPressureID);

            var oldProductionQty = parseFloat(editabledata.ProductionQty) || 0;
            var oldWastageQty = parseFloat(editabledata.WastageQty) || 0;
            var oldOtherWastageQty = parseFloat(editabledata.OtherWastageQty) || 0;

            var newProductionQty = parseFloat($('#txtProductionQty').val()) || 0;
            var newWastageQty = parseFloat($('#txtWastageQty').val()) || 0;
            var newOtherWastageQty = parseFloat($('#txtOtherWastageQty').val()) || 0;

            var diffProduction = newProductionQty - oldProductionQty;
            var diffWastage = newWastageQty - oldWastageQty;
            var diffOtherWastage = newOtherWastageQty - oldOtherWastageQty;

            consumedQty = actualconsumed + (diffProduction * PerPlate);

            if (arraydata.ComponentTypeID === AluminiumCoil) {
                wastageQty = Wastage + (diffWastage * PerPlate) + diffOtherWastage;
            } else {
                wastageQty = Wastage + (diffWastage * PerPlate);
            }

            totalqty = consumedQty + wastageQty;
            wastagePercentage = (wastageQty > 0 && approvedQty > 0) ? (wastageQty / approvedQty) * 100 : 0;
            balanceQty = (approvedQty > 0) ? (approvedQty - totalqty) : 0;

            if (minstockRequestTransID == stockRequestTransID) {
                InputOtherWastageQty = OtherWastageQty / PerPlate;
                if (totalOtherWastageQty > 0) {
                    totalOtherWastageQty = Math.round(totalOtherWastageQty / PerPlate);
                }
            }
        }

        var dataObj = {
            ActualConsumedQty: consumedQty,
            WastageQty: wastageQty,
            WastagePercentage: wastagePercentage,
            BalanceQty: balanceQty,
            StockRequestTransID: stockRequestTransID
        };

        // ❗ Validation and breaking the loop if exceeded
        if (arraydata.ComponentTypeID === AluminiumCoil) {
            if ((InputProductionQty + InputWastageQty + result.OtherWastageQty / PerPlate) > (arraydata.ProbableProductionQty - (totalProductionQty + totalWastageQty + totalOtherWastageQty))) {
                markInvalid("#txtProductionQty", "Production Quantity exceeded the Aluminium Coil limit! Max Allowed: " + Math.round(arraydata.ProbableProductionQty - (totalProductionQty + totalWastageQty + totalOtherWastageQty)));                
                ConsumedData = [];
                break;
                return false;
            }
        } else {
            if ((InputProductionQty + InputWastageQty) > (arraydata.ProbableProductionQty - (totalProductionQty + totalWastageQty))) {
                markInvalid("#txtProductionQty", "Production Quantity exceeded the RRS limit! Max Allowed: " + Math.round(arraydata.ProbableProductionQty - (totalProductionQty + totalWastageQty)));
                ConsumedData = [];
                break; 
                return false;
            }
        }

        ConsumedData.push(dataObj);
    }

    if (ConsumedData.length > 0) {
        result.HydrolicConsumption = ConsumedData;
        BatchConsumedData.push(result);
       UpdateHydrolicPressure(result);
    }
});
function UpdateHydrolicPressure(OutputData) {
    if (ENABLE_VERBOSE_Logging) //console.log(OutputData);

    $.ajax({
        url: UpdateHydrolicPressureUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(OutputData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.success && !response.isExists) {
                if (OutputData.HydrolicPressureID == 0)
                    Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                else
                    Swal.fire({ title: "Update!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                $('#btnCloseProduction').click();
                EditData($('#hdnStockRequestID').val());
            }
            else if (!response.success && response.isExists) {
                Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
            }          
            else
                Swal.fire({ title: "Production Warning", text: response.message, icon: "warning", confirmButtonColor: "#556ee6" });
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

    return false;
}
function DisplayHydrolicPressureTransData() {
    $("#divHydrolicPressureTrans").empty();
    let tableContent = `<div class="alert alert-info mt-1" role="alert">
        <i class="bx bx-archive-in me-3"></i>Production Output
    </div>`;
    tableContent += '<div class="table-responsive">';
    tableContent += `
        <table class="table  align-middle" id="tblSRApproved">
            <thead>
                <tr class="table-light">
                    <th>S No.</th>
                    <th>Batch No.</th>
                    <th>Date</th>
                    <th>Start Time</th>
                    <th>End Time</th>
                    <th>Total Time</th>
                    <th>Operator</th>
                    <th>Item</th>
                    <th>WareHouse</th>
                    <th>Rack Location</th>
                    <th>Production Qty</th>
                    <th>Wastage Qty</th>
                    <th>Aluminium Cutting Wastage Qty</th>
                     <th>Status</th>`;
    if (TabId < 3) {
        tableContent += `<th>Action</th>`;
    }
    tableContent += `</tr>
            </thead>
            <tbody>`;

    // Declare summary totals
    let totalProductionQty = 0;
    let totalWastageQty = 0;
    let totalOtherWastageQty = 0;

    if (BatchConsumedData.length != 0) {
        BatchConsumedData.forEach((entry, index) => {
            tableContent += `
                <tr data-sno="${entry.sNo}">
                    <td>${index + 1}</td>
                     <td>${entry.BatchNo}</td>
                    <td>${entry.sProductionDate}</td>
                    <td>${entry.sStartTime}</td>
                    <td>${entry.sEndTime}</td>
                    <td>${entry.TotalTime}</td>
                    <td>${entry.OperatorName}</td>
                    <td>${entry.ItemName}</td>
                    <td>${entry.WareHouseName}</td>
                    <td>${entry.RackLocationName}</td>
                    <td>${entry.ProductionQty}</td>
                    <td>${entry.WastageQty}</td>
                    <td>${entry.OtherWastageQty}</td>
                    <td> <span class="badge ${entry.ColorCode}">${entry.Status}</span></td>`;
            if (TabId < 3) {
                tableContent += `
                    <td class='text-center'>
                        <a href="javascript:void(0);" onclick="EditProduction(${entry.HydrolicPressureID})"  class="btn btn-sm btn-soft-info" data-bs-toggle="modal" data-bs-target="#divAddEditModal">
                            <i class="mdi mdi-pencil-outline"></i>
                        </a>
                        <a href="javascript:void(0);" onclick="DeleteProduction('${entry.HydrolicPressureID}')" class="btn btn-sm btn-soft-danger" title="Click here to Delete Stock Request Item">
                            <i class="mdi-delete-outline align-middle"></i>
                        </a>
                    </td>`;
            }
            tableContent += `</tr>`;

            // Update totals
            totalProductionQty += parseFloat(entry.ProductionQty) || 0;
            totalWastageQty += parseFloat(entry.WastageQty) || 0;
            totalOtherWastageQty += parseFloat(entry.OtherWastageQty) || 0;

            // Update calculated values
            gStockRequestTransData.forEach((arraydata) => {
                var stockRequestTransID = arraydata.StockRequestTransID;
                var PerPlate = arraydata.PerPlate;
                var approvedQty = arraydata.ApprovedQty || 0;
                var actualconsumed = parseFloat($("#txtActualConsumed_" + stockRequestTransID).val() || 0);
                var Wastage = parseFloat($("#txtWastageQuantity_" + stockRequestTransID).val() || 0);
                if (arraydata.ComponentTypeID == AluminiumCoil) {
                    var totalwastage = Wastage + entry.OtherWastageQty;
                }
                else {
                    var totalwastage = Wastage;
                }
                var ProductionQuantity = parseFloat(arraydata.ProductionQuantity) || 0;
                var UnitName = arraydata.UnitName;

                var consumedQty = PerPlate * entry.ProductionQty + actualconsumed;
                var wastageQty = PerPlate * entry.WastageQty + totalwastage;
                var totalqty = consumedQty + wastageQty;

                var wastagePercentage = (wastageQty > 0 && approvedQty > 0) ? (wastageQty / approvedQty) * 100 : 0;
                var balanceQty = (approvedQty > 0)
                    ? (approvedQty - totalqty)
                    : 0;

                $("#txtWastagePer_" + stockRequestTransID).val(wastagePercentage.toFixed(4) + " %");
                $("#txtBalanceQty_" + stockRequestTransID).val(balanceQty.toFixed(4) + " " + UnitName);
                $("#txtActualConsumed_" + stockRequestTransID).val(consumedQty.toFixed(4) + " " + UnitName);
                $("#txtWastageQuantity_" + stockRequestTransID).val(wastageQty.toFixed(4) + " " + UnitName);
                //$("#txtBalancePcs_" + stockRequestTransID).val((balanceQty * ProductionQuantity).toFixed(4) + " Pcs");
                //$("#txtConsumedPcs_" + stockRequestTransID).val((consumedQty * ProductionQuantity).toFixed(4) + " Pcs");
                $("#txtBalancePcs_" + stockRequestTransID).val(Math.round(balanceQty / PerPlate) + " Pcs");
                $("#txtConsumedPcs_" + stockRequestTransID).val(Math.round(consumedQty / PerPlate) + " Pcs");

            });
        });

        let sfootercolorcode = "bg-info bg-gradient";
        // Add summary row
        tableContent += `
            <tr class="table-dark fw-bold">
                <td colspan="10" class="${sfootercolorcode} text-end">Total</td>
                <td class="${sfootercolorcode}">${totalProductionQty} Pcs</td>
                <td class="${sfootercolorcode}">${totalWastageQty} Pcs</td>
                <td class="${sfootercolorcode}">${totalOtherWastageQty.toFixed(4)} Kg</td>
                <td class="${sfootercolorcode}"></td>`;

        if (TabId < 3) {
            tableContent += `<td class="${sfootercolorcode}"></td>`;
            tableContent += `<td class="${sfootercolorcode}"></td>`;
        }
        tableContent += `</tr>`;
    } else {
        tableContent += `<tr><td colspan="1" class="text-center">No Records To Display</td></tr>`;
    }

    tableContent += `
            </tbody>
        </table>
    </div>`;

    $("#divHydrolicPressureTrans").html(tableContent);
    $("#btnCloseProduction").click();
}
function EditProduction(id) {

    ClearModuleFormFields();
    $('#btnSaveProduction').hide();
    $('#btnUpdateProduction').show();

    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Edit Production Entry");

    var editabledata = BatchConsumedData.find(item => item.HydrolicPressureID == id);

    $("#hdnHydrolicPressureID").val(editabledata.HydrolicPressureID);
    $('#ddlEmployeee').val(editabledata.OperatorID).change();
    $('#txtProductionDate').val(editabledata.sProductionDate);
    $('#ddlItem').val(editabledata.ItemID).change();
    $('#ddlRackLocation').val(editabledata.RackLocationID).change();
    $('#txtProductionQty').val(editabledata.ProductionQty)
    $('#txtWastageQty').val(editabledata.WastageQty)
    $('#txtOtherWastageQty').val(editabledata.OtherWastageQty)
    $('#txtstartTime').val(editabledata.sStartTime)
    $('#txtendTime').val(editabledata.sEndTime)
    $('#txtTotalTime').val(editabledata.TotalTime)

    BalanceQtyTable();

   // $("#divRecordLog").show();
    $("#spnLastUpdatedBy").html("Last Updated By: " + editabledata.LastUpdatedByName);
    $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(editabledata.LastUpdatedDate));
    return false;

}

$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divSearchPage").show();
    $("#divPendingStockRequest").hide();

    GetStockRequest(1);
    return false;
});
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
                EditData($('#hdnStockRequestID').val());
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

$("#btnCompleted").on('click', function () {
    var id = $('#hdnStockRequestID').val();
    if (BatchConsumedData.length <= 0) {
        $.jGrowl("Kindly Add Atleast One Item.", { sticky: false, theme: 'warning', life: jGrowlLife });
        return false
    }

    CompleteHydrolicPressure(id);

    return false;
});
function CompleteHydrolicPressure(id) {
    if (ENABLE_VERBOSE_Logging) //console.log(id);
    $.ajax({
        url: CompleteHydrolicPressureUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(id),
        success: function (response) {
            if (response.success && !response.isExists) {
                Swal.fire({ title: "Completed!", text: "Completed SuccessFully", icon: "success", confirmButtonColor: "#556ee6" });
                $('#btnClose').click();
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
}

$('#divAddEditModal').on('shown.bs.modal', function () {
    $('#ddlRackLocation').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlComponentType').select2({ dropdownParent: $('#divPendingStockRequest'), width: '100%' });
    $('#ddlSize').select2({ dropdownParent: $('#divPendingStockRequest'), width: '100%' });
    $('#ddlColor').select2({ dropdownParent: $('#divPendingStockRequest'), width: '100%' });
    $('#ddlItem').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlEmployeee').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
});
function GetRackLocationDropdownByFilter(ComponentTypeID) {
    $.ajax({
        url: GetRackLocationDropdownByFilterUrl,
        type: 'GET',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: { ComponentTypeID: ComponentTypeID },
        async: false,
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);

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


