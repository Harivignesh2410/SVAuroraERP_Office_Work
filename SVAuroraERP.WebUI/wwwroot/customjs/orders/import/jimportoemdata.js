let USERID = $("#hdnUserID").val();
$(function () {
    pLoadingSetup(false);
    $("#btnSave").show();
    $("#btnUpdate").hide();
    if (!_CMActionAdd) {
        $("#btnAddNew").remove();
        $("#btnSave").remove();
    }
    if (!_CMActionUpdate) $("#btnUpdate").remove();
    $("#divRecords").show();
    $("#divAddEdit").hide();
    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#btnFilter").click();
    GetOEMByEmbossingStationList("ddlOEMListFilter", OEMListByEmbossingStationUrl, _TOKEN, USERID);
    pLoadingSetup(true);
});

$("#btnFilter").on('click', function () {
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    FilterImportOEM();
    return false;
});
$("#btnRefresh").on('click', function () {
    $("#btnFilter").click();
    return false;
});
$("#btnClearFilter").on('click', function () {
    ClearFormFieldsForFilter();
    $("#btnFilter").click();
});
function ClearFormFieldsForFilter() {
    $("#ddlOEMListFilter").val("0").change();
    $("#txtStartDate").val("");
    $("#txtEndDate").val("");
    return false;
}
function FilterImportOEM() {
    var OEMID = parseInt($("#ddlOEMListFilter").val());
    const dateRange = $("#txtFilterDate").val()?.trim();
    let startDate = null;
    let endDate = null;

    if (dateRange) {
        if (dateRange.includes("to")) {
            const parts = dateRange.split("to");
            startDate = parts[0].trim() || null;
            endDate = parts[1].trim() || null;
        } else {
            startDate = dateRange || null;
            endDate = startDate;
        }
    }
    var FilterData = new Object;
    FilterData.sStartDate = $("#txtStartDate").val() || "",
        FilterData.sEndDate = $("#txtEndDate").val() || "",
        FilterData.OEMID = parseInt($("#ddlOEMListFilter").val());

    getRecordList(FilterData);
}
function getRecordList(FilterData) {
    if ($.fn.DataTable.isDataTable('#tblrecordlist')) {
        $('#tblrecordlist').DataTable().clear().destroy();
    }

    $('#tblrecordlist').DataTable({
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
                    OEMID: FilterData.OEMID,
                    sStartDate: FilterData.sStartDate || "",
                    sEndDate: FilterData.sEndDate || "",
                };
            },
            processData: true, // Important for FormData            
            beforeSend: function () {
                // Show loader
                $('body').append(`
                    <div id="dt-loader" class="skote-loader">
                        <div class="spinner-border text-primary" role="status">
                            <span class="sr-only">Loading...</span>
                        </div>
                    </div>
                `);
            },
            complete: function () {
                // Hide loader
                $('#dt-loader').remove();
            }
        },
        language: { oPaginate: { sNext: '<i class="mdi mdi-chevron-right"></i>', sPrevious: '<i class="mdi mdi-chevron-left"></i>' } },
        "columns": [
            {
                data: null, // Serial number (S No.)
                render: function (data, type, row, meta) {
                    return meta.settings._iDisplayStart + meta.row + 1; // Display row number (S. No.)
                },
                "orderable": false,
                "width": "5%",
            },
            { "data": "ImportedDate", "orderable": true, "width": "10%" },
            { "data": "CompanyName", "orderable": true, "width": "10%" },
            { "data": "FileName", "orderable": true, },
            { "data": "DataRowCount", "orderable": true, "width": "10%" },
            { "data": "InsertedCount", "orderable": true, "width": "10%" },
            { "data": "RemovedCount", "orderable": true, "width": "10%" },
            {
                data: null,
                "className": "text-center",
                bSortable: false,
                render: function (data, type, row) {

                    // Create action buttons based on permissions
                    const viewBtn = _CMPermissions.HasView ? `
                    <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                        <a href="javascript:void(0);" onclick="EditData(${row.ImportOEMID})" class="btn btn-sm btn-soft-primary">
                            <i class="mdi mdi-eye-outline"></i>
                        </a>
                    </li>
                ` : '';
                    const deleteBtn = _CMPermissions.HasDelete ? `
                    <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
                        <a href="javascript:void(0);" onclick="DeleteData('${row.ImportOEMID}')" class="btn btn-sm btn-soft-danger">
                            <i class="mdi mdi-delete-outline"></i>
                        </a>
                    </li>
                ` : '';

                    return `
                    <ul class="list-unstyled hstack gap-1 mb-0">
                        ${viewBtn}
                        ${deleteBtn}
                    </ul>`;

                },
                "width": "5%",
                "orderable": false
            }
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}
$("#btnAddNew").on('click', function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddEdit").show();
    $("#divRecords").hide();
    $("#divOEMImportTrans").empty();
    $("#divOEMImportTrans").hide();
    $("#divimportfile").show();
    $("#divListimportdata").hide();
    ClearFormFields();
    $("#divAddEdit .card-body :input").attr("disabled", false);
    $("#divCardTitle").html("<i class='fas fa-plus-square align-middle me-1'></i>Import New OEM File");
    GetOEMByEmbossingStationList("ddlOEMList", OEMListByEmbossingStationUrl, _TOKEN, USERID);
    return false;
});

