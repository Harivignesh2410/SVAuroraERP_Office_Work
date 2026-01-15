$('#txtUserName, #txtPassword').on('keypress', function (event) {
    // Check if the pressed key is Enter (key code 13)
    if (event.which === 13) {
        event.preventDefault(); // Prevent default form submission
        $('#btnSignIn').click(); // Trigger the click event on the login button
    }
});

$("#btnSignIn").click(function () {
    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid'); // Remove previous invalid class

    b = detect.parse(navigator.userAgent);

    var userlogin = new Object();
    userlogin.UserName = $("#txtUserName").val();
    userlogin.UserPassword = $("#txtPassword").val();
    userlogin.DeviceType = b.device.type;
    userlogin.BrowserName = b.browser.name;
    userlogin.OSFamily = b.os.family;
    userlogin.OSName = b.os.name;
    userlogin.UserAgent = navigator.userAgent;
    userlogin.ReturnURL = $("#hdnReturnUrl").val();

    if (!userlogin.UserName) {
        $('#txtUserName').addClass('is-invalid'); //Mark field as invalid
        $('#txtUserName').after('<div class="invalid-feedback">Please enter Username</div>');
        $('#txtUserName').focus(); return false;
    }

    if (!userlogin.UserPassword) {
        $('#txtPassword').addClass('is-invalid'); //Mark field as invalid
        $('#txtPassword').after('<div class="invalid-feedback">Please enter Password</div>');
        $('#txtPassword').focus(); return false;
    }

    $("#btnSignIn").prop("disabled", true);  // Disable button
    $("#btnText").text("Signing In...");    // Change button text
    $("#loadingSpinner").removeClass("d-none");  // Show spinner

    GetUserlogin(userlogin);

    return false;
});

function GetUserlogin(data) {
    $.ajax({
        url: SignInURL,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(data),
        success: function (response) {
            if (response.success) {
                window.location.href = response.redirectpage;
            } else {
                Swal.fire({ title: "Invalid", text: response.message, icon: "warning", confirmButtonColor: "#556ee6" });
                resetSignInButton();
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
            resetSignInButton();
        }
    });

    return false;
}

function resetSignInButton() {
    $("#btnSignIn").prop("disabled", false);
    $("#btnText").html("<i class='fas fa-user-lock font-size-16 align-middle me-2'></i>Sign In");
    $("#loadingSpinner").addClass("d-none");
}