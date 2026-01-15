$(function () {
    pLoadingSetup(false);
    getRecordList();
    $("#btnSave").show();
    $("#btnUpdate").hide();
    GetApplicationList("ddlApplication", ApplicationListUrl, _TOKEN)
    pLoadingSetup(true);
});

$("#btnAddNew").on('click', function () {
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Role");
    ClearFormFields();

    return false;
});
$('#divAddEditModal').on('shown.bs.modal', function () {
    $('#ddlApplication').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnRoleID").val(0);

    $("#txtRoleName").val("");
    $("#ddlApplication").val("0").change();
    $("#txtDescription").val("");
    $("#chkActive").prop("checked", true);
    $("#divchkSelectAll").hide();

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdateDate").empty();

    $("#divRoleModule").empty();
    $('#chkSelectAll').prop('checked', false);

    //GetRoleModule();
    return false;
}
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
            url: ListDataURL,
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
                    return meta.row + 1; // Display row number (S. No.)
                },
                orderable: false, // Disable sorting for this column},
                "width": "5%",
                "orderable": false
            },
            { "data": "ApplicationName", "orderable": true },
            { "data": "RoleName", "orderable": true },
            { "data": "Description", "orderable": true },
            {
                "data": "IsActive",
                "render": function (data, type, row) {
                    if (data) {
                        return '<span class="badge bg-success">Active</span>';
                    } else {
                        return '<span class="badge bg-danger">Inactive</span>';
                    }
                },
                "width": "10%",
                "orderable": false
            },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    return `
                                                    <ul class="list-unstyled hstack gap-1 mb-0">
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                                                            <a href="javascript:void(0);" onclick="EditData(${row.RoleID}, true)" class="btn btn-sm btn-soft-primary" data-bs-toggle="modal" data-bs-target="#divAddEditModal">
                                                                <i class="mdi mdi-eye-outline"></i>
                                                            </a>
                                                        </li>
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Edit">
                                                            <a href="javascript:void(0);" onclick="EditData(${row.RoleID},false)"  class="btn btn-sm btn-soft-info" data-bs-toggle="modal" data-bs-target="#divAddEditModal">
                                                                <i class="mdi mdi-pencil-outline"></i>
                                                            </a>
                                                        </li>
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
                                                            <a href="javascript:void(0);" onclick="DeleteData('${row.RoleID}')" class="btn btn-sm btn-soft-danger">
                                                                <i class="mdi mdi-delete-outline"></i>
                                                            </a>
                                                        </li>
                                                    </ul>`;
                },
                "width": "10%",
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
    var roleData = new Object();

    roleData.RoleID = 0;
    if (this.id == "btnUpdate" && $("#hdnRoleID").val() > 0) roleData.RoleID = $("#hdnRoleID").val();

    roleData.ApplicationID = $('#ddlApplication').val();
    roleData.RoleName = $('#txtRoleName').val();
    roleData.Description = $('#txtDescription').val();
    roleData.IsActive = $("#chkActive").is(':checked') ? true : false;

    //Role Name
    if (!roleData.RoleName) {
        $('#txtRoleName').addClass('is-invalid'); //Mark field as invalid
        $('#txtRoleName').after('<div class="invalid-feedback">Please enter Role Name</div>');
        $('#txtRoleName').focus(); isValid = false;
        //$.jGrowl("Please enter Role Name", { sticky: false, theme: 'warning', life: jGrowlLife });
        return false;
    }

    let selectedModuleIDs = []; // Initialize an array to hold ModuleIDs

    // Find all checked checkboxes with IDs starting with 'chkModule_'
    $('input[type="checkbox"]:checked[id^="chkModule_"]').each(function () {
        // Extract the ModuleID by splitting the ID and take the second part        
        selectedModuleIDs.push(parseInt($(this).val())); // Add the ModuleID to the array
    });

    if (selectedModuleIDs.length == 0) {
        $.jGrowl("Please select atleast one Module ", { sticky: false, theme: 'warning', life: jGrowlLife });
        return false;
    }

    roleData.RoleModuleIDs = selectedModuleIDs;

    // If validation fails, keep focus on the first invalid input
    if (!isValid) return;

    SaveandUpdateRole(roleData);

    return false;
});

function SaveandUpdateRole(roleData) {
    if (ENABLE_VERBOSE_Logging) console.log(roleData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(roleData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.success && !response.isExists) {
                if (roleData.RoleID == 0)
                    Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                else if (roleData.RoleID > 0)
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
        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

    return false;
}

function EditData(id, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) console.log(id);
    ClearFormFields();

    $.ajax({
        url: GetByIDDataUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            if (ViewFlag) {
                $("#btnSave").hide();
                $("#btnUpdate").hide();

                $("#divAddEditModal .modal-body :input").attr("disabled", true);
                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Role");
            }
            else {
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit New Role");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }
            var roledata = response.data;            
            $("#hdnRoleID").val(roledata.RoleID);
            $('#ddlApplication').val(roledata.ApplicationID).change();
            $("#txtRoleName").val(roledata.RoleName);
            $("#txtDescription").val(roledata.Description);
            $("#chkActive").prop('checked', roledata.IsActive);

            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + roledata.LastUpdatedByName);
            $("#spnLastUpdateDate").html("Date: " + ISTtoLocalTime(roledata.LastUpdateDateIST));

            if (roledata.RoleModuleIDs != null) {
                // Split RoleModuleIDs into an array
                var enabledModules = roledata.RoleModuleIDs.split(',');
                // Enable checkboxes with name="chkModule" based on RoleModuleIDs
                $('input[name="chkModule"]').each(function () {
                    if (enabledModules.includes($(this).val())) {
                        $(this).prop('checked', true);
                    }
                });
            }
        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}

function DeleteData(id) {
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


$('#ddlApplication').on('change', function () {
    var ApplicationID = $(this).val();
    if (ApplicationID != 0)
        GetRoleModule(ApplicationID);
});
function GetRoleModule(ID) {
    $("#divRoleModule").empty();

    $.ajax({
        url: GetRoleModuleUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        async: false, //Wait for the action to get complete
        success: function (response) {
            if (response.success) {
                $("#divchkSelectAll").show();
                var RoleModuleList = "";

                $.each(response.data, function (index, rolemodule) {
                    RoleModuleList += "<div class='form-check-inline form-check-info mr-4'>";
                    RoleModuleList += " <input type='checkbox' name='chkModule' class='form-check-input' id='chkModule_" + rolemodule.ModuleID + "' value='" + rolemodule.ModuleID + "' />";
                    RoleModuleList += " <label class='custom-control-label' for='chkModule_" + rolemodule.ModuleID + "'> " + rolemodule.ModuleName + "</label>";
                    RoleModuleList += "</div>";
                });

                $("#divRoleModule").html(RoleModuleList);
            }
            else
                $("#divRoleModule").empty();

        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}

// Select All Checkbox
$('#chkSelectAll').on('change', function () {
    // Check or Uncheck all checkboxes based on chkSelectAll
    $('input[name="chkModule"]').prop('checked', this.checked);
});

// Individual Checkboxes Behavior
$('input[name="chkModule"]').on('change', function () {
    // If all individual checkboxes are checked, check chkSelectAll
    // If one or more are unchecked, uncheck chkSelectAll
    $('#chkSelectAll').prop('checked', $('input[name="chkModule"]:checked').length === $('input[name="chkModule"]').length);
});