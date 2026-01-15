var MaterialInwardArray = [];
var PurchaseTransList = [];
var pendingQuantity = [];
var purchaseentryID = 0;
var latestPendingQuantity = 0;

$(function () {
    pLoadingSetup(false);
    //GetPurchasePendingData();

    $("#divPurchasePendingData").show();
    $("#divPurchaseDetails").hide();
    $("#divMaterialInward").hide();
    FilterPurchaseEntry();

    $('#ddlSupplierType').select2();

    pLoadingSetup(true);
});
function getMaxSNo() {
    if (MaterialInwardArray.length === 0) return 0;
    return Math.max(...MaterialInwardArray.map(item => parseInt(item.sNo) || 0));
}
$(document).on("click", "#btnProceed", function () {
    $("#divSearchPage").hide();
    $("#divPurchasePendingData").hide();
    $("#divMaterialInward").show();
    $("#divPurchaseDetails").show();
    //$("#divsidebar").hide();

});
function DisplayPurchasePendingData(pendingdata) {
    $("#divPurchasePendingData").empty();
    if (pendingdata != null) {
        6
        var invoiceCardHTML = "";
        // var count = 0;
        invoiceCardHTML = "<div class='row g-4'>";
        pendingdata.forEach(function (invoiceData) {
            invoiceCardHTML += `                   
                    <div class="col-xl-4 col-lg-6 col-md-6">
                        <div class="card">
                            <div class="card-body">
                            <div class="row g-3 mb-3">
                               <div class="col-md-8">
                                <h5 class="fw-semibold">Invoice No.
                                     <a href="#" onclick="ViewMaterialInwardData(${invoiceData.PurchaseEntryID}, true)" data-bs-toggle="modal" data-bs-target="#divViewModal">
                                       ${invoiceData.PurchaseInvoiceNo || 'N/A'}
                                     </a>
                                </h5>
                               </div>
                               <div class="col-md-4 text-end">
                                 <span class="${invoiceData.ColorCode}">${invoiceData.PurchaseStatus}</span>
                               </div>
                             </div>
                                <div class="table-responsive">
                                    <table class="table">
                                        <tbody> 
                                            <tr>
                                                <th scope="row">Supplier</th>
                                                <td>${invoiceData.SupplierName || 'N/A'}</td>
                                            </tr>
                                            <tr>
                                                <th scope="row">Date</th>
                                                <td>${invoiceData.sPurchaseInvoiceDate || 'N/A'}</td>
                                            </tr>
                                            <tr>
                                                <th scope="row">Component(s)</th>
                                                <td>
                                                    ${invoiceData.ComponentNames || 'N/A'}
                                                </td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                <div class="hstack gap-5 w-4">
                                   <button type="button"  onclick="ViewMaterialInwardData(${invoiceData.PurchaseEntryID})" class="btn btn-soft-primary ">
                                            <i class="bx bxs-navigation"></i> View Details
                                    </button>
                                    <button class="btn btn-soft-primary  ms-auto" id="btnProceed" onclick="GetPurchaseTransDetailsByID(${invoiceData.PurchaseEntryID})">Proceed</button>
                                </div>
                            </div>
                        </div>
                         </div>`;
        });

        invoiceCardHTML += "</div>";
        $("#divPurchasePendingData").append(invoiceCardHTML);

        $("#divSearchResultSummary").empty();

        var textdata = `<div class="alert alert-success" role="alert">
                                We have found <strong>${pendingdata.length}</strong> search result(s) for you!
                                </div>`;

        $("#divSearchResultSummary").append(textdata);
    }
    else {

        $("#divPurchasePendingData").empty();
    }
}
function ViewMaterialInwardData(id, ViewFlag) {
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
                                            <th>Item</th>
                                            <th>HSN Code</th>
                                            <th>Units</th>
                                            <th>Size</th>
                                            <th>Quantity</th>
                                            <th>Color</th>
                                        </tr>
                                    </thead>
                                    <tbody>`;

                // Add items to table
                response.data.PurchaseEntryTransList.forEach((item, index) => {
                    itemsTable += `
                        <tr>
                            <td>${index + 1}</td>
                            <td>${item.ItemName || 'N/A'}</td>
                            <td>${item.HSNCode || 'N/A'}</td>
                            <td>${item.UnitName || 'N/A'}</td>
                            <td>${item.SizeName || 'N/A'}</td>
                            <td>${item.Quantity || 'N/A'}</td>
                            <td>${item.ColorName || 'N/A'}</td>
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
//Get PurchaseTrans Details by Purchase ID
function GetPurchaseTransDetailsByID(ID) {
    $("#divPurchaseDetails").empty();
    //MaterialInwardArray = [];
    $.ajax({
        url: GetMaterialInwardDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            if (response != null && response.data != null) {
                $("#hdnPurchaseEntryID").val(ID);
                
                $("#divSearchPage").hide();
                PurchaseTransList = response.data.PurchaseEntryTransList;

                // Fetch inward data first before rendering UI
                GetMaterialInwardDataByPurchaseID(ID, function () {
                    RenderPurchaseDetails(response.data);
                    DisplayDataTable(MaterialInwardArray);
                });

                $("#divMaterialInward").show();
            } else {
                $("#divPurchaseDetails").empty();
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(response);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
function GetMaterialInwardDataByPurchaseID(ID, callback) {
    MaterialInwardArray = [];

    $.ajax({
        url: GetMaterialInwardDataByPurchaseIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            if (response.result.Error != true && response.result.Success == true) {
                var result = response.result.Value;
                result.forEach((batchdata, index) => {
                    var objTemp = {
                        sNo: index + 1,
                        PendingInwardInspectionID: batchdata.PendingInwardInspectionID,
                        PurchaseTransID: batchdata.PurchaseTransID,
                        BatchNo: batchdata.BatchNo,
                        BatchQuantity: batchdata.BatchQuantity,
                        IsAutoBatch: batchdata.IsAutoBatch,
                        PendingQuantity: batchdata.PendingQuantity,
                        LessQuantity:batchdata.LessQuantity,
                        ExcessQuantity: batchdata.ExcessQuantity,
                        StatusFlag: ""
                    };
                    MaterialInwardArray.push(objTemp);
                    pendingQuantity = result[result.length - 1].PendingQuantity;
                });
            }
            

            if (MaterialInwardArray.length == 0) {
                $('#btnSave').show();
                $('#btnUpdate').hide();
            } else {
                $('#btnSave').hide();
                $('#btnUpdate').show();
            }

            //  DisplayDataTable(MaterialInwardArray);

            if (callback) {
                callback(); // Call the function after data is ready
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
function RenderPurchaseDetails(data) {
    var headerDetails = `
        <div class="row task-dates">
            <div class="col-sm-3 col-6">
                <p class="text-muted mb-2">Bill No.</p>
                <h5 class="font-size-14"><i class="bx bx-copy-alt me-1 text-primary"></i>${data.PurchaseInvoiceNo}</h5>
            </div>
            <div class="col-sm-3 col-6">                
                <p class="text-muted mb-2">Bill Date</p>
                <h5 class="font-size-14"><i class="bx bx-calendar-check me-1 text-primary"></i>${data.sPurchaseInvoiceDate}</h5>
            </div>
            <div class="col-sm-3 col-6">                
                <p class="text-muted mb-2">Supplier</p>
                <h5 class="font-size-14"><i class="bx bx-user me-1 text-primary"></i>${data.SupplierName}</h5>
            </div>
            <div class="col-sm-3 col-6">                
                <p class="text-muted mb-2">Bill Amount (Rs.)</p>
                <h5 class="font-size-14"><i class="bx bx-rupee me-1 text-danger"></i>${data.PurchaseInvoiceAmount}</h5>
            </div>
        </div>
    `;

    var itemsTable = `
        <div class="table-responsive mt-4">
            <table class="table align-middle">
                <thead class="table-info">
                    <tr>
                        <th>S.No</th>
                        <th>Item</th>
                        <th>HSN Code</th>
                        <th>Quantity</th>
                        <th>Units</th>
                        <th>Size</th>
                        <th>Color</th>
                        <th>Component Type</th>
                        <th>Action</th>
                    </tr>
                </thead>
                <tbody>`;

    PurchaseTransList.forEach((item, index) => {
        const totalBatchQuantity = MaterialInwardArray
            .filter(entry => entry.PurchaseTransID === item.PurchaseTransID)
            .reduce((sum, entry) => sum + entry.BatchQuantity, 0);

        const pendingQuantity = item.Quantity - totalBatchQuantity;
        const buttonHtml = pendingQuantity > 0
            ? `<button type="button" id="btnAddNewItems_${item.PurchaseTransID}" class="btn btn-sm btn-warning btn-rounded waves-effect waves-light w-100"><i class="fas fa-plus-square label-icon me-2"></i>Add</button>`
            : `<button type="button" class="badge bg-success" disabled=""><i class="fas fa-check-circle label-icon me-2"></i>Completed</button>`;

        itemsTable += `
            <tr>
                <td>${index + 1}</td>
                <td>${item.ItemName || 'N/A'}</td>
                <td>${item.HSNCode || 'N/A'}</td>
                <td>${item.Quantity || 'N/A'}</td>
                <td>${item.UnitName || 'N/A'}</td>
                <td>${item.SizeName || 'N/A'}</td>
                <td>${item.ColorName || 'N/A'}</td>
                <td>${item.ComponentTypeName || 'N/A'}</td>
                <td>${buttonHtml}</td>
            </tr>`;
    });

    itemsTable += `
                </tbody>
            </table>
        </div>
        <div class="table-responsive">
            <div id="divReport" class="mt-2"></div>
        </div>`;

    $("#divPurchaseDetails").append(headerDetails + itemsTable);
}
function GetBatchInputModal(ID) {
    $("#hdnPurchaseTransID").val(ID);

    $("#divModal").modal('show');
}
$(document).on("click", "[id^='btnAddNewItems_']", function () {
    // const purchaseTransID = parseInt($("#hdnPurchaseTransID").val());
    const purchaseTransID = parseInt($(this).attr("id").split("_")[1]);
    parseInt($("#hdnPurchaseTransID").val(purchaseTransID));
    const purchaseEntry = PurchaseTransList.find(item => item.PurchaseTransID === purchaseTransID);

    if (!purchaseEntry) {
        return false;
    }

    if (purchaseEntry.ComponentTypeID === AluminiumCoil) {
        $("#divModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Batch ");        
        GetPurchaseDetailsbyID(purchaseTransID);
        $('#btnSaveBatch').show();
        $('#btnUpdateBatch').hide();
        ClearModuleFormFields();
        GenerateBatchNo();
        $("#divModal").modal("show");
    } else {
        GetBatchInputModal(purchaseTransID);
    }

    return false;
});

$("#btnSaveBatch,#btnUpdateBatch").on("click", function () {
    let isValid = true;

    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    let MaterialInward = new Object();
    MaterialInward.BatchNo = $('#txtBatchNo').val();
    MaterialInward.BatchQuantity = parseFloat($('#txtBatchQuantity').val());
    MaterialInward.PurchaseTransID = parseInt($("#hdnPurchaseTransID").val());
    MaterialInward.IsAutoBatch = false;
    MaterialInward.PendingInwardInspectionID = parseInt($("#hdnPendingInwardInspectionID").val()) || 0;
    MaterialInward.LessQuantity = parseFloat($('#txtLessQuantity').val());
    MaterialInward.ExcessQuantity = parseFloat($('#txtExcessQuantity').val());

    // Find the related PurchaseTrans entry
    const purchaseEntry = PurchaseTransList.find(item =>
        item.PurchaseTransID === MaterialInward.PurchaseTransID
    );

    if (!purchaseEntry) {
        $.jGrowl("Invalid PurchaseTransID!", { sticky: false, theme: 'error', life: 3000 });
        return false;
    }

    // Calculate the total batch quantity for this PurchaseTransID
    const existingBatches = MaterialInwardArray.filter(entry =>
        entry.PurchaseTransID === MaterialInward.PurchaseTransID &&
        entry.StatusFlag !== "D" &&
        (this.id === "btnUpdateBatch" ? entry.sNo !== parseInt($("#hdnSNo").val()) : true)
    );

    const totalBatchQuantity = existingBatches.reduce((sum, entry) => sum + parseFloat(entry.BatchQuantity), 0);

    // Calculate the actual pending quantity
    const actualPendingQuantity = purchaseEntry.Quantity - totalBatchQuantity;

    // Store the pending quantity for this batch
    MaterialInward.PendingQuantity = actualPendingQuantity - MaterialInward.BatchQuantity;

    // Validation checks...
    if (!MaterialInward.BatchNo) {
        $('#txtBatchNo').addClass('is-invalid');
        $('#txtBatchNo').after('<div class="invalid-feedback">Please Enter Batch No</div>');
        $('#txtBatchNo').focus();
        isValid = false;
        return false;
    }

    if (!MaterialInward.BatchQuantity || MaterialInward.BatchQuantity <= 0) {
        $('#txtBatchQuantity').addClass('is-invalid');
        $('#txtBatchQuantity').after('<div class="invalid-feedback">Please Enter a Valid Quantity</div>');
        $('#txtBatchQuantity').focus();
        isValid = false;
        return false;
    }

    if (MaterialInward.BatchQuantity > actualPendingQuantity) {
        $.jGrowl(`The entered quantity (${MaterialInward.BatchQuantity}) exceeds the pending quantity (${actualPendingQuantity.toFixed(2)}). Please enter a valid value.`, {
            sticky: false,
            theme: 'warning',
            life: 3000
        });
        $('#txtBatchQuantity').focus();
        isValid = false;
        return false;
    }


    if (isValid) {
        const PendingInwardInspectionID = parseInt($("#hdnPendingInwardInspectionID").val());

        if (this.id === "btnSaveBatch") {
            var maxSNo = getMaxSNo();
            MaterialInward.sNo = maxSNo + 1;
            MaterialInward.StatusFlag = "I"; // Insert
            MaterialInward.PendingInwardInspectionID = 0;

            if (!isDuplicateEntry(MaterialInward.BatchNo, 0)) {
                Add_MaterialInward(MaterialInward);
            } else {
                $.jGrowl("The entered batch number already exists!", { sticky: false, theme: 'warning', life: 3000 });
                return false;
            }
        } else if (this.id === "btnUpdateBatch") {
            // For updates
            const currentSNo = parseInt($("#hdnSNo").val());
            MaterialInward.sNo = currentSNo;

            if (PendingInwardInspectionID > 0) {
                MaterialInward.StatusFlag = "U"; // Update
                MaterialInward.PendingInwardInspectionID = PendingInwardInspectionID;
            } else {
                MaterialInward.StatusFlag = "I"; // Insert
                MaterialInward.PendingInwardInspectionID = 0;
            }

            if (!isDuplicateEntry(MaterialInward.BatchNo, MaterialInward.PurchaseTransID)) {
                Update_MaterialInward(MaterialInward);
            } else {
                $.jGrowl(MaterialInward.BatchNo + " is already added to the list.", {
                    sticky: false,
                    theme: 'warning',
                    life: jGrowlLife
                });
                return false;
            }
        }

        if (this.id === "btnSaveBatch") {
            ClearModuleFormFields();
        } else {
            $("#divModal").modal('hide');
        }
    }
});

//Clear the Form field for the Batch Module
function ClearModuleFormFields() {
    $("#divModal .modal-body :input").attr("disabled", false);
    $("#hdnSNo").val("0");
    //$("#hdnPendingInwardInspectionID").val("0");

    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#txtBatchNo").val("");
    $("#txtBatchQuantity").val("0");
    $("#txtLessQuantity").val("0");
    $("#txtExcessQuantity").val("0");

    $("#btnSaveBatch").show();
    $("#btnUpdateBatch").hide();

    return false;
}
// To find the Duplicate Batch number in the Material Inward Inspection Array
function isDuplicateEntry(BatchNo, PurchaseTransID) {
    for (let i = 0; i < MaterialInwardArray.length; i++) {
        if (MaterialInwardArray[i].BatchNo == BatchNo && MaterialInwardArray[i].PurchaseTransID !== PurchaseTransID) {
            return true;
        }
    }
    return false;
}
// To Add the Data in the Material Inward Inspeciton Array
function Add_MaterialInward(oData) {
    MaterialInwardArray.push(oData);


    // Update button dynamically
    const purchaseEntry = PurchaseTransList.find(item => item.PurchaseTransID === oData.PurchaseTransID);
    if (purchaseEntry) {
        const totalBatchQuantity = MaterialInwardArray
            .filter(entry => entry.PurchaseTransID === oData.PurchaseTransID)
            .reduce((sum, entry) => sum + entry.BatchQuantity, 0);

        const pendingQuantity = purchaseEntry.Quantity - totalBatchQuantity;

        const buttonSelector = `#btnAddNewItems_${oData.PurchaseTransID}`;
        if (pendingQuantity <= 0) {
            $(buttonSelector).replaceWith(`<button type="button" class="badge bg-success" disabled=""><i class="fas fa-check-circle label-icon me-2"></i>Completed</button>`);
        }
    }

    //$.jGrowl(oData.BatchNo + " successfully added to the list.", { sticky: false, theme: 'success', life: jGrowlLife });

    $("#divModal").modal('hide');
    //DisplayDataTable(MaterialInwardArray);
    SaveandUpdateMaterialInward(oData);
    return false;
}

// To Display the data in the table formate
function DisplayDataTable(MaterialInwardArray) {
    $("#divReport").empty();
    let tableContent = '<div class="table-responsive">';
    if (MaterialInwardArray.length != 0) {
        tableContent += `
        <table class="table table-striped table-sm w-100" id="tblSearchResult">
            <thead>
                <tr class="table-info">
                    <th>S.No</th>
                    <th>Code</th>
                    <th>Item</th>
                    <th>HSN Code</th>
                    <th>Quantity</th>
                    <th>Units</th>
                    <th>Colour</th>
                    <th>Size</th>
                    <th>Batch No</th>
                    <th class='text-end'>Pending Quantity</th>
                    <th class='text-end'>Batch Quantity</th>
                    <th style="text-align:center;">Action</th>
                </tr>
            </thead>
            <tbody>`;

        // Group the array by PurchaseTransID
        const groupedByPurchaseTransID = {};

        // First process all non-deleted entries
        MaterialInwardArray.filter(entry => entry.StatusFlag !== "D").forEach(entry => {
            if (!groupedByPurchaseTransID[entry.PurchaseTransID]) {
                groupedByPurchaseTransID[entry.PurchaseTransID] = [];
            }
            groupedByPurchaseTransID[entry.PurchaseTransID].push(entry);
        });

        // For each PurchaseTransID, calculate and update the pending quantities
        Object.keys(groupedByPurchaseTransID).forEach(purchaseTransID => {
            const entries = groupedByPurchaseTransID[purchaseTransID];
            const purchaseEntry = PurchaseTransList.find(item =>
                item.PurchaseTransID == purchaseTransID
            );

            if (purchaseEntry) {
                let remainingQuantity = purchaseEntry.Quantity;

                // Sort entries by sNo to ensure they're processed in the right order
                entries.sort((a, b) => a.sNo - b.sNo);

                // Update the pending quantity for each entry based on the cumulative batch quantities
                entries.forEach((entry, index) => {
                    remainingQuantity -= parseFloat(entry.BatchQuantity);
                    entry.displayPendingQuantity = remainingQuantity.toFixed(2);
                });
            }
        });

        // Now display all entries with their updated pending quantities
        let rowIndex = 1;
        MaterialInwardArray.filter(entry => entry.StatusFlag !== "D").forEach(entry => {
            const purchaseEntry = PurchaseTransList.find(item =>
                item.PurchaseTransID == entry.PurchaseTransID
            );

            if (purchaseEntry) {
                tableContent += `
                <tr data-sno="${entry.sNo}">
                    <td>${rowIndex++}</td>
                    <td>${purchaseEntry.ItemCode}</td>
                    <td>${purchaseEntry.ItemName}</td>
                    <td>${purchaseEntry.HSNCode}</td>
                    <td>${purchaseEntry.Quantity}</td>
                    <td>${purchaseEntry.UnitName}</td>
                    <td>${purchaseEntry.ColorName}</td>
                    <td>${purchaseEntry.SizeName}</td>
                    <td>${entry.BatchNo}</td>
                    <td class='text-end'>${entry.displayPendingQuantity}</td>
                    <td class='text-end'>${parseFloat(entry.BatchQuantity).toFixed(2)}</td>
                    <td style="text-align:center;">`;

                if (entry.IsAutoBatch == false) {
                    tableContent += `
                    <a href="javascript:void(0);" onclick="Edit_MaterialInward(${entry.sNo},false)"
                        class="btn btn-sm btn-soft-info" data-bs-toggle="modal" data-bs-target="#divModal">
                        <i class="mdi mdi-pencil-outline"></i>
                    </a>`;
                }
                tableContent += `
                <a href="javascript:void(0);" onclick="Delete_MaterialInward(${entry.sNo})"
                    class="btn btn-sm btn-soft-danger" title="Delete">
                    <i class="mdi mdi-delete-outline"></i>
                </a>
                    </td>
                </tr>`;
            }
        });

        tableContent += `
            </tbody>
        </table>
    </div>`;
    }

    $("#divReport").html(tableContent);
}

// To edit the data in the modal
function Edit_MaterialInward(ID) {
    const idToFind = parseInt(ID);
   
    const entry = MaterialInwardArray.find(entry => parseInt(entry.sNo) === idToFind);
    GetPurchaseDetailsbyID(entry.PurchaseTransID);
    if (!entry) {
        return false;
    }

    $("#divModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Batch");

    // Clear previous validation states
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#hdnSNo").val(entry.sNo);
    var data=$("#hdnPendingInwardInspectionID").val(entry.PendingInwardInspectionID);
    $("#hdnPurchaseTransID").val(entry.PurchaseTransID || 0);

    // Calculate remaining quantity excluding this batch
    const purchaseEntry = PurchaseTransList.find(item =>
        item.PurchaseTransID === entry.PurchaseTransID
    );

    if (purchaseEntry) {
        const otherBatches = MaterialInwardArray.filter(otherEntry =>
            otherEntry.PurchaseTransID === entry.PurchaseTransID &&
            otherEntry.StatusFlag !== "D" &&
            otherEntry.sNo !== entry.sNo
        );

        const totalOtherBatchesQuantity = otherBatches.reduce(
            (sum, otherEntry) => sum + parseFloat(otherEntry.BatchQuantity), 0
        );

        // Set the max available quantity for this batch
        const availableQuantity = purchaseEntry.Quantity - totalOtherBatchesQuantity;
        $("#txtMaxAvailableQuantity").val(availableQuantity.toFixed(2));
    }

    $("#txtBatchNo").val(entry.BatchNo || '');
    $("#txtBatchQuantity").val(parseFloat(entry.BatchQuantity).toFixed(2) || '');
    $("#txtLessQuantity").val(parseFloat(entry.LessQuantity).toFixed(2) || '');
    $("#txtExcessQuantity").val(parseFloat(entry.ExcessQuantity).toFixed(2) || '');

    // Show/hide buttons
    $("#btnSaveBatch").hide();
    $("#btnUpdateBatch").show();

    // Focus on first field
    $("#txtBatchNo").focus();

    return false;
}
//To Update the value in the Material Inward Inspection Array
function Update_MaterialInward(oData) {
    const index = MaterialInwardArray.findIndex(item => parseInt(item.sNo) === parseInt(oData.sNo));

    if (index === -1) {

        return false;
    }
    oData.sNo = parseInt(oData.sNo);
    MaterialInwardArray[index] = oData;
    // Update the configuration array
    for (var i = 0; i < MaterialInwardArray.length; i++) {
        if (MaterialInwardArray[i].sNo === oData.sNo) {

            MaterialInwardArray[i].BatchNo = oData.BatchNo;
            MaterialInwardArray[i].BatchQuantity = oData.BatchQuantity;
            break;
        }
    }
    SaveandUpdateMaterialInward(oData);
   // DisplayDataTable(MaterialInwardArray);
    $("#btnSaveBatch").show();
    $("#btnUpdateBatch").hide();
    
   // ClearModuleFormFields();
   // $.jGrowl("Item updated successfully!", { sticky: false, theme: 'success', life: jGrowlLife });
    return false;
}
function Delete_MaterialInward(ID) {
    if (ID == 0) return false;

    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "question",
        showCancelButton: true,
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "No, cancel!",
        customClass: { confirmButton: "btn btn-success mt-2", cancelButton: "btn btn-danger ms-2 mt-2" },
        buttonsStyling: false,
    }).then(function (t) {
        if (t.value) {
            let deletedItem = null;

            for (var i = 0; i < MaterialInwardArray.length; i++) {
                if (MaterialInwardArray[i].sNo == ID) {
                    deletedItem = MaterialInwardArray[i];
                    var index = MaterialInwardArray.findIndex(record => record.sNo === ID);

                    if (MaterialInwardArray[i].PendingInwardInspectionID > 0) {
                        MaterialInwardArray[i].StatusFlag = "D";
                        // MaterialInwardArray.splice(index, 1);
                    } else {
                        MaterialInwardArray.splice(index, 1);
                    }
                    break;
                }
            }

            if (deletedItem) {
                const purchaseEntry = PurchaseTransList.find(item => item.PurchaseTransID === deletedItem.PurchaseTransID);

                if (purchaseEntry) {
                    const remainingBatches = MaterialInwardArray.filter(entry =>
                        entry.PurchaseTransID === deletedItem.PurchaseTransID &&
                        entry.StatusFlag !== "D"
                    );
                    pendingQuantity += deletedItem.BatchQuantity;
                    const totalBatchQuantity = remainingBatches.reduce((sum, entry) =>
                        sum + entry.BatchQuantity, 0
                    );

                    const actionCell = $(`tr:contains('${purchaseEntry.ItemName}')`).find('td:last-child');
                    // const actionCell = $(`tr[data-id="${deletedItem.PurchaseTransID}"] td:last-child`);
                    //const actionCell = $(`tr:contains('${purchaseEntry.ItemName}')`).find('td:last-child');

                    if (totalBatchQuantity < purchaseEntry.Quantity) {
                        actionCell.html(`
                            <button type="button" id="btnAddNewItems_${deletedItem.PurchaseTransID}" class="btn btn-sm btn-warning btn-rounded waves-effect waves-light w-50">
                            <i class="fas fa-plus-square label-icon me-2"></i>Add</button>
                        `);
                    }
                }
            }

            //  DisplayDataTable(MaterialInwardArray);

            //Swal.fire({
            //    title: "Deleted",
            //    text: "Your data deleted successfully!",
            //    icon: "success",
            //    confirmButtonColor: "#556ee6"
            //});
            $.ajax({
                url: DeleteDataUrl,
                type: 'POST',
                headers: { "RequestVerificationToken": _TOKEN },
                contentType: 'application/json',
                data: JSON.stringify(MaterialInwardArray),
                success: function (response) {
                    if (response.Item1.Success && !response.Item1.Error) {
                        Swal.fire({ title: "Deleted!", text: DeleteSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                        GetPurchaseTransDetailsByID($("#hdnPurchaseEntryID").val());
                    }
                    else
                        Swal.fire({ title: "Error", text: DeleteErrorMessage, icon: "warning", confirmButtonColor: "#556ee6" });
                }, error: function (xhr, status, error) {
                    Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
                }
            });

        } else {
            Swal.fire({
                title: "Cancelled",
                text: "Your data is safe :)",
                icon: "error"
            });
        }
    });

    ClearModuleFormFields();
    return false;
}
function SaveandUpdateMaterialInward(MaterialInwardInspection) {
    // Create an array if a single object is passed
    const dataToSend = Array.isArray(MaterialInwardInspection) ?
        MaterialInwardInspection : [MaterialInwardInspection];

    if (ENABLE_VERBOSE_Logging) //console.log(dataToSend);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(dataToSend), // Send as array
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.Item1.Success && !response.Item1.Error) {
                // Note: 'id' is not defined here, you need to fix this
                // Either pass id as a parameter or use a different approach

                Swal.fire({
                    title: "Saved!",
                    text: SaveSuccessMessage,
                    icon: "success",
                    confirmButtonColor: "#556ee6"
                });

                GetPurchaseTransDetailsByID($("#hdnPurchaseEntryID").val());
                //ClearModuleFormFields();
            }
            else if (!response.Item1.Success && response.Item1.Error) {
                Swal.fire({ title: response.Item1.Message, text: "", icon: "warning", confirmButtonColor: "#556ee6" });
            }
            else if (!response.Item1.Success && !response.Item1.Error) {
                Swal.fire({ title: "Error", text: SaveErrorMessage, icon: "error", confirmButtonColor: "#556ee6" });
            }
            else
                Swal.fire({ title: "Error", text: response.message, icon: "error", confirmButtonColor: "#556ee6" });
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });

    return false;
}
//functionality for the proceed button
$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divSearchPage").show();
    $("#divMaterialInward").hide();
    FilterPurchaseEntry();
    //GetPurchasePendingData();
    $("#divPurchasePendingData").show();
    $("#divPurchaseDetails").hide();

    return false;
});
let batchPreferences = {};
function GetBatchInputModal(ID) {
    $("#hdnPurchaseTransID").val(ID);
    const purchaseEntry = PurchaseTransList.find(item => item.PurchaseTransID === ID);

    const existingBatches = MaterialInwardArray.filter(entry =>
        entry.PurchaseTransID === ID &&
        entry.StatusFlag !== "D");

    const hasManualBatches = existingBatches.some(entry => entry.IsAutoBatch === false || entry.IsAutoBatch === 0);

    if (purchaseEntry.ComponentTypeID === AluminiumCoil) {
        openManualBatchEntry(ID);   
    }
    else if (hasManualBatches) {
        openManualBatchEntry(ID);
    }
    
    else if (batchPreferences[ID]) {
        if (batchPreferences[ID] === 'auto') {
    
            generateAutoBatch(purchaseEntry);
        } else {
    
            openManualBatchEntry(ID);
        }
    }
    else {
        Swal.fire({
            title: 'Batch Assignment',
            text: 'Are you sure you want to assign the Batch No automatically?',
            icon: 'question',
            showCancelButton: true,
            confirmButtonText: 'Yes, assign automatically',
            cancelButtonText: 'No, I\'ll enter manually',
            confirmButtonColor: '#556ee6',
            cancelButtonColor: '#74788d'
        }).then((result) => {
            if (result.isConfirmed) {
                batchPreferences[ID] = 'auto';
                generateAutoBatch(purchaseEntry);
            } else if (result.dismiss === Swal.DismissReason.cancel) {
                batchPreferences[ID] = 'manual';
                openManualBatchEntry(ID);
            }
        });
    }
}
function generateAutoBatch(purchaseEntry) {
    $.ajax({
        url: GetNextBatchNumberUrl,
        type: "GET",
        success: function (response) {
            if (response.batchNumber) {
                let MaterialInward = {
                    BatchNo: response.batchNumber,
                    BatchQuantity: purchaseEntry.Quantity,
                    PurchaseTransID: purchaseEntry.PurchaseTransID,
                    sNo: getMaxSNo() + 1,
                    StatusFlag: "I",
                    PendingInwardInspectionID: 0,
                    IsAutoBatch: true
                };

                if (!isDuplicateEntry(MaterialInward.BatchNo, 0)) {
                    Add_MaterialInward(MaterialInward);

                    Swal.fire({
                        title: 'Success',
                        text: `Batch ${response.batchNumber} has been automatically created`,
                        icon: 'success',
                        confirmButtonColor: '#556ee6'
                    });
                } else {
                    Swal.fire({
                        title: 'Error',
                        text: 'Duplicate batch number detected. Please try again.',
                        icon: 'error',
                        confirmButtonColor: '#556ee6'
                    });
                }
            } else {
                Swal.fire("Error", "Failed to generate batch number", "error");
            }
        },
        error: function () {
            Swal.fire("Error", "Failed to generate batch number", "error");
        }
    });
}

function openManualBatchEntry(purchaseTransID) {
    $("#divModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Batch ");
    GetPurchaseDetailsbyID(purchaseTransID);
    $('#btnSaveBatch').show();
    $('#btnUpdateBatch').hide();
    ClearModuleFormFields();
    GenerateBatchNo();
    $("#divModal").modal("show");
}
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

    GetPendingPurchaseEntryByFilter(FilterData);
}
function GetPendingPurchaseEntryByFilter(FilterData) {
    $.ajax({
        url: GetPendingPurchaseEntryByFilterUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(FilterData),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) //console.log(response);
            DisplayPurchasePendingData(response.data);
            $("btnFilter").hide();
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) //console.log(error);
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
function GetPurchaseDetailsbyID(targetPurchaseTransID) {
    const purchaseEntry = PurchaseTransList.find(item => item.PurchaseTransID === targetPurchaseTransID);

    if (!purchaseEntry) {
        return false;
    }

    const existingBatches = MaterialInwardArray.filter(entry =>
        entry.PurchaseTransID === targetPurchaseTransID && entry.StatusFlag !== "D"
    );

    const totalBatchQuantity = existingBatches.reduce((sum, entry) =>
        sum + parseFloat(entry.BatchQuantity), 0);

    const actualPendingQuantity = purchaseEntry.Quantity - totalBatchQuantity;

    $("#divPurchaseDetailsbyID").empty();
    var itemsTable = `
        <div class="table-responsive mt-4">
            <table class="table align-middle">
                <thead class="table-info">
                    <tr>
                        <th>Item</th>
                        <th>HSN Code</th>
                        <th>Quantity</th>
                        <th>Units</th>
                        <th>Size</th>
                        <th>Color</th>
                        <th>Component Type</th>
                        <th>Pending Quantity</th>
                    </tr>
                </thead>
                <tbody>`;

    const filteredData = PurchaseTransList.filter(item => item.PurchaseTransID === targetPurchaseTransID);
    filteredData.forEach((item) => {
        itemsTable += `
            <tr>
                <td>${item.ItemName || 'N/A'}</td>
                <td>${item.HSNCode || 'N/A'}</td>
                <td>${item.Quantity || 'N/A'}</td>
                <td>${item.UnitName || 'N/A'}</td>
                <td>${item.SizeName || 'N/A'}</td>
                <td>${item.ColorName || 'N/A'}</td>
                <td>${item.ComponentTypeName || 'N/A'}</td>
                <td>${actualPendingQuantity}</td>
            </tr>`;
    });
    itemsTable += `
                </tbody>
            </table>
        </div>
        <div class="table-responsive">
            <div id="divReport" class="mt-2"></div>
        </div>`;
    $("#divPurchaseDetailsbyID").append(itemsTable);

}
function GenerateBatchNo() {
    $.ajax({
        url: GetNextBatchNumberUrl,
        type: "GET",
        success: function (response) {
            $("#txtBatchNo").val(response.batchNumber);
        },
        error: function () {
            Swal.fire("Error", "Failed to generate batch number", "error");
        }
    });
}