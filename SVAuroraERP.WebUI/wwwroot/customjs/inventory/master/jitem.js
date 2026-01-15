var PageTitle = "Item";
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

$('#btnRefresh').on('click', function () {
    getRecordList();
    return false;
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnItemID").val(0);

    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#txtItemCode").val("");
    $("#txtHSNCode").val("");
    $("#txtItemName").val("");
    $("#txtDescription").val("");
    $("#txtPrice").val("");
    $("#ddlUnit").val("0").change();
    $("#ddlComponentType").val("0").change();
    $("#chkActive").prop("checked", true);
    $("#chkStock").prop("checked", true);

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

    //Added on 2025.01.05 by Sivakumar
    $("#ddlItemCategory").val("0").change();
    $("#ddlColor").val("0").change();
    $("#ddlSize").val("0").change();

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
    var ItemData = new Object();

    ItemData.ItemID = 0;
    if (this.id == "btnUpdate" && $("#hdnItemID").val() > 0) ItemData.ItemID = $("#hdnItemID").val();

    ItemData.ItemCode = $("#txtItemCode").val();
    ItemData.HSNCode = $('#txtHSNCode').val();
    ItemData.ItemName = $('#txtItemName').val();
    ItemData.Description = $('#txtDescription').val();
    ItemData.Price = $('#txtPrice').val();
    ItemData.UnitID = $('#ddlUnit').val();
    ItemData.IsActive = $("#chkActive").is(':checked') ? true : false;
    ItemData.IsStockRequired = $("#chkStock").is(':checked') ? true : false;

    //Added on 2025.01.05 by Sivakumar
    ItemData.ItemCategoryID = $('#ddlItemCategory').val();
    ItemData.ColorID = $('#ddlColor').val();
    ItemData.SizeID = $('#ddlSize').val();
    ItemData.ComponentTypeID = $('#ddlComponentType').val();

    if (!ItemData.ItemCode) return markInvalid("#txtItemCode", "Please enter Item Code"); 

    if (!ItemData.HSNCode) return markInvalid("#txtHSNCode", "Please enter HSN Code"); 
    if (!ItemData.Price) return markInvalid("#txtPrice", "Please enter Item Price"); 

    if (!ItemData.ItemCategoryID || ItemData.ItemCategoryID == 0) return markInvalid("#ddlItemCategory", "Please select Category"); 
    if (!ItemData.ComponentTypeID || ItemData.ComponentTypeID == 0) return markInvalid("#ddlComponentType", "Please Select Component Type"); 
    if (!ItemData.ItemName) return markInvalid("#txtItemName", "Please enter Item Name"); 

    if (!ItemData.UnitID || ItemData.UnitID == 0) return markInvalid("#ddlUnit", "Please select Unit"); 

    if (!ItemData.ColorID || ItemData.ColorID == 0) return markInvalid("#ddlColor", "Please select Color"); 

    if (!ItemData.SizeID || ItemData.SizeID == 0) return markInvalid("#ddlSize", "Please Select Size"); 


    SaveandUpdateItem(ItemData);

    return false;
});
function SaveandUpdateItem(ItemData) {
    if (ENABLE_VERBOSE_Logging) ////console.log(ItemData);

        $.ajax({
            url: SaveUpdateDataUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(ItemData),
            success: function (response) {
                if (ENABLE_VERBOSE_Logging) //console.log(response);

                    if (response.Success && !response.Error) {
                        if (ItemData.ItemID == 0)
                            Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                        else if (ItemData.ItemID > 0)
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
        //        // d.search.value = $('#tblrecordlist_filter input').val();  // Make sure the search value is passed
        //        // Pass additional parameters if needed
        //        return $.extend({}, d, {
        //            // Custom parameters here (if any)
        //        });
        //    }
        //},
        "ajax": {
            url: ItemDataTableUrl,
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
            { "data": "ItemCode", "orderable": true, "width": "5%" },
            { "data": "HSNCode", "orderable": true, "width": "5%" },
            { "data": "ItemCategoryName", "orderable": true, "width": "10%" },
            { "data": "ItemName", "orderable": true },
            {
                "data": "Price", "orderable": true, "width": "10%", "className": "text-end",
                "render": function (data, type, row) {
                    return `<i class="bx bx-rupee text-pink font-size-16"></i>${row.Price.toFixed(2)}`
                },
            },
            { "data": "UnitName", "orderable": true, "width": "5%" },
            { "data": "ColorName", "orderable": true, "width": "5%" },
            { "data": "SizeName", "orderable": true, "width": "5%" },
            { "data": "ComponentTypeName", "orderable": true, "width": "5%" },
            {
                "data": "IsStockRequired",
                "render": function (data, type, row) {
                    if (data) {
                        return '<span class="badge bg-success">Stock Required</span>';
                    } else {
                        return '<span class="badge bg-warning">Not Required</span>';
                    }
                },
                "width": "10%",
                "className": "text-center",
                "orderable": false
            },
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
                    return SetActionButtons(data.ItemID, _CMPermissions); 
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
    if ((!_CMActionView && ViewFlag) || (!_CMActionUpdate && !ViewFlag)) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }

    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
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
            var Itemdata = response.Value;

            $("#hdnItemID").val(Itemdata.ItemID);
            $("#txtItemCode").val(Itemdata.ItemCode);
            $("#txtHSNCode").val(Itemdata.HSNCode);
            $("#txtItemName").val(Itemdata.ItemName);
            $("#txtDescription").val(Itemdata.Description);
            $("#txtPrice").val(Itemdata.Price);
            $("#ddlUnit").val(Itemdata.UnitID).change();
            $("#chkStock").prop('checked', Itemdata.IsStockRequired);
            $("#chkActive").prop('checked', Itemdata.IsActive);

            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + Itemdata.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(Itemdata.LastUpdatedDateIST));

            //Added on 2025.01.05 by Sivakumar
            $("#ddlItemCategory").val(Itemdata.ItemCategoryID).change();
            $("#ddlColor").val(Itemdata.ColorID).change();
            $("#ddlSize").val(Itemdata.SizeID).change();
            $("#ddlComponentType").val(Itemdata.ComponentTypeID).change();

        },
        error: function (xhr, status, error) {
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
    $('#ddlItemCategory').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlUnit').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlColor').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlSize').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlComponentType').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
});