$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divRecords").show();
    $("#divAddEdit").hide();

    $("#btnFilter").click();
    return false;
});
function ClearFormFields() {
    $("#ddlOEMList").val("0").change();
    $("#ImportExcelFile").val("");
    return false;
}
// Corrected JavaScript for AJAX call:
$("#btnImport").on('click', function () {
    var formData = new FormData();
    var fileInput = $('#ImportExcelFile')[0];

    if (fileInput.files.length === 0) {
        alert("Please select an Excel file.");
        return;
    }

    // Append file and OEMID (matching the C# method parameter)
    formData.append("ImportExcelFile", fileInput.files[0]);
    formData.append("OEMID", $("#ddlOEMList").val());

    $.ajax({
        url: GetOEMImportDataFromExcelUrl,
        type: 'POST',
        headers: {
            "RequestVerificationToken": _TOKEN
        },
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.result) {
                if (response.data.Success) {
                    Swal.fire({
                        title: "Success",
                        text: response.data.Message,
                        icon: "success",
                        confirmButtonColor: "#556ee6"
                    });
                    EditData(response.data.ID);
                } else {
                    Swal.fire({
                        title: "Error",
                        text: response.data.Message,
                        icon: "error",
                        confirmButtonColor: "#556ee6"
                    });
                }
            } else {
                $("#divMaterialData").html("<div class='alert alert-warning'>No data found in Excel.</div>");
            }
        },
        error: function (xhr) {
            Swal.fire({
                title: "Error",
                text: "Failed to process Excel file",
                icon: "error",
                confirmButtonColor: "#556ee6"
            });
        }
    });
});

function EditData(ID) {
    ClearFormFields();
    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            $("#divAddEdit .card-body :input").attr("disabled", true);
            $("#btnCloseWindow,#btnClose").attr("disabled", false);
            $("#divCardTitle").html("<i class='fas fa-eye align-middle me-1'></i>View Imported Data");
            if (response.data.Value != null) {
                DisplayImportedData(response.data.Value)
            }
            else {
                Swal.fire({
                    title: "Error",
                    text: response.data.Message,
                    icon: "warning",
                    confirmButtonColor: "#556ee6"
                });
            }

        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) //console.log(error);

                Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
