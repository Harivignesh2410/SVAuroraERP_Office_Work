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
                data: "PlateTypeName"
            },

            // Vehicle Plate Size / Color
            {
                data: null,
                render: function (data, type, row) {
                    return `
                <span>${row.PlateSizeName}</span><br>
                <span>${row.PlateColorName}</span>
            `;
                }
            },

            // Replacement Reason
            {
                data: "ReplacementReasonName"
            },

            // Dealer
            {
                data: "DealerName"
            },

            // Customer
            {
                data: null,
                render: function (data, type, row) {
                    return `
                <span>${row.CustomerName || '-'}</span><br>
                <span class="text-muted small">${row.CustomerPhoneNo || '-'}</span>
            `;
                }
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
                                    onclick="viewReplacementOrder(${row.OnlineHSRPReplacementOrderID})">
                                <i class="bx bx-show"></i> View
                            </button>

                            <button class="btn btn-outline-success"
                                    onclick="ApproveReplacementOrder(${row.OnlineHSRPReplacementOrderID})">
                                <i class="bx bx-check"></i> Approve
                            </button>
                        </div>
                    `;
                }

            }
        ]

    });
}

$('#btnFilter').on('click', function () {
    var FilterData = new Object;
    FilterData.sStartDate = $("#txtStartDate").val() || "",
        FilterData.sEndDate = $("#txtEndDate").val() || "",
        FilterData.OEMID = $("#ddlOEMFilter").val() || 0,
        FilterData.DealerID = $("#ddlDealerFilter").val() || 0
    getRecordList(FilterData);
});
$('#btnClearFilter').on('click', function () {
    $("#txtStartDate").val(""),
        $("#txtEndDate").val(""),
        $("#ddlOEMFilter").val(0).change(),
        $("#ddlDealerFilter").val(0).change()
    $("#btnFilter").click();
});
function viewReplacementOrder(replacementOrderId) {

    $.ajax({
        url: ViewOrderUrl,
        type: "GET",
        data: { replacementOrderId },
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
function ApproveReplacementOrder(replacementOrderId) {

    $.ajax({
        url: ViewOrderUrl,
        type: "GET",
        data: { replacementOrderId },
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
                replacementOrderId
            );

            $('#viewOrderModal').modal('show');

            // store order id
            $('#hdnApproveReplacementOrderID').val(replacementOrderId);

            bindOrderData(data);
        }
    });
}
function bindOrderData(data) {

    $("#lblOrderNo").text(data.OrderNo || "-");
    $("#lblOrderDate").text(data.sOrderDate || "-");
    $("#lblOEM").text(data.OEMName || "-");
    $("#lblDealer").text(data.DealerName || "-");
    $("#lblReplacementReason").text(data.ReplacementReasonName || "-");

    $("#lblVehicleNo").text(data.VehicleNo || "-");
    $("#lblVehicleClass").text(data.VehicleClassName || "-");
    $("#lblChasisNo").text(data.ChasisNo || "-");
    $("#lblEngineNo").text(data.EngineNo || "-");

    $("#lblPlateType").text(data.PlateTypeName || "-");
    $("#lblPlateSize").text(data.PlateSizeName || "-");
    $("#lblPlateColor").text(data.PlateColorName || "-");
    $("#lblIsFrontPlate").text(data.IsFrontPlate ? "Yes" : "No");
    $("#lblIsRearPlate").text(data.IsRearPlate ? "Yes" : "No");

    $("#lblCustomerName").text(data.CustomerName || "-");
    $("#lblCustomerPhone").text(data.CustomerPhoneNo || "-");
    $("#lblCustomerEmail").text(data.CustomerEmail || "-");
    $("#lblCustomerAddress").text(data.CustomerAddress || "-");
}

$('#btnApprove').on('click', function () {
    var Approvedata = new Object();

    Approvedata.OnlineHSRPReplacementOrderID = $('#hdnApproveReplacementOrderID').val();
    Approvedata.EmbossingStationID = $('#ddlEmbossingStation').val();
    if (!Approvedata.EmbossingStationID || Approvedata.EmbossingStationID === "0") return markInvalid("#ddlEmbossingStation", " Please Select Embossing Station");

    confirmApproveReplacementOrder(Approvedata);
});
$('#btnReject').on('click', function () {
    var Approvedata = new Object();

    Approvedata.OnlineHSRPReplacementOrderID = $('#hdnApproveReplacementOrderID').val();
    Approvedata.EmbossingStationID = 0;

    RejectReplacementOrder(Approvedata);
});
function confirmApproveReplacementOrder(Approvedata) {
    Swal.fire({
        title: "Confirm Approval",
        text: "Do you want to approve this replacement order?",
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
                        text: response.Message || "Replacement order approved successfully.",
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
function RejectReplacementOrder(Approvedata) {

    Swal.fire({
        title: "Confirm Rejection",
        text: "Do you want to reject this replacement order?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Yes, Reject"
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
                        text: "Replacement order rejected successfully.",
                        icon: "success",
                        confirmButtonColor: "#556ee6"
                    }).then(() => {
                        $('#btnClose').click();
                        $("#btnFilter").click();
                    });
                }
            }
        });
    });
}

