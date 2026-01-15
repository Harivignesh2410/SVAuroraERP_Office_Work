let USERID = $("#hdnUserID").val();
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
$(function () {
    pLoadingSetup(false);
    GetOEMList("ddlOEMFilter", OEMListUrl, _TOKEN);
    GetEmbossingStationByUser("ddlEmbossingStationFilter", EmbossingStationListUrl, _TOKEN, USERID);
    // GetDealerList("ddlDealerFilter", DealerListUrl, _TOKEN);
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
                <span>${ISTtoLocal(row.RegDate)}</span>
            `;
                }
            },
            {
                data: null,
                width: "10%",
                render: function (data, type, row) {
                    return `
            <span >${row.FrontLaserSerialNo || '-'}</span><br>
            <span >${row.FrontPlateDimension || '-'}</span> 
        `;
                }
            },
            {
                data: null,
                width: "10%",
                render: function (data, type, row) {
                    return `
            <span >${row.RearLaserSerialNo || '-'}</span><br>
            <span >${row.RearPlateDimension || ''}</span> 
        `;
                }
            },

            {
                data: null,
                width: "15%",
                render: function (data, type, row) {
                    return `
                <span>${row.DealerCode} - ${row.Dealer}</span><br>
                <span >${row.OEM}</span>
            `;
                }
            },
            {
                data: null,
                width: "10%",
                render: function (data, type, row) {
                    return `
                <span >${row.EngineNo || '-'}</span><br>
                <span >${row.ChasisNo || '-'}</span>
            `;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `<button id="btnDetails_${row.HSRPOrderID}" 
                        data-orderid="${row.HSRPOrderID}" 
                        class="btn btn-sm btn-primary">
                    Show Details
                </button>`;
                },
                orderable: false,
                className: "text-center"
            },
        ]
    });

    $('#tblrecordlist tbody').off('click', 'button[id^="btnDetails_"]');

    $('#tblrecordlist tbody').on('click', 'button[id^="btnDetails_"]', function () {

        let id = $(this).data('orderid');
        getOrderHistory(id);
        getInvoiceDetails(id);
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
                render: data => `<span >${data || "-"}</span>`
            },
            {
                data: "DealerCode",
                title: "Code",
                render: data => `<span >${data || "-"}</span>`
            },
            {
                data: "DealerCity",
                title: "City",
                render: data => `<span>${data || "-"}</span>`
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
