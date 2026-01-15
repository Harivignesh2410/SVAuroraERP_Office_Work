let selectedIds = [];
$(function () {
    pLoadingSetup(false);
    $("#divAddEdit").hide();
    getRecordList();


    pLoadingSetup(true);
});

$('#btnRefresh').on('click', function () {
    getRecordList();
});
function getRecordList() {
    // Destroy existing instance
    if ($.fn.DataTable.isDataTable('#tblrecordlist')) {
        $('#tblrecordlist').DataTable().clear().destroy();
    }

    // Initialize table and assign it to a variable
    let table = $('#tblrecordlist').DataTable({
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
                    SortDirection: d.order[0].dir
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
            { "data": "EmbossingStationName", "orderable": true },

            { "data": "CompanyName", "orderable": true },
            { "data": "ModeOfTransport", "orderable": true, "width": "10%" },
            { "data": "sGenerateDate", "orderable": true, "width": "10%" },
            { "data": "TotalOrders", "orderable": true, "width": "10%" },

            {
                data: null,
                render: function (data, type, row) {
                    return `
            <button class="btn btn-sm btn-primary btn-details"
                onclick="OrderDetails(${row.GenerateDeliveryID})">
               View Details
            </button>
        `;
                },
                orderable: false,
                className: "text-center",
            }
  
        ]
    });
    $(".dataTables_paginate").addClass("pagination-rounded");
    return false;
}

function OrderDetails(GenerateDeliveryID) {
    $("#divRecords").hide();
    $("#divAddEdit").show();

    $.ajax({
        url: ListOrderDeliveryDataUrl,
        type: "POST",
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: "application/json",
        data: JSON.stringify(GenerateDeliveryID),
        beforeSend: function () {
            $('body').append(`
                <div id="dt-loader" class="skote-loader">
                    <div class="spinner-border text-primary" role="status">
                        <span class="sr-only">Loading...</span>
                    </div>
                </div>
            `);
        },
        success: function (response) {
            if (response && response.length > 0) {
                DisplayDeliveryOrderList(response);
            } else {
                $("#divGenerateDispatchData").html("<p class='text-danger text-center'>No records found for this delivery.</p>");
            }
        },
        complete: function () {
            $('#dt-loader').remove();
        },
        error: function () {
            $("#divGenerateDispatchData").html("<p class='text-danger text-center'>Error loading data.</p>");
        }
    });
}

function DisplayDeliveryOrderList(orderArray) {

    let container = $("#divGenerateDispatchData");

    container.html(`
        <div class="table-responsive mt-2">
            <table id="tblOrderlist" class="table table-bordered align-middle w-100">
                <thead class="table-light">
                    <tr>
                        <th style="width:5%">S.No</th>
                        <th>Order No</th>
                        <th>Order Date</th>
                        <th>Dealer Name & PO No </th>
                        <th>Plate  Colour</th>
                        <th>Front Plate Details</th>
                        <th>Rear Plate Details</th>
                        <th>Registration No</th>
                        <th>Registration Date</th>
                    </tr>
                </thead>
                <tbody id="tbodyOrderlist"></tbody>
            </table>
        </div>
    `);

    let tbody = $("#tbodyOrderlist");
    tbody.empty();

    if (!orderArray || orderArray.length === 0) {
        tbody.html(`
            <tr>
                <td colspan="7" class="text-center text-danger">
                    No records found.
                </td>
            </tr>
        `);
        return;
    }

    orderArray.forEach((item, index) => {
        tbody.append(`
            <tr>
                <td class="text-center">${index + 1}</td>
                <td>${item.OrderNo ?? '-'}</td>
                 <td>${item.sOrderDate ?? '-'}</td>
                <td>${item.Dealer ?? '-'} <br> <span>${item.DealerPONo ?? '-'}</span></td>
                <td>${item.PlateColor ?? '-'}</td>
                <td>${item.FrontPlateSize ?? '-'} <br> <span>${item.FrontLaserSerialNo ?? '-'}</span></td>
                <td>${item.RearPlateSize ?? '-'} <br> <span>${item.RearLaserSerialNo ?? '-'}</span></td>
                <td>${item.RegNo ?? '-'}</td>
                <td>${item.sRegDate ?? '-'}</td>
            </tr>
        `);
    });
}
// 🔹 Close button functionality
$('#btnCloseWindow,#btnClose').on('click', function () {
    $("#divAddEdit").hide();
    $("#divRecords").show();
});
