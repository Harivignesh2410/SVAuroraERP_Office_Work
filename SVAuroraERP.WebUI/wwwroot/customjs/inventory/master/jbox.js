var PageTitle = "Box";
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
    $('#divAddEditModal').modal('show');
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New " + PageTitle);
    ClearFormFields();

    return false;
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnBoxID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#ddlSize").val("0").change();
    $("#txtBoxName").val("");
    $("#txtMaxPercentage").val("");
    $("#txtInnerBoxCount").val("");
    $("#txtInnerBoxQuantity").val("");
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

    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid'); // Remove previous invalid class

    // Collect data from input fields
    var BoxData = new Object();
    var boxcapacity = 0;
    BoxData.BoxID = 0;
    if (this.id == "btnUpdate" && $("#hdnBoxID").val() > 0) BoxData.BoxID = $("#hdnBoxID").val();

    BoxData.SizeID = $('#ddlSize').val();
    BoxData.BoxName = $('#txtBoxName ').val();
    BoxData.MaxCapacity = $('#txtMaxPercentage').val();
    BoxData.IsActive = $("#chkActive").is(':checked') ? true : false;
    BoxData.InnerBoxCount = $('#txtInnerBoxCount').val();
    BoxData.InnerBoxQuantity = $('#txtInnerBoxQuantity').val();

    boxcapacity = BoxData.InnerBoxCount * BoxData.InnerBoxQuantity;
    if (boxcapacity > BoxData.MaxCapacity) {
        $.jGrowl("Inner Box Count and Quantity are Not Match with Max Capacity !!", { sticky: false, theme: 'warning', life: jGrowlLife });
        return false;
    }
    if (BoxData.SizeID == 0) return markInvalid("#ddlSize", "Please Select Size");
    if (!BoxData.BoxName) return markInvalid("#txtBoxName", "Please enter Box Name"); 
    if (!BoxData.MaxCapacity) return markInvalid("#txtMaxPercentage", "Please Enter Max Percentage"); 
    if (!BoxData.InnerBoxCount) return markInvalid("#txtInnerBoxCount", "Please Enter Inner Box Count"); 
    if (!BoxData.InnerBoxQuantity) return markInvalid("#txtInnerBoxQuantity", "Please Enter Inner Box Quantity"); 
    

     SaveandUpdateBox(BoxData);

    return false;
});
function SaveandUpdateBox(BoxData) {
    if (ENABLE_VERBOSE_Logging) console.log(BoxData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(BoxData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
            if (response != null && response.result != null) {
                if (response.result.Success && !response.result.Error) {
                    Swal.fire({
                        title: BoxData.BoxID == 0 ? "Saved!" : "Updated!",
                        text: BoxData.BoxID == 0 ? SaveSuccessMessage : UpdateSuccessMessage,
                        icon: "success"
                    }).then(() => {
                        $('#divAddEditModal').modal('hide');
                        $("#btnRefresh").click();// Refresh the DataTable
                    });
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
                "orderable": false,
                "width": "5%",
            },
            { "data": "BoxName", "orderable": true, "width": "10%" },
            { "data": "SizeName", "orderable": true },
            { "data": "MaxCapacity", "orderable": true },
            { "data": "InnerBoxCount", "orderable": true },
            { "data": "InnerBoxQuantity", "orderable": true },
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
                    return SetActionButtons(data.BoxID, _CMPermissions);
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
                $("#divAddEditModal .modal-body :input").attr("disabled", true);
                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View " + PageTitle);
                $("#btnSave").hide();
                $("#btnUpdate").hide();
            }
            else {
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit " + PageTitle);
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }
            $('#divAddEditModal').modal('show');
            var Boxdata = response;
            $("#ddlSize").val(Boxdata.SizeID).change();
            $("#hdnBoxID").val(Boxdata.BoxID);
            $("#txtMaxPercentage").val(Boxdata.MaxCapacity);
            $("#txtBoxName").val(Boxdata.BoxName);
            $("#chkActive").prop('checked', Boxdata.IsActive);
            $("#txtInnerBoxQuantity").val(Boxdata.InnerBoxQuantity);
            $("#txtInnerBoxCount").val(Boxdata.InnerBoxCount);

            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + Boxdata.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(Boxdata.LastUpdatedDate));

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
$('#divAddEditModal').on('shown.bs.modal', function () {
    $('#ddlSize').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
});