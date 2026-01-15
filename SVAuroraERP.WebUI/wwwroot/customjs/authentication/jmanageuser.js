$(function () {
    //getRecordList();
    $("#btnSave").show();
    $("#btnUpdate").hide();
    GetApplicationList("ddlApplication", ApplicationListUrl, _TOKEN)
    loadUserTable('#tblinventoryrecordlist', 1);
});

$("#btnAddNew").on('click', function () {
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New User");
    ClearFormFields();

    $("#password").show();
    return false;
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnUserID").val(0);

    $("#txtFirstName").val("");
    $("#ddlApplication").val("0").change();
    $("#txtLastName").val("");
    $("#txtEmail").val("");
    $("#txtUserName").val("");
    $("#txtPassword").val("");
    $("#txtConfirmPassword").val("");
    $("#ddlUserRole").val(0).change();
    $("#ddlPageList").val(0).change();
    $("#chkActive").prop("checked", true);

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdateDate").empty();

    return false;
}
$('#btnRefresh').on('click', function () {
    loadUserTable('#tblinventoryrecordlist', 1);
    return false;
})
$('#ddlApplication').on('change', function () {
    var ApplicationID = parseInt($(this).val());

    $.ajax({
        url: GetRoleByApplicationIDUrl,
        type: 'GET',
        data: { Application: ApplicationID },
        async: false,
        success: function (response) {
            $("#ddlUserRole").empty();
            $("#ddlUserRole").append("<option value='0'>--Select--</option>");

            $.each(response.data, function (i, result) {
                $("#ddlUserRole").append("<option value='" + result.RoleID + "'>" + result.RoleName + "</option>");
            });

        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: "Error",
                text: xhr.responseText,
                icon: "error",
                confirmButtonColor: "#556ee6"
            });
        }
    });
    return false;
});

$(".CloseButton").click(function () {
    $("#divRecordList").show();
    $("#divAddEditView").hide();

    $("#divPageAccessList").empty();
    return false;
});

