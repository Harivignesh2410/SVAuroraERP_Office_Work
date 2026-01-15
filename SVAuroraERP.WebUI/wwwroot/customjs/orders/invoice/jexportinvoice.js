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
$('#btnClearFilter').on('click', function () {
    $("#txtStartDate").val(""),
        $("#txtEndDate").val(""),
        $("#ddlOEMFilter").val(0).change(),
        $("#ddlDealerFilter").val(0).change(),
        $("#btnFilter").click();
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
            { data: "OEM" },
            {
                data: null, "width": "5%",
                render: function (data, type, row) {
                    return `
                          <span>${row.Dealer}</span><br>
                          <span >${row.DealerCode}</span>
                        `;
                }
            },
            { data: "DealerPONo" },
            { data: "PartNo" },
            { data: "Qty" },
            { data: "RegNo" },
            { data: "FrontLaserSerialNo" },
            { data: "RearLaserSerialNo" },
            { data: "PlateColor" },
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

function getInvoiceFilterObject() {
    return {
        sStartDate: $("#txtStartDate").val() || "",
        sEndDate: $("#txtEndDate").val() || "",
        OEMID: $("#ddlOEMFilter").val() || 0,
        DealerID: $("#ddlDealerFilter").val() || 0
    };
}
function getTimeStamp() {
    const d = new Date();
    const pad = n => n.toString().padStart(2, '0');
    return d.getFullYear()
        + pad(d.getMonth() + 1)
        + pad(d.getDate())
        + pad(d.getHours())
        + pad(d.getMinutes())
        + pad(d.getSeconds());
}


$("#btnExportNormal").on('click', function () {

    $.jGrowl("Please wait, exporting invoice data...", {
        sticky: false,
        theme: 'warning',
        life: jGrowlLife
    });

    const filterObject = getInvoiceFilterObject();

    $.ajax({
        url: ExportNormalExcelUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(filterObject),
        xhrFields: { responseType: 'blob' },

        success: function (data, status, xhr) {

            const blob = new Blob([data], {
                type: xhr.getResponseHeader("Content-Type")
            });

            const filename = `Invoice-${getTimeStamp()}.xlsx`;

            const link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            link.download = filename;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        },
        error: function (xhr, status, error) {
            Swal.fire("Error", xhr.responseText || error, "error");
        }
    });

    return false;
});
$("#btnExportHSRP").on('click', function () {

    $.jGrowl("Please wait, exporting HSRP invoice data...", {
        sticky: false,
        theme: 'warning',
        life: jGrowlLife
    });

    const filterObject = getInvoiceFilterObject();

    $.ajax({
        url: ExportHSRPExcelUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(filterObject),
        xhrFields: { responseType: 'blob' },

        success: function (data, status, xhr) {

            const blob = new Blob([data], {
                type: xhr.getResponseHeader("Content-Type")
            });

            const filename = `HSRP-Invoice-${getTimeStamp()}.xlsx`;

            const link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            link.download = filename;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        },
        error: function (xhr, status, error) {
            Swal.fire("Error", xhr.responseText || error, "error");
        }
    });

    return false;
});

