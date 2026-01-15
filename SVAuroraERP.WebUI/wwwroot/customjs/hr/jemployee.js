$(function () {
    pLoadingSetup(false);
    if (!_CMActionAdd) {
        $("#btnAddNew").remove();
        $("#btnSave").remove();
    }
    if (!_CMActionUpdate) $("#btnUpdate").remove();
    getRecordList();

    $("#btnSave").show();
    $("#btnUpdate").hide();

    //// Set default visibility on page load
    $("#divAddEdit").hide();  // Hide the add/edit section
    $("#divRecords").show();  // Show the records section

    // Initialize datepickers
    $("#txtDoBDate,#txtFatherDate,#txtMothertDate,#txtSpouseDate,#txtAnnuDate,#txtChild1Date,#txtChild2Date").datetimepicker({
        pickTime: false,
        useCurrent: true,
        format: 'DD/MM/YYYY',
        minYear: 1951,
        maxDate: moment()
    });

    $(".ClearDate").click(function () { var x = $(this).prev().attr('id'); $("#" + x).val(""); });
    $(".SelectDate").click(function () { var x = $(this).next().attr('id'); $("#" + x).focus(); });

    pLoadingSetup(true);
});

$("#btnAddNew").on("click", function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddEdit").show();
    $("#divRecords").hide();
    ClearFormFields();

    $("#divCardTitle").html("<i class='fas fa-plus-square align-middle me-1'></i>Add New Employee");

    return false;
});
$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divAddEdit").hide();
    $("#divRecords").show();

    getRecordList();
});

$("#btnRefresh").on('click', function () {
    getRecordList();
    return false;
});

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
    let isValid = true; // Flag to track overall validity

    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid'); // Remove previous invalid class

    // Collect data from input fields
    var EmpData = new Object();

    EmpData.EmployeeID = 0;
    if (this.id == "btnUpdate" && $("#hdnEmployeeID").val() > 0) EmpData.EmployeeID = $("#hdnEmployeeID").val();

    //Basic Details
    EmpData.EmployeeCode = $('#txtEmployeeID').val();
    EmpData.EmployeeTypeID = $('#ddlEmployeeType').val();
    EmpData.Gender = $('#ddlGender').val();
    EmpData.IsActive = $("#chkStatus").is(':checked') ? true : false;
    EmpData.FirstName = $('#txtFirstName').val();
    EmpData.MiddleName = $('#txtMiddleName').val();
    EmpData.SurName = $('#txtSurName').val();
    EmpData.DesignationID = $("#ddlDesignationList").val(); //Added on 

    //Communication Details
    EmpData.TelNo1 = $('#txtTelNo1').val();
    EmpData.TelNo2 = $('#txtTelNo2').val();
    EmpData.MobileNo = $('#txtMobileNo').val();
    EmpData.Email = $('#txtEmail').val();
    EmpData.AddressLine1 = $('#txtAddressLine1').val();
    EmpData.AddressLine2 = $('#txtAddressLine2').val();
    EmpData.City = $('#txtCity').val();
    EmpData.State = $('#txtState').val();
    EmpData.Zipcode = $('#txtPincode').val();
    EmpData.PlaceofBirth = $('#txtPlaceofBirth').val();
    EmpData.EmergencyRelationshipContactID = $('#ddlERelationShip').val();
    EmpData.EmergencyContactName = $('#txtEContactPerson').val();
    EmpData.EmergencyContactNo = $('#txtEPhoneNo').val();

    //Personal Details
    EmpData.sDOB = $('#txtDoBDate').val();
    EmpData.FatherName = $('#txtFatherName').val();
    EmpData.sFatherDOB = $('#txtFatherDate').val();
    EmpData.MotherName = $('#txtMotherName').val();
    EmpData.sMotherDOB = $('#txtMothertDate').val();
    EmpData.MaritalStatus = $('#ddlMartialStatus').val();
    EmpData.SpouseName = $('#txtSpouseName').val();
    EmpData.sSpouseDOB = $('#txtSpouseDate').val();
    EmpData.sAnniversaryDate = $('#txtAnnuDate').val();
    EmpData.ChildOneName = $('#txtChildOneName').val();
    EmpData.sChildOneDOB = $('#txtChild1Date').val();
    EmpData.ChildTwoName = $('#txtChildTwoName').val();
    EmpData.sChildTwoDOB = $('#txtChild2Date').val();

    EmpData.BloodGroupID = $("#ddlBloodGroupList").val();

    if (!EmpData.EmployeeCode) return markInvalid("#txtEmployeeID", "Please enter Employee ID");
    if (!EmpData.EmployeeTypeID || EmpData.EmployeeTypeID == 0) return markInvalid("#ddlEmployeeType", "Please select Employee Type");
    if (!EmpData.Gender || EmpData.Gender == 0) return markInvalid("#ddlGender", "Please select Gender");
    if (!EmpData.FirstName) return markInvalid("#txtFirstName", "Please enter First Name");
    if (!EmpData.SurName) return markInvalid("#txtSurName", "Please enter Surname");
    if (!EmpData.MobileNo) return markInvalid("#txtMobileNo", "Please enter Mobile Number");
    if (!EmpData.AddressLine1) return markInvalid("#txtAddressLine1", "Please enter Address Line 1");
    if (!EmpData.sDOB) return markInvalid("#txtDoBDate", "Please Select Your Date of Birth");

    if (isValid) {
        SaveandUpdateEmployee(EmpData);
    }

    return false;
});

