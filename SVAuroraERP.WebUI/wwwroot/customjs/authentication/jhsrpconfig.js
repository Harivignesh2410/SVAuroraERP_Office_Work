var PageTitle = "HSRP Configuration";
$(function () {
    pLoadingSetup(false);

    $("#btnSave").show();
    EditData(true); 

    pLoadingSetup(true);
});

$("#btnUpdate").on('click', function () {
    if (this.id == "btnUpdate") {
        if (!_CMActionAdd) {
            $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
            return false;
        }
    }

    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid'); // Remove previous invalid class

    // Collect data from input fields
    var OEMData = new Object();
    OEMData.BoxID = 0;

    OEMData.TVSOEMID = $('#ddlHSRPUserTVS').val();
    OEMData.SaravanaEngOEMID = $('#ddlHSRPUserSaravana ').val();
    OEMData.EroyceMotorsOEMID = $('#ddlHSRPUserEroyceMotors ').val();


    SaveandUpdateOEM(OEMData);

    return false;
});
function SaveandUpdateOEM(OEMData) {
    if (ENABLE_VERBOSE_Logging) console.log(OEMData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(OEMData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
                if (response != null && response.result != null) {
                    if (response.result.Success && !response.result.Error) {
                        Swal.fire({
                            title: "Saved!",
                            text:  SaveSuccessMessage,
                            icon: "success"
                        })
                    }
                    else if (!response.result.Success && response.result.Error) {
                        Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
                    }
                    else if (!response.result.Success && !response.result.Error) {
                        Swal.fire({ title: "Error", text: SaveErrorMessage, icon: "error", confirmButtonColor: "#556ee6" });
                    }
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

$("#btnUpdateRole").on('click', function () {
    if (this.id == "btnUpdateRole") {
        if (!_CMActionAdd) {
            $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
            return false;
        }
    }

    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid'); // Remove previous invalid class

    // Collect data from input fields
    var RoleConfigData = new Object();

    RoleConfigData.SuperAdminRoleID = $('#ddlSuperAdmin').val() || 0;
    RoleConfigData.AdminRoleID = $('#ddlAdmin ').val() || 0;
    RoleConfigData.EmbossingStationRoleID = $('#ddlEmbossingStation ').val() || 0;
    RoleConfigData.OEMRoleID = $('#ddlOEMRole ').val() || 0;
    RoleConfigData.DealerRoleID = $('#ddlDealer ').val() || 0;

    RoleConfigData.DealerSubUserID = $('#ddlDealerSubUser ').val() || 0;
    RoleConfigData.EmbossingSubUserID = $('#ddlEmbossingSubuser ').val() || 0;
    RoleConfigData.OEMSubUserID = $('#ddlOEMSubUser ').val() || 0;

    if (RoleConfigData.SuperAdminRoleID == 0)
        return markInvalid("#ddlSuperAdmin", "Please select Super Admin role");
    if (RoleConfigData.AdminRoleID == 0)
        return markInvalid("#ddlAdmin", "Please select Admin role");
    if (RoleConfigData.EmbossingStationRoleID == 0)
        return markInvalid("#ddlEmbossingStation", "Please select Embossing Station role");
    if (RoleConfigData.OEMRoleID == 0)
        return markInvalid("#ddlOEMRole", "Please select OEM role");
    if (RoleConfigData.DealerRoleID == 0)
        return markInvalid("#ddlDealer", "Please select Dealer role");

    if (RoleConfigData.DealerSubUserID == 0)
        return markInvalid("#ddlDealerSubUser", "Please select Dealer Sub User");
    if (RoleConfigData.EmbossingSubUserID == 0)
        return markInvalid("#ddlEmbossingSubuser", "Please select Embossing Sub User");
    if (RoleConfigData.OEMSubUserID == 0)
        return markInvalid("#ddlOEMSubUser", "Please select OEM Sub User");



    SaveandUpdateRole(RoleConfigData);

    return false;
});
function SaveandUpdateRole(RoleConfigData) {
    if (ENABLE_VERBOSE_Logging) console.log(RoleConfigData);

    $.ajax({
        url: SaveUpdateDataRoleUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(RoleConfigData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
                if (response != null && response.result != null) {
                    if (response.result.Success && !response.result.Error) {
                        Swal.fire({
                            title: "Role Saved!",
                            text: SaveSuccessMessage,
                            icon: "success"
                        })
                    }
                    else if (!response.result.Success && response.result.Error) {
                        Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
                    }
                    else if (!response.result.Success && !response.result.Error) {
                        Swal.fire({ title: "Error", text: SaveErrorMessage, icon: "error", confirmButtonColor: "#556ee6" });
                    }
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
function EditData(ViewFlag) {
    if (ENABLE_VERBOSE_Logging) console.log("EditData called");

    if ((!_CMActionView && ViewFlag) || (!_CMActionUpdate && !ViewFlag)) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }

    $.ajax({
        url: GetHSRPConfigDataURL,
        type: 'GET',
        contentType: 'application/json',
        success: function (response) {
            $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit " + PageTitle);
            $('#divAddEditModal').modal('show');

            // --- OEM Config ---
            var OEMData = response.OEMConfigList[0]; // assuming single row
            $("#ddlHSRPUserTVS").val(OEMData.TVSOEMID).change();
            $("#ddlHSRPUserSaravana").val(OEMData.SaravanaEngOEMID).change();
            $("#ddlHSRPUserEroyceMotors").val(OEMData.EroyceMotorsOEMID).change();

            // --- Role Config ---
            var RoleData = response.RoleConfigList[0]; // assuming single row
            $("#ddlSuperAdmin").val(RoleData.SuperAdminRoleID).change();
            $("#ddlAdmin").val(RoleData.AdminRoleID).change();
            $("#ddlEmbossingStation").val(RoleData.EmbossingStationRoleID).change();
            $("#ddlOEMRole").val(RoleData.OEMRoleID).change();
            $("#ddlDealer").val(RoleData.DealerRoleID).change();

            $("#ddlDealerSubUser").val(RoleData.DealerSubUserID).change();
            $("#ddlEmbossingSubuser").val(RoleData.EmbossingSubUserID).change();
            $("#ddlOEMSubUser").val(RoleData.OEMSubUserID).change();


     
            // --- Record log (Last Updated) ---
            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + OEMData.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(OEMData.LastUpdatedDate));

            $("#divRecordLogRole").show();
            $("#spnLastUpdatedByRole").html("Last Updated By: " + RoleData.LastUpdatedByName);
            $("#spnLastUpdatedDateRole").html("Date: " + ISTtoLocalTime(RoleData.LastUpdatedDate));

        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
