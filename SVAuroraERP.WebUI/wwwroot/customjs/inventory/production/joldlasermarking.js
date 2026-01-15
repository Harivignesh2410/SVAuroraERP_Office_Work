var ConsumedData = [];
function getValidatedFloat(selector) {
    let value = parseFloat($(selector).val());
    return isNaN(value) ? 0 : value;
}
$(function () {
    pLoadingSetup(false);
    $("#divLaserNoMarking").hide();
    $("#divSearchPage").show();

    GetLaserNoMarkingList();
    pLoadingSetup(true);
});
function GetLaserNoMarkingList() {
    $.ajax({
        url: GetLaserNoMarkingListUrl,
        type: 'GET',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
            DisplayPendingforapprovalData(response.data);
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
                       <button type="button" onclick="EditData(${entry.StockRequestID})" class="btn btn-sm btn-primary btn-rounded waves-effect waves-light" title="Click here to Add New">
						LaserNo Marking
					   </button>
                    </td>`;
        });
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
    //ClearFormFields();
    stockRequestId = 0;
    $.ajax({
        url: GetStockRequestDetailsByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            var stockdata = response.data;
            $("#hdnStockRequestID").val(id);
            $("#divLaserNoMarking").show();
            $("#divSearchPage").hide();

            $('#txtLaserNoPrefix').val("");
            $('#txtLaserStartingNo').val("0");
            $('#txtNoOfPlateCount').val("0");
            $('#txtRejectedPlateCount').val("0");
            $('#txtStartingNo').val("0");
            $('#txtEndingNo').val("0");

            $("#ddlMachineType").val("0").change();
            $("#ddlEmployee").val("0").change();
            $('#txtApprovedQty').val(response.data.VStockRequestTrans[0].Quantity.toFixed(2));
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
        <div class="row g-2 task-dates">
            <div class="col-lg-4 col-md-4 col-sm-6 col-12">
                <p class="text-muted mb-2">Request No.</p>
                <h5 class="font-size-14"><i class="bx bx-copy-alt me-1 text-primary"></i>${data.RequestNo}</h5>
            </div>
            <div class="col-lg-4 col-md-4 col-sm-6 col-12">
                <p class="text-muted mb-2">Request Date</p>
                <h5 class="font-size-14"><i class="bx bx-calendar-check me-1 text-primary"></i>${data.sRequestDate}</h5>
            </div>
                       <div class="col-lg-4 col-md-4 col-sm-6 col-12">
                <p class="text-muted mb-2">Approved By</p>
                <h5 class="font-size-14"><i class="bx bx-user me-1 text-primary"></i>${data.ApprovedByName}</h5>
            </div>
            
        </div>
    `;
    $("#divPendingStockRequestTrans").append(headerDetails);
    $(".decimal").inputmask("decimal", { digits: 2, radixPoint: "." });
}

$("#btnUpdate").on('click', function () {
    var isValid = true; // Initialize validation flag

    var MarkingData = new Object();
    MarkingData.LaserNoMarkingID = 0;
    MarkingData.StockRequestID = parseInt($('#hdnStockRequestID').val());
    MarkingData.MachineID = parseInt($('#ddlMachineType').val());
    MarkingData.EmployeeID = ($("#ddlEmployeee").val());
    MarkingData.LaserNoPrefix = $("#txtLaserNoPrefix").val();
    MarkingData.LaserNoStartingNo = parseInt($("#txtLaserStartingNo").val());
    MarkingData.PlateCount = parseInt($("#txtNoOfPlateCount").val());
    MarkingData.RejectedPlateCount = parseInt($("#txtRejectedPlateCount").val());
    MarkingData.StartingNo = parseInt($("#txtStartingNo").val().slice(-4));
    MarkingData.EndingNo = parseInt($("#txtEndingNo").val().slice(-4));
    MarkingData.Remarks = $("#txtRemarks").val();

    if (MarkingData.MachineID == 0) {
        $('#ddlMachineType').addClass('is-invalid');
        $('#ddlMachineType').after('<div class="invalid-feedback">Please Select Machine Type</div>');
        if (isValid) {
            $('#ddlMachineType').focus();
        }
        isValid = false;
    }
    if (MarkingData.EmployeeID == 0) {
        $('#ddlEmployeee').addClass('is-invalid');
        $('#ddlEmployeee').after('<div class="invalid-feedback">Please Select Employee</div>');
        if (isValid) {
            $('#ddlEmployeee').focus();
        }
        isValid = false;
    }
    if (MarkingData.LaserNoPrefix == '') {
        $('#txtLaserNoPrefix').addClass('is-invalid');
        $('#txtLaserNoPrefix').after('<div class="invalid-feedback">Please Enter LaserNo Prefix </div>');
        if (isValid) {
            $('#txtHologramFinishedQty').focus();
        }
        isValid = false;
    }
    if (!MarkingData.LaserNoStartingNo) {
        $('#txtLaserStartingNo').addClass('is-invalid');
        $('#txtLaserStartingNo').after('<div class="invalid-feedback">Please Enter Starting Laser No</div>');
        if (isValid) {
            $('#txtLaserStartingNo').focus();
        }
        isValid = false;
    }
    if (!MarkingData.PlateCount) {
        $('#txtNoOfPlateCount').addClass('is-invalid');
        $('#txtNoOfPlateCount').after('<div class="invalid-feedback">Please Enter Plate count</div>');
        if (isValid) {
            $('#txtNoOfPlateCount').focus();
        }
        isValid = false;
    }

    //if (!MarkingData.RejectedPlateCount) {
    //    $('#txtRejectedPlateCount').addClass('is-invalid');
    //    $('#txtRejectedPlateCount').after('<div class="invalid-feedback">Please Enter Rejected Plate Count</div>');
    //    if (isValid) {
    //        $('#txtRejectedPlateCount').focus();
    //    }
    //    isValid = false;
    //}
    if (!MarkingData.EndingNo) {
        $('#txtEndingNo').addClass('is-invalid');
        $('#txtEndingNo').after('<div class="invalid-feedback">Please Enter Ending Laser No</div>');
        if (isValid) {
            $('#txtEndingNo').focus();
        }
        isValid = false;
    }
    if (!MarkingData.StartingNo) {
        $('#txtStartingNo').addClass('is-invalid');
        $('#txtStartingNo').after('<div class="invalid-feedback">Please Enter Starting Laser No</div>');
        if (isValid) {
            $('#txtStartingNo').focus();
        }
        isValid = false;
    }

    //var Approveddata = getValidatedFloat("#txtApprovedQty");
    //var totalqty = MarkingData.HologramFinishedQty + MarkingData.RejectedPlateQty + MarkingData.HologramWastageQty;
    //if (Approveddata != totalqty) {
    //    $.jGrowl("Kindly Enter the valid Quantity", { sticky: false, theme: 'warning', life: 3000 });
    //    isValid = false;
    //}


    // Only submit if all validations pass
    if (isValid) {
        UpdateLaserNoMarking(MarkingData);
    }

    return false;
});
function UpdateLaserNoMarking(MarkingData) {
    if (ENABLE_VERBOSE_Logging) //console.log(MarkingData);

    $.ajax({
        url: SaveLaserNoMarkingUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(MarkingData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.success && !response.isExists) {
                Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                $("#btnClose").click();
            }
            else if (response.success && response.isExists) {
                Swal.fire({ title: "Data already exists!", text: "Laser No.Prefix/Starting No. Exists", icon: "warning", confirmButtonColor: "#556ee6" });
            }
            else if (!response.success && !response.isExists) {
                Swal.fire({ title: "Error", text: SaveErrorMessage, icon: "error", confirmButtonColor: "#556ee6" });
            }
            else
                Swal.fire({ title: "Error", text: response.message, icon: "error", confirmButtonColor: "#556ee6" });
        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

    return false;
}
$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divSearchPage").show();
    $("#divLaserNoMarking").hide();

    GetLaserNoMarkingList();
    return false;
});

$("#txtLaserNoPrefix, #txtNoOfPlateCount,#txtLaserStartingNo").on("input", function () {

    let Prefix = $("#txtLaserNoPrefix").val() || ''; // Default to 0 if empty
    let NoofPlate = parseFloat($("#txtNoOfPlateCount").val()) || 0;
    let StartingNo = parseFloat($("#txtLaserStartingNo").val()) || 0; // Default to 0 if empty

    var starting = Prefix + "000000" + StartingNo;
    var end = StartingNo + NoofPlate;
    var ending = Prefix + "000000" + end;

    $("#txtStartingNo").val(starting);
    $("#txtEndingNo").val(ending);
});


