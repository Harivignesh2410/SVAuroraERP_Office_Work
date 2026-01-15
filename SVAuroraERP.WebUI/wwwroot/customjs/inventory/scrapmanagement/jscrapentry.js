var PageTitle = "Scrap Entry";
$(function () {
    pLoadingSetup(false);

    $("#btnSave").hide();
    $("#btnClose").show();
    getRecordList();
    GetAvailableScrapStock();
    if (!_CMActionAdd) {
        $("#btnAddNew").remove();
        $("#btnSave").remove();
    }
    if (!_CMActionUpdate) $("#btnUpdate").remove();
    $("#btnUpdate").hide();
    //// Set default visibility on page load
    $("#divAddEdit").hide();  // Hide the add/edit section
    $("#divRecords").show();  // Show the records section
    $("#txtScrapDate").datetimepicker({
        pickTime: false,
        useCurrent: true,
        format: 'DD/MM/YYYY',
        minYear: 1951,
        maxDate: moment()
    });
    pLoadingSetup(true);
});

$("#btnAddNew").on("click", function () {
    $("#divAddEdit").show();
    $("#divRecords").hide();
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    ClearFormFields();
    GetAvailableScrapStock();

    $("#divCardTitle").html("<i class='bx bxs-plus-square align-middle me-1'></i>Add New " + PageTitle);

    return false;
});
$("#btnCloseWindow,#btnClose").on('click', function () {
    $("#divAddEdit").hide();
    $("#divRecords").show();
    $("#btnRefresh").click();
    return false;
});
$("#btnRefresh").on('click', function () {
    getRecordList();
    GetAvailableScrapStock();
    return false;
});
function ClearFormFields() {
    $("#divAddEdit .card-body :input").attr("disabled", false);
    $("#hdnScrapEntryID").val("0");
    $('.form-control').removeClass('is-invalid');
    $("#txtScrapEntyNo").val("");
    $("#txtScrapDate").val("");
    $("#txtTotalSoldQty").prop("disabled", true).val("");

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#divRecordLog").hide();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

    return false;
}
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
                orderable: false, // Disable sorting for this column},
                "width": "5%",
                "orderable": false
            },
            //{ "data": "SupplierCode", "orderable": true, "width": "5%" },
            { "data": "ScrapEntryNo", "orderable": true, "width": "10%" },
            { "data": "sScrapDate", "orderable": true, "width": "10%" },
            { "data": "TotalSoldQty", "orderable": true, "width": "10%" },
            { "data": "ComponentSizeList", "orderable": true },
            {
                data: null,
                bSortable: false,
                "className": "text-center",
                render: function (data, type, row) {
                    return `
                        <ul class="list-unstyled hstack gap-1 mb-0">
                            <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                                <a href="javascript:void(0);" onclick="EditData(${row.ScrapEntryID}, true)" class="btn btn-sm btn-soft-primary">
                                    <i class="mdi mdi-eye-outline"></i>
                                </a>
                            </li>
                            <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
                                <a href="javascript:void(0);" onclick="DeleteData('${row.ScrapEntryID}')" class="btn btn-sm btn-soft-danger">
                                    <i class="mdi mdi-delete-outline"></i>
                                </a>
                            </li>
                        </ul>`;
                },

                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}
