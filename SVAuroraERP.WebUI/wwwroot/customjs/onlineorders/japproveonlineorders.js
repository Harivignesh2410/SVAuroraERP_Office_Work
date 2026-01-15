$(function () {
    pLoadingSetup(false);
    GetOEMList("ddlOEMFilter", OEMListUrl, _TOKEN);
    $("#btnFilter").click();
    pLoadingSetup(true);
});
$('#ddlOEMFilter').on('change', function () {
    let selectedOEMID = $(this).val();
    GetDealerListByOEMID("ddlDealerFilter", DealerListByOEMIDUrl, _TOKEN, selectedOEMID);
});
function getRecordList(FilterData) {
    if ($.fn.DataTable.isDataTable('#tblrecordlist')) {
        $('#tblrecordlist').DataTable().clear().destroy();
    }
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
                    SortDirection: d.order[0].dir,
                    sStartDate: FilterData.sStartDate || "",
                    sEndDate: FilterData.sEndDate || "",
                    OEMID: FilterData.OEMID || 0,
                    DealerID: FilterData.DealerID || 0,
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
            // S No
            {
                data: null,
                render: (data, type, row, meta) => meta.row + 1,
                orderable: false,
                width: "5%",
                className: "text-center"
            },

            // Order No / Date
            {
                data: null,
                render: function (data, type, row) {
                    return `
                <span class="fw-semibold">${row.OrderNo}</span><br>
                <span class="fw-semibold" >${row.sOrderDate}</span>
            `;
                }
            },

            // OEM
            {
                data: "OEMName"
            },

            // Vehicle No / Class Name
            {
                data: null,
                render: function (data, type, row) {
                    return `
                <span>${row.VehicleNo}</span><br>
                <span>${row.VehicleClassName}</span>
            `;
                }
            },

            // Vehicle Plate Type
            {
                data: "VehiclePlateType"
            },

            // Vehicle Plate Size / Color
            {
                data: null,
                render: function (data, type, row) {
                    return `
                <span>${row.VehiclePlateSizeName}</span><br>
                <span>${row.VehiclePlateColorName}</span>
            `;
                }
            },

            // Fitment Type
            {
                data: "FitmentTypeName"
            },

            // Dealer
            {
                data: "DealerName"
            },

            {
                data: null,
                "className": "text-center",
                bSortable: false,
                render: function (data, type, row) {
                    return `<span class="badge ${row.ColorCode}  btn-rounded">
                            ${row.OrderStatusName || ''}
                        </span>`;
                },
                "width": "5%",
                "orderable": false
            },

            // Action (View + Approve)
            {
                data: null,
                orderable: false,
                className: "text-center",
                render: function (data, type, row) {
                    return `
                        <div class="btn-group btn-group-sm">
                            <button class="btn btn-outline-primary"
                                    onclick="viewOnlineOrder(${row.OnlineHSRPOrderID})">
                                <i class="bx bx-show"></i> View
                            </button>

                            <button class="btn btn-outline-success"
                                    onclick="ApproveOnlineOrder(${row.OnlineHSRPOrderID})">
                                <i class="bx bx-check"></i> Approve
                            </button>
                        </div>
                    `;
                }

            }
        ]

    });

    $('#tblrecordlist tbody').off('click', 'button[id^="btnDetails_"]');

    $('#tblrecordlist tbody').on('click', 'button[id^="btnDetails_"]', function () {

        let id = $(this).data('orderid');
        getOrderHistory(id);
        getInvoiceDetails(id);
        getShipmentAndDeliveryDetails(id);
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
            tr.addClass('shown Details');
            $(this).text("Hide Details");

            $(".dataTables_paginate").addClass("pagination-rounded");
            return false;
        }
    });
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
function viewOnlineOrder(hsrpOrderId) {

    $.ajax({
        url: ViewOrderUrl,
        type: "GET",
        data: { hsrpOrderId },
        success: function (response) {

            if (!response || response.Success !== true) {
                Swal.fire("Warning", response?.Message || "Invalid response", "warning");
                return;
            }

            let data = response.data.Value;

            // 🔹 MODE: VIEW
            $('#divembossingstation').hide();
            $('#btnApprove').hide();
            $('#btnReject').hide();

            $('#viewOrderModal').modal('show');

            bindOrderData(data);
        }
    });
}
$('#viewOrderModal').on('shown.bs.modal', function () {
    $('#ddlEmbossingStation').select2({ dropdownParent: $('#viewOrderModal'), width: '100%' });
});
function ApproveOnlineOrder(hsrpOrderId) {

    $.ajax({
        url: ViewOrderUrl,
        type: "GET",
        data: { hsrpOrderId },
        success: function (response) {

            if (!response || response.Success !== true) {
                Swal.fire("Warning", response?.Message || "Invalid response", "warning");
                return;
            }

            let data = response.data.Value;

            // 🔹 MODE: APPROVAL
            $('#divembossingstation').show();
            $('#btnApprove').show();
            $('#btnReject').show();

            // Load Embossing Stations
            GetEmbossingStationByHSRPOnlineOrderID(
                "ddlEmbossingStation",
                EmbossingStationByHSRPOnlineOrderIDUrl,
                _TOKEN,
                hsrpOrderId
            );

            $('#viewOrderModal').modal('show');

            // store order id
            $('#hdnApproveHSRPOrderID').val(hsrpOrderId);

            bindOrderData(data);
        }
    });
}
function bindOrderData(data) {

    $("#lblOrderNo").text(data.OrderNo || "-");
    $("#lblOrderDate").text(data.sOrderDate || "-");
    $("#lblOEM").text(data.OEMName || "-");
    $("#lblDealer").text(data.DealerName || "-");

    $("#lblVehicleNo").text(data.VehicleNo || "-");
    $("#lblVehicleClass").text(data.VehicleClassName || "-");
    $("#lblChasisNo").text(data.ChasisNo || "-");
    $("#lblEngineNo").text(data.EngineNo || "-");

    $("#lblPlateType").text(data.VehiclePlateType || "-");
    $("#lblPlateSize").text(data.VehiclePlateSizeName || "-");
    $("#lblPlateColor").text(data.VehiclePlateColorName || "-");
    $("#lblFitmentType").text(data.FitmentTypeName || "-");

    $("#lblStatus").html(
        `<span class="badge ${data.ColorCode || 'bg-secondary'} px-2 py-1">
            ${data.OrderStatusName || '-'}
        </span>`
    );
}

