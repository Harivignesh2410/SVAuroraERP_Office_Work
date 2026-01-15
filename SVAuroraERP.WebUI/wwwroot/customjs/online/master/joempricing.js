$(function () {
    pLoadingSetup(false);
    getRecordList();

    $("#btnSave").show();
    $("#btnUpdate").hide();
    if (!_CMActionAdd) {
        $("#btnAddNew").remove();
        $("#btnSave").remove();
    }
    GetOEMList("ddlOEMList", OEMListUrl, _TOKEN);

    if (!_CMActionUpdate) $("#btnUpdate").remove();
    pLoadingSetup(true);
});
$("#btnAddNew").on('click', function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New OEM Pricing");
    ClearFormFields();

    return false;
});
$('#divAddEditModal').on('shown.bs.modal', function () {
    $('#ddlOEMList').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlPartnumberList').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlVehiclePlazeSizeFrontList').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlVehiclePlazeSizeRearList').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });

});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnOEMPricingID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#ddlOEMList").val(0);
    $("#ddlPartnumberList").val(0);
    $("#ddlVehiclePlazeSizeFrontList").val(0);
    $("#ddlVehiclePlazeSizeRearList").val(0);


    $("#txtSnaplock").val("");
    $("#txtRivets").val("");
    $("#txRate").val("");
    $("#txtCourierCharges").val("");
    $("#txtTotal").val("");


    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

    return false;
}
$('#btnRefresh').on('click', function () {
    getRecordList();
    return false;
});

$('#txRate, #txtCourierCharges').on('keyup change input', function () {
    const rate = parseFloat($('#txRate').val()) || 0;
    const courier = parseFloat($('#txtCourierCharges').val()) || 0;
    const total = rate + courier;

    $('#txtTotal').val(total.toFixed(2));
});
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
            headers: { "RequestVerificationToken": _TOKEN },
            "type": "POST",
            data: function (d) {
                return {
                    Draw: d.draw,
                    Start: d.start,
                    Length: d.length,
                    SearchValue: d.search.value,
                    SortColumn: d.columns[d.order[0].column].data,
                    SortDirection: d.order[0].dir
                };
            },
            processData: true, // Important for FormData            
            beforeSend: function () {
                // Show loader
                $('body').append(`
                    <div id="dt-loader" class="skote-loader">
                        <div class="spinner-border text-primary" role="status">
                            <span class="sr-only">Loading...</span>
                        </div>
                    </div>
                `);
            },
            complete: function () {
                // Hide loader
                $('#dt-loader').remove();
            }
        },
        language: { oPaginate: { sNext: '<i class="mdi mdi-chevron-right"></i>', sPrevious: '<i class="mdi mdi-chevron-left"></i>' } },
        "columns": [
            {
                data: null, // Serial number (S No.)
                render: function (data, type, row, meta) {
                    return meta.settings._iDisplayStart + meta.row + 1; // Display row number (S. No.)
                },
                orderable: false, // Disable sorting for this column},
                "width": "5%",
                "orderable": false
            },
            { "data": "OEMName", "orderable": true, "width": "15%" },
            { "data": "PartNumber", "orderable": true, "width": "15%" },
            { "data": "VehiclePlateSizeNameFront", "orderable": true, "width": "10%" },
            { "data": "VehiclePlateSizeNameRear", "orderable": true },
            {
                "data": "Rate", "orderable": true, "width": "10%", "className": "text-end",
                "render": function (data, type, row) {
                    return `<i class="bx bx-rupee text-primary font-size-16"></i>${row.Rate.toFixed(2)}`
                },
            },
            {
                "data": "CourierCharges", "orderable": true, "width": "10%", "className": "text-end",
                "render": function (data, type, row) {
                    return `<i class="bx bx-rupee text-primary font-size-16"></i>${row.CourierCharges.toFixed(2)}`
                },
            },
            {
                "data": "TotalAmount", "orderable": true, "width": "10%", "className": "text-end",
                "render": function (data, type, row) {
                    return `<i class="bx bx-rupee text-primary font-size-16"></i>${row.TotalAmount.toFixed(2)}`
                },
            },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    return SetActionButtons(row.OEMPricingID, _CMPermissions);
                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}

