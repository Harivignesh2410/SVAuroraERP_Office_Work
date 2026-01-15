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
    GetApplicationList("ddlApplication", ApplicationListUrl, _TOKEN);
    pLoadingSetup(true);
});
$("#btnAddNew").click(function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddEdit .card-title").html("<i class='fas fa-plus-square'></i>&nbsp;&nbsp;Add New Sub Dealer User");
    $("#divaddnew").hide();
    $("#divAddEdit").show();
    GetGlobalRoleIDByPageID(5, "ddlApplication", "ddlUserRole");
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

    $("#ddlDealerList").val(0).trigger("change");
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

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

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
    $('#ddlOEMList').select2({ dropdownParent: $('#divAddEdit'), width: '100%' });
    $('#ddlDealerList').select2({ dropdownParent: $('#divAddEdit'), width: '100%' });
});
$('#ddlOEMList').on('change', function () {
    $('#ddlDealerList').empty();
    $('#ddlDealerList').append('<option value="0" disabled selected>--Select Dealer--</option>');
    var OEMID = $(this).val();

    GetDealerByOEMID(OEMID, 'ddlDealerList');
});
function GetDealerByOEMID(OEMID, CONTROL) {
    if (OEMID > 0) {
        $.ajax({
            url: DealerByOEMIDUrl,
            type: 'get',
            headers: { "RequestVerificationToken": _TOKEN },
            data: { OEMID: OEMID },
            async: false,
            success: function (data) {
                $('#' + CONTROL).empty();
                $('#' + CONTROL).append("<option value='0'>--Select  Dealer--</option>");
                $.each(data.result.Value, function (i, result) {
                    $('#' + CONTROL).append('<option value="' + result.HSRPUserID + '">' + result.CompanyName + '</option>');
                });

                $('#' + CONTROL).val(0).change();
            }
        });
    } else {
        $('#' + CONTROL).empty();
        $('#' + CONTROL).append('<option value="0" disabled selected>--Select  Dealer--</option>');
    }
}

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
    $('.form-control').removeClass('is-invalid');

    var SubDealerData = new Object();

    SubDealerData.HSRPUserID = 0;
    if (this.id == "btnUpdate" && $("#hdnHSRPUserID").val() > 0) SubDealerData.HSRPUserID = $("#hdnHSRPUserID").val();
    SubDealerData.HSRPUserTypeID = 5;
    SubDealerData.DealerID = $('#ddlDealerList').val();
    SubDealerData.EmbossingStationID = null;
    SubDealerData.OEMID = $('#ddlOEMList').val();
    SubDealerData.HSRPUserCode = $('#txtUserCode').val();
    SubDealerData.CompanyName = $('#txtCompanyName').val();
    SubDealerData.Address1 = $('#txtAdderssline1').val();
    SubDealerData.Address2 = $('#txtAdderssline2').val();
    SubDealerData.DistrictID = $('#ddlDistrictList').val();
    SubDealerData.City = $('#txtCity').val();
    SubDealerData.Pincode = $('#txtpincode').val();
    SubDealerData.GSTIN = $('#txtGSTIN').val();
    SubDealerData.ContactPerson = $('#txtContactperson').val();
    SubDealerData.ContactNo = $('#txtContactNo').val();
    SubDealerData.DeliveryAddress1 = $('#txtDeliveryAddress1').val();
    SubDealerData.DeliveryAddress2 = $('#txtDeliveryAddress2').val();
    SubDealerData.DeliveryDistrictID = $('#ddlDeliveryDistrictList').val();
    SubDealerData.DeliveryCity = $('#txtDeliveryCity').val();
    SubDealerData.DeliveryPincode = $('#txtDeliverypincode').val();
    SubDealerData.IsActive = $("#chkActive").is(':checked') ? true : false;

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

    SubDealerData.Userdata = userData;

    if (!SubDealerData.DealerID || SubDealerData.DealerID === "0") return markInvalid("#ddlDealerList", " Please Select Dealer");
    if (!SubDealerData.DistrictID || SubDealerData.DistrictID === "0") return markInvalid("#ddlDistrictList", " Please Select District");
    if (!SubDealerData.DeliveryDistrictID || SubDealerData.DeliveryDistrictID === "0") return markInvalid("#ddlDeliveryDistrictList", " Please Select Delivery District");
    if (!SubDealerData.HSRPUserCode) return markInvalid("#txtUserCode", "Please enter Code");
    if (!SubDealerData.CompanyName) return markInvalid("#txtCompanyName", "Please enter Company Name ")
    if (!SubDealerData.Address1) return markInvalid("#txtAdderssline1", "Please enter Adderss line 1");
    if (!SubDealerData.Address2) return markInvalid("#txtAdderssline2", "Please enter Adderss line 2")
    if (!SubDealerData.Pincode) return markInvalid("#txtpincode", "Please enter Pincode");
    if (!SubDealerData.GSTIN) return markInvalid("#txtGSTIN", "Please enter GSTIN ");
    if (!SubDealerData.ContactPerson) return markInvalid("#txtContactperson", "Please enter Person Contact Number");
    if (!SubDealerData.ContactNo) return markInvalid("#txtContactNo", "Please enter Contact No")
    if (!SubDealerData.DeliveryAddress1) return markInvalid("#txtDeliveryAddress1", "Please enter Delivery Address Line 1");
    if (!SubDealerData.DeliveryAddress2) return markInvalid("#txtDeliveryAddress2", "Please enter Delivery Address Line 2")
    if (!SubDealerData.GSTIN) return markInvalid("#txtGSTIN", "Please enter GSTIN ");
    if (!SubDealerData.City) return markInvalid("#txtCity", "Please enter City ");
    if (!SubDealerData.DeliveryCity) return markInvalid("#txtDeliveryCity", "Please enter Delivery City");
    if (!SubDealerData.DeliveryPincode) return markInvalid("#txtDeliverypincode", "Please enter Delivery Pincode");
    if (!isValid) return;

    if (!userData.FirstName) return markInvalid("#txtFirstName", "Please enter First Name");

    if (!userData.UserName) return markInvalid("#txtUserName", "Please enter User Name");

    if (userData.UserID == 0) {
        if (!userData.PasswordHash) return markInvalid("#txtPassword", "Please enter Password");
        if (!userData.ConfirmPassword) return markInvalid("#txtConfirmPassword", "Please enter Confirm Password");
    }
    if (userData.RoleID == 0) return markInvalid("#ddlUserRole", "Please Kindly Config the Role in HSRP Config");
    if (userData.LandingPageID == 0) return markInvalid("#ddlPageList", "Please Select Landing Page");  

    if (!isValid) return;
    SaveandUpdate(SubDealerData);

    return false;
});
function SaveandUpdate(SubDealerData) {
    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(SubDealerData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.Success && !response.Error) {
                if (SubDealerData.HSRPUserID == 0)
                    Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                else if (SubDealerData.HSRPUserID > 0)
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
                "orderable": false
            },
            { "data": "HSRPUserCode", "orderable": true },
            { "data": "DealerName", "orderable": true },
            {
                data: null,
                render: function (data, type, row) {
                    return row.CompanyName + " (" + row.City + ")";
                },
                orderable: false
            },
            { "data": "ContactPerson", "orderable": true },
            {
                data: null,
                render: function (data, type, row) {
                    return row.Address1 + "<br>" + row.Address2 + "<br>" + row.City + "-" + row.Pincode;
                },
                orderable: false,
            },
            { "data": "DistrictName", "orderable": true },
            {
                data: null,
                render: function (data, type, row) {
                    return row.DeliveryAddress1 + "<br>" + row.DeliveryAddress2 + "<br>" + row.DeliveryCity + "-" + row.DeliveryPincode;
                },
                orderable: false,
            },
            { "data": "DeliveryDistrict", "orderable": true },
            {
                "data": "IsActive",
                "render": function (data, type, row) {
                    return SetStatus(data);
                },
                "className": "text-center",
                "orderable": false
            },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    return SetActionButtons(row.HSRPUserID, _CMPermissions);
                },
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
                $("#divAddEdit .card-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Sub Dealer User");
                $("#btnCloseWindow,#btnClose").attr("disabled", false);
                $("#divaddnew").hide();
                $("#divAddEdit").show();
            }
            else {
                $("#divAddEdit .card-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Sub Dealer User");
                $("#btnSave").hide();
                $("#btnUpdate").show();
                $("#divAddEdit .card-body :input").attr("disabled", false);
                $("#divAddEdit").show();
                $("#divaddnew").hide();
            }
            var SubDealerData = response.Value;

            $("#divPassword").hide();
            $("#divCPassword").hide();
            $("#hdnUserID").val(SubDealerData.UserID);

            $("#hdnHSRPUserID").val(SubDealerData.HSRPUserID);
            $("#txtUserCode").val(SubDealerData.HSRPUserCode);
            $("#txtCompanyName").val(SubDealerData.CompanyName);
            $("#txtAdderssline1").val(SubDealerData.Address1);
            $("#txtAdderssline2").val(SubDealerData.Address2);
            $("#ddlOEMList").val(SubDealerData.OEMID).change();
            $("#ddlDealerList").val(SubDealerData.DealerID).change();
            $("#ddlStateList").val(SubDealerData.StateID).change();
            $("#ddlDistrictList").val(SubDealerData.DistrictID).change();
            $("#txtCity").val(SubDealerData.City);
            $("#txtpincode").val(SubDealerData.Pincode);
            $("#txtGSTIN").val(SubDealerData.GSTIN);
            $("#txtContactperson").val(SubDealerData.ContactPerson);
            $("#txtContactNo").val(SubDealerData.ContactNo);
            $("#txtDeliveryAddress1").val(SubDealerData.DeliveryAddress1);
            $("#txtDeliveryAddress2").val(SubDealerData.DeliveryAddress2);
            $("#ddlDeliveryStateList").val(SubDealerData.DeliveryStateID).change();
            $("#ddlDeliveryDistrictList").val(SubDealerData.DeliveryDistrictID).change();
            $("#txtDeliveryCity").val(SubDealerData.DeliveryCity);
            $("#txtDeliverypincode").val(SubDealerData.DeliveryPincode);
            $("#chkActive").prop('checked', SubDealerData.IsActive);


            $("#txtEmail").val(SubDealerData.Email);
            $("#txtUserName").val(SubDealerData.UserName);
            $("#ddlApplication").val(SubDealerData.ApplicationID).change();

            if (SubDealerData.UserID == 0) {
                GetGlobalRoleIDByPageID(4, "ddlApplication", "ddlUserRole");

                $("#txtEmail").val("");
                $("#txtUserName").val("");
                $("#divPassword").show();
                $("#divCPassword").show();
            }
            $("#ddlUserRole").val(SubDealerData.RoleID).change();
            $("#ddlPageList").val(SubDealerData.LandingPageID).change();

            $("#ddlApplication").prop("disabled", true);
            $("#ddlUserRole").prop("disabled", true); 

            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + SubDealerData.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(SubDealerData.LastUpdatedDate));

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