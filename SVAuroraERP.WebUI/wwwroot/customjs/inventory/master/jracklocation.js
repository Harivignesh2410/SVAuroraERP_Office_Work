var CapacityArray = [];
function getMaxSNo() {
    if (purchaseEntryArray.length === 0) return 0;
    return Math.max(...purchaseEntryArray.map(item => parseInt(item.sNo) || 0));
}
$(function () {
    pLoadingSetup(false);
    if (!_CMActionAdd) {
        $("#btnAddNew").remove();
        $("#btnSave").remove();
        $("#btnSaveCapacity").remove();
    }
    if (!_CMActionUpdate) $("#btnUpdate").remove();
    if (!_CMActionUpdate) $("#btnUpdateCapacity").remove();
    getRecordList();

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#btnSaveCapacity").hide();
    $("#btnUpdateCapacity").hide();

    pLoadingSetup(true);
});
$("#btnAddNew").on('click', function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Rack Location");

    // Hide RackCapacity tab
    $("#tabRackCapacity").hide();

    // Reset tab state - activate RackLocation tab
    $('a[href="#divRackLocation"]').tab('show');
    $("#divRackLocation").addClass("show active");
    $("#divRackCapacity").removeClass("show active");

    $('#divAddEditModal').modal('show');
    ClearFormFields();

    return false;
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnRackLocationID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#ddlWareHouse").val("0").change();
    $("#txtRackLocationCode").val("");
    $("#txtRackLocationName").val("");
    $("#chkActive").prop("checked", true);
    $("#ddlComponentType").val("0").change();

    // Reset tab state here too for redundancy
    $('a[href="#divRackLocation"]').tab('show');
    $("#divRackLocation").addClass("show active");
    $("#divRackCapacity").removeClass("show active");

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

    CapacityArray = [];
    var RackLocationSizeCapacityID = parseInt($("#hdnRackLocationSizeCapacityID").val()) || 0;
    if (this.id === "btnSave") {
        $("#purchaseTable tbody tr").each(function () {
            let sizeID = $(this).find("input").attr("id").replace("txt", ""); // Extract SizeID from input ID
            let capacity = $(this).find("input").val().trim(); // Get the entered capacity

            CapacityArray.push({
                SizeID: sizeID,
                Capacity: capacity || "0.00", // Default to 0.00 if empty
                StatusFlag: "I",
                RackLocationSizeCapacityID: 0
            });
        });

    } else if (this.id === "btnUpdate") {
        // For updates
        const currentSNo = parseInt($("#hdnSNo").val());
        CapacityArray.sNo = currentSNo;

        if (RackLocationSizeCapacityID > 0) {
            $("#purchaseTable tbody tr").each(function () {
                let sizeID = $(this).find("input").attr("id").replace("txt", ""); // Extract SizeID from input ID
                let capacity = $(this).find("input").val().trim(); // Get the entered capacity

                CapacityArray.push({
                    SizeID: sizeID,
                    Capacity: capacity || "0.00", // Default to 0.00 if empty
                    StatusFlag: "U",
                    RackLocationSizeCapacityID: RackLocationSizeCapacityID
                });
            });

        } else {
            $("#purchaseTable tbody tr").each(function () {
                let sizeID = $(this).find("input").attr("id").replace("txt", ""); // Extract SizeID from input ID
                let capacity = $(this).find("input").val().trim(); // Get the entered capacity

                CapacityArray.push({
                    SizeID: sizeID,
                    Capacity: capacity || "0.00", // Default to 0.00 if empty
                    StatusFlag: "I",
                    RackLocationSizeCapacityID: 0
                });
            });
        }
    }

    // Collect data from input fields
    var RackLocationData = new Object();

    RackLocationData.RackLocationID = 0;
    if (this.id == "btnUpdate" && $("#hdnRackLocationID").val() > 0) RackLocationData.RackLocationID = $("#hdnRackLocationID").val();

    RackLocationData.WareHouseID = $('#ddlWareHouse ').val();
    RackLocationData.RackLocationName = $('#txtRackLocationName ').val();
    RackLocationData.RackLocationCode = $('#txtRackLocationCode').val();
    RackLocationData.IsActive = $("#chkActive").is(':checked') ? true : false;

    RackLocationData.ComponentTypeID = $('#ddlComponentType').val();
    RackLocationData.RackLocationSizeCapacity = CapacityArray;

    if (RackLocationData.WareHouseID == 0) return markInvalid("#ddlWareHouse", "Please Select WareHouse Name");
    if (RackLocationData.ComponentTypeID == 0) return markInvalid("#ddlComponentType", "Please Select Componenet Type ");
    if (!RackLocationData.RackLocationName) return markInvalid("#txtRackLocationName", "Please enter Rack Location Name");
    if (!RackLocationData.RackLocationCode) return markInvalid("#txtRackLocationCode", "Please enter Rack Location Code");
    if (!isValid) return;
    if (isValid) {

        if (this.id === "btnSaveItem") {
            ClearModuleFormFields();
        } else {
            $("#divAddUnitModal").modal('hide');
        }
    }

    SaveandUpdateRackLocation(RackLocationData);

    return false;
});
function SaveandUpdateRackLocation(RackLocationData) {
    if (ENABLE_VERBOSE_Logging) //console.log(RackLocationData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(RackLocationData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
            if (response != null && response != null) {
                if (response.Success && !response.Error) {
                    if (RackLocationData.RackLocationID == 0)
                        Swal.fire({ title: "Rack Capacity tab enabled", text: "Now You can Update Your Rack Capacity", icon: "success", confirmButtonColor: "#556ee6" });
                    else if (RackLocationData.RackLocationID > 0)
                        Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                    $("#btnSaveCapacity").show();
                    $("#btnUpdateCapacity").hide();
                    EditData(response.ID, false);

                    $("#btnRefresh").click();
                }
                else if (!response.Success && response.Error) {
                    Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
                }
                else if (!response.Suceess && !response.Error) {
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
            url: RackLocationDataTableaUrl,
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
            { "data": "RackLocationCode", "orderable": true, "width": "10%" },
            { "data": "RackLocationName", "orderable": true },
            { "data": "WareHouseName", "orderable": true },
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
                    return SetActionButtons(data.RackLocationID, _CMPermissions);
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
                // Hide action buttons
                $("#btnSave").hide();
                $("#btnUpdate").hide();
                $("#btnUpdateCapacity").hide();
                $("#btnSaveCapacity").hide();

                $("#divAddEditModal .modal-body :input").attr("disabled", true);
                $("#divListSize input").attr("disabled", true);

                $("#btnClose, #btnCloseCapacity").attr("disabled", false).removeClass("disabled");
                $(".nav-link").attr("disabled", false);

                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Rack Location");

                //$("#btnClose, #btnCloseCapacity").removeClass("btn-outline-danger").addClass("btn-danger");
            } else {
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Rack Location");
                $("#btnSave").hide();
                $("#btnUpdate").show();
                $("#btnSaveCapacity").hide();
                $("#btnUpdateCapacity").show();

                $("#btnClose, #btnCloseCapacity").addClass("btn-outline-danger").removeClass("btn-danger");
            }
            $('#divAddEditModal').modal('show');
            $("#tabRackCapacity").show();

            if (ViewFlag) {
                $(".nav-tabs .nav-link").removeClass("disabled");
            }
            var taxdata = response.Value;

            // Populate non-capacity fields
            $("#ddlWareHouse").val(taxdata.WareHouseID).change();
            $("#ddlComponentType").val(taxdata.ComponentTypeID).change();
            $("#hdnRackLocationID").val(taxdata.RackLocationID);
            $("#txtRackLocationCode").val(taxdata.RackLocationCode);
            $("#txtRackLocationName").val(taxdata.RackLocationName);
            $("#chkActive").prop('checked', taxdata.IsActive);

            // Store capacity data for later use
            var capacityData = taxdata.RackLocationSizeCapacity;

            // Call GetSizeList and wait for it to complete before populating capacity fields
            GetSizeList(capacityData);

            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + taxdata.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(taxdata.LastUpdatedDate));

            // After loading data, ensure close buttons are clickable
            setTimeout(function () {
                if (ViewFlag) {
                    $("#purchaseTable input").attr("disabled", true);
                }
                $("#btnClose, #btnCloseCapacity").attr("disabled", false).removeClass("disabled");
            }, 100);
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
$('#divAddEditModal').on('shown.bs.modal', function () {
    $('#ddlWareHouse').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlComponentType').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });

});
function GetSizeList(capacityData) {
    $.ajax({
        url: GetSizeListUrl,
        type: 'GET',
        contentType: 'application/json',
        success: function (response) {
            let tableContent = `
        <table id="purchaseTable" class="table table-sm table-hover align-middle">
            <thead>
                <tr class="table-light">
                    <th>S.No</th>
                    <th>Size</th>
                    <th>Capacity</th>
                </tr>
            </thead>
            <tbody>`;

            response.data.Value.forEach((entry, index) => {
                tableContent += `
                          <tr data-sno="${index + 1}">
                             <td>${index + 1}</td>
                              <td>${entry.SizeName || ""}</td>
                              <td><input id="txt${entry.SizeID}" type="text" class="form-control text-end decimal" placeholder="0.00"></td>
                          </tr>`;
            });

            tableContent += `
                           </tbody>
                        </table>`;

            // Update the divTableData container
            $("#divListSize").empty();
            $("#divListSize").html(tableContent);
            $(".decimal").inputmask("decimal", { digits: 2, radixPoint: "." });

            // Now populate capacity values after the table is created
            if (capacityData && capacityData.length > 0) {
                setTimeout(function () {
                    capacityData.forEach(entry => {
                        $(`#txt${entry.SizeID}`).val(entry.Capacity);
                    });
                }, 100); // Small delay to ensure DOM is ready
            }

        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
$("#divRackCapacity").on('click', function () {

    return false;
});
$("#btnSaveCapacity,#btnUpdateCapacity").on('click', function () {
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

    CapacityArray = [];
    var RackLocationSizeCapacityID = parseInt($("#hdnRackLocationSizeCapacityID").val()) || 0;
    var RackLocationID = parseInt($("#hdnRackLocationID").val()) || 0;

    // Check if RackLocationID is valid
    if (!RackLocationID || RackLocationID <= 0) {
        Swal.fire({
            title: "Error",
            text: "Invalid Rack Location ID. Please save the Rack Location details first.",
            icon: "error",
            confirmButtonColor: "#556ee6"
        });
        return false;
    }

    if (this.id === "btnSaveCapacity") {
        // For new capacity entries
        $("#purchaseTable tbody tr").each(function () {
            let sizeID = $(this).find("input").attr("id").replace("txt", ""); // Extract SizeID from input ID
            let capacity = $(this).find("input").val().trim(); // Get the entered capacity

            CapacityArray.push({
                SizeID: sizeID,
                Capacity: capacity || "0.00", // Default to 0.00 if empty
                StatusFlag: "I", // Insert flag
                RackLocationSizeCapacityID: 0,
                RackLocationID: RackLocationID
            });
        });

        // Call save function
        UpdateRackCapacity(CapacityArray);
    }
    else if (this.id === "btnUpdateCapacity") {
        // For updating existing capacity entries
        $("#purchaseTable tbody tr").each(function () {
            let sizeID = $(this).find("input").attr("id").replace("txt", ""); // Extract SizeID from input ID
            let capacity = $(this).find("input").val().trim(); // Get the entered capacity
            if (RackLocationSizeCapacityID > 0) {
                CapacityArray.push({
                    SizeID: sizeID,
                    Capacity: capacity || "0.00", // Default to 0.00 if empty
                    StatusFlag: "U", // Update flag
                    RackLocationSizeCapacityID: RackLocationSizeCapacityID,
                    RackLocationID: RackLocationID
                });
            } else {
                CapacityArray.push({
                    SizeID: sizeID,
                    Capacity: capacity || "0.00", // Default to 0.00 if empty
                    StatusFlag: "I", // Insert flag
                    RackLocationSizeCapacityID: 0,
                    RackLocationID: RackLocationID
                });
            }
        });

        // Call update function
        UpdateRackCapacity(CapacityArray);
    }

    return false;
});
function UpdateRackCapacity(RackCapacityData) {
    if (ENABLE_VERBOSE_Logging) //console.log(RackCapacityData);

    $.ajax({
        url: UpdateRackCapacityUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(RackCapacityData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.Success && !response.Error) {
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