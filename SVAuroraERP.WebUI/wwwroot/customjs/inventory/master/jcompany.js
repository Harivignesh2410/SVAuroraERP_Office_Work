$(function () {
    pLoadingSetup(false);
    EditData();

    $("#btnUpdate").show();
    $("#divRecordLog").hide();
    pLoadingSetup(true);
});

$("#btnUpdate").on('click', function () {
    let isValid = true; // Flag to track overall validity

    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid'); // Remove previous invalid class

    // Collect data from input fields
    var CompanyData = new Object();

    CompanyData.CompanyID = 0;
    if (this.id == "btnUpdate" && $("#hdnCompanyID").val() > 0) CompanyData.CompanyID = $("#hdnCompanyID").val();

    CompanyData.CompanyName = $('#txtCompanyName').val();
    CompanyData.GSTNo = $('#txtGstNo').val();
    CompanyData.AddressLine1 = $('#txtAddressLine1').val();
    CompanyData.AddressLine2 = $('#txtAddressLine2').val();
    CompanyData.City = $('#txtCity').val();
    CompanyData.State = $('#txtState').val();
    CompanyData.Pincode = $('#txtPincode').val();
    CompanyData.TelNo1 = $('#txtTelNo1').val();
    CompanyData.TelNo2 = $('#txtTelNo2').val();
    CompanyData.MobileNo = $('#txtMobileNo').val();
    CompanyData.Email = $('#txtEmail').val();
    CompanyData.BankName = $('#txtBankName').val();
    CompanyData.BranchName = $('#txtBranchName').val();
    CompanyData.IFSCCode = $('#txtIFSCCode').val();
    CompanyData.AccountHolderName = $('#txtAccountHolder').val();
    CompanyData.AccountType = $('#ddlAccountType').val();
    CompanyData.AccountNo = $('#txtAccountNo').val();
    CompanyData.PANNo = $('#txtPanNo').val();

    if (!CompanyData.CompanyName) {
        $('#txtCompanyName').addClass('is-invalid'); //Mark field as invalid
        $('#txtCompanyName').after('<div class="invalid-feedback">Please enter Company Name</div>');
        $('#txtCompanyName').focus(); isValid = false;

        return false;
    }
    if (!CompanyData.GSTNo) {
        $('#txtGstNo').addClass('is-invalid'); //Mark field as invalid
        $('#txtGstNo').after('<div class="invalid-feedback">Please enter GST No</div>');
        $('#txtGstNo').focus(); isValid = false;

        return false;
    }
    if (!CompanyData.PANNo) {
        $('#txtPanNo').addClass('is-invalid'); //Mark field as invalid
        $('#txtPanNo').after('<div class="invalid-feedback">Please enter PAN No</div>');
        $('#txtPanNo').focus(); isValid = false;

        return false;
    }
    if (!CompanyData.AddressLine1) {
        $('#txtAddressLine1').addClass('is-invalid'); //Mark field as invalid
        $('#txtAddressLine1').after('<div class="invalid-feedback">Please enter AddressLine1</div>');
        $('#txtAddressLine1').focus(); isValid = false;

        return false;
    }
    if (!CompanyData.MobileNo) {
        $('#txtMobileNo').addClass('is-invalid'); //Mark field as invalid
        $('#txtMobileNo').after('<div class="invalid-feedback">Please enter Mobile No</div>');
        $('#txtMobileNo').focus(); isValid = false;

        return false;
    }
    if (!CompanyData.Email) {
        $('#txtEmail').addClass('is-invalid'); //Mark field as invalid
        $('#txtEmail').after('<div class="invalid-feedback">Please enter Email</div>');
        $('#txtEmail').focus(); isValid = false;

        return false;
    }

    if (!isValid) return;

    SaveandUpdateCompany(CompanyData);

    return false;
});
function SaveandUpdateCompany(CompanyData) {
    if (ENABLE_VERBOSE_Logging) console.log(CompanyData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(CompanyData),
        success: function (response) {

            if (ENABLE_VERBOSE_Logging) //console.log(response);

            if (response.Success) {
                if (CompanyData.CompanyID == 0)
                    Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                else if (CompanyData.CompanyID > 0)
                    Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                EditData();
            }
            else if (!response.Success && response.Error) {
                Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
            }
            else if (!response.Success && !response.Error) {
                Swal.fire({ title: "Error", text: SaveErrorMessage, icon: "error", confirmButtonColor: "#556ee6" });
            }
            else {
                Swal.fire({ title: "Error", text: response.message, icon: "error", confirmButtonColor: "#556ee6" });
            }
        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

    return false;
}

function EditData() {
    $.ajax({
        url: GetDataUrl,
        type: 'GET',
        contentType: 'application/json',
        success: function (response) {

            $("#btnUpdate").show();

            var Companydata = response;

            $("#hdnCompanyID").val(Companydata.CompanyID);
            $("#txtCompanyName").val(Companydata.CompanyName);
            $("#txtGstNo").val(Companydata.GSTNo);

            $("#txtAddressLine1").val(Companydata.AddressLine1);
            $("#txtAddressLine2").val(Companydata.AddressLine2);
            $("#txtCity").val(Companydata.City);
            $("#txtState").val(Companydata.State);
            $("#txtPincode").val(Companydata.Pincode);

            $("#txtTelNo1").val(Companydata.TelNo1);
            $("#txtTelNo2").val(Companydata.TelNo2);
            $("#txtMobileNo").val(Companydata.MobileNo);
            $("#txtEmail").val(Companydata.Email);

            $("#txtBankName").val(Companydata.BankName);
            $("#txtBranchName").val(Companydata.BranchName);
            $("#txtIFSCCode").val(Companydata.IFSCCode);
            $("#txtAccountHolder").val(Companydata.AccountHolderName);
            $("#ddlAccountType").val(Companydata.AccountType);
            $("#txtAccountNo").val(Companydata.AccountNo);
            $("#txtPanNo").val(Companydata.PANNo);

            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + Companydata.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + Companydata.LastUpdatedDate);
        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
