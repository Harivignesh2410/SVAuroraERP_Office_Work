var gOtherChargesList = [];
var gItemList = [];
var gTaxList = [];
let purchaseEntryArray = [];
function getMaxSNo() {
    if (purchaseEntryArray.length === 0) return 0;
    return Math.max(...purchaseEntryArray.map(item => parseInt(item.sNo) || 0));
}

$(function () {
    pLoadingSetup(false);

    $("#btnSave").show();
    $("#btnUpdate").hide();
    $("#btnAddNew").hide();

    $("#divAddEdit").hide();
    $("#divRecords").show();
    $("#divPurchaseOrderDetails").hide();
    BindOtherChargesList();
    BindItemList();
    BindTaxList();
    //Other Charges
    LoadOtherChargesList("ddlOtherCharges1");
    LoadOtherChargesList("ddlOtherCharges2");
    LoadOtherChargesList("ddlOtherCharges3");

    LoadOtherChargesList("ddlFinalOtherCharges");

    // Item
    LoadItemList("ddlitem");

    //Tax
    LoadTaxList("ddlTax1");
    LoadTaxList("ddlTax2");

    LoadTaxList("ddlFinalTax1");
    LoadTaxList("ddlFinalTax2");
    if (!_CMActionAdd) {
        $("#btnAddNew").remove();
        $("#btnSave").remove();
    }
    if (!_CMActionUpdate) $("#btnUpdate").remove();
    //calendar
    $("#txtBillDate").datetimepicker({
        pickTime: false,
        useCurrent: true,
        format: 'DD/MM/YYYY',
        minYear: 1951,
        maxDate: moment()
    });

    $(".ClearDate").click(function () { var x = $(this).prev().attr('id'); $("#" + x).val(""); });
    $(".SelectDate").click(function () { var x = $(this).next().attr('id'); $("#" + x).focus(); });
    InitializePurchaseEntry();

    if ($.cookie("PurchaseOrderID") != undefined) {

        //$("#hdnPurchaseOrderID").val($.cookie("PurchaseOrderID"));
        //$("#btnAddNew").click();
        //GetPurchaseOrderDetailsByID($.cookie("PurchaseOrderID"));

        let purchaseOrderID = $.cookie("PurchaseOrderID");

        $("#hdnPurchaseOrderID").val(purchaseOrderID);
        $("#btnAddNew").click();

        // Call function with ID
        GetPurchaseOrderDetailsByID(purchaseOrderID);

        $.cookie("PurchaseOrderID", null);
        //$.removeCookie("PurchaseOrderID");
    }
    else
        getRecordList();

    pLoadingSetup(true);
});

function getValidatedFloat(selector) {
    let value = parseFloat($(selector).val());
    return isNaN(value) ? 0 : value;
}
function getValidatedPercentage(selector) {
    let percentage = parseFloat($(selector).find("option:selected").attr("Percentage"));
    return isNaN(percentage) ? 0 : percentage;
}
//for ID
function getValidatedID(selector) {
    let value = parseInt($(selector).val(), 10); // Parse as an integer
    return isNaN(value) || value <= 0 ? 0 : value; // Return 0 if invalid or <= 0
}

$("#btnAddNew").on('click', function () {
    if (!_CMActionAdd) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $("#divAddEdit").show();
    $("#divPurchaseOrderDetails").show();
    $("#divRecords").hide();
    $("#tabDocument").hide();
    $("#btnSave").show();
    $("#btnUpdate").hide();
    ClearFormFields();

    $("#divCardTitle").html("<i class='fas fa-plus-square align-middle me-1'></i>Add New Purchase Entry");

    return false;
});

//Bind Dropdowns
function BindOtherChargesList() {
    $.ajax({
        url: OtherChargesListUrl, // Use the variable defined in Razor page
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        async: false,
        success: function (response) {
            gOtherChargesList = [];

            $.each(response, function (i, result) {
                var OtherCharges = new Object();
                OtherCharges.OtherChargesID = result.OtherChargesID;
                OtherCharges.Type = result.Type;
                OtherCharges.OtherChargesDescription = result.OtherChargesDescription;

                gOtherChargesList.push(OtherCharges);
            });
        }
    });
}
function LoadOtherChargesList(ctrlname) {
    $("#" + ctrlname).empty();
    $("#" + ctrlname).append("<option value='0'>--Select--</option>");

    $.each(gOtherChargesList, function (i, result) {
        $("#" + ctrlname).append("<option value='" + result.OtherChargesID + "' Type='" + result.Type + "'>" + result.OtherChargesDescription + "</option>");
    });
}

// Item DropDown
function BindItemList() {
    $.ajax({
        url: ItemListUrl, // Use the variable defined in Razor page
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        async: false,
        success: function (response) {
            gItemList = [];

            $.each(response, function (i, result) {
                var Item = new Object();
                Item.ItemID = result.ItemID;
                Item.ItemCode = result.ItemCode;
                Item.HSNCode = result.HSNCode;
                Item.ItemName = result.ItemName;
                Item.UnitName = result.UnitName;

                gItemList.push(Item);
            });
        }
    });
}
function LoadItemList(ctrlname) {
    $("#" + ctrlname).empty();
    $("#" + ctrlname).append("<option value='0'>--Select--</option>");

    $.each(gItemList, function (i, result) {
        $("#" + ctrlname).append("<option value='" + result.ItemID + "' ItemCode='" + result.ItemCode + "' HSNCode='" + result.HSNCode + "'>" + result.ItemName + result.UnitName + "</option>");
    });
}

//Tax DropDown
function BindTaxList() {
    $.ajax({
        url: TaxListUrl, // Use the variable defined in Razor page
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        async: false,
        success: function (response) {
            gTaxList = [];

            $.each(response, function (i, result) {
                var Tax = new Object();
                Tax.TaxID = result.TaxID;
                Tax.TaxName = result.TaxName;
                Tax.TaxPercentage = result.TaxPercentage;

                gTaxList.push(Tax);
            });
        }
    });
}
function LoadTaxList(ctrlname) {
    $("#" + ctrlname).empty();
    $("#" + ctrlname).append("<option value='0'>--Select--</option>");

    $.each(gTaxList, function (i, result) {
        $("#" + ctrlname).append("<option value='" + result.TaxID + "' Percentage='" + result.TaxPercentage + "'>" + result.TaxName + "</option>");
    });
}

//Material Value
$("#txtPrice, #txtQuantity").on("input", function () {

    let price = parseFloat($("#txtPrice").val()) || 0; // Default to 0 if empty
    let quantity = parseFloat($("#txtQuantity").val()) || 0; // Default to 0 if empty


    let materialValue = price * quantity;


    $("#txtMaterialValue").val(materialValue.toFixed(2)); // Format to 2 decimal places
});

