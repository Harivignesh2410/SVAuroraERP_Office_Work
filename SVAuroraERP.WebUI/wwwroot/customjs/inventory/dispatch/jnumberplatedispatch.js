var SelectedPackingArray = [];
$(function () {
    pLoadingSetup(false);
    getRecordList();
    $("#divAddEdit").hide();
    $("#divRecords").show();
    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#txtDispatchDate,#txtDocketDate").datetimepicker({
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

$("#btnAddNew").on('click', function () {
    SelectedPackingArray = [];
    //ClearFormFields();
    $("#divCardTitle").html("<i class='fas fa-plus-square align-middle me-1'></i>Add New Dispatch");
    $("#divRecords").hide();
    $("#divAddEdit").show();
    $("#divPackingList").empty();
   
    $("#btnSave").show();
    $("#btnUpdate").hide();
   //GetPackingList();
    $('#divVehicleDropdown').hide();
    $('#divVehicle').hide();
    $("#divAddEdit .card-body :input").attr("disabled", false);
    Clearform();
    return false;
});
function Clearform() {
    $("#txtDispatchDate").val('');
    $("#ddlModeOfTransport").val('0').change();
    $("#ddlCourier").val('0').change();
    $("#txtVehicle").val('');
    $("#txtDocketNo").val('');
    $("#txtDocketDate").val('');
    $("#ddlEmbossingStation").val('0').change();
}
$('#btnRefresh').on('click', function () {
    getRecordList();
    return false;
});

$('#ddlModeOfTransport').on('change', function () {
    var selectedValue = $(this).val();

    if (selectedValue === "1") {
        // Show Courier Vehicle dropdown
        $('#divVehicleDropdown').show();
        $('#divVehicle').hide();
    } else if (selectedValue === "2") {
        // Show Own Vehicle input field
        $('#divVehicleDropdown').hide();
        $('#divVehicle').show();
    } else {
        $('#divVehicleDropdown').hide();
        $('#divVehicle').hide();
    }
});

$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divRecords").show();
    $("#divAddEdit").hide();

   // getRecordList();
    return false;
});

$('#ddlEmbossingStation').on('change', function () {
    var selectedValue = $(this).val();

    if (selectedValue > 0) {
        GetPackingList(parseInt(selectedValue));
    }
});


function GetPackingList(ID) {
    $.ajax({
        url: GetPackingListByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            if (response != null) {
                gMaxCapacity = response.MaxCapacity;
                DisplayPackingList(response.data,true);
            } 

        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonCategory: "#556ee6" });
        }
    });
    return false;
}
function DisplayPackingList(Packingdata, viewFlag) {
    $("#divPackingList").empty();
    let colorCode = "bg-secondary bg-gradient text-white";
    let tableContent = '';

    tableContent += `<div class="alert alert-success" role="alert">
                        We have found <strong>${Packingdata.length}</strong> pending Box(s) to Dispatch!
                    </div>`;

    tableContent += `<div class="table-responsive">
        <table class="table table-striped align-middle table-sm" id="tblSearchResult">
            <thead>
                <tr class="table-light">
                    ${viewFlag ? `<th><input type="checkbox" class="form-check-input bg-success" id="selectAllCheckbox" /></th>` : ``}
                    <th class="${colorCode}">Packing No</th>
                    <th class="${colorCode}">Packing Date</th>
                    <th class="${colorCode}">Box</th>
                    <th class="${colorCode}">Size</th>
                    <th class="${colorCode}">Color</th>
                    <th class="${colorCode}">No.of.Inner Box Count</th>   
                    <th class="${colorCode}">Total No.of.Plates</th> 
                    <th class="${colorCode}">Pcs/Box</th> 
                    <th class="${colorCode}">Allotted To</th> 
                    <th class="${colorCode}">Action</th> 
                </tr>
            </thead>
            <tbody>`;

    if (Packingdata.length > 0) {
        Packingdata.forEach((packingdata) => {
            tableContent += `
                <tr>
                    ${viewFlag ? `<td><input type="checkbox" class="rowCheckbox form-check-input bg-success" id="Selected_${packingdata.PackingID}"/></td>` : ``}
                    <td>${packingdata.PackingNo}</td>
                    <td>${packingdata.PackingDate}</td>
                    <td>${packingdata.BoxName}</td>
                    <td>${packingdata.SizeName}</td>
                    <td>${packingdata.ColorName}</td>
                    <td>${packingdata.BoxCount}</td>
                    <td>${packingdata.TotalQuantity}</td>
                    <td>${packingdata.PcsPerBox}</td>
                    <td>${packingdata.CompanyName}</td>
                    <td>
                        <ul class="list-unstyled hstack gap-1 mb-0">
                            <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                                <a href="javascript:void(0);" onclick="GetPackingByID(${packingdata.PackingID})" 
                                   class="btn btn-sm btn-soft-primary" data-bs-toggle="modal" data-bs-target="#divAddEditModal">
                                    <i class="mdi mdi-eye-outline"></i>
                                </a>
                            </li>
                        </ul>
                    </td>
                </tr>`;
        });
    } else {
        let colCount = viewFlag ? 11 : 10;
        tableContent += `<tr><td colspan="${colCount}" class="text-center">No Packing Data to Display</td></tr>`;
    }

    tableContent += `
            </tbody>
        </table>
    </div>`;

    $("#divPackingList").html(tableContent);
}
// Select All Checkbox Handler
$(document).on('change', '#selectAllCheckbox', function () {
    let isChecked = this.checked;
    $('.rowCheckbox').prop('checked', isChecked).trigger('change'); // trigger change to update array
});

