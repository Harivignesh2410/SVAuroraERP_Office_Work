var gBoxListarr = [];
var gBatchStock = [];
var gPackingSelectedData = [];
var gMaxCapacity = 0;
var gCurrentTotalPlates = 0;
var extractedData = [];
let innerBoxSaveDataarray = [];

$(function () {
    pLoadingSetup(false);
    $("#divAddEdit").hide();
    $("#divRecords").show();
    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#btnAutoGenerate").hide();
    $("#btnSave").attr("disabled", true);
    getRecordList();
    BindBoxList();
    LoadBoxList("ddlBox");
    $("#txtPackingDate").datetimepicker({
        pickTime: false,
        useCurrent: true,
        format: 'DD/MM/YYYY',
        minYear: 1951,
        maxDate: moment()
    });

    $(".ClearDate").click(function () { var x = $(this).prev().attr('id'); $("#" + x).val(""); });
    $(".SelectDate").click(function () { var x = $(this).next().attr('id'); $("#" + x).focus(); });

    pLoadingSetup(true);
});
function BindBoxList() {
    $.ajax({
        url: BoxListUrl, // Use the variable defined in Razor page
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        async: false,
        success: function (response) {
            gBoxListarr = [];

            $.each(response, function (i, result) {
                var Box = new Object();
                Box.BoxID = result.BoxID;
                Box.SizeID = result.SizeID;
                Box.SizeName = result.SizeName;
                Box.BoxName = result.BoxName;

                gBoxListarr.push(Box);
            });
        }
    });
}
function LoadBoxList(ctrlname) {
    $("#" + ctrlname).empty();
    $("#" + ctrlname).append("<option value='0'>--Select Box--</option>");

    $.each(gBoxListarr, function (i, result) {
        $("#" + ctrlname).append("<option value='" + result.BoxID + "' SizeID='" + result.SizeID + "' HSNCode='" + result.HSNCode + "'>" + result.BoxName + "</option>");
    });
}