function DisplayImportedData2(importedData) {
    $("#divAddEdit").show();
    $("#divRecords").hide();
    $("#divOEMImportTrans").empty().show();
    $("#divimportfile").hide();
    $("#divOEMImportdata").empty();
    $("#divListimportdata").show();

    //  Import Summary in Alert Box
    const headerDetails = `
        <div class="alert alert-info border-0 shadow-sm mb-4">
            <div class="fw-bold mb-2">
                Import Summary
            </div>
            <div class="table-responsive">
                <table class="table table-bordered align-middle mb-0">
                    <thead class="table-light">
                        <tr>
                            <th>OEM</th>
                            <th>File Name</th>
                            <th>Imported Date</th>
                            <th>Data Row Count</th>
                            <th>Inserted Count</th>
                            <th>Removed Count</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><i class="bx bx-building me-1 text-primary"></i>${importedData.CompanyName || ''}</td>
                            <td><i class="bx bx-file me-1 text-primary"></i>${importedData.FileName || ''}</td>
                             <td><i class="bx bx-calendar me-1 text-primary"></i>${importedData.ImportedDate || ''}</td>
                            <td><i class="bx bx-list-ol me-1 text-primary"></i>${importedData.DataRowCount || ''}</td>
                            <td><i class="bx bx-check-circle me-1 text-success"></i>${importedData.InsertedCount || ''}</td>
                            <td><i class="bx bx-trash me-1 text-danger"></i>${importedData.RemovedCount || ''}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    `;
    $("#divOEMImportdata").html(headerDetails);

    // 🌟 Imported Order Details Table (below the alert)
    let tableContent = `
        <div class="table-responsive">
            <table id="tblOEMImportTrans" class="table table-hover table-striped align-middle mb-0" style="width:100%;">
                <thead class="table-primary text-center">
                    <tr>
                        <th>Vendor Code</th>
                        <th>Dealer Code</th>
                        <th>Dealer Name</th>
                        <th>Po No</th>
                        <th>So No</th>
                        <th>Vehicle Registration Date</th>
                        <th>Part No</th>
                        <th>Vehicle Registration No</th>
                        <th>Plate Color</th>
                        <th>Order Date</th>
                        <th>Chassis No</th>
                        <th>Engine No</th>
                        <th>Frame No</th>
                        <th>Order No</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
    `;

    if (importedData.VOEMImportTrans && importedData.VOEMImportTrans.length) {
        importedData.VOEMImportTrans.forEach((entry) => {
            tableContent += `
                <tr>
                    <td>${entry.VendorCode || ''}</td>
                    <td>${entry.DealerCode || ''}</td>
                    <td>${entry.DealerName || ''}</td>
                    <td>${entry.PoNo || ''}</td>
                    <td>${entry.SoNo || ''}</td>
                    <td>${entry.VehicleRegistrationDate || ''}</td>
                    <td>${entry.PartNo || ''}</td>
                    <td>${entry.VehicleRegistrationNo || ''}</td>
                    <td>${entry.PlateColor || ''}</td>
                    <td>${entry.OrderDate || ''}</td>
                    <td>${entry.ChassisNo || ''}</td>
                    <td>${entry.EngineNo || ''}</td>
                    <td>${entry.FrameNo || ''}</td>
                    <td>${entry.OrderNo || ''}</td>
                    <td class="text-center">
                        <span class="badge rounded-pill ${entry.ColorCode || 'bg-secondary'} px-3 py-2">
                            ${entry.ImportStatus || ''}
                        </span>
                    </td>
                </tr>`;
        });
    } else {
        tableContent += `
            <tr>
                <td colspan="14" class="text-center text-muted py-4">
                    <i class="bx bx-info-circle me-1"></i> No imported data available
                </td>
            </tr>`;
    }

    tableContent += `
                </tbody>
            </table>
        </div>
    `;

    $("#divOEMImportTrans").html(tableContent);

    // 🌟 Initialize DataTable
    $("#tblOEMImportTrans").DataTable({
        bAutoWidth: false,
        bPaginate: false,
        bFilter: true,
        bSort: false,
        order: [],
        scrollY: "65vh",
        scrollCollapse: true,
        scrollX: true,
        fixedHeader: true,
        language: {
            search: "_INPUT_",
            searchPlaceholder: "Search records...",
            info: "",
            infoEmpty: "",
        },
        dom: '<"top"f>rt<"bottom"lp><"clear">'
    });

    // 🌟 Style search box
    $(".dataTables_filter input")
        .addClass("form-control form-control-sm ms-2")
        .attr("placeholder", "Search...");
}
function DisplayImportedData1(importedData) {
    $("#divAddEdit").show();
    $("#divRecords").hide();
    $("#divOEMImportTrans").empty().show();
    $("#divimportfile").hide();
    $("#divOEMImportdata").empty();
    $("#divListimportdata").show();

    // 🌟 Header Card
    const headerDetails = `
        <div class="card shadow-sm mb-4 border-0">
            <div class="card-header bg-primary bg-gradient text-white fw-bold">
                <i class="bx bx-data me-2"></i> Import Summary
            </div>
            <div class="card-body p-3">
                <div class="table-responsive">
                    <table class="table table-bordered align-middle mb-0">
                        <thead class="table-light">
                            <tr>
                                <th>OEM</th>
                                <th>File Name</th>
                                <th>Data Row Count</th>
                                <th>Inserted Count</th>
                                <th>Removed Count</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <td><i class="bx bx-building me-1 text-primary"></i>${importedData.CompanyName || ''}</td>
                                <td><i class="bx bx-file me-1 text-primary"></i>${importedData.FileName || ''}</td>
                                <td><i class="bx bx-list-ol me-1 text-primary"></i>${importedData.DataRowCount || ''}</td>
                                <td><i class="bx bx-check-circle me-1 text-success"></i>${importedData.InsertedCount || ''}</td>
                                <td><i class="bx bx-trash me-1 text-danger"></i>${importedData.RemovedCount || ''}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    `;
    $("#divOEMImportdata").html(headerDetails);

    // 🌟 Data Table - More elegant design
    let tableContent = `
        <div class="card shadow-sm border-0">
            <div class="card-header bg-light fw-semibold">
                <i class="bx bx-spreadsheet me-2 text-primary"></i> Imported Order Details
            </div>
            <div class="card-body p-0 mt-3">
                <div class="table-responsive">
                    <table id="tblOEMImportTrans" class="table table-hover table-striped align-middle mb-0" style="width:100%;">
                        <thead class="table-primary text-center">
                            <tr>
                                <th>Vendor Code</th>
                                <th>Dealer Code</th>
                                <th>Po No</th>
                                <th>So No</th>
                                <th>Vehicle Registration Date</th>
                                <th>Part No</th>
                                <th>Vehicle Registration No</th>
                                <th>Plate Color</th>
                                <th>Order Date</th>
                                <th>Chassis No</th>
                                <th>Engine No</th>
                                <th>Frame No</th>
                                <th>Order No</th>
                                <th>Status</th>
                            </tr>
                        </thead>
                        <tbody>
    `;

    if (importedData.VOEMImportTrans && importedData.VOEMImportTrans.length) {
        importedData.VOEMImportTrans.forEach((entry, index) => {
            tableContent += `
                <tr>
                    <td>${entry.VendorCode || ''}</td>
                    <td>${entry.DealerCode || ''}</td>
                    <td>${entry.PoNo || ''}</td>
                    <td>${entry.SoNo || ''}</td>
                    <td>${entry.VehicleRegistrationDate || ''}</td>
                    <td>${entry.PartNo || ''}</td>
                    <td>${entry.VehicleRegistrationNo || ''}</td>
                    <td>${entry.PlateColor || ''}</td>
                    <td>${entry.OrderDate || ''}</td>
                    <td>${entry.ChassisNo || ''}</td>
                    <td>${entry.EngineNo || ''}</td>
                    <td>${entry.FrameNo || ''}</td>
                    <td>${entry.OrderNo || ''}</td>
                    <td class="text-center">
                        <span class="badge rounded-pill ${entry.ColorCode || 'bg-secondary'} px-3 py-2">
                            ${entry.ImportStatus || ''}
                        </span>
                    </td>
                </tr>`;
        });
    } else {
        tableContent += `
            <tr>
                <td colspan="15" class="text-center text-muted py-4">
                    <i class="bx bx-info-circle me-1"></i> No imported data available
                </td>
            </tr>`;
    }

    tableContent += `
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
    `;

    $("#divOEMImportTrans").html(tableContent);

    // 🌟 Initialize DataTable with smooth scroll and fixed header
    $("#tblOEMImportTrans").DataTable({
        bAutoWidth: false,
        bPaginate: false,
        bFilter: true,
        bSort: false,
        order: [],
        scrollY: "65vh",
        scrollCollapse: true,
        scrollX: true,
        fixedHeader: true,
        language: {
            search: "_INPUT_",
            searchPlaceholder: "Search records...",
            info: "",
            infoEmpty: "",
        },
        dom: '<"top"f>rt<"bottom"lp><"clear">'
    });

    // Apply nicer styling for search box
    $(".dataTables_filter input")
        .addClass("form-control form-control-sm ms-2")
        .attr("placeholder", "Search...");
}