function SaveandUpdateEmployee(EmpData) {
    if (ENABLE_VERBOSE_Logging) //console.log(EmpData);

        $.ajax({
            url: SaveUpdateDataUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(EmpData),
            success: function (response) {
                if (ENABLE_VERBOSE_Logging) //console.log(response);
                    if (response != null && response != null) {
                        if (response.resultdata.Success && !response.resultdata.Error) {
                            Swal.fire({
                                title: EmpData.EmployeeID == 0 ? "Saved!" : "Updated!",
                                text: EmpData.EmployeeID == 0 ? SaveSuccessMessage : UpdateSuccessMessage,
                                icon: "success"
                            }).then(() => {
                                //$('#divAddEdit').hide();
                                //$("#btnRefresh").click();
                                 $("#btnClose").click();
                            });
                        }
                        else if (!response.resultdata.Success && !response.resultdata.Error) {
                            Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
                        }
                        else if (!response.resultdata.Success && response.resultdata.Error) {
                            Swal.fire({ title: "Error", text: response.resultdata.Message, icon: "error", confirmButtonColor: "#556ee6" });
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

function ClearFormFields() {
    $("#divAddEdit .card-body :input").attr("disabled", false);
    $("#hdnEmployeeID").val(0);
    $("#txtEmployeeID").val("");
    $("#ddlEmployeeType").val("0").change();
    $("#ddlGender").val("0").change();
    $("#chkStatus").prop("checked", true);
    $("#txtFirstName").val("");
    $("#txtMiddleName").val("");
    $("#txtSurName").val("");
    $("#ddlDesignationList").val("0").change();

    $("#txtTelNo1").val("");
    $("#txtTelNo2").val("");
    $("#txtMobileNo").val("");
    $("#txtEmail").val("");
    $("#txtAddressLine1").val("");
    $("#txtAddressLine2").val("");
    $("#txtCity").val("");
    $("#txtState").val("");
    $("#txtPincode").val("");
    $("#txtPlaceofBirth").val("");
    $("#ddlERelationShip").val("0").change();
    $("#txtEContactPerson").val("");
    $("#txtEPhoneNo").val("");

    $("#txtDoBDate").val("");
    $("#txtFatherName").val("");
    $("#txtFatherDate").val("");
    $("#txtMotherName").val("");
    $("#txtMothertDate").val("");
    $("#ddlMartialStatus").val("0").change();
    $("#txtSpouseName").val("");
    $("#txtSpouseDate").val("");
    $("#txtAnnuDate").val("");
    $("#txtChildOneName").val("");
    $("#txtChild1Date").val("");
    $("#txtChildTwoName").val("");
    $("#txtChild2Date").val("");

    $("#ddlBloodGroupList").val("0").change();
    $("#btnSave").show();
    $("#btnUpdate").hide();

    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

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
        //"ajax": {
        //    url: ListDataUrl,
        //    "type": "GET",
        //    "data": function (d) {
        //        // d.search.value = $('#tblrecordlist_filter input').val();  // Make sure the search value is passed
        //        // Pass additional parameters if needed
        //        return $.extend({}, d, {
        //            // Custom parameters here (if any)
        //        });
        //    }
        //},
        "ajax": {
            url: EmployeeDataTableUrl,
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
            { "data": "EmployeeCode", "orderable": true, "width": "5%", },
            {
                "data": "EmployeeID", "orderable": true, "width": "5%", render: function (data, type, row) {
                    return `
                    <div class="avatar-xs img-fluid rounded-circle">
                        <img src="../../images/users/avatar-1.jpg" alt="" class="member-img img-fluid d-block rounded-circle">
                    </div>
                `;
                },
            },
            {
                "data": "FirstName", "orderable": true,
                render: function (data, type, row) {
                    return `
                    <div>
                        <strong>${row.Gender == 1 ? `<i class="fas fa-user me-1 text-primary"></i>` : `<i class="fas fa-user me-1 text-pink"></i>`} <a href="#" class="text-black" data-bs-toggle="tooltip" data-bs-placement="bottom" data-bs-original-title="Click here to View/Edit Profile"> ${row.FirstName} </a></strong>
                        <br>
                        <span style="font-size: smaller; color: gray;">${row.Email}</span>
                    </div>
                `;
                },
            },
            { "data": "City", "orderable": true },
            { "data": "DesignationName", "orderable": true },
            { "data": "MobileNo", "orderable": true, "width": "5%", },
            {
                "data": "IsActive",
                "className": "text-center",
                "render": function (data, type, row) {
                    return SetStatus(data);
                },
                "width": "5%",
                "orderable": false
            },
            {
                data: null,
                bSortable: false,
                "className": "text-center",
                render: function (data, type, row) {
                    return SetActionButtons(data.EmployeeID, _CMPermissions);
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
    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { EmployeeID: ID },
        success: function (response) {
            if (response.success) {
                if (ViewFlag) {
                    $("#divAddEdit .card-body :input").attr("disabled", true);
                    $("#divCardTitle").html("<i class='fas fa-eye align-middle me-1'></i>View Employee");
                    $("#btnSave").hide();
                    $("#btnUpdate").hide();

                    $("#btnCloseWindow,#btnClose").attr("disabled", false);
                }
                else {
                    $("#divCardTitle").html("<i class='fas fa-edit  me-1'></i>Edit Employee");
                    $("#btnSave").hide();
                    $("#btnUpdate").show();
                }

                $("#divAddEdit").show();
                $("#divRecords").hide();

                var EmpData = response.data;
                $("#hdnEmployeeID").val(EmpData.EmployeeID);
                $("#txtEmployeeID").val(EmpData.EmployeeCode);
                $("#ddlEmployeeType").val(EmpData.EmployeeTypeID).change();
                $("#ddlGender").val(EmpData.Gender).change();
                $("#chkStatus").prop("checked", EmpData.IsActive);
                $("#txtFirstName").val(EmpData.FirstName);
                $("#txtMiddleName").val(EmpData.MiddleName);
                $("#txtSurName").val(EmpData.SurName);
                $("#ddlDesignationList").val(EmpData.DesignationID).change();

                $("#txtTelNo1").val(EmpData.TelNo1);
                $("#txtTelNo2").val(EmpData.TelNo2);
                $("#txtMobileNo").val(EmpData.MobileNo);
                $("#txtEmail").val(EmpData.Email);
                $("#txtAddressLine1").val(EmpData.AddressLine1);
                $("#txtAddressLine2").val(EmpData.AddressLine2);
                $("#txtCity").val(EmpData.City);
                $("#txtState").val(EmpData.State);
                $("#txtPincode").val(EmpData.Zipcode);
                $("#txtPlaceofBirth").val(EmpData.PlaceofBirth);
                $("#ddlERelationShip").val(EmpData.EmergencyRelationshipContactID).change();
                $("#txtEContactPerson").val(EmpData.EmergencyContactName);
                $("#txtEPhoneNo").val(EmpData.EmergencyContactNo);

                $("#txtDoBDate").val(EmpData.sDOB);
                $("#txtFatherName").val(EmpData.FatherName);
                $("#txtFatherDate").val(EmpData.fDOB);
                $("#txtMotherName").val(EmpData.MotherName);
                $("#txtMothertDate").val(EmpData.mDOB);
                $("#ddlMartialStatus").val(EmpData.MaritalStatus).change();
                $("#txtSpouseName").val(EmpData.SpouseName);
                $("#txtSpouseDate").val(EmpData.SpDOB);
                $("#txtAnnuDate").val(EmpData.ADOB);
                $("#txtChildOneName").val(EmpData.ChildOneName);
                $("#txtChild1tDate").val(EmpData.c1DOB);
                $("#txtChildTwoName").val(EmpData.ChildTwoName);
                $("#txtChild2Date").val(EmpData.c2DOB);
                $("#ddlBloodGroupList").val(EmpData.BloodGroupID).change();

                $("#divRecordLog").show();
                $("#spnLastUpdatedBy").html("Last Updated By: " + EmpData.LastUpdatedByName);
                $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(EmpData.LastUpdatedDateIST));
            }
            else if (!response.success) {
                Swal.fire({ title: "Error", text: response.resultdata.Message, icon: "error", confirmButtonColor: "#556ee6" });
            }
        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(response);
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
