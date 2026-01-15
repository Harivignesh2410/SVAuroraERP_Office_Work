var PageTitle = "Manage Job Card";
$(function () {
    pLoadingSetup(false);
    GetEmbossingStationList("ddlEmbossingStationFilter", EmbossingStationListUrl, _TOKEN);
    GetDealerList("ddlDealerFilter", DealerListUrl, _TOKEN);
    $("#divAdvancedFilter").show();
    $("#divAddEdit").hide();

    $("#btnFilter").click();
    pLoadingSetup(true);
});

$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divRecords").show();
    $("#divAddEdit").hide();
    $("#btnFilter").click();
    $("#divAdvancedFilter").show();
    return false;
});

$('#btnFilter').on('click', function () {
    var FilterData = new Object;
    FilterData.sStartDate = $("#txtStartDate").val() || "",
        FilterData.sEndDate = $("#txtEndDate").val() || "",
        FilterData.EmbossingStationID = $("#ddlEmbossingStationFilter").val() || 0,
       FilterData.DealerID = $("#ddlDealerFilter").val() || 0,
        FilterData.SearchText = $("#txtSearchbox").val() || ""
    getRecordList(FilterData);
});
$('#btnClearFilter').on('click', function () {
    $("#txtStartDate").val(""),
        $("#txtEndDate").val(""),
        $("#ddlEmbossingStationFilter").val(0).change(),
        $("#ddlDealerFilter").val(0).change(),
        $("#txtSearchbox").val("")
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
        "columns": [
            {
                data: null, // Serial number (S No.)
                render: function (data, type, row, meta) {
                    return meta.settings._iDisplayStart + meta.row + 1; // Display row number (S. No.)                    
                },
                orderable: false, // Disable sorting for this column},
                "width": "2%",
                "orderable": false
            },
            { "data": "JobCardNo", "orderable": true, "width": "5%" },
            { "data": "sJobCardDate", "orderable": true, "width": "5%" },
    
            { "data": "EmbossingName", "orderable": true, "width": "10%" },
            { "data": "EmbossingCity", "orderable": true, "width": "10%" },
            { "data": "JobCardTransCount", "orderable": true, "width": "10%" },

            {
                data: null,
                bSortable: false,
                "className": "text-center",
                render: function (data, type, row) {
                    return SetActionViewAndExportOnly(row.HSRPJobCardID);
                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
    return false;
}

function EditData(ID, ViewFlag) {
    if (ENABLE_VERBOSE_Logging)
        console.log("Editing JobCard ID:", ID);

    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            var JobCard = response.result.Value;
            if (!JobCard) {
                Swal.fire({ title: "Error", text: "Job Card not found!", icon: "error" });
                return;
            }
            $("#divAddEdit .card-title").html("<i class='mdi mdi-eye'></i>&nbsp;&nbsp;View " + PageTitle);

            // Show the card
            $("#divJobCardDetails").removeClass("d-none");
            $("#spnJobCardNo").text(JobCard.JobCardNo || "-");
            $("#spnJobCardDate").text(JobCard.sJobCardDate || "-");
            $("#spnEmbossingStation").text(JobCard.EmbossingName || "-");
          

            $("#divAdvancedFilter").hide();
            $("#divAddEdit").show();
            $("#divRecords").hide();

            JobCardArray = [];

            if (JobCard.VHSRPJobCardTrans && JobCard.VHSRPJobCardTrans.length > 0) {
                JobCard.VHSRPJobCardTrans.forEach((trans, index) => {
                    JobCardArray.push({
                        SNo: index + 1,
                        HSRPJobCardTransID: trans.HSRPJobCardTransID,
                        OrderID: trans.OrderID,
                        OrderNo: trans.OrderNo,
                        sOrderDate: trans.sOrderDate,
                        RegNo: trans.RegNo,
                        EngineNo: trans.EngineNo,
                        ChasisNo: trans.ChasisNo,
                        Dealer: trans.Dealer,
                        Description: trans.Description,
                        FrontLaserSerialNo: trans.FrontLaserSerialNo,
                        RearLaserSerialNo: trans.RearLaserSerialNo,
                        FrontPlateSize: trans.FrontPlateSize,
                        RearPlateSize: trans.RearPlateSize,
                        ColorCode: trans.ColorCode,
                        StatusFlag: ""
                    });
                });
            }

            DisplayDataTable(JobCardArray);

        },
        error: function (xhr) {
            Swal.fire({
                title: "Error",
                text: xhr.responseText || "Failed to load data.",
                icon: "error",
                confirmButtonColor: "#556ee6"
            });
        }
    });
}


function DisplayDataTable(JobCardArray) {
    let container = $("#divTableData");
    container.empty();

    let tableContent = `
        <table id="tblJobCardTrans" class="table table-bordered table-hover">
            <thead class="table-light">
                <tr>
                    <th>S.No</th>
                    <th>Order No</th>
                    <th>Order Date</th>
                    <th>Reg No</th>
                    <th>Engine No</th>
                    <th>Chasis No</th>
                    <th>Front Laser No / Size</th>
                    <th>Rear Laser No/ Size</th>
                    <th>Status</th>
                </tr>
            </thead>
            <tbody>
    `;

    JobCardArray.forEach((item, index) => {
        tableContent += `
            <tr data-sno="${item.SNo}">
                <td>${item.SNo}</td>
                <td>${item.OrderNo || "-"}</td>
                <td>${item.sOrderDate || "-"}</td>
                <td>${item.RegNo || "-"}</td>
                <td>${item.EngineNo || "-"}</td>
                <td>${item.ChasisNo || "-"}</td>
                <td>${item.FrontLaserSerialNo || "-"}<br> ${item.FrontPlateSize || "-"}</td>
                <td>${item.RearLaserSerialNo || "-"}<br> ${item.RearPlateSize || "-"}</td>
                  <td><span class="${item.ColorCode}">${item.Description}</span></td>
            </tr>
        `;
    });
    container.html(tableContent);
}
function PrintReport(ID) {
    PrintReportByID(JobCardPDFExportUrl, "OrderID", ID);
    return false;
}