//calculate total tax
$("#txtTax1Amount, #txtTax2Amount").on("input", function () {

    let TaxAmount1 = parseFloat($("#txtTax1Amount").val()) || 0; // Default to 0 if empty
    let TaxAmount2 = parseFloat($("#txtTax2Amount").val()) || 0; // Default to 0 if empty

    let TotalTax = TaxAmount1 + TaxAmount2;

    $("#txtTotalTax").val(TotalTax.toFixed(2)); // Format to 2 decimal places
});

//Total sub amount
function calculateSubAmount() {
    let TaxAmount1 = parseFloat($("#txtTax1Amount").val()) || 0;
    let TaxAmount2 = parseFloat($("#txtTax2Amount").val()) || 0;
    let TaxableCharges = parseFloat($("#txtTaxableCharges").val()) || 0;
    let TaxAmount = 0;
    TaxAmount = TaxAmount1 + TaxAmount2 + TaxableCharges;
    $("#txtSubAmount").val(TaxAmount.toFixed(2));
}

//Calculating the TableCharge 
function calculateTaxableCharges() {
    // Ensure price and quantity are filled before calculation
    let price = parseFloat($("#txtPrice").val()) || 0;
    let quantity = parseFloat($("#txtQuantity").val()) || 0;

    if (price === 0 || quantity === 0) {
        // Skip calculation if either price or quantity is missing
        return;
    }

    let materialValue = parseFloat($("#txtMaterialValue").val()) || 0;
    let subAmount = materialValue;

    for (let i = 1; i <= 3; i++) {
        let otherChargesAmount = parseFloat($(`#txtOtherCharge${i}`).val()) || 0;
        let otherChargesType = $(`#ddlOtherCharges${i} option:selected`).attr("Type");

        if (otherChargesType === "1") {
            subAmount += otherChargesAmount; // Add if Type = 1
        } else if (otherChargesType === "2") {
            subAmount -= otherChargesAmount; // Subtract if Type = 2
        }
    }

    // Check if subAmount is less than zero
    if (subAmount <= 0) {
        $.jGrowl("Other Charges More than Material Value!!", { sticky: false, theme: 'warning', life: jGrowlLife });
        subAmount = materialValue; // Reset subAmount to zero if it's less than zero
    }

    $("#txtTaxableCharges").val(subAmount.toFixed(2));
    $("#txtSubAmount").val(subAmount.toFixed(2));
}

//calculating the Taxamount
function calculateTransTaxAmount() {

    let taxPercentage1 = parseFloat($("#ddlTax1 option:selected").attr("Percentage")) || 0;
    let taxPercentage2 = parseFloat($("#ddlTax2 option:selected").attr("Percentage")) || 0;

    let taxableCharges = parseFloat($("#txtTaxableCharges").val()) || 0;

    let taxAmount1 = (taxableCharges * taxPercentage1) / 100;
    let taxAmount2 = (taxableCharges * taxPercentage2) / 100;

    $("#txtTax1Amount").val(taxAmount1.toFixed(2)).trigger("input");
    $("#txtTax2Amount").val(taxAmount2.toFixed(2)).trigger("input");

}