function GetAvailableScrapStock() {
    var data = new Object();
    data.ALUMINUMCOILID = AluminiumCoil;
    data.BLANKPLATEID = BLANKPLATE;
    data.HOLOGRAMPLATEID = HOLOGRAMPLATE;
    data.SCRAPENTRYID = $("#hdnScrapEntryID").val() || 0;
    $.ajax({
        url: GetAvailableScrapStockUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(data),
        async: false,
        success: function (response) {
                DisplayScrapStockData(response.result.Value);
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({
                title: "Error",
                text: error.responseText,
                icon: "error",
                confirmButtonColor: "#556ee6"
            });
        }
    });
    return false;
}
function DisplayScrapStockData(Stockdata) {
    $("#divScrapStock").empty();
    let colorCode = "bg-primary bg-gradient text-white";

    let totalWastage = 0, totalSold = 0, totalBalance = 0;
    Stockdata.forEach(entry => {
        totalWastage += entry.WastageQtyInKG || 0;
        totalSold += entry.SoldQty || 0;
        totalBalance += entry.BalanceQty || 0;
    });

    let SCRAPENTRYID = parseInt($("#hdnScrapEntryID").val()) || 0;
    let isExistingEntry = SCRAPENTRYID > 0;

    // Summary section
    let summaryCards = `
       <div class="row">
            <div class="col-md-4">
                <div class="card mini-stats-wid">
                    <div class="card-body">
                        <div class="d-flex">
                            <div class="flex-grow-1">
                                <p class="text-muted fw-medium">Total Wastage</p>
                                <h4 class="mb-0" id="totalWastage">${totalWastage.toFixed(2)} KG</h4>
                            </div>
                            <div class="flex-shrink-0 align-self-center">
                                <div class="mini-stat-icon avatar-sm rounded-circle bg-danger">
                                    <span class="avatar-title">
                                        <i class="bx bx-trash font-size-24"></i>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card mini-stats-wid">
                    <div class="card-body">
                        <div class="d-flex">
                            <div class="flex-grow-1">
                                <p class="text-muted fw-medium">Total Sold</p>
                                <h4 class="mb-0" id="totalSold">${totalSold.toFixed(2)} KG</h4>
                            </div>
                            <div class="flex-shrink-0 align-self-center">
                                <div class="avatar-sm rounded-circle bg-success mini-stat-icon">
                                    <span class="avatar-title rounded-circle bg-success">
                                        <i class="bx bx-shopping-bag font-size-24"></i>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-4">
                <div class="card mini-stats-wid">
                    <div class="card-body">
                        <div class="d-flex">
                            <div class="flex-grow-1">
                                <p class="text-muted fw-medium">Total Balance</p>
                                <h4 class="mb-0" id="totalBalance">${totalBalance.toFixed(2)} KG</h4>
                            </div>
                            <div class="flex-shrink-0 align-self-center">
                                <div class="avatar-sm rounded-circle bg-info mini-stat-icon">
                                    <span class="avatar-title rounded-circle bg-info">
                                        <i class="bx bx-wallet font-size-24"></i>
                                    </span>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>`;
    $("#divScrapSummary").html(summaryCards);

    // Table
    let tableContent = `
        <div class="table-responsive shadow-sm rounded">
            <table class="table table-sm table-hover align-middle" id="tblScrapStock">
                <thead>
                    <tr>
                        <th class="${colorCode} text-center">#</th>
                        <th class="${colorCode}">Component</th>
                        <th class="${colorCode}">Size</th>
                        <th class="${colorCode} text-end">Wastage Qty (KG)</th>
                        <th class="${colorCode} text-end">Total Sold Qty (KG)</th>
                        <th class="${colorCode} text-end">Balance Qty (KG)</th>`;

    // Conditionally add column header
    if (isExistingEntry) {
        tableContent += `<th class="${colorCode} text-end">Trans Sold Qty (KG)</th>`;
    } else {
        tableContent += `
            <th class="${colorCode} text-center">Sold Qty</th>
            <th class="${colorCode} text-center">All</th>`;
    }

    tableContent += `
                    </tr>
                </thead>
                <tbody>`;

    // Table body
    if (Stockdata.length > 0) {
        Stockdata.forEach((entry, index) => {
            tableContent += `
                <tr>
                    <td class="text-center">${index + 1}</td>
                    <td>${entry.ComponentTypeName}</td>
                    <td>${entry.SizeName}</td>
                    <td class="text-end">${entry.WastageQtyInKG.toFixed(2)} KG</td>
                    <td class="text-end">${entry.SoldQty.toFixed(2)} KG</td>
                    <td class="text-end balance-qty" data-balance="${entry.BalanceQty.toFixed(2)}">
                        ${entry.BalanceQty.toFixed(2)} KG
                    </td>`;

            if (isExistingEntry) {
                // Show TransSoldQty
                tableContent += `<td class="text-end">${(entry.TransSoldQty || 0).toFixed(2)} KG</td>`;
            } else {
                // Show input and checkbox for new entry
                tableContent += `
                    <td class="text-center">
                        <input type="number"
                               class="form-control text-end sold-qty-input"
                               id="txtSoldQty_${entry.FK_ComponentTypeID}_${entry.FK_SizeID}"
                               data-component="${entry.ComponentTypeID}"
                               data-size="${entry.SizeID}"
                               min="0" step="0.01"
                               placeholder="Enter Qty" />
                    </td>
                    <td class="text-center">
                        <input type="checkbox" class="form-check-input select-all-checkbox" title="Use full balance" />
                    </td>`;
            }

            tableContent += `</tr>`;
        });
    } else {
        tableContent += `<tr><td colspan="8" class="text-center text-muted">No stock data available</td></tr>`;
    }

    tableContent += `
                </tbody>
            </table>
        </div>`;

    $("#divScrapStock").html(tableContent);

    // Only bind input/checkbox logic for new entries
    if (!isExistingEntry) {
        function updateTotalSoldQty() {
            let total = 0;
            $(".sold-qty-input").each(function () {
                const val = parseFloat($(this).val()) || 0;
                total += val;
            });
            $("#txtTotalSoldQty").val(total.toFixed(2));
        }

        $(".sold-qty-input").on("input", function () {
            const $input = $(this);
            const $row = $input.closest("tr");
            const balanceQty = parseFloat($row.find(".balance-qty").data("balance")) || 0;
            const enteredQty = parseFloat($input.val()) || 0;

            if (enteredQty > balanceQty) {
                markInvalid("#" + $input.attr("id"), "Entered quantity cannot be greater than the balance quantity.");
                $input.val("");
            }

            updateTotalSoldQty();
        });

        $(".select-all-checkbox").on("change", function () {
            const $row = $(this).closest("tr");
            const balanceQty = parseFloat($row.find(".balance-qty").data("balance")) || 0;
            const $input = $row.find(".sold-qty-input");

            if ($(this).is(":checked")) {
                $input.val(balanceQty.toFixed(2)).prop("disabled", true);
            } else {
                $input.val("").prop("disabled", false);
            }
            updateTotalSoldQty();
        });
    }

    // Scroll handling
    if (Stockdata.length >= 5) {
        $("#divScrapStock").css({ "max-height": "350px", "overflow-y": "auto" });
    } else {
        $("#divScrapStock").css({ "max-height": "", "overflow-y": "" });
    }

    // DataTable init
    $("#tblScrapStock").DataTable({
        paging: false,
        searching: false,
        ordering: false,
        info: false,
        autoWidth: false
    });
}