$('#ddlUserRole').on('change', function () {
    $('#ddlPageList').empty();
    var roleID = $(this).val();
    console.log(roleID);

    GetPageList(roleID, 0);
});
function GetPageList(roleID, selectedID) {
    if (roleID > 0) {
        $.ajax({
            url: ListPageControlURL,
            type: 'get',
            headers: { "RequestVerificationToken": _TOKEN },
            data: { RoleID: roleID },
            success: function (data) {
                $('#ddlPageList').empty();
                $("#ddlPageList").append("<option value='0'>--Select--</option>");
                $.each(data, function (i, result) {
                    $('#ddlPageList').append('<option value="' + result.Value + '">' + result.Text + '</option>');
                });

                $("#ddlPageList").val(selectedID);
            }
        });
    } else {
        $('#ddlPageList').empty();
        $('#ddlPageList').append('<option value="0">--No data--</option>');
    }
}
function getRecordList1() {
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
            { "data": "UserName", "orderable": true },
            { "data": "RoleName", "orderable": true },
            { "data": "FirstName", "orderable": true },
            { "data": "LastName", "orderable": true },
            { "data": "PageName", "orderable": true },
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
                                                            <a href="javascript:void(0);" onclick="EditData(${row.UserID}, true)" class="btn btn-sm btn-soft-primary" data-bs-toggle="modal" data-bs-target="#divAddEditModal">
                                                                <i class="mdi mdi-eye-outline"></i>
                                                            </a>
                                                        </li>
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Edit">
                                                            <a href="javascript:void(0);" onclick="EditData(${row.UserID},false)"  class="btn btn-sm btn-soft-info" data-bs-toggle="modal" data-bs-target="#divAddEditModal">
                                                                <i class="mdi mdi-pencil-outline"></i>
                                                            </a>
                                                        </li>
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
                                                            <a href="javascript:void(0);" onclick="DeleteData('${row.UserID}')" class="btn btn-sm btn-soft-danger">
                                                                <i class="mdi mdi-delete-outline"></i>
                                                            </a>
                                                        </li>
                                                         <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Change Password">
                                                            <a href="javascript:void(0);" onclick="ChangeUserPassword(${row.UserID})" class="btn btn-sm btn-soft-warning">
                                                                <i class="mdi mdi-key-change"></i>
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

$('a[href="#home-1"]').on('shown.bs.tab', function () {
    loadUserTable('#tblinventoryrecordlist', 1);
});

$('a[href="#profile-1"]').on('shown.bs.tab', function () {
    loadUserTable('#tblhsrprecordlist', 2);
});
function loadUserTable(tableId, applicationId) {

    if ($.fn.DataTable.isDataTable(tableId)) {
        $(tableId).DataTable().clear().destroy();
    }

    $(tableId).DataTable({
        processing: true,
        serverSide: true,
        ordering: true,
        pageLength: 10,
        ajax: {
            url: ListDataUrl,
            type: "POST",
            headers: { "RequestVerificationToken": _TOKEN },
            data: function (d) {
                return {
                    Draw: d.draw,
                    Start: d.start,
                    Length: d.length,
                    SearchValue: d.search.value,
                    SortColumn: d.columns[d.order[0].column].data,
                    SortDirection: d.order[0].dir,
                    ApplicationID: applicationId
                };
            },
            beforeSend: function () {
                $('body').append(`
                    <div id="dt-loader" class="skote-loader">
                        <div class="spinner-border text-primary" role="status">
                            <span class="sr-only">Loading...</span>
                        </div>
                    </div>
                `);
            },
            complete: function () {
                $('#dt-loader').remove();
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
                    return meta.row + 1; // Display row number (S. No.)
                },
                orderable: false, // Disable sorting for this column},
                "width": "5%",
                "orderable": false
            },
            { "data": "UserName", "orderable": true },
            { "data": "RoleName", "orderable": true },
            { "data": "FirstName", "orderable": true },
            { "data": "LastName", "orderable": true },
            { "data": "PageName", "orderable": true },
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
                                                            <a href="javascript:void(0);" onclick="EditData(${row.UserID}, true)" class="btn btn-sm btn-soft-primary" data-bs-toggle="modal" data-bs-target="#divAddEditModal">
                                                                <i class="mdi mdi-eye-outline"></i>
                                                            </a>
                                                        </li>
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Edit">
                                                            <a href="javascript:void(0);" onclick="EditData(${row.UserID},false)"  class="btn btn-sm btn-soft-info" data-bs-toggle="modal" data-bs-target="#divAddEditModal">
                                                                <i class="mdi mdi-pencil-outline"></i>
                                                            </a>
                                                        </li>
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
                                                            <a href="javascript:void(0);" onclick="DeleteData('${row.UserID}')" class="btn btn-sm btn-soft-danger">
                                                                <i class="mdi mdi-delete-outline"></i>
                                                            </a>
                                                        </li>
                                                         <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Change Password">
                                                            <a href="javascript:void(0);" onclick="ChangeUserPassword(${row.UserID})" class="btn btn-sm btn-soft-warning">
                                                                <i class="mdi mdi-key-change"></i>
                                                            </a>
                                                        </li>
                                                    </ul>`;
                },
                "width": "10%",
                "orderable": false
            },
        ]
    });
}

$("#btnSave,#btnUpdate").on('click', function () {
    let isValid = true;
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    var userData = new Object();
    userData.UserID = 0;
    if (this.id == "btnUpdate" && $("#hdnUserID").val() > 0) userData.UserID = $("#hdnUserID").val();
    userData.FirstName = $("#txtFirstName").val().trim();
    userData.LastName = $("#txtLastName").val();
    userData.Email = $("#txtEmail").val();
    userData.UserName = $("#txtUserName").val();
    userData.PasswordHash = $("#txtPassword").val();
    userData.ConfirmPassword = $("#txtConfirmPassword").val();
    userData.RoleID = $("#ddlUserRole").val();
    userData.LandingPageID = $("#ddlPageList").val();
    userData.IsActive = $("#chkStatus").is(':checked') ? true : false;

    if (!userData.FirstName) {
        $('#txtFirstName').addClass('is-invalid'); //Mark field as invalid
        $('#txtFirstName').after('<div class="invalid-feedback">Please enter First Name</div>');
        $('#txtFirstName').focus(); isValid = false;
        $.jGrowl("Please enter Fist Name", { sticky: false, theme: 'warning', life: jGrowlLife });
        return false;
    }
    //if (!userData.LastName) {
    //    $('#txtLastName').addClass('is-invalid'); //Mark field as invalid
    //    $('#txtLastName').after('<div class="invalid-feedback">Please enter Last Name</div>');
    //    $('#txtLastName').focus(); isValid = false;
    //    $.jGrowl("Please enter Last Name", { sticky: false, theme: 'warning', life: jGrowlLife });
    //    return false;
    //}

    /*
    if (!userData.Email) {
        $('#txtEmail').addClass('is-invalid'); //Mark field as invalid
        $('#txtEmail').after('<div class="invalid-feedback">Please enter Email</div>');
        $('#txtEmail').focus(); isValid = false;
        $.jGrowl("Please enter Role Name", { sticky: false, theme: 'warning', life: jGrowlLife });
        return false;
    }*/

    if (!userData.UserName) {
        $('#txtUserName').addClass('is-invalid'); //Mark field as invalid
        $('#txtUserName').after('<div class="invalid-feedback">Please enter User Name</div>');
        $('#txtUserName').focus(); isValid = false;
        $.jGrowl("Please enter Username Name", { sticky: false, theme: 'warning', life: jGrowlLife });
        return false;
    }

    if (userData.UserID == 0) {
        if (!userData.PasswordHash) {
            $('#txtPassword').addClass('is-invalid'); //Mark field as invalid
            $('#txtPassword').after('<div class="invalid-feedback">Please enter Password</div>');
            $('#txtPassword').focus(); isValid = false;
            $.jGrowl("Please enter UserPassword", { sticky: false, theme: 'warning', life: jGrowlLife });
            return false;
        }
        if (!userData.ConfirmPassword) {
            $('#txtConfirmPassword').addClass('is-invalid'); //Mark field as invalid
            $('#txtConfirmPassword').after('<div class="invalid-feedback">Please enter Confirm Password</div>');
            $('#txtConfirmPassword').focus(); isValid = false;
            $.jGrowl("Please Confirm Password", { sticky: false, theme: 'warning', life: jGrowlLife });
            return false;
        }
    }

    if (!userData.RoleID) {
        $('#ddlUserRole').addClass('is-invalid'); //Mark field as invalid
        $('#ddlUserRole').after('<div class="invalid-feedback">Please Select Role</div>');
        $('#ddlUserRole').focus(); isValid = false;
        $.jGrowl("Please select Role", { sticky: false, theme: 'warning', life: jGrowlLife });
        return false;
    }
    if (!userData.LandingPageID) {
        $('#ddlPageList').addClass('is-invalid'); //Mark field as invalid
        $('#ddlPageList').after('<div class="invalid-feedback">Please Select Landing Page</div>');
        $('#ddlPageList').focus(); isValid = false;
        $.jGrowl("Please Select Landing Page", { sticky: false, theme: 'warning', life: jGrowlLife });
        return false;
    }

    if (!isValid) return;
    SaveandUpdateUser(userData);
    return false;
});
function SaveandUpdateUser(userData) {
    $.ajax({
        url: SaveUpdateDataUrl,
        type: "POST",
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: "application/json",
        data: JSON.stringify(userData),
        success: function (data) {
            console.log(data);
            if (ENABLE_VERBOSE_Logging) console.log(data);
            if (data.success) {
                if (userData.UserID == 0)
                    Swal.fire({ title: "Saved!", text: data.message, icon: "success", confirmButtonColor: "#556ee6" });
                else if (userData.UserID > 0)
                    Swal.fire({ title: "Updated!", text: data.message, icon: "success", confirmButtonColor: "#556ee6" });

                $('#divAddEditModal').modal('hide');
                $("#btnRefresh").click();
            }
            else if (!data.success)
                Swal.fire({ title: "Warning!", text: data.message, icon: "warning", confirmButtonColor: "#556ee6" });

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
function EditData(id, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) console.log(id);

    $.ajax({
        url: GetByIDDataUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { UserID: id },
        success: function (response) {
            console.log(response);
            ClearFormFields();

            if (ViewFlag) {
                $("#btnSave").hide();
                $("#btnUpdate").hide();

                $("#divAddEditModal .modal-body :input").attr("disabled", true);
                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View User");
            }
            else {
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit New User");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }

            var roledata = response.data;

            $("#password").hide();

            $("#hdnUserID").val(roledata.UserID);
            
            $("#txtFirstName").val(roledata.FirstName);
            $("#txtLastName").val(roledata.LastName);
            $("#txtEmail").val(roledata.Email);
            $("#txtUserName").val(roledata.UserName);
            $("#txtPassword").val(roledata.PasswordHash);
            $("#ddlApplication").val(roledata.ApplicationID).change();
            $("#ddlUserRole").val(roledata.RoleID).change();
            GetPageList(roledata.RoleID, roledata.LandingPageID);
            //$("#ddlPageList").val(roledata.LandingPageID);
            $("#chkActive").prop('checked', roledata.IsActive);

            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + roledata.LastUpdatedByName);
            $("#spnLastUpdateDate").html("Date: " + ISTtoLocalTime(roledata.LastUpdateDateIST));
        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}

$('#divAddEditModal').on('shown.bs.modal', function () {
    $('#ddlUserRole').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlPageList').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlApplication').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
});

// Change Password functionality
var _changePasswordUserID = 0;

function ChangeUserPassword(userID) {
    _changePasswordUserID = userID;
    $("#txtNewPassword").val("");
    $("#txtConfirmNewPassword").val("");
    $("#divChangePasswordModal").modal("show");
    return false;
}

$("#btnChangePassword").on('click', function () {
    var newPassword = $("#txtNewPassword").val().trim();
    var confirmPassword = $("#txtConfirmNewPassword").val().trim();

    if (!newPassword) {
        markInvalid("#txtNewPassword", "Please enter New Password");
        return false;
    }

    if (!confirmPassword) {
        markInvalid("#txtConfirmNewPassword", "Please enter Confirm Password");
        return false;
    }

    if (newPassword !== confirmPassword) {
        markInvalid("#txtConfirmNewPassword", "New Password and Confirm Password do not match");
        return false;
    }

    $('.form-control').removeClass('is-invalid');

    $.ajax({
        url: ChangePasswordUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify({ UserID: _changePasswordUserID, NewPassword: newPassword }),
        success: function (response) {
            if (response && response.result) {
                var result = response.result;
                if (result.Success && !result.Error) {
                    Swal.fire({
                        title: "Success!",
                        text: "Password changed successfully!",
                        icon: "success"
                    }).then(() => {
                        $("#divChangePasswordModal").modal('hide');
                        $("#btnRefresh").click();
                    });
                } else {
                    Swal.fire({ title: "Error", text: result.Message || "Failed to change password", icon: "error", confirmButtonColor: "#556ee6" });
                }
            } else {
                Swal.fire({ title: "Error", text: "An error occurred", icon: "error", confirmButtonColor: "#556ee6" });
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText || error, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
    return false;
});