$("#btnSaveItem,#btnUpdateItem").on("click", function () {
    let isValid = true;

    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    let PurchaseEntryTrans = new Object();

    // Other Charges
    PurchaseEntryTrans.OtherChargesID1 = $("#ddlOtherCharges1").val() || 0;
    if (PurchaseEntryTrans.OtherChargesID1 > 0) {
        PurchaseEntryTrans.OtherChargesDescription1 = $("#ddlOtherCharges1 option:selected").text();
        PurchaseEntryTrans.OtherChargesType1 = $("#ddlOtherCharges1 option:selected").attr("Type");
    } else {
        PurchaseEntryTrans.OtherChargesDescription1 = "";
        PurchaseEntryTrans.OtherChargesType1 = "";
    }

    PurchaseEntryTrans.OtherChargesID2 = $("#ddlOtherCharges2").val() || 0;
    if (PurchaseEntryTrans.OtherChargesID2 > 0) {
        PurchaseEntryTrans.OtherChargesDescription2 = $("#ddlOtherCharges2 option:selected").text();
        PurchaseEntryTrans.OtherChargesType2 = $("#ddlOtherCharges2 option:selected").attr("Type");
    } else {
        PurchaseEntryTrans.OtherChargesDescription2 = "";
        PurchaseEntryTrans.OtherChargesType2 = "";
    }

    PurchaseEntryTrans.OtherChargesID3 = $("#ddlOtherCharges3").val() || 0;
    if (PurchaseEntryTrans.OtherChargesID3 > 0) {
        PurchaseEntryTrans.OtherChargesDescription3 = $("#ddlOtherCharges3 option:selected").text();
        PurchaseEntryTrans.OtherChargesType3 = $("#ddlOtherCharges3 option:selected").attr("Type");
    } else {
        PurchaseEntryTrans.OtherChargesDescription3 = "";
        PurchaseEntryTrans.OtherChargesType3 = "";
    }

    // Item
    let selectedItemID = $("#ddlitem").val();
    let selectedItem = gItemList.find(item => item.ItemID == selectedItemID);

    if (selectedItem) {
        PurchaseEntryTrans.ItemID = selectedItem.ItemID;
        PurchaseEntryTrans.ItemName = selectedItem.ItemName;
        PurchaseEntryTrans.HSNCode = selectedItem.HSNCode;
        PurchaseEntryTrans.ItemCode = selectedItem.ItemCode;
        PurchaseEntryTrans.UnitName = selectedItem.UnitName;
    } else {
        PurchaseEntryTrans.ItemID = null;
        PurchaseEntryTrans.ItemName = null;
        PurchaseEntryTrans.HSNCode = null;
        PurchaseEntryTrans.ItemCode = null;
        PurchaseEntryTrans.UnitName = null;
    }

    // Tax
    PurchaseEntryTrans.TaxID1 = $("#ddlTax1").val() || 0;
    if (PurchaseEntryTrans.TaxID1 > 0) {
        PurchaseEntryTrans.TaxName1 = $("#ddlTax1 option:selected").text();
        PurchaseEntryTrans.TaxPercentage1 = $("#ddlTax1 option:selected").attr("Percentage");
    } else {
        PurchaseEntryTrans.TaxName1 = "";
        PurchaseEntryTrans.TaxPercentage1 = "0";
    }

    PurchaseEntryTrans.TaxID2 = $("#ddlTax2").val() || 0;
    if (PurchaseEntryTrans.TaxID2 > 0) {
        PurchaseEntryTrans.TaxName2 = $("#ddlTax2 option:selected").text();
        PurchaseEntryTrans.TaxPercentage2 = $("#ddlTax2 option:selected").attr("Percentage");
    } else {
        PurchaseEntryTrans.TaxName2 = "";
        PurchaseEntryTrans.TaxPercentage2 = "0";
    }

    // Other values with proper type conversion
    PurchaseEntryTrans.Pcs = getValidatedFloat("#txtNos");
    PurchaseEntryTrans.Rate = getValidatedFloat("#txtPrice");
    PurchaseEntryTrans.Quantity = getValidatedFloat("#txtQuantity");
    PurchaseEntryTrans.MaterialValue = getValidatedFloat("#txtMaterialValue");

    PurchaseEntryTrans.OtherChargesIDAmount1 = getValidatedFloat("#txtOtherCharge1");
    PurchaseEntryTrans.OtherChargesIDAmount2 = getValidatedFloat("#txtOtherCharge2");
    PurchaseEntryTrans.OtherChargesIDAmount3 = getValidatedFloat("#txtOtherCharge3");
    PurchaseEntryTrans.OtherChargesAmount = (
        (PurchaseEntryTrans.OtherChargesIDAmount1 || 0) +
        (PurchaseEntryTrans.OtherChargesIDAmount2 || 0) +
        (PurchaseEntryTrans.OtherChargesIDAmount3 || 0)
    ).toFixed(2);

    PurchaseEntryTrans.TaxableChargesAmount = getValidatedFloat("#txtTaxableCharges");
    PurchaseEntryTrans.TaxAmount1 = getValidatedFloat("#txtTax1Amount");
    PurchaseEntryTrans.TaxAmount2 = getValidatedFloat("#txtTax2Amount");
    PurchaseEntryTrans.TaxAmount = getValidatedFloat("#txtTotalTax");
    PurchaseEntryTrans.SubTotal = getValidatedFloat("#txtSubAmount");

    // Validation
    if (!PurchaseEntryTrans.ItemID) {
        $('#ddlitem').addClass('is-invalid');
        $('#ddlitem').after('<div class="invalid-feedback">Please Select the Item</div>');
        $('#ddlitem').focus();
        isValid = false;
        return false;
    }

    if (!PurchaseEntryTrans.Quantity) {
        $('#txtQuantity').addClass('is-invalid');
        $('#txtQuantity').after('<div class="invalid-feedback">Please Enter the Quantity</div>');
        $('#txtQuantity').focus();
        isValid = false;
        return false;
    }

    //if (!PurchaseEntryTrans.Pcs) {
    //    $('#txtNos').addClass('is-invalid');
    //    $('#txtNos').after('<div class="invalid-feedback">Please Enter the NoS</div>');
    //    $('#txtNos').focus();
    //    isValid = false;
    //    return false;
    //}

    if (!PurchaseEntryTrans.Rate) {
        $('#txtPrice').addClass('is-invalid');
        $('#txtPrice').after('<div class="invalid-feedback">Please Enter the Price</div>');
        $('#txtPrice').focus();
        isValid = false;
        return false;
    }

    if (isValid) {
        const purchaseTransID = parseInt($("#hdnPurchaseTransID").val()) || 0;

        if (this.id === "btnSaveItem") {
            // For new entries
            const maxSNo = getMaxSNo();
            PurchaseEntryTrans.sNo = maxSNo + 1;
            PurchaseEntryTrans.StatusFlag = "I"; // Insert
            PurchaseEntryTrans.PurchaseTransID = 0;



            if (!isDuplicateEntry(PurchaseEntryTrans.ItemID, 0)) {
                Add_PurchaseTrans(PurchaseEntryTrans);

            } else {
                $.jGrowl("The entered item already exists!", { sticky: false, theme: 'warning', life: 3000 });
                return false;
            }
        } else if (this.id === "btnUpdateItem") {
            // For updates
            const currentSNo = parseInt($("#hdnSNo").val());
            PurchaseEntryTrans.sNo = currentSNo;

            if (purchaseTransID > 0) {
                PurchaseEntryTrans.StatusFlag = "U"; // Update
                PurchaseEntryTrans.PurchaseTransID = purchaseTransID;
            } else {
                PurchaseEntryTrans.StatusFlag = "I"; // Insert
                PurchaseEntryTrans.PurchaseTransID = 0;
            }

            if (!isDuplicateEntry(PurchaseEntryTrans.ItemID, PurchaseEntryTrans.sNo)) {
                Update_PurchaseTrans(PurchaseEntryTrans);
            } else {
                $.jGrowl(PurchaseEntryTrans.ItemName + " is already added to the list.", {
                    sticky: false,
                    theme: 'warning',
                    life: jGrowlLife
                });
                return false;
            }
        }

        if (this.id === "btnSaveItem") {
            ClearModuleFormFields();
        } else {
            $("#divAddUnitModal").modal('hide');
        }
    }
});

