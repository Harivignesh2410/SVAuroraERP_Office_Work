$(function () {
    pLoadingSetup(false);
    getRecordList();
    getProcessTypeRecordList();
    $("#btnSave").show();
    $("#btnUpdate").hide();
    pLoadingSetup(true);
});

function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnProductionConfigurationID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#ddlComponentType").val(0).change();
    $("#ddlProcessType").val(0).change();
    //  $("#chkActive").prop("checked", true);

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

    return false;
}

$("#btnAddNew").on('click', function () {
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Input  Configuration");
    ClearFormFields();

    return false;
});
$("#btnAddNew2").on('click', function () {
    $("#divAddEditModal1.modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Output Production Configuration");
    ClearFormFields();

    return false;
});

$('#btnRefresh').on('click', function () {
    getRecordList();
    return false;
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
            url: ConfigurationListDataUrl,
            "type": "GET",
            "data": function (d) {
                // d.search.value = $('#tblrecordlist_filter input').val();  // Make sure the search value is passed
                // Pass additional parameters if needed
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
                    return meta.settings._iDisplayStart + meta.row + 1; // Display row number (S. No.)
                },
                orderable: false, // Disable sorting for this column},
                "width": "5%",
                "orderable": false
            },
            { "data": "ProcessTypeName", "orderable": true },
            { "data": "ComponentTypeName", "orderable": true },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    return SetModalAction(row.ProductionConfigurationID);
                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}

$('#output').on('click', function () {
    //console.log("Output");
    //getProcessTypeRecordList();    
});
function getProcessTypeRecordList() {
    // Check if DataTable has already been initialized
    if ($.fn.DataTable.isDataTable('#tblrecordlist1')) {
        $('#tblrecordlist1').DataTable().clear().destroy();  // Destroy previous instance
    }

    $('#tblrecordlist1').DataTable({
        "processing": true,
        "serverSide": true,
        "ordering": true,  // Enable sorting on columns
        "ajax": {
            url: ProcessTypeListDataUrl,
            "type": "GET",
            "data": function (d) {
                // d.search.value = $('#tblrecordlist_filter input').val();  // Make sure the search value is passed
                // Pass additional parameters if needed
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
                    return meta.settings._iDisplayStart + meta.row + 1; // Display row number (S. No.)
                },
                orderable: false, // Disable sorting for this column},
                "width": "5%",
                "orderable": false
            },
            { "data": "ProcessTypeName", "orderable": true, },
            { "data": "ProcessDescription", "orderable": true, },
            { "data": "ComponentTypeName", "orderable": true },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    return `
                            <ul class="list-unstyled hstack gap-1 mb-0">
                                <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                                    <a href="javascript:void(0);" onclick="EditProcessTypeData(${row.ProcessTypeID}, true)" class="btn btn-sm btn-soft-primary" data-bs-toggle="modal" data-bs-target="#divAddEditModal1">
                                        <i class="mdi mdi-eye-outline"></i>
                                    </a>
                                </li>
                                <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Edit">
                                    <a href="javascript:void(0);" onclick="EditProcessTypeData(${row.ProcessTypeID},false)"  class="btn btn-sm btn-soft-info" data-bs-toggle="modal" data-bs-target="#divAddEditModal1">
                                        <i class="mdi mdi-pencil-outline"></i>
                                    </a>
                                </li>                             
                            </ul>`;
                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}

$("#btnSave,#btnUpdate").on('click', function () {
    let isValid = true; // Flag to track overall validity

    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid'); // Remove previous invalid class

    // Collect data from input fields
    var InputData = new Object();

    InputData.ProductionConfigurationID = 0;
    if (this.id == "btnUpdate" && $("#hdnProductionConfigurationID").val() > 0) InputData.ProductionConfigurationID = $("#hdnProductionConfigurationID").val();

    InputData.ProcessTypeID = $('#ddlProcessType').val();
    InputData.ComponentTypeID = $('#ddlComponentType').val();
    // InputData.IsActive = $("#chkActive").is(':checked') ? true : false;

    if (!InputData.ComponentTypeID) {
        $('#ddlComponentType').addClass('is-invalid'); //Mark field as invalid
        $('#ddlComponentType').after('<div class="invalid-feedback">Please Select Component Type </div>');
        $('#ddlComponentType').focus(); isValid = false;
        return false;
    }
    if (!InputData.ProcessTypeID) {
        $('#ddlProcessType').addClass('is-invalid'); //Mark field as invalid
        $('#ddlProcessType').after('<div class="invalid-feedback">Please Select Process Type</div>');
        $('#ddlProcessType').focus(); isValid = false;
        return false;
    }
    // If validation fails, keep focus on the first invalid input
    if (!isValid) return;

    SaveandUpdateInputConfig(InputData);

    return false;
});

function SaveandUpdateInputConfig(InputData) {
    if (ENABLE_VERBOSE_Logging) //console.log(InputData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(InputData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.success && !response.isExists) {
                if (InputData.ProductionConfigurationID == 0)
                    Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                else if (InputData.ProductionConfigurationID > 0)
                    Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                $('#divAddEditModal').modal('hide');
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
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

    return false;
}

$('#divAddEditModal').on('shown.bs.modal', function () {
    $('#ddlComponentType').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlProcessType').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
});

function EditData(ID, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) //console.log(ID);
    ClearFormFields();

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
                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Input Configuration");
            }
            else {
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Input Configuration");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }
            var data = response.data;
            //console.log(data);
            $("#hdnProductionConfigurationID").val(data.ProductionConfigurationID);
            $("#ddlProcessType").val(data.ProcessTypeID).change();
            $("#ddlComponentType").val(data.ComponentTypeID).change();

            $("#divRecordLog1").show();
            $("#spnLastUpdatedBy1").html("Last Updated By: " + data.LastUpdatedByName);
            $("#spnLastUpdatedDate1").html("Date: " + ISTtoLocalTime(data.LastUpdatedDate));
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) //console.log(error);

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
        }, error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
    return false;
}