function DisplayImportedData(importedData) {
    $("#divAddEdit").show();
    $("#divRecords").hide();
    $("#divOEMImportTrans").empty().show();
    $("#divimportfile").hide();
    $("#divOEMImportdata").empty();
    $("#divListimportdata").show();
    $("#divOEMImportdataSummary").show();

    // 🌟 Import Summary in Alert Box
    const headerDetails = `
        <div class="alert alert-info border-0 shadow-sm mb-4">
            <div class="fw-bold mb-2">
                Import Summary
            </div>
            <div class="table-responsive">
                <table class="table table-bordered align-middle mb-0">
                    <thead class="table-light">
                        <tr>
                            <th>OEM</th>
                            <th>File Name</th>
                            <th>Imported Date</th>
                            <th>Data Row Count</th>
                            <th>Inserted Count</th>
                            <th>Removed Count</th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td><i class="bx bx-building me-1 text-primary"></i>${importedData.CompanyName || ''}</td>
                            <td><i class="bx bx-file me-1 text-primary"></i>${importedData.FileName || ''}</td>
                            <td><i class="bx bx-calendar me-1 text-primary"></i>${importedData.ImportedDate || ''}</td>
                            <td><i class="bx bx-list-ol me-1 text-primary"></i>${importedData.DataRowCount || ''}</td>
                            <td><i class="bx bx-check-circle me-1 text-success"></i>${importedData.InsertedCount || ''}</td>
                            <td><i class="bx bx-trash me-1 text-danger"></i>${importedData.RemovedCount || ''}</td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    `;
    $("#divOEMImportdata").html(headerDetails);
    const groupedSummary = importedData.VOEMImportTrans.reduce((acc, row) => {
        const id = row.ImportStatusID;
        if (!acc[id]) {
            acc[id] = {
                ImportStatusID: id,
                ImportStatus: row.ImportStatus, // Get status name from the data itself
                Count: 0
            };
        }
        acc[id].Count++;
        return acc;
    }, {});

    let headerRow = "";
    let countRow = "";

    Object.values(groupedSummary).forEach(item => {
        headerRow += `<th class="text-center small" style="min-width:80px; font-size:0.75rem; padding:0.25rem;">${item.ImportStatus}</th>`;
        countRow += `<td class="text-center fw-bold small" style="font-size:0.875rem; padding:0.25rem;">${item.Count}</td>`;
    });

    const SummaryData = `
    <div class="alert alert-warning border-0 shadow-sm mb-3" style="padding:0.75rem;">
        <div class="fw-bold mb-2" style="font-size:0.875rem;">Import Status Summary</div>
        <div class="table-responsive">
            <table class="table table-bordered table-sm align-middle mb-0" style="font-size:0.8rem;">
                <thead class="table-light">
                    <tr>
                        ${headerRow}
                    </tr>
                </thead>
                <tbody>
                    <tr>
                        ${countRow}
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
`;
    $("#divOEMImportdataSummary").html(SummaryData);
    /*** */

    // 🌟 Updated Imported Order Details Table (merged columns)
    let tableContent = `
        <div class="table-responsive">
            <table id="tblOEMImportTrans" class="table table-hover table-striped align-middle mb-0" style="width:100%;">
                <thead class="table-primary text-center">
                    <tr>
                        <th>Vendor Code</th>
                        <th>Dealer Code / Name</th>
                        <th>Po No / So No</th>
                        <th>Reg No. / Reg Date</th>
                        <th>Part No / Plate Color</th>
                        <th>Chassis / Engine No.</th>
                        <th>Order No / Date</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>
    `;
    if (importedData.VOEMImportTrans && importedData.VOEMImportTrans.length) {
        importedData.VOEMImportTrans.forEach((entry) => {
            tableContent += `
                <tr>
                    <td>${entry.VendorCode || ''}</td>
                    <td>${(entry.DealerCode || '') + (entry.DealerName ? ' / ' + entry.DealerName : '')}</td>
                    <td>${(entry.PoNo || '') + (entry.SoNo ? ' / ' + entry.SoNo : '')}</td>
                    <td>${(entry.VehicleRegistrationNo || '') + (entry.VehicleRegistrationDate ? ' / ' + entry.VehicleRegistrationDate : '')}</td>
                    <td>${(entry.PartNo || '') + (entry.PlateColor ? ' / ' + entry.PlateColor : '')}</td>
                    <td>${(entry.ChassisNo || '') + (entry.EngineNo ? ' / ' + entry.EngineNo : '')}</td>
                    <td>${(entry.OrderNo && entry.OrderNo !== '-' ?
                    (entry.OrderDate ? entry.OrderNo + ' / ' + entry.OrderDate : entry.OrderNo)
                    : '-')
                }</td>  

                    <td class="text-center">
                        <span class="badge rounded-pill ${entry.ColorCode || 'bg-secondary'} px-3 py-2">
                            ${entry.ImportStatus || ''}
                        </span>
                    </td>
                </tr>`;
        });
    } else {
        tableContent += `
            <tr>
                <td colspan="8" class="text-center text-muted py-4">
                    <i class="bx bx-info-circle me-1"></i> No imported data available
                </td>
            </tr>`;
    }

    tableContent += `
                </tbody>
            </table>
        </div>
    `;

    $("#divOEMImportTrans").html(tableContent);

    // 🌟 Initialize DataTable
    $("#tblOEMImportTrans").DataTable({
        bAutoWidth: false,
        bPaginate: false,
        bFilter: true,
        bSort: false,
        order: [],
        scrollY: "65vh",
        scrollCollapse: true,
        scrollX: true,
        fixedHeader: true,
        language: {
            search: "_INPUT_",
            searchPlaceholder: "Search records...",
            info: "",
            infoEmpty: "",
        },
        dom: '<"top"f>rt<"bottom"lp><"clear">'
    });

    // 🌟 Style search box
    $(".dataTables_filter input")
        .addClass("form-control form-control-sm ms-2")
        .attr("placeholder", "Search...");
}