//find the duplicate entry of the data
function isDuplicateEntry(itemID, sNo) {
    for (let i = 0; i < purchaseEntryArray.length; i++) {

        if (purchaseEntryArray[i].ItemID === itemID && purchaseEntryArray[i].sNo !== sNo) {
            return true;
        }
    }
    return false;
}
function Add_PurchaseTrans(oData) {
    purchaseEntryArray.push(oData);
    // Change this message
    $.jGrowl(oData.ItemName + " successfully added to the list.", { sticky: false, theme: 'success', life: jGrowlLife });
    DisplayDataTable(purchaseEntryArray);
    return false;
}
function Edit_PurchaseTrans(ID) {
    if (!ID) {
        return false;
    }

    // Convert ID to number for consistent comparison
    const idToFind = parseInt(ID);
    const entry = purchaseEntryArray.find(entry => parseInt(entry.sNo) === idToFind);

    if (!entry) {
        return false;
    }
    $("#divAddUnitModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Item");
    $("#divAddUnitModal").show();
    // Clear previous validation states
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#hdnSNo").val(entry.sNo);
    $("#hdnPurchaseTransID").val(entry.PurchaseTransID || 0);

    // Set item dropdown and related fields
    $("#ddlitem").val(entry.ItemID).change();

    // Set basic fields with null checks
    $("#txtQuantity").val(entry.Quantity || '');
    $("#txtNos").val(entry.Pcs || '');
    $("#txtPrice").val(entry.Rate || '');
    $("#txtMaterialValue").val(entry.MaterialValue || '');

    // Set Other Charges with null checks
    $("#ddlOtherCharges1").val(entry.OtherChargesID1 || 0).change();
    $("#txtOtherCharge1").val(entry.OtherChargesIDAmount1 || '');
    $("#ddlOtherCharges2").val(entry.OtherChargesID2 || 0).change();
    $("#txtOtherCharge2").val(entry.OtherChargesIDAmount2 || '');
    $("#ddlOtherCharges3").val(entry.OtherChargesID3 || 0).change();
    $("#txtOtherCharge3").val(entry.OtherChargesIDAmount3 || '');

    // Set Tax fields with null checks
    $("#txtTaxableCharges").val(entry.TaxableChargesAmount || '');
    $("#ddlTax1").val(entry.TaxID1 || 0).change();
    $("#txtTax1Amount").val(entry.TaxAmount1 || '');
    $("#ddlTax2").val(entry.TaxID2 || 0).change();
    $("#txtTax2Amount").val(entry.TaxAmount2 || '');
    $("#txtTotalTax").val(entry.TaxAmount || '');
    $("#txtSubAmount").val(entry.SubTotal || '');

    // Show/hide buttons
    $("#btnSaveItem").hide();
    $("#btnUpdateItem").show();

    // Focus on first field
    $("#ddlitem").focus();

    return false;
}
//Update the field in the table
function Update_PurchaseTrans(oData) {
    const index = purchaseEntryArray.findIndex(item => parseInt(item.sNo) === parseInt(oData.sNo));

    if (index === -1) {
        return false;
    }
    oData.sNo = parseInt(oData.sNo);
    purchaseEntryArray[index] = oData;
    // Update the configuration array
    for (var i = 0; i < purchaseEntryArray.length; i++) {
        if (purchaseEntryArray[i].sNo === oData.sNo) {
            purchaseEntryArray[i].ItemName = oData.ItemName;
            purchaseEntryArray[i].HSNCode = oData.HSNCode;
            purchaseEntryArray[i].UnitName = oData.UnitName;
            purchaseEntryArray[i].Quantity = oData.Quantity;
            purchaseEntryArray[i].Pcs = oData.Pcs;
            purchaseEntryArray[i].Rate = oData.Rate;
            purchaseEntryArray[i].MaterialValue = oData.MaterialValue;
            purchaseEntryArray[i].OtherChargesDescription1 = oData.OtherChargesDescription1;
            purchaseEntryArray[i].OtherChargesIDAmount1 = oData.OtherChargesIDAmount1;
            purchaseEntryArray[i].OtherChargesDescription2 = oData.OtherChargesDescription2;
            purchaseEntryArray[i].OtherChargesIDAmount2 = oData.OtherChargesIDAmount2;
            purchaseEntryArray[i].OtherChargesDescription3 = oData.OtherChargesDescription3;
            purchaseEntryArray[i].OtherChargesIDAmount3 = oData.OtherChargesIDAmount3;
            purchaseEntryArray[i].TaxableChargesAmount = oData.TaxableChargesAmount;
            purchaseEntryArray[i].TaxName1 = oData.TaxName1;
            purchaseEntryArray[i].TaxAmount1 = oData.TaxAmount1;
            purchaseEntryArray[i].TaxName2 = oData.TaxName2;
            purchaseEntryArray[i].TaxAmount2 = oData.TaxAmount2;
            purchaseEntryArray[i].TaxAmount = oData.TaxAmount;
            purchaseEntryArray[i].SubTotal = oData.SubTotal;
            purchaseEntryArray[i].StatusFlag = oData.StatusFlag;
            break;
        }
    }
    DisplayDataTable(purchaseEntryArray);
    $("#btnSaveItem").show();
    $("#btnUpdateItem").hide();
    ClearModuleFormFields();
    $.jGrowl("Item updated successfully!", { sticky: false, theme: 'success', life: jGrowlLife });
    return false;
}
function DisplayDataTable(purchaseEntryArray) {
    let tableContent = `
        <table id="purchaseTable" class="table  table-hover text-center align-middle">
            <thead>
                <tr class="table-light">
                    <th>S.No</th>
                    <th>Item</th>
                    <th>HSN</th>
                    <th>Units</th>
                    <th>Quantity</th>
                    <th>Pcs</th>
                    <th>Rate</th>
                    <th>Material Value (Rs.)</th>
                    <th>Other Charges 1</th>
                    <th>Amount (Rs.)</th>
                    <th>Other Charges 2</th>
                    <th>Amount (Rs.)</th>
                    <th>Other Charges 3</th>
                    <th>Amount (Rs.)</th>
                    <th>Taxable Charges</th>
                    <th>Total Tax</th>
                    <th>Sub Total (Rs.)</th>
                    <th>Actions</th>
                </tr>
            </thead>
            <tbody>`;

    purchaseEntryArray.forEach((entry, index) => {
        if (entry.StatusFlag != "D") {
            tableContent += `
   <tr data-sno="${entry.sNo}">
        <td>${index + 1}</td>
        <td>${entry.ItemName || ""}</td>
        <td>${entry.HSNCode || ""}</td>
        <td>${entry.UnitName || ""}</td>
        <td>${entry.Quantity || ""}</td>
        <td>${entry.Pcs || ""}</td>
        <td class="text-end">${entry.Rate || ""}</td>
        <td class="text-end">${entry.MaterialValue || ""}</td>
        <td>${entry.OtherChargesDescription1 || ""}</td>
        <td class="text-end">${entry.OtherChargesIDAmount1 || ""}</td>
        <td>${entry.OtherChargesDescription2 || ""}</td>
        <td class="text-end">${entry.OtherChargesIDAmount2 || ""}</td>
        <td>${entry.OtherChargesDescription3 || ""}</td>
        <td class="text-end">${entry.OtherChargesIDAmount3 || ""}</td>
        <td class="text-end">${entry.TaxableChargesAmount || ""}</td>
        <td class="text-end">${entry.TaxAmount || ""}</td>
        <td class="text-end">${entry.SubTotal || ""}</td>
        <td style="text-align:center;">
           <a href="javascript:void(0);" onclick="Edit_PurchaseTrans(${entry.sNo})" class="btn btn-sm btn-soft-info" title="Edit" data-bs-toggle="modal" data-bs-target="#divAddUnitModal">
                <i class="mdi mdi-pencil-outline"></i>
            </a>
            <a href="javascript:void(0);" onclick="Delete_PurchaseTrans(${entry.sNo})" class="btn btn-sm btn-soft-danger" title="Delete">
                <i class="mdi mdi-delete-outline"></i>
            </a>
        </td>
    </tr>`;

        }
    });

    tableContent += `
            </tbody>
        </table>`;

    // Update the divTableData container
    $("#divTableData").empty();
    $("#divTableData").html(tableContent);
    calculateGrossAmount();
    calculateTaxAmount();
}

