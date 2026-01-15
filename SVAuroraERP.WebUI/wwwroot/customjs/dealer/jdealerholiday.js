var isEditingDealerHoliday = false; // Flag to prevent OEM change handler during EditData

$(function () {
    pLoadingSetup(false);
    
    // Initialize filter OEM dropdown
    LoadFilterOEMList();
    
    // Initialize filter date pickers
    $("#filterFromDate").datetimepicker({
        pickTime: false,
        useCurrent: false,
        format: 'DD/MM/YYYY',
        minYear: 1951,
        maxDate: moment().add(1, 'year')
    });
    
    $("#filterToDate").datetimepicker({
        pickTime: false,
        useCurrent: false,
        format: 'DD/MM/YYYY',
        minYear: 1951,
        maxDate: moment().add(1, 'year')
    });
    
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
        
        // Clear date filters
        $("#filterFromDate").val('');
        $("#filterToDate").val('');
        
        // Reload data table with cleared filters
        getRecordList();
    });
    
    // Clear date functionality for filters
    $(".ClearDate").click(function () { 
        var x = $(this).prev().attr('id'); 
        $("#" + x).val(""); 
    });
    
    // Select date functionality for filters
    $(".SelectDate").click(function () { 
        var x = $(this).next().attr('id'); 
        $("#" + x).focus(); 
    });
    
    // OEM dropdown change handler for form
    $("#ddlOEM").on('change', function () {
        // Skip if we're in EditData mode
        if (isEditingDealerHoliday) {
            return;
        }
        var oemID = $(this).val();
        if (oemID && oemID > 0) {
            LoadDealersByOEMID(oemID);
        } else {
            $("#ddlDealer").empty().append('<option value="0">--Select Dealer--</option>').val(0);
        }
    });
    
    // Initialize datetimepicker for Holiday Date
    $("#txtHolidayDate").datetimepicker({
        pickTime: false,
        useCurrent: true,
        format: 'DD/MM/YYYY',
        minYear: 1951,
        maxDate: moment().add(1, 'year')
    });
    
    // Clear date functionality for form
    $(".ClearDate").click(function () { 
        var x = $(this).prev().attr('id'); 
        $("#" + x).val(""); 
    });
    
    // Select date functionality - click on calendar icon to focus input
    $(document).on('click', '#divHolidayDate .input-group-text:first', function () {
        $("#txtHolidayDate").focus();
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
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Dealer Holiday");
    ClearFormFields();

    return false;
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnDealerHolidayID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');
    $('.form-select').removeClass('is-invalid');

    $("#ddlOEM").val(0);
    $("#ddlDealer").val(0).empty().append('<option value="0">--Select Dealer--</option>');
    $("#txtHolidayDate").val("");
    $("#txtReason").val("");
    $("#chkIsFullDay").prop("checked", true);
    $(".holiday-type-checkbox").prop("checked", false);

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

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
function LoadFilterDealersByOEMID(OEMID) {
    $.ajax({
        url: GetDealersByOEMIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { OEMID: OEMID },
        success: function (response) {
            $("#filterDealer").empty();
            $("#filterDealer").append('<option value="0">--All Dealers--</option>');
            if (response.result && response.result.Value && response.result.Value.length > 0) {
                $.each(response.result.Value, function (index, dealer) {
                    if (dealer.Value != "0") {
                        $("#filterDealer").append('<option value="' + dealer.Value + '">' + dealer.Text + '</option>');
                    }
                });
            }
        }
    });
}
function LoadDealersByOEMID(OEMID, callback) {
    $.ajax({
        url: GetDealersByOEMIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { OEMID: OEMID },
        success: function (response) {
            $("#ddlDealer").empty();
            //$("#ddlDealer").append('<option value="0">--Select Dealer--</option>');
            if (response.result && response.result.Value && response.result.Value.length > 0) {
                $.each(response.result.Value, function (index, dealer) {
                    $("#ddlDealer").append('<option value="' + dealer.Value + '">' + dealer.Text + '</option>');
                });
            }
            // Trigger change only if no callback (normal flow)
            if (!callback) {
                $("#ddlDealer").val(0).trigger('change');
            }
            // Execute callback if provided (for EditData scenario)
            if (callback && typeof callback === 'function') {
                callback();
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            $("#ddlDealer").empty().append('<option value="0">--Select Dealer--</option>');
            if (callback && typeof callback === 'function') {
                callback();
            }
        }
    });
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
    $('.form-select').removeClass('is-invalid');

    var DealerHolidayData = new Object();
    DealerHolidayData.DealerHolidayID = 0;
    if (this.id == "btnUpdate" && $("#hdnDealerHolidayID").val() > 0) DealerHolidayData.DealerHolidayID = parseInt($("#hdnDealerHolidayID").val());

    DealerHolidayData.DealerID = parseInt($('#ddlDealer').val());
    DealerHolidayData.Reason = $('#txtReason').val();
    DealerHolidayData.IsFullDay = $("#chkIsFullDay").is(':checked') ? true : false;

    // Get date in DD/MM/YYYY format from datetimepicker
    var sHolidayDate = $('#txtHolidayDate').val(); // DD/MM/YYYY format from datetimepicker

    // Validation
    if (!DealerHolidayData.DealerID || DealerHolidayData.DealerID == 0) {
        markInvalid("#ddlDealer", "Please select Dealer");
        isValid = false;
    }
    if (!sHolidayDate || sHolidayDate.trim() === '') {
        markInvalid("#txtHolidayDate", "Please select Holiday Date");
        isValid = false;
    }

    if (!isValid) return false;

    // Get selected HolidayType IDs
    var HolidayTypeIDs = [];
    $('.holiday-type-checkbox:checked').each(function () {
        HolidayTypeIDs.push(parseInt($(this).val()));
    });

    var requestData = {
        DealerHoliday: DealerHolidayData,
        HolidayTypeIDs: HolidayTypeIDs,
        sHolidayDate: sHolidayDate // DD/MM/YYYY format from datetimepicker
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
                if (requestData.DealerHoliday.DealerHolidayID == 0)
                    Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                else if (requestData.DealerHoliday.DealerHolidayID > 0)
                    Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                $('#divAddEditModal').modal('hide');
                $("#btnRefresh").click();
            }
            else if (!response.Success && response.Error) {
                Swal.fire({ title: "Data already exists!", text: response.Message || "", icon: "warning", confirmButtonColor: "#556ee6" });
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
            "data": function (d) {
                // Parse dates from filter inputs
                var fromDateStr = null;
                var toDateStr = null;
                var filterFromDateStr = $("#filterFromDate").val();
                var filterToDateStr = $("#filterToDate").val();
                
                if (filterFromDateStr) {
                    // Convert DD/MM/YYYY to YYYY-MM-DD format for backend (avoid timezone issues)
                    var fromParts = filterFromDateStr.split('/');
                    if (fromParts.length === 3) {
                        var year = fromParts[2];
                        var month = String(fromParts[1]).padStart(2, '0');
                        var day = String(fromParts[0]).padStart(2, '0');
                        fromDateStr = year + '-' + month + '-' + day;
                    }
                }
                
                if (filterToDateStr) {
                    // Convert DD/MM/YYYY to YYYY-MM-DD format for backend (avoid timezone issues)
                    var toParts = filterToDateStr.split('/');
                    if (toParts.length === 3) {
                        var year = toParts[2];
                        var month = String(toParts[1]).padStart(2, '0');
                        var day = String(toParts[0]).padStart(2, '0');
                        toDateStr = year + '-' + month + '-' + day;
                    }
                }
                
                // Get sort column name from column data
                var sortColumnName = "HolidayDate"; // default
                if (d.order && d.order.length > 0 && d.columns && d.columns[d.order[0].column]) {
                    var colData = d.columns[d.order[0].column].data;
                    // Map column data to actual property names
                    if (colData === "OEMName") sortColumnName = "OEMName";
                    else if (colData === "DealerName") sortColumnName = "DealerName";
                    else if (colData === "HolidayDate") sortColumnName = "HolidayDate";
                    else if (colData === "Reason") sortColumnName = "Reason";
                }
                
                return {
                    Draw: d.draw,
                    Start: d.start,
                    Length: d.length,
                    SearchValue: d.search.value || '',
                    SortColumn: sortColumnName,
                    SortDirection: d.order && d.order.length > 0 ? d.order[0].dir : 'desc',
                    OEMID: $("#filterOEM").val() && $("#filterOEM").val() > 0 ? parseInt($("#filterOEM").val()) : null,
                    DealerID: $("#filterDealer").val() && $("#filterDealer").val() > 0 ? parseInt($("#filterDealer").val()) : null,
                    FromDate: fromDateStr,
                    ToDate: toDateStr
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
            { 
                "data": "OEMName", 
                "orderable": true,
                "render": function (data, type, row) {
                    return data || '';
                }
            },
            { 
                "data": "DealerName", 
                "orderable": true,
                "render": function (data, type, row) {
                    var dealerCode = row.DealerCode || '';
                    var dealerName = data || '';
                    var city = row.City || '';
                    var parts = [];
                    
                    if (dealerCode) {
                        parts.push(dealerCode);
                    }
                    if (dealerName) {
                        parts.push(dealerName);
                    }
                    
                    var formatted = parts.join(' - ');
                    
                    if (city) {
                        formatted += ' (' + city + ')';
                    }
                    
                    return formatted || dealerName || '';
                }
            },
            {
                "data": "HolidayDate",
                "render": function (data, type, row) {
                    // Use sHolidayDate if available, otherwise format HolidayDate
                    if (row.sHolidayDate) {
                        return row.sHolidayDate;
                    }
                    if (data) {
                        var date = new Date(data);
                        return date.toLocaleDateString('en-GB'); // DD/MM/YYYY format
                    }
                    return "";
                },
                "orderable": true
            },
            { "data": "Reason", "orderable": true },
            {
                "data": "HolidayTypes",
                "orderable": false,
                "render": function (data, type, row) {
                    return data || "-";
                }
            },
            {
                "data": "IsFullDay",
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
                    return SetActionButtons(row.DealerHolidayID, _CMPermissions);
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
    
    // Set flag to prevent OEM change handler from interfering
    isEditingDealerHoliday = true;
    
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
                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Dealer Holiday");
            }
            else {
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Dealer Holiday");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }
            if (response != null && response.Value != null) {
                var result = response;
                if (!result.Error && result.Success && result.ID > 0) {
                    var DealerHolidayData = response.Value;
                    //console.log(DealerHolidayData);
                    $("#hdnDealerHolidayID").val(DealerHolidayData.DealerHolidayID);
                    
                    // Show modal first
                    $('#divAddEditModal').modal('show');
                    
                    // Use OEMID from the domain response
                    var dealerIDToSet = DealerHolidayData.DealerID;
                    if (DealerHolidayData.OEMID && DealerHolidayData.OEMID > 0) {
                        // Set OEM value (change handler is disabled by flag)
                        $("#ddlOEM").val(DealerHolidayData.OEMID);
                        // Update select2 if it exists
                        if ($.fn.select2 && $("#ddlOEM").hasClass('select2-hidden-accessible')) {
                            $("#ddlOEM").trigger('change.select2');
                        }
                        // Load dealers for this OEM and set dealer after loading completes
                        LoadDealersByOEMID(DealerHolidayData.OEMID, function() {
                            // Set dealer after dealers are loaded
                            if (dealerIDToSet) {
                                $("#ddlDealer").val(dealerIDToSet);
                                // Update select2 if it exists
                                if ($.fn.select2 && $("#ddlDealer").hasClass('select2-hidden-accessible')) {
                                    $("#ddlDealer").trigger('change.select2');
                                }
                            }
                            // Reset flag after dealer is set
                            isEditingDealerHoliday = false;
                        });
                    } else {
                        // If no OEM, set dealer directly
                        if (dealerIDToSet) {
                            $("#ddlDealer").val(dealerIDToSet);
                            if ($.fn.select2 && $("#ddlDealer").hasClass('select2-hidden-accessible')) {
                                $("#ddlDealer").trigger('change.select2');
                            }
                        }
                        // Reset flag
                        isEditingDealerHoliday = false;
                    }
                    
                    // Use sHolidayDate directly from the response (already in DD/MM/YYYY format)
                    // If sHolidayDate is not available, format it from HolidayDate
                    var holidayDateValue = DealerHolidayData.sHolidayDate;
                    if (!holidayDateValue && DealerHolidayData.HolidayDate) {
                        // Fallback: format the date if sHolidayDate is not available
                        var date = new Date(DealerHolidayData.HolidayDate);
                        if (!isNaN(date.getTime())) {
                            var day = String(date.getDate()).padStart(2, '0');
                            var month = String(date.getMonth() + 1).padStart(2, '0');
                            var year = date.getFullYear();
                            holidayDateValue = day + '/' + month + '/' + year;
                        }
                    }
                    
                    // Set the date value - use modal shown event to ensure it's set after modal is fully rendered
                    $('#divAddEditModal').on('shown.bs.modal', function () {
                        if (holidayDateValue) {
                            $("#txtHolidayDate").val(holidayDateValue);
                            
                            // Try to update datetimepicker if it exists
                            var datePicker = $("#txtHolidayDate").data('DateTimePicker');
                            if (datePicker) {
                                try {
                                    var momentDate = moment(holidayDateValue, 'DD/MM/YYYY');
                                    if (momentDate.isValid()) {
                                        datePicker.setDate(momentDate);
                                    }
                                } catch (e) {
                                    console.log('DateTimePicker setDate error:', e);
                                }
                            }
                        }
                        // Remove the event handler after first use
                        $('#divAddEditModal').off('shown.bs.modal');
                    });
                    
                    // Also set it immediately as fallback
                    if (holidayDateValue) {
                        $("#txtHolidayDate").val(holidayDateValue);
                    }
                    
                    $("#txtReason").val(DealerHolidayData.Reason || "");
                    $("#chkIsFullDay").prop('checked', DealerHolidayData.IsFullDay);

                    // Set HolidayType checkboxes
                    $(".holiday-type-checkbox").prop("checked", false);
                    if (DealerHolidayData.HolidayTypeIDs && DealerHolidayData.HolidayTypeIDs.length > 0) {
                        DealerHolidayData.HolidayTypeIDs.forEach(function (holidayTypeID) {
                            $("#chkHolidayType_" + holidayTypeID).prop("checked", true);
                        });
                    }

                    $("#divRecordLog").show();
                    $("#spnLastUpdatedBy").html("Last Updated By: " + (DealerHolidayData.LastUpdatedByName || ""));
                    $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(DealerHolidayData.LastUpdatedDate));
                }
                else {
                    Swal.fire({ title: "Error", text: result.Message, icon: "warning", confirmButtonColor: "#556ee6" });
                    isEditingDealerHoliday = false; // Reset flag on error
                }
            }
            else {
                Swal.fire({ title: "Error", text: "Something went wrong!", icon: "warning", confirmButtonColor: "#556ee6" });
                isEditingDealerHoliday = false; // Reset flag on error
            }

        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
            isEditingDealerHoliday = false; // Reset flag on error
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
    $('#ddlOEM').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });
    $('#ddlDealer').select2({ dropdownParent: $('#divAddEditModal'), width: '100%' });    
});
