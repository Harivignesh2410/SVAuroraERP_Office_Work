$(function () {
    pLoadingSetup(false);
    getRecordList();

    $("#btnSave").show();
    $("#btnUpdate").hide();
    if (!_CMActionAdd) {
        $("#btnAddNew").remove();
        $("#btnSave").remove();
    }
    if (!_CMActionUpdate) $("#btnUpdate").remove();
    pLoadingSetup(true);
});
$("#btnAddNew").on('click', function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Vehicle Plate Size Mapping");
    ClearFormFields();

    return false;
});
$('#divAddEditModal').on('shown.bs.modal', function () {
    $('#ddlVehiclePlateBackSize').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlVehiclePlateFrontSize').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlVehicleClass').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlPlateType').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlFuel').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlVehiclePlateColor').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlVehicleCategory').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlVehicleType').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlVehiclePlateType').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlVehiclePlateSize').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnVehiclePlateSizeID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#ddlPlateType").val(0);
    $("#txtDescription").val("");
    $("#ddlVehicleCategory").val(0).change();
    $("#ddlVehicleType").val(0).change();
    $("#ddlVehicleClass").val(0).change();
    $("#ddlFuel").val(0).change();
    $("#ddlVehiclePlateColor").val(0).change();
    $("#ddlVehiclePlateType").val(0).change();
    $("#ddlVehiclePlateSize").val(0).change();
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

    var VehiclePlateSizeMappingData = new Object();

    VehiclePlateSizeMappingData.VehiclePlateSizeMappingID = 0;
    if (this.id == "btnUpdate" && $("#hdnVehiclePlateSizeMappingID").val() > 0) VehiclePlateSizeMappingData.VehiclePlateSizeMappingID = $("#hdnVehiclePlateSizeMappingID").val();

    VehiclePlateSizeMappingData.VehicleCategoryID = $('#ddlVehicleCategory').val();
    VehiclePlateSizeMappingData.VehicleTypeID = $('#ddlVehicleType').val();
    VehiclePlateSizeMappingData.VehicleClassID = $('#ddlVehicleClass').val();
    VehiclePlateSizeMappingData.FuelID = $('#ddlFuel').val();
    VehiclePlateSizeMappingData.VehiclePlateColorID = $('#ddlVehiclePlateColor').val();
    VehiclePlateSizeMappingData.VehiclePlateTypeID = $('#ddlVehiclePlateType').val();
    VehiclePlateSizeMappingData.VehiclePlateSizeID = $('#ddlVehiclePlateSize').val();
    VehiclePlateSizeMappingData.Description = $('#txtDescription').val();

    if (!VehiclePlateSizeMappingData.VehicleCategoryID || VehiclePlateSizeMappingData.VehicleCategoryID === "0")
        return markInvalid("#ddlVehicleCategory", " Please Select Vehicle Category");
    if (!VehiclePlateSizeMappingData.VehicleTypeID || VehiclePlateSizeMappingData.VehicleTypeID === "0")
        return markInvalid("#ddlVehicleType", " Please Select Vehicle Type");
    if (!VehiclePlateSizeMappingData.FuelID || VehiclePlateSizeMappingData.FuelID === "0")
        return markInvalid("#ddlFuel", " Please Select Vehicle Fuel Type");
    if (!VehiclePlateSizeMappingData.VehicleClassID || VehiclePlateSizeMappingData.VehicleClassID === "0")
        return markInvalid("#ddlVehicleClass", " Please Select Vehicle class Type");
    if (!VehiclePlateSizeMappingData.VehiclePlateColorID || VehiclePlateSizeMappingData.VehiclePlateColorID === "0")
        return markInvalid("#ddlVehiclePlateColor", " Please Select Vehicle Plate Color");
    if (!VehiclePlateSizeMappingData.VehiclePlateTypeID || VehiclePlateSizeMappingData.VehiclePlateTypeID === "0")
        return markInvalid("#ddlVehiclePlateType", " Please Select Vehicle Plate Type");
    if (!VehiclePlateSizeMappingData.VehiclePlateSizeID || VehiclePlateSizeMappingData.VehiclePlateSizeID === "0")
        return markInvalid("#ddlVehiclePlateSize", " Please Select Vehicle Plate Size");

    if (!isValid) return;

    SaveandUpdate(VehiclePlateSizeMappingData);

    return false;
});
function SaveandUpdate(VehiclePlateSizeMappingData) {
    if (ENABLE_VERBOSE_Logging)

        $.ajax({
            url: SaveUpdateDataUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(VehiclePlateSizeMappingData),
            success: function (response) {
                if (ENABLE_VERBOSE_Logging) //console.log(response);

                    if (response.Success && !response.Error) {
                        if (VehiclePlateSizeMappingData.VehiclePlateSizeMappingID == 0)
                            Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                        else if (VehiclePlateSizeMappingData.VehiclePlateSizeMappingID > 0)
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
            { "data": "VehicleTypeName", "orderable": true, "width": "10%" },
            { "data": "VehicleCategoryName", "orderable": true, "width": "10%" },
            { "data": "FuelName", "orderable": true, "width": "10%" },
            { "data": "VehicleClassName", "orderable": true },
            { "data": "VehiclePlateColorName", "orderable": true },
            { "data": "VehiclePlateSizeName", "orderable": true },
            { "data": "Description", "orderable": true, "width": "15%" },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    return SetActionButtons(row.VehiclePlateSizeMappingID, _CMPermissions);
                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
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
                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Vehicle Plate Size Mapping");
            }
            else {
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Vehicle Plate Size Mapping");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }
            if (response != null && response.Value != null) {
                var result = response;
                if (!result.Error && result.Success && result.ID > 0) {
                    $('#divAddEditModal').modal('show');
                    var VehiclePlateSizeMappingData = response.Value;
                    $("#hdnVehiclePlateSizeMappingID").val(VehiclePlateSizeMappingData.VehiclePlateSizeMappingID);
                    $("#ddlVehicleCategory").val(VehiclePlateSizeMappingData.VehicleCategoryID).change();
                    $("#ddlVehicleType").val(VehiclePlateSizeMappingData.VehicleTypeID).change();
                    $("#ddlFuel").val(VehiclePlateSizeMappingData.FuelID).change();
                    $("#ddlVehiclePlateColor").val(VehiclePlateSizeMappingData.VehiclePlateColorID).change();
                    $("#ddlVehiclePlateType").val(VehiclePlateSizeMappingData.VehiclePlateTypeID).change();
                    $("#txtDescription").val(VehiclePlateSizeMappingData.Description);
                    $("#ddlVehicleClass").val(VehiclePlateSizeMappingData.VehicleClassID).change();
                    $("#ddlVehiclePlateSize").val(VehiclePlateSizeMappingData.VehiclePlateSizeID).change();
                    $("#divRecordLog").show();
                    $("#spnLastUpdatedBy").html("Last Updated By: " + VehiclePlateSizeMappingData.LastUpdatedByName);
                    $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(VehiclePlateSizeMappingData.LastUpdatedDate));
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