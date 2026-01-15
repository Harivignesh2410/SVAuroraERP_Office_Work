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
    pLoadingSetup(true);
});

$("#btnAddNew").on('click', function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-WareHouse-20 align-middle me-1'></i>&nbsp;Add New WareHouse");
    ClearFormFields();

    return false;
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnWareHouseID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#txtWareHouseCode").val("");
    $("#txtWareHouseName").val("");
    $("#chkActive").prop("checked", true);

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

    return false;
}
$('#btnRefresh').on('click', function () {
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
    var WareHouseData = new Object();

    WareHouseData.WareHouseID = 0;
    if (this.id == "btnUpdate" && $("#hdnWareHouseID").val() > 0) WareHouseData.WareHouseID = $("#hdnWareHouseID").val();

    WareHouseData.WareHouseName = $('#txtWareHouseName ').val();
    WareHouseData.WareHouseCode = $('#txtWareHouseCode').val();
    WareHouseData.IsActive = $("#chkActive").is(':checked') ? true : false;

    //WareHouse Name
    if (!WareHouseData.WareHouseCode) return markInvalid("#txtWareHouseCode", "Please enter WareHouse Code");
    if (!WareHouseData.WareHouseName) return markInvalid("#txtWareHouseName", "Please enter WareHouse Name");
    // If validation fails, keep focus on the first invalid input
    if (!isValid) return;

    SaveandUpdateWareHouse(WareHouseData);

    return false;
});

function SaveandUpdateWareHouse(WareHouseData) {
    if (ENABLE_VERBOSE_Logging) console.log(WareHouseData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(WareHouseData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
            if (response != null && response != null) {
                if (response.Success && !response.Error)  {
                        Swal.fire({
                            title: WareHouseData.WareHouseID == 0 ? "Saved!" : "Updated!",
                            text: WareHouseData.WareHouseID == 0 ? SaveSuccessMessage : UpdateSuccessMessage,
                            icon: "success"
                        }).then(() => {
                            $('#divAddEditModal').modal('hide');
                            $("#btnRefresh").click();// Refresh the DataTable
                        });
                }
                else if (!response.Success && response.Error) {
                    Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
                }
                else if (!response.Success && !response.Error) {
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
            url: WareHouseDataTableUrl,
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
            { "data": "WareHouseCode", "orderable": true, "width": "10%" },
            { "data": "WareHouseName", "orderable": true },
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
                    return SetActionButtons(data.WareHouseID, _CMPermissions);
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
        data: { ID: ID },
        success: function (response) {
            if (ViewFlag) {
                $("#btnSave").hide();
                $("#btnUpdate").hide();

                $("#divAddEditModal .modal-body :input").attr("disabled", true);
                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View WareHouse");
            }
            else {
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-WareHouse-20 align-middle me-1'></i>&nbsp;Edit WareHouse");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }
            $('#divAddEditModal').modal('show');
            var taxdata = response.Value;
            $("#hdnWareHouseID").val(taxdata.WareHouseID);
            $("#txtWareHouseCode").val(taxdata.WareHouseCode);
            $("#txtWareHouseName").val(taxdata.WareHouseName);
            $("#chkActive").prop('checked', taxdata.IsActive);


            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + taxdata.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(taxdata.LastUpdatedDateIST));

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
            if (response.Success && !response.Error) {
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