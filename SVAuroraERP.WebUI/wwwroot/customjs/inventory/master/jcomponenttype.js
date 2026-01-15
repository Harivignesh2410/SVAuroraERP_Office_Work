var PageTitle = "Component Type";
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
    $("#hdnSizeID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#txtComponentTypeCode").val("");
    $("#txtComponentTypeName").val("");
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
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    var ComponentData = new Object();

    ComponentData.ComponentTypeID = 0;
    if (this.id == "btnUpdate" && $("#hdnComponentTypeID").val() > 0) ComponentData.ComponentTypeID = $("#hdnComponentTypeID").val();

    ComponentData.ComponentTypeCode = $('#txtComponentTypeCode ').val();
    ComponentData.ComponentTypeName = $('#txtComponentTypeName').val();
    ComponentData.IsActive = $("#chkActive").is(':checked') ? true : false;

    //Size Name
    if (!ComponentData.ComponentTypeName) return markInvalid("#txtComponentTypeName", "Please enter Component Name"); 
    if (!ComponentData.ComponentTypeCode) return markInvalid("#txtComponentTypeCode", "Please enter Componenet Code"); 
    
    SaveandUpdateSize(ComponentData);

    return false;
});

function SaveandUpdateSize(ComponentData) {
    if (ENABLE_VERBOSE_Logging) //console.log(ComponentData);
    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(ComponentData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);
            if (response != null && response.resultdata != null) {
                if (response.resultdata.Success == true && response.resultdata.Error == false) {
                    if (response.resultdata.ID == 0)
                        Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                    else if (response.resultdata.ID > 0)
                        Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                    $('#divAddEditModal').modal('hide');
                    $("#btnRefresh").click();
                }
                else if (response.resultdata.Success == false && response.resultdata.Error == true && response.resultdata.Message === "Data Already Exitst") {
                    Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
                }
                else if (!response.resultdata.success && response.resultdata.Error) {
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

function getRecordList1() {
    
    if ($.fn.DataTable.isDataTable('#tblrecordlist')) {
        $('#tblrecordlist').DataTable().clear().destroy(); 
    }

    $('#tblrecordlist').DataTable({
        "processing": true,
        "serverSide": true,
        "ordering": true,  
        "ajax": {
            url: ListDataUrl,
            "type": "GET",
            "data": function (d) {
               
                return $.extend({}, d, {
                    // Custom parameters here (if any)
                });
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
            { "data": "ComponentTypeCode", "orderable": true, "width": "10%" },
            { "data": "ComponentTypeName", "orderable": true },
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
                    return SetActionButtons(data.ComponentTypeID, _CMPermissions); 
                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}
function getRecordList() {

    if ($.fn.DataTable.isDataTable('#tblrecordlist')) {
        $('#tblrecordlist').DataTable().clear().destroy();
    }

    $('#tblrecordlist').DataTable({
        "processing": true,
        "serverSide": true,
        "ordering": true,
        //"ajax": {
        //    url: ListDataUrl,
        //    "type": "GET",
        //    "data": function (d) {

        //        return $.extend({}, d, {
        //            // Custom parameters here (if any)
        //        });
        //    }
        //},
        "ajax": {
            url: DataTableUrl,
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
            { "data": "ComponentTypeCode", "orderable": true, "width": "10%" },
            { "data": "ComponentTypeName", "orderable": true },
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
                    return SetActionButtons(data.ComponentTypeID, _CMPermissions);
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
            var Componentdata = response.dataResponse.Value;
            $("#hdnComponentTypeID").val(Componentdata.ComponentTypeID);
            $("#txtComponentTypeCode").val(Componentdata.ComponentTypeCode);
            $("#txtComponentTypeName").val(Componentdata.ComponentTypeName);
            $("#chkActive").prop('checked', Componentdata.IsActive);


            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + Componentdata.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(Componentdata.LastUpdatedDate));

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