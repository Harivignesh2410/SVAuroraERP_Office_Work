_PageTitle = "View Assigned Laser No";

$(function () {
    pLoadingSetup(false);
    GetEmbossingStationList("ddlEmbossingStationFilter", EmbossingStationListUrl, _TOKEN);
    GetLaserNoStockStatusList("ddlStatusFilter", LaserNoStockStatusListUrl, _TOKEN);
    GetVehiclePlateSizeList("ddlSizeFilter", VehiclePlateSizeListUrl, _TOKEN);
    //GetApplicationList("ddlApplication", ApplicationListUrl, _TOKEN);
    $("#btnFilter").click();
    //etSummaryList();
    pLoadingSetup(true);
});

$("#btnFilter").on('click', function () {
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    FilterDPREntery();
    return false;
});
$("#btnClearFilter").on('click', function () {
    ClearFormFieldsForFilter();
    $("#btnFilter").click();
});
function ClearFormFieldsForFilter() {
    $("#ddlEmbossingStationFilter").val("0").change();
    $("#ddlStatusFilter").val("0").change();
    $("#ddlSizeFilter").val("0").change();
    $("#ddlColorFilter").val("0").change();
    $("#txtStartDate").val("");
    $("#txtEndDate").val("");
    return false;
}
function FilterDPREntery() {
    const dateRange = $("#txtFilterDate").val()?.trim();
    let startDate = null;
    let endDate = null;

    if (dateRange) {
        if (dateRange.includes("to")) {
            const parts = dateRange.split("to");
            startDate = parts[0].trim() || null;
            endDate = parts[1].trim() || null;
        } else {
            startDate = dateRange || null;
            endDate = startDate;
        }
    }

    var FilterData = {
        EmbossingStationID: parseInt($('#ddlEmbossingStationFilter').val()) || 0,
        StockStatusID: parseInt($('#ddlStatusFilter').val()) || 0,
        SizeID: parseInt($('#ddlSizeFilter').val()) || 0,
        ColorID: parseInt($('#ddlColorFilter').val()) || 0,

        sStatingDate: startDate && startDate !== "" ? startDate : '',
        sEndingDate: endDate && endDate !== "" ? endDate : '',
    };

    getRecordList(FilterData);
    GetLaserNoStockSummary(FilterData);
}
function getRecordList(FilterData) {
    // Check if DataTable has already been initialized
    if ($.fn.DataTable.isDataTable('#tblrecordlist')) {
        $('#tblrecordlist').DataTable().clear().destroy();
    }

    $('#tblrecordlist').DataTable({
        "processing": true,
        "serverSide": true,
        "ordering": true,
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
                    SortDirection: d.order[0].dir,
                    EmbossingStationID: FilterData.EmbossingStationID,
                    StockStatusID: FilterData.StockStatusID,
                    SizeID: FilterData.SizeID,
                    ColorID: FilterData.ColorID,
                    sStatingDate: FilterData.sStatingDate,
                    sEndingDate: FilterData.sEndingDate,
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
                data: null,
                render: function (data, type, row, meta) {
                    return meta.settings._iDisplayStart + meta.row + 1; // Display row number (S. No.)
                },
                "orderable": false,
                "width": "5%",
            },
            {
                data: "StockInsertedDate",
                orderable: true,
                width: "12%",
                render: function (data) {
                    return `<i class="bx bx-calendar text-primary me-1"></i> ${data || ''}`;
                },
                className: "text-nowrap"
            },
            {
                data: "DispatchNo",
                orderable: true,
                width: "12%",
                render: function (data) {
                    return `<i class="bx bx-package text-info me-1"></i> ${data || ''}`;
                },
                className: "fw-semibold text-dark"
            },
            {
                data: "SerialNo",
                orderable: true,
                width: "12%",
                render: function (data) {
                    return ` ${data || ''}`;
                },
                className: "text-muted"
            },
            {
                data: "EmbossingStationName",
                orderable: true,
                width: "14%",
                render: function (data) {
                    return `<i class="bx bx-building-house text-warning me-1"></i> ${data || ''}`;
                },
                className: "fw-semibold"
            },
            {
                data: "Dimension",
                orderable: true,
                width: "10%",
                render: function (data) {
                    return ` ${data || ''}`;
                },
                className: "text-center text-nowrap"
            },
            {
                data: null,
                "className": "text-center",
                bSortable: false,
                render: function (data, type, row) {
                    return `<span class=" ${row.ColorCode} px-3 py-2">
                            ${row.LaserNoStatus || ''}
                        </span>`;
                },
                "width": "5%",
                "orderable": false
            },
            {
                data: null,
                className: "text-center",
                orderable: false,
                render: function (data, type, row) {
                    return `
                        <button type="button" 
                                class="btn btn-success btn-sm waves-effect waves-light"
                                onclick="GetLaserNoStockLogByID(${row.HSRPLaserNoStockID}, this)">
                            <i class="bx bx-history font-size-18 align-middle me-1"></i> View Status Log
                        </button>`;
                },
                width: "8%"
            }

        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}

function GetLaserNoStockLogByID1(ID, button) {
    const $row = $(button).closest('tr');
    const table = $('#tblrecordlist').DataTable();
    const row = table.row($row);

    // If already shown, hide the accordion
    if (row.child.isShown()) {
        row.child.hide();
        $(button).removeClass("btn-danger").addClass("btn-success")
            .html(`<i class="bx bx-history font-size-18 align-middle me-1"></i> View Status Log`);
        return;
    }

    // Change button state to indicate loading
    $(button).prop("disabled", true)
        .html(`<span class="spinner-border spinner-border-sm"></span> Loading...`);

    // Fetch log data
    $.ajax({
        url: GetLaserNoStockLogByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            let html = "";

            if (response && response.Value && response.Value.length > 0) {
                html += `
                    <div class="accordion-body p-3  rounded shadow-sm">
                        <div class="table-responsive">
                            <table class="table table-bordered  table-sm align-middle mb-0">
                                <thead class="table-warning text-center">
                                    <tr>
                                        <th>Sl.No</th>
                                        <th>Status</th>
                                        <th>Updated By</th>
                                        <th>Updated Date</th>
                                    </tr>
                                </thead>
                                <tbody>
                `;

                response.Value.forEach((log, index) => {
                    html += `
                        <tr>
                            <td class="text-center">${index + 1}</td>
                            <td class="text-center"><span class="${log.ColorCode}">${log.LaserNoStatus ?? '-'} </span></td>
                            <td class="text-center">${log.LastUpdatedByName ?? '-'}</td>
                            <td class="text-center">${formatDate(log.LastUpdatedDate)}</td>
                        </tr>
                    `;
                });

                html += `
                                </tbody>
                            </table>
                        </div>
                    </div>
                `;
            } else {
                html = `<div class="text-center p-3 bg-light rounded">No status log records available.</div>`;
            }

            // Show as child row (accordion style)
            row.child(html).show();

            // Update button style to indicate open
            $(button).prop("disabled", false)
                .removeClass("btn-success").addClass("btn-danger")
                .html(`<i class="bx bx-chevron-up font-size-18 align-middle me-1"></i> Hide Log`);
        },
        error: function (xhr, status, error) {
            console.error("Error fetching log data:", error);
            row.child('<div class="text-center text-danger p-3">Error loading log data.</div>').show();
            $(button).prop("disabled", false);
        }
    });
}

