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
    
    getRecordList();
    LoadTimeSlots();

    // Initialize datetimepicker for Slot Date
    $("#txtSlotDate").datetimepicker({
        pickTime: false,
        useCurrent: true,
        format: 'DD/MM/YYYY',
        minYear: 1951,
        maxDate: moment().add(1, 'year') // Allow future dates for slot configuration
    });
    
    // Clear date functionality
    $(".ClearDate").click(function () { 
        var x = $(this).prev().attr('id'); 
        $("#" + x).val(""); 
        $("#divSlotDateWarning").hide().empty();
    });
    
    // Select date functionality
    $(".SelectDate").click(function () { 
        var x = $(this).next().attr('id'); 
        $("#" + x).focus(); 
    });

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
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Slot Configuration");
    ClearFormFields();

    return false;
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnConfigID").val(0);
    $("#hdnOriginalSlotDate").val('');

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');
    $('.form-select').removeClass('is-invalid');

    $("#ddlOEM").val(0);
    $("#ddlDealer").val(0).empty().append('<option value="0">--Select Dealer--</option>');
    $("#txtSlotDate").val('');
    $("#divSlotDateWarning").hide().empty();
    LoadTimeSlots();
    // Note: chkActive is removed as we now have per-time-slot active checkboxes

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

    return false;
}
$("#ddlOEM").on('change', function () {
    var oemID = $(this).val();
    if (oemID && oemID > 0) {
        LoadDealersByOEMID(oemID);
    } else {
        $("#ddlDealer").empty().append('<option value="0">--Select Dealer--</option>').val(0);
    }
    // Clear slot date and configurations when OEM changes
    $("#txtSlotDate").val('');
    $("#hdnOriginalSlotDate").val('');
    ClearTimeSlotConfigs();
});
function LoadDealersByOEMID(OEMID) {
    $.ajax({
        url: GetDealersByOEMIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { OEMID: OEMID },
        success: function (response) {
            $("#ddlDealer").empty();
            if (response.result && response.result.Value && response.result.Value.length > 0) {
                $.each(response.result.Value, function (index, dealer) {
                    $("#ddlDealer").append('<option value="' + dealer.Value + '">' + dealer.Text + '</option>');
                });
            } else {
                $("#ddlDealer").append('<option value="0">--Select Dealer--</option>');
            }
            $("#ddlDealer").val(0).trigger('change');
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            $("#ddlDealer").empty().append('<option value="0">--Select Dealer--</option>');
        }
    });
}
function ClearTimeSlotConfigs() {
    $('.time-slot-capacity').val(10).attr('data-configid', 0);
    $('.time-slot-active').prop('checked', true);
    $("#hdnConfigID").val(0);
    $("#btnSave").show();
    $("#btnUpdate").hide();
}
function LoadTimeSlots() {
    var html = '';
    //console.log('Loading time slots:', TimeSlotList);
    if (TimeSlotList && TimeSlotList.length > 0) {
        $.each(TimeSlotList, function (index, slot) {
            //console.log(slot);
            html += '<div class="row mb-2 align-items-center">';
            html += '<div class="col-md-6">';
            html += '<label class="form-label">' + slot.Text + '</label>';
            html += '</div>';
            html += '<div class="col-md-4">';
            html += '<input type="number" class="form-control time-slot-capacity" data-timeslotid="' + slot.Value + '" data-configid="0" placeholder="Max Capacity" min="1" value="10">';
            html += '</div>';
            html += '<div class="col-md-2">';
            html += '<div class="form-check form-check-success">';
            html += '<input type="checkbox" class="form-check-input time-slot-active" data-timeslotid="' + slot.Value + '" checked id="chkTimeSlot_' + slot.Value + '">';
            html += '<label class="form-check-label" for=chkTimeSlot_' + slot.Value + '>Enable</label>';
            html += '</div>';
            html += '</div>';
            html += '</div>';
        });
    }
    $("#divTimeSlotList").html(html);
}
$("#ddlDealer").on('change', function () {
    // Clear slot date and configurations when dealer changes
    $("#txtSlotDate").val('');
    $("#hdnOriginalSlotDate").val('');
    ClearTimeSlotConfigs();
    $("#divSlotDateWarning").hide().empty();
});
$("#txtSlotDate").on('change', function () {
    var dealerID = $("#ddlDealer").val();
    var slotDate = $("#txtSlotDate").val();
    var originalDate = $("#hdnOriginalSlotDate").val();
    var isEditMode = originalDate && originalDate.length > 0; // Check if we're in edit mode
    
    if (dealerID && dealerID > 0 && slotDate) {
        ValidateAndLoadSlotDate(dealerID, slotDate, isEditMode);
    } else {
        $("#divSlotDateWarning").hide().empty();
    }
});
function ValidateAndLoadSlotDate(DealerID, SlotDate, preserveOriginalDate) {
    preserveOriginalDate = preserveOriginalDate || false;
    
    $.ajax({
        url: ValidateSlotDateUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { DealerID: DealerID, sSlotDate: SlotDate },
        success: function (response) {
            if (response.IsValid === false) {
                $("#divSlotDateWarning").html('<i class="fas fa-exclamation-triangle"></i> ' + response.Message).show();
                $("#txtSlotDate").addClass('is-invalid');
            } else {
                $("#divSlotDateWarning").hide().empty();
                $("#txtSlotDate").removeClass('is-invalid');
                // Load existing configurations for this dealer and date
                // If preserveOriginalDate is true, we're changing date during edit, so preserve the original date
                LoadExistingConfigurations(DealerID, SlotDate, preserveOriginalDate);
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
        }
    });
}
function LoadExistingConfigurations(DealerID, SlotDate, preserveOriginalDate) {
    // preserveOriginalDate: if true, don't overwrite hdnOriginalSlotDate (used when date changes during edit)
    preserveOriginalDate = preserveOriginalDate || false;
    
    $.ajax({
        url: GetDataByDealerAndDateUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { DealerID: DealerID, sSlotDate: SlotDate },
        success: function (response) {
            if (response.Success && response.Value && response.Value.length > 0) {
                $.each(response.Value, function (index, config) {
                    $('.time-slot-capacity[data-timeslotid="' + config.TimeSlotID + '"]').val(config.MaxCapacity).attr('data-configid', config.ConfigID || 0);
                    $('.time-slot-active[data-timeslotid="' + config.TimeSlotID + '"]').prop('checked', config.IsActive);
                });
                $("#hdnConfigID").val(response.Value[0].ConfigID);
                // Only store original date if not preserving it (i.e., first load, not date change during edit)
                if (!preserveOriginalDate) {
                    $("#hdnOriginalSlotDate").val(SlotDate);
                }
                // If we have existing configs, show update button (unless we're preserving original date which means we're in edit mode)
                if (!preserveOriginalDate) {
                    $("#btnSave").hide();
                    $("#btnUpdate").show();
                }
            } else {
                // Reset to defaults - no existing configs for this date
                $('.time-slot-capacity').val(10).attr('data-configid', 0);
                $('.time-slot-active').prop('checked', true);
                $("#hdnConfigID").val(0);
                // Only clear original date if not preserving it
                if (!preserveOriginalDate) {
                    // New record - no existing configs
                    $("#hdnOriginalSlotDate").val('');
                    $("#btnSave").show();
                    $("#btnUpdate").hide();
                } else {
                    // Edit mode - user changed date to a date with no existing configs
                    // Keep original date, show Update button (backend will delete old date's configs and create new ones)
                    $("#btnSave").hide();
                    $("#btnUpdate").show();
                }
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
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

    var DealerID = parseInt($('#ddlDealer').val());
    var sSlotDate = $("#txtSlotDate").val(); // DD/MM/YYYY format from datetimepicker
    var sOriginalSlotDate = $("#hdnOriginalSlotDate").val() || null; // DD/MM/YYYY format
    var ConfigID = parseInt($("#hdnConfigID").val()) || 0;

    var OEMID = parseInt($('#ddlOEM').val());
    
    // Validation
    if (!OEMID || OEMID == 0) {
        markInvalid("#ddlOEM", "Please select OEM");
        isValid = false;
    }
    if (!DealerID || DealerID == 0) {
        markInvalid("#ddlDealer", "Please select Dealer");
        isValid = false;
    }
    if (!sSlotDate) {
        markInvalid("#txtSlotDate", "Please select Slot Date");
        isValid = false;
    }

    // Validate all time slots have valid max capacity
    var timeSlotConfigs = [];
    var timeSlotCount = $('.time-slot-capacity').length;
    
    if (timeSlotCount === 0) {
        Swal.fire({ title: "Error", text: "No time slots available. Please refresh the page.", icon: "error", confirmButtonColor: "#556ee6" });
        return false;
    }
    
    $('.time-slot-capacity').each(function () {
        var timeSlotID = parseInt($(this).data('timeslotid')) || 0;
        var configID = parseInt($(this).data('configid')) || 0;
        var maxCapacity = parseInt($(this).val()) || 0;
        var isActive = $('.time-slot-active[data-timeslotid="' + timeSlotID + '"]').is(':checked');
        
        if (timeSlotID <= 0) {
            markInvalid($(this), "Invalid time slot");
            isValid = false;
            return;
        }
        
        if (maxCapacity <= 0) {
            markInvalid($(this), "Max Capacity must be greater than 0");
            isValid = false;
            return;
        }
        
        timeSlotConfigs.push({
            ConfigID: configID,
            TimeSlotID: timeSlotID,
            MaxCapacity: maxCapacity,
            IsActive: isActive
        });
    });

    if (!isValid || timeSlotConfigs.length === 0) {
        if (timeSlotConfigs.length === 0) {
            Swal.fire({ title: "Error", text: "Please configure at least one time slot.", icon: "error", confirmButtonColor: "#556ee6" });
        }
        return false;
    }

    // Validate date again before saving
    if (DealerID > 0 && sSlotDate) {
        $.ajax({
            url: ValidateSlotDateUrl,
            type: 'GET',
            contentType: 'application/json',
            data: { DealerID: DealerID, sSlotDate: sSlotDate },
            async: false,
            success: function (response) {
                if (response.IsValid === false) {
                    Swal.fire({ title: "Validation Error", text: response.Message, icon: "warning", confirmButtonColor: "#556ee6" });
                    isValid = false;
                }
            }
        });
    }

    if (!isValid) return false;

    var requestData = {
        ConfigID: ConfigID,
        DealerID: DealerID,
        sSlotDate: sSlotDate, // DD/MM/YYYY format from datetimepicker
        sOriginalSlotDate: sOriginalSlotDate, // Original date in DD/MM/YYYY format (null for new records)
        TimeSlotConfigs: timeSlotConfigs
    };

    SaveandUpdate(requestData);

    return false;
});
function SaveandUpdate(DealerSlotConfigData) {
    if (ENABLE_VERBOSE_Logging) console.log(DealerSlotConfigData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(DealerSlotConfigData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.Success && !response.Error) {
                if (DealerSlotConfigData.ConfigID == 0)
                    Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                else if (DealerSlotConfigData.ConfigID > 0)
                    Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                $('#divAddEditModal').modal('hide');
                $("#btnRefresh").click();
            }
            else if (!response.Success && response.Error) {
                Swal.fire({ title: "Data already exists!", text: response.Message || "", icon: "warning", confirmButtonColor: "#556ee6" });
            }
            else if (!response.Success && !response.Error) {
                Swal.fire({ title: "Error", text: response.Message || SaveErrorMessage, icon: "error", confirmButtonColor: "#556ee6" });
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
                var fromDate = null;
                var toDate = null;
                var filterFromDateStr = $("#filterFromDate").val();
                var filterToDateStr = $("#filterToDate").val();
                
                if (filterFromDateStr) {
                    // Convert DD/MM/YYYY to DateTime format for backend
                    var fromParts = filterFromDateStr.split('/');
                    if (fromParts.length === 3) {
                        fromDate = new Date(fromParts[2], fromParts[1] - 1, fromParts[0]);
                    }
                }
                
                if (filterToDateStr) {
                    // Convert DD/MM/YYYY to DateTime format for backend
                    var toParts = filterToDateStr.split('/');
                    if (toParts.length === 3) {
                        toDate = new Date(toParts[2], toParts[1] - 1, toParts[0]);
                    }
                }
                
                // Get sort column name from column data
                var sortColumnName = "SlotDate"; // default
                if (d.order && d.order.length > 0 && d.columns && d.columns[d.order[0].column]) {
                    var colData = d.columns[d.order[0].column].data;
                    // Map column data to actual property names
                    if (colData === "OEMName") sortColumnName = "OEMName";
                    else if (colData === "DealerName") sortColumnName = "DealerName";
                    else if (colData === "SlotDate") sortColumnName = "SlotDate";
                    else if (colData === "TotalCapacity") sortColumnName = "TotalCapacity";
                }
                
                return {
                    Draw: d.draw,
                    Start: d.start,
                    Length: d.length,
                    SearchValue: d.search.value || '',
                    SortColumn: sortColumnName,
                    SortDirection: d.order && d.order.length > 0 ? d.order[0].dir : 'asc',
                    OEMID: $("#filterOEM").val() && $("#filterOEM").val() > 0 ? parseInt($("#filterOEM").val()) : null,
                    DealerID: $("#filterDealer").val() && $("#filterDealer").val() > 0 ? parseInt($("#filterDealer").val()) : null,
                    FromDate: fromDate ? fromDate.toISOString().split('T')[0] : null,
                    ToDate: toDate ? toDate.toISOString().split('T')[0] : null
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
                "data": "SlotDate", 
                "orderable": true,
                "render": function (data, type, row) {
                    // Use sSlotDate if available, otherwise format SlotDate
                    if (row.sSlotDate) {
                        return row.sSlotDate;
                    }
                    if (data) {
                        var date = new Date(data);
                        return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
                    }
                    return '';
                }
            },
            { 
                "data": "TotalTimeSlots", 
                "orderable": false,
                "render": function (data, type, row) {
                    return data || 0;
                }
            },
            { 
                "data": "TotalCapacity", 
                "orderable": true,
                "render": function (data, type, row) {
                    return data || 0;
                }
            },
            {
                "data": "IsActive",
                "render": function (data, type, row) {
                    // Show status badge with active/inactive time slots info
                    var statusHtml = SetStatus(data);
                    if (row.ActiveTimeSlots !== undefined && row.TotalTimeSlots !== undefined) {
                        statusHtml += '<br><small class="text-muted">(' + row.ActiveTimeSlots + '/' + row.TotalTimeSlots + ' active)</small>';
                    }
                    return statusHtml;
                },
                "width": "8%",
                "className": "text-center",
                "orderable": false
            },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    return SetActionButtons(row.ConfigID, _CMPermissions);
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

                $("#divAddEditModal .modal-body :input").attr("disabled", true);
                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Slot Configuration");
            }
            else {
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Slot Configuration");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }
            if (response != null && response.Value != null) {
                var result = response;
                if (!result.Error && result.Success && result.ID > 0) {
                    var DealerSlotConfigData = response.Value;
                    console.log('EditData - Full Response:', response);
                    console.log('EditData - DealerSlotConfigData:', DealerSlotConfigData);
                    
                    // Use sSlotDate directly from the response (already in DD/MM/YYYY format)
                    // If sSlotDate is not available, format it from SlotDate
                    var slotDateValue = DealerSlotConfigData.sSlotDate;
                    if (!slotDateValue && DealerSlotConfigData.SlotDate) {
                        // Fallback: format the date if sSlotDate is not available
                        var date = new Date(DealerSlotConfigData.SlotDate);
                        if (!isNaN(date.getTime())) {
                            var day = String(date.getDate()).padStart(2, '0');
                            var month = String(date.getMonth() + 1).padStart(2, '0');
                            var year = date.getFullYear();
                            slotDateValue = day + '/' + month + '/' + year;
                        }
                    }
                    
                    //console.log('EditData - slotDateValue:', slotDateValue);
                    //console.log('EditData - sSlotDate from response:', DealerSlotConfigData.sSlotDate);
                    //console.log('EditData - SlotDate from response:', DealerSlotConfigData.SlotDate);
                    
                    // Show modal first
                    $('#divAddEditModal').modal('show');
                    
                    // Set values after modal is shown
                    $("#hdnConfigID").val(DealerSlotConfigData.ConfigID);
                    
                    // Use OEMID from the domain response
                    if (DealerSlotConfigData.OEMID && DealerSlotConfigData.OEMID > 0) {
                        $("#ddlOEM").val(DealerSlotConfigData.OEMID).trigger('change');
                        // Wait for dealer list to load, then set dealer
                        setTimeout(function() {
                            $("#ddlDealer").val(DealerSlotConfigData.DealerID);
                        }, 300);
                    } else {
                        $("#ddlDealer").val(DealerSlotConfigData.DealerID);
                    }
                    
                    // Set the date value - use modal shown event to ensure it's set after modal is fully rendered
                    $('#divAddEditModal').on('shown.bs.modal', function () {
                        if (slotDateValue) {
                            //console.log('Modal shown - Setting txtSlotDate value to:', slotDateValue);
                            $("#txtSlotDate").val(slotDateValue);
                            
                            // Try to update datetimepicker if it exists
                            var datePicker = $("#txtSlotDate").data('DateTimePicker');
                            if (datePicker) {
                                try {
                                    var momentDate = moment(slotDateValue, 'DD/MM/YYYY');
                                    if (momentDate.isValid()) {
                                        datePicker.setDate(momentDate);
                                        //console.log('DateTimePicker updated successfully');
                                    } else {
                                        console.log('Invalid moment date:', slotDateValue);
                                    }
                                } catch (e) {
                                    console.log('DateTimePicker setDate error:', e);
                                }
                            } else {
                                console.log('DateTimePicker not found, but value is set:', $("#txtSlotDate").val());
                            }
                        } else {
                            console.log('slotDateValue is empty or null');
                        }
                        // Remove the event handler after first use
                        $('#divAddEditModal').off('shown.bs.modal');
                    });
                    
                    // Also set it immediately as fallback
                    if (slotDateValue) {
                        $("#txtSlotDate").val(slotDateValue);
                    }
                    
                    // Store original date for comparison when saving
                    $("#hdnOriginalSlotDate").val(slotDateValue);
                    
                    // Load existing configurations for this dealer and date (all time slots for this date)
                    if (slotDateValue) {
                        LoadExistingConfigurations(DealerSlotConfigData.DealerID, slotDateValue, true);
                    }
                    // Don't validate on edit, just load
                    $("#divSlotDateWarning").hide().empty();
                    $("#txtSlotDate").removeClass('is-invalid');

                    $("#divRecordLog").show();
                    $("#spnLastUpdatedBy").html("Last Updated By: " + (DealerSlotConfigData.LastUpdatedByName || ""));
                    $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(DealerSlotConfigData.LastUpdatedDate));
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