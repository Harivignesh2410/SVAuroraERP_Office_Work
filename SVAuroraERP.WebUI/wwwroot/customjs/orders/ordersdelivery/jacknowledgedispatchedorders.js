let selectedIds = [];
$(function () {
    pLoadingSetup(false);
    getRecordList();
    $("#divOrderList").hide();
    $("#txtDeliveryDate").datetimepicker({
        pickTime: false,
        useCurrent: true,
        format: 'DD/MM/YYYY',
        minDate: moment()
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
function ClearFormFields() {

    $("#txtDeliveryDate").val("");

    $("#selectAllCheckbox").prop("checked", false);

    $(".row-checkbox").prop("checked", false);

    return false;
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
                onclick="OrderDetails(${row.GenerateDeliveryID}, '${row.CompanyName.replace(/'/g, "\\'")}')">
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

let currentGenerateDeliveryID = 0;
function OrderDetails(GenerateDeliveryID, DealerName) {
    ClearFormFields();
    $("#divRecords").hide();
    $("#divOrderList").show();
    $("#txtCompanyName").val(DealerName);

    // Store it in a global variable instead of hidden field
    currentGenerateDeliveryID = GenerateDeliveryID;

    console.log("Stored GenerateDeliveryID:", currentGenerateDeliveryID); // Debug

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
                    <td><input type="checkbox" class="row-checkbox" data-id="${item.OrderID}" /></td>
                    <td>${index + 1}</td>
                    <td>${(item.OrderNo || "-")}</br>${formatDateDMY(item.OrderDate)}</td>
                    <td>${(item.RegNo || "-")}</br> ${(formatDateDMY(item.RegDate))}</td>
                    <td>${(item.FrontLaserSerialNo || "-")}</br>${(item.FrontPlateDimension || "-")}</td>                   
                    <td>${(item.RearLaserSerialNo || "-")} </br> ${(item.RearPlateDimension || "-")}</td>
                     <td>${formatDateDMY(item.GenerateDate) || "-"}</td>
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

    $("#selectAllCheckbox").off("change").on("change", function () {
        $(".row-checkbox").prop("checked", $(this).prop("checked"));
    });

    $(document).off("change", ".row-checkbox").on("change", ".row-checkbox", function () {
        if (!$(this).prop("checked")) {
            $("#selectAllCheckbox").prop("checked", false);
        } else if ($(".row-checkbox:checked").length === $(".row-checkbox").length) {
            $("#selectAllCheckbox").prop("checked", true);
        }
    });
}

$('#btnSave').on('click', function () {

    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    let selectedOrderIds = GetSelectedOrderIDs();

    if (selectedOrderIds.length === 0) {
        return Swal.fire("Warning", "Please select at least one order", "warning");
    }

    let deliveryDate = $('#txtDeliveryDate').val();
    if (!deliveryDate) {
        return Swal.fire("Warning", "Please select a delivery date", "warning");
    }

    let dataObj = {
        sDeliveryDate: deliveryDate,
        GenerateDelieveryDataID: currentGenerateDeliveryID,
        OrderList: selectedOrderIds.join(',')
    };

    console.log(dataObj);

    SaveandUpdate(dataObj);
    return false;
});

function SaveandUpdate(dataObj) {
    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        data: dataObj,
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
                        currentGenerateDeliveryID = 0;
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
            console.error("AJAX Error:", xhr.responseText);
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