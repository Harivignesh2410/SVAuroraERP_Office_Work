var frontPath = null;
var rearPath = null;
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
$('#divAddEditModal').on('shown.bs.modal', function () {
    $('#ddlPlateSizeType').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlVehiclePlateColour').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
});
$("#btnAddNew").on('click', function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Vehicle Plate Image");
    ClearFormFields();

    return false;
});

$('#btnRefresh').on('click', function () {
    getRecordList();
    return false;
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnVehicleClassID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#ddlPlateSizeType").val("0").change();
    $("#ddlVehiclePlateColour").val("0").change();
    $("#FrontImage").val("");
    $("#RearImage").val("");

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

    return false;
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
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    const plateSizeID = $('#ddlPlateSizeType').val();
    const plateColorID = $('#ddlVehiclePlateColour').val();

    if (!plateSizeID) return markInvalid("#ddlPlateSizeType", "Please Select Vehicle Plate Size");
    if (!plateColorID) return markInvalid("#ddlVehiclePlateColour", "Please Select Vehicle Plate Color");
    if (!isValid) return;

    let vehiclePlateImageID = 0;
    if (this.id === "btnUpdate" && $("#hdnVehiclePlateImageID").val() > 0) {
        vehiclePlateImageID = $("#hdnVehiclePlateImageID").val();
    }

    const frontImage = document.getElementById("FrontImage").files[0];
    const rearImage = document.getElementById("RearImage").files[0];

    
    if (frontImage || rearImage) {
        UploadImage(function (frontPath, rearPath) {
            let VehiclePlateSizeData = {
                VehiclePlateImageID: vehiclePlateImageID,
                VehiclePlateSizeID: plateSizeID,
                VehiclePlateColorID: plateColorID,
                FrontImageURL: frontPath,
                RearImageURL: rearPath
            };
            SaveandUpdate(VehiclePlateSizeData);
        });
    } else {
        let VehiclePlateSizeData = {
            VehiclePlateImageID: vehiclePlateImageID,
            VehiclePlateSizeID: plateSizeID,
            VehiclePlateColorID: plateColorID,
            FrontImageURL: null,
            RearImageURL: null
        };
        SaveandUpdate(VehiclePlateSizeData);
    }

    return false;
});
function SaveandUpdate(VehiclePlateImageData) {
    if (ENABLE_VERBOSE_Logging) console.log(VehiclePlateImageData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(VehiclePlateImageData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.Success && !response.Error) {
                if (VehiclePlateImageData.VehiclePlateImageID == 0)
                    Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                else if (VehiclePlateImageData.VehiclePlateImageID > 0)
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
            "type": "GET",
            "data": function (d) {
                // d.search.value = $('#tblrecordlist_filter input').val();  // Make sure the search value is passed
                // Pass additional parameters if needed
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
            { "data": "VehiclePlateSizeName", "orderable": true },
            { "data": "VehiclePlateColorName", "orderable": true },
            {
                data: "FrontImageURL",
                render: function (data, type, row) {
                    return `<img src="${data}" alt="Front" style="height:40px;width:auto;" />`;
                },
                "orderable": false, 
                "width": "20%"
            },
            {
                data: "RearImageURL",
                render: function (data, type, row) {
                    return `<img src="${data}" alt="Rear" style="height:40px;width:auto;" />`;
                },
                "orderable": false,
                "width": "20%"
            },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    return `<ul class="list-unstyled hstack gap-1 mb-0">
                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
                            <a href="javascript:void(0);" onclick="DeleteData('${row.VehiclePlateImageID}')" class="btn btn-sm btn-soft-danger">
                                <i class="mdi mdi-delete-outline"></i>
                            </a>
                        </li>
                    </ul>`;
                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}
function UploadImage(callback) {
    var formData = new FormData();
    var frontImage = document.getElementById("FrontImage").files[0];
    var rearImage = document.getElementById("RearImage").files[0];

    //if (!frontImage || !rearImage) {
    //    alert("Please select both Front and Rear images!");
    //    return;
    //}

    formData.append("FrontImage", frontImage);
    formData.append("RearImage", rearImage);

    $.ajax({
        url: UploadImageURl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        data: formData,
        contentType: false,
        processData: false,
        success: function (data) {
            if (data.success) {
                // Pass the image paths back through the callback
                callback(data.frontImagePath, data.rearImagePath);
            } else {
                Swal.fire("Error", "Image upload failed!", "error");
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: xhr.responseText || error, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
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

                $("#divAddEditModal .modal-body :input").attr("disabled", true);
                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Vehicle Class");
            }
            else {
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Vehicle Plate Image");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }
            if (response != null && response.Value != null) {
                var result = response;
                if (!result.Error && result.Success && result.ID > 0) {
                    $('#divAddEditModal').modal('show');
            var VehiclePlateImageData = response.Value;
            $("#hdnVehiclePlateImageID").val(VehiclePlateImageData.VehiclePlateImageID);
            $("#ddlPlateSizeType").val(VehiclePlateImageData.VehiclePlateSizeID);
            $("#ddlVehiclePlateColour").val(VehiclePlateImageData.VehiclePlateColorID);
            $("#FrontImage").val(VehiclePlateImageData.FrontImageURL);
            $("#RearImage").val(VehiclePlateImageData.RearImageURL);

            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + VehiclePlateImageData.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + VehiclePlateImageData.LastUpdatedDate);
        }
                else
                    Swal.fire({ title: "Error", text: result.Message, icon: "warning", confirmButtonColor: "#556ee6" });
    }
            else
        Swal.fire({ title: "Error", text: "Something went wrong!", icon: "warning", confirmButtonColor: "#556ee6" });

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