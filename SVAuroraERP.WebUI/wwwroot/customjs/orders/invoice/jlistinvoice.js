let selectedIds = [];
$(function () {
    pLoadingSetup(false);
    GetOEMList("ddlOEMFilter", OEMListUrl, _TOKEN);
    getRecordList();
    $("#btnFilter").click();
    $("#divOrderList").hide();
    $("#txtDispatchDate").datetimepicker({
        pickTime: false,
        useCurrent: true,
        format: 'DD/MM/YYYY',
        minYear: 1951,
        maxDate: moment()
    });

    pLoadingSetup(true);
});
$('#ddlOEMFilter').on('change', function () {
    let selectedOEMID = $(this).val();
    GetDealerListByOEMID("ddlDealerFilter", DealerListUrl, _TOKEN, selectedOEMID);
});
$('#btnRefresh').on('click', function () {
    getRecordList();
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
                FilterData = FilterData || {};

                return {
                    Draw: d.draw,
                    Start: d.start,
                    Length: d.length,
                    SearchValue: d.search.value,
                    SortColumn: d.columns[d.order[0].column].data,
                    SortDirection: d.order[0].dir,

                    sStartDate: FilterData.sStartDate ?? "",
                    sEndDate: FilterData.sEndDate ?? "",
                    OEMID: FilterData.OEMID ?? 0,
                    DealerID: FilterData.DealerID ?? 0
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
            { data: null, render: (data, type, row, meta) => meta.row + 1, orderable: false, width: "5%", title: "S No." },
            { data: "InvoiceNo" },
            { data: "sInvoiceDate" },
            { data: "DealerCode" },
            { data: "Dealer" },
            { data: "City" },
            { data: "OrderCount", "className": "text-end", },
            {
                "data": "NetAmount", "orderable": true, "width": "10%", "className": "text-end",
                "render": function (data, type, row) {
                    return `<i class="bx bx-rupee text-primary font-size-16"></i>${row.NetAmount.toFixed(2)}`
                },
            },

            {
                data: null,
                render: function (data, type, row) {
                    return `<div class="btn-group-vertical" role="group" aria-label="Vertical button group">
                                <div class="btn-group" role="group">
                                    <button id="btnGroupVerticalDrop${row.InvoiceID}" type="button"
                                        class="btn btn-sm btn-outline-pink dropdown-toggle"
                                        data-bs-toggle="dropdown" aria-expanded="false">
                                        <i class="bx bx-list-check me-1"></i> Actions
                                    </button>
                                    <ul class="dropdown-menu shadow-sm" aria-labelledby="btnGroupVerticalDrop${row.InvoiceID}">
                                            <li>
                                                <a class="dropdown-item d-flex align-items-center btn-update"
                                                    href="#"
                                                    onclick="OrderDetails(${row.InvoiceID}, '${row.sInvoiceDate}', '${row.InvoiceNo}')">
                                                    <i class="bx bx-pencil text-info me-2"></i> Show Details
                                                </a>

                                            </li>
                                            <li>
                                                <a class="dropdown-item d-flex align-items-center btn-print-order" onclick="PrintInvoiceReport(${row.InvoiceID})">
                                                    <i class="bx bx-printer text-success me-2"></i> Print 
                                                </a>
                                            </li>
                                    </ul>
                                </div>
                            </div>`;
                },
                orderable: false,
                className: "text-center",
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
            // Show child
            rowInstance.child(template[0]).show();
            tr.addClass('shown Details');
            $(this).text("Hide Details");
        }
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
    return false;
}
$('#btnFilter').on('click', function () {
    var FilterData = new Object;
    FilterData.sStartDate = $("#txtStartDate").val() || "",
        FilterData.sEndDate = $("#txtEndDate").val() || "",
        FilterData.OEMID = $("#ddlOEMFilter").val() || 0,
        FilterData.DealerID = $("#ddlDealerFilter").val() || 0,
    getRecordList(FilterData);
});
function GetSelectedOrderIDs() {
    selectedIds = [];
    $('.row-checkbox:checked').each(function () {
        const id = $(this).data('id');
        if (id) {
            selectedIds.push(id);
        }
    });
    return selectedIds;
}
$('#selectAllCheckbox').on('change', function () {
    $('.row-checkbox').prop('checked', this.checked);
});
$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divRecords").show();
    $("#divOrderList").hide();
    $("#divAdvancedFilter").show();

    getRecordList();
    return false;
});
function formatDateDMY(dateStr) {
    if (!dateStr) return "-";

    const date = new Date(dateStr);

    return date.toLocaleDateString("en-GB", {
        day: "2-digit",
        month: "short",
        year: "numeric"
    });
}

function OrderDetails(InvoiceID, sInvoiceDate, InvoiceNo) {
    $("#divRecords").hide();
    $("#divAdvancedFilter").hide();
    $("#divOrderList").show();
    if ($.fn.DataTable.isDataTable('#tblOrderlist')) {
        $('#tblOrderlist').DataTable().clear().destroy();
    }

    // Initialize table and assign it to a variable
    $('#tblOrderlist').DataTable({
        "processing": true,
        "serverSide": true,
        "ordering": true,
        pageLength: 100,
        "ajax": {
            url: ListOrderDataUrl,
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
                    InvoiceID: InvoiceID || 0,
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
            //{
            //    data: null,
            //    orderable: false,
            //    searchable: false,
            //    width: "3%",
            //    render: function (data, type, row) {
            //        return `<input type="checkbox" class="row-checkbox" data-id="${row.InvoiceTransID}" />`;
            //    }
            //},
            { data: null, render: (data, type, row, meta) => meta.row + 1, orderable: false, width: "5%", title: "S No." },
            
            {
                data: null,
                render: function (data, type, row) {
                    return `
                    <span>${InvoiceNo || '-'}</span><br>
                     <span>${sInvoiceDate || '-'}</span>
                `;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
            <span>${row.OrderNo || '-'}</span><br>
            <span>${formatDateDMY(row.OrderDate) || '-'}</span>
        `;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                 return `
                    <span>${row.DealerCode || '-'}</span><br>
                     <span>${row.Dealer || '-'}</span>
                `;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                 return `
                    <span>${row.RegNo || '-'}</span><br>
                     <span>${row.RegDate || '-'}</span>
                `;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                   return `
                        <span>${row.FrontLaserSerialNo || '-'}</span><br>
                         <span>${row.FrontPlateDimension || '-'}</span>
                    `;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                return `
                  <span>${row.RearLaserSerialNo || '-'}</span><br>
                    <span>${row.RearPlateDimension || '-'}</span>
                `;
                }
            },

        ]
    });
    $(".dataTables_paginate").addClass("pagination-rounded");
    return false;
}
function PrintInvoiceReport(ID) {
    PrintReportByID(InvoicePDFExportUrl, "InvoiceID", ID);
    return false;
}