function DeleteData(ID) {
    if (ENABLE_VERBOSE_Logging) //console.log(ID);

        Swal.fire({
            title: "Are you sure?",
            text: "You won't be able to revert this!",
            icon: "question",
            showCancelButton: !0,
            confirmButtonText: "Yes, delete it!",
            cancelButtonText: "No, cancel!",
            customClass: { confirmButton: "btn btn-success mt-2", cancelButton: "btn btn-danger ms-2 mt-2" },
            buttonsStyling: !1,
        }).then(function (t) {
            t.value
                ? ConfirmDelete(ID)
                : t.dismiss === Swal.DismissReason.cancel && Swal.fire({ title: "Cancelled", text: "Your data is safe :)", icon: "error" });
        });

    return false;
}
function ConfirmDelete(ID) {
    $.ajax({
        url: DeleteDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(ID),
        success: function (response) {
            if (response && response.result) {
                var result = response.result;
                if (!result.Error && result.Success && result.ID > 0) {
                    Swal.fire({
                        title: "Deleted!",
                        text: result.Message,
                        icon: "success",
                        confirmButtonColor: "#556ee6"
                    });
                    $("#btnFilter").click();
                } else if (result.Error && !result.Success && result.ID > 0) {
                    Swal.fire({
                        title: "Warning",
                        text: result.Message,
                        icon: "warning",
                        confirmButtonColor: "#556ee6"
                    });
                }
            } else {
                Swal.fire({
                    title: "Error",
                    text: errorMsg,
                    icon: "warning",
                    confirmButtonColor: "#556ee6"
                });
            }
        }, error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
    return false;
}