// Utility for date formatting
function formatDate1(dateString) {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleString('en-IN', {
        day: '2-digit', month: 'short', year: 'numeric',
        hour: '2-digit', minute: '2-digit', second: '2-digit'
    });
}


function GetLaserNoStockLogByID(ID, button) {
    // Fetch log data
    $.ajax({
        url: GetLaserNoStockLogByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            let html = "";

            if (response && response.Value && response.Value.length > 0) {
                const logs = response.Value;

                html += `
                    <div class="table-responsive mt-2">
                        <table class="table table-bordered table-striped table-sm align-middle mb-0">
                            <thead class="table-warning text-center">
                                <tr>
                                    <th>Sl.No</th>
                                    <th>Status</th>
                                    <th>Updated By</th>
                                    <th>Updated Date</th>
                                </tr>
                            </thead>
                            <tbody>
                `;

                logs.forEach((log, index) => {
                    html += `
                        <tr>
                            <td class="text-center">${index + 1}</td>
                            <td class="text-center">
                                <span class="${log.ColorCode} fw-semibold">${log.LaserNoStatus ?? '-'}</span>
                            </td>
                            <td class="text-center">${log.LastUpdatedByName ?? '-'}</td>
                            <td class="text-center">${formatDate(log.LastUpdatedDate)}</td>
                        </tr>
                    `;
                });

                html += `
                            </tbody>
                        </table>
                    </div>
                `;
            } else {
                html = `<div class="alert alert-info text-center mb-0 mt-2">No status log records available.</div>`;
            }

            $("#divLaserNoStockLog").html(html);

            $("#divAddEditModal .modal-title").html(`<i class="bx bx-history font-size-20 align-middle me-1"></i> Status Log`);

            $("#divAddEditModal").modal("show");

        },
        error: function (xhr, status, error) {
            console.error("Error fetching log data:", error);
            $("#divLaserNoStockLog").html('<div class="alert alert-danger text-center mb-0">Error loading log data.</div>');
            $("#divAddEditModal").modal("show");
            $(button).prop("disabled", false).html(originalText);
        }
    });
}

