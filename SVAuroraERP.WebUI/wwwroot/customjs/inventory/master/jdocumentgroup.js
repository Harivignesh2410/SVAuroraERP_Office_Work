var PageTitle = "Document Group";
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
    $("#hdnDocumentGroupID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#txtDocumentGroupCode").val("");
    $("#txtDocumentGroupName").val("");
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
    $('.form-control').removeClass('is-invalid'); // Remove previous invalid class

    var DocumentGroupData = new Object();

    DocumentGroupData.DocumentGroupID = 0;
    if (this.id == "btnUpdate" && $("#hdnDocumentGroupID").val() > 0) DocumentGroupData.DocumentGroupID = $("#hdnDocumentGroupID").val();

    DocumentGroupData.DocumentGroupName = $('#txtDocumentGroupName ').val();
    DocumentGroupData.DocumentGroupCode = $('#txtDocumentGroupCode').val();
    DocumentGroupData.IsActive = $("#chkActive").is(':checked') ? true : false;

    //DocumentGroup Name
    if (!DocumentGroupData.DocumentGroupName) return markInvalid("#txtDocumentGroupName", "Please enter DocumentGroup Name"); 
    if (!DocumentGroupData.DocumentGroupCode) return markInvalid("#txtDocumentGroupCode", "Please enter DocumentGroup Code"); 

    SaveandUpdateDocumentGroup(DocumentGroupData);

    return false;
});

function SaveandUpdateDocumentGroup(DocumentGroupData) {
    if (ENABLE_VERBOSE_Logging) //console.log(DocumentGroupData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(DocumentGroupData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);
            if (response != null && response != null) {
                if (response.Success && !response.Error) {
                    if (DocumentGroupData.DocumentGroupID == 0)
                        Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                    else if (DocumentGroupData.DocumentGroupID > 0)
                        Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                    $('#divAddEditModal').modal('hide');
                    $("#btnRefresh").click();
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
        //        // d.search.value = $('#tblrecordlist_filter input').val();  // Make sure the search value is passed
        //        // Pass additional parameters if needed
        //        return $.extend({}, d, {
        //            // Custom parameters here (if any)
        //        });
        //    }
        //},
        "ajax": {
            url: DocumentGroupDataTableUrl,
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
                data: null, 
                render: function (data, type, row, meta) {
                    return meta.settings._iDisplayStart + meta.row + 1; // Display row number (S. No.)
                },
                orderable: false, 
                "width": "5%",
                "orderable": false
            },
            { "data": "DocumentGroupCode", "orderable": true, "width": "10%" },
            { "data": "DocumentGroupName", "orderable": true },
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
                    return SetActionButtons(data.DocumentGroupID, _CMPermissions);  
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
            var taxdata = response.Value;
            $("#hdnDocumentGroupID").val(taxdata.DocumentGroupID);
            $("#txtDocumentGroupCode").val(taxdata.DocumentGroupCode);
            $("#txtDocumentGroupName").val(taxdata.DocumentGroupName);
            $("#chkActive").prop('checked', taxdata.IsActive);

            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + taxdata.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(taxdata.LastUpdatedDate));

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