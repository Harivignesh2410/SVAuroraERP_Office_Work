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
    $("#divAddEditModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Time Slot");
    ClearFormFields();

    return false;
});
function ClearFormFields() {
    $("#divAddEditModal .modal-body :input").attr("disabled", false);
    $("#hdnTimeSlotID").val(0);

    // Remove previous invalid class
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#txtSlotName").val("");
    $("#txtStartTime").val("");
    $("#txtEndTime").val("");
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

    var TimeSlotData = new Object();

    TimeSlotData.TimeSlotID = 0;
    if (this.id == "btnUpdate" && $("#hdnTimeSlotID").val() > 0) TimeSlotData.TimeSlotID = parseInt($("#hdnTimeSlotID").val());

    TimeSlotData.SlotName = $('#txtSlotName').val();
    TimeSlotData.StartTime = $('#txtStartTime').val();
    TimeSlotData.EndTime = $('#txtEndTime').val();
    TimeSlotData.IsActive = $("#chkActive").is(':checked') ? true : false;

    // Validation
    if (!TimeSlotData.SlotName) {
        markInvalid("#txtSlotName", "Please enter Slot Name");
        isValid = false;
    }
    if (!TimeSlotData.StartTime) {
        markInvalid("#txtStartTime", "Please select Start Time");
        isValid = false;
    }
    if (!TimeSlotData.EndTime) {
        markInvalid("#txtEndTime", "Please select End Time");
        isValid = false;
    }

    // Validate that StartTime < EndTime
    if (TimeSlotData.StartTime && TimeSlotData.EndTime) {
        var startTime = new Date('2000-01-01T' + TimeSlotData.StartTime);
        var endTime = new Date('2000-01-01T' + TimeSlotData.EndTime);
        if (startTime >= endTime) {
            markInvalid("#txtEndTime", "End Time must be greater than Start Time");
            isValid = false;
        }
    }

    if (!isValid) return false;

    // Convert time strings to TimeSpan format (HH:mm:ss)
    TimeSlotData.StartTime = TimeSlotData.StartTime + ":00";
    TimeSlotData.EndTime = TimeSlotData.EndTime + ":00";

    SaveandUpdate(TimeSlotData);

    return false;
});
function SaveandUpdate(TimeSlotData) {
    if (ENABLE_VERBOSE_Logging) console.log(TimeSlotData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(TimeSlotData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.Success && !response.Error) {
                if (TimeSlotData.TimeSlotID == 0)
                    Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                else if (TimeSlotData.TimeSlotID > 0)
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
                // Get sort column name from column data
                var sortColumnName = "StartTime"; // default
                if (d.order && d.order.length > 0 && d.columns && d.columns[d.order[0].column]) {
                    var colData = d.columns[d.order[0].column].data;
                    // Map column data to actual property names
                    if (colData === "SlotName") sortColumnName = "SlotName";
                    else if (colData === "StartTime") sortColumnName = "StartTime";
                    else if (colData === "EndTime") sortColumnName = "EndTime";
                }
                
                return {
                    Draw: d.draw,
                    Start: d.start,
                    Length: d.length,
                    SearchValue: d.search.value || '',
                    SortColumn: sortColumnName,
                    SortDirection: d.order && d.order.length > 0 ? d.order[0].dir : 'asc'
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
            { "data": "SlotName", "orderable": true },
            {
                "data": "StartTime",
                "render": function (data, type, row) {
                    if (data) {
                        // Format TimeSpan to HH:mm
                        var timeParts = data.split(':');
                        if (timeParts.length >= 2) {
                            return timeParts[0] + ':' + timeParts[1];
                        }
                        return data;
                    }
                    return "";
                },
                "orderable": true
            },
            {
                "data": "EndTime",
                "render": function (data, type, row) {
                    if (data) {
                        // Format TimeSpan to HH:mm
                        var timeParts = data.split(':');
                        if (timeParts.length >= 2) {
                            return timeParts[0] + ':' + timeParts[1];
                        }
                        return data;
                    }
                    return "";
                },
                "orderable": true
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
                    return SetActionButtons(row.TimeSlotID, _CMPermissions);
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
                $("#divAddEditModal .modal-title").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Time Slot");
            }
            else {
                $("#divAddEditModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Time Slot");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }
            if (response != null && response.Value != null) {
                var result = response;
                if (!result.Error && result.Success && result.ID > 0) {
                    $('#divAddEditModal').modal('show');
                    var TimeSlotData = response.Value;
                    $("#hdnTimeSlotID").val(TimeSlotData.TimeSlotID);
                    $("#txtSlotName").val(TimeSlotData.SlotName);
                    
                    // Format TimeSpan to HH:mm for time input
                    if (TimeSlotData.StartTime) {
                        var startTimeParts = TimeSlotData.StartTime.split(':');
                        if (startTimeParts.length >= 2) {
                            $("#txtStartTime").val(startTimeParts[0] + ':' + startTimeParts[1]);
                        }
                    }
                    if (TimeSlotData.EndTime) {
                        var endTimeParts = TimeSlotData.EndTime.split(':');
                        if (endTimeParts.length >= 2) {
                            $("#txtEndTime").val(endTimeParts[0] + ':' + endTimeParts[1]);
                        }
                    }
                    
                    $("#chkActive").prop('checked', TimeSlotData.IsActive);

                    $("#divRecordLog").show();
                    $("#spnLastUpdatedBy").html("Last Updated By: " + (TimeSlotData.LastUpdatedByName || ""));
                    $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(TimeSlotData.LastUpdatedDate));
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