//##########################
//  OUTPUT CONFIGURATION
//##########################
$('#btnRefreshProcessType').on('click', function () {
    getProcessTypeRecordList();
    return false;
});
$('#divAddEditModal1').on('shown.bs.modal', function () {
    $('#ddlComponentType1').select2({ dropdownParent: $('#divAddEditModal1'), width: '100%' });
});
function EditProcessTypeData(ID, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) //console.log(ID);
    ClearFormFields();

    $.ajax({
        url: GetProcessTypeByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            if (ViewFlag) {
                $("#btnSave").hide();
                $("#btnUpdate").hide();

                $("#divAddEditModal1 .modal-body :input").attr("disabled", true);
                $("#divAddEditModal1 .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Output Configuration");
            }
            else {
                $("#divAddEditModal1 .modal-body :input").attr("disabled", false);
                $("#divAddEditModal1 .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Output Configuration");
                $("#btnSave").hide();
                $("#btnUpdate").show();

                $("#txtProcesstype").attr("disabled", true);
            }

            var data = response.data;
            //console.log(data);
            $("#hdnProcessTypeID").val(data.ProcessTypeID);
            $("#txtProcesstype").val(data.ProcessTypeName)
            $("#txtDescription").val(data.ProcessDescription)
            $("#ddlComponentType1").val(data.OutputComponentTypeID).change();

            $("#divRecordLog2").show();
            //$("#spnLastUpdatedBy1").html("Last Updated By: " + data.LastUpdatedByName);
            $("#spnLastUpdatedDate2").html("Date: " + ISTtoLocalTime(data.LastUpdatedDate));
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
$("#btnUpdate1").on('click', function () {
    let isValid = true; // Flag to track overall validity

    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid'); // Remove previous invalid class

    // Collect data from input fields
    var ProcessData = new Object();

    ProcessData.ProcessTypeID = 0;
    if (this.id == "btnUpdate1" && $("#hdnProcessTypeID").val() > 0)
        ProcessData.ProcessTypeID = $("#hdnProcessTypeID").val();

    ProcessData.ProcessDescription = $("#txtDescription").val();
    ProcessData.OutputComponentTypeID = $('#ddlComponentType1').val();

    if (!ProcessData.OutputComponentTypeID) {
        $('#ddlComponentType1').addClass('is-invalid'); //Mark field as invalid
        $('#ddlComponentType1').after('<div class="invalid-feedback">Please Select Component Type </div>');
        $('#ddlComponentType1').focus(); isValid = false;
        return false;
    }
    if (!ProcessData.ProcessTypeID) {
        $('#ddlProcessType').addClass('is-invalid'); //Mark field as invalid
        $('#ddlProcessType').after('<div class="invalid-feedback">Please Select Process Type</div>');
        $('#ddlProcessType').focus(); isValid = false;
        return false;
    }
    // If validation fails, keep focus on the first invalid input
    if (!isValid) return;


    UpdateDataOutputConfig(ProcessData);

    return false;
});
function UpdateDataOutputConfig(ProcessData) {
    if (ENABLE_VERBOSE_Logging) //console.log(ProcessData);

    $.ajax({
        url: UpdateDataOutputConfigDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(ProcessData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);

            if (response.success && !response.isExists) {
                Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                $('#divAddEditModal1').modal('hide');
                $("#btnRefreshProcessType").click();
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