var gBatchStock = [];
var gStockRequestData = [];

$(function () {
    pLoadingSetup(false);

    $("#btnSave").show();
    $("#btnUpdate").hide();

    $("#divAddEdit").hide();
    $("#divRecords").show();

    //calendar
    $("#txtRequestDate").datetimepicker({
        pickTime: false,
        useCurrent: true,
        format: 'DD/MM/YYYY',
        minYear: 1951,
        maxDate: moment()
    });

    // Set today's date manually in the input field
    $("#txtRequestDate").val(moment().format('DD/MM/YYYY'));
    
    $(".ClearDate").click(function () { var x = $(this).prev().attr('id'); $("#" + x).val(""); });
    $(".SelectDate").click(function () { var x = $(this).next().attr('id'); $("#" + x).focus(); });

    getRecordList();

    pLoadingSetup(true);
});

$('#ddlProcessType,#ddlSize').on('change', function () {
    gBatchStock = [];

    var FilterData = new Object();

    FilterData.ProcessTypeID = $('#ddlProcessType').val();
    if (FilterData.ProcessTypeID == 2) {
        $('#ddlSize').val(SizeNone);
        $("#ddlSize").prop("disabled", true);
    } else {
        $("#ddlSize").prop("disabled", false);   
    }
    FilterData.SizeID = $('#ddlSize').val();
    if (FilterData.ProcessTypeID > 0 && FilterData.SizeID > 0) {
        GetBatchStockByFilter(FilterData);
    }

    return false;
});
function GetBatchStockByFilter(FilterData) {
    //console.log(FilterData);
    $.ajax({
        url: GetBatchStockByFilterUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(FilterData),
        async:false,
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
            DisplayBatchStockData(response.data);
            $("btnFilter").hide();
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
function DisplayBatchStockData(batchStock) {
    $("#divBatchStock").empty();
    let colorCode = "bg-secondary bg-gradient text-white";
    let tableContent = '<div class="table-responsive">';

    gBatchStock = batchStock;
    tableContent += `
        <table class="table table-sm align-middle" id="tblSearchResult">
            <thead>
                <tr class="table-light">
                    <th class="${colorCode}">Component</th>
                    <th class="${colorCode}">Item</th>
                    <th class="${colorCode}">Colour</th>
                    <th class="${colorCode}">Size</th>                    
                    <th class="${colorCode}">Batch No</th>
                    <th class='${colorCode} text-end'>Batch Quantity</th>
                    <th class='${colorCode} text-end'>Consumed Quantity</th>
                    <th class='${colorCode} text-end'>Balance Quantity</th>
                    <th class='${colorCode} text-end'>Probable No of<br> Production Quantity</th>
                    <th class='${colorCode} text-center'>Action</th>
                </tr>
            </thead>
            <tbody>`;

    if (batchStock.length !== 0) {
        batchStock.forEach((entry, index) => {
            // if (entry.BalanceQty > 0) {
            let isSelected = gStockRequestData.some(stock => stock.BatchStockID === entry.BatchStockID);
            let rowClass = isSelected ? "table-success" : "";

            tableContent += `
            <tr class="${rowClass}" data-batchstockid="${entry.BatchStockID}">                  
                <td>${entry.ComponentTypeName}</td>
                <td>${entry.ItemName}</td>
                <td>${entry.ColorName}</td>
                <td>${entry.SizeName}</td>
                <td>${entry.BatchNo}</td>
                <td class='text-end'>${entry.BatchQuantity.toFixed(2)} ${entry.UnitName}</td>
                <td class='text-end'>${entry.ConsumedQty.toFixed(2)} ${entry.UnitName}</td>`;

            //2 Scenarios
            //if (entry.BalanceQty >= 0 && entry.ProdConsumedQty == 0)
            tableContent += `<td class='text-end'>${entry.BalanceQty.toFixed(2) || 0} ${entry.UnitName}</td>`;

            //else if (entry.ProdBalanceQty > 0 && entry.BalanceQty == 0) 
            //tableContent += `<td class='text-end'>${entry.ProdBalanceQty.toFixed(2) || 0}</td>`;

            tableContent += `
                <td class='text-end'>${entry.ProbableProductionQuantity} Pcs</td>
                <td class='text-center'>`;

            if (!isSelected && entry.StatusID == 1) {
                tableContent += `
                <a href="javascript:void(0);" onclick="AddtoRequest('${entry.BatchStockID}')" class="btn btn-sm btn-soft-warning" title="Click here to add to Stock Request List">
                    <i class="mdi mdi-basket-plus font-size-16 me-2 align-middle"></i>Add to List
                </a>`;
            } else {
                if (entry.StatusID == 1 && isSelected)
                    tableContent += ``;
                else if (entry.StatusID == 2)
                    tableContent += `<span class="badge bg-danger">In Production</span>`;
                else
                    tableContent += ``;
            }
            tableContent += `</td></tr>`;

            // }
        });
    }

    tableContent += `
            </tbody>
        </table>
    </div>`;

    $("#divBatchStock").html(tableContent);

    // Enable scroll if there are 5 or more rows
    if (batchStock.length >= 5) {
        $("#divBatchStock").css({
            'height': '300px',
            'overflow-y': 'auto'
        });
    } else {
        $("#divBatchStock").css({
            'height': '',
            'overflow-y': ''
        });
    }

    $("#tblSearchResult").DataTable({
        "bAutoWidth": false,
        "bPaginate": false,
        "bFilter": true,
        "bSort": false,
        "order": [], // Disable initial sorting
        "pagingType": "full_numbers"
    });

    return false;
}
function AddtoRequest(BatchStockID) {
    let searchID = Number(BatchStockID);

    if (gBatchStock.length > 0) {
        let filteredStocks = gBatchStock.find(stock => stock.BatchStockID === searchID);
        if (!filteredStocks) return false;

        //let duplicateComponent = gStockRequestData.find(stock =>
        //    stock.ComponentTypeID === filteredStocks.ComponentTypeID &&
        //    stock.ColorID !== filteredStocks.ColorID
        //);
        let duplicateComponent = gStockRequestData.find(stock =>
            stock.ComponentTypeID === filteredStocks.ComponentTypeID
        );

        //if (duplicateComponent) {
        //    $.jGrowl("Kindly Select the Same Color Component", { sticky: false, theme: 'warning', life: jGrowlLife });
        //    return false;
        //}
        if (duplicateComponent) {
            $.jGrowl("Kindly Select the Other Component", { sticky: false, theme: 'warning', life: jGrowlLife });
            return false;
        }

        let alreadyExists = gStockRequestData.find(stock => stock.BatchStockID === searchID);
        if (alreadyExists) return false;

        filteredStocks.StockRequestTransID = 0;
        filteredStocks.Quantity = filteredStocks.BalanceQty;
        gStockRequestData.push(filteredStocks);

        $("#ddlSize").val(filteredStocks.SizeID).change();
        //$("#ddlProcessType").val(filteredStocks.ProcessTypeID).change();
        $("#ddlSize").prop("disabled", true);
        $("#ddlProcessType").prop("disabled", true);

        if ($('#ddlProcessType').val() != 1) {
            $("#ddlColor").val(filteredStocks.ColorID).change();
            $("#ddlColor").prop("disabled", true);
        }

        DisplayStockRequestData();

        // **Highlight the selected row in green**
        // Remove highlight from all rows
        $("#tblSearchResult tbody tr").removeClass("table-success");

        // Find the row with this BatchStockID and add green background class
        $("#tblSearchResult tbody tr").each(function () {
            let batchNoInRow = $(this).find("td:eq(4)").text(); // Batch No is 5th column (0-based index 4)
            if (batchNoInRow === filteredStocks.BatchNo) {
                $(this).addClass("table-success"); // Bootstrap class for green background
            }
        });
    }

    return false;
}
function DisplayStockRequestData() {
    $("#divStockRequestItem").empty();

    let tableContent = '<div class="alert alert-info mt-5" role="alert">Item Details</div>';
    let colorCode = "bg-success bg-gradient text-white";
    tableContent += '<div class="table-responsive">'
    tableContent += `
        <table class="table table-sm table-striped align-middle" id="tblStockRequestItemData">
            <thead>
                <tr>
                    <th class="${colorCode}">Component</th>
                    <th class="${colorCode}">Item</th>
                    <th class="${colorCode}">Colour</th>
                    <th class="${colorCode}">Size</th>
				    <th class="${colorCode}">Batch No</th>
					<th class='${colorCode} text-end'>Request Quantity</th>
                    <th class='${colorCode} text-end'>Probable No of <br>Production Quantity</th>
                    <th class='${colorCode} text-center'>Action</th>
                </tr>
            </thead>
            <tbody>`;

    if (gStockRequestData.length != 0) {
        gStockRequestData.forEach((entry, index) => {
            tableContent += `
                    <tr>                  
                    <td>${entry.ComponentTypeName}</td>
                    <td>${entry.ItemName}</td>
                    <td>${entry.ColorName}</td>
                    <td>${entry.SizeName}</td>
                    <td>${entry.BatchNo}</td>
                    <th class='text-end'>${entry.Quantity.toFixed(2)} ${entry.UnitName}</th>
                    <th class='text-end'>${entry.ProbableProductionQuantity} Pcs</th>`;

            //New or Pending Status
            if ($("#hdnStatusID").val() == 1 || $("#hdnStatusID").val() == 0) {
                tableContent += `
                    <td class='text-center'> 
                            <a href="javascript:void(0);" onclick="DeleteStockRequestItem('${entry.BatchStockID}')" class="btn btn-sm btn-soft-danger" title="Click here to Delete Stock Request Item">
                                <i class="mdi-delete-outline align-middle"></i>
                            </a>
                     </td>`;
            }
            else
                tableContent += `<td></td>`;

            tableContent += `</tr> `;
        });
    }
    else {
        tableContent += `<tr> <td colspan="7" class="text-center">No data</td></tr> `;
    }

    tableContent += `
            </tbody>
        </table>
    </div> `;

    $("#divStockRequestItem").html(tableContent);

    return false;
}
function DeleteStockRequestItem(BatchStockID) {
    if (ENABLE_VERBOSE_Logging) //console.log(BatchStockID);

    Swal.fire({
        title: "Are you sure to delete?",
        text: "You won't be able to revert this!",
        icon: "question",
        showCancelButton: !0,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        customClass: { confirmButton: "btn btn-success mt-2", cancelButton: "btn btn-danger ms-2 mt-2" },
        buttonsStyling: !1,
    }).then(function (t) {
        t.value
            ? ConfirmDeleteStockRequestItem(BatchStockID)
            : t.dismiss === Swal.DismissReason.cancel && Swal.fire({ title: "Cancelled", text: "Your data is safe :)", icon: "error" });
    });

    return false;
}
function ConfirmDeleteStockRequestItem(BatchStockID) {
    // Ensure BatchStockID is a number (if stored as a string, convert it)
    let searchID = Number(BatchStockID);

    // Remove the item by filtering out the matching BatchStockID
    let filteredStocks = gStockRequestData.find(stock => stock.BatchStockID === searchID);
    if (filteredStocks && filteredStocks.StockRequestTransID > 0) DeleteStockRequestTransByID(filteredStocks.StockRequestTransID);

    gStockRequestData = gStockRequestData.filter(stock => stock.BatchStockID !== searchID);
    if (gStockRequestData.length == 0) {
        $("#ddlSize").prop("disabled", false);
        $("#ddlProcessType").prop("disabled", false);
    }

    DisplayBatchStockData(gBatchStock);
    DisplayStockRequestData();
    return false;
}
function DeleteStockRequestTransByID(id) {
    $.ajax({
        url: DeleteStockRequestransByIDUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(id),
        success: function (response) {
            if (response.success) {
                Swal.fire({ title: "Deleted!", text: DeleteSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
            }
            else
                Swal.fire({ title: "Error", text: DeleteErrorMessage, icon: "warning", confirmButtonColor: "#556ee6" });

        }, error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
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
            { "data": "RequestNo", "orderable": true, "width": "10%" },
            { "data": "sRequestDate", "orderable": true },
            { "data": "ProcessTypeName", "orderable": true },
            { "data": "RequestedByName", "orderable": true },
            { "data": "ApprovedByName", "orderable": true },
            {
                "data": "StockRequestStatus",
                "className": "text-center",
                "render": function (data, type, row) {
                    return `<span class="badge ${row.ColorCode}"> ${row.StockRequestStatus}</span> `;
                },
                "width": "10%",
                "orderable": false
            },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    let actionButtons = '';
                    if (row.StatusID > 1) {
                        actionButtons = `
                <ul class="list-unstyled hstack gap-1 mb-0">
                    <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                        <a href="javascript:void(0);" onclick="EditData(${row.StockRequestID}, true)" class="btn btn-sm btn-soft-primary">
                            <i class="mdi mdi-eye-outline"></i>
                        </a>
                    </li>
                                   </ul> ` ;
                        return actionButtons;
                    }
                    else {
                        return SetAction(row.StockRequestID);
                    }
                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");

    return false;
}

$("#btnAddNew").on('click', function () {
    $("#divAddEdit").show();
    $("#divRecords").hide();

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#ddlSize").prop("disabled", false);
    $("#divAddEdit .card-body :input").attr("disabled", false);
    ClearFormFields();
    gStockRequestData = [];

    $("#divCardTitle").html("<i class='fas fa-plus-square align-middle me-1'></i>Add New Stock Request");

    return false;
});

$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divRecords").show();
    $("#divAddEdit").hide();

    getRecordList();
    return false;
});
$('#btnRefresh').on('click', function () {
    getRecordList();
    return false;
});
function ClearFormFields() {
    $("#txtRequestNo").val("");
   // $("#txtRequestDate").val("");
    $("#ddlProcessType").val(0).change();
    $("#ddlComponent").val(0).change();
    $("#ddlSize").val(0).change();
    $("#ddlColor").val(0).change();

    $("#divBatchStock").empty();
    $("#divStockRequestItem").empty();
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();
    $("#hdnStatusID").val(0);
    $("#hdnStockRequestID").val(0);
    $("#hdnRequestedBy").val(0);
    $("#divRecordLog").hide();
    $("#txtRequestDate").val(moment().format('DD/MM/YYYY'));
    return false;
}

$("#btnSave,#btnUpdate").on('click', function () {
    let isValid = true;
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    var StockRequest = new Object();

    // Set ID and StatusFlag
    StockRequest.StockRequestID = 0;
    if (this.id == "btnUpdate" && $("#hdnStockRequestID").val() > 0) {
        StockRequest.StockRequestID = $("#hdnStockRequestID").val();
        StockRequest.RequestedBy = $("#hdnRequestedBy").val();
    }

    StockRequest.sRequestDate = $("#txtRequestDate").val();
    StockRequest.ProcessTypeID = $("#ddlProcessType").val();

    // Validations
    if (!StockRequest.sRequestDate) {
        $('#txtRequestDate').addClass('is-invalid');
        $('#txtRequestDate').after('<div class="invalid-feedback">Please select Request Date</div>');
        $('#txtRequestDate').focus();
        return false;
    }

    if (!StockRequest.ProcessTypeID || StockRequest.ProcessTypeID == 0) {
        $.jGrowl("Please select Process Type", { sticky: false, theme: 'warning', life: 3000 });
        $('#ddlProcessType').focus();
        return false;
    }

    if (gStockRequestData.length <= 0) {
        $.jGrowl("No Material(s) selected for Stock Request", { sticky: false, theme: 'warning', life: 3000 });
        return false;
    }

    StockRequest.StockRequestTrans = gStockRequestData;

    SaveandUpdateStockRequest(StockRequest);
    return false;
});
function SaveandUpdateStockRequest(StockRequestData) {
    if (ENABLE_VERBOSE_Logging) //console.log(StockRequestData);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(StockRequestData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);

            if (response.success) {
                if (StockRequestData.StockRequestID == 0) {
                    Swal.fire({ title: response.SRRequestNo, text: "Stock Request Submitted Successfully", icon: "success", confirmButtonColor: "#556ee6" });
                }
                else if (StockRequestData.StockRequestID > 0)
                    Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                //EditData(response.ID, false);
                $("#btnClose").click();
            }
            else if (!response.success && response.isExists) {
                Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
            }
            else if (!response.success && !response.isExists) {
                Swal.fire({ title: "Error", text: SaveErrorMessage, icon: "error", confirmButtonColor: "#556ee6" });
            }
            else
                Swal.fire({ title: "Error", text: response.message, icon: "error", confirmButtonColor: "#556ee6" });
        }, error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

    return false
}
function EditData(id, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) //console.log(id);
    ClearFormFields();
    gStockRequestData = [];
    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            if (ViewFlag) {
                $("#btnSave").hide();
                $("#btnUpdate").hide();

                $("#divAddEdit .card-body :input").attr("disabled", true);
                $("#divCardTitle").html("<i class='fas fa-eye'></i>&nbsp;&nbsp;View Stock Request");
            }
            else {
                $("#divAddEdit .card-body :input").attr("disabled", false);
                $("#divCardTitle").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Stock Request");

                $("#btnSave").hide();
                $("#btnUpdate").show();
            }
            $("#divAddEdit").show();
            $("#divRecords").hide();
            var stockdata = response.data;
            $("#hdnStockRequestID").val(stockdata.StockRequestID);
            $("#hdnRequestedBy").val(stockdata.RequestedBy);
            $("#txtRequestNo").val(stockdata.RequestNo);
            $("#txtRequestDate").val(stockdata.sRequestDate);
            $("#ddlProcessType").val(stockdata.ProcessTypeID).change();
            $("#hdnStatusID").val(stockdata.StatusID);

            $("#ddlProcessType").attr("disabled", true);

            var result = response.data.VStockRequestTrans;
            result.forEach((entry) => {
                var objTemp = new Object();
                objTemp.ItemName = entry.ItemName;
                objTemp.ComponentTypeName = entry.ComponentTypeName;
                objTemp.ColorName = entry.ColorName;
                objTemp.SizeName = entry.SizeName;
                objTemp.SizeID = entry.SizeID;
                objTemp.ColorID = entry.ColorID;
                objTemp.BatchNo = entry.BatchNo;
                objTemp.BatchQuantity = entry.BatchQuantity;
                objTemp.BatchStockID = entry.BatchStockID;
                objTemp.StockRequestTransID = entry.StockRequestTransID;
                objTemp.Quantity = entry.Quantity;
                objTemp.UnitName = entry.UnitName;
                objTemp.ProbableProductionQuantity = entry.ProbableProductionQuantity;
                objTemp.ComponentTypeID = entry.ComponentTypeID;

                gStockRequestData.push(objTemp);
            });

            if (gStockRequestData.length > 0) {
                let SizeID = gStockRequestData[0].SizeID;
                let ColorID = gStockRequestData[0].ColorID;

                $("#ddlSize").val(SizeID).change();
                $("#ddlSize").prop("disabled", true);

                //Enable Color dropdown only for Hydrolic Pressure and disable both Size and Color dropdown for all other Production process
                if ($('#ddlProcessType').val() != 1) {
                    $("#ddlColor").val(ColorID).change();
                    $("#ddlColor").prop("disabled", true);
                }
            }
            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + stockdata.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(stockdata.LastUpdatedDate));

            DisplayStockRequestData();
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
function DeleteData(id) {
    if (ENABLE_VERBOSE_Logging) //console.log(id);

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
            if (response.success && response.isExists) {
                Swal.fire({ title: "Deleted!", text: DeleteSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                $("#btnRefresh").click();
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

$("#ddlComponent").on('change', function () {

    if ($('#ddlProcessType').val() == 2 && $('#ddlComponent').val() == Hologram) {
        $("#ddlColor").val(ColorNone).change();
        $("#ddlSize").val(SizeNone).change();

        //console.log($("#ddlSize").val());
    }
    return false;
});
