var NumberPlateDispatchID = 0;
$(function () {
    pLoadingSetup(false);
    getRecordList();
    $("#divAddEdit").hide();
    $("#divRecords").show();
    pLoadingSetup(true);
});
$('#btnRefresh').on('click', function () {
    getRecordList();
    return false;
});
$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divRecords").show();
    $("#divAddEdit").hide();
    $('#btnRefresh').click();
    return false;
});
function getRecordList() {
    // Check if DataTable has already been initialized
    if ($.fn.DataTable.isDataTable('#tblrecordlist')) {
        $('#tblrecordlist').DataTable().clear().destroy();  // Destroy previous instance
    }

    $('#tblrecordlist').DataTable({
        "processing": true,
        "serverSide": true,
        "ordering": true,  // Enable sorting on columns
        "ajax": {
            url: ListDataUrl,
            "type": "GET",
            "data": function (d) {
                // d.search.value = $('#tblrecordlist_filter input').val();  // Make sure the search value is passed
                // Pass additional parameters if needed
                return $.extend({}, d, {
                    // Custom parameters here (if any)
                });
            }
        },
        language: { oPaginate: { sNext: '<i class="mdi mdi-chevron-right"></i>', sPrevious: '<i class="mdi mdi-chevron-left"></i>' } },
        "columns": [
            {
                data: null, // Serial number (S No.)
                render: function (data, type, row, meta) {
                    return meta.settings._iDisplayStart + meta.row + 1; // Display row number (S. No.)
                },
                bSortable: false, // Disable sorting for this column},
                "width": "5%",
                "orderable": false
            },
            { "data": "DispatchNo", "orderable": true, "width": "10%" },
            { "data": "DispatchDate", "orderable": true },
            { "data": "ModeofTransportName", "orderable": true },
            { "data": "TransportDetails", "orderable": true },
            { "data": "DocketNo", "orderable": true },
            { "data": "DocketBookingDate", "orderable": true },
            { "data": "EmbossingStationName", "orderable": true },
            {
                "data": "StatusID",
                "render": function (data, type, row) {
                    return `<span class="${row.ColorCode}">${row.StatusName}</span>`;
                },
                "width": "5%",
                "className": "text-center",
                "orderable": false
            },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    actionButtons = `
                                     <button type="button" onclick="GetPackingListByID(${row.NumberPlateDispatchID})" class="btn btn-sm btn-info waves-effect waves-light" title="Click here to Add New">
						                <i class="bx bx-paper-plane me-2"></i>Acknowledgement 
					                   </button> ` ;
                    return actionButtons;
                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");

    return false;
}
function GetPackingListByID1(ID) {
    $.ajax({
        url: GetPackingListByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            console.log(response);
            if (response != null) {
                $("#divCardTitle").html("<i class='bx bx-log-in-circle font-size-20 align-middle me-1'></i>&nbsp;Acknowledgement for Dispatch");
                $("#divAddEdit").show();
                $("#divRecords").hide();
                //gMaxCapacity = response.MaxCapacity;
                $("#divPackingList").empty();
                //let tableContent = '<div class="alert alert-w mt-2" role="alert">Packing</div>';
                let colorCode = "bg-secondary bg-gradient text-white";
                let tableContent = '<div class="table-responsive">';

                tableContent += `
                        <table class="table  align-middle" id="tblSearchResult">
                            <thead>
                                <tr class="table-light">
                                    <th class="${colorCode}">Packing No</th>
                                    <th class="${colorCode}">Packing Date</th>
                                    <th class="${colorCode}">Box</th>
                                    <th class="${colorCode}">Size</th>
                                    <th class="${colorCode}">Color</th>
                                    <th class="${colorCode}">No of InnerBoxCount</th>   
                                    <th class="${colorCode}">Total No.of.Plates</th> 
                                    <th class="${colorCode}">PcsPerBox</th> 
                                    <th class="${colorCode}">Status</th> 
                                    <th class="${colorCode}">Action</th> 
                                </tr>
                            </thead>
                           <tbody>`;
                if (response.data.length != 0) {
                    response.data.forEach((packingdata) => {
                        tableContent += `
                                <tr>
                                    <td>${packingdata.PackingNo}</td>
                                    <td>${packingdata.PackingDate}</td>
                                    <td>${packingdata.BoxName}</td>
                                    <td>${packingdata.SizeName}</td>
                                    <td>${packingdata.ColorName}</td>
                                    <td>${packingdata.BoxCount}</td>
                                    <td>${packingdata.TotalQuantity}</td>
                                    <td><span class="${packingdata.ColorCode}">${packingdata.StatusName}</span></td>
                                    <td>${packingdata.PcsPerBox}</td>
                                    <td>
                                        <ul class="list-unstyled hstack gap-1 mb-0">
                                            <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                                               <a href="javascript:void(0);" onclick="GetPackingByID(${packingdata.PackingID}, true)" class="btn btn-sm btn-soft-primary" data-bs-toggle="collapse" data-bs-target="#collapseWidthExample" aria-expanded="true" aria-controls="collapseWidthExample">
                                               <i class="bx bx-down-arrow-alt me-2"></i>View
                                               </a>
                                            </li>
                                            <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Edit">
                                               <a href="javascript:void(0);" onclick="AcknowledgePacking(${packingdata.PackingID}, false)" class="btn btn-sm btn-success">
                                               <i class="bx bx-check-shield me-2"></i>Approve
                                               </a>
                                           </li>
                                        </ul>
                                    </td>
                                </tr>`;
                    });
                } else {
                    tableContent += '<tr><td colspan="10" class=text-center>No Packing Data to Display</td></tr>';
                }

                tableContent += `
                            </tbody>
                        </table>
                    </div>`;

                $("#divPackingList").html(tableContent);
            }


        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonCategory: "#556ee6" });
        }
    });
    return false;
}
function GetPackingListByID(ID) {
    $.ajax({
        url: GetPackingListByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            if (response != null) {
                NumberPlateDispatchID = ID;
                $("#divCardTitle").html("<i class='bx bx-log-in-circle font-size-20 align-middle me-1'></i>&nbsp;Acknowledgement for Dispatch");
                $("#divAddEdit").show();
                $("#divRecords").hide();
                $("#divPackingList").empty();

                let colorCode = "bg-secondary bg-gradient text-white";
                let tableContent = '<div class="table-responsive">';

                tableContent += `
                    <table class="table align-middle" id="tblSearchResult">
                        <thead>
                            <tr class="table-light">
                                <th class="${colorCode}">Packing No</th>
                                <th class="${colorCode}">Packing Date</th>
                                <th class="${colorCode}">Box</th>
                                <th class="${colorCode}">Size</th>
                                <th class="${colorCode}">Color</th>
                                <th class="${colorCode}">No of InnerBoxCount</th>   
                                <th class="${colorCode}">Total No.of.Plates</th> 
                                <th class="${colorCode}">PcsPerBox</th> 
                                <th class="${colorCode}">Status</th> 
                                <th class="${colorCode}">Action</th> 
                            </tr>
                        </thead>
                        <tbody>`;

                if (response.data.length != 0) {
                    response.data.forEach((packingdata) => {
                        
                        let approveBtn = "";
                        if (packingdata.StatusID == 2) {
                            approveBtn = `
                                <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Approve">
                                    <a href="javascript:void(0);" onclick="AcknowledgePacking(${packingdata.PackingID}, false)" 
                                       class="btn btn-sm btn-success">
                                       <i class="bx bx-check-shield me-2"></i>Approve
                                    </a>
                                </li>`;
                        }

                        tableContent += `
                            <tr>
                                <td>${packingdata.PackingNo}</td>
                                <td>${packingdata.PackingDate}</td>
                                <td>${packingdata.BoxName}</td>
                                <td>${packingdata.SizeName}</td>
                                <td>${packingdata.ColorName}</td>
                                <td>${packingdata.BoxCount}</td>
                                <td>${packingdata.TotalQuantity}</td>
                                <td>${packingdata.PcsPerBox}</td>
                                <td><span class="${packingdata.ColorCode}">${packingdata.StatusName}</span></td>
                                <td>
                                    <ul class="list-unstyled hstack gap-1 mb-0">
                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                                            <a href="javascript:void(0);" 
                                               onclick="GetPackingByID(${packingdata.PackingID}, true)" 
                                               class="btn btn-sm btn-soft-primary" 
                                               data-bs-toggle="collapse" 
                                               data-bs-target="#collapseWidthExample" 
                                               aria-expanded="true" 
                                               aria-controls="collapseWidthExample">
                                               <i class="bx bx-down-arrow-alt me-2"></i>View
                                            </a>
                                        </li>
                                        ${approveBtn}
                                    </ul>
                                </td>
                            </tr>`;
                    });
                } else {
                    tableContent += '<tr><td colspan="10" class="text-center">No Packing Data to Display</td></tr>';
                }

                tableContent += `
                        </tbody>
                    </table>
                </div>`;

                $("#divPackingList").html(tableContent);
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonCategory: "#556ee6" });
        }
    });
    return false;
}