$("#btnAddNew").on('click', function () {
    $("#divAddEdit").show();
    $("#divRecords").hide();

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#ddlBox").prop("disabled", false);
    $("#ddlColor").prop("disabled", false);
    $("#divAddEdit .card-body :input").attr("disabled", false);
    $("#divalertNumberPlate").show();
    // Make sure dropdowns and other filters are hidden
    $("#ddlBox").closest(".form-group").show();
    $("#ddlColor").closest(".form-group").show();
    $("#btnFilter").closest(".form-group").show();
    // Clear all previous data completely
    ClearFormFields();

    // Reset global state variables
    gPackingSelectedData = [];
    gCurrentTotalPlates = 0;
    innerBoxSaveDataarray = [];

    // Hide sections that should not be visible initially
    $("#divSelectedItem").empty().hide();
    $("#divInnerBoxes").empty();
    $("#divBox").empty();
    $("#divBatchStock").empty();

    // Only show the filter section
    $("#btnAutoGenerate").hide();
    $("#btnSave").attr("disabled", true);

    $("#divCardTitle").html("<i class='fas fa-plus-square align-middle me-1'></i>Add New Packing");

    return false;
});
$('#divAddEdit').on('shown.bs.modal', function () {
    $('#ddlBox').select2({ dropdownParent: $('#divAddEdit'), width: '100%' });
    $('#ddlColor').select2({ dropdownParent: $('#divAddEdit'), width: '100%' });
});
$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divRecords").show();
    $("#divAddEdit").hide();

    getRecordList();
    return false;
});
$("#btnFilter").on('click', function () {
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    FilterPackingStock();

    return false;
});
$('#btnRefresh').on('click', function () {
    getRecordList();
    return false;
});
$("#ddlColor").on('change', function () {
    var FilterData = new Object();

    let selectedBoxID = $("#ddlBox").val();
    let selectedBox = gBoxListarr.find(item => item.BoxID == selectedBoxID);


    FilterData.BoxID = $('#ddlBox').val();
    if (FilterData.BoxID!=0)
    FilterData.SizeID = selectedBox.SizeID;
    FilterData.ColorID = $('#ddlColor').val();

    if (FilterData.ColorID != 0 && FilterData.SizeID != 0) {
        GetBoxdatabyID(FilterData.BoxID);
        GetPackingStockByFilter(FilterData);
    }
});
function GetPackingStockByFilter(FilterData) {
    $.ajax({
        url: GetPackingStockByFilterUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(FilterData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
            DisplayPackingStockData(response.data)
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
function DisplayPackingStockData(batchStock) {
    $("#divBatchStock").empty();
    let colorCode = "bg-secondary bg-gradient text-white";
    let tableContent = '<div class="table-responsive mt-3">';
    gBatchStock = batchStock;

    tableContent += `
        <table class="table table-striped align-middle" id="tblSearchResult">
            <thead>
                <tr class="table-light">
                    <th class="${colorCode}">Item Name</th>
                    <th class="${colorCode}">Batch No</th>
                    <th class="${colorCode}">Starting Laser No</th>
                    <th class="${colorCode}">Ending Laser No</th>                    
				    <th class="${colorCode}">Quantity</th>
                    <th class='${colorCode} text-center'>Action</th>
                </tr>
            </thead>
            <tbody>`;

    if (batchStock.length != 0) {
        batchStock.forEach((entry) => {
            let isAlreadySelected = gPackingSelectedData.some(stock => stock.BatchStockID === entry.BatchStockID);

            let actionButton = isAlreadySelected
                ? `<span class="badge bg-warning">Selected</span>`
                : `<a href="javascript:void(0);" onclick="AddtoRequest('${entry.BatchStockID}')" class="btn btn-sm btn-soft-success" title="Click here to add to Stock Request List">
                       <i class="bx bxs-select-multiple font-size-16 me-2 align-middle"></i>Select
                   </a>`;

            tableContent += `
            <tr>
                <td>${entry.ItemName}</td>
                <td>${entry.BatchNo}</td>
                <td>${entry.StartLaserNo}</td> 
                <td>${entry.EndLaserNo}</td>
                <td class='text-center'>${entry.PlateCount}</td>
                <td class='text-center'>${actionButton}</td>
            </tr>`;
        });

    } else {
        tableContent += `<tr><td colspan="9" class="text-center">No Batch Records To Display</td></tr>`;
    }

    tableContent += `
            </tbody>
        </table>
    </div>`;

    $("#divBatchStock").html(tableContent);

    return false;
}


function GetBoxdatabyID(id) {
    $.ajax({
        url: GetBoxdatabyIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            if (response != null) {
                gMaxCapacity = response.MaxCapacity;
                $("#divBox").empty();
                let colorCode = "bg-secondary bg-gradient text-white";
                let tableContent = '<div class="table-responsive">';

                tableContent += `
        <table class="table table-striped align-middle" id="tblSearchResult">
            <thead>
                <tr class="table-light">
                    <th class="${colorCode}">Box</th>
                    <th class="${colorCode}">Max Capacity</th>
                    <th class="${colorCode}">No. of Inner Boxes</th>
                    <th class="${colorCode}">Inner Box Quantity</th>                    
                </tr>
            </thead>
            <tbody>`;


                tableContent += `
                <tr>
                    <td>${response.BoxName}</td>
                    <td>${response.MaxCapacity}</td>
                    <td>${response.InnerBoxCount}</td>
                    <td>${response.InnerBoxQuantity}</td>
                </tr>`;


                tableContent += `
            </tbody>
        </table>
    </div>`;

                $("#divBox").html(tableContent);
            }


        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonCategory: "#556ee6" });
        }
    });
    return false;
}
function AddtoRequest(BatchStockID) {
    let searchID = Number(BatchStockID);

    // Check if already added
    if (gPackingSelectedData.find(stock => stock.BatchStockID === searchID)) {
        Swal.fire("Already Added", "This batch is already in your selection.", "info");
        return false;
    }

    let batch = gBatchStock.find(stock => stock.BatchStockID === searchID);
    if (!batch) return false;

    let availableQty = batch.BalanceQty || batch.PlateCount;
    let remainingCapacity = gMaxCapacity - gCurrentTotalPlates;

    if (remainingCapacity <= 0) {
        Swal.fire("Box Full", "You cannot add more items. Box reached its max capacity.", "warning");
        return false;
    }

    let qtyToAdd = Math.min(availableQty, remainingCapacity);

    let baseStartNo = Number(batch.StartingNo);
    if (isNaN(baseStartNo)) {
        Swal.fire("Error", `Invalid Starting Laser No for batch ${batch.BatchNo}`, "error");
        return false;
    }

    let startOffset = batch.PlateCount - availableQty;
    let actualStartingNo = baseStartNo + startOffset;
    let actualEndingNo = actualStartingNo + qtyToAdd - 1;

    let newItem = {
        ...batch,
        StockRequestTransID: 0,
        Quantity: qtyToAdd,
        StartingNo: actualStartingNo,
        EndingNo: actualEndingNo,
        PlateCount: qtyToAdd
    };

    gPackingSelectedData.push(newItem);
    gCurrentTotalPlates += qtyToAdd;

    // Refresh the table to hide the "Select" button
    DisplayPackingStockData(gBatchStock);

    // Update UI
    $("#ddlBox").prop("disabled", true);
    $("#ddlColor").prop("disabled", true);

    DisplayStockRequestData();
    AutoGenerateInnerBoxes();
    return false;
}

function updateCapacityInfo() {
    let remainingCapacity = gMaxCapacity - gCurrentTotalPlates;

    let capacityInfo = `
        <div class="alert ${remainingCapacity > 0 ? 'alert-info' : 'alert-warning'} mb-3">
            <div class="d-flex justify-content-between">
                <span>Box Capacity: ${gMaxCapacity}</span>
                <span>Used: ${gCurrentTotalPlates}</span>
                <span>Remaining: ${remainingCapacity}</span>
            </div>
            <div class="progress mt-2" style="height: 10px;">
                <div class="progress-bar ${remainingCapacity > 0 ? 'bg-info' : 'bg-danger'}" 
                     role="progressbar" 
                     style="width: ${(gCurrentTotalPlates / gMaxCapacity) * 100}%" 
                     aria-valuenow="${gCurrentTotalPlates}" 
                     aria-valuemin="0" 
                     aria-valuemax="${gMaxCapacity}"></div>
            </div>
        </div>
    `;

    // Insert before the selected items table
    $("#divSelectedItem").prepend(capacityInfo);
}
function DisplayStockRequestData() {
    $("#divSelectedItem").empty();
    $("#divSelectedItem").show();

    // Add capacity info at the top
    let remainingCapacity = gMaxCapacity - gCurrentTotalPlates;
    let capacityInfoHtml = `
        <div class="alert ${remainingCapacity > 0 ? 'alert-info' : 'alert-warning'} mb-3">
            <div class="d-flex justify-content-between">
                <span>Box Capacity: ${gMaxCapacity}</span>
                <span>Used: ${gCurrentTotalPlates}</span>
                <span>Remaining: ${remainingCapacity}</span>
            </div>
            <div class="progress mt-2" style="height: 10px;">
                <div class="progress-bar ${remainingCapacity > 0 ? 'bg-info' : 'bg-danger'}" 
                     role="progressbar" 
                     style="width: ${(gCurrentTotalPlates / gMaxCapacity) * 100}%" 
                     aria-valuenow="${gCurrentTotalPlates}" 
                     aria-valuemin="0" 
                     aria-valuemax="${gMaxCapacity}"></div>
            </div>
        </div>
    `;

    $("#divSelectedItem").append(capacityInfoHtml);

    // Add Item Details header
    let tableContent = '<div class="alert alert-info mt-2" role="alert">Selected Number Plate</div>';
    let colorCode = "bg-success bg-gradient text-white";
    tableContent += '<div class="table-responsive">'
    tableContent += `
        <table class="table table-striped align-middle" id="tblStockRequestItemData">
            <thead>
                 <tr class="table-light">
                    <th class="${colorCode}">Item Name</th>
                    <th class="${colorCode}">Batch No</th>
                    <th class="${colorCode}">Starting Laser No</th>
                    <th class="${colorCode}">Ending Laser No</th>                    
                    <th class="${colorCode}">Quantity</th>
                    <th class='${colorCode} text-center'>Action</th>
                </tr>
            </thead>
            <tbody>`;

    if (gPackingSelectedData.length != 0) {
        gPackingSelectedData.forEach((entry, index) => {
            tableContent += `
                <tr>
                    <td>${entry.ItemName}</td>
                    <td>${entry.BatchNo}</td>
                    <td>${entry.StartLaserNo}</td> 
                    <td>${entry.EndLaserNo}</td>
                    <td class='text-center'>${entry.PlateCount}</td>
                    <td class='text-center'>`;

            // Only show remove button if not in view mode
            if ($("#btnSave").is(":visible") || $("#btnUpdate").is(":visible")) {
                tableContent += `<a href="javascript:void(0);" onclick="RemoveFromRequest(${index})" class="btn btn-sm btn-soft-danger" title="Remove this item">
                            <i class="bx bx-trash font-size-16 me-2 align-middle"></i>Remove
                        </a>`;
            } else {
                tableContent += `<span class="text-muted">View Mode</span>`;
            }

            tableContent += `</td>
                </tr>`;
        });
    }
    else {
        tableContent += `<tr> <td colspan="7" class="text-center">No data</td></tr> `;
    }

    tableContent += `
            </tbody>
        </table>
    </div> `;

    $("#divSelectedItem").append(tableContent);

    // Only show auto generate button if not in view mode
    if ($("#btnSave").is(":visible") || $("#btnUpdate").is(":visible")) {
        $("#btnAutoGenerate").show();
    } else {
        $("#btnAutoGenerate").hide();
    }

    if (remainingCapacity == 0) {
        $("#btnSave").attr("disabled", false);
    }
    else {
        $("#btnSave").attr("disabled", true);
    }

    return false;
}
function RemoveFromRequest(index) {
    if (index >= 0 && index < gPackingSelectedData.length) {
        // Subtract the removed quantity from the total
        gCurrentTotalPlates -= gPackingSelectedData[index].PlateCount;

        // Remove the item
        gPackingSelectedData.splice(index, 1);
        // Sort again
        gPackingSelectedData.sort((a, b) => a.StartingLaserNo - b.StartingLaserNo);
        // If no items left, enable dropdown controls again
        if (gPackingSelectedData.length === 0) {
            $("#ddlBox").prop("disabled", false);
            $("#ddlColor").prop("disabled", false);
        }

        // Update display
        DisplayStockRequestData();
        DisplayPackingStockData(gBatchStock); 
        AutoGenerateInnerBoxes();
    }
    return false;
}
$("#btnAutoGenerate").on("click", function () {
    AutoGenerateInnerBoxes();
    return false;
});
function AutoGenerateInnerBoxes() {
    // Clear previous data
    $("#divInnerBoxes").empty();
    innerBoxSaveDataarray = []; // Clear the array before populating it again

    if (gPackingSelectedData.length === 0) {
        $("#divSelectedItem").hide();
        $("#btnAutoGenerate").hide();
        return false;
    }

    gPackingSelectedData.sort((a, b) => a.StartingLaserNo - b.StartingLaserNo);

    const innerBoxCount = parseInt($("#divBox table tbody tr td:eq(2)").text()) || 0;
    const innerBoxQuantity = parseInt($("#divBox table tbody tr td:eq(3)").text()) || 0;

    if (innerBoxCount <= 0 || innerBoxQuantity <= 0) {
        Swal.fire("Invalid Configuration", "Box inner count or quantity is invalid.", "error");
        return false;
    }

    // Collect all laser numbers from selected items
    let allLaserNumbers = [];
    gPackingSelectedData.forEach(item => {
        const prefix = item.LaserNoPrefix || "CD";
        const start = parseInt(item.StartingNo);
        const end = parseInt(item.EndingNo);

        for (let i = start; i <= end; i++) {
            const formattedNumber = i.toString().padStart(8, '0'); // KD00001132
            const fullLaserNo = prefix + formattedNumber;
            //const formattedNumber = i.toString();
            allLaserNumbers.push({
                prefix: prefix,
                number: i,
                displayFormat: fullLaserNo,        // For display → KD00001132
                rawNumber: i.toString()            // For storing → 1132 (no padding)
            });
        }
    });

    if (allLaserNumbers.length === 0) {
        Swal.fire("No Data", "No valid laser numbers found.", "warning");
        return false;
    }

    const currentDate = new Date();
    const yearMonth = currentDate.getFullYear().toString().substring(2) +
        (currentDate.getMonth() + 1).toString().padStart(2, '0');
    const baseInnerBoxId = "IN" + yearMonth + $("#ddlBox").val().toString().padStart(8, '0');

    // In add mode, don't show the innerBoxNo column
    let tableContent = `
    <div class="alert alert-success mt-3" role="alert">Inner Box Distribution</div>
    <div class="table-responsive">
        <table class="table table-striped table-bordered align-middle" id="tblInnerBoxes">
            <thead>
                <tr class="bg-primary text-white">
                    <th class="text-center">S.No</th>
                    <th class="text-center">Starting Laser No</th>
                    <th class="text-center">Ending Laser No</th>
                    <th class="text-center">No.of Plate</th>
                </tr>
            </thead>
            <tbody>
    `;

    const totalInnerBoxes = Math.min(innerBoxCount, Math.ceil(allLaserNumbers.length / innerBoxQuantity));

    for (let i = 0; i < totalInnerBoxes; i++) {
        const startIdx = i * innerBoxQuantity;
        const endIdx = Math.min(startIdx + innerBoxQuantity - 1, allLaserNumbers.length - 1);

        if (startIdx > allLaserNumbers.length - 1) break;

        // Generate inner box number but don't display it in add mode
        const innerBoxNo = baseInnerBoxId + (i + 1).toString().padStart(2, '0');

        // Get the displayed format of the laser numbers
        const startLaser = allLaserNumbers[startIdx].displayFormat;
        const endLaser = allLaserNumbers[endIdx].displayFormat;
        const plateCount = endIdx - startIdx + 1;

        tableContent += `
        <tr>
            <td class="text-center">${i + 1}</td>
            <td class="text-center">${startLaser}</td>
            <td class="text-center">${endLaser}</td>
            <td class="text-center">${plateCount}</td>
        </tr>
    `;

        // Still store the innerBoxNo in the data array for saving
        innerBoxSaveDataarray.push({
            InnerBoxNo: innerBoxNo,
            StartingLaserNo: allLaserNumbers[startIdx].rawNumber, // 1132
            EndingLaserNo: allLaserNumbers[endIdx].rawNumber,     // e.g., 1156
            Quantity: plateCount,
            LaserNoPrefix: allLaserNumbers[startIdx].prefix
        });

    }

    tableContent += `
            </tbody>
        </table>
    </div>
    `;

    $("#divInnerBoxes").html(tableContent);
    return false;
}

$("#btnSave").on('click', function () {
    let isValid = true;
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    var Packing = new Object();

    // Set ID and StatusFlag
    Packing.PackingID = 0;
    if (this.id == "btnUpdate" && $("#hdnPackingID").val() > 0) {
        Packing.PackingID = $("#hdnPackingID").val();
    }

    Packing.sPackingDate = $("#txtPackingDate").val();
    Packing.BOXID = $("#ddlBox").val();
    Packing.ColorID = $("#ddlColor").val();
    Packing.ColorID = $("#ddlColor").val();
    Packing.AllotedToID = $("#ddlEmbossingsataion").val();

    // Validations
    if (!Packing.sPackingDate) {
        $('#txtPackingDate').addClass('is-invalid');
        $('#txtPackingDate').after('<div class="invalid-feedback">Please select Packing Date</div>');
        $('#txtPackingDate').focus();
        return false;
    }

    if (innerBoxSaveDataarray.length <= 0) {
        $.jGrowl("No Packing Data Are Selected", { sticky: false, theme: 'warning', life: 3000 });
        return false;
    }

    Packing.PackingTrans = innerBoxSaveDataarray;

  SaveandUpdatePacking(Packing);
    return false;
}); 
function SaveandUpdatePacking(Packing) {
    if (ENABLE_VERBOSE_Logging) //console.log(Packing);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(Packing),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);

            if (response.data.Item1) {
                if (Packing.PackingID == 0) {
                    Swal.fire({ title:"Packing Submitted ", text: "Packing & Inner Box No Generated Successfully", icon: "success", confirmButtonColor: "#556ee6" });
                    
                }
                else if (Packing.PackingID > 0)
                    Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                EditData(response.data.Item2, true);
                //$("#btnClose").click();
            }
            else if (!response.success && response.isExists) {
                Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
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

    return false
}
function getRecordList() {
    // Check if DataTable has already been initialized
    if ($.fn.DataTable.isDataTable('#tblrecordlist')) {
        $('#tblrecordlist').DataTable().clear().destroy();  // Destroy previous instance
    }

    $('#tblrecordlist').DataTable({
        "processing": true,
        "serverSide": true,
        "ordering": true,  // Enable sorting on columns
        "ajax": {
            url: ListDataUrl,
            "type": "GET",
            "data": function (d) {
                return $.extend({}, d, {
                    // Custom parameters here (if any)
                });
            }
        },
        language: { oPaginate: { sNext: '<i class="mdi mdi-chevron-right"></i>', sPrevious: '<i class="mdi mdi-chevron-left"></i>' } },
        "columns": [
            {
                data: null, // Serial number (S No.)
                render: function (data, type, row, meta) {
                    return meta.settings._iDisplayStart + meta.row + 1;
                },
                bSortable: false,
                "width": "5%",
                "orderable": false
            },
            { "data": "PackingNo", "orderable": true, "width": "10%" },
            { "data": "PackingDate", "orderable": true },
            { "data": "BoxName", "orderable": true },
            { "data": "SizeName", "orderable": true },
            { "data": "ColorName", "orderable": true },
            { "data": "BoxCount", "orderable": true, "className": "text-end" },
            { "data": "TotalQuantity", "orderable": true, "className": "text-end" },
            { "data": "PcsPerBox", "orderable": true, "className": "text-end" },
            { "data": "CompanyName", "orderable": true, "className": "text-end" },
            {
                "data": "StatusID", "orderable": true, "width": "10%", "className": "text-center",
                "render": function (data, type, row) {
                    return `<span class="${row.ColorCode}">${row.StatusName}</span>`;
                }
            },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    // Always show the "View" button
                    let actionButtons = `
                        <ul class="list-unstyled hstack gap-1 mb-0">
                            <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                                <a href="javascript:void(0);" onclick="EditData(${row.PackingID}, true)" class="btn btn-sm btn-soft-primary">
                                    <i class="mdi mdi-eye-outline"></i>
                                </a>
                            </li>`;

                    // Show "Delete" button only if StatusID == 1
                    if (row.StatusID === 1) {
                        actionButtons += `
                            <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
                                <a href="javascript:void(0);" onclick="DeleteData('${row.PackingID}')" class="btn btn-sm btn-soft-danger">
                                    <i class="mdi mdi-delete-outline"></i>
                                </a>
                            </li>`;
                    }

                    actionButtons += `</ul>`;
                    return actionButtons;
                },
                "width": "5%",
                "orderable": false
            }
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
    return false;
}
function ClearFormFields() {
    $("#txtPackingNo").val("");
    $("#txtPackingDate").val("");

    $("#ddlBox").val(0).change();
    $("#ddlColor").val(0).change();
    $("#ddlEmbossingsataion").val(0).change();

    $("#divBatchStock").empty();
    $("#divBox").empty();
    $("#divSelectedItem").empty();
    $("#divInnerBoxes").empty();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();
    $("#hdnPackingID").val(0);
    // Hide filter and selection sections
    $("#divBatchStock").show();
    $("#divSelectedItem").show();
    $("#divBox").show();

    // Reset global variables that store data
    gPackingSelectedData = [];
    gCurrentTotalPlates = 0;
    gMaxCapacity = 0;

    return false;
}
function EditData(id, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) //console.log(id);
    ClearFormFields();
    innerBoxSaveDataarray = [];
    $("#divInnerBoxes").empty();
    gPackingSelectedData = [];
    gCurrentTotalPlates = 0;

    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            $("#divAddEdit").show();
            $("#divRecords").hide();

            var stockdata = response.data;
            $("#hdnPackingID").val(stockdata.PackingID);

            if (ViewFlag) {
                // In view mode, show minimal UI
                $("#btnSave").hide();
                $("#btnUpdate").hide();
                $("#btnAutoGenerate").hide();
                $("#divAddEdit .card-body :input").attr("disabled", true);
                $("#btnClose").attr("disabled", false);
                $("#divCardTitle").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Packing");

                // Show only packing no, packing date
                $("#txtPackingNo").val(stockdata.PackingNo);
                $("#txtPackingDate").val(stockdata.PackingDate);
                $("#ddlEmbossingsataion").val(stockdata.AllotedToID).change();

                // Hide filter and selection sections
                $("#divBatchStock").hide();
                $("#divSelectedItem").hide();
                $("#divBox").hide();
                //$("#btnFilter").hide();
                $("#divalertNumberPlate").hide();

                // Other fields should be hidden
                $(".form-group").not(":has(#txtPackingNo, #txtPackingDate,#ddlEmbossingsataion)").hide();

                // Make sure dropdowns and other filters are hidden
                $("#ddlBox").closest(".form-group").hide();
                $("#ddlColor").closest(".form-group").hide();
                $("#btnFilter").closest(".form-group").hide();
            }
            else {
                // In edit mode, show full UI
                $("#divAddEdit .card-body :input").attr("disabled", false);
                $("#divCardTitle").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Packing");
                $("#btnSave").hide();
                $("#btnUpdate").show();
                $("#divSelectedItem").show();

                // Show all fields
                $(".form-group").show();

                // Set all form values
                $("#txtPackingNo").val(stockdata.PackingNo);
                $("#ddlBox").val(stockdata.BOXID).change();
                $("#ddlColor").val(stockdata.ColorID).change();
                $("#txtPackingDate").val(stockdata.PackingDate);

                // Get the max capacity from the box data
                GetBoxdatabyID(stockdata.BOXID);
            }

            // Display inner boxes regardless of view mode
            if (stockdata.PackingTrans && stockdata.PackingTrans.length > 0) {
                let innerBoxTableContent = `
                <div class="alert alert-success mt-3" role="alert">Inner Box Distribution</div>
                <div class="table-responsive">
                    <table class="table table-striped table-bordered align-middle" id="tblInnerBoxes">
                        <thead>
                            <tr class="bg-primary text-white">
                                <th class="text-center">S.No</th>
                                <th class="text-center">Starting Laser No</th>
                                <th class="text-center">Ending Laser No</th>
                                <th class="text-center">No.of Plate</th>
                                <th class="text-center">Inner Box No</th>
                            </tr>
                        </thead>
                        <tbody>
                `;

                stockdata.PackingTrans.forEach((entry, index) => {
                    // Calculate the total plates for capacity info
                    gCurrentTotalPlates += entry.Quantity;

                    // Format the laser numbers for display
                    //const startLaser = entry.LaserNoPrefix || 'CD'+'0000' + entry.StartingLaserNo;
                    //const endLaser = entry.LaserNoPrefix || 'CD' + '0000'+entry.EndingLaserNo;

                    const startLaser = (entry.LaserNoPrefix || 'CD') + entry.StartingLaserNo.toString().padStart(8, '0');
                    const endLaser = (entry.LaserNoPrefix || 'CD') + entry.EndingLaserNo.toString().padStart(8, '0');


                    innerBoxTableContent += `
                    <tr>
                        <td class="text-center">${index + 1}</td>
                        <td class="text-center">${startLaser}</td>
                        <td class="text-center">${endLaser}</td>
                        <td class="text-center">${entry.Quantity}</td>
                        <td class="text-center">${entry.InnerBoxNo || "-"}</td>
                    </tr>
                    `;

                    // Store for Save/Update operations (not needed in view mode but kept for consistency)
                    if (!ViewFlag) {
                        innerBoxSaveDataarray.push({
                            InnerBoxNo: entry.InnerBoxNo,
                            StartingLaserNo: entry.StartingLaserNo,
                            EndingLaserNo: entry.EndingLaserNo,
                            Quantity: entry.Quantity,
                            LaserNoPrefix: entry.LaserNoPrefix
                        });
                    }
                });

                innerBoxTableContent += `
                        </tbody>
                    </table>
                </div>
                `;

                $("#divInnerBoxes").html(innerBoxTableContent);

                // Only create selection display in edit mode
                if (!ViewFlag) {
                    // Create selection display
                    if (stockdata.BatchDetails && stockdata.BatchDetails.length > 0) {
                        gPackingSelectedData = stockdata.BatchDetails;
                    } else {
                        // Create placeholder data if BatchDetails not available
                        const dummySelectedItem = {
                            ItemName: stockdata.ItemName || "Number Plate",
                            BatchNo: stockdata.BatchNo || "-",
                            LaserNoPrefix: stockdata.PackingTrans[0].LaserNoPrefix,
                            StartingNo: stockdata.PackingTrans[0].StartingLaserNo,
                            EndingNo: stockdata.PackingTrans[stockdata.PackingTrans.length - 1].EndingLaserNo,
                            PlateCount: gCurrentTotalPlates
                        };
                        gPackingSelectedData.push(dummySelectedItem);
                    }
                    DisplayStockRequestData();
                }
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
function DeleteData(id) {
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
            if (response.success && response.isExists) {
                Swal.fire({ title: "Deleted!", text: DeleteSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                $("#btnRefresh").click();
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





