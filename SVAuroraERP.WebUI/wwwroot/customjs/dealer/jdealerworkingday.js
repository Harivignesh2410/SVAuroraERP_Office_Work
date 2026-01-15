$(function () {
    pLoadingSetup(false);
    
    // Initialize filter OEM dropdown
    LoadFilterOEMList();
    
    // Filter OEM change handler
    $("#filterOEM").on('change', function () {
        var oemID = $(this).val();
        if (oemID && oemID > 0) {
            LoadFilterDealersByOEMID(oemID);
        } else {
            $("#filterDealer").empty().append('<option value="0">--All Dealers--</option>').val(0);
        }
    });
    
    // Filter button click handler
    $("#btnFilter").on('click', function () {
        getRecordList();
    });
    
    // Clear filter button handler
    $("#btnClearFilter").on('click', function () {
        // Clear OEM filter
        if ($.fn.select2 && $("#filterOEM").hasClass('select2-hidden-accessible')) {
            $("#filterOEM").val(0).trigger('change.select2');
        } else {
            $("#filterOEM").val(0).trigger('change');
        }
        
        // Clear Dealer filter
        $("#filterDealer").empty().append('<option value="0">--All Dealers--</option>');
        if ($.fn.select2 && $("#filterDealer").hasClass('select2-hidden-accessible')) {
            $("#filterDealer").val(0).trigger('change.select2');
        } else {
            $("#filterDealer").val(0).trigger('change');
        }
        
        // Reload data table with cleared filters
        getRecordList();
    });
    
    // OEM dropdown change handler for form
    $("#ddlOEM").on('change', function () {
        var oemID = $(this).val();
        if (oemID && oemID > 0) {
            LoadDealersByOEMID(oemID);
        } else {
            $("#ddlDealer").empty().append('<option value="0">--Select Dealer--</option>').val(0);
        }
    });
    
    getRecordList();
    
    // Initialize select2 for filter dropdowns
    if ($.fn.select2) {
        $('#filterOEM').select2({ width: '100%' });
        $('#filterDealer').select2({ width: '100%' });
    }

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
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Dealer Working Day");
    ClearFormFields();

    return false;
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnDealerID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');
    $('.form-select').removeClass('is-invalid');

    $("#ddlOEM").val(0);
    $("#ddlDealer").val(0).empty().append('<option value="0">--Select Dealer--</option>');
    $(".working-day-checkbox").prop("checked", false);

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
$("#ddlDealer").on('change', function () {
    var dealerID = $(this).val();
    if (dealerID && dealerID > 0) {
        LoadDealerWorkingDays(dealerID);
    } else {
        ClearWorkingDays();
    }
});

// Update LoadDealerWorkingDays to not auto-set buttons (let EditData handle it)
function LoadDealerWorkingDays(DealerID) {
    $.ajax({
        url: GetDataByDealerIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { DealerID: DealerID },
        success: function (response) {
            ClearWorkingDays();
            if (response != null && response.Value != null) {
                var workingDays = response.Value;
                if (workingDays && workingDays.length > 0) {
                    $.each(workingDays, function (index, item) {
                        if (item.IsWorking) {
                            $("#chkDay_" + item.DayOfWeek).prop("checked", true);
                        }
                    });
                    $("#hdnDealerID").val(DealerID);
                    // Only auto-set buttons if this is called from dropdown change (not from EditData)
                    // Check if modal is not shown yet (meaning it's a manual selection)
                    if (!$('#divAddEditModal').hasClass('show')) {
                        $("#btnSave").hide();
                        $("#btnUpdate").show();
                    }
                }
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
        }
    });
}
function ClearWorkingDays() {
    $(".working-day-checkbox").prop("checked", false);
    $("#hdnDealerID").val(0);
    $("#btnSave").show();
    $("#btnUpdate").hide();
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
    $('.form-select').removeClass('is-invalid');

    var OEMID = parseInt($('#ddlOEM').val());
    var DealerID = parseInt($('#ddlDealer').val());

    // Validation
    if (!OEMID || OEMID == 0) {
        markInvalid("#ddlOEM", "Please select OEM");
        isValid = false;
    }
    if (!DealerID || DealerID == 0) {
        markInvalid("#ddlDealer", "Please select Dealer");
        isValid = false;
    }

    if (!isValid) return false;

    // Get selected Working Days (DayOfWeek values: 1-7)
    var WorkingDays = [];
    $('.working-day-checkbox:checked').each(function () {
        WorkingDays.push(parseInt($(this).val()));
    });

    var requestData = {
        DealerID: DealerID,
        WorkingDays: WorkingDays
    };

    SaveandUpdate(requestData);

    return false;
});
function SaveandUpdate(requestData) {
    if (ENABLE_VERBOSE_Logging) console.log(requestData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(requestData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.Success && !response.Error) {
                Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                $('#divAddEditModal').modal('hide');
                $("#btnRefresh").click();
            }
            else if (!response.Success && response.Error) {
                Swal.fire({ title: "Error", text: response.Message || "An error occurred", icon: "warning", confirmButtonColor: "#556ee6" });
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
function LoadFilterOEMList() {
    // Use the same OEM list from the model
    if (typeof OEMList !== 'undefined' && OEMList && OEMList.length > 0) {
        $("#filterOEM").empty();
        $("#filterOEM").append('<option value="0">--All OEM--</option>');
        $.each(OEMList, function (index, oem) {
            if (oem.Value != "0") {
                $("#filterOEM").append('<option value="' + oem.Value + '">' + oem.Text + '</option>');
            }
        });
    }
}

function LoadFilterDealersByOEMID(oemID) {
    $.ajax({
        url: GetDealersByOEMIDUrl,
        type: 'GET',
        headers: { "RequestVerificationToken": _TOKEN },
        data: { OEMID: oemID },
        success: function (response) {
            $("#filterDealer").empty();
            $("#filterDealer").append('<option value="0">--All Dealers--</option>');
            if (response && response.result && response.result.Value) {
                $.each(response.result.Value, function (index, dealer) {
                    if (dealer.Value != "0") {
                        $("#filterDealer").append('<option value="' + dealer.Value + '">' + dealer.Text + '</option>');
                    }
                });
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
        }
    });
}

function LoadDealersByOEMID(oemID) {
    $.ajax({
        url: GetDealersByOEMIDUrl,
        type: 'GET',
        headers: { "RequestVerificationToken": _TOKEN },
        data: { OEMID: oemID },
        success: function (response) {
            $("#ddlDealer").empty();
            $("#ddlDealer").append('<option value="0">--Select Dealer--</option>');
            if (response && response.result && response.result.Value) {
                $.each(response.result.Value, function (index, dealer) {
                    if (dealer.Value != "0") {
                        $("#ddlDealer").append('<option value="' + dealer.Value + '">' + dealer.Text + '</option>');
                    }
                });
            }
            $("#ddlDealer").val(0).trigger('change');
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            $("#ddlDealer").empty().append('<option value="0">--Select Dealer--</option>');
        }
    });
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
            "data": function (d) {
                // Get sort column name from column data
                var sortColumnName = "DealerName"; // default
                if (d.order && d.order.length > 0 && d.columns && d.columns[d.order[0].column]) {
                    var colData = d.columns[d.order[0].column].data;
                    // Map column data to actual property names
                    if (colData === "OEMName") sortColumnName = "OEMName";
                    else if (colData === "DealerName") sortColumnName = "DealerName";
                }
                
                return {
                    Draw: d.draw,
                    Start: d.start,
                    Length: d.length,
                    SearchValue: d.search.value || '',
                    SortColumn: sortColumnName,
                    SortDirection: d.order && d.order.length > 0 ? d.order[0].dir : 'asc',
                    OEMID: $("#filterOEM").val() && $("#filterOEM").val() > 0 ? parseInt($("#filterOEM").val()) : null,
                    DealerID: $("#filterDealer").val() && $("#filterDealer").val() > 0 ? parseInt($("#filterDealer").val()) : null
                };
            },
            processData: true,
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
                orderable: false,
                "width": "5%",
                "orderable": false
            },
            { "data": "OEMName", "orderable": true },
            { 
                "data": "DealerName", 
                "orderable": true,
                "render": function (data, type, row) {
                    var dealerCode = row.DealerCode || '';
                    var dealerName = data || '';
                    var city = row.City || '';
                    if (dealerCode && dealerName) {
                        return dealerCode + ' - ' + dealerName + (city ? ' (' + city + ')' : '');
                    }
                    return dealerName;
                }
            },
            {
                "data": "Monday",
                "render": function (data, type, row) {
                    return renderDayBadge(data, "Mon");
                },
                "width": "8%",
                "className": "text-center",
                "orderable": false
            },
            {
                "data": "Tuesday",
                "render": function (data, type, row) {
                    return renderDayBadge(data, "Tue");
                },
                "width": "8%",
                "className": "text-center",
                "orderable": false
            },
            {
                "data": "Wednesday",
                "render": function (data, type, row) {
                    return renderDayBadge(data, "Wed");
                },
                "width": "8%",
                "className": "text-center",
                "orderable": false
            },
            {
                "data": "Thursday",
                "render": function (data, type, row) {
                    return renderDayBadge(data, "Thu");
                },
                "width": "8%",
                "className": "text-center",
                "orderable": false
            },
            {
                "data": "Friday",
                "render": function (data, type, row) {
                    return renderDayBadge(data, "Fri");
                },
                "width": "8%",
                "className": "text-center",
                "orderable": false
            },
            {
                "data": "Saturday",
                "render": function (data, type, row) {
                    return renderDayBadge(data, "Sat");
                },
                "width": "8%",
                "className": "text-center",
                "orderable": false
            },
            {
                "data": "Sunday",
                "render": function (data, type, row) {
                    return renderDayBadge(data, "Sun");
                },
                "width": "8%",
                "className": "text-center",
                "orderable": false
            },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    // For working days, we edit by dealer, so pass DealerID instead
                    //return SetActionButtonsByDealer(row.DealerID, row.WorkingDayID, _CMPermissions);
                    return SetActionButtons(row.DealerID, _CMPermissions);
                },
                "width": "10%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}
function renderDayBadge(isWorking, dayLabel) {
    if (isWorking) {
        return '<span class="badge bg-success">' + dayLabel + '</span>';
    } else {
        return '<span class="badge bg-danger">' + dayLabel + '</span>';
    }
}
function SetActionButtonsByDealer(DealerID, WorkingDayID, permissions) {
    var buttons = '';
    if (permissions.HasView) {
        buttons += '<button type="button" class="btn btn-sm btn-info waves-effect waves-light me-1" onclick="EditData(' + DealerID + ', true)" title="View"><i class="fas fa-eye"></i></button>';
    }
    if (permissions.HasEdit) {
        buttons += '<button type="button" class="btn btn-sm btn-primary waves-effect waves-light me-1" onclick="EditData(' + DealerID + ', false)" title="Edit"><i class="fas fa-edit"></i></button>';
    }
    if (permissions.HasDelete) {
        buttons += '<button type="button" class="btn btn-sm btn-danger waves-effect waves-light" onclick="DeleteDataByDealer(' + DealerID + ')" title="Delete"><i class="fas fa-trash"></i></button>';
    }
    return buttons;
}
function EditData(DealerID, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) console.log(DealerID);
    ClearFormFields();
    if ((!_CMActionView && ViewFlag) || (!_CMActionUpdate && !ViewFlag)) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    
    // Load dealer working days to get OEM information
    $.ajax({
        url: GetDataByDealerIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { DealerID: DealerID },
        success: function (response) {
            if (response != null && response.Value != null) {
                var workingDays = response.Value;
                if (workingDays && workingDays.length > 0) {
                    // Get OEM information from first record
                    var firstRecord = workingDays[0];
                    var oemID = firstRecord.OEMID || 0;
                    
                    // Set OEM and load dealers
                    if (oemID > 0) {
                        $("#ddlOEM").val(oemID).trigger('change');
                        // Wait for dealers to load, then set dealer
                        setTimeout(function() {
                            $("#ddlDealer").val(DealerID).trigger('change');
                            // Load working days checkboxes
                            $.each(workingDays, function (index, item) {
                                if (item.IsWorking) {
                                    $("#chkDay_" + item.DayOfWeek).prop("checked", true);
                                }
                            });
                        }, 500);
                    } else {
                        // If no OEM, just set dealer directly
                        $("#ddlDealer").val(DealerID).trigger('change');
                    }
                }
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
        }
    });
    
    $('#divAddEditModal').modal('show');
    
    if (ViewFlag) {
        $("#btnSave").hide();
        $("#btnUpdate").hide();
        $("#divAddEditModal .modal-body :input").attr("disabled", true);
        $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Dealer Working Day");
    }
    else {
        $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Dealer Working Day");
        $("#btnSave").hide();
        $("#btnUpdate").show();
    }
    
    return false;
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
function DeleteDataByDealer(DealerID) {
    if (ENABLE_VERBOSE_Logging) console.log("DeleteDataByDealer: " + DealerID);
    if (!_CMActionDelete) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    Swal.fire({
        title: "Are you sure?",
        text: "This will delete all working days for this dealer. You won't be able to revert this!",
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
            $.ajax({
                url: DeleteDataByDealerUrl,
                type: 'POST',
                headers: { "RequestVerificationToken": _TOKEN },
                contentType: 'application/json',
                data: JSON.stringify(DealerID),
                success: function (response) {
                    if (ENABLE_VERBOSE_Logging) console.log(response);
                    if (response.resultdata && response.resultdata.Success && !response.resultdata.Error) {
                        Swal.fire({ title: "Deleted!", text: DeleteSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                        getRecordList(); // Refresh list or table
                    }
                    else {
                        Swal.fire({ title: "Error", text: response.resultdata?.Message || DeleteErrorMessage, icon: "error", confirmButtonColor: "#556ee6" });
                    }
                },
                error: function (xhr, status, error) {
                    if (ENABLE_VERBOSE_Logging) console.log(error);
                    Swal.fire({ title: "Error", text: error.responseText || DeleteErrorMessage, icon: "error", confirmButtonColor: "#556ee6" });
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
    $('#ddlOEM').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlDealer').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });    
});
