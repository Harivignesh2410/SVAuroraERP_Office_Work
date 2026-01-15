var calendar = null;

$(function () {
    pLoadingSetup(false);
    
    // Initialize filter OEM dropdown
    LoadFilterOEMList();
    
    // Initialize select2 for filter dropdowns
    if ($.fn.select2) {
        $('#filterOEM').select2({ width: '100%' });
        $('#filterDealer').select2({ width: '100%' });
    }
    
    // Filter OEM change handler
    $("#filterOEM").on('change', function () {
        var oemID = $(this).val();
        if (oemID && oemID > 0) {
            LoadFilterDealersByOEMID(oemID);
        } else {
            $("#filterDealer").empty().append('<option value="0">--All Dealers--</option>').val(0);
            if ($.fn.select2 && $("#filterDealer").hasClass('select2-hidden-accessible')) {
                $("#filterDealer").trigger('change.select2');
            }
        }
    });
    
    // Filter button click handler
    $("#btnFilter").on('click', function () {
        loadCalendarData();
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
        
        // Reload calendar with cleared filters
        loadCalendarData();
    });
    
    // Initialize calendar
    initializeCalendar();
    
    // Navigation button handlers
    $("#btnPrev").on('click', function () {
        if (calendar) {
            calendar.prev();
            // Update title after calendar renders
            setTimeout(function () {
                updateCalendarTitle();
                loadCalendarData();
            }, 150);
        }
    });
    
    $("#btnToday").on('click', function () {
        if (calendar) {
            calendar.today();
            // Update title after calendar renders
            setTimeout(function () {
                updateCalendarTitle();
                loadCalendarData();
            }, 150);
        }
    });
    
    $("#btnNext").on('click', function () {
        if (calendar) {
            calendar.next();
            // Update title after calendar renders
            setTimeout(function () {
                updateCalendarTitle();
                loadCalendarData();
            }, 150);
        }
    });
    
    // View type button handlers
    $("#btnViewMonth, #btnViewWeek, #btnViewDay").on('click', function () {
        var viewType = $(this).data('view');
        
        // Update button states
        $("#btnViewMonth, #btnViewWeek, #btnViewDay").removeClass('btn-primary active').addClass('btn-light');
        $(this).removeClass('btn-light').addClass('btn-primary active');
        
        // Change calendar view
        if (calendar) {
            calendar.changeView(viewType);
            updateViewButtonState();
            updateCalendarTitle();
            setTimeout(function () {
                loadCalendarData();
            }, 100);
        }
    });
    
    // Load initial calendar data
    loadCalendarData();
    
    // Update calendar title and view button state initially
    setTimeout(function () {
        updateCalendarTitle();
        updateViewButtonState();
    }, 200);
    
    pLoadingSetup(true);
});

function updateViewButtonState() {
    if (!calendar) {
        return;
    }
    
    var currentView = calendar.getViewName();
    
    // Update button states based on current view
    $("#btnViewMonth, #btnViewWeek, #btnViewDay").removeClass('btn-primary active').addClass('btn-light');
    
    if (currentView === 'month') {
        $("#btnViewMonth").removeClass('btn-light').addClass('btn-primary active');
    } else if (currentView === 'week') {
        $("#btnViewWeek").removeClass('btn-light').addClass('btn-primary active');
    } else if (currentView === 'day') {
        $("#btnViewDay").removeClass('btn-light').addClass('btn-primary active');
    }
}

function LoadFilterOEMList() {
    $("#filterOEM").empty();
    $("#filterOEM").append('<option value="0">--All OEM--</option>');
    
    if (OEMList && OEMList.length > 0) {
        $.each(OEMList, function (index, oem) {
            $("#filterOEM").append('<option value="' + oem.Value + '">' + oem.Text + '</option>');
        });
    }
    
    if ($.fn.select2 && $("#filterOEM").hasClass('select2-hidden-accessible')) {
        $("#filterOEM").trigger('change.select2');
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
                    $("#filterDealer").append('<option value="' + dealer.Value + '">' + dealer.Text + '</option>');
                });
            }
            if ($.fn.select2 && $("#filterDealer").hasClass('select2-hidden-accessible')) {
                $("#filterDealer").trigger('change.select2');
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            $("#filterDealer").empty().append('<option value="0">--All Dealers--</option>');
        }
    });
}

function initializeCalendar() {
    var container = document.getElementById('calendar');
    
    calendar = new tui.Calendar(container, {
        defaultView: 'month',
        taskView: false,
        scheduleView: ['time'],
        useCreationPopup: false,
        useDetailPopup: true,
        calendars: [
            {
                id: 'slot-config',
                name: 'Slot Configuration',
                color: '#ffffff',
                bgColor: '#03bd9e',
                borderColor: '#03bd9e',
                dragBgColor: '#03bd9e'
            }
        ]
    });
    
    // Handle schedule click
    calendar.on('clickSchedule', function (e) {
        var schedule = e.schedule;
        if (schedule && schedule.raw) {
            // You can add custom click handling here
            if (ENABLE_VERBOSE_Logging) console.log('Schedule clicked:', schedule);
        }
    });
    
    // Update title after calendar renders (this fires after navigation)
    calendar.on('afterRenderSchedule', function () {
        updateCalendarTitle();
    });
    
    calendar.on('beforeChangeView', function () {
        setTimeout(function () {
            updateViewButtonState();
            updateCalendarTitle();
            loadCalendarData();
        }, 150);
    });
}

