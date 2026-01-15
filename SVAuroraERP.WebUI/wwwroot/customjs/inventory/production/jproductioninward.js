var ConsumedData = [];
function getValidatedFloat(selector) {
    let value = parseFloat($(selector).val());
    return isNaN(value) ? 0 : value;
}
$(function () {
    pLoadingSetup(false);
    $("#divPendingStockRequest").hide();
    $("#divSearchPage").show();
    initDatePicker(); // Run on page load

    FilterPurchaseEntry();
    pLoadingSetup(true);
});
function initDatePicker() {
    if ($('#datepicker6').length) {
        $('#datepicker6').datepicker({
            format: 'dd M, yyyy',
            autoclose: true,
            todayHighlight: true,
            immediateUpdates: true
        });
    }
}
$("#divSearchPage").on('shown.bs.collapse', function () {
    initDatePicker();
});
// Filter Button Click
$("#btnFilter").on('click', function () {
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');
    FilterPurchaseEntry();
    return false;
});
// Function to Filter Data
function FilterPurchaseEntry() {
    var FilterData = {
        ProcessTypeID: $('#ddlProcessType').val(),
        sStartDate: $('#txtStartDate').val(),
        sEndDate: $('#txtEndDate').val(),
        SearchInWord: $('#txtSearchbox').val()
    };

    GetPendingForApprovalByFilter(FilterData);
}
// AJAX Call
function GetPendingForApprovalByFilter(FilterData) {
    $.ajax({
        url: GetPendingForApprovalByFilterUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(FilterData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
            DisplayPendingforapprovalData(response.data);
            //$("#btnFilter").hide();
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
function DisplayPendingforapprovalData(Completeddata) {
    $("#divSearchResultSummary").empty();
    let tableContent = '<div class="table-responsive">';

    tableContent += `
        <table class="table table-striped align-middle" id="tblSRApproved">
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

    if (Completeddata.length != 0) {
        Completeddata.forEach((entry, index) => {

            tableContent += `
                <tr data-sno="${entry.sNo}">
                    <td>${index + 1}</td>
                    <td>${entry.RequestNo}</td>
                    <td>${entry.sRequestDate}</td>
                    <td>${entry.ProcessTypeName}</td>
                    <td>${entry.RequestedByName}</td>
                    <td>${entry.ApprovedByName}</td>
                    <td>${entry.sApprovedDate}</td>
                    <td> <span class="badge ${entry.ColorCode}">${entry.StockRequestStatus}</span></td>
                    <td>
                       <button type="button" onclick="EditData(${entry.StockRequestID})" class="btn btn-sm btn-outline-warning waves-effect waves-light" title="Click here to Add New">
						<i class="bx bx-package me-2"></i>Inward Stock
					   </button>
                    </td>`;
        });
    }
    else {
        tableContent += `<tr><td colspan="11" class="text-center">No Batch Records To Display</td></tr>`;
    }
    tableContent += `
            </tbody>
        </table>
    </div> `;

    $("#divSearchResultSummary").html(tableContent);

    $("#tblSRApproved").DataTable({
        "bAutoWidth": false,
        "bPaginate": false,
        "bFilter": true,
        "bSort": false,
        "order": [], // Disable initial sorting
        "pagingType": "full_numbers"
    });
}
function EditData(id) {
    if (ENABLE_VERBOSE_Logging) //console.log(id);
    stockRequestId = 0;
    $.ajax({
        url: GetStockRequestDetailsByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            var stockdata = response.data;
            $("#hdnStockRequestID").val(id);
            $("#divPendingStockRequest").show();
            $("#divSearchPage").hide();
            $("#ddlComponentType").val(response.data.OutputComponentTypeID).change();

            $("#ddlSize").val(response.data.VStockRequestTrans[0].SizeID).change();
            $("#ddlColor").val(response.data.VStockRequestTrans[0].ColorID).change();

            $("#ddlComponentType").prop("disabled", true);
            $("#ddlSize").prop("disabled", true);
            if (stockdata.ProcessTypeID > 1) {
                $("#ddlColor").prop("disabled", true);
            }
            $('#txtExpectedProductQty').val("");
            $('#txtActualProductQty').val("");
            $('#ddlEmployeee').val("0").change();
            $("#ddlRackLocation").val("0").change();
            GetRackLocationDropdownByFilter(parseInt($('#ddlComponentType').val()));
            RenderPendingstrockRequestDetails(stockdata);
            if (stockdata.ProcessTypeID == 2) {
                GetHologramPunchingByID(id);
            }

        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
function RenderPendingstrockRequestDetails(data) {
    
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
    //gBatchStock = batchStock;
    tableContent += `
        <table class="table table-striped align-middle" id="tblSearchResult">
            <thead>
                <tr class="table-light">
                    <th class="${colorCode}">Component</th>
                    <th class="${colorCode}">Item</th>
                    <th class="${colorCode}">Size</th>
                    <th class="${colorCode}">Colour</th>
				    <th class="${colorCode}">Batch No</th>
					<th class='${colorCode} text-end'>Approved Qty</th>					
                    <th class='${colorCode} text-end'>Actual</br>Consumed Qty</th>
                    <th class='${colorCode} text-end'>Wastage Qty</th>
                    <th class='${colorCode} text-end'>Wastage %</th>
                    <th class='${colorCode} text-end'>Balance Qty</th>
                </tr>
            </thead>
            <tbody>`;

    if (data.VStockRequestTrans.length != 0) {
        data.VStockRequestTrans.forEach((entry, index) => {
            if (entry.ProcessTypeID == 2) {
                GetHologramPunchingByID(entry.StockRequestID, entry.ComponentTypeID, entry.StockRequestTransID, entry.Quantity)
            }
            tableContent += `
                            <tr>                  
                                <td>${entry.ComponentTypeName}</td>
                                <td>${entry.ItemName}</td>
                                <td>${entry.SizeName}</td>
                                <td>${entry.ColorName}</td>
                                <td>${entry.BatchNo}</td>
                                <th class='text-end'>${entry.Quantity.toFixed(2)}</th>                    
                                <td>
                                    <input 
                                        id="txtActualConsumed_${entry.StockRequestTransID}"  
                                        StockRequestTransID='${entry.StockRequestTransID}' 
                                        type="text" 
                                        class="form-control text-end decimal" 
                                        placeholder="0.00"
                                        data-approvedqty="${entry.Quantity.toFixed(2)}">
                                </td>
                                <td>
                                    <input id="txtWastageQuantity_${entry.StockRequestTransID}" type="text" class="form-control text-end decimal" placeholder="0.00">
                                </td>
                                <td>
                                    <input id="txtWastagePer_${entry.StockRequestTransID}" type="text" class="form-control text-end decimal" readonly placeholder="0.00">
                                </td>
                                <td>
                                    <input id="txtBalanceQty_${entry.StockRequestTransID}" type="text" class="form-control text-end decimal" readonly placeholder="0.00">
                                </td>
                            </tr>`;
        });
    }
    else {
        tableContent += `<tr><td colspan="9" class="text-center">No Batch Records To Display</td></tr>`;
    }

    tableContent += `
                </tbody>
            </table>
        </div>
        <div class="table-responsive">
            <div id="divReport" class="mt-2"></div>
        </div>`;

    $("#divPendingStockRequestTrans").append(headerDetails + tableContent);
    $(".decimal").inputmask("decimal", { digits: 2, radixPoint: "." });
}
$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divSearchPage").show();
    $("#divPendingStockRequest").hide();

    FilterPurchaseEntry();
    return false;
});
$(document).on("input", "input[id^='txtWastageQuantity_']", function () {
    let batchStockID = this.id.replace("txtWastageQuantity_", ""); // Extract ID dynamically
    let wastageQty = parseFloat($(this).val()) || 0; // Get wastage quantity, default to 0 if empty
    let approvedQty = parseFloat($("#txtActualConsumed_" + batchStockID).val()) || 0; // Get approved quantity

    if (approvedQty > 0) {
        let wastagePercentage = (wastageQty / approvedQty) * 100;
        $("#txtWastagePer_" + batchStockID).val(wastagePercentage.toFixed(2)); // Set Wastage %
    } else {
        $("#txtWastagePer_" + batchStockID).val("0.00"); // Default to 0 if no approved qty
    }
});
$(document).on("input", "input[id^='txtActualConsumed_']", function () {
    let batchStockID = this.id.replace("txtActualConsumed_", "");
    let actualQty = parseFloat($(this).val()) || 0;
    let approvedQty = parseFloat($(this).data("approvedqty")) || 0;

    if (approvedQty > 0) {
        let balanceQty = approvedQty - actualQty;
        $("#txtBalanceQty_" + batchStockID).val(balanceQty.toFixed(2));
    } else {
        $("#txtBalanceQty_" + batchStockID).val("0.00");
    }
});
$("#btnUpdate").on('click', function () {
    var isValid = true; // Initialize validation flag
    ConsumedData = [];

    $("input[id^='txtActualConsumed_']").each(function () {
        //var rowId = parseInt($(this).attr("id").split("_")[1]); // Extract row ID
        var SRTransID = parseInt($(this).attr("StockRequestTransID"));
        var actualConsumed = getValidatedFloat(this);
        var wastageQty = getValidatedFloat("#txtWastageQuantity_" + SRTransID);
        var wastagePercentage = getValidatedFloat("#txtWastagePer_" + SRTransID);
        var balanceQty = getValidatedFloat("#txtBalanceQty_" + SRTransID);

        // Clear previous validation errors
        $(this).removeClass('is-invalid');
        $("#txtWastageQuantity_" + SRTransID).removeClass('is-invalid');
        $('.invalid-feedback').remove();

        if (!actualConsumed) {
            $(this).addClass('is-invalid');
            $(this).after('<div class="invalid-feedback">Please enter Actual Consumed Qty</div>');
            if (isValid) { // Only focus on first error
                $(this).focus();
            }
            isValid = false;
        }


        if (isValid) { // Only add data if this row is valid
            var dataObj = {
                ActualConsumedQty: actualConsumed,
                WastageQty: wastageQty,
                WastagePercentage: wastagePercentage,
                BalanceQty: balanceQty,
                StockRequestTransID: SRTransID
            };
            ConsumedData.push(dataObj);
        }
    });

    // Continue with other validations even if row validations failed
    var OutputData = new Object();
    OutputData.ProductionInwardID = 0;
    OutputData.StockRequestID = parseInt($('#hdnStockRequestID').val());
    OutputData.OutputComponentTypeID = parseInt($('#ddlComponentType').val());
    OutputData.ExpectedProductionQty = getValidatedFloat("#txtExpectedProductQty");
    OutputData.ActualProductionQty = getValidatedFloat("#txtActualProductQty");
    OutputData.ItemID = parseInt($('#ddlItem').val());
    OutputData.RackLocationID = parseInt($('#ddlRackLocation').val());
    OutputData.OperatorID = parseInt($('#ddlEmployeee').val());

    // Clear previous validation errors
    $('#txtExpectedProductQty').removeClass('is-invalid');
    $('#txtActualProductQty').removeClass('is-invalid');

    if (!OutputData.ExpectedProductionQty) {
        $('#txtExpectedProductQty').addClass('is-invalid');
        $('#txtExpectedProductQty').after('<div class="invalid-feedback">Please enter Expected Product Qty</div>');
        if (isValid) {
            $('#txtExpectedProductQty').focus();
        }
        isValid = false;
    }
    if (OutputData.ItemID == 0) {
        $('#ddlItem').addClass('is-invalid');
        $('#ddlItem').after('<div class="invalid-feedback">Please Select Item</div>');
        if (isValid) {
            $('#ddlItem').focus();
        }
        isValid = false;
    }
    if (OutputData.RackLocationID == 0) {
        $('#ddlRackLocation').addClass('is-invalid');
        $('#ddlRackLocation').after('<div class="invalid-feedback">Please Select RackLoaction</div>');
        if (isValid) {
            $('#ddlRackLocation').focus();
        }
        isValid = false;
    }

    if (!OutputData.ActualProductionQty) {
        $('#txtActualProductQty').addClass('is-invalid');
        $('#txtActualProductQty').after('<div class="invalid-feedback">Please enter Actual Product Qty </div>');
        if (isValid) {
            $('#txtActualProductQty').focus();
        }
        isValid = false;
    }

    // Only submit if all validations pass
    if (isValid) {
        OutputData.ProductionConsumption = ConsumedData;
        UpdateProductionInward(OutputData);
    }

    return false;
});
function UpdateProductionInward(OutputData) {
    if (ENABLE_VERBOSE_Logging) //console.log(OutputData);

    $.ajax({
        url: UpdateProductionInwardUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(OutputData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.success && !response.isExists) {
                Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                $("#btnClose").click();
            }
            else if (!response.success && response.isExists) {
                Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
            }
            else if (!response.success && !response.isExists) {
                Swal.fire({ title: "Error", text: SaveErrorMessage, icon: "error", confirmButtonColor: "#556ee6" });
            }
            else
                Swal.fire({ title: "Error", text: response.message, icon: "error", confirmButtonColor: "#556ee6" });
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

    return false;
}
$("#ddlComponentType,#ddlSize,#ddlColor").on('change', function () {

    var filterdata = new Object();
    filterdata.ComponentTypeID = parseInt($('#ddlComponentType').val());
    filterdata.SizeID = parseInt($('#ddlSize').val());
    filterdata.ColorID = parseInt($('#ddlColor').val());

    GetItemDropdownByFilter(filterdata);
    return false;
});
function GetItemDropdownByFilter(FilterData) {
    $.ajax({
        url: GetItemDropdownByFilterUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(FilterData),
        async: false,
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);

            $("#ddlItem").empty();
            $("#ddlItem").append("<option value='0'>--Select--</option>");

            $.each(response.data.Value, function (i, result) {
                $("#ddlItem").append("<option value='" + result.ItemID + "'>" + result.ItemName + "</option>");
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

$('#divPendingStockRequest').on('shown.bs.modal', function () {
    $('#ddlRackLocation').select2({ dropdownParent: $('#divPendingStockRequest'), width: '100%' });
    $('#ddlComponentType').select2({ dropdownParent: $('#divPendingStockRequest'), width: '100%' });
    $('#ddlSize').select2({ dropdownParent: $('#divPendingStockRequest'), width: '100%' });
    $('#ddlColor').select2({ dropdownParent: $('#divPendingStockRequest'), width: '100%' });
    $('#ddlItem').select2({ dropdownParent: $('#divPendingStockRequest'), width: '100%' });
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

function GetHologramPunchingByID(id,componentid,transId,approvedQty) {
    $.ajax({
        url: GetHologramPunchingByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            var data = response;
            if (componentid == BLANKPLATE) {
                $("#txtActualConsumed_" + transId).val(response.data.HologramFinishedQty);
                $("#txtWastageQuantity_" + transId).val(response.data.RejectedPlateQty);
                //console.log($("#txtActualConsumed_" + transId).val());
                //console.log($("#txtWastageQuantity_" + transId).val());
            }
            else {
                $("#txtActualConsumed_" + transId).val(approvedQty);
                $("#txtWastageQuantity_" + transId).val(response.data.HologramWastageQty);
            }

        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}