$("#btnAddNewItems").on("click", function () {
    $("#divAddUnitModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Item");
    ClearModuleFormFields();

    return false;
});
function ClearModuleFormFields() {
    $("#divAddUnitModal .modal-body :input").attr("disabled", false);
    $("#hdnSNo").val("0");
    $("#hdnPurchaseTransID").val("0");
    $("#hdnItemID").val(0);

    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#ddlitem").val("0").change();
    $("#txtQuantity").val("");
    $("#txtNos").val("");
    $("#txtPrice").val("");
    $("#txtMaterialValue").val("");
    $("#ddlOtherCharges1").val("0").change();
    $("#txtOtherCharge1").val("");
    $("#ddlOtherCharges2").val("0").change();
    $("#txtOtherCharge2").val("");
    $("#ddlOtherCharges3").val("0").change();
    $("#txtOtherCharge3").val("");
    $("#txtTaxableCharges").val("");
    $("#ddlTax1").val("0").change();
    $("#txtTax1Amount").val("");
    $("#ddlTax2").val("0").change();
    $("#txtTax2Amount").val("");
    $("#txtTotalTax").val("");
    $("#txtSubAmount").val("");
    $("#chkActive").prop("checked", true);
    $("#chkStock").prop("checked", true);

    $("#btnSaveItem").show();
    $("#btnUpdateItem").hide();

    return false;
}
function Delete_PurchaseTrans(ID) {
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
            for (var i = 0; i < purchaseEntryArray.length; i++) {
                if (purchaseEntryArray[i].sNo == ID) {
                    var index = purchaseEntryArray.findIndex(record => record.sNo === ID);
                    if (purchaseEntryArray[i].PurchaseTransID > 0)
                        purchaseEntryArray[i].StatusFlag = "D";
                    else
                        purchaseEntryArray.splice(index, 1);
                }
            }
            Swal.fire({
                title: "Deleted",
                text: "Your data deleted successfully!",
                icon: "success",
                confirmButtonColor: "#556ee6"
            });
            DisplayDataTable(purchaseEntryArray);
        } else {
            Swal.fire({
                title: "Cancelled",
                text: "Your data is safe :)",
                icon: "error"
            });
        }
    });

    ClearModuleFormFields();
    $("#ddlitem").focus();
    return false;
}

$("#txtPrice, #txtQuantity").on("input", function () {
    calculateTaxableCharges();
});

$("#txtOtherCharge1, #txtOtherCharge2, #txtOtherCharge3").on("input", function () {
    calculateTaxableCharges();
});

$("#ddlOtherCharges1, #ddlOtherCharges2, #ddlOtherCharges3").on("change", function () {
    calculateTaxableCharges();
});

$("#ddlTax1, #ddlTax2").on("change", function () {
    calculateTransTaxAmount();
});

// Recalculate Tax Amount if Taxable Charges change
$("#txtTaxableCharges").on("input", function () {
    calculateTransTaxAmount();
});

$("#txtTax1Amount, #txtTax2Amount, #txtTaxableCharges").on("input", function () {
    calculateSubAmount();
});
$("#txtTax1Amount, #txtTax2Amount").on("input", function () {
    calculateSubAmount();
});

//-------------------------------------------------------------------------------------------------------------------------------
//close button
$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divRecords").show();
    $("#divAddEdit").hide();

    getRecordList();
    return false;
});
//clearn form fields
function ClearFormFields() {
    $("#divAddEdit .card-body :input").attr("disabled", false);
    $("#hdnPurchaseEntryID").val(0);
    $("#txtBillNo").val("");
    $("#txtBillDate").val("");
    $("#ddlSupplierType").val("0").change();


    $("#txtRoundOff1").val("");
    $("#txtRoundOff2").val("");
    $("#ddlFinalOtherCharges").val("0").change();
    $("#txtfinalOthercharge").val("");
    $("#txtGrossAmount").val("");
    $("#ddlFinalTax1").val("0").change();
    $("#txtFinalTaxAmount1").val("");

    $("#ddlFinalTax2").val("0").change();
    $("#txtFinalTaxAmount2").val("");
    $("#txtfinalTotalTax").val("");
    $("#txtNetBillAmount").val("");
    $("#txtNarration").val("");
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

    ClearModuleFormFields();
    purchaseEntryArray = []; divTableData
    $("#divTableData").empty();

    return false;
}

//Refresh button
$('#btnRefresh').on('click', function () {
    getRecordList();
    return false;
});
$("#txtRoundOff1, #txtRoundOff2, #txtfinalOthercharge").on("input", function () {
    calculateGrossAmount();
    calculateTaxAmount();
});
$("#ddlFinalOtherCharges").on("change", function () {
    calculateGrossAmount();
    calculateTaxAmount();
});


function calculateGrossAmount() {
    let totalSubAmount = 0;

    if (purchaseEntryArray && purchaseEntryArray.length > 0) {
        totalSubAmount = purchaseEntryArray.reduce((sum, item) => {
            const itemSubTotal = parseFloat(item.SubTotal) || 0;
            return sum + itemSubTotal;
        }, 0);
    }

    $("#txtSubAmount").val(totalSubAmount.toFixed(2));

    let roundOff1 = parseFloat($("#txtRoundOff1").val()) || 0;
    let roundOff2 = parseFloat($("#txtRoundOff2").val()) || 0;


    let grossAmount = totalSubAmount;


    if (roundOff1 > 0) {
        grossAmount += roundOff1;
    }
    if (roundOff2 > 0) {
        grossAmount -= roundOff2;
    }

    const finalOtherCharges = parseFloat($("#txtfinalOthercharge").val()) || 0;
    const otherChargesType = $("#ddlFinalOtherCharges option:selected").attr("Type");

    if (finalOtherCharges > 0) {
        if (otherChargesType === "1") {
            grossAmount += finalOtherCharges;  // Add if Type = 1
        } else if (otherChargesType === "2") {
            grossAmount -= finalOtherCharges;  // Subtract if Type = 2
        }
    }

    //if (grossAmount <= 0) {
    //    $.jGrowl("Other Charges More than Material Value!!", { sticky: false, theme: 'warning', life: jGrowlLife });

    //}

    if (grossAmount > 0) {
        $("#txtGrossAmount").val(grossAmount.toFixed(2));
        $("#txtNetBillAmount").val(grossAmount.toFixed(2));
    }
    return grossAmount;
}
$("#txtRoundOff1, #txtRoundOff2").on("input", calculateGrossAmount);
$("#txtfinalOthercharge").on("input", calculateGrossAmount);
$("#ddlFinalOtherCharges").on("change", calculateGrossAmount);

