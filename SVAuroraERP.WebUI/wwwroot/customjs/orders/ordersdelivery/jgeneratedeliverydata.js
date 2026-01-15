let selectedIds = [];
let _DealerID = 0;

$(function () {
    pLoadingSetup(false);
    getRecordList();
    toggleCourierFields($("#ddlModeofTransport").val());
    GetCourierList("ddlCourier", CourierListUrl, _TOKEN);
    $("#divOrderList").hide();
    $("#txtDispatchDate").datetimepicker({
        pickTime: false,
        useCurrent: true,
        format: 'DD/MM/YYYY',
        minYear: 1951,
        maxDate: moment()
    });
    $(".ClearDate").click(function () { var x = $(this).prev().attr('id'); $("#" + x).val(""); });
    $(".SelectDate").click(function () { var x = $(this).next().attr('id'); $("#" + x).focus(); });

    pLoadingSetup(true);
});

$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divRecords").show();
    $("#divOrderList").hide();

    getRecordList();
    return false;
});
$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divRecords").show();
    $("#divOrderList").hide();

    getRecordList();
    return false;
});
$("#ddlModeofTransport").on("change", function () {
    var mode = $(this).val();
    toggleCourierFields(mode);
});
function toggleCourierFields(modeValue) {
    if (modeValue == 1) {
        $("#divCourierMode").show();
        $("#divDocketNo").show();
    } else {
        $("#divCourierMode").hide();
        $("#divDocketNo").hide();
    }
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
            {
                data: null,
                title: "Dealer Details",
                render: function (data, type, row) {
                    return `
                        <div>
                            <strong>${row.Dealer || '-'}</strong><br>
                            <div class="text-muted">Code: ${row.DealerCode || '-'}</div>
                        </div>
                    `;
                }
            },
            { data: "DealerPONo" },
            { data: "ContactNo" },
            { data: "TotalOrders" },
            {
                data: null,
                render: function (data, type, row) {
                    return ` <button class="btn btn-sm btn-primary btn-details"
                onclick="OrderDetails(${row.DealerID},'${row.Dealer}')">
                Generate
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
/// 2,3,12 ** 4,11
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

function OrderDetails(dealerId, DealerName) {
    $("#divRecords").hide();
    $("#divOrderList").show();
    $("#txtCompanyName").val(DealerName);
    _DealerID = dealerId;
    //$("#txtCompanyName").data("dealerid", dealerId); 

    $.ajax({
        url: ListOrderDataUrl, // your endpoint URL
        type: "POST",
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: "application/json",
        data: JSON.stringify(dealerId),
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
function formatDateDMY(dateStr) {
    if (!dateStr) return "-";

    const date = new Date(dateStr);

    return date.toLocaleDateString("en-GB", {
        day: "2-digit",
        month: "short",
        year: "numeric"
    });
}
function DisplayOrderList(orderArray) {
    let table = $('#tblOrderlist');
    let tbody = table.find('tbody');

    // 🔹 Destroy DataTable if it exists
    if ($.fn.DataTable.isDataTable(table)) {
        table.DataTable().clear().destroy();
    }

    tbody.empty();

    if (!orderArray || orderArray.length === 0) {
        tbody.html(`<tr><td colspan="6" class="text-center text-danger">No records found.</td></tr>`);
        $('#selectedCount').text("Selected 0 out of 0");
        return;
    }

    // 🔹 Populate table
    orderArray.forEach((item, index) => {
        tbody.append(`
            <tr>
                <td><input type="checkbox" class="row-checkbox" data-id="${item.OrderID}" /></td>
                <td>${index + 1}</td>
                <td>${item.OrderNo || "-"}<br/>${formatDateDMY(item.OrderDate) || "-"}</td>
                <td>${item.RegNo || "-"}<br/>${formatDateDMY(item.RegDate) || "-"}</td>
                <td>${item.FrontLaserSerialNo || "-"}<br/>${item.FrontPlateDimension || "-"}</td>
                <td>${item.RearLaserSerialNo || "-"}<br/>${item.RearPlateDimension || "-"}</td>
            </tr>
        `);
    });

    // 🔹 Initialize DataTable
    table.DataTable({
        "bAutoWidth": false,
        "bPaginate": false,
        "bFilter": true,
        "bSort": true,
        "order": []
    });

    // 🔹 Update count function
    function updateSelectedCount() {
        const total = $(".row-checkbox").length;
        const selected = $(".row-checkbox:checked").length;
        $('#selectedCount').text(`Selected ${selected} out of ${total}`);
    }

    // 🔹 Initialize count
    updateSelectedCount();

    // 🔹 Select all checkbox
    $("#selectAllCheckbox").off("change").on("change", function () {
        $(".row-checkbox").prop("checked", $(this).prop("checked"));
        updateSelectedCount();
    });

    // 🔹 Individual checkbox change
    $(document).off("change", ".row-checkbox").on("change", ".row-checkbox", function () {
        if (!$(this).prop("checked")) {
            $("#selectAllCheckbox").prop("checked", false);
        } else if ($(".row-checkbox:checked").length === $(".row-checkbox").length) {
            $("#selectAllCheckbox").prop("checked", true);
        }
        updateSelectedCount();
    });
}
$('#btnSave').on('click', function () {
    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    let selectedOrderIds = GetSelectedOrderIDs();
    if (selectedOrderIds.length === 0) {
        return Swal.fire("Warning", "Please select at least one order", "warning");
    }

    if (_DealerID == 0 || isNaN(_DealerID)) {
        return Swal.fire("Error", "Invalid Dealer ID", "error");
    }

    var dealerId = parseInt($('#txtCompanyName').data('dealerid')) || 0;
    var fileInput = $('#fileUploadImage')[0];

    var formData = new FormData();
    formData.append("FK_DealerID", _DealerID);
    formData.append("FK_ModeOfTransportID", $('#ddlModeofTransport').val() || "");
    formData.append("FK_CourierID", $('#ddlCourier').val() || "");
    formData.append("DocketNo", $('#txtDocketNo').val() || "");
    formData.append("sDispatchDate", $('#txtDispatchDate').val() || "");
    formData.append("ConsignmentDetails", $('#txtConsignmentDetails').val() || "");
    formData.append("CollectingPerson", $('#txtCollectingPerson').val() || "");
    formData.append("OrderList", selectedOrderIds.join(','));


    let modeOfTransport = $("#ddlModeofTransport").val();

    if (!modeOfTransport || modeOfTransport === "0") {
        return markInvalid("#ddlModeofTransport", "Please select Mode of Transport");
    }
    if ($("#txtDispatchDate").val() == "") return markInvalid("#txtDispatchDate", " Please Select the Date");


    if (fileInput && fileInput.files.length > 0) {
        let file = fileInput.files[0];
        formData.append("UploadImage", file); // actual file
        formData.append("UploadImageName", file.name); // just the file name
    } else {
        formData.append("UploadImageName", ""); // optional fallback
    }

    SaveandUpdate(formData);
    return false;
});
function SaveandUpdate(formData) {
    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
            if (response) {
                if (response.success) {
                    Swal.fire({
                        title: "Success!",
                        text: response.message || "Data saved successfully.",
                        icon: "success",
                        confirmButtonColor: "#556ee6"
                    }).then(() => {
                        $('#btnClose').click();

                        ClearFormFields();
                    });
                } else {
                    Swal.fire({
                        title: "Error",
                        text: response.message || "Something went wrong while saving data.",
                        icon: "error",
                        confirmButtonColor: "#556ee6"
                    });
                }
            } else {
                Swal.fire({
                    title: "Error",
                    text: "Invalid server response.",
                    icon: "error",
                    confirmButtonColor: "#556ee6"
                });
            }
        },
        error: function (xhr, status, error) {
            Swal.fire({
                title: "Error",
                text: xhr.responseText || "An unexpected error occurred.",
                icon: "error",
                confirmButtonColor: "#556ee6"
            });
        }
    });
    return false;
}
function ClearFormFields() {
    $("#ddlModeofTransport").val("0").change();
    $("#ddlCourier").val("0").change();
    $("#txtDocketNo").val("");
    $("#txtConsignmentDetails").val("");
    $("#txtCollectingPerson").val("");
    $("#txtDispatchDate").val("");

    $("#selectAllCheckbox").prop("checked", false);
    $(".row-checkbox").prop("checked", false);

    $("#tblOrderlist tbody").empty();
    $("#fileUploadImage").val("");

    $("#imgPreview").attr("src", "").hide();
    $("#lblFileName").text("");

    return false;
}
