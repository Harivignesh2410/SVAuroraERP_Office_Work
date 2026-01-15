_PageTitle = "Fitted Orders";
let flatData = [];
let selectedIds = [];
var HSRPOrderArray = [];
let USERID = $("#hdnUserID").val();
$(function () {
    pLoadingSetup(false);
    GetOEMList("ddlOEMFilter", OEMListUrl, _TOKEN);
    GetEmbossingStationByUser("ddlEmbossingStationFilter", EmbossingStationListUrl, _TOKEN, USERID);
    //  GetDealerList("ddlDealerFilter", DealerListUrl, _TOKEN);
    GetOrderTypeList("ddlOrderType", OrderTypeListUrl, _TOKEN)
    $("#btnFilter").click();
    getSummaryList();
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
                render: function (data, type, row, meta) {
                    return meta.settings._iDisplayStart + meta.row + 1;
                },
                orderable: false,
                width: "5%"
            },
            {
                data: null,
                render: function (data, type, row) {
                    return `
            <div class="btn-group-vertical">
                <div class="btn-group">
                    <button class="btn btn-sm btn-outline-pink dropdown-toggle"
                        data-bs-toggle="dropdown">
                        <i class="bx bx-list-check me-1"></i> Actions
                    </button>

                    <ul class="dropdown-menu shadow-sm">
                        <li>
                            <a class="dropdown-item d-flex align-items-center btn-vahan-api"
                               href="javascript:;" 
                               data-id="${row.HSRPOrderID}">
                                <i class="bx bx-cloud-upload text-primary me-2"></i> Vahaan Submission
                            </a>
                        </li>

                    </ul>
                </div>
            </div>
        `;
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
                data: "FrontLaserNoURL",
                orderable: false,
                width: "10%",
                render: function (url) {
                    if (!url) return "-";

                    return `
                        <img src="${url}"
                             class="img-thumbnail front-thumb"
                             data-src="${url}"
                             style="max-height:60px; cursor: zoom-in;" />`;

               }
            },
            {
                data: "RearLaserNoURL",
                orderable: false,
                width: "10%",
                render: function (url) {
                    if (!url) return "-";
                    return `
                    <img src="${url}"
                         class="img-thumbnail rear-thumb"
                         data-src="${url}"
                         style="max-height:60px; cursor: zoom-in;" />`;

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
                <span >${row.RegNo}</span><br>
                <span >${ISTtoLocal(row.RegDate)}</span>
            `;
                }
            },
            {
                data: null,
                width: "10%",
                render: function (data, type, row) {
                    return `
            <span >${row.FrontLaserSerialNo || '-'}</span><br>
            <span >${row.FrontPlateDimension || '-'}</span> 
        `;
                }
            },
            {
                data: null,
                width: "10%",
                render: function (data, type, row) {
                    return `
            <span >${row.RearLaserSerialNo || '-'}</span><br>
            <span >${row.RearPlateDimension || ''}</span> 
        `;
                }
            },


            {
                data: null,
                width: "15%",
                render: function (data, type, row) {
                    return `
                <span>${row.DealerCode} - ${row.Dealer}</span><br>
                <span>${row.OEM}</span>
            `;
                }
            },
            {
                data: null,
                width: "10%",
                render: function (data, type, row) {
                    return `
                <span>${row.EngineNo || '-'}</span><br>
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
function GetHSRPDataByID(HSRPOrderID) {

    HSRPOrderArray = null;

    $.ajax({
        url: GetHSRPDataByIDUrl,
        type: 'GET',
        data: { HSRPOrderID: HSRPOrderID },

        success: function (response) {

            if (!response) return;

            $("#divQualityProcessModal .modal-body :input").prop("disabled", false);
            $("#divQualityProcessModal .modal-title")
                .html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Quality Process");

            $("#btnQualityCheck").show();
            $('#divQualityProcessModal').modal('show');

            checkFrontInputs();
            checkRearInputs();

            HSRPOrderArray = response;
            let FtableContent = `
                <div class="table-responsive">
                    <table class="table table-striped align-middle">
                        <thead>
                            <tr class="table-light">
                                <th>Vehicle No</th>
                                <th>Laser No</th>
                                <th>Color / Size</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>${response.RegNo || '-'}</td>
                                <td>${response.FrontLaserSerialNo || '-'}</td>
                                <td>${response.FrontPlateDimension || '-'}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>`;

            $("#divFrontdata").html(FtableContent);

            // ================= REAR TABLE =================

            let RtableContent = `
                <div class="table-responsive">
                    <table class="table table-striped align-middle">
                        <thead>
                            <tr class="table-light">
                                <th>Vehicle No</th>
                                <th>Laser No</th>
                                <th>Color / Size</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td>${response.RegNo || '-'}</td>
                                <td>${response.RearLaserSerialNo || '-'}</td>
                                <td>${response.RearPlateDimension || '-'}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>`;

            $("#divReardata").html(RtableContent);
        },

        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({
                title: "Error",
                text: xhr.responseText || error,
                icon: "error",
                confirmButtonColor: "#556ee6"
            });
        }
    });
}

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

$('#btnRefresh').on('click', function () {
    $("#btnFilter").click();
});

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
                data: "Dealer",
                title: "Dealer",
                render: data => `<span>${data || "-"}</span>`
            },
            {
                data: "DealerCode",
                title: "Code",
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

$(document).on("click", ".btn-vahan-api", function () {
    let id = $(this).data("id");

    $("#btnVahanSubmit").data("id", id);
    $("#btnVahanReUpload").data("id", id);

    LoadVehicleImageData(id);
});

function LoadVehicleImageData(ID) {
    $.ajax({
        url: GetVehicleImageDataByIDUrl,
        type: "GET",
        data: { ID: ID },

        success: function (response) {
            if (response && response.Value) {
                let FittedOrderData = response.Value;
                if (FittedOrderData) {
                    HSRPOrderArray = FittedOrderData;
                    // ===== POPULATE TEXT FIELDS =====

                    $("#modalheading").text("Vahan API Submission - " + FittedOrderData.RegNo);

                    $("#txtPlateDimension").text(
                        (FittedOrderData.FrontPlateDimension || FittedOrderData.FrontPlateSize || '-') +
                        " / " +
                        (FittedOrderData.RearPlateDimension || FittedOrderData.RearPlateSize || '-')
                    );

                    $("#txtLaserNo").text(
                        (FittedOrderData.FrontLaserSerialNo || '-') +
                        " / " +
                        (FittedOrderData.RearLaserSerialNo || '-')
                    );

                    $("#txtColor").text(FittedOrderData.PlateColor || '-');

                    $("#divOrderdata").empty();
                    let OrderdataContent = `<div class="table-responsive">
                    <table class="table table-striped align-middle">
                        <thead>
                            <tr class="table-light">
                                <th>Dealer</th>
                                <th>OEM</th>
                                <th>Embossing Station</th>
                                <th>Order No</th>
                                 <th>Reg No</th>
                                <th>Order Date & Reg Date</th>
                            </tr>
                        </thead>
                        <tbody>`;

                    OrderdataContent += `
                            <tr>
                                <td>${FittedOrderData.Dealer},<br>${FittedOrderData.DealerCode},<br>${FittedOrderData.DealerCity}</td>
                               <td>${FittedOrderData.OEM},<br>${FittedOrderData.OEMCode},<br>${FittedOrderData.OEMCity}</td>
                                 <td>${FittedOrderData.EmbossingStation},<br>${FittedOrderData.EmbossingStationCode},<br>${FittedOrderData.EmbossingStationCity}</td>
                                <td>${FittedOrderData.OrderNo}</td>
                                <td>${FittedOrderData.RegNo}</td>
                                <td>${FittedOrderData.sOrderDate}<br>${FittedOrderData.sRegDate}</td>                          
                            </tr>`;

                    OrderdataContent += `</tbody></table></div>`;
                    $("#divOrderdata").html(OrderdataContent);
                    /****/
                    $("#divFrontdata").empty();
                    let FtableContent = `<div class="table-responsive">
                    <table class="table table-striped align-middle">
                        <thead>
                            <tr class="table-light">
                                <th>Vehicle No</th>
                                <th>Laser No</th>
                                <th>Color / Size</th>
                            </tr>
                        </thead>
                        <tbody>`;

                    FtableContent += `
                            <tr>
                                <td>${FittedOrderData.RegNo}</td>
                                <td>${FittedOrderData.FrontLaserSerialNo}</td>
                                <td>${FittedOrderData.FrontPlateDimension}</td>
                            </tr>`;

                    FtableContent += `</tbody></table></div>`;
                    $("#divFrontdata").html(FtableContent);

                    $("#divReardata").empty();
                    let RtableContent = `<div class="table-responsive">
                    <table class="table table-striped align-middle">
                        <thead>
                            <tr class="table-light">
                                <th>Vehicle No</th>
                                <th>Laser No</th>
                                <th>Color / Size</th>
                            </tr>
                        </thead>
                        <tbody>`;

                    RtableContent += `
                            <tr>
                                <td>${FittedOrderData.RegNo}</td>
                                <td>${FittedOrderData.RearLaserSerialNo}</td>
                                <td>${FittedOrderData.RearPlateDimension}</td>
                            </tr>`;

                    RtableContent += `</tbody></table></div>`;
                    $("#divReardata").html(RtableContent);
                    // FRONT IMAGE

                    // FRONT IMAGE
                    if (FittedOrderData.FrontLaserNoURL) {
                        $("#frontImagePreview")
                            .attr("src", FittedOrderData.FrontLaserNoURL)
                            .css("cursor", "zoom-in")
                            .off("click")
                            .on("click", function () {
                                openImageZoom(
                                    FittedOrderData.FrontLaserNoURL,
                                    "Front Plate Image"
                                );
                            })
                            .show();
                    } else {
                        $("#frontImagePreview").hide();
                    }

                    // REAR IMAGE
                    if (FittedOrderData.RearLaserNoURL) {
                        $("#rearImagePreview")
                            .attr("src", FittedOrderData.RearLaserNoURL)
                            .css("cursor", "zoom-in")
                            .off("click")
                            .on("click", function () {
                                openImageZoom(
                                    FittedOrderData.RearLaserNoURL,
                                    "Rear Plate Image"
                                );
                            })
                            .show();
                    } else {
                        $("#rearImagePreview").hide();
                    }
                    $("#txtRegNo").val("");
                    $('input[name="vehicleCategory"]').prop('checked', false);


                    $("#modalVahanAPI").modal("show");
                }

            }
        },

        error: function (xhr) {
            Swal.fire("Error", xhr.responseText, "error");
        }
    });
}

$(document).on("click", "#btnVahanSubmit", function () {

    let id = $(this).data("id");

    let RegisterNo = $("#txtRegNo").val().trim(); 
    let originalNo = HSRPOrderArray.RegNo;

    if (RegisterNo === "") {
        $.jGrowl("Please enter Register No!", { theme: 'danger' });
        return;
    }
    if (RegisterNo != originalNo) {
        $.jGrowl("Kindly Enter the Valid Registration No", { theme: 'warning' });
        return;
    }

    if (!$("input[name='vehicleCategory']:checked").length) {
        $.jGrowl("Please Select Vehicle Category", { theme: 'warning' });
        return false;
    }


  
    Swal.fire({
        title: "Confirm Submit",
        text: "Are you sure you want to submit this Vahan API?",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Yes, Submit",
        cancelButtonText: "No",
        confirmButtonColor: "#556ee6",
        cancelButtonColor: "#d33"
    }).then((result) => {

        if (result.isConfirmed) {
            VahanSubmitAction({
                HSRPOrderID: id,
                IsSubmit: true
            });
        }
    });
});

$(document).on("click", "#btnVahanReUpload", function () {

    let id = $(this).data("id");

    Swal.fire({
        title: "Confirm Re-Upload",
        text: "Are you sure you want to re-upload this Vahan API?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Yes, Re-Upload",
        cancelButtonText: "No",
        confirmButtonColor: "#556ee6",
        cancelButtonColor: "#d33"
    }).then((result) => {

        if (result.isConfirmed) {
            VahanSubmitAction({
                HSRPOrderID: id,
                IsSubmit: false
            });
        }
    });
});


function VahanSubmitAction(Data) {
    $.ajax({
        url: SaveFittedOrdersUrl,
        type: "POST",
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(Data),

        success: function (response) {
            $("#modalVahanAPI").modal("hide");
            $("#btnRefresh").click();
            Swal.fire({
                title: "Success",
                text: response.message || (Data.IsSubmit ? "Submitted successfully!" : " Reupload submit!"),
                icon: "success",
                confirmButtonColor: "#556ee6"
            });
        },

        error: function (xhr) {
            Swal.fire({
                title: "Error",
                text: xhr.responseText || "Something went wrong!",
                icon: "error",
                confirmButtonColor: "#556ee6"
            });
        }
    });
}

function openImageZoom(src, title) {
    if (!src) return;

    $("#imgZoomTarget")
        .attr("src", src)
        .removeClass("d-none");

    $("#imgZoomModal .modal-title").text(title || "Plate Image");

    const modalEl = document.getElementById("imgZoomModal");

    let modal = bootstrap.Modal.getInstance(modalEl);
    if (!modal) {
        modal = new bootstrap.Modal(modalEl, {
            backdrop: true,
            keyboard: true
        });
    }

    modal.show();
}

$(document).on("click", ".front-thumb, .rear-thumb", function () {
    const src = $(this).data("src") || $(this).attr("src");
    const title = $(this).hasClass("front-thumb")
        ? "Front Plate Image"
        : "Rear Plate Image";

    openImageZoom(src, title);
});




