$(document).on("purchaseEntryArrayUpdated", calculateGrossAmount);

//calculate taxamount for field
function calculateTaxAmount() {

    let taxPercentage1 = parseFloat($("#ddlFinalTax1 option:selected").attr("Percentage")) || 0;
    let taxPercentage2 = parseFloat($("#ddlFinalTax2 option:selected").attr("Percentage")) || 0;

    let GrossAmount = parseFloat($("#txtGrossAmount").val()) || 0;

    let taxAmount1 = (GrossAmount * taxPercentage1) / 100;
    let taxAmount2 = (GrossAmount * taxPercentage2) / 100;

    $("#txtFinalTaxAmount1").val(taxAmount1.toFixed(2)).trigger("input");
    $("#txtFinalTaxAmount2").val(taxAmount2.toFixed(2)).trigger("input");

}
$("#ddlFinalTax1, #ddlFinalTax2").on("change", function () {
    calculateTaxAmount();
});
$("#txtGrossAmount").on("input", function () {
    calculateTaxAmount();
});

//calculate total tax amount
$("#txtFinalTaxAmount1, #txtFinalTaxAmount2").on("input", function () {

    let TaxAmount1 = parseFloat($("#txtFinalTaxAmount1").val()) || 0; // Default to 0 if empty
    let TaxAmount2 = parseFloat($("#txtFinalTaxAmount2").val()) || 0; // Default to 0 if empty

    let TotalTax = TaxAmount1 + TaxAmount2;

    $("#txtfinalTotalTax").val(TotalTax.toFixed(2)); // Format to 2 decimal places
});

function calculateNetBillAmount() {
    let TaxAmount1 = parseFloat($("#txtFinalTaxAmount1").val()) || 0;
    let TaxAmount2 = parseFloat($("#txtFinalTaxAmount2").val()) || 0;
    let GrossAmount = parseFloat($("#txtGrossAmount").val()) || 0;
    let NetBill = 0;
    NetBill = TaxAmount1 + TaxAmount2 + GrossAmount;
    $("#txtNetBillAmount").val(NetBill.toFixed(2));
}
$("#txtFinalTaxAmount1, #txtFinalTaxAmount2, #txtGrossAmount").on("input", function () {
    calculateNetBillAmount();
});
$("#txtFinalTaxAmount1, #txtFinalTaxAmount2").on("input", function () {
    calculateNetBillAmount();
});

