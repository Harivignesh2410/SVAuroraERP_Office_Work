$(function () {
    pLoadingSetup(false);
    getRecordList();
    $("#divAddEdit").hide();
    $("#btnSave").show();
    $("#btnUpdate").hide();
    if (!_CMActionAdd) {
        $("#btnAddNew").remove();
        $("#btnSave").remove();
    }
    if (!_CMActionUpdate) $("#btnUpdate").remove();
    GetApplicationList("ddlApplication", ApplicationListUrl, _TOKEN)
    pLoadingSetup(true);
});
$("#btnAddNew").click(function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddEdit .card-title").html("<i class='fas fa-plus-square'></i>&nbsp;&nbsp;Add New OEM");
    $("#divaddnew").hide();
    $("#divAddEdit").show();
    GetGlobalRoleIDByPageID(3, "ddlApplication", "ddlUserRole");
    ClearFormFields();

    $("#divPassword").show();
    $("#divCPassword").show();
});
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
$('#ddlUserRole').on('change', function () {
    $('#ddlPageList').empty();
    var roleID = $(this).val();

    GetGlobalPageList(roleID, 'ddlPageList');
});

$("#btnClose,#btnCloseWindow").click(function () {
    $("#divaddnew").show();
    $("#divAddEdit").hide();
});
function ClearFormFields() {
    $("#divAddEdit .card-body :input").attr("disabled", false);
    $("#hdnHSRPUserID").val(0);
    $("#hdnUserID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#txtUserCode").val("");
    $("#txtCompanyName").val("");
    $("#txtAdderssline1").val("");
    $("#txtAdderssline2").val("");
    $("#ddlStateList").val(0).trigger("change");
    $("#ddlDistrictList").val(0).trigger("change");
    $("#txtCity").val("");
    $("#txtpincode").val("");
    $("#txtGSTIN").val("");
    $("#txtContactNo").val("");
    $("#txtContactperson").val("");
    $("#ddlDeliveryStateList").val(0).trigger("change");
    $("#ddlDeliveryDistrictList").val(0).trigger("change");
    $("#txtDeliveryCity").val("");
    $("#txtDeliveryAddress1").val("");
    $("#txtDeliveryAddress2").val("");
    $("#txtDeliverypincode").val("");
    $("#chkEnableOnline").prop("checked", false);
    $('#divOnlineOEMName').hide();
    $("#chkStatus").prop("checked", true);
    $("#txtOnlineOEMName").val("");
    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

    $("#chkStatus").prop("checked", true);

    $("#ddlApplication").val("0").change();
    $("#txtEmail").val("");
    $("#txtUserName").val("");
    $("#txtPassword").val("");
    $("#txtConfirmPassword").val("");
    $("#ddlUserRole").val(0).change();
    $("#ddlPageList").val(0).change();

    return false;
}
$('#btnRefresh').on('click', function () {
    getRecordList();
    return false;
});
$('#divAddEdit').on('shown.bs.card', function () {
    $('#ddlStateList,#ddlSDeliverytateList').select2({ dropdownParent: $('#divAddEdit'), width: '100%' });
    $('#ddlDistrictList').select2({ dropdownParent: $('#divAddEdit'), width: '100%' });
});

$('#ddlStateList').on('change', function () {
    $('#ddlDistrictList').empty();
    $('#ddlDistrictList').append('<option value="0" disabled selected>--Select District--</option>');
    var StateID = $(this).val();

    GetDeliveryDistrictList(StateID, 'ddlDistrictList');
});
$('#chkEnableOnline').on('change', function () {
    if ($(this).is(':checked')) {
        $('#divOnlineOEMName').show();
    } else {
        $('#divOnlineOEMName').hide();
    }
});

