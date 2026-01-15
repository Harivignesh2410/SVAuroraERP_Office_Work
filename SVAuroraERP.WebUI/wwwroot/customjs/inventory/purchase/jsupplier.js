var arraySupplierData = [];
$(function () {
    pLoadingSetup(false);

    getRecordList();

    $("#btnSave").hide();
    $("#btnClose").show();

    $("#btnUpdate").hide();

    //// Set default visibility on page load
    $("#divAddEdit").hide();  // Hide the add/edit section
    $("#divRecords").show();  // Show the records section

    pLoadingSetup(true);
});
$("#btnAddNew").on("click", function () {
    $("#divAddEdit").show();
    $("#divRecords").hide();

    ClearFormFields();

    $("#divCardTitle").html("<i class='fas fa-plus-square align-middle me-1'></i>Add New Supplier");

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
function ClearFormFields() {
    $("#divAddEdit .card-body :input").attr("disabled", false);
    $("#hdnSupplierID").val(0);

    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#txtSupplierCode").val("");
    $("#txtFullName").val("");
    $("#txtGstNo").val("");
    $("#chkStatus").prop("checked", true);
    $("#txtAddressLine1").val("");
    $("#txtAddressLine2").val("");
    $("#txtCity").val("");
    $("#txtState").val("");
    $("#txtCountry").val("");
    $("#txtPincode").val("");
    $("#txtTelNo1").val("");
    $("#txtTelNo2").val("");
    $("#txtMobileNo").val("");
    $("#txtEmail").val("");


    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

    return false;
}
$("#btnSave,#btnUpdate").on('click', function () {
    let isValid = true;

    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    var SupplierData = new Object();
    SupplierData.SupplierID = 0;

    if (this.id == "btnUpdate" && $("#hdnSupplierID").val() > 0)
        SupplierData.SupplierID = $("#hdnSupplierID").val();

    // Collect the form data
    SupplierData.SupplierCode = $('#txtSupplierCode').val();
    SupplierData.SupplierName = $('#txtFullName').val();
    SupplierData.MobileNo = $('#txtMobileNo').val();

    SupplierData.GSTNo = $('#txtGstNo').val();
    SupplierData.AddressLine1 = $('#txtAddressLine1').val();
    SupplierData.AddressLine2 = $('#txtAddressLine2').val();
    SupplierData.City = $('#txtCity').val();
    SupplierData.State = $("#txtState").val();
    SupplierData.Country = $("#txtCountry").val();
    SupplierData.Pincode = $('#txtPincode').val();

    SupplierData.TelNo1 = $('#txtTelNo1').val();
    SupplierData.TelNo2 = $('#txtTelNo2').val();

    SupplierData.Email = $('#txtEmail').val();
    SupplierData.IsActive = $("#chkStatus").is(':checked') ? true : false;
    // Validate empty fields
    if (!SupplierData.SupplierCode) {
        $('#txtSupplierCode').addClass('is-invalid');
        $('#txtSupplierCode').after('<div class="invalid-feedback">Please enter Supplier Code</div>');
        $('#txtSupplierCode').focus();
        isValid = false;
    }
    if (!SupplierData.SupplierName) {
        $('#txtFullName').addClass('is-invalid');
        $('#txtFullName').after('<div class="invalid-feedback">Please enter Supplier Name</div>');
        $('#txtFullName').focus();
        isValid = false;
    }
    if (!SupplierData.MobileNo) {
        $('#txtMobileNo').addClass('is-invalid');
        $('#txtMobileNo').after('<div class="invalid-feedback">Please enter Mobile No</div>');
        $('#txtMobileNo').focus();
        isValid = false;
    }

    if (!isValid) return false;

    SaveandUpdateSupplier(SupplierData);

    return false;
});
function SaveandUpdateSupplier(SupplierData) {
    if (ENABLE_VERBOSE_Logging) //console.log(SupplierData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(SupplierData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.resultdata.Success) {
                if (SupplierData.SupplierID == 0)
                    Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                else if (SupplierData.SupplierID > 0)
                    Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                $('#divAddEdit').hide();
                $("#divRecords").show();
                $("#btnRefresh").click();
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
        //"ajax": {
        //    url: ListDataUrl,
        //    "type": "GET",
        //    "data": function (d) {

        //        arraySupplierData.push(d.columns);
        //        // d.search.value = $('#tblrecordlist_filter input').val();  // Make sure the search value is passed
        //        // Pass additional parameters if needed
        //        return $.extend({}, d, {
        //            // Custom parameters here (if any)
        //        });
        //    }
        //},
        "ajax": {
            url: SupplierDataTableUrl,
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
            { "data": "SupplierCode", "orderable": true, "width": "10%", },
            { "data": "SupplierName", "orderable": true },
            { "data": "GSTNo", "orderable": true, "width": "10%" },
            { "data": "City", "orderable": true, "width": "10%" },
            { "data": "MobileNo", "orderable": true, "width": "10%" },
            { "data": "Email", "orderable": true, "width": "10%" },
            {
                "data": "IsActive",
                "className": "text-center",
                "render": function (data, type, row) {
                    return SetStatus(data);
                },
                "className": "text-center",
                "width": "5%",
                "orderable": false
            },
            {
                data: null,
                bSortable: false,
                "className": "text-center",
                render: function (data, type, row) {
                    return SetAction(row.SupplierID);
                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}

function EditData(id, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) //console.log(id);
    ClearFormFields();

    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            if (ViewFlag) {
                $("#btnSave").hide();
                $("#btnUpdate").hide();

                $("#divAddEdit .card-body :input").attr("disabled", true);
                $("#divAddEdit .card-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Supplier");

                $("#btnCloseWindow,#btnClose").attr("disabled", false);
            }
            else {
                $("#divAddEdit .card-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit New Supplier");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }

            $("#divAddEdit").show();
            $("#divRecords").hide();

            var SupplierData = response.data.Value;

            $("#hdnSupplierID").val(SupplierData.SupplierID);
            $("#txtSupplierCode").val(SupplierData.SupplierCode);
            $("#txtFullName").val(SupplierData.SupplierName);
            $("#txtGstNo").val(SupplierData.GSTNo);
            $("#txtAddressLine1").val(SupplierData.AddressLine1);
            $("#txtAddressLine2").val(SupplierData.AddressLine2);
            $("#txtCity").val(SupplierData.City);
            $("#txtState").val(SupplierData.State);
            $("#txtCountry").val(SupplierData.Country);
            $("#txtPincode").val(SupplierData.Pincode);
            $("#txtTelNo1").val(SupplierData.TelNo1);
            $("#txtTelNo2").val(SupplierData.TelNo2);
            $("#txtMobileNo").val(SupplierData.MobileNo);
            $("#txtEmail").val(SupplierData.Email);

            $("#chkStatus").prop('checked', SupplierData.IsActive);

            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + SupplierData.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(SupplierData.LastUpdatedDateIST));

        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

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
$("#txtSupplierCode, #txtFullName, #txtMobileNo").on("blur", function () {
    let field = $(this).attr('id');
    let value = $(this).val().trim();

    if (value) {
        let supplierData = {
            SupplierID: $("#hdnSupplierID").val() || 0,
            SupplierCode: $("#txtSupplierCode").val(),
            SupplierName: $("#txtFullName").val(),
            MobileNo: $("#txtMobileNo").val()
        };

        $(`#${field}`).removeClass('is-invalid');
        $(`#${field}`).next('.invalid-feedback').remove();

        $.ajax({
            url: CheckDuplicateUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(supplierData),
            success: function (response) {

                if (response.success && response.isDuplicate) {
                    let fieldLabel = '';
                    switch (field) {
                        case 'txtSupplierCode':
                            fieldLabel = 'Supplier Code';
                            break;
                        case 'txtFullName':
                            fieldLabel = 'Supplier Name';
                            break;
                        case 'txtMobileNo':
                            fieldLabel = 'Mobile Number';
                            break;
                    }

                    $(`#${field}`).addClass('is-invalid');
                    $(`#${field}`).after(
                        `<div class="invalid-feedback">${fieldLabel} "${value}" already exists in the database.</div>`
                    );
                    $(`#${field}`).val('');
                    $(`#${field}`).focus();
                }
            },
            error: function (xhr, status, error) {
                Swal.fire({
                    title: "Error",
                    text: "Error checking for duplicate entry: " + error,
                    icon: "error",
                    confirmButtonColor: "#556ee6"
                });
            }
        });
    }
});