
$(function () {
    pLoadingSetup(false);
    $("#divSearchPage").show();
    $("#divSearchResultSummary").show();
    FilterPurchaseEntry();
    //InitializeSearchPage();
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
    FilterData.sStartDate = $('#txtStartDate').val();
    FilterData.sEndDate = $('#txtEndDate').val();
    FilterData.SearchInWord = $('#txtSearchbox').val();

    GetPurchaseEntryByFilter(FilterData);
}
function GetPurchaseEntryByFilter(FilterData) {
    $.ajax({
        url: GetPurchaseEntryByFilterUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(FilterData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
            DisplayPurchaseEntryData(response.data);
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
    FilterPurchaseEntry();
    return false;
});
function DisplayPurchaseEntryData(Completeddata) {
    $("#divSearchResultSummary").empty();
    let tableContent = '<div class="table-responsive">';

    tableContent += `
        <table class="table table-striped table-sm" id="tblSearchResult">
            <thead>
                <tr class="table-info">
                   <th>S No.</th>
							<th>Bill No</th>
							<th>Date</th>
							<th>Supplier</th>
							<th>Total Nos</th>
							<th>Total Qty</th>
							<th>Total ItemTax</th>
							<th>Gross Amount (Rs.)</th>
							<th>Tax Amount (Rs.)</th>
							<th>Bill Amount (Rs.)</th>
							<th>Status</th>
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
                    <td>${entry.TotalPcs}</td>
                    <td>${entry.TotalQuantity}</td>
                    <td>${entry.TotalItemTax}</td>
                    <td>${entry.GrossAmount}</td>
                    <td>${entry.TaxAmount}</td>
                    <td>${entry.PurchaseInvoiceAmount.toFixed(2)}</td>
                    <td><span class="${entry.ColorCode}">${entry.PurchaseStatus}</span></td>
                    <td>
                        <ul class="list-unstyled hstack gap-1 mb-0">
                            <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                               <a href="javascript:void(0);" onclick="OpenPurchaseEntry(${entry.PurchaseEntryID}, 'view')" class="btn btn-sm btn-soft-primary">
                               <i class="mdi mdi-eye-outline"></i>
                               </a>
                            </li>
                            <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Edit">
                               <a href="javascript:void(0);" onclick="OpenPurchaseEntry(${entry.PurchaseEntryID}, 'edit')" class="btn btn-sm btn-soft-info">
                               <i class="mdi mdi-pencil-outline"></i>
                               </a>
                           </li>
                          <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
                              <a href="javascript:void(0);" onclick="DeleteData('${entry.PurchaseEntryID}')" class="btn btn-sm btn-soft-danger">
                                      <i class="mdi mdi-delete-outline"></i>
                               </a>
                          </li>
                        </ul>
                    </td>`;
        });
    }
    else {
        tableContent += `<tr><td colspan="11" class="text-center">No Batch Records To Display</td></tr>`;
    }
    tableContent += `
            </tbody>
        </table>
    </div> `;

    $("#divSearchResultSummary").html(tableContent);
}
function OpenPurchaseEntry(ID, mode) {
    $.cookie("PurchaseEntryID", parseInt(ID));
    $.cookie("PurchaseEntryMode", mode);

    const filterData = {
        supplierID: $('#ddlSupplierType').val() || "0",
        startDate: $('#txtStartDate').val() || "",
        endDate: $('#txtEndDate').val() || "",
        searchText: $('#txtSearchbox').val() || ""
    };
    $.cookie("PurchaseEntryFilters", JSON.stringify(filterData));

    window.location.href = "/Inventory/Purchase/PurchaseEntry";
    return false;
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
        },
        error: function (xhr, status, error) {
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
    return false;
}