$("#btnSave,#btnUpdate").on('click', function () {
    if (this.id == "btnSave") {
        if (!_CMActionAdd) {
            $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
            return false;
        }
    } else if (this.id == "btnUpdate") {
        if (!_CMActionUpdate) {
            $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
            return false;
        }
    }

    // Clear previous validation
    $('.form-control').removeClass('is-invalid');

    // 🔹 Prepare main object
    let ScrapEntryData = {};
    ScrapEntryData.ScrapEntryID = 0;
    if ((this.id == "btnUpdate" && $("#hdnScrapEntryID").val() > 0))
        ScrapEntryData.ScrapEntryID = $("#hdnScrapEntryID").val();

    // 🔹 Form inputs
    ScrapEntryData.sScrapDate = $('#txtScrapDate').val();
    ScrapEntryData.TotalSoldQty = $('#txtTotalSoldQty').val();

    if (!ScrapEntryData.sScrapDate)
        return markInvalid("#txtScrapDate", "Please Select Scrap Date");
    if (!ScrapEntryData.TotalSoldQty || parseFloat(ScrapEntryData.TotalSoldQty) <= 0)
        return markInvalid("#txtTotalSoldQty", "Please enter Sold Qty");

    // 🔹 Collect Sold Quantity details like $("input[id^='btnAddQuantity_']")
    let ScrapEntryTrans = [];
    $("input[id^='txtSoldQty_']").each(function () {
        // Example ID: txtSoldQty_3_5 → ComponentID = 3, SizeID = 5
        let idParts = this.id.split("_");
        let componentID = parseInt(idParts[1]) || 0;
        let sizeID = parseInt(idParts[2]) || 0;
        let soldQtyValue = parseFloat($(this).val()) || 0;

        if (soldQtyValue > 0) {
            ScrapEntryTrans.push({
                ComponentTypeID: componentID,
                SizeID: sizeID,
                SoldQty: parseFloat(soldQtyValue.toFixed(2))
            });
        }
    });

    ScrapEntryData.ScrapEntryTransList = ScrapEntryTrans;

    SaveandUpdateScrap(ScrapEntryData);

    return false;
});
function SaveandUpdateScrap(ScrapEntryData) {

        $.ajax({
            url: SaveUpdateDataUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(ScrapEntryData),
            success: function (response) {
                if (response != null && response.dataResponse != null) {
                    var result = response.dataResponse;
                    if (!result.Error && result.Success && result.ID > 0) {
                        if (ScrapEntryData.ScrapEntryID == 0) {
                            Swal.fire({
                                title: "Saved!",
                                text: SaveSuccessMessage,
                                icon: "success",
                                confirmButtonColor: "#556ee6"
                            }).then(function () {
                            });
                            EditData(result.ID);
                        }
                        else if (ScrapEntryData.ScrapEntryID > 0) {
                            // Existing supplier updated
                            Swal.fire({
                                title: "Updated!",
                                text: UpdateSuccessMessage,
                                icon: "success",
                                confirmButtonColor: "#556ee6"
                            });

                            // Refresh the data if needed
                            //EditData(saveSupplierid, false);
                        }
                    }
                    else if (result.Error && !result.Success && result.ID > 0) {
                        Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
                    } else {
                        Swal.fire({ title: "Warning", text: result.Message, icon: "warning", confirmButtonColor: "#556ee6" });
                    }
                } else {
                    Swal.fire({ title: "Error!", text: response.Message, icon: "error", confirmButtonColor: "#556ee6" });
                }
            },
            error: function (xhr, status, error) {
                if (ENABLE_VERBOSE_Logging) //console.log(error);
                    Swal.fire({ title: "Error", text: xhr.responseText, icon: "error", confirmButtonColor: "#556ee6" });
            }
        });

    return false;
}
function EditData(id) {
    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
                $("#btnSave").hide();
                $("#btnUpdate").hide();

                $("#divAddEdit .card-body :input").attr("disabled", true);
                $("#divCardTitle").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Stock Request");
          
            $("#divAddEdit").show();
            $("#divRecords").hide();
            var stockdata = response.result.Value;



            $("#txtScrapEntyNo").val(stockdata.ScrapEntryNo);
            $("#txtScrapDate").val(stockdata.sScrapDate);
            $("#txtTotalSoldQty").val(stockdata.TotalSoldQty);
            $("#hdnScrapEntryID").val(stockdata.ScrapEntryID);

            GetAvailableScrapStock();
            
            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + stockdata.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(stockdata.LastUpdatedDate));

        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}

// Delete a punching record with confirmation
function DeleteData(id) {
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
                ? ConfirmDelete(id)
                : t.dismiss === Swal.DismissReason.cancel && Swal.fire({ title: "Cancelled", text: "Your data is safe :)", icon: "error" });
        });

    return false;
}
function ConfirmDelete(id) {
    $.ajax({
        url: DeleteDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(id),
        success: function (response) {
            if (response && response.result) {
                var result = response.result;
                if (!result.Error && result.Success && result.ID > 0) {
                    Swal.fire({
                        title: "Deleted!",
                        text: DeleteSuccessMessage,
                        icon: "success",
                        confirmButtonColor: "#556ee6"
                    });
                    $("#btnRefresh").click();
                }
                else if (result.Error && !result.Success && result.ID > 0) {
                    Swal.fire({
                        title: "Warning",
                        text: result.Message,
                        icon: "warning",
                        confirmButtonColor: "#556ee6"
                    });
                }
                else {
                    Swal.fire({ title: "Error", text: result.Message, icon: "error", confirmButtonColor: "#556ee6" });
                }
            }
            else
                Swal.fire({ title: "Error", text: DeleteErrorMessage, icon: "warning", confirmButtonColor: "#556ee6" });
        },
        error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
    return false;
}


