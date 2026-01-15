var appOrderArray = [];
//var ID = $("#hdnUserID").val();
$(function () {
    pLoadingSetup(false);
    GetAppLauncherByUserID();
    GetAppLauncherByRoleID();
    GetUserProfile();
    EditData();
    pLoadingSetup(true);
});

function InitializeDragandDrag() {
    var drake = dragula([document.getElementById("divAppLauncher"), document.getElementById("divAvailablePages")]);

    // Attach event to dragend
    drake.on('dragend', function () {
        saveAppLauncherOrder(); // Save the order after drag ends
        togglePlaceholders();
    });
}



function saveAppLauncherOrder() {
    appOrderArray = [];
    $("#divAppLauncher .col-lg-2").each(function (index) {
        var PageControlID = $(this).data("id").replace("add_", ""); // Extract ID from data-id
        appOrderArray.push({ OrdinalNo: index + 1, PageControlID: PageControlID });
    });

    //console.log("App Launcher Order:", appOrderArray);
}


function togglePlaceholders() {
    // Check if upcoming-task is empty
    if ($('#divAppLauncher .col-lg-2').length === 0) {
        $('#divAppLauncher .placeholder').removeClass('d-none').addClass('d-block');
        $('#divAppLauncher').css('background-color', 'transparent');  // Remove any background color
    } else {
        $('#divAppLauncher .placeholder').removeClass('d-block').addClass('d-none');
        $('#divAppLauncher').css('background-color', '');  // Reset to default background
    }

    // Check if complete-task is empty
    if ($('#divAvailablePages .col-lg-2').length === 0) {
        $('#divAvailablePages .placeholder').removeClass('d-none').addClass('d-block');
        $('#divAvailablePages').css('background-color', 'transparent');  // Remove any background color
    } else {
        $('#divAvailablePages .placeholder').removeClass('d-block').addClass('d-none');
        $('#divAvailablePages').css('background-color', '');  // Reset to default background
    }
}

