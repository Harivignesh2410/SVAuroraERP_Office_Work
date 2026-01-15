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
    $("#divDealerEnabledOnline").hide();
    GetApplicationList("ddlApplication", ApplicationListUrl, _TOKEN);
    pLoadingSetup(true);
});
$("#btnAddNew").click(function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddEdit .card-title").html("<i class='fas fa-plus-square'></i>&nbsp;&nbsp;Add New Dealer");
    $("#divaddnew").hide();
    $("#divAddEdit").show();
    GetGlobalRoleIDByPageID(4, "ddlApplication", "ddlUserRole");
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
    $("#ddlOEMList").val(0).trigger("change");
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
    $("#chkActive").prop("checked", true);
    $("#chkIsDealerEnabledOnline").prop("checked", false);
    $("#divDealerEnabledOnline").hide();
    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();
    $("#chkSameAsAddress").prop("checked", false);
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

    var DealerData = new Object();

    DealerData.HSRPUserID = 0;
    if (this.id == "btnUpdate" && $("#hdnHSRPUserID").val() > 0) DealerData.HSRPUserID = $("#hdnHSRPUserID").val();
    DealerData.HSRPUserTypeID = 4;
    DealerData.EmbossingStationID = null;
    DealerData.OEMID = $('#ddlOEMList').val();
    DealerData.HSRPUserCode = $('#txtUserCode').val();
    DealerData.CompanyName = $('#txtCompanyName').val();
    DealerData.Address1 = $('#txtAdderssline1').val();
    DealerData.Address2 = $('#txtAdderssline2').val();
    DealerData.DistrictID = $('#ddlDistrictList').val();
    DealerData.City = $('#txtCity').val();
    DealerData.Pincode = $('#txtpincode').val();
    DealerData.GSTIN = $('#txtGSTIN').val();
    DealerData.ContactPerson = $('#txtContactperson').val();
    DealerData.ContactNo = $('#txtContactNo').val();
    DealerData.DeliveryAddress1 = $('#txtDeliveryAddress1').val();
    DealerData.DeliveryAddress2 = $('#txtDeliveryAddress2').val();
    DealerData.DeliveryDistrictID = $('#ddlDeliveryDistrictList').val();
    DealerData.DeliveryCity = $('#txtDeliveryCity').val();
    DealerData.DeliveryPincode = $('#txtDeliverypincode').val();
    DealerData.IsActive = $("#chkActive").is(':checked') ? true : false;
    DealerData.IsDealerEnabledOnline = $("#chkIsDealerEnabledOnline").is(':checked') ? true : false;

    var userData = new Object();
    userData.UserID = 0;
    if (this.id == "btnUpdate" && $("#hdnHSRPUserID").val() > 0) userData.UserID = $("#hdnUserID").val();
    userData.FirstName = $('#txtCompanyName').val();
    userData.Email = $("#txtEmail").val();
    userData.UserName = $("#txtUserName").val();
    userData.PasswordHash = $("#txtPassword").val();
    userData.ConfirmPassword = $("#txtConfirmPassword").val();
    userData.RoleID = $("#ddlUserRole").val();
    userData.LandingPageID = $("#ddlPageList").val();
    userData.IsActive = $("#chkActive").is(':checked') ? true : false;

    DealerData.Userdata = userData;

    if (!DealerData.DistrictID || DealerData.DistrictID === "0") return markInvalid("#ddlDistrictList", " Please Select District");
    if (!DealerData.DeliveryDistrictID || DealerData.DeliveryDistrictID === "0") return markInvalid("#ddlDeliveryDistrictList", " Please Select Delivery District");
    if (!DealerData.HSRPUserCode) return markInvalid("#txtUserCode", "Please enter Code");
    if (!DealerData.CompanyName) return markInvalid("#txtCompanyName", "Please enter Company Name ")
    if (!DealerData.Address1) return markInvalid("#txtAdderssline1", "Please enter Adderss line 1");
    if (!DealerData.Address2) return markInvalid("#txtAdderssline2", "Please enter Adderss line 2")
    if (!DealerData.Pincode) return markInvalid("#txtpincode", "Please enter Pincode");
    if (!DealerData.GSTIN) return markInvalid("#txtGSTIN", "Please enter GSTIN ");
    if (!DealerData.ContactPerson) return markInvalid("#txtContactperson", "Please enter Person Contact Number");
    if (!DealerData.ContactNo) return markInvalid("#txtContactNo", "Please enter Contact No")
    if (!DealerData.DeliveryAddress1) return markInvalid("#txtDeliveryAddress1", "Please enter Delivery Address Line 1");
    if (!DealerData.DeliveryAddress2) return markInvalid("#txtDeliveryAddress2", "Please enter Delivery Address Line 2")
    if (!DealerData.GSTIN) return markInvalid("#txtGSTIN", "Please enter GSTIN ");
    if (!DealerData.City) return markInvalid("#txtCity", "Please enter City ");
    if (!DealerData.DeliveryCity) return markInvalid("#txtDeliveryCity", "Please enter Delivery City");
    if (!DealerData.DeliveryPincode) return markInvalid("#txtDeliverypincode", "Please enter Delivery Pincode");

    if (!userData.FirstName) return markInvalid("#txtFirstName", "Please enter First Name");

    if (!userData.UserName) return markInvalid("#txtUserName", "Please enter User Name");

    if (userData.UserID == 0) {
        if (!userData.PasswordHash) return markInvalid("#txtPassword", "Please enter Password");
        if (!userData.ConfirmPassword) return markInvalid("#txtConfirmPassword", "Please enter Confirm Password");
    }
    if (userData.RoleID == 0 || userData.RoleID == null) return markInvalid("#ddlUserRole", "Please Kindly Config the Role in HSRP Config");
    if (userData.LandingPageID == 0 || userData.LandingPageID == null) return markInvalid("#ddlPageList", "Please Select Landing Page");

    if (!isValid) return;

    SaveandUpdate(DealerData);

    return false;
});
function SaveandUpdate(DealerData) {
    if (ENABLE_VERBOSE_Logging)

        $.ajax({
            url: SaveUpdateDataUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(DealerData),
            success: function (response) {
                if (ENABLE_VERBOSE_Logging) //console.log(response);

                    if (response.Success && !response.Error) {
                        if (DealerData.HSRPUserID == 0)
                            Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                        else if (DealerData.HSRPUserID > 0)
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
            { "data": "HSRPUserCode", "orderable": true, "width": "10%" },
            {
                data: null,
                render: function (data, type, row) {
                    return row.CompanyName + " (" + row.City + ")";
                },
                orderable: false
            },
            { "data": "OEMName", "orderable": true, "width": "10%" },
            { "data": "ContactPerson", "orderable": true, "width": "10%" },
            {
                data: null,
                render: function (data, type, row) {
                    return row.Address1 + "<br>" + row.Address2 + "<br>" + row.City;
                },
                orderable: false,
                width: "15%"
            },
            // { "data": "GSTIN", "orderable": true, "width": "5%" },
            { "data": "DistrictName", "orderable": true, "width": "10%" },
            //{ "data": "City", "orderable": true, "width": "10%" },
            { "data": "Pincode", "orderable": true, "width": "10%" },
            {
                data: null,
                render: function (data, type, row) {
                    return row.DeliveryAddress1 + "<br>" + row.DeliveryAddress2 + "<br>" + row.DeliveryCity;
                },
                orderable: false,
                width: "15%"
            },
            { "data": "DeliveryDistrict", "orderable": true, "width": "10%" },
            //{ "data": "DeliveryCity", "orderable": true, "width": "10%" },
            { "data": "DeliveryPincode", "orderable": true, "width": "10%" },
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
                $("#divAddEdit .card-body :input").attr("disabled", true);
                $("#divAddEdit .card-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Dealer");
                $("#btnCloseWindow,#btnClose").attr("disabled", false);
                $("#divaddnew").hide();
                $("#divAddEdit").show();
            }
            else {
                $("#divAddEdit .card-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Dealer");
                $("#btnSave").hide();
                $("#btnUpdate").show();
                $("#divAddEdit .card-body :input").attr("disabled", false);
                $("#divAddEdit").show();
                $("#divaddnew").hide();
            }
            var DealerData = response.Value;
            $("#divPassword").hide();
            $("#divCPassword").hide();
            $("#hdnUserID").val(DealerData.UserID);

            $("#hdnHSRPUserID").val(DealerData.HSRPUserID);
            $("#txtUserCode").val(DealerData.HSRPUserCode);
            $("#txtCompanyName").val(DealerData.CompanyName);
            $("#txtAdderssline1").val(DealerData.Address1);
            $("#txtAdderssline2").val(DealerData.Address2);
            $("#ddlOEMList").val(DealerData.OEMID).change();
            $("#ddlStateList").val(DealerData.StateID).change();
            $("#ddlDistrictList").val(DealerData.DistrictID).change();
            $("#txtCity").val(DealerData.City);
            $("#txtpincode").val(DealerData.Pincode);
            $("#txtGSTIN").val(DealerData.GSTIN);
            $("#txtContactperson").val(DealerData.ContactPerson);
            $("#txtContactNo").val(DealerData.ContactNo);
            $("#txtDeliveryAddress1").val(DealerData.DeliveryAddress1);
            $("#txtDeliveryAddress2").val(DealerData.DeliveryAddress2);
            $("#ddlDeliveryStateList").val(DealerData.DeliveryStateID).change();
            $("#ddlDeliveryDistrictList").val(DealerData.DeliveryDistrictID).change();
            $("#txtDeliveryCity").val(DealerData.DeliveryCity);
            $("#txtDeliverypincode").val(DealerData.DeliveryPincode);
            $("#chkActive").prop('checked', DealerData.IsActive);
            $("#chkIsDealerEnabledOnline").prop('checked', DealerData.IsDealerEnabledOnline);

            $("#txtEmail").val(DealerData.Email);
            $("#txtUserName").val(DealerData.UserName);
            $("#ddlApplication").val(DealerData.ApplicationID).change();

            if (DealerData.UserID == 0) {
                GetGlobalRoleIDByPageID(4 , "ddlApplication", "ddlUserRole");

                $("#txtEmail").val("");
                $("#txtUserName").val("");
                $("#divPassword").show();
                $("#divCPassword").show();
            }
            $("#ddlUserRole").val(DealerData.RoleID).change();
            $("#ddlPageList").val(DealerData.LandingPageID).change();

            $("#ddlApplication").prop("disabled", true);
            $("#ddlUserRole").prop("disabled", true); 


            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + DealerData.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(DealerData.LastUpdatedDate));

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

$("#chkSameAsAddress").on("change", function () {

    if ($(this).is(":checked")) {

        $("#txtDeliveryAddress1").val($("#txtAdderssline1").val());
        $("#txtDeliveryAddress2").val($("#txtAdderssline2").val());
        $("#txtDeliveryCity").val($("#txtCity").val());
        $("#txtDeliverypincode").val($("#txtpincode").val());

        $("#ddlDeliveryStateList")
            .val($("#ddlStateList").val())
            .trigger("change");

        setTimeout(function () {
            $("#ddlDeliveryDistrictList")
                .val($("#ddlDistrictList").val())
                .trigger("change");
        }, 300);

        $("#txtDeliveryAddress1,#txtDeliveryAddress2,#txtDeliveryCity,#txtDeliverypincode")
            .prop("readonly", true);

        $("#ddlDeliveryStateList,#ddlDeliveryDistrictList")
            .prop("disabled", true)
            .trigger("change");

    } else {

        $("#txtDeliveryAddress1,#txtDeliveryAddress2,#txtDeliveryCity,#txtDeliverypincode")
            .val("")
            .prop("readonly", false);

        $("#ddlDeliveryStateList,#ddlDeliveryDistrictList")
            .prop("disabled", false)
            .val(0)
            .trigger("change");
    }
});