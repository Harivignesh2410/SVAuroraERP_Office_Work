var stockRequestId = 0;
$(function () {
    pLoadingSetup(false);
    $("#divPendingStockRequest").hide();
    $("#divSearchPage").show();

    // Initialize Datepicker
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

    initDatePicker(); // Run on page load

    FilterPurchaseEntry();
    pLoadingSetup(true);
});

// Ensure Datepicker is re-initialized when the div is displayed
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
        <table class="table table-striped align-middle" id="tblPendingforApproval">
            <thead>
                <tr class="table-light">
				        <th>S No.</th>
						<th>Request No.</th>
						<th>Date</th>
						<th>Process Type</th>
						<th>Requested By</th>
						<th>Status</th>
						<th></th>
                </tr>
            </thead>
            <tbody>`;

    if (Completeddata.length != 0) {
        Completeddata.forEach((entry, index) => {
            tableContent += `
                <tr data-sno="${index + 1}">
                    <td>${index + 1}</td>
                    <td>${entry.RequestNo}</td>
                    <td>${entry.sRequestDate}</td>
                    <td>${entry.ProcessTypeName}</td>
                    <td>${entry.RequestedByName}</td>
                    <td><span class="badge ${entry.ColorCode}">${entry.StockRequestStatus}</span></td>
                    <td>
                        <ul class="list-unstyled hstack gap-1 mb-0 text-center">
                            <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                               <a href="javascript:void(0);" onclick="EditData(${entry.StockRequestID}, true)" class="btn btn-sm btn-soft-primary">
                               <i class="mdi mdi-eye-outline me-2"></i>View
                               </a>
                            </li>
                            <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Edit">
                               <a href="javascript:void(0);" onclick="EditData(${entry.StockRequestID}, false)" class="btn btn-sm btn-success">
                               <i class="mdi mdi-check-all me-2"></i>Approve
                               </a>
                           </li>
                        </ul>
                    </td>
                 </tr>`;
        });
    }
    tableContent += `
            </tbody>
        </table>
    </div> `;

    $("#divSearchResultSummary").html(tableContent);

    $("#tblPendingforApproval").DataTable({
        "bAutoWidth": false,
        "bPaginate": false,
        "bFilter": true,
        "bSort": false,
        "order": [], // Disable initial sorting
        "pagingType": "full_numbers"
    });
}
function EditData(id, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) //console.log(id);
    //ClearFormFields();
    stockRequestId = 0;
    $.ajax({
        url: GetStockRequestDetailsByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            if (ViewFlag) {
                $("#btnApproved").hide();
                $("#btnRejected").hide();
                //$("#divAddEdit .card-body :input").attr("disabled", true);
                $("#divCardTitle").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Stock Request");
            }
            else {
                $("#divCardTitle").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Approve Stock Request");
                $("#btnApproved").show();
                $("#btnRejected").show();
                stockRequestId = id;
            }
            $("#divPendingStockRequest").show();
            $("#divSearchPage").hide();
            // $("#divNarration").hide();
            $("#btnApproved").prop("disabled", false);
            var stockdata = response.data;
            RenderPendingstrockRequestDetails(stockdata);
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
        <div class="row task-dates">
            <div class="col-sm-3 col-6">
                <div class="mt-4">
                    <p class="text-muted mb-2">Request No.</p>
                    <h5 class="font-size-14"><i class="bx bx-copy-alt me-1 text-primary"></i>${data.RequestNo}</h5>
                </div>
            </div>
            <div class="col-sm-3 col-6">
                <div class="mt-4">
                    <p class="text-muted mb-2">Request Date</p>
                    <h5 class="font-size-14"><i class="bx bx-calendar-check me-1 text-primary"></i>${data.sRequestDate}</h5>
                </div>
            </div>
            <div class="col-sm-3 col-6">
                <div class="mt-4">
                    <p class="text-muted mb-2">Process Type</p>
                    <h5 class="font-size-14"><i class="bx bx-chevrons-right me-1 text-primary"></i>${data.ProcessTypeName}</h5>
                </div>
            </div>
            <div class="col-sm-3 col-6">
                <div class="mt-4">
                    <p class="text-muted mb-2">Request By</p>
                    <h5 class="font-size-14"><i class="bx bx-user me-1 text-primary"></i>${data.RequestedByName}</h5>
                </div>
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
                    <th class="${colorCode}">Colour</th>
                    <th class="${colorCode}">Size</th>                    
				    <th class="${colorCode}">Batch No</th>
					<th class='${colorCode} text-end'>Requested Quantity</th>					
                    <th class='${colorCode} text-end'>Available Stock</th>
                    <th class="${colorCode} text-center">Status</th>
                </tr>
            </thead>
            <tbody>`;

    var StockAvailableCount = 0;
    if (data.VStockRequestTrans.length != 0) {
        StockAvailableCount = data.VStockRequestTrans.filter(x => x.StockStatus === "Stock Available").length;
        data.VStockRequestTrans.forEach((entry, index) => {
            tableContent += `
                    <tr>                  
                    <td>${entry.ComponentTypeName}</td>
                    <td>${entry.ItemName}</td>
                    <td>${entry.ColorName}</td>
                    <td>${entry.SizeName}</td>
                    <td>${entry.BatchNo}</td>
                    <td class='text-end'>${entry.Quantity.toFixed(2)}</td>
                    <th class='text-end'>${entry.BalanceQty.toFixed(2)}</th>
                    <td class='text-center'>${GetStockStatus(entry.StockStatus)}</td>
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

    //Disable Approve Button when any Insufficient Stock
    //if (StockAvailableCount != data.VStockRequestTrans.length) $("#btnApproved").prop("disabled", true);
}

$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divSearchPage").show();
    $("#divPendingStockRequest").hide();

    FilterPurchaseEntry();
    return false;
});

$("#btnApproved").on('click', function () {
    var StockApprovalRequest = new Object();
    StockApprovalRequest.StockRequestID = stockRequestId;
    StockApprovalRequest.StatusID = 2; //Approved
    StockApprovalRequest.Narration = $('#txtNarration').val();

    SaveApproveorReject(StockApprovalRequest);

    return false;
});
$("#btnRejected").on('click', function () {
    if ($('#txtNarration').val() == "") {
        $.jGrowl("Kindly provide rejection reason!", { sticky: false, theme: 'warning', life: 3000 });
        $('#txtNarration').focus();
        return false;
    }

    var StockApprovalRequest = new Object();
    StockApprovalRequest.StockRequestID = stockRequestId;
    StockApprovalRequest.StatusID = 3; //Rejected
    StockApprovalRequest.Narration = $('#txtNarration').val();

    SaveApproveorReject(StockApprovalRequest);

    return false;
});
function SaveApproveorReject(StockApprovalRequest) {
    $.ajax({
        url: SaveApprovalUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(StockApprovalRequest),
        success: function (response) {
            if (response.success && !response.isExists) {
                if (StockApprovalRequest.StatusID == 2)
                    Swal.fire({ title: "Approved", text: "Successfully Approved", icon: "success", confirmButtonColor: "#556ee6" });
                else
                    Swal.fire({ title: "Rejected", text: "Successfully Rejected", icon: "success", confirmButtonColor: "#556ee6" });

                $("#btnClose").click();
            }
            else
                Swal.fire({ title: "Error", text: response.message, icon: "error", confirmButtonColor: "#556ee6" });
        },
        error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
    return false;
}