//save the Purchase Entry data
$("#btnSave,#btnUpdate").on('click', function () {
    if (this.id == "btnSave") {
        if (!_CMActionAdd) {
            $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
            return false;
        }
    }
    else if (this.id == "btnUpdate") {
        if (!_CMActionUpdate) {
            $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
            return false;
        }
    }
    let isValid = true;
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    var PurchaseEntry = new Object();

    // Set ID and StatusFlag
    PurchaseEntry.PurchaseEntryID = 0;
    if (this.id == "btnUpdate" && $("#hdnPurchaseEntryID").val() > 0) {
        PurchaseEntry.PurchaseEntryID = $("#hdnPurchaseEntryID").val();
    }
    PurchaseEntry.StatusFlag = (PurchaseEntry.PurchaseEntryID === 0) ? "I" : "U";

    // Basic Details
    PurchaseEntry.PurchaseInvoiceNo = $('#txtBillNo').val();
    PurchaseEntry.sPurchaseInvoiceDate = $('#txtBillDate').val();
    PurchaseEntry.SupplierID = getValidatedID('#ddlSupplierType');
    PurchaseEntry.GrossAmount = getValidatedFloat("#txtGrossAmount");
    PurchaseEntry.RoundedOffPlus = getValidatedFloat("#txtRoundOff1");
    PurchaseEntry.RoundedOffMinus = getValidatedFloat("#txtRoundOff2");
    PurchaseEntry.OtherChargesID = getValidatedID('#ddlFinalOtherCharges');
    PurchaseEntry.OtherChargesAmount = getValidatedFloat("#txtfinalOthercharge");

    // Tax Details
    PurchaseEntry.TaxID1 = getValidatedID('#ddlFinalTax1');
    PurchaseEntry.TaxPercentage1 = getValidatedPercentage("#ddlFinalTax1");
    PurchaseEntry.TaxAmount1 = getValidatedFloat("#txtFinalTaxAmount1");
    PurchaseEntry.TaxID2 = getValidatedID('#ddlFinalTax2');
    PurchaseEntry.TaxPercentage2 = getValidatedPercentage("#ddlFinalTax2");
    PurchaseEntry.TaxAmount2 = getValidatedFloat("#txtFinalTaxAmount2");
    PurchaseEntry.TaxAmount = getValidatedFloat("#txtfinalTotalTax");
    PurchaseEntry.PurchaseInvoiceAmount = getValidatedFloat("#txtNetBillAmount");
    PurchaseEntry.Narration = $('#txtNarration').val();
    PurchaseEntry.PurchaseOrderID = $('#hdnPurchaseOrderID').val();

    // Transactions
    PurchaseEntry.PurchaseEntryTransList = purchaseEntryArray;

    // Validations
    if (!PurchaseEntry.PurchaseInvoiceNo) {
        $('#txtBillNo').addClass('is-invalid');
        $('#txtBillNo').after('<div class="invalid-feedback">Please enter Bill No</div>');
        $('#txtBillNo').focus();
        return false;
    }

    if (!PurchaseEntry.sPurchaseInvoiceDate) {
        $('#txtBillDate').addClass('is-invalid');
        $('#txtBillDate').after('<div class="invalid-feedback">Please select Date</div>');
        $('#txtBillDate').focus();
        return false;
    }

    if (!PurchaseEntry.SupplierID) {
        $('#ddlSupplierType').addClass('is-invalid');
        $('#ddlSupplierType').after('<div class="invalid-feedback">Please select Supplier</div>');
        $('#ddlSupplierType').focus();
        return false;
    }
    var count = 0;
    purchaseEntryArray.forEach(function (purchase) {
        if (purchase.StatusFlag != 'D') {
            count++;
        }
    });
    if (count == 0) {
        $.jGrowl("Kindly Enter Atlest one Item", { sticky: false, theme: 'warning', life: 3000 });
        return false;
    }

    if (isValid) {
        SaveandUpdatePurchaseEntry(PurchaseEntry);
    }

    return false;
});
function SaveandUpdatePurchaseEntry(PurchaseEntry) {
    if (ENABLE_VERBOSE_Logging) //console.log(PurchaseEntry);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(PurchaseEntry),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.success && !response.isExists) {
                if (PurchaseEntry.PurchaseEntryID == 0) {
                    $.jGrowl("Purchase Entry Saved Successfully", { sticky: false, theme: 'success', life: 3000 });
                    Swal.fire({ title: "Document Tab Enable", text: " Now you Can Upload your Documents", icon: "success", confirmButtonColor: "#556ee6" });

                }
                else if (PurchaseEntry.PurchaseEntryID > 0)
                    Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                EditData(response.ID, false);
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
                orderable: false, // Disable sorting for this column},
                "width": "5%",
                "orderable": false
            },
            { "data": "PurchaseInvoiceNo", "orderable": true, "width": "5%" },
            { "data": "sPurchaseInvoiceDate", "orderable": true, "width": "5%" },
            { "data": "SupplierName", "orderable": true, "width": "10%" },
            {
                "data": "TotalPcs", "orderable": true, "width": "10%", "className": "text-end",
                "render": function (data, type, row) {
                    return `${row.TotalPcs.toFixed(2)}`
                },
            },
            {
                "data": "TotalQuantity", "orderable": true, "width": "10%", "className": "text-end",
                "render": function (data, type, row) {
                    return `${row.TotalQuantity.toFixed(2)}`
                },
            },
            {
                "data": "TotalItemTax", "orderable": true, "width": "10%", "className": "text-end",
                "render": function (data, type, row) {
                    return `${row.TotalItemTax.toFixed(2)}`
                },
            },
            {
                "data": "GrossAmount", "orderable": true, "width": "10%", "className": "text-end",
                "render": function (data, type, row) {
                    return `<i class="bx bx-rupee text-primary font-size-16"></i>${row.GrossAmount.toFixed(2)}`
                },
            },
            {
                "data": "TaxAmount", "orderable": true, "width": "10%", "className": "text-end",
                "render": function (data, type, row) {
                    return `<i class="bx bx-rupee text-primary font-size-16"></i>${row.TaxAmount.toFixed(2)}`
                },
            },
            {
                "data": "PurchaseInvoiceAmount", "orderable": true, "width": "10%", "className": "text-end",
                "render": function (data, type, row) {
                    return `<i class="bx bx-rupee text-primary font-size-16"></i>${row.PurchaseInvoiceAmount.toFixed(2)}`
                },
            },
            {
                "data": "PurchaseStatus", "orderable": true, "width": "10%", "className": "text-center",
                "render": function (data, type, row) {
                    return `<span class="${row.ColorCode}">${row.PurchaseStatus}</span>`
                },
            },
            {
                data: null,
                bSortable: false,
                "className": "text-center",
                render: function (data, type, row) {
                    return SetActionButtons(row.PurchaseEntryID, _CMPermissions);

                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}
//Edit data
function EditData(ID, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) //console.log(ID);
    ClearModuleFormFields();
    if ((!_CMActionView && ViewFlag) || (!_CMActionUpdate && !ViewFlag)) {
        $.jGrowl(_CMAccessDeined, { sticky: false, theme: 'danger', life: jGrowlLife });
        return false;
    }
    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            if (ViewFlag) {
                $("#divAddEdit .card-body :input").attr("disabled", true);
                $("#divCardTitle").html("<i class='fas fa-eye align-middle me-1'></i>View Purchase Entry");
                $("#btnSave").hide();
                $("#btnUpdate").hide();
                $("#btnSaveAndnew").hide();

                $("#btnCloseWindow,#btnClose").attr("disabled", false);
            }
            else {
                $("#divCardTitle").html("<i class='fas fa-edit  me-1'></i>Edit Purchase Entry");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }

            $("#divAddEdit").show();
            $("#divRecords").hide();
            $("#tabDocument").show();

            var PurchaseEntry = response.data;

            GetPurchaseOrderDetailsByID(PurchaseEntry.PurchaseOrderID);
            $("#hdnPurchaseEntryID").val(PurchaseEntry.PurchaseEntryID);
            $("#txtBillNo").val(PurchaseEntry.PurchaseInvoiceNo);
            $("#txtBillDate").val(PurchaseEntry.sPurchaseInvoiceDate);

            //$("#txtBillDate").val(PurchaseEntry.PurchaseInvoiceDate);
            $("#ddlSupplierType").val(PurchaseEntry.SupplierID).change();
            $("#txtRoundOff1").val(PurchaseEntry.RoundedOffPlus);
            $("#txtRoundOff2").val(PurchaseEntry.RoundedOffMinus);
            $("#ddlFinalOtherCharges").val(PurchaseEntry.OtherChargesID).change();
            $("#txtfinalOthercharge").val(PurchaseEntry.OtherChargesAmount);
            $("#txtGrossAmount").val(PurchaseEntry.GrossAmount);
            $("#ddlFinalTax1").val(PurchaseEntry.TaxID1).change();
            $("#txtFinalTaxAmount1").val(PurchaseEntry.TaxAmount1);
            $("#ddlFinalTax2").val(PurchaseEntry.TaxID2).change();
            $("#txtFinalTaxAmount2").val(PurchaseEntry.TaxAmount2);
            $("#txtfinalTotalTax").val(PurchaseEntry.TaxAmount);
            $("#txtNetBillAmount").val(PurchaseEntry.PurchaseInvoiceAmount);
            $("#txtNarration").val(PurchaseEntry.Narration);

            purchaseEntryArray = [];

            PurchaseEntry.PurchaseEntryTransList.forEach((purchaseItem, index) => {
                var objTemp = new Object();

                // Basic fields
                objTemp.SNo = index + 1;
                objTemp.sNo = objTemp.SNo;
                objTemp.PurchaseTransID = purchaseItem.PurchaseTransID;
                objTemp.ItemID = purchaseItem.ItemID;
                objTemp.ItemCode = purchaseItem.ItemCode;
                objTemp.ItemName = purchaseItem.ItemName;
                objTemp.HSNCode = purchaseItem.HSNCode;
                objTemp.UnitName = purchaseItem.UnitName;
                objTemp.Quantity = purchaseItem.Quantity;
                objTemp.Rate = purchaseItem.Rate;
                objTemp.SubTotal = purchaseItem.SubTotal;
                objTemp.Pcs = purchaseItem.Pcs;
                objTemp.Rate = purchaseItem.Rate;
                objTemp.MaterialValue = purchaseItem.MaterialValue;

                // Other Charges 1
                objTemp.OtherChargesID1 = purchaseItem.OtherChargesID1 || 0;
                objTemp.OtherChargesDescription1 = purchaseItem.OtherChargesDescription1 || "";
                objTemp.OtherChargesType1 = purchaseItem.OtherChargesType1 || "";
                objTemp.OtherChargesIDAmount1 = purchaseItem.OtherChargesIDAmount1 || 0;

                // Other Charges 2
                objTemp.OtherChargesID2 = purchaseItem.OtherChargesID2 || 0;
                objTemp.OtherChargesDescription2 = purchaseItem.OtherChargesDescription2 || "";
                objTemp.OtherChargesType2 = purchaseItem.OtherChargesType2 || "";
                objTemp.OtherChargesIDAmount2 = purchaseItem.OtherChargesIDAmount2 || 0;

                // Other Charges 3
                objTemp.OtherChargesID3 = purchaseItem.OtherChargesID3 || 0;
                objTemp.OtherChargesDescription3 = purchaseItem.OtherChargesDescription3 || "";
                objTemp.OtherChargesType3 = purchaseItem.OtherChargesType3 || "";
                objTemp.OtherChargesIDAmount3 = purchaseItem.OtherChargesIDAmount3 || 0;

                // Tax 1
                objTemp.TaxID1 = purchaseItem.TaxID1 || 0;
                objTemp.TaxName1 = purchaseItem.TaxName1 || "";
                objTemp.TaxPercentage1 = purchaseItem.TaxPercentage1 || "0";
                objTemp.TaxAmount1 = purchaseItem.TaxAmount1 || 0;

                // Tax 2
                objTemp.TaxID2 = purchaseItem.TaxID2 || 0;
                objTemp.TaxName2 = purchaseItem.TaxName2 || "";
                objTemp.TaxPercentage2 = purchaseItem.TaxPercentage2 || "0";
                objTemp.TaxAmount2 = purchaseItem.TaxAmount2 || 0;

                // Calculated fields
                objTemp.OtherChargesAmount = (
                    (parseFloat(objTemp.OtherChargesIDAmount1) || 0) +
                    (parseFloat(objTemp.OtherChargesIDAmount2) || 0) +
                    (parseFloat(objTemp.OtherChargesIDAmount3) || 0)
                ).toFixed(2);

                objTemp.TaxableChargesAmount = purchaseItem.TaxableChargesAmount || 0;
                objTemp.TaxAmount = purchaseItem.TaxAmount || 0;

                // Size object (if needed)
                if (purchaseItem.Size) {
                    var oSize = new Object();
                    oSize.SizeID = purchaseItem.Size.SizeID;
                    oSize.SizeName = purchaseItem.Size.SizeName;
                    objTemp.Size = oSize;
                }

                objTemp.StatusFlag = "";
                purchaseEntryArray.push(objTemp);
            });
            DisplayDataTable(purchaseEntryArray);

            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + PurchaseEntry.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(PurchaseEntry.LastUpdatedDateIST));

        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(response);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}