$('#btnApprove').on('click', function () {
    var Approvedata = new Object();

    Approvedata.OnlineHSRPOrderID = $('#hdnApproveHSRPOrderID').val();
    Approvedata.EmbossingStationID = $('#ddlEmbossingStation').val();
    if (!Approvedata.EmbossingStationID || Approvedata.EmbossingStationID === "0") return markInvalid("#ddlEmbossingStation", " Please Select Embossing Station");

    confirmApproveOnlineOrder(Approvedata);
});
$('#btnReject').on('click', function () {
    var Approvedata = new Object();

    Approvedata.OnlineHSRPOrderID = $('#hdnApproveHSRPOrderID').val();
    Approvedata.EmbossingStationID = 0;

    Rejectorder(Approvedata);
});
function confirmApproveOnlineOrder(Approvedata) {
    Swal.fire({
        title: "Confirm Approval",
        text: "Do you want to approve this order?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Yes, Approve"
    }).then((result) => {

        if (!result.isConfirmed) return;

        $.ajax({
            url: ApproveOrderUrl,
            type: "POST",
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(Approvedata),
            success: function (response) {

                if (!response) {
                    Swal.fire("Error", "Invalid server response", "error");
                    return;
                }
                var response = response.result;

                if (response.Success === true) {
                    Swal.fire({
                        title: "Success!",
                        text: response.Message || "Order approved successfully.",
                        icon: "success",
                        confirmButtonColor: "#556ee6"
                    }).then(() => {
                        $('#btnClose').click();
                        $("#btnFilter").click();
                    });
                }
                else {
                    Swal.fire({
                        title: "Laser Allocation Failed",
                        text: response.Message,
                        icon: "warning",
                        confirmButtonColor: "#556ee6"
                    });
                }
            }
        });
    });
}
function Rejectorder(Approvedata) {

    Swal.fire({
        title: "Confirm Rejection",
        text: "Do you want to reject this order?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Yes, Reject",
        cancelButtonText: "Cancel"
    }).then((result) => {

        if (!result.isConfirmed) return;

        // 🔴 Force rejection rule
        Approvedata.EmbossingStationID = 0;

        $.ajax({
            url: ApproveOrderUrl,   // same API used for approve/reject
            type: "POST",
            headers: {
                "RequestVerificationToken": _TOKEN
            },
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(Approvedata),
            beforeSend: function () {
                pLoadingSetup(false);
            },
            success: function (apiResponse) {

                if (!apiResponse || !apiResponse.result) {
                    Swal.fire("Error", "Invalid server response", "error");
                    return;
                }

                const response = apiResponse.result;

                if (response.Success === true) {
                    Swal.fire({
                        title: "Rejected!",
                        text: "Order rejected successfully.",
                        icon: "success",
                        confirmButtonColor: "#556ee6"
                    }).then(() => {
                        $('#btnClose').click();
                        $("#btnFilter").click();
                    });
                } else {
                    Swal.fire(
                        "Failed",
                        response.Message || "Unable to reject order",
                        "warning"
                    );
                }
            },
            error: function (xhr) {
                Swal.fire(
                    "Server Error",
                    xhr.responseText || "Something went wrong",
                    "error"
                );
            },
            complete: function () {
                pLoadingSetup(true);
            }
        });
    });
}




