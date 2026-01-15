$(function () {
    pLoadingSetup(false);
    getRecordList();
    $("#divOrderList").hide();


    pLoadingSetup(true);
});


$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divRecords").show();
    $("#divOrderList").hide();

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

            { data: "CompanyName" },

            { data: "TotalOrders" },
            {
                data: null,
                render: function (data, type, row) {
                    return ` <button class="btn btn-sm btn-primary btn-details"
                onclick="OrderDetails(${row.GenerateDeliveryID})">
                Show Details
            </button>`;
                },
                orderable: false,
                className: "text-center",
            },
        ]
    });
    $(".dataTables_paginate").addClass("pagination-rounded");
    return false;
}

let currentGenerateDeliveryID = 0;
function OrderDetails(GenerateDeliveryID) {
    $("#divRecords").hide();
    $("#divOrderList").show();

    currentGenerateDeliveryID = GenerateDeliveryID;


    $.ajax({
        url: ListOrderDataUrl,
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
                DisplayOrderList(response);
            } else {
                $("#divTableData").html("<p class='text-danger'>No data found for this dealer.</p>");
            }
        },
        complete: function () {
            $('#dt-loader').remove();
        },
        error: function () {
            $("#divTableData").html("<p class='text-danger'>Error loading data.</p>");
        }
    });
}


function DisplayOrderList(orderArray) {
    let table = $('#tblOrderlist');

    if ($.fn.DataTable.isDataTable(table)) {
        table.DataTable().clear().destroy();
    }

    let tbody = table.find('tbody');
    tbody.empty();

    if (!orderArray || orderArray.length === 0) {
        tbody.html(`<tr><td colspan="6" class="text-center text-danger">No records found.</td></tr>`);
    } else {
        orderArray.forEach((item, index) => {
            tbody.append(`
                <tr>
                    <td>${index + 1}</td>
                    <td>${item.OrderNo || "-"}<br/>${formatDateDMY(item.OrderDate) || "-"}</td>
                    <td>${item.RegNo || "-"}<br/>${formatDateDMY(item.RegDate) || "-"}</td>
                    <td>${item.FrontLaserSerialNo || "-"}<br/>${item.FrontPlateDimension || "-"}</td>
                    <td>${item.RearLaserSerialNo || "-"}<br/>${item.RearPlateDimension || "-"}</td>
                    <td>${formatDateDMY(item.DeliveredDate )|| "-"}</td>
                </tr>
            `);
        });
    }

    table.DataTable({
        "bAutoWidth": false,
        "bPaginate": false,
        "bFilter": true,
        "bSort": true,
        "order": []
    });

}