$(function () {
    pLoadingSetup(false);
    //GetPurchasePendingData();

    $("#divSearchPage").show();
    $("#divSearchResultSummary").show();
    FilterPurchaseEntry();


    pLoadingSetup(true);
});

$("#btnFilter").on('click', function () {
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    FilterPurchaseEntry();

    return false;
});
function FilterPurchaseEntry() {
    var FilterData = new Object();

    FilterData.SupplierID = $('#ddlSupplierType').val();
    FilterData.ComponentTypeID = $('#ddlComponentType').val();
    FilterData.sStartDate = $('#txtStartDate').val();
    FilterData.sEndDate = $('#txtEndDate').val();
    FilterData.SearchInWord = $('#txtSearchbox').val();

    GetPendingInspectionByFilter(FilterData);
}
function GetPendingInspectionByFilter(FilterData) {
    $.ajax({
        url: GetPendingPurchaseEntryByFilterUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(FilterData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
            DisplayCompletedInspectionData(response.data);
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

$("#btnClearFilter").on('click', function () {
    $('#txtStartDate').val("");
    $('#txtEndDate').val("");
    $('#txtSearchbox').val("");
    $("#ddlSupplierType").val("0").change();
    $("#ddlComponentType").val("0").change();
    FilterPurchaseEntry();
    return false;
});
function DisplayCompletedInspectionData(Completeddata) {
    $("#divSearchResultSummary").empty();
    let tableContent = '<div class="table-responsive">';

    tableContent += `
        <table class="table table-striped align-middle table-sm" id="tblCompletedInspection">
            <thead>
                <tr class="table-info">
                   <th>S No.</th>
				    <th>Bill No.</th>
					<th>Date</th>
					<th>Supplier</th>                    
                    <th>Code</th>
                    <th>Item</th>
                    <th>HSN Code</th>
                    <th>Colour</th>
                    <th>Size</th>
                    <th>Component</th>
                    <th class='text-end'>Quantity</th>                    
                    <th>Batch No</th>
                    <th class='text-end'>Batch Quantity</th>
                    <th>Action</th>
                </tr>
            </thead>
            <tbody>`;

    if (Completeddata.length != 0) {
        Completeddata.forEach((entry, index) => {
            tableContent += `
                <tr data-sno="${entry.sNo}">
                    <td>${index + 1}</td>
                    <td>${entry.PurchaseInvoiceNo}</td>
                    <td>${entry.sPurchaseInvoiceDate}</td>
                    <td>${entry.SupplierName}</td>
                    <td>${entry.ItemCode}</td>
                    <td>${entry.ItemName}</td>
                    <td>${entry.HSNCode}</td>
                    <td>${entry.ColorName}</td>
                    <td>${entry.SizeName}</td>
                    <td>${entry.ComponentTypeName}</td>
                    <td class='text-end'>${entry.Quantity.toFixed(2)} (${entry.UnitName})</td>
                    <td>${entry.BatchNo}</td>
                    <td class='text-end'>${entry.BatchQuantity.toFixed(2)}</td>                    
                    <td class='text-center'> 
                        <a href="javascript:void(0);" onclick="DeleteData('${entry.PendingInwardInspectionID}')" class="btn btn-sm btn-soft-danger">
                            <i class="mdi mdi-delete-outline"></i>
                        </a>
                     </td>
                   </tr>`;
        });
    }
    else {
        tableContent += `<tr><td colspan="14" class="text-center">No records to display</td></tr>`;
    }
    tableContent += `
            </tbody>
        </table>
    </div> `;

    $("#divSearchResultSummary").html(tableContent);

    $("#tblCompletedInspection").DataTable({
        "bAutoWidth": false,
        "bPaginate": false,
        "bFilter": true,
        "bSort": false,
        "order": [], // Disable initial sorting
        "pagingType": "full_numbers"
    });
}
function ViewMaterialInwardData(id) {
    if (ENABLE_VERBOSE_Logging) //console.log(id);
    $("#divViewModal").modal("show");

    $("#divbilldetails").empty();

    $.ajax({
        url: GetMaterialInwardDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { id: id },
        success: function (response) {

            if (response != null && response.data != null) {
                // Header details
                var headerDetails = `                    
                            <div class="row">
                                <div class="col-md-3">
                                    <div class="mb-2">Bill Number</div>
                                    <div class="fw-bold">${response.data.PurchaseInvoiceNo || 'N/A'}</div>
                                </div>
                                 <div class="col-md-3">
                                    <div class="mb-2">Bill Date</div>
                                    <div class="fw-bold">${response.data.sPurchaseInvoiceDate || 'N/A'}</div>
                                </div>
                                <div class="col-md-3">
                                    <div class="mb-2">Supplier</div>
                                    <div class="fw-bold">${response.data.SupplierName || 'N/A'}</div>
                                </div>                               
                                <div class="col-md-3">
                                    <div class="mb-2">Invoice Age</div>
                                    <div class="fw-bold">${response.data.PhoneNumber || 'N/A'}</div>
                                </div>
                            </div>
                       `;

                // Items table
                var itemsTable = `
                            <div class="table-responsive mt-3">
                                <table class="table table-sm">
                                    <thead class="table-info">
                                        <tr>
                                            <th>S.No</th>
                                            <th>Batch No</th>
                                            <th>BatchQuantity</th>
                                        </tr>
                                    </thead>
                                    <tbody>`;

                // Add items to table
                response.data.PendingInwardInspectionList.forEach((item, index) => {
                    itemsTable += `
                        <tr>
                            <td>${index + 1}</td>
                            <td>${item.BatchNo || 'N/A'}</td>
                            <td>${item.BatchQuantity || 'N/A'}</td>
                        </tr>`;
                });

                itemsTable += `
                                    </tbody>
                                </table>
                            </div>`;

                // Append both sections to the container
                $("#divbilldetails").append(headerDetails + itemsTable);
            } else {

                $("#divbilldetails").empty();
            }
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
                $("#btnFilter").click();
            }
            else
                Swal.fire({ title: "Error", text: DeleteErrorMessage, icon: "warning", confirmButtonColor: "#556ee6" });
        }, error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
    return false;
}