// Utility function for date formatting
function formatDate(dateString) {
    if (!dateString) return '-';
    const date = new Date(dateString);
    return date.toLocaleString('en-IN', {
        day: '2-digit', month: 'short', year: 'numeric',
        hour: '2-digit', minute: '2-digit', second: '2-digit'
    });
}


function GetLaserNoStockSummary(Filterdata) {
    $.ajax({
        url: GetLaserNoStockSummaryUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(Filterdata),
        success: function (response) {
            if (response && response.data.length > 0) {
                DisplaySummaryTable(response.data);
            } else {
                $("#divAssignedSummary").html('<div class="alert alert-info text-center mb-0">No summary data available.</div>');
            }
        },
        error: function (xhr, status, error) {
            console.error("Error fetching summary data:", error);
            $("#divAssignedSummary").html('<div class="alert alert-danger text-center mb-0">Error loading summary data.</div>');
        }
    });
}

function DisplaySummaryTable(data) {
    const grouped = {};

    // Group data by Station > Size > Color
    data.forEach(item => {
        const { EmbossingStationID, EmbossingStationName, SizeID, Dimension, ColorID, ColorCode, StockStatusID } = item;

        if (!grouped[EmbossingStationID]) grouped[EmbossingStationID] = { StationName: EmbossingStationName, Sizes: {} };
        if (!grouped[EmbossingStationID].Sizes[SizeID]) grouped[EmbossingStationID].Sizes[SizeID] = { Dimension: Dimension, Colors: {} };
        if (!grouped[EmbossingStationID].Sizes[SizeID].Colors[ColorID]) {
            grouped[EmbossingStationID].Sizes[SizeID].Colors[ColorID] = {
                Color: ColorCode,
                Total: 0, Available: 0, Hold: 0, Assigned: 0, InOrder: 0, Fitted: 0, VahanSubmitted: 0, Cancelled: 0
            };
        }

        const colorGroup = grouped[EmbossingStationID].Sizes[SizeID].Colors[ColorID];
        colorGroup.Total++;

        switch (StockStatusID) {
            case 1: colorGroup.Available++; break;
            case 2: colorGroup.Hold++; break;
            case 3: colorGroup.Assigned++; break;
            case 4: colorGroup.InOrder++; break;
            case 5: colorGroup.Fitted++; break;
            case 6: colorGroup.VahanSubmitted++; break;
            case 7: colorGroup.Cancelled++; break;
        }
    });

    // Build HTML
    let html = '';
    let sno = 1;

    Object.values(grouped).forEach((station, index) => {
        // Initialize totals for this station
        let totals = { Total: 0, Available: 0, Hold: 0, Assigned: 0, InOrder: 0, Fitted: 0, VahanSubmitted: 0, Cancelled: 0 };

        html += `
        <div class="card mb-2 shadow-sm">
            <div class="card-header bg-primary text-white" id="heading${index}">
                <h6 class="mb-0 d-flex justify-content-between align-items-center">
                    ${station.StationName}
                    <button class="btn btn-sm btn-light d-flex align-items-center" 
                            data-bs-toggle="collapse" 
                            data-bs-target="#collapse${index}" 
                            aria-expanded="false" 
                            aria-controls="collapse${index}">
                        <i class="bx bx-chevron-down accordion-icon me-1"></i> View Summary
                    </button>
                </h6>
            </div>
            <div id="collapse${index}" class="collapse" data-bs-parent="#divAssignedSummary">
                <div class="card-body table-responsive p-0">
                    <table class="table   align-middle mb-0">
                        <thead class="table-light text-center">
                            <tr>
                                <th>S.No</th>
                                <th>HSRP Dimension</th>
                                <th>Total</th>
                                <th>Available</th>
                                <th>Hold</th>
                                <th>Assigned</th>
                                <th>In-Order</th>
                                <th>Fitted</th>
                                <th>Vahan Submitted</th>
                                <th>Cancelled</th>
                            </tr>
                        </thead>
                        <tbody>
        `;

        Object.values(station.Sizes).forEach(size => {
            Object.values(size.Colors).forEach(color => {
                html += `
                    <tr class="text-center">
                        <td>${sno++}</td>
                        <td>${size.Dimension}</td>
                        <td>${color.Total}</td>
                        <td>${color.Available}</td>
                        <td>${color.Hold}</td>
                        <td>${color.Assigned}</td>
                        <td>${color.InOrder}</td>
                        <td>${color.Fitted}</td>
                        <td>${color.VahanSubmitted}</td>
                        <td>${color.Cancelled}</td>
                    </tr>
                `;

                // Update totals
                totals.Total += color.Total;
                totals.Available += color.Available;
                totals.Hold += color.Hold;
                totals.Assigned += color.Assigned;
                totals.InOrder += color.InOrder;
                totals.Fitted += color.Fitted;
                totals.VahanSubmitted += color.VahanSubmitted;
                totals.Cancelled += color.Cancelled;
            });
        });

        // Add totals row
        html += `
            <tr class="text-center fw-bold bg-secondary text-white">
                <td colspan="2">Total</td>
                <td>${totals.Total}</td>
                <td>${totals.Available}</td>
                <td>${totals.Hold}</td>
                <td>${totals.Assigned}</td>
                <td>${totals.InOrder}</td>
                <td>${totals.Fitted}</td>
                <td>${totals.VahanSubmitted}</td>
                <td>${totals.Cancelled}</td>
            </tr>
        `;

        html += `
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        `;
    });

    $("#divAssignedSummary").html(html);

    // Toggle icon rotation
    $('#divAssignedSummary').on('show.bs.collapse', '.collapse', function () {
        $(this).prev('.card-header').find('.accordion-icon').addClass('bx-chevron-down align-middle text-primary bx-sm');
    }).on('hide.bs.collapse', '.collapse', function () {
        $(this).prev('.card-header').find('.accordion-icon').removeClass('bx-chevron-down align-middle text-primary bx-sm');
    });
}