//Delete Data
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

$('#divAddUnitModal').on('shown.bs.modal', function () {
    $('#ddlitem').select2({ dropdownParent: $('#divAddUnitModal'), width: '100%' });
    $('#ddlOtherCharges1').select2({ dropdownParent: $('#divAddUnitModal'), width: '100%' });
    $('#ddlOtherCharges2').select2({ dropdownParent: $('#divAddUnitModal'), width: '100%' });
    $('#ddlOtherCharges3').select2({ dropdownParent: $('#divAddUnitModal'), width: '100%' });
    $('#ddlTax1').select2({ dropdownParent: $('#divAddUnitModal'), width: '100%' });
    $('#ddlTax2').select2({ dropdownParent: $('#divAddUnitModal'), width: '100%' });

});

$('#divDocument').on('shown.bs.modal', function () {
    $('#ddlDocumentType').select2({ dropdownParent: $('#Document'), width: '100%' });
    $('#ddlDocumentGroup').select2({ dropdownParent: $('#Document'), width: '100%' });
});

//------------------------------------------------------------------------------------------------

function GetPurchaseOrderDetailsByID(ID) {
    $("#divPurchaseOrderDetails").show();
    $("#divPurchaseOrderDetails").empty();

    $.ajax({
        url: GetPurchaseOrderDetailsByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {
            if (response != null && response.resultdata.Value != null) {
                $("#divAddEdit").show();
                $("#divRecords").hide();

                // Header details
                var headerDetails = `
                    <div class="alert alert-info mb-4">
                        <div class="row task-dates">
                            <div class="col-sm-3 col-6">
                                <div class="mt-2">
                                    <p class="text-muted mb-2">P.O No.</p>
                                    <h5 class="font-size-14"><i class="bx bx-copy-alt me-1 text-primary"></i>${response.resultdata.Value.PurchaseOrderNo}</h5>
                                </div>
                            </div>
                            <div class="col-sm-3 col-6">
                                <div class="mt-2">
                                    <p class="text-muted mb-2">P.O Date</p>
                                    <h5 class="font-size-14"><i class="bx bx-calendar-check me-1 text-primary"></i>${response.resultdata.Value.sPurchaseOrderDate}</h5>
                                </div>
                            </div>
                            <div class="col-sm-3 col-6">
                                <div class="mt-2">
                                    <p class="text-muted mb-2">Supplier</p>
                                    <h5 class="font-size-14"><i class="bx bx-user me-1 text-primary"></i>${response.resultdata.Value.SupplierName}</h5>
                                </div>
                            </div>
                            <div class="col-sm-3 col-6">
                                <div class="mt-2">
                                    <p class="text-muted mb-2">P.O Amount (Rs.)</p>
                                    <h5 class="font-size-14"><i class="bx bx-rupee me-1 text-primary"></i>${response.resultdata.Value.PurchaseOrderValue}</h5>
                                </div>
                            </div>
                        </div>
                    </div>`;

                $("#divPurchaseOrderDetails").html(headerDetails);
                $("#ddlSupplierType").val(response.resultdata.Value.SupplierID).prop("disabled", true).change();
                $("#hdnPurchaseOrderID").val(response.resultdata.Value.PurchaseOrderID);

            } else {
                $("#divPurchaseOrderDetails").empty();
            }
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
function InitializePurchaseEntry() {
    if ($.cookie("PurchaseEntryID") !== undefined) {
        const purchaseEntryId = $.cookie("PurchaseEntryID");
        const mode = $.cookie("PurchaseEntryMode");

        // Clear cookies immediately
        $.cookie("PurchaseEntryID", null);
        $.cookie("PurchaseEntryMode", null);

        if (mode === 'view') {
            EditData(purchaseEntryId, true);  // true for view mode
        } else if (mode === 'edit') {
            EditData(purchaseEntryId, false);  // false for edit mode
        }
        else if (mode === 'delete') {
            DeleteData(purchaseEntryId);
        }
    }
}