// Individual Checkbox Handler
$(document).on('change', '.rowCheckbox', function () {
    let checkbox = $(this);
    let idAttr = checkbox.attr('id'); // e.g., "Selected_123"
    let packingID = parseInt(idAttr.replace("Selected_", ""), 10); // Extract numeric ID

    if (checkbox.is(':checked')) {
        if (!SelectedPackingArray.some(obj => obj.PackingID === packingID)) {
            SelectedPackingArray.push({ PackingID: packingID });
        }
    } else {
        // Remove object from array if unchecked
        SelectedPackingArray = SelectedPackingArray.filter(obj => obj.PackingID !== packingID);
    }

    // Update selectAll checkbox status
    let allChecked = $('.rowCheckbox').length === $('.rowCheckbox:checked').length;
    $('#selectAllCheckbox').prop('checked', allChecked);

    if (ENABLE_VERBOSE_Logging) console.log("Selected Packing IDs:", SelectedPackingArray);
});
function GetPackingByID(id) {
    if (ENABLE_VERBOSE_Logging) //console.log(id);
    $("#divInnerBoxes").empty();
    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            $("#divAddEdit").show();
            $("#divRecords").hide();

            var stockdata = response.data;

            // Display inner boxes regardless of view mode
            if (stockdata.PackingTrans && stockdata.PackingTrans.length > 0) {
                let sHeaderColor = "bg-secondary bg-gradient text-white";

                let innerBoxTableContent = `
                <div class="table-responsive">
                    <table class="table table-bordered align-middle table-sm w-100" id="tblInnerBoxes">
                        <thead>
                            <tr>
                                <th class="text-center ${sHeaderColor}">S.No</th>
                                <th class="text-center ${sHeaderColor}">Starting Laser No</th>
                                <th class="text-center ${sHeaderColor}">Ending Laser No</th>
                                <th class="text-center ${sHeaderColor}">No.of Plate</th>
                                <th class="text-center ${sHeaderColor}">Inner Box No</th>
                                  <th class="text-center ${sHeaderColor}">Color</th>
                                <th class="text-center ${sHeaderColor}">Size</th>
                            </tr>
                        </thead>
                        <tbody>
                `;

                stockdata.PackingTrans.forEach((entry, index) => {

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
                        <td class="text-center">${entry.ColorName}</td>
                        <td class="text-center">${entry.SizeName}</td>
                    </tr>
                    `;
                });
                    
                innerBoxTableContent += `
                        </tbody>
                    </table>
                </div>
                `;

                $("#divInnerBoxes").html(innerBoxTableContent);

            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}

$("#btnSave").on('click', function () {
    let isValid = true;
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    var NumberPlateDispatch = new Object();

    NumberPlateDispatch.NumberPlateDispatchID = 0;
    if (this.id == "btnUpdate" && $("#hdnNumberPlateDispatchID").val() > 0) {
        NumberPlateDispatch.NumberPlateDispatchID = $("#hdnNumberPlateDispatchID").val();
    }

    NumberPlateDispatch.sDispatchDate = $("#txtDispatchDate").val();
    NumberPlateDispatch.ModeofTransportID = $("#ddlModeOfTransport").val();
    if (NumberPlateDispatch.ModeofTransportID == 1) {
        NumberPlateDispatch.CourierID = $("#ddlCourier").val();
        NumberPlateDispatch.OwnVehicleDetails = null;
    }
    else {
        NumberPlateDispatch.CourierID = null;
        NumberPlateDispatch.OwnVehicleDetails = $("#txtVehicle").val();
    }
    NumberPlateDispatch.DocketNo = $("#txtDocketNo").val();
    NumberPlateDispatch.sDocketBookingDate = $("#txtDocketDate").val();
    NumberPlateDispatch.EmbossingStationID = $("#ddlEmbossingStation").val();

    // Validations
    if (!NumberPlateDispatch.sDispatchDate) {
        $('#txtDispatchDate').addClass('is-invalid');
        $('#txtDispatchDate').after('<div class="invalid-feedback">Please select Dispatch Date</div>');
        $('#txtDispatchDate').focus();
        return false;
    }
    if (!NumberPlateDispatch.DocketNo) {
        $('#txtDocketNo').addClass('is-invalid');
        $('#txtDocketNo').after('<div class="invalid-feedback">Please Enter Docket No</div>');
        $('#txtDocketNo').focus();
        return false;
    }
    if (!NumberPlateDispatch.sDocketBookingDate) {
        $('#txtDocketDate').addClass('is-invalid');
        $('#txtDocketDate').after('<div class="invalid-feedback">Please select Docket Date</div>');
        $('#txtDocketDate').focus();
        return false;
    }
    if (NumberPlateDispatch.ModeofTransportID==0) {
        $('#ddlModeOfTransport').addClass('is-invalid');
        $('#ddlModeOfTransport').after('<div class="invalid-feedback">Please select Mode of Transport</div>');
        $('#ddlModeOfTransport').focus();
        return false;
    }
    if (NumberPlateDispatch.ModeofTransportID == 1) {
        if (NumberPlateDispatch.CourierID == 0) {
            $('#ddlCourier').addClass('is-invalid');
            $('#ddlCourier').after('<div class="invalid-feedback">Please select Courier</div>');
            $('#ddlCourier').focus();
            return false;
        }
    }
    else if (NumberPlateDispatch.ModeofTransportID == 2) {
        if (NumberPlateDispatch.OwnVehicleDetails == 0) {
            $('#txtVehicle').addClass('is-invalid');
            $('#txtVehicle').after('<div class="invalid-feedback">Please Enter Vehicle No.</div>');
            $('#txtVehicle').focus();
            return false;
        }
    }
    

    if (SelectedPackingArray.length <= 0) {
        $.jGrowl("No NumberPlateDispatch Data Are Selected", { sticky: false, theme: 'warning', life: 3000 });
        return false;
    }

    NumberPlateDispatch.NumberPlateDispatchTrans = SelectedPackingArray;

    SaveandUpdateNumberPlateDispatch(NumberPlateDispatch);
    return false;
});
function SaveandUpdateNumberPlateDispatch(NumberPlateDispatch) {
    if (ENABLE_VERBOSE_Logging) //console.log(NumberPlateDispatch);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(NumberPlateDispatch),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);

            if (response.data.Item1) {
                if (NumberPlateDispatch.NumberPlateDispatchID == 0) {
                    Swal.fire({ title: response.data.Item2, text: "NumberPlateDispatch Successfully", icon: "success", confirmButtonColor: "#556ee6" });

                }
                else if (NumberPlateDispatch.NumberPlateDispatchID > 0)
                    Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                //EditData(response.data.Item2, true);
                $("#btnClose").click();
                $("#btnRefresh").click();
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
        language: {
            oPaginate: {
                sNext: '<i class="mdi mdi-chevron-right"></i>',
                sPrevious: '<i class="mdi mdi-chevron-left"></i>'
            }
        },
        "columns": [
            {
                data: null, // Serial number (S No.)
                render: function (data, type, row, meta) {
                    return meta.settings._iDisplayStart + meta.row + 1; // Display row number (S. No.)
                },
                bSortable: false,
                "width": "5%",
                "orderable": false
            },
            { "data": "DispatchNo", "orderable": true, "width": "10%" },
            { "data": "DispatchDate", "orderable": true },
            { "data": "ModeofTransportName", "orderable": true },
            { "data": "TransportDetails", "orderable": true },
            { "data": "DocketNo", "orderable": true },
            { "data": "DocketBookingDate", "orderable": true },
            { "data": "EmbossingStationName", "orderable": true },
            {
                "data": "StatusID",
                "render": function (data, type, row) {
                    return `<span class="${row.ColorCode}">${row.StatusName}</span>`;
                },
                "width": "5%",
                "className": "text-center",
                "orderable": false
            },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    let actionButtons = `<ul class="list-unstyled hstack gap-1 mb-0">`;

                    actionButtons += `
                             <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                                <a href="javascript:void(0);" onclick="Editdata(${row.NumberPlateDispatchID}, true)" class="btn btn-sm btn-soft-primary">
                                    <i class="mdi mdi-eye-outline"></i>
                                </a>
                            </li>`;
                    // Show Delete button only when StatusID == 1
                    if (row.StatusID === 1) {
                        actionButtons += `
                            <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
                                <a href="javascript:void(0);" onclick="DeleteData('${row.NumberPlateDispatchID}')" class="btn btn-sm btn-soft-danger">
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
function Editdata(ID) {

    $.ajax({
        url: GetNumberPlateDispatchByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            if (response != null) {
                var data = response.data
                $("#divCardTitle").html("<i class='fas fa-eye align-middle me-1'></i>View  Dispatch");
                $("#divAddEdit .card-body :input").attr("disabled", true);
                $("#btnCloseWindow,#btnClose").attr("disabled", false);
                $("#divRecords").hide();
                $("#divAddEdit").show();
                $("#btnSave").hide();
                $("#btnUpdate").hide();
                 
                $('#divVehicleDropdown').hide();
                $('#divVehicle').hide();

                $("#txtDispatchNo").val(data.DispatchNo);
                $("#txtDispatchDate").val(data.DispatchDate);
                $("#ddlModeOfTransport").val(data.ModeofTransportID).change();
                $("#ddlCourier").val(data.FK_CourierID).change();
                $("#txtVehicle").val(data.DispatchNo);
                $("#txtDocketNo").val(data.DocketNo);
                $("#txtDocketDate").val(data.DocketBookingDate);
                $("#ddlEmbossingStation").val(data.EmbossingStationID).change();

                DisplayPackingList(data.NumberPlateDispatchTrans,false);
               
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonCategory: "#556ee6" });
        }
    });
    return false;
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
            if (response.resultdata.Success && !response.resultdata.Error) {
                Swal.fire({
                    title: "Deleted!",
                    text: response.resultdata.Message,
                    icon: "success",
                    confirmButtonColor: "#556ee6"
                }).then(() => {
                });
                $("#btnRefresh").click();
            }
            else
                Swal.fire({ title: "Error", text: response.resultdata.Message, icon: "warning", confirmButtonColor: "#556ee6" });
        },
        error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
    return false;
}