function DisplaySummaryTable1(data) {
    const grouped = {};

    // Group data by Station > Size > Color
    data.forEach(item => {
        const { EmbossingStationID, EmbossingStationName, SizeID, Dimension, ColorID, ColorCode, StockStatusID } = item;

        if (!grouped[EmbossingStationID]) grouped[EmbossingStationID] = { StationName: EmbossingStationName, Sizes: {} };
        if (!grouped[EmbossingStationID].Sizes[SizeID]) grouped[EmbossingStationID].Sizes[SizeID] = { Dimension: Dimension, Colors: {} };
        if (!grouped[EmbossingStationID].Sizes[SizeID].Colors[ColorID]) {
            grouped[EmbossingStationID].Sizes[SizeID].Colors[ColorID] = {
                Color: ColorCode,
                Total: 0, Available: 0, Hold: 0, Assigned: 0, InOrder: 0, Fitted: 0, VahanSubmitted: 0, Cancelled: 0
            };
        }

        const colorGroup = grouped[EmbossingStationID].Sizes[SizeID].Colors[ColorID];
        colorGroup.Total++;

        switch (StockStatusID) {
            case 1: colorGroup.Available++; break;
            case 2: colorGroup.Hold++; break;
            case 3: colorGroup.Assigned++; break;
            case 4: colorGroup.InOrder++; break;
            case 5: colorGroup.Fitted++; break;
            case 6: colorGroup.VahanSubmitted++; break;
            case 7: colorGroup.Cancelled++; break;
        }
    });

    // Build HTML
    let html = '';
    let sno = 1;

    Object.values(grouped).forEach((station, index) => {
        html += `
        <div class="card mb-2 shadow-sm">
            <div class="card-header bg-primary text-white" id="heading${index}">
                <h6 class="mb-0 d-flex justify-content-between align-items-center">
                    ${station.StationName}
                    <button class="btn btn-sm btn-light d-flex align-items-center" 
                            data-bs-toggle="collapse" 
                            data-bs-target="#collapse${index}" 
                            aria-expanded="false" 
                            aria-controls="collapse${index}">
                        <i class="bx bx-chevron-down accordion-icon me-1"></i> View Summary
                    </button>
                </h6>
            </div>
            <div id="collapse${index}" class="collapse" data-bs-parent="#divAssignedSummary">
                <div class="card-body table-responsive p-0">
                    <table class="table table-striped table-hover align-middle mb-0">
                        <thead class="table-light text-center">
                            <tr>
                                <th>S.No</th>
                                <th>HSRP Dimension</th>
                                <th>Total</th>
                                <th>Available</th>
                                <th>Hold</th>
                                <th>Assigned</th>
                                <th>In-Order</th>
                                <th>Fitted</th>
                                <th>Vahan Submitted</th>
                                <th>Cancelled</th>
                            </tr>
                        </thead>
                        <tbody>
        `;

        Object.values(station.Sizes).forEach(size => {
            Object.values(size.Colors).forEach(color => {
                html += `
                    <tr class="text-center">
                        <td>${sno++}</td>
                        <td>${size.Dimension}</td>
                        <td>${color.Total}</td>
                        <td><span >${color.Available}</span></td>
                        <td><span >${color.Hold}</span></td>
                        <td><span >${color.Assigned}</span></td>
                        <td><span >${color.InOrder}</span></td>
                        <td><span >${color.Fitted}</span></td>
                        <td><span >${color.VahanSubmitted}</span></td>
                        <td><span >${color.Cancelled}</span></td>
                    </tr>
                `;
            });
        });

        html += `
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        `;
    });

    $("#divAssignedSummary").html(html);

    // Toggle icon rotation
    $('#divAssignedSummary').on('show.bs.collapse', '.collapse', function () {
        $(this).prev('.card-header').find('.accordion-icon').addClass('bx-chevron-down align-middle text-primary bx-sm');
    }).on('hide.bs.collapse', '.collapse', function () {
        $(this).prev('.card-header').find('.accordion-icon').removeClass('bx-chevron-down align-middle text-primary bx-sm');
    });
}




