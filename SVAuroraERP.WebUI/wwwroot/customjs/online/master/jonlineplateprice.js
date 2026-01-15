$(function () {
    pLoadingSetup(false);
    getRecordList();
    $("#divAddnew").hide();
    $("#btnSave").show();
    $("#btnUpdate").hide();
    if (!_CMActionAdd) {
        $("#btnAddNew").remove();
        $("#btnSave").remove();
    }
    if (!_CMActionUpdate) $("#btnUpdate").remove();
    pLoadingSetup(true);
});
$("#btnAddNew").click(function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddnew .card-title").html("<i class='fas fa-plus-square'></i>&nbsp;&nbsp;Add Online Plate Price");
    $("#tblOnlinePlatePrice").hide();
    $("#divRecordLog").hide();
    $("#divAddnew").show();
    ClearFormFields();
});
$("#btnClose,#btnCloseWindow").click(function () {
    $("#divAddnew").hide();
    $("#tblOnlinePlatePrice").show();
});
$('#divAddnew').on('shown.bs.card', function () {
    $('#ddlVehicleCategory').select2({ dropdownParent: $('#divAddnew'), width: '100%' });
    $('#ddlVehicleType').select2({ dropdownParent: $('#divAddnew'), width: '100%' });
    $('#ddlVehicleClass').select2({ dropdownParent: $('#divAddnew'), width: '100%' });
    $('#ddlFuel').select2({ dropdownParent: $('#divAddnew'), width: '100%' });
    $('#ddlVehiclePlateColor').select2({ dropdownParent: $('#divAddnew'), width: '100%' });
    $('#ddlVehiclePlateType').select2({ dropdownParent: $('#divAddnew'), width: '100%' });
    $('#ddlVehiclePlateSize').select2({ dropdownParent: $('#divAddnew'), width: '100%' });
});
function ClearFormFields() {
    $("#divAddnew .card-body :input").attr("disabled", false);
    $("#hdnOnlinePlatePriceID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#ddlVehicleCategory").val(0).trigger("change");
    $("#ddlVehicleType").val(0).trigger("change");
    $("#ddlVehicleClass").val(0).trigger("change");
    $("#ddlFuel").val(0).trigger("change");
    $("#ddlVehiclePlateColor").val(0).trigger("change");
    $("#ddlVehiclePlateType").val(0).trigger("change");
    $("#ddlVehiclePlateSize").val(0).trigger("change");
    $("#txtFront").val("");
    $("#txtRear").val("");
    $("#txtSnapLock").val("");
    $("#txtTLPSticker").val("");
    $("#txtEmbossingFitmentCharges").val("");
    $("#txtDealerFitmentCharges").val("");
    $("#txtHomeFitmentCharges").val("");
    $("#txtDealerCourierCharge").val("");
    $("#txtDealerLocationChangeCharge").val("");
    $("#txtOtherCharges").val("");
    $("#ChkcheckIsActive").prop("checked", true);

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

    var OnlinePlatePriceData = new Object();

    OnlinePlatePriceData.OnlinePlatePriceID = 0;
    if (this.id == "btnUpdate" && $("#hdnOnlinePlatePriceID").val() > 0) OnlinePlatePriceData.OnlinePlatePriceID = $("#hdnOnlinePlatePriceID").val();
    OnlinePlatePriceData.VehicleCategoryID = $('#ddlVehicleCategory').val();
    OnlinePlatePriceData.VehicleTypeID = $('#ddlVehicleType').val();
    OnlinePlatePriceData.VehicleClassID = $('#ddlVehicleClass').val();
    OnlinePlatePriceData.FuelID = $('#ddlFuel').val();
    OnlinePlatePriceData.VehiclePlateColorID = $('#ddlVehiclePlateColor').val();
    OnlinePlatePriceData.VehiclePlateTypeID = $('#ddlVehiclePlateType').val();
    OnlinePlatePriceData.VehiclePlateSizeID = $('#ddlVehiclePlateSize').val();
    OnlinePlatePriceData.Front = $('#txtFront').val();
    OnlinePlatePriceData.Rear = $('#txtRear').val();
    OnlinePlatePriceData.SnapLock = $('#txtSnapLock').val();
    OnlinePlatePriceData.TLPSticker = $('#txtTLPSticker').val();
    OnlinePlatePriceData.EmbossingFitmentCharges = $('#txtEmbossingFitmentCharges').val();
    OnlinePlatePriceData.DealerFitmentCharges = $('#txtDealerFitmentCharges').val();
    OnlinePlatePriceData.HomeFitmentCharges = $('#txtHomeFitmentCharges').val();
    OnlinePlatePriceData.DealerCourierCharge = $('#txtDealerCourierCharge').val();
    OnlinePlatePriceData.DealerLocationChangeCharge = $('#txtDealerLocationChangeCharge').val();
    OnlinePlatePriceData.OtherCharges = $('#txtOtherCharges').val();
    OnlinePlatePriceData.IsActive = $("#ChkcheckIsActive").is(':checked') ? true : false;
    if (!OnlinePlatePriceData.VehicleCategoryID || OnlinePlatePriceData.VehicleCategoryID === "0") return markInvalid("#ddlVehicleCategory", " Please Select Vehicle Category");
    if (!OnlinePlatePriceData.VehicleTypeID || OnlinePlatePriceData.VehicleTypeID === "0") return markInvalid("#ddlVehicleType", " Please Select Vehicle Type");
    if (!OnlinePlatePriceData.VehicleClassID || OnlinePlatePriceData.VehicleClassID === "0") return markInvalid("#ddlVehicleClass", " Please Select Vehicle Class");
    if (!OnlinePlatePriceData.FuelID || OnlinePlatePriceData.FuelID === "0") return markInvalid("#ddlFuel", " Please Select Fuel Type");
    if (!OnlinePlatePriceData.VehiclePlateColorID || OnlinePlatePriceData.VehiclePlateColorID === "0") return markInvalid("#ddlVehiclePlateColor", " Please Select Vehicle Plate Color");
    if (!OnlinePlatePriceData.VehiclePlateTypeID || OnlinePlatePriceData.VehiclePlateTypeID === "0") return markInvalid("#ddlVehiclePlateType", " Please Select Vehicle Plate Type");
    if (!OnlinePlatePriceData.VehiclePlateSizeID || OnlinePlatePriceData.VehiclePlateSizeID === "0") return markInvalid("#ddlVehiclePlateSize", " Please Select Vehicle Plate Size");
    //if (!OnlinePlatePriceData.Front) return markInvalid("#txtFront", "Please enter Front Plate Charges");
    //if (!OnlinePlatePriceData.Rear) return markInvalid("#txtRear", "Please enter Rear Plate Charges");
    //if (!OnlinePlatePriceData.SnapLock) return markInvalid("#txtSnapLock", "Please enter SnapLock Charges");
    //if (!OnlinePlatePriceData.TLPSticker) return markInvalid("#txtTLPSticker", "Please enter TLPSticker Charges");
    //if (!OnlinePlatePriceData.EmbossingFitmentCharges) return markInvalid("#txtEmbossingFitmentCharges", "Please enter Embossing Fitment Charges");
    //if (!OnlinePlatePriceData.DealerFitmentCharges) return markInvalid("#txtDealerFitmentCharges", "Please enter Dealer Fitment Charges Charges");
    //if (!OnlinePlatePriceData.HomeFitmentCharges) return markInvalid("#txtHomeFitmentCharges", "Please enter HomeFitmentCharges Charges");
    //if (!OnlinePlatePriceData.DealerCourierCharge) return markInvalid("#txtDealerCourierCharge", "Please enter Dealer Courier Charges");
    //if (!OnlinePlatePriceData.DealerLocationChangeCharge) return markInvalid("#txtDealerLocationChangeCharge", "Please enter Dealer Location Change Charges");
    //if (!OnlinePlatePriceData.OtherCharges) return markInvalid("#txtOtherCharges", "Please enter Other Charges");
    if (!isValid) return;

    SaveandUpdate(OnlinePlatePriceData);

    return false;
});
function SaveandUpdate(OnlinePlatePriceData) {
    if (ENABLE_VERBOSE_Logging)

        $.ajax({
            url: SaveUpdateDataUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(OnlinePlatePriceData),
            success: function (response) {
                if (ENABLE_VERBOSE_Logging) //console.log(response);

                    if (response.Success && !response.Error) {
                        if (OnlinePlatePriceData.OnlinePlatePriceID == 0)
                            Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                        else if (OnlinePlatePriceData.OnlinePlatePriceID > 0)
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
            url: OnlinePlatePriceListDataURL,
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
            { "data": "VehicleCategoryName", "orderable": true, "width": "5%" },
            { "data": "VehicleTypeName", "orderable": true },
            { "data": "VehicleClassName", "orderable": true },
            { "data": "FuelName", "orderable": true, "width": "5%" },
            { "data": "VehiclePlateColorName", "orderable": true },
            { "data": "VehiclePlateTypeName", "orderable": true },
            { "data": "VehiclePlateSizeName", "orderable": true },
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
                    return SetActionButtons(row.OnlinePlatePriceID, _CMPermissions);
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
                $("#divAddnew .card-body :input").attr("disabled", true);
                $("#divAddnew .card-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Online Plate Price");
                $("#btnCloseWindow,#btnClose").attr("disabled", false);
                $("#divAddnew").show();
                $("#tblOnlinePlatePrice").hide();
            }
            else {
                $("#divAddnew .card-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Online Plate Price");
                $("#btnSave").hide();
                $("#btnUpdate").show();
                $("#divAddnew .card-body :input").attr("disabled", false);
                $("#divAddnew").show();
                $("#tblOnlinePlatePrice").hide();
            }
            if (response != null && response.Value != null) {
                var result = response;
                if (!result.Error && result.Success && result.ID > 0) {
                    $('#divAddEditModal').modal('show');
                    var OnlinePlatePriceData = response.Value;
                    $("#hdnOnlinePlatePriceID").val(OnlinePlatePriceData.OnlinePlatePriceID);
                    $("#ddlVehicleCategory").val(OnlinePlatePriceData.VehicleCategoryID).change();
                    $("#ddlVehicleType").val(OnlinePlatePriceData.VehicleTypeID).change();
                    $("#ddlVehicleClass").val(OnlinePlatePriceData.VehicleClassID).change();
                    $("#ddlFuel").val(OnlinePlatePriceData.FuelID).change();
                    $("#ddlVehiclePlateColor").val(OnlinePlatePriceData.VehiclePlateColorID).change();
                    $("#ddlVehiclePlateType").val(OnlinePlatePriceData.VehiclePlateTypeID).change();
                    $("#ddlVehiclePlateSize").val(OnlinePlatePriceData.VehiclePlateSizeID).change();
                    $("#txtFront").val(OnlinePlatePriceData.Front);
                    $("#txtRear").val(OnlinePlatePriceData.Rear);
                    $("#txtSnapLock").val(OnlinePlatePriceData.SnapLock);
                    $("#txtTLPSticker").val(OnlinePlatePriceData.TLPSticker);
                    $("#txtEmbossingFitmentCharges").val(OnlinePlatePriceData.EmbossingFitmentCharges);
                    $("#txtDealerFitmentCharges").val(OnlinePlatePriceData.DealerFitmentCharges);
                    $("#txtHomeFitmentCharges").val(OnlinePlatePriceData.HomeFitmentCharges);
                    $("#txtDealerCourierCharge").val(OnlinePlatePriceData.DealerCourierCharge);
                    $("#txtDealerLocationChangeCharge").val(OnlinePlatePriceData.DealerLocationChangeCharge);
                    $("#txtOtherCharges").val(OnlinePlatePriceData.OtherCharges);
                    $("#ChkcheckIsActive").prop('checked', OnlinePlatePriceData.IsActive);


                    $("#divRecordLog").show();
                    $("#spnLastUpdatedBy").html("Last Updated By: " + OnlinePlatePriceData.LastUpdatedByName);
                    $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(OnlinePlatePriceData.LastUpdatedDate));
                }
                else
                    Swal.fire({ title: "Error", text: result.Message, icon: "warning", confirmButtonColor: "#556ee6" });
            }
            else
                Swal.fire({ title: "Error", text: "Something went wrong!", icon: "warning", confirmButtonColor: "#556ee6" });

        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) //console.log(error);

                Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
$('#ddlVehicleClass').on('change', function () {
    $('#ddlVehiclePlateType').empty();

    var VehicleClassID = $(this).val();

    GetVehicleClass(VehicleClassID);
});

function GetVehicleClass(VehicleClassID) {
    if (VehicleClassID > 0) {
        $.ajax({
            url: LoadPlateTypeByVehicleClassIDUrl,
            type: 'GET',
            headers: { "RequestVerificationToken": _TOKEN },
            data: { ID: VehicleClassID },
            async: false,
            success: function (response) {
                $('#ddlVehiclePlateType').empty();

                if (response && response.result && response.result.length > 0) {
                    $.each(response.result, function (i, brand) {
                        $('#ddlVehiclePlateType').append(
                            '<option value="' + brand.VehiclePlateTypeID + '">' + brand.VehiclePlateTypeName + '</option>'
                        );
                    });
                } else {
                    $('#ddlVehiclePlateType').append('<option value="0">--No Record--</option>');
                }
                $("#ddlVehiclePlateType").val(0).change();
            }
        });
    } else {
        $('#ddlVehiclePlateType').empty().append('<option value="0">--Select--</option>');
    }
}

$('#ddlVehiclePlateType').on('change', function () {
    $('#ddlVehiclePlateSize').empty();
    var VehiclePlateTypeID = $(this).val();
    var VehicleClassID = $('#ddlVehicleClass').val();

    GetProductListFilter(VehicleClassID, VehiclePlateTypeID);
});
function GetProductListFilter(VehicleClassID, VehiclePlateTypeID) {
    if (VehiclePlateTypeID > 0 && VehicleClassID > 0) {
        $.ajax({
            url: LoadSizeByPlateTypeIDUrl,
            type: 'GET',
            headers: { "RequestVerificationToken": _TOKEN },
            data: { ClassID: VehicleClassID, PlateTypeID: VehiclePlateTypeID },
            async: false,
            success: function (response) {
                $('#ddlVehiclePlateSize').empty();

                if (response && response.result && response.result.length > 0) {
                    $.each(response.result, function (i, brand) {
                        $('#ddlVehiclePlateSize').append(
                            '<option value="' + brand.VehiclePlateSizeID + '">' + brand.VehiclePlateSizeName + '</option>'
                        );
                    });
                } else {
                    $('#ddlVehiclePlateSize').append('<option value="0">--No Record--</option>');
                }
                $("#ddlVehiclePlateSize").val(0).change();
            }
        });
    } else {
        $('#ddlVehiclePlateSize').empty().append('<option value="0">--Select--</option>');
    }
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