$("#btnSave,#btnUpdate").on('click', function () {
    if (this.id == "btnSave") {
        if (!_CMActionAdd) {
            $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
            return false;
        }
    }
    else if (this.id == "btnUpdate") {
        if (!_CMActionUpdate) {
            $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
            return false;
        }
    }
    let isValid = true;
    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    var OEMPricingData = new Object();

    OEMPricingData.OEMPricingID = 0;
    if (this.id == "btnUpdate" && $("#hdnOEMPricingID").val() > 0) OEMPricingData.OEMPricingID = $("#hdnOEMPricingID").val();
    OEMPricingData.HSRPPartNumberID = $('#ddlPartnumberList').val();
    OEMPricingData.VehiclePlateSizeFrontID = $('#ddlVehiclePlazeSizeFrontList').val();
    OEMPricingData.VehiclePlateSizeRearID = $('#ddlVehiclePlazeSizeRearList').val();
    OEMPricingData.SnapLock = $('#txtSnaplock').val();
    OEMPricingData.Rivets = $('#txtRivets').val();
    OEMPricingData.Rate = parseFloat($('#txRate').val()) || 0;
    OEMPricingData.CourierCharges = parseFloat($('#txtCourierCharges').val()) || 0;
    OEMPricingData.TotalAmount = parseFloat($('#txtTotal').val()) || 0;


    if (!$("#ddlOEMList").val() || $("#ddlOEMList").val() === "0") return markInvalid("#ddlOEMList", "Please select OEM");
    if (!OEMPricingData.HSRPPartNumberID || OEMPricingData.HSRPPartNumberID === "0") return markInvalid("#ddlPartnumberList", " Please select Part Number");
    if (!OEMPricingData.VehiclePlateSizeFrontID || OEMPricingData.VehiclePlateSizeFrontID === "0") return markInvalid("#ddlVehiclePlazeSizeFrontList", "Please Select Vehicle Plate Front");
    if (!OEMPricingData.VehiclePlateSizeRearID || OEMPricingData.VehiclePlateSizeRearID === "0") return markInvalid("#ddlVehiclePlazeSizeRearList", "Please Select Vehicle Plate Rear");

    if (!OEMPricingData.Rate) return markInvalid("#txRate", "Please enter Rate");
    if (!isValid) return;

    SaveandUpdate(OEMPricingData);

    return false;
});
function SaveandUpdate(OEMPricingData) {
    if (ENABLE_VERBOSE_Logging)

        $.ajax({
            url: SaveUpdateDataUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(OEMPricingData),
            success: function (response) {
                if (ENABLE_VERBOSE_Logging) //console.log(response);

                    if (response.Success && !response.Error) {
                        if (OEMPricingData.OEMPricingID == 0)
                            Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                        else if (OEMPricingData.OEMPricingID > 0)
                            Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                        $('#divAddEditModal').modal('hide');
                        $("#btnRefresh").click();
                    }
                    else if (!response.Success && response.Error) {
                        Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
                    }
                    else if (!response.Success && !response.Error) {
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
function EditData(ID, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) console.log(ID);
    ClearFormFields();
    if ((!_CMActionView && ViewFlag) || (!_CMActionUpdate && !ViewFlag)) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            if (ViewFlag) {
                $("#btnSave").hide();
                $("#btnUpdate").hide();

                $("#divAddEditModal .modal-body :input").attr("disabled", true);
                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View OEMPricing");
            }
            else {
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit OEMPricing");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }
            if (response != null && response.Value != null) {
                var result = response;
                if (!result.Error && result.Success && result.ID > 0) {
                    $('#divAddEditModal').modal('show');
                    var OEMPricingData = response.Value;
                    $("#hdnOEMPricingID").val(OEMPricingData.OEMPricingID);

                    $("#ddlOEMList").val(OEMPricingData.OEMID).change();
                    $("#ddlPartnumberList").val(OEMPricingData.HSRPPartNumberID).change();
                    $("#ddlVehiclePlazeSizeFrontList").val(OEMPricingData.VehiclePlateSizeFrontID).change();
                    $("#ddlVehiclePlazeSizeRearList").val(OEMPricingData.VehiclePlateSizeRearID).change();


                    $("#txtSnaplock").val(OEMPricingData.SnapLock);
                    $("#txtRivets").val(OEMPricingData.Rivets);
                    $("#txRate").val(OEMPricingData.Rate);
                    $("#txtCourierCharges").val(OEMPricingData.CourierCharges);
                    $("#txtTotal").val(OEMPricingData.TotalAmount);


                    $("#divRecordLog").show();
                    $("#spnLastUpdatedBy").html("Last Updated By: " + OEMPricingData.LastUpdatedByName);
                    $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(OEMPricingData.LastUpdatedDate));
                }
                else
                    Swal.fire({ title: "Error", text: result.Message, icon: "warning", confirmButtonColor: "#556ee6" });
            }
            else
                Swal.fire({ title: "Error", text: "Something went wrong!", icon: "warning", confirmButtonColor: "#556ee6" });
        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

}

function DeleteData(id) {
    if (ENABLE_VERBOSE_Logging) //console.log(id);
        if (!_CMActionDelete) {
            $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
            return false;
        }
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        customClass: {
            confirmButton: "btn btn-success mt-2",
            cancelButton: "btn btn-danger ms-2 mt-2"
        },
        buttonsStyling: false
    }).then(function (result) {
        if (result.value) {
            ConfirmDelete(id, DeleteDataUrl, _TOKEN, DeleteSuccessMessage, DeleteErrorMessage)
                .then(function (deleted) {
                    if (deleted) {
                        getRecordList(); // Refresh list or table
                    }
                });
        } else if (result.dismiss === Swal.DismissReason.cancel) {
            Swal.fire({
                title: "Cancelled",
                text: "Your data is safe :)",
                icon: "error"
            });
        }
    });
    return false;
}

$('#ddlOEMList').on('change', function () {

    const oemID = $(this).val();

    // Clear Part Number list
    $("#ddlPartnumberList")
        .empty()
        .append("<option value='0'>-- Select Part Number --</option>")
        .trigger("change");

    if (oemID && oemID !== "0") {
        GetPartNumberByOEM("ddlPartnumberList", PartNumberByOEMUrl, _TOKEN, oemID);
    }
});
