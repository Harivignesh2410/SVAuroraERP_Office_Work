_PageTitle = "Complete QC";
let flatData = [];
let selectedIds = [];
let USERID = $("#hdnUserID").val();
$(function () {
    pLoadingSetup(false);
    GetOEMList("ddlOEMFilter", OEMListUrl, _TOKEN);
    GetEmbossingStationByUser("ddlEmbossingStationFilter", EmbossingStationListUrl, _TOKEN, USERID);
    //  GetDealerList("ddlDealerFilter", DealerListUrl, _TOKEN);
    GetOrderTypeList("ddlOrderType", OrderTypeListUrl, _TOKEN)
    $("#btnFilter").click();
    getSummaryList();
    pLoadingSetup(true);
});

$('#ddlOEMFilter').on('change', function () {
    let selectedOEMID = $(this).val();
    GetDealerListByOEMID("ddlDealerFilter", DealerListByOEMIDUrl, _TOKEN, selectedOEMID);
});
function getRecordList(FilterData) {
    // Destroy existing instance
    if ($.fn.DataTable.isDataTable('#tblrecordlist')) {
        $('#tblrecordlist').DataTable().clear().destroy();
    }

    // Initialize table and assign it to a variable
    let table = $('#tblrecordlist').DataTable({
        "processing": true,
        "serverSide": true,
        "ordering": true,
        "pageLength": 100,
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
                    sStartDate: FilterData.sStartDate || "",
                    sEndDate: FilterData.sEndDate || "",
                    orderTypeID: FilterData.orderTypeID || 0,
                    OEMID: FilterData.OEMID || 0,
                    EmbossingStationID: FilterData.EmbossingStationID || 0,
                    DealerID: FilterData.DealerID || 0,
                    SearchText: FilterData.SearchText || ""
                };
            },
            beforeSend: function () {
                $('body').append(`
                    <div id="dt-loader" class="skote-loader">
                        <div class="spinner-border text-primary" role="status">
                            <span class="sr-only">Loading...</span>
                        </div>
                    </div>
                `);
            },
            complete: function () {
                $('#dt-loader').remove();
            }
        },
        language: {
            oPaginate: {
                sNext: '<i class="mdi mdi-chevron-right"></i>',
                sPrevious: '<i class="mdi mdi-chevron-left"></i>'
            }
        },
        columns: [


            {
                data: null,
                render: function (data, type, row, meta) {
                    return meta.settings._iDisplayStart + meta.row + 1;
                },
                orderable: false,
                width: "5%"
            },
            {
                data: null,
                render: function (data, type, row) {
                    return HsrpGroupViewAction(row.HSRPOrderID);
                },
                orderable: false,
                className: "text-center",
                width: "8%"
            },
            {
                data: null, // null because we’ll combine multiple fields manually
                orderable: false,
                render: function (data, type, row) {
                    return `
                            <div>
                                <div>${row.OrderNo}</div>
                              <div>${ISTtoLocal(row.OrderDate)}&nbsp;${row.OrderTypeID == 2 ? `<span class="badge" style="background-color:#F70306 
                              ;">${row.OrderTypeName}</span>` : ""}</div>                          
                            </div>
                        `;//F70306//03F7F3
                }
            },
            {
                data: "Description",
                orderable: true,
                width: "10%",
                className: "text-center text-light",
                render: function (data, type, row) {
                    return `<span class="${row.ColorCode}">${row.Description}</span>`;
                }
            },

            {
                data: "ProcessDate",
                render: function (data) {
                    return ISTtoLocal(data);
                },
                width: "10%"
            },

            {
                data: null,
                width: "10%",
                render: function (data, type, row) {
                    return `
                <span >${row.RegNo}</span><br>
                <span >${ISTtoLocal(row.RegDate)}</span>
            `;
                }
            },
            {
                data: null,
                width: "10%",
                render: function (data, type, row) {
                    return `
              <span >${row.FrontLaserSerialNo || '-'}</span><br>
            <span >${row.FrontPlateSize || '-'}</span> -
             <span >${row.PlateColor || '-'}</span>
        `;
                }
            },
            {
                data: null,
                width: "10%",
                render: function (data, type, row) {
                    return `
             <span >${row.RearLaserSerialNo || '-'}</span><br>
            <span >${row.RearPlateSize || '-'}</span> -
             <span >${row.PlateColor || '-'}</span>
        `;
                }
            },


            {
                data: null,
                width: "15%",
                render: function (data, type, row) {
                    return `
                <span>${row.DealerCode} - ${row.Dealer}</span><br>
                <span>${row.OEM}</span>
            `;
                }
            },
            {
                data: null,
                width: "10%",
                render: function (data, type, row) {
                    return `
                <span>${row.EngineNo || '-'}</span><br>
                <span >${row.ChasisNo || '-'}</span>
            `;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `<button class="btn btn-sm btn-primary btn-details" data-orderid="${row.HSRPOrderID}">Show Details</button>`;
                },
                orderable: false,
                className: "text-center",
            },
        ]
    });

    $('#tblrecordlist tbody').off('click', '.btn-details');

    // Use event delegation on the table body
    $('#tblrecordlist tbody').on('click', '.btn-details', function () {
        let id = $(this).data('orderid');
        let tr = $(this).closest('tr');
        let rowInstance = table.row(tr);

        // Toggle child row
        if (tr.hasClass('shown')) {
            rowInstance.child.hide();
            tr.removeClass('shown Details');
            $(this).text("Show Details");
        } else {
            // Clone child template
            let template = $('#childRowTemplate').clone().removeClass('d-none');
            let record = rowInstance.data();

            if (!record) {
                console.error('Record not found for ID:', id);
                return;
            }

            // Populate child data
            template.find('[data-field]').each(function () {
                let field = $(this).data('field');
                let value = record[field] || '-';

                if (field === 'sOrderDate' || field === 'sRegDate') {
                    value = ISTtoLocal(value);
                }

                if ($(this).is('a'))
                    $(this).attr('href', value !== '-' ? value : '#');
                else
                    $(this).text(value);
            });

            // Build status timeline
            const currentStatusID = record.OrderStatusID;
            const timelineContainer = template.find('.status-timeline');
            timelineContainer.empty();

            const reversedIds = Object.keys(statusMap).reverse();
            for (let statusId of reversedIds) {
                statusId = parseInt(statusId);
                if (statusId > currentStatusID) continue;

                const iconHtml =
                    statusId < currentStatusID
                        ? '<i class="bx bxs-check-circle text-success font-size-18"></i>'
                        : '<i class="bx bxs-right-arrow-circle bx-fade-right text-primary font-size-18"></i>';

                const date = statusDates[statusId] ? ISTtoLocal(statusDates[statusId]) : '';

                const li = $(`
                <li class="event-list ${statusId === currentStatusID ? 'active' : ''}">
                    <div class="event-timeline-dot">
                        ${iconHtml}
                    </div>
                    <div class="d-flex align-items-start mt-3 mb-4">
                        <div class="flex-shrink-0 me-4">
                            <h5 class="font-size-16 mb-0 d-flex align-items-center">
                                ${statusIcons[statusId]}
                                <span class="ms-3 fw-semibold">${statusMap[statusId]}</span>
                                ${statusId === currentStatusID ? '<i class="bx bx-right-arrow-alt font-size-16 text-primary align-middle ms-3"></i>' : ''}
                            </h5>
                            <h6 class="text-warning ms-4 mt-2 fw-normal">${date || ''}</h6>
                        </div>
                    </div>
                </li>
            `);

                timelineContainer.append(li);
            }

            // Show child
            rowInstance.child(template[0]).show();
            tr.addClass('shown Details');
            $(this).text("Hide Details");
        }
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
    return false;
}

function ISTtoLocal(istDate) {
    // Create a Date object from the UTC timestamp
    const date = new Date(istDate);

    // Format the date to the desired format
    const formattedDate = date.toLocaleString("en-GB", {
        day: "2-digit",
        month: "short",
        year: "numeric",
        //hour: "2-digit",
        //minute: "2-digit",
        //second: "2-digit",
        //hour12: true,
    });

    return formattedDate;
}
const statusDates = {
    1: "2025-10-03T09:00:00",
    2: "2025-10-04T09:15:00",
    3: "2025-10-05T11:00:00",
    4: "2025-10-06T13:20:00",
    5: "2025-10-07T10:30:00", // Current status
    6: "2025-10-08T10:30:00",
    7: "2025-10-09T10:30:00",
};

$('#btnFilter').on('click', function () {
    var FilterData = new Object;
    FilterData.sStartDate = $("#txtStartDate").val() || "",
        FilterData.sEndDate = $("#txtEndDate").val() || "",
        FilterData.orderTypeID = $("#ddlOrderType").val() || 0,
        FilterData.OEMID = $("#ddlOEMFilter").val() || 0,
        FilterData.EmbossingStationID = $("#ddlEmbossingStationFilter").val() || 0,
        FilterData.DealerID = $("#ddlDealerFilter").val() || 0,
        FilterData.SearchText = $("#txtSearchbox").val() || ""
    getRecordList(FilterData);
});
$('#btnClearFilter').on('click', function () {
    $("#txtStartDate").val(""),
        $("#txtEndDate").val(""),
        $("#ddlOrderType").val(0).change(),
        $("#ddlOEMFilter").val(0).change(),
        $("#ddlEmbossingStationFilter").val(0).change(),
        $("#ddlDealerFilter").val(0).change(),
        $("#txtSearchbox").val("")
    $("#btnFilter").click();
});

$('#btnRefresh').on('click', function () {
    $("#btnFilter").click();
});

$('#btnAddNew').on('click', function () {
    handleAssignOrAddNew();
});

$(document).off("click", ".btn-assign").on("click", ".btn-assign", function (e) {
    e.preventDefault();

    // ✅ Automatically check the checkbox for this row
    const row = $(this).closest("tr");
    const checkbox = row.find(".row-checkbox");
    checkbox.prop("checked", true);

    // Proceed with the modal flow
    handleAssignOrAddNew();
});


function handleAssignOrAddNew() {
    let selectedIds = GetSelectedOrderIDs();
    let count = selectedIds.length;

    if (count === 0) {
        return markInvalid("", "Please select at least one Order");
    }

    // Build modal content dynamically
    let modalBodyHtml = `
        <div class="text-center my-3">
            <h5 class="fw-semibold text-primary">
                You have selected <span class="text-danger">${count}</span> order${count > 1 ? 's' : ''}.
            </h5>
            <p class="text-muted mt-2">Do you want to proceed with the next step?</p>     
        </div>
    `;

    // Set modal title and body
    $("#divAddEditModal .modal-title").html(`
        <i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>
        Assign / Add New ${_PageTitle}
    `);
    $("#divAddEditModal .modal-body").html(modalBodyHtml);

    // Show modal
    $('#divAddEditModal').modal('show');
}
$('#btnProceed').on('click', function () {
    let selectedData = GetSelectedOrderDetails();

    if (selectedData.length === 0)
        return Swal.fire("Warning", "Please select at least one order", "warning");

    //const uniqueDealers = [...new Set(selectedData.map(x => x.DealerID))];
    //if (uniqueDealers.length > 1)
    //    return Swal.fire("Validation Failed", "Multiple dealers selected. Only one dealer allowed per Job Card.", "error");

    const uniqueEmbossing = [...new Set(selectedData.map(x => x.EmbossingStationID))];
    if (uniqueEmbossing.length > 1)
        return Swal.fire("Validation Failed", "Orders must belong to a single Embossing Station.", "error");

    const FinalselectedIds = [...new Set(selectedData.map(x => x.HSRPOrderID))];
    const LaserNoStockData = {
        OrderIds: FinalselectedIds.join(','),
        //DealerID: uniqueDealers[0],
        EmbossingID: uniqueEmbossing[0]
    };

    SaveandUpdate(LaserNoStockData);
});

function GetSelectedOrderIDs() {
    selectedIds = [];
    $('.row-checkbox:checked').each(function () {
        const id = $(this).data('id');
        if (id) selectedIds.push(id);
    });
    return selectedIds;
}

function GetSelectedOrderDetails() {
    let selectedData = [];
    $('.row-checkbox:checked').each(function () {
        const orderid = $(this).data('id');
        const dealerid = $(this).data('dealerid');
        const embossingid = $(this).data('embossingid');

        if (orderid)
            selectedData.push({
                HSRPOrderID: orderid,
                //DealerID: dealerid,
                EmbossingStationID: embossingid
            });
    });
    return selectedData;
}

$('#selectAllCheckbox').on('change', function () {
    $('.row-checkbox').prop('checked', this.checked);
});

function SaveandUpdate(LaserNoStockData) {
    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(LaserNoStockData),
        success: function (response) {
            if (response && response) {
                var result = response;

                if (result.success) {
                    Swal.fire({
                        title: "Success!",
                        text: result.Message || "Data saved successfully.",
                        icon: "success",
                        confirmButtonColor: "#556ee6"
                    }).then(() => {
                        $('#divAddEditModal').modal('hide');
                        getSummaryList();
                        $("#btnFilter").click();
                    });
                }
                else {
                    Swal.fire({
                        title: "Error",
                        text: result.Message || "Something went wrong while saving data.",
                        icon: "error",
                        confirmButtonColor: "#556ee6"
                    });
                }
            }
            else {
                Swal.fire({
                    title: "Error",
                    text: "Invalid server response.",
                    icon: "error",
                    confirmButtonColor: "#556ee6"
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: "Error",
                text: error.responseText || "An unexpected error occurred.",
                icon: "error",
                confirmButtonColor: "#556ee6"
            });
        }
    });
    return false;
}
function getSummaryList() {
    if ($.fn.DataTable.isDataTable('#tblSummarylist')) {
        $('#tblSummarylist').DataTable().clear().destroy();
    }

    $('#tblSummarylist').DataTable({
        processing: false,
        serverSide: false,
        ordering: false,
        searching: false,
        paging: false,
        ajax: {
            url: SummaryListDataUrl,
            type: "GET",
            headers: { "RequestVerificationToken": _TOKEN },
            dataSrc: function (json) {
                if (json && Array.isArray(json.result.Value.lstLaserNoSummary)) {
                    const data = json.result.Value.lstLaserNoSummary.map((row, index) => ({
                        SNo: index + 1,
                        Dealer: row.Dealer,
                        DealerCode: row.DealerCode,
                        DealerCity: row.DealerCity,
                        Count: row.PendingCount
                    }));

                    const totalOrders = data.reduce((sum, r) => sum + (parseInt(r.Count) || 0), 0);
                    setTimeout(() => {
                        if (!$("#tblSummarylist tfoot").length) {
                            $("#tblSummarylist").append(`
                                <tfoot class="table-info fw-bold">
                                    <tr>
                                        <td colspan="4" class="text-end">Total Orders</td>
                                        <td class=" text-center">${totalOrders}</td>
                                    </tr>
                                </tfoot>
                            `);
                        }
                    }, 100);

                    return data;
                }
                return [];
            },
            beforeSend: function () {
                $('body').append(`
                    <div id="dt-loader" class="skote-loader">
                        <div class="spinner-border text-primary" role="status">
                            <span class="sr-only">Loading...</span>
                        </div>
                    </div>
                `);
            },
            complete: function () {
                $('#dt-loader').remove();
            }
        },
        dom: 't',
        language: {
            emptyTable: "No records found"
        },
        columns: [
            { data: "SNo", title: "S.No", width: "5%" },
            {
                data: "Dealer",
                title: "Dealer",
                render: data => `<span>${data || "-"}</span>`
            },
            {
                data: "DealerCode",
                title: "Code",
                render: data => `<span >${data || "-"}</span>`
            },
            {
                data: "DealerCity",
                title: "City",
                render: data => `<span >${data || "-"}</span>`
            },
            {
                data: "Count",
                title: "Order Count",
                className: "text-center",
                render: data => `<span>${data || 0}</span>`
            }
        ],
        createdRow: function (row, data) {
            $(row).addClass("align-middle");
        },
        initComplete: function () {
            $("#tblSummarylist").addClass("table table-bordered table-hover table-sm align-middle");
        }
    });

    return false;
}
function PrintOrderReport(ID) {
    PrintReportByID(OrderReportPDFExportUrl, "OrderID", ID);
    return false;
}
function PrintTLPSticker(ID) {
    PrintReportByID(TLPStickerPDFExportUrl, "OrderID", ID);
    return false;
}

$(document).on("click", ".btn-update", function (e) {
    e.preventDefault();

    let orderId = $(this).data("orderid");
    if (!orderId) {
        console.error("Order ID missing");
        return;
    }

    // Redirect to UpdateOrderData page with ID in query string
    window.location.href = `/Orders/ManageOrder/UpdateOrderData?orderId=${orderId}`;
});