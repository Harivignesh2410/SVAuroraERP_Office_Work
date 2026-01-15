let selectedIds = [];
$(function () {
    pLoadingSetup(false);
    getSummaryList();
    $("#divSummary").show();
    $("#divOrderList").hide();
    pLoadingSetup(true);
});
$('#btnRefresh').on('click', function () {
    getSummaryList();
});
function getSummaryList() {
    if ($.fn.DataTable.isDataTable('#tblSummarylist')) {
        $('#tblSummarylist').DataTable().clear().destroy();
    }

    $('#tblSummarylist').DataTable({
        processing: true,
        serverSide: false,
        ordering: false,
        searching: true,
        paging: true,
        pageLength: 5,
        ajax: {
            url: SummaryListDataUrl,
            type: "GET",
            headers: { "RequestVerificationToken": _TOKEN },
            dataSrc: function (json) {
                if (json && Array.isArray(json.result.Value)) {
                    return json.result.Value.map((row, index) => ({
                        SNo: index + 1,
                        DealerID: row.DealerID,
                        Dealer: row.Dealer,
                        DealerCode: row.DealerCode,
                        DealerCity: row.DealerCity,
                        DealerPONo: row.DealerPONo,
                        TotalOrders: row.TotalOrders
                    }));
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
        language: {
            oPaginate: {
                sNext: '<i class="mdi mdi-chevron-right"></i>',
                sPrevious: '<i class="mdi mdi-chevron-left"></i>'
            }
        },
        columns: [
            { data: "SNo", title: "S No.", width: "5%", className: "text-center" },
            { data: "DealerPONo", title: "PO No.", width: "10%" },
            {
                data: null,
                render: function (data, type, row) {
                    return `<span >${row.DealerCode}</span>`;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `<span >${row.Dealer}</span>`;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `<span>${row.DealerCity}</span>`;
                }
            },
            {
                data: "TotalOrders",
                title: "Order Count",
                className: "text-center",
                render: function (data) {
                    return `<span>${data}</span>`;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
                            <button class="btn btn-sm btn-primary btn-details" onclick="OrderDetails('${row.DealerID}', '${row.DealerPONo}')">
                                Proceed
                            </button>
                           `;
                },
                orderable: false,
                className: "text-center"
            }

        ]

    });

    return false;
}
function formatDateDMY(dateStr) {
    if (!dateStr) return "-";

    const date = new Date(dateStr);

    return date.toLocaleDateString("en-GB", {
        day: "2-digit",
        month: "short",
        year: "numeric"
    });
}

function OrderDetails(DealerID, DealerPONo) {
    $("#divSummary").hide();
    $("#divOrderList").show();

    if ($.fn.DataTable.isDataTable('#tblOrderlist')) {
        $('#tblOrderlist').DataTable().clear().destroy();
    }

   $('#tblOrderlist').DataTable({
        processing: true,
       serverSide: true,
       ordering: true,
        ajax: {
            url: ListOrderDataUrl,
            headers: { "RequestVerificationToken": _TOKEN },
            type: "POST",
            data: function (d) {
                return {
                    Draw: d.draw,
                    Start: d.start,
                    Length: d.length,
                    SearchValue: d.search.value,
                    SortColumn: d.columns[d.order[0].column].data,
                    SortDirection: d.order[0].dir,
                    DealerID: DealerID || 0,
                    DealerPONo: DealerPONo
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
                orderable: false,
                searchable: false,
                width: "3%",
                render: function (data, type, row) {
                    return `<input type="checkbox"
                   class="row-checkbox form-check-input text-success"
                   data-id="${row.HSRPOrderID}"
                   data-dealerid="${row.DealerID}"
                   checked />`;
                }

            },
            { data: null, render: (data, type, row, meta) => meta.row + 1, orderable: false, width: "5%", title: "S No." },
         
      
            {
                data: null,
                render: function (data, type, row) {
                    return `
            <div>${row.OrderNo}</div>
            <div>${formatDateDMY(row.OrderDate)}</div>`;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
            <div>${row.RegNo}</div>
            <div>${formatDateDMY(row.RegDate)}</div>`;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
            <div>${row.FrontLaserSerialNo}</div>
            <div>${row.FrontPlateDimension}</div>`;
                }
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
            <div>${row.RearLaserSerialNo}</div>
            <div>${row.RearPlateDimension}</div>`; 
                }
            },

            //{
            //    data: null,
            //    render: function (data, type, row) {
            //        return `
            //                <button class="btn btn-sm btn-primary btn-details" onclick="PrintInvoiceReport('${row.InvoiceID}')">
            //                    Show Details
            //                </button>
            //               `;
            //    },
            //    orderable: false,
            //    className: "text-center"
            //}
        ],
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
    return false;
}

function GenerateInvoice(orderInfo) {
    if (!orderInfo || !orderInfo.DealerID || !orderInfo.OrderID) {
        Swal.fire({ title: "Error", text: "Missing order information!", icon: "error" });
        return false;
    }

    $.ajax({
        url: GenerateInvoiceUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(orderInfo),
        success: function (response) {
            if (response && response.success) {
                Swal.fire({
                    title: "Invoice Generated",
                    text: `Invoice No : ${response.result.Message} generated successfully!`,
                    icon: "success"
                });                
                $("#divOrderList").hide();
                $("#divSummary").show();
                getSummaryList();
            } else {
                Swal.fire({ title: "Error", text: response.message || "Failed to generate invoice!", icon: "error" });
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText || "Something went wrong!", icon: "error" });
        }
    });
}

function GetSelectedOrderDetails() {
    let selected = {
        orderIds: [],
        dealerId: null
    };

    $('.row-checkbox:checked').each(function () {
        const orderId = $(this).data('id');
        const dealerId = $(this).data('dealerid');

        if (orderId) {
            selected.orderIds.push(orderId);
        }

        // Capture dealerId from the first checked row
        if (!selected.dealerId && dealerId) {
            selected.dealerId = dealerId;
        }
    });

    return selected;
}
$('#selectAllCheckbox').on('change', function () {
    $('.row-checkbox').prop('checked', this.checked);
});

$("#btnClose, #btnCloseWindow").on('click', function () {
    $("#divSummary").show();
    $("#divOrderList").hide();
    getSummaryList();
    return false;
});
$('#btnCreateInvoice').on('click', function () {
    const selected = GetSelectedOrderDetails();

    if (selected.orderIds.length === 0) {
        alert("Please select at least one order to create an invoice.");
        return;
    }

    var JobcardData = {
        DealerID: selected.dealerId,
        OrderID: selected.orderIds.join(',')
    };
    GenerateInvoice(JobcardData);
});
function PrintInvoiceReport(ID) {
    PrintReportByID(InvoicePDFExportUrl, "InvoiceID", ID);
    return false;
}