function updateCalendarTitle() {
    if (!calendar) {
        return;
    }
    
    try {
        var viewName = calendar.getViewName();
        var currentDate = calendar.getDate();
        var title = '';
        
        if (typeof moment !== 'undefined') {
            if (viewName === 'month') {
                // For month view, use the first day of the visible month
                var viewStart = calendar.getDateRangeStart();
                if (viewStart) {
                    title = moment(viewStart).format('MMMM YYYY');
                } else {
                    title = moment(currentDate).format('MMMM YYYY');
                }
            } else if (viewName === 'week') {
                var viewStart = calendar.getDateRangeStart();
                var viewEnd = calendar.getDateRangeEnd();
                if (viewStart && viewEnd) {
                    title = moment(viewStart).format('MMM D') + ' - ' + moment(viewEnd).format('MMM D, YYYY');
                } else {
                    title = moment(currentDate).format('MMMM YYYY');
                }
            } else if (viewName === 'day') {
                title = moment(currentDate).format('MMMM D, YYYY');
            } else {
                title = moment(currentDate).format('MMMM YYYY');
            }
        } else {
            // Fallback if moment is not available
            var viewStart = calendar.getDateRangeStart();
            var date = viewStart ? new Date(viewStart) : new Date(currentDate);
            
            if (viewName === 'month') {
                title = date.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
            } else if (viewName === 'week') {
                var viewEnd = calendar.getDateRangeEnd();
                if (viewStart && viewEnd) {
                    var startStr = new Date(viewStart).toLocaleDateString('en-US', { month: 'short', day: 'numeric' });
                    var endStr = new Date(viewEnd).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
                    title = startStr + ' - ' + endStr;
                } else {
                    title = date.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
                }
            } else if (viewName === 'day') {
                title = date.toLocaleDateString('en-US', { month: 'long', day: 'numeric', year: 'numeric' });
            } else {
                title = date.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
            }
        }
        
        $("#calendarTitle").text(title);
    } catch (e) {
        if (ENABLE_VERBOSE_Logging) console.log('Error updating calendar title:', e);
    }
}

function loadCalendarData() {
    if (!calendar) {
        return;
    }
    
    var oemID = $("#filterOEM").val() && $("#filterOEM").val() > 0 ? parseInt($("#filterOEM").val()) : null;
    var dealerID = $("#filterDealer").val() && $("#filterDealer").val() > 0 ? parseInt($("#filterDealer").val()) : null;
    
    // Get current view's date range
    var view = calendar.getViewName();
    var viewStart = calendar.getDateRangeStart();
    var viewEnd = calendar.getDateRangeEnd();
    
    // Add buffer based on view type
    if (typeof moment !== 'undefined') {
        if (view === 'month') {
            viewStart = moment(viewStart).startOf('month').subtract(7, 'days').toDate();
            viewEnd = moment(viewEnd).endOf('month').add(7, 'days').toDate();
        } else if (view === 'week') {
            viewStart = moment(viewStart).startOf('week').subtract(1, 'day').toDate();
            viewEnd = moment(viewEnd).endOf('week').add(1, 'day').toDate();
        } else if (view === 'day') {
            viewStart = moment(viewStart).startOf('day').toDate();
            viewEnd = moment(viewEnd).endOf('day').toDate();
        }
    }
    
    $.ajax({
        url: GetCalendarDataUrl,
        type: 'GET',
        contentType: 'application/json',
        data: {
            OEMID: oemID,
            DealerID: dealerID,
            FromDate: viewStart && typeof moment !== 'undefined' ? moment(viewStart).format('YYYY-MM-DD') : (viewStart ? viewStart.toISOString().split('T')[0] : null),
            ToDate: viewEnd && typeof moment !== 'undefined' ? moment(viewEnd).format('YYYY-MM-DD') : (viewEnd ? viewEnd.toISOString().split('T')[0] : null)
        },
        beforeSend: function () {
            // Show loader
            $('body').append(`
                <div id="calendar-loader" class="skote-loader">
                    <div class="spinner-border text-primary" role="status">
                        <span class="sr-only">Loading...</span>
                    </div>
                </div>
            `);
        },
        success: function (response) {
            if (response && response.Success && response.Value) {
                var schedules = response.Value;
                
                // Clear existing schedules
                calendar.clear();
                
                // Create schedules array for tui-calendar
                var calendarSchedules = schedules.map(function (schedule) {
                    return {
                        id: schedule.id,
                        calendarId: schedule.calendarId || 'slot-config',
                        title: schedule.title,
                        body: schedule.body,
                        start: schedule.start,
                        end: schedule.end,
                        category: schedule.category || 'time',
                        dueDateClass: schedule.dueDateClass || '',
                        color: schedule.color || '#03bd9e',
                        bgColor: schedule.bgColor || '#03bd9e',
                        borderColor: schedule.borderColor || '#03bd9e',
                        raw: schedule.raw || {}
                    };
                });
                
                // Create schedules in calendar
                calendar.createSchedules(calendarSchedules);
            } else {
                calendar.clear();
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            calendar.clear();
            Swal.fire({ 
                title: "Error", 
                text: "Error loading calendar data.", 
                icon: "error", 
                confirmButtonColor: "#556ee6" 
            });
        },
        complete: function () {
            // Hide loader
            $('#calendar-loader').remove();
        }
    });
}


