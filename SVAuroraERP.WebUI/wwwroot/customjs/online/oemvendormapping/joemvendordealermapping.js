var PageName = "OEM Vendor Dealer Mapping";
$(function () {
    pLoadingSetup(false);
    getRecordList();

    $("#btnSave").show();
    $("#btnUpdate").hide();
    if (!_CMActionAdd) {
        $("#btnAddNew").remove();
        $("#btnSave").remove();
    }
    if (!_CMActionUpdate) $("#btnUpdate").remove();
    pLoadingSetup(true);
});
$("#btnAddNew").on('click', function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp; Add New " + PageName);
    ClearFormFields();

    return false;
});
$('#divAddEditModal').on('shown.bs.modal', function () {
    $('#ddlOEMList').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlDealerList').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlEmbossingStationList').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlOEMVendorNameList').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnOEMVendorDealerMappingID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#ddlOEMList").val(0);
    $("#ddlDealerList").val(0);
    $("#ddlEmbossingStationList").val(0);
    $("#ddlOEMVendorNameList").val(0);
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

    let isValid = true;
    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    var OEMVendorDealerMappingData = new Object();

    OEMVendorDealerMappingData.OEMVendorDealerMappingID = 0;
    if (this.id == "btnUpdate" && $("#hdnOEMVendorDealerMappingID").val() > 0) OEMVendorDealerMappingData.OEMVendorDealerMappingID = $("#hdnOEMVendorDealerMappingID").val();
    OEMVendorDealerMappingData.DealerID = $('#ddlDealerList').val();
    OEMVendorDealerMappingData.EmbossingStationID = $('#ddlEmbossingStationList').val();
    OEMVendorDealerMappingData.OEMVendorCodeMappingID = $('#ddlOEMVendorNameList').val();

    OEMVendorDealerMappingData.IsActive = $("#chkActive").is(':checked') ? true : false;
    if (!OEMVendorDealerMappingData.DealerID || OEMVendorDealerMappingData.DealerID === "0") return markInvalid("#ddlDealerList", " Please Select Dealer");
    if (!OEMVendorDealerMappingData.EmbossingStationID || OEMVendorDealerMappingData.EmbossingStationID === "0") return markInvalid("#ddlEmbossingStationList", " Please Select Embossing Station");
    if (!OEMVendorDealerMappingData.OEMVendorCodeMappingID || OEMVendorDealerMappingData.OEMVendorCodeMappingID === "0") return markInvalid("#ddlOEMVendorNameList", " Please Select OEM Vendor Code Mapping");
    if (!isValid) return;

    SaveandUpdate(OEMVendorDealerMappingData);

    return false;
});
function SaveandUpdate(OEMVendorDealerMappingData) {
    if (ENABLE_VERBOSE_Logging)

        $.ajax({
            url: SaveUpdateDataUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(OEMVendorDealerMappingData),
            success: function (response) {
                if (ENABLE_VERBOSE_Logging) //console.log(response);

                    if (response.Success && !response.Error) {
                        if (OEMVendorDealerMappingData.OEMVendorDealerMappingID == 0)
                            Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                        else if (OEMVendorDealerMappingData.OEMVendorDealerMappingID > 0)
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
            { "data": "DealerName", "orderable": true },
            { "data": "OEMName", "orderable": true },
            { "data": "EmbossingStationName", "orderable": true, "width": "10%" },
            { "data": "VendorCode", "orderable": true, "width": "10%" },
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
                    return SetActionButtons(row.OEMVendorDealerMappingID, _CMPermissions);

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
                $('#divAddEditModal').modal('show');
                $("#divAddEditModal .modal-body :input").attr("disabled", true);
                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View " + PageName);
            }
            else {
                $('#divAddEditModal').modal('show');
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit " + PageName);
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }
            var OEMVendorDealerMappingData = response.Value;
            $("#hdnOEMVendorDealerMappingID").val(OEMVendorDealerMappingData.OEMVendorDealerMappingID);
            $("#ddlOEMList").val(OEMVendorDealerMappingData.HSRPOEMID).change();
            $("#ddlOEMVendorNameList").val(OEMVendorDealerMappingData.OEMVendorCodeMappingID).change();
            $("#ddlDealerList").val(OEMVendorDealerMappingData.DealerID).change();
            $("#ddlEmbossingStationList").val(OEMVendorDealerMappingData.EmbossingStationID).change();
            $("#chkActive").prop("checked", OEMVendorDealerMappingData.IsActive);



            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + OEMVendorDealerMappingData.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(OEMVendorDealerMappingData.LastUpdatedDate));

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
$('#ddlOEMList').on('change', function () {
    $('#ddlDealerList').empty();
    var OEMID = $(this).val();

    GetDealerList(OEMID);
});
function GetDealerList(OEMID) {
    if (OEMID > 0) {
        $.ajax({
            url: DealerListByOEMIDUrl,
            type: 'get',
            headers: { "RequestVerificationToken": _TOKEN },
            data: { OEMID: OEMID },
            async: false,
            success: function (data) {
                $('#ddlDealerList').empty();
                $("#ddlDealerList").append("<option value='0'>--Select Dealer--</option>");
                $.each(data.result.Value, function (i, result) {
                    $('#ddlDealerList').append('<option value="' + result.HSRPUserID + '">' + result.CompanyName + '</option>');
                });

                $("#ddlDealerList").val(0).change();
            }
        });
    } else {
        $('#ddlDealerList').empty();
        $('#ddlDealerList').append('<option value="0" disabled selected>--Select Dealer--</option>');
    }
}
$('#ddlOEMList').on('change', function () {
    $('#ddlOEMVendorNameList').empty();
    var OEMNameID = $(this).val();

    GetOEMVendorNameList(OEMNameID);
});
function GetOEMVendorNameList(OEMNameID) {
    if (OEMNameID > 0) {
        $.ajax({
            url: VendorListByOEMIDUrl,
            type: 'get',
            headers: { "RequestVerificationToken": _TOKEN },
            data: { ESID: OEMNameID },
            async: false,
            success: function (data) {
                $('#ddlOEMVendorNameList').empty();

                // Correct placeholder option
                $("#ddlOEMVendorNameList").append("<option value='0'>--Select Vendor Code--</option>");

                // Populate list
                $.each(data.result.Value, function (i, result) {
                    $('#ddlOEMVendorNameList').append(
                        '<option value="' + result.OEMVendorCodeMappingID + '">'
                        + result.VendorCode + ' - '
                        + result.DistrictName + ', '
                        + result.StateName + '</option>'
                    );
                });

                // Reset to default
                $("#ddlOEMVendorNameList").val(0).change();
            }
        });
    } else {
        $('#ddlOEMVendorNameList').empty();
        $('#ddlOEMVendorNameList').append('<option value="0" disabled selected>--Select Vendor Code--</option>');
    }
}