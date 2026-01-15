_PageTitle = "Laser No Allocation";
let flatData = [];
let selectedIds = [];
let validOrderIdsCsv = "";
let USERID = $("#hdnUserID").val();
$(function () {
    pLoadingSetup(false);
    //GetOEMList("ddlOEMFilter", OEMListUrl, _TOKEN);
    GetEmbossingStationByUser("ddlEmbossingStationFilter", EmbossingStationListUrl, _TOKEN, USERID);
    //GetDealerList("ddlDealerFilter", DealerListUrl, _TOKEN);
    //GetDealerListByOEMID("ddlDealerFilter", DealerListByOEMIDUrl, _TOKEN, $('#ddlOEMFilter').val());

    GetOrderTypeList("ddlOrderType", OrderTypeListUrl, _TOKEN)
    $("#btnFilter").click();
    getSummaryList();
    //getOrderHistory(orderId);
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
                orderable: false,
                searchable: false,
                width: "3%",
                render: function (data, type, row) {
                    return `<input type="checkbox" class="row-checkbox" data-id="${row.HSRPOrderID}" />`;
                }
            },

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
                    return HsrpGroupAction(row.HSRPOrderID);
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
                <span>${row.RegNo}</span><br>
                <span >${ISTtoLocal(row.RegDate)}</span>
            `;
                }
            },
            {
                data: null,
                width: "15%",
                render: function (data, type, row) {
                    return `
                <span >${row.DealerCode} - ${row.Dealer}</span><br>
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
        let tr = $(this).closest('tr');
        let rowInstance = table.row(tr);

        // Toggle child row
        if (tr.hasClass('shown')) {
            rowInstance.child.hide();
            tr.removeClass('shown Details');
            $(this).text("Show Details");
        } else {
            let template = $('#childRowTemplate').clone().removeClass('d-none');
            let record = rowInstance.data();

            if (!record) {
                console.error('Record not found for ID:', id);
                return;
            }

            // Fill child fields
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

            rowInstance.child(template[0]).show();
            getInvoiceDetails(id);
            tr.addClass('shown Details');
            $(this).text("Hide Details");
        }
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
    return false;
}
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
$('#btnAddNew,#btnAddNewBottom').on('click', function () {
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
                   <div id="CheckAvailableOrderLaserNo"></div>
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
    CheckAvailableOrderLaserNo();

}

$('#btnProceed').on('click', function () {

    if (!validOrderIdsCsv || validOrderIdsCsv.length === 0) {
        return Swal.fire(
            "Warning",
            "No valid orders available for allocation.",
            "warning"
        );
    }
    const LaserNoStockData = {
        OrderIds: validOrderIdsCsv.join(',')
    };


    SaveandUpdate(LaserNoStockData);
});


function SaveandUpdate(LaserNoStockData) {
    //if (ENABLE_VERBOSE_Logging) //console.log(DocumentTypeData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(LaserNoStockData),
        success: function (response) {
            //  if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response != null && response.result != null) {
                var result = response.result;
                if (!result.Error && result.Success) {
                    let parts = result.Message.split('|').map(s => s.trim());

                    // Convert to object
                    let summary = {};
                    parts.forEach(p => {
                        let [key, value] = p.split(':').map(x => x.trim());
                        summary[key] = parseInt(value);
                    });

                    // Decide alert type
                    let alertType = "success";
                    let alertTitle = "Success!";

                    if (summary["Total Laser No Assigned"] === 0) {
                        alertType = "warning";
                        alertTitle = "Warning!";
                    }

                    if (summary["Front Laser No Available"] === 0 || summary["Rear Laser No Available"] === 0) {
                        alertType = "error";
                        alertTitle = "Error!";
                    }

                    // Format message with <br>
                    let formattedSummary = parts.join('<br>');

                    // Show SweetAlert
                    Swal.fire({
                        title: alertTitle,
                        html: formattedSummary,
                        icon: alertType
                    }).then(() => {
                        $('#divAddEditModal').modal('hide');
                        getSummaryList();
                        $("#btnFilter").click();
                    });

                }
                else if (!result.Success && result.Error) {
                    Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
                }
                else if (!result.Success && !result.Error) {
                    Swal.fire({ title: "Error", text: SaveErrorMessage, icon: "error", confirmButtonColor: "#556ee6" });
                }
                else
                    Swal.fire({ title: "Warning", text: result.Message, icon: "Warning", confirmButtonColor: "#556ee6" });
            }
            else
                Swal.fire({ title: "Error", text: result.message, icon: "error", confirmButtonColor: "#556ee6" });

        },
        error: function (xhr, status, error) {
            //  if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
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
                data: "DealerCode",
                title: "Code",
                render: data => `<span>${data || "-"}</span>`
            },
            {
                data: "Dealer",
                title: "Dealer",
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

$('#btnRefresh').on('click', function () {
    $("#btnFilter").click();
});
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

    // capture current page URL
    let returnUrl = encodeURIComponent(window.location.href);

    window.location.href =
        `/Orders/ManageOrder/UpdateOrderData?orderId=${orderId}&returnUrl=${returnUrl}`;
});
async function CheckAvailableOrderLaserNo() {

    let selectedIds = GetSelectedOrderIDs(); // returns array

    if (!selectedIds || selectedIds.length === 0) {
        await Swal.fire("Warning", "Please select at least one order", "warning");
        return;
    }

    // Remove duplicates & convert to CSV
    const finalSelectedIds = [...new Set(selectedIds)];
    const orderIdsCsv = finalSelectedIds.join(",");

    const request = {
        OrderIds: orderIdsCsv
    };

    try {
        const response = await $.ajax({
            url: CheckAvailableOrderLaserNoDataUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(request)
        });

        if (response.success) {

            const data = response.result.Value;

            // ✅ SAVE valid order IDs (CSV) for next step
            validOrderIdsCsv = data.ValidOrderIds;

            let summaryHtml = `
                <div>
                    <strong class="text-warning">Laser Availability Summary</strong>
                    <hr class="my-2"/>
                </div>

                <div class="table-responsive">
                    <table class="table table-bordered table-sm mb-0 align-middle">
                        <tbody>
                            <tr>
                                <th class="text-start">Total Orders</th>
                                <td><b>${data.Summary.TotalOrders}</b></td>
                            </tr>
                            <tr>
                                <th class="text-start">Front Laser Available</th>
                                <td><b>${data.Summary.FrontLaserAvailable}</b></td>
                            </tr>
                            <tr>
                                <th class="text-start">Rear Laser Available</th>
                                <td><b>${data.Summary.RearLaserAvailable}</b></td>
                            </tr>
                            <tr>
                                <th class="text-start">Ready to Import</th>
                                <td>
                                    <b>${data.Summary.BothLaserAvailable}</b>
                                    <span class="ms-1">(Only these will be allocated)</span>
                                </td>
                            </tr>
                            <tr>
                                <th class="text-start">Unable to Process</th>
                                <td><b>${data.Summary.RejectedCount}</b></td>
                            </tr>
                            <tr>
                                <th class="text-start">Not Processed Reason</th>
                                <td>${data.Summary.RejectedReasons ?? '—'}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            `;

            $("#CheckAvailableOrderLaserNo").html(summaryHtml);
        }
        else {
            Swal.fire("Error", response.message, "error");
        }
    }
    catch (err) {
        console.error(err);
        Swal.fire("Error", "Error while checking laser availability.", "error");
    }
}