$('#ddlDeliveryStateList').on('change', function () {
    $('#ddlDeliveryDistrictList').empty();
    $('#ddlDeliveryDistrictList').append('<option value="0" disabled selected>--Select Delivery District--</option>');
    var StateID = $(this).val();

    GetDeliveryDistrictList(StateID, 'ddlDeliveryDistrictList');
});
function GetDeliveryDistrictList(StateID, CONTROL) {
    if (StateID > 0) {
        $.ajax({
            url: DeliveryDistrictListByStateIDUrl,
            type: 'get',
            headers: { "RequestVerificationToken": _TOKEN },
            data: { StateID: StateID },
            async: false,
            success: function (data) {
                $('#' + CONTROL).empty();
                $('#' + CONTROL).append("<option value='0'>--Select  District--</option>");
                $.each(data.result.Value, function (i, result) {
                    $('#' + CONTROL).append('<option value="' + result.DistrictID + '">' + result.DistrictName + '</option>');
                });

                $('#' + CONTROL).val(0).change();
            }
        });
    } else {
        $('#' + CONTROL).empty();
        $('#' + CONTROL).append('<option value="0" disabled selected>--Select  District--</option>');
    }
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

    var OEMData = new Object();

    OEMData.HSRPUserID = 0;
    if (this.id == "btnUpdate" && $("#hdnHSRPUserID").val() > 0) OEMData.HSRPUserID = $("#hdnHSRPUserID").val();
    OEMData.HSRPUserTypeID = 3;
    OEMData.OEMID = null;
    OEMData.EmbossingStationID = null;
    OEMData.DealerID = null;
    OEMData.HSRPUserCode = $('#txtUserCode').val();
    OEMData.CompanyName = $('#txtCompanyName').val();
    OEMData.Address1 = $('#txtAdderssline1').val();
    OEMData.Address2 = $('#txtAdderssline2').val();
    OEMData.DistrictID = $('#ddlDistrictList').val();
    OEMData.City = $('#txtCity').val();
    OEMData.Pincode = $('#txtpincode').val();
    OEMData.GSTIN = $('#txtGSTIN').val();
    OEMData.ContactPerson = $('#txtContactperson').val();
    OEMData.ContactNo = $('#txtContactNo').val();
    OEMData.DeliveryAddress1 = $('#txtDeliveryAddress1').val();
    OEMData.DeliveryAddress2 = $('#txtDeliveryAddress2').val();
    OEMData.DeliveryDistrictID = $('#ddlDeliveryDistrictList').val();
    OEMData.DeliveryCity = $('#txtDeliveryCity').val();
    OEMData.DeliveryPincode = $('#txtDeliverypincode').val();
    OEMData.IsActive = $("#chkStatus").is(':checked') ? true : false;
    OEMData.OnlineOEMName = $('#txtOnlineOEMName').val();
    OEMData.IsOEMEnabledOnline = $("#chkEnableOnline").is(':checked') ? true : false;

    var userData = new Object();
    userData.UserID = 0;
    if (this.id == "btnUpdate" && $("#hdnUserID").val() > 0) userData.UserID = $("#hdnUserID").val();
    userData.FirstName = $('#txtCompanyName').val();
    userData.Email = $("#txtEmail").val();
    userData.UserName = $("#txtUserName").val();
    userData.PasswordHash = $("#txtPassword").val();
    userData.ConfirmPassword = $("#txtConfirmPassword").val();
    userData.RoleID = $("#ddlUserRole").val();
    userData.LandingPageID = $("#ddlPageList").val();
    userData.IsActive = $("#chkStatus").is(':checked') ? true : false;
    
    OEMData.Userdata = userData;
    if (!OEMData.DistrictID || OEMData.DistrictID === "0") return markInvalid("#ddlDistrictList", " Please Select District");
    if (!OEMData.DeliveryDistrictID || OEMData.DeliveryDistrictID === "0") return markInvalid("#ddlDeliveryDistrictList", " Please Select Delivery District");
    if (!OEMData.HSRPUserCode) return markInvalid("#txtUserCode", "Please enter Code");
    if (!OEMData.CompanyName) return markInvalid("#txtCompanyName", "Please enter Company Name ")
    if (!OEMData.Address1) return markInvalid("#txtAdderssline1", "Please enter Adderss line 1");
    if (!OEMData.Address2) return markInvalid("#txtAdderssline2", "Please enter Adderss line 2")
    if (!OEMData.Pincode) return markInvalid("#txtpincode", "Please enter Pincode");
    if (!OEMData.GSTIN) return markInvalid("#txtGSTIN", "Please enter GSTIN ");
    if (!OEMData.ContactPerson) return markInvalid("#txtContactperson", "Please enter Person Contact Number");
    if (!OEMData.ContactNo) return markInvalid("#txtContactNo", "Please enter Contact No")
    if (!OEMData.DeliveryAddress1) return markInvalid("#txtDeliveryAddress1", "Please enter Delivery Address Line 1");
    if (!OEMData.DeliveryAddress2) return markInvalid("#txtDeliveryAddress2", "Please enter Delivery Address Line 2")
    if (!OEMData.GSTIN) return markInvalid("#txtGSTIN", "Please enter GSTIN ");
    if (!OEMData.City) return markInvalid("#txtCity", "Please enter City ");
    if (!OEMData.DeliveryCity) return markInvalid("#txtDeliveryCity", "Please enter Delivery City");
    if (!OEMData.DeliveryPincode) return markInvalid("#txtDeliverypincode", "Please enter Delivery Pincode")

    if (!userData.FirstName) return markInvalid("#txtFirstName", "Please enter First Name");
    if (!userData.UserName) return markInvalid("#txtUserName", "Please enter User Name");
    if (userData.UserID == 0) {
        if (!userData.PasswordHash) return markInvalid("#txtPassword", "Please enter Password");
        if (!userData.ConfirmPassword) return markInvalid("#txtConfirmPassword", "Please enter Confirm Password");
    }
    if (userData.RoleID == 0 || userData.RoleID == null) return markInvalid("#ddlUserRole", "Please Kindly Config the Role in HSRP Config");
    if (userData.LandingPageID == 0 || userData.LandingPageID == null) return markInvalid("#ddlPageList", "Please Select Landing Page");

    SaveandUpdate(OEMData);

    return false;
});
function SaveandUpdate(OEMData) {
    if (ENABLE_VERBOSE_Logging)

        $.ajax({
            url: SaveUpdateDataUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(OEMData),
            success: function (response) {
                if (ENABLE_VERBOSE_Logging) //console.log(response);

                    if (response.Success && !response.Error) {
                        if (OEMData.HSRPUserID == 0)
                            Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                        else if (OEMData.HSRPUserID > 0)
                            Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                        $("#btnClose").click();
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
            { "data": "HSRPUserCode", "orderable": true, "width": "5%" },
            {
                data: null,
                render: function (data, type, row) {
                    return row.CompanyName + " (" + row.City + ")";
                },
                orderable: false
            },
            { "data": "ContactPerson", "orderable": true, "width": "5%" },
            {
                data: null,
                render: function (data, type, row) {
                    return row.Address1 + "<br>" + row.Address2 + "<br>" + row.City;
                },
                orderable: false,
                width: "15%"
            },
            { "data": "DistrictName", "orderable": true, "width": "5%" },
            { "data": "Pincode", "orderable": true, "width": "5%" },
            {
                data: null,
                render: function (data, type, row) {
                    return row.DeliveryAddress1 + "<br>" + row.DeliveryAddress2 + "<br>" + row.DeliveryCity;
                },
                orderable: false,
                width: "15%"
            },
            { "data": "DeliveryDistrict", "orderable": true, "width": "5%" },
            //{ "data": "DeliveryCity", "orderable": true, "width": "5%" },
            { "data": "DeliveryPincode", "orderable": true, "width": "5%" },
            {
                "data": "IsActive",
                "render": function (data, type, row) {
                    return SetStatus(data);
                },
                "width": "5%",
                "className": "text-center",
                "orderable": false
            },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    return SetActionButtons(row.HSRPUserID, _CMPermissions);
                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}
function EditData(ID, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) //console.log(ID);
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
                $("#divAddEdit .card-body :input").attr("disabled", true);
                $("#divAddEdit .card-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View OEM");
                $("#btnCloseWindow,#btnClose").attr("disabled", false);
                $("#divaddnew").hide();
                $("#divAddEdit").show();
            }
            else {
                $("#divAddEdit .card-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit OEM ");
                $("#btnSave").hide();
                $("#btnUpdate").show();
                $("#divAddEdit .card-body :input").attr("disabled", false);
                $("#btnCloseWindow,#btnClose").attr("disabled", false);
                $("#divAddEdit").show();
                $("#divaddnew").hide();
            }
            var OEMData = response.Value;

            $("#divPassword").hide();
            $("#divCPassword").hide();

            $("#hdnHSRPUserID").val(OEMData.HSRPUserID);
            $("#hdnUserID").val(OEMData.UserID);
            $("#txtUserCode").val(OEMData.HSRPUserCode);
            $("#txtCompanyName").val(OEMData.CompanyName);
            $("#txtAdderssline1").val(OEMData.Address1);
            $("#txtAdderssline2").val(OEMData.Address2);
            $("#ddlStateList").val(OEMData.StateID).change();
            $("#ddlDistrictList").val(OEMData.DistrictID).change();
            $("#txtCity").val(OEMData.City);
            $("#txtpincode").val(OEMData.Pincode);
            $("#txtGSTIN").val(OEMData.GSTIN);
            $("#txtContactperson").val(OEMData.ContactPerson);
            $("#txtContactNo").val(OEMData.ContactNo);
            $("#txtDeliveryAddress1").val(OEMData.DeliveryAddress1);
            $("#txtDeliveryAddress2").val(OEMData.DeliveryAddress2);
            $("#ddlDeliveryStateList").val(OEMData.DeliveryStateID).change();
            $("#ddlDeliveryDistrictList").val(OEMData.DeliveryDistrictID).change();
            $("#txtDeliveryCity").val(OEMData.DeliveryCity);
            $("#txtDeliverypincode").val(OEMData.DeliveryPincode);
            $("#chkStatus").prop('checked', OEMData.IsActive);
            $("#chkEnableOnline").prop('checked', OEMData.IsOEMEnabledOnline);
            if (OEMData.IsOEMEnabledOnline == true) {
                $("#divOnlineOEMName").show();
                $("#txtOnlineOEMName").val(OEMData.OnlineOEMName);
            } else {
                $("#divOnlineOEMName").hide();
            }

            $("#txtEmail").val(OEMData.Email);
            $("#txtUserName").val(OEMData.UserName);
            $("#ddlApplication").val(OEMData.ApplicationID).change();

            if (OEMData.UserID == 0) {
                GetGlobalRoleIDByPageID(3, "ddlApplication", "ddlUserRole");

                $("#txtEmail").val("");
                $("#txtUserName").val("");
                $("#divPassword").show();
                $("#divCPassword").show();
            }
            $("#ddlUserRole").val(OEMData.RoleID).change();
            $("#ddlPageList").val(OEMData.LandingPageID).change();

            $("#ddlApplication").prop("disabled", true);
            $("#ddlUserRole").prop("disabled", true); 


            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + OEMData.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(OEMData.LastUpdatedDate));

        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) //console.log(error);

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