function GetPackingByID(packingID, isView) {
    // Find the master row
    let $targetRow = $(`#tblSearchResult tr`).filter(function () {
        return $(this).find("a").attr("onclick")?.includes(`GetPackingByID(${packingID}`);
    });

    // Check if the accordion row already exists
    let $existingAccordion = $targetRow.next('.accordion-container');
    if ($existingAccordion.length > 0) {
        // Toggle existing collapse
        const collapseEl = $existingAccordion.find('.collapse')[0];
        const bsCollapse = bootstrap.Collapse.getInstance(collapseEl) || new bootstrap.Collapse(collapseEl, { toggle: false });
        bsCollapse.toggle(); // This will open if closed, close if open
        return; // Exit function, no need to re-fetch data
    }

    // If not exists, fetch data
    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: packingID },
        success: function (response) {
            if (!response || !response.data) return;

            var stockdata = response.data;

            // Build the horizontal accordion
            let innerBoxContent = `
                <div class="expanded-row collapse horizontal-collapse" id="collapse-${packingID}">
                    <div class="card card-body p-2 border-0">
                        <div class="table-responsive">
                            <table class="table table-bordered align-middle mb-0">
                                <thead class="table-light">
                                    <tr>
                                        <th class="text-center">S.No</th>
                                        <th class="text-center">Starting Laser No</th>
                                        <th class="text-center">Ending Laser No</th>
                                        <th class="text-center">No.of Plate</th>
                                        <th class="text-center">Inner Box No</th>
                                        <th class="text-center">Color</th>
                                        <th class="text-center">Size</th>
                                    </tr>
                                </thead>
                                <tbody>
            `;

            if (stockdata.PackingTrans && stockdata.PackingTrans.length > 0) {
                stockdata.PackingTrans.forEach((entry, index) => {
                    const startLaser = (entry.LaserNoPrefix || 'CD') + entry.StartingLaserNo.toString().padStart(8, '0');
                    const endLaser = (entry.LaserNoPrefix || 'CD') + entry.EndingLaserNo.toString().padStart(8, '0');

                    innerBoxContent += `
                        <tr>
                            <td class="text-center">${index + 1}</td>
                            <td class="text-center">${startLaser}</td>
                            <td class="text-center">${endLaser}</td>
                            <td class="text-center">${entry.Quantity}</td>
                            <td class="text-center">${entry.InnerBoxNo || "-"}</td>
                            <td class="text-center">${entry.ColorName}</td>
                            <td class="text-center">${entry.SizeName}</td>
                        </tr>
                    `;
                });
            } else {
                innerBoxContent += `
                    <tr>
                        <td colspan="7" class="text-center text-muted">No Inner Box data available</td>
                    </tr>
                `;
            }

            innerBoxContent += `
                                </tbody>
                            </table>
                        </div>
                    </div>
                </div>
            `;

            $targetRow.after(`<tr class="accordion-container"><td colspan="9">${innerBoxContent}</td></tr>`);

            const collapseEl = document.getElementById(`collapse-${packingID}`);
            const bsCollapse = new bootstrap.Collapse(collapseEl, { toggle: true });

            collapseEl.addEventListener('shown.bs.collapse', function () {
                $targetRow.find('a[data-bs-toggle="collapse"] i').removeClass('bx bx-down-arrow-alt').addClass('bx bx-up-arrow-alt');
            });
            collapseEl.addEventListener('hidden.bs.collapse', function () {
                $targetRow.find('a[data-bs-toggle="collapse"] i').removeClass('bx bx-up-arrow-alt').addClass('bx bx-down-arrow-alt');
            });

            $('html, body').animate({
                scrollTop: $targetRow.offset().top - 100
            }, 300);

        },
        error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
function AcknowledgePacking(packingID) {
    if (!packingID) return;

    Swal.fire({
        title: "Confirm Acknowledge",
        text: `Are you sure you want to acknowledge this packing?`,
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Yes, Acknowledge",
        cancelButtonText: "Cancel",
        confirmButtonColor: "#28a745"
    }).then((result) => {
        if (!result.isConfirmed) return;

        $.ajax({
            url: AcknowledgeInnerBoxUrl,
            type: 'GET',
            contentType: 'application/json',
            data: { packingID: packingID },
            success: function (response) {
              
                Swal.fire({
                    title: "Success",
                    text: "Packing acknowledged successfully.",
                    icon: "success",
                    confirmButtonColor: "#556ee6"
                }).then(() => {
                });
                GetPackingListByID(NumberPlateDispatchID);
            },
            error: function (xhr, status, error) {
                let errorMessage = "Something went wrong while acknowledging.";
                if (xhr.responseText) errorMessage = xhr.responseText;
                Swal.fire({
                    title: "Error",
                    text: errorMessage + " (Status: " + xhr.status + ")",
                    icon: "error",
                    confirmButtonColor: "#556ee6"
                });
            }
        });
    });
}