function GetAppLauncherByRoleID() {
    $("#divAvailablePages").empty();
    $.ajax({
        url: GetAppLauncherByRoleIDUrl,
        type: 'GET',
        success: function (response) {
            if (response != null && response.data != null) {

                // Header details
                var headerDetails = `
				     <div class="placeholder col-12 text-center d-none bg-light">
								No items available. Drag items here
                     </div>.
				     `;

                response.data.forEach((item, index) => {
                    headerDetails += `
                         <div class="col-lg-2" data-id="add_${item.PageControlID}">
                             <a class="text-body" href="#">
                                   <div class="card">
                                        <div class="card-body p-2 text-center">
                                            <img src="${item.PageIcon}" alt="" class="avatar-sm">
                                             <h5 class="mt-4 mb-2 font-size-13">${item.PageName}</h5>
                                             <p class="mb-0 text-muted">${item.MenuDisplayName}</p>
                                         </div>
                                   </div>
                              </a>
                         </div>`;
                });

                // Append both sections to the container
                $("#divAvailablePages").append(headerDetails);

                InitializeDragandDrag();
            } else {

                $("#divAvailablePages").empty();
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
$("#btnSave").on('click', function () {

    SaveAppLauncher(appOrderArray);

    return false;
});
function SaveAppLauncher(appOrderArray) {
    if (ENABLE_VERBOSE_Logging) console.log(appOrderArray);

    $.ajax({
        url: SaveAppLauncherUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(appOrderArray),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.success && !response.isExists) {
                Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

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
function GetAppLauncherByUserID() {
    $("#divAppLauncher").empty();
    $.ajax({
        url: GetAppLauncherByUserIDUrl,
        type: 'GET',
        success: function (response) {
            if (response != null && response.data != null) {
                // Header details
                var headerDetails = `
				     <div class="placeholder col-12 text-center d-none bg-light">
								No items available. Drag items here
                     </div>
				     `;

                response.data.forEach((item, index) => {
                    headerDetails += `
                         <div class="col-lg-2" data-id="add_${item.PageControlID}">
                             <a class="text-body" href="#">
                                   <div class="card">
                                        <div class="card-body p-2 text-center">
                                            <img src="${item.PageIcon}" alt="" class="avatar-sm">
                                             <h5 class="mt-4 mb-2 font-size-14">${item.PageName}</h5>
                                             <p class="mb-0 text-muted font-size-12">${item.MenuDisplayName}</p>
                                         </div>
                                   </div>
                              </a>
                         </div>`;
                });

                // Append both sections to the container
                $("#divAppLauncher").append(headerDetails);

                InitializeDragandDrag();
            } else {

                $("#divAppLauncher").empty();
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}

$("#btnPwdSave").on('click', function () {

    let isValid = true; // Flag to track overall validity

    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid'); // Remove previous invalid class

    // Collect data from input fields
    var changepassword = new Object();

    changepassword.CurrentPassword = $('#profileoldPassword').val();
    changepassword.NewPassword = $('#profilenewPassword').val();
    changepassword.ConfirmPassword = $('#profileconfirmNewPassword').val();

    //Size Name
    if (!changepassword.CurrentPassword) {
        $('#profileoldPassword').addClass('is-invalid'); //Mark field as invalid
        $('#profileoldPassword').after('<div class="invalid-feedback">Please enter Old Password  </div>');
        $('#profileoldPassword').focus(); isValid = false;
        return false;
    }
    if (!changepassword.NewPassword) {
        $('#profilenewPassword').addClass('is-invalid'); //Mark field as invalid
        $('#profilenewPassword').after('<div class="invalid-feedback">Please enter New Password </div>');
        $('#profilenewPassword').focus(); isValid = false;
        return false;
    }
    if (!changepassword.ConfirmPassword) {
        $('#profileconfirmNewPassword').addClass('is-invalid'); //Mark field as invalid
        $('#profileconfirmNewPassword').after('<div class="invalid-feedback">Please  Confirm New Password</div>');
        $('#profileconfirmNewPassword').focus(); isValid = false;
        return false;
    }
    // If validation fails, keep focus on the first invalid input
    if (!isValid) return;

    SaveChangePassword(changepassword);

    return false;
});
function SaveChangePassword(changepassword) {
    if (ENABLE_VERBOSE_Logging) console.log(changepassword);
    $.ajax({
        url: SaveChangePasswordUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(changepassword),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.Result.Success && !response.Result.Error) {
                Swal.fire({ title: "Updated!", text: response.Result.Message, icon: "success", confirmButtonColor: "#556ee6" });

                $('#updatePasswordModal').modal('hide');
                ClearFormFields();
            }
            else if (!response.Result.Success && response.Result.Error) {
                Swal.fire({ title: "Password Does Not Match!", text: response.Result.Message, icon: "warning", confirmButtonColor: "#556ee6" });
            }
            else if (!response.Result.Success && !response.Result.Error) {
                Swal.fire({ title: "Error", text: response.Result.Message, icon: "error", confirmButtonColor: "#556ee6" });
            }
            else
                Swal.fire({ title: "Error", text: response.Result.Message, icon: "error", confirmButtonColor: "#556ee6" });
        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

    return false;
}
$("#btnUserSave").on('click', function () {

    let isValid = true; // Flag to track overall validity

    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid'); // Remove previous invalid class

    // Collect data from input fields
    var Username = new Object();

    Username.FirstName = $('#txtFirstName').val();
    Username.LastName = $('#txtLastName').val();
    Username.Email = $('#txtEmail').val();
    Username.LandingPageID = $('#ddlPageList').val();


    // If validation fails, keep focus on the first invalid input
    if (!isValid) return;

    UpdateUserName(Username);

    return false;
});
function UpdateUserName(Username) {
    if (ENABLE_VERBOSE_Logging) console.log(Username);
    $.ajax({
        url: UpdateUserNameUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(Username),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.Result.Success && !response.Result.Error) {
                Swal.fire({ title: "Updated!", text: response.Result.Message, icon: "success", confirmButtonColor: "#556ee6" });

                $('#updateNameModal').modal('hide');
                ClearFormFields();
            }
            else if (!response.Result.Success && response.Result.Error) {
                Swal.fire({ title: "Password Does Not Match!", text: response.Result.Message, icon: "warning", confirmButtonColor: "#556ee6" });
            }
            else if (!response.Result.Success && !response.Result.Error) {
                Swal.fire({ title: "Error", text: response.Result.Message, icon: "error", confirmButtonColor: "#556ee6" });
            }
            else
                Swal.fire({ title: "Error", text: response.Result.Message, icon: "error", confirmButtonColor: "#556ee6" });
        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

    return false;
}
function ClearFormFields() {

    $("#profileoldPassword").val("");
    $("#profilenewPassword").val("");
    $("#profileconfirmNewPassword").val("");
    $("#profileName").val("");
    $("#Password").val("");

    return false;
}

function GetUserProfile() {
    $("#divUserDetails").empty();
    $.ajax({
        url: GetUserProfileUrl,
        type: 'GET',
        success: function (response) {
            if (response != null && response.data != null) {
                // Header details
                var headerDetails = `<div class="bg-primary-subtle">
				<div class="row">
					<div class="col-7">
						<div class="text-primary p-3">
							<h5 class="text-primary">Welcome Back!</h5>
							<p class="text-primary">PROFILE PAGE</p>
						</div>
					</div>
					<div class="col-4 align-self-end">
						<img src="/assets/images/profile-img.png" alt="" class="img-fluid">
					</div>
				</div>
			</div>
			<div class="card-body pt-0">
				<div class="row">
					<div class="col-sm-4">
						<div class="avatar-md profile-user-wid mb-4 text-center">
							<img src="${response.data.UserProfilePicURL}" alt="" class="img-thumbnail rounded-circle mb-2">
						</div>
						<h5 class="font-size-15 text-truncate">${response.data.UserName}</h5>
						<p class="text-muted mb-0 text-truncate">${response.data.RoleName} </p>
					</div>
				</div>
			</div>`;



                // Append both sections to the container
                $("#divUserDetails").append(headerDetails);

                InitializeDragandDrag();
            } else {

                $("#divUserDetails").empty();
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

}

function EditData() {
    if (ENABLE_VERBOSE_Logging) console.log();
    ClearFormFields();
    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        success: function (response) {
            var userdata = response.data;
            $("#hdnUserID").val(userdata.UserID);
            $("#txtFirstName").val(userdata.FirstName);
            $("#txtLastName").val(userdata.LastName);
            $("#txtEmail").val(userdata.Email);
            // $("#hdnRoleID").val(roledata.RoleID);
            GetPageList(userdata.RoleID, userdata.LandingPageID);

        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
function GetPageList(roleID, selectedID) {
    if (roleID > 0) {
        $.ajax({
            url: ListPageControlURL,
            type: 'get',
            headers: { "RequestVerificationToken": _TOKEN },
            data: { RoleID: roleID },
            success: function (data) {
                //console.log(data);
                $('#ddlPageList').empty();
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

$('#updateNameModal').on('shown.bs.modal', function () {
    $('#ddlPageList').select2({ dropdownParent: $('#updateNameModal'), width: '100%' });
});

$("#btnProfileSave").on("click", function () {
    var formData = new FormData();
    var fileInput = document.getElementById("profileImage").files[0];

    if (!fileInput) {
        alert("Please select an image!");
        return;
    }

    formData.append("profileImage", fileInput);

    $.ajax({
        url: UploadImageURl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.success) {
                var userdata = data.filePath;

                UploadProfile(userdata);
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

});

function UploadProfile(path) {
    if (!path) {
        console.error("Error: Path is null or undefined.");
        return;
    }

    $.ajax({
        url: UploadProfileUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(path),
        success: function (response) {
            Swal.fire({ title: "Updated!", text: response.result.Message, icon: "success", confirmButtonColor: "#556ee6" });
            $('#updateProfileModal').modal('hide');
            GetUserProfile();
        },
        error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: xhr.responseText || error, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}