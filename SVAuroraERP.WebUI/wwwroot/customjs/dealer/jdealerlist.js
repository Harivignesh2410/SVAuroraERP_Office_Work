$(function () {
    pLoadingSetup(false);
    getRecordList();
    pLoadingSetup(true);
});
$("#btnRefresh").on('click', function () {
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
            {data: "HSRPUserCode"},
            {data: "CompanyName"},         
            {
                data: null, "width": "5%",
                render: function (data, type, row) {
                    return `
                          <span>${row.ContactPerson}</span><br>
                          <span >${(row.ContactNo)}</span>
                        `;
                }
            },
            {
                data: null, "width": "5%",
                render: function (data, type, row) {
                    return `
                          <span >${row.Address1}</span><br>
                          <span >${row.Address2}</span><br>
                          <span >${row.City}</span>
                           <span > - ${row.Pincode}</span>
                        `;
                }
            },
            {
                data: null, "width": "5%",
                render: function (data, type, row) {
                    return `
                          <span >${row.DeliveryAddress1}</span><br>
                          <span >${row.DeliveryAddress2}</span><br>
                          <span >${row.DeliveryCity}</span>
                           <span > - ${row.DeliveryPincode}</span>
                        `;
                }
            }
        ]
    });
    $(".dataTables_paginate").addClass("pagination-rounded");
    return false;
}