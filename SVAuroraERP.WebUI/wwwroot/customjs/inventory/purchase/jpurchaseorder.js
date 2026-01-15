var gItemList = [];
let purchaseOrderArray = [];
function getMaxSNo() {
    if (purchaseOrderArray.length === 0) return 0;
    return Math.max(...purchaseOrderArray.map(item => parseInt(item.sNo) || 0));
}
function getValidatedFloat(selector) {
    let value = parseFloat($(selector).val());
    return isNaN(value) ? 0 : value;
}
function getValidatedID(selector) {
    let value = parseInt($(selector).val(), 10); // Parse as an integer
    return isNaN(value) || value <= 0 ? 0 : value; // Return 0 if invalid or <= 0
}
$(function () {
    pLoadingSetup(false);

    $("#btnSave").show();
    $("#btnUpdate").hide();

    $("#divAddEdit").hide();
    $("#divRecords").show();

    BindItemList();
    LoadItemList("ddlItem");
    //calendar
    $("#txtPoDate").datetimepicker({
        pickTime: false,
        useCurrent: true,
        format: 'DD/MM/YYYY',
        minYear: 1951,
        maxDate: moment()
    });

    $(".ClearDate").click(function () { var x = $(this).prev().attr('id'); $("#" + x).val(""); });
    $(".SelectDate").click(function () { var x = $(this).next().attr('id'); $("#" + x).focus(); });

    getRecordList();
    pLoadingSetup(true);
});
$("#btnAddNew").on('click', function () {
    $("#divAddEdit").show();
    $("#divRecords").hide();
    $("#btnSave").show();
    $("#btnUpdate").hide();
    // ClearFormFields();

    $("#divCardTitle").html("<i class='fas fa-plus-square align-middle me-1'></i>Add New Purchase Order");

    return false;
});
$("#btnAddNewItems").on("click", function () {
    $("#divAddUnitModal .modal-title").html("<i class='bx bxs-plus-square font-size-20 align-middle me-1'></i>&nbsp;Add New Item");
    ClearModuleFormFields();
    $("#btnSaveItem").show();
    $("#btnUpdateItem").hide();
    return false;
});

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
function ClearModuleFormFields() {
    $("#divAddUnitModal .modal-body :input").attr("disabled", false);
    $("#hdnSNo").val("0");
    $("#hdnPurchaseOrderTransID").val("0");
    $("#hdnItemID").val(0);

    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#ddlItem").val("0").change();
    $("#txtQuantity").val("");


    $("#btnSaveItem").show();
    $("#btnUpdateItem").hide();

    return false;
}
$("#btnSaveItem,#btnUpdateItem").on("click", function () {
    let isValid = true;

    // Clear previous validation messages
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    let PurchaseOrderTrans = new Object();

    let selectedItemID = $("#ddlItem").val();
    let selectedItem = gItemList.find(item => item.ItemID == selectedItemID);


    if (selectedItem) {
        PurchaseOrderTrans.ItemID = selectedItem.ItemID;
        PurchaseOrderTrans.ItemName = selectedItem.ItemName;
        PurchaseOrderTrans.HSNCode = selectedItem.HSNCode;
        PurchaseOrderTrans.ItemCode = selectedItem.ItemCode;
        PurchaseOrderTrans.UnitName = selectedItem.UnitName;
    } else {
        PurchaseOrderTrans.ItemID = null;
        PurchaseOrderTrans.ItemName = null;
        PurchaseOrderTrans.HSNCode = null;
        PurchaseOrderTrans.ItemCode = null;
        PurchaseOrderTrans.UnitName = null;
    }
    PurchaseOrderTrans.Quantity = getValidatedFloat("#txtQuantity");

    // Validation
    if (!PurchaseOrderTrans.ItemID) {
        $('#ddlitem').addClass('is-invalid');
        $('#ddlitem').after('<div class="invalid-feedback">Please Select the Item</div>');
        $('#ddlitem').focus();
        isValid = false;
        return false;
    }

    if (!PurchaseOrderTrans.Quantity) {
        $('#txtQuantity').addClass('is-invalid');
        $('#txtQuantity').after('<div class="invalid-feedback">Please Enter the Quantity</div>');
        $('#txtQuantity').focus();
        isValid = false;
        return false;
    }


    if (isValid) {
        const purchaseOrderTransID = parseInt($("#hdnPurchaseOrderTransID").val()) || 0;

        if (this.id === "btnSaveItem") {
            // For new entries
            const maxSNo = getMaxSNo();
            PurchaseOrderTrans.sNo = maxSNo + 1;
            PurchaseOrderTrans.StatusFlag = "I"; // Insert
            PurchaseOrderTrans.PurchaseOrderTransID = 0;

            if (!isDuplicateEntry(PurchaseOrderTrans.ItemID, 0)) {
                Add_PurchaseTrans(PurchaseOrderTrans);

            } else {
                $.jGrowl("The entered item already exists!", { sticky: false, theme: 'warning', life: 3000 });
                return false;
            }
        } else if (this.id === "btnUpdateItem") {
            // For updates
            const currentSNo = parseInt($("#hdnSNo").val());
            PurchaseOrderTrans.sNo = currentSNo;

            if (purchaseOrderTransID > 0) {
                PurchaseOrderTrans.StatusFlag = "U"; // Update
                PurchaseOrderTrans.PurchaseOrderTransID = purchaseOrderTransID;
            } else {
                PurchaseOrderTrans.StatusFlag = "I"; // Insert
                PurchaseOrderTrans.PurchaseOrderTransID = 0;
            }

            if (!isDuplicateEntry(PurchaseOrderTrans.ItemID, PurchaseOrderTrans.sNo)) {
                Update_PurchaseTrans(PurchaseOrderTrans);
            } else {
                $.jGrowl(PurchaseOrderTrans.ItemName + " is already added to the list.", {
                    sticky: false,
                    theme: 'warning',
                    life: jGrowlLife
                });
                return false;
            }
        }

        // Clear form or hide modal based on operation
        if (this.id === "btnSaveItem") {
            ClearModuleFormFields();
        } else {
            $("#divAddUnitModal").modal('hide');
        }
    }
});
function isDuplicateEntry(itemID, sNo) {
    for (let i = 0; i < purchaseOrderArray.length; i++) {

        if (purchaseOrderArray[i].ItemID === itemID && purchaseOrderArray[i].sNo !== sNo) {
            return true;
        }
    }
    return false;
}
function Add_PurchaseTrans(oData) {
    purchaseOrderArray.push(oData);
    // Change this message
    $.jGrowl(oData.ItemName + " successfully added to the list.", { sticky: false, theme: 'success', life: jGrowlLife });
    DisplayDataTable(purchaseOrderArray, false);
    return false;
}
function DisplayDataTable(purchaseOrderArray, ViewFlag) {
    $("#divTableData").show();
    let tableContent = `
        <table id="purchaseTable" class="table table-condensed table-hover">
            <thead>
                <tr class="table-light">
                    <th>S.No</th>
                    <th>Item</th>
                    <th>Code</th>
                    <th>HSN</th>
                    <th>Quantity</th>
                    <th>Units</th>
                    <th style="text-align:center">Actions</th>
                </tr>
            </thead>
            <tbody>`;

    if (purchaseOrderArray.length > 0) {
        purchaseOrderArray.forEach((entry, index) => {
            if (entry.StatusFlag != "D") {
                tableContent += `
           <tr data-sno="${entry.sNo}">
                <td>${index + 1}</td>
                <td>${entry.ItemName || ""}</td>
                <td>${entry.ItemCode || ""}</td>
                <td>${entry.HSNCode || ""}</td>
                <td>${entry.Quantity.toFixed(2) || ""}</td>
                <td>${entry.UnitName || ""}</td>`;

                if (ViewFlag == false) {
                    tableContent += `
                    <td style="text-align:center;">
                        <a href="javascript:void(0);" onclick="Edit_PurchaseOrderTrans(${entry.sNo})" class="btn btn-sm btn-soft-info" title="Edit" data-bs-toggle="modal" data-bs-target="#divAddUnitModal">
                            <i class="mdi mdi-pencil-outline"></i>
                        </a>
                        <a href="javascript:void(0);" onclick="Delete_PurchaseOrderTrans(${entry.sNo})" class="btn btn-sm btn-soft-danger" title="Delete">
                            <i class="mdi mdi-delete-outline"></i>
                        </a>
                    </td>`;
                }
                else {
                    tableContent += '<td style="text-align:center">No Action<td>';
                }

                tableContent += `</tr>`;
            }
        });
    }
    else {
        tableContent += `<tr><td colspan="6" class="text-center">No Records To Display</td></tr>`;
    }
    tableContent += `
            </tbody>
        </table>`;

    // Update the divTableData container
    $("#divTableData").empty();
    $("#divTableData").html(tableContent);
}
function Edit_PurchaseOrderTrans(ID) {
    if (!ID) {
        return false;
    }

    // Convert ID to number for consistent comparison
    const idToFind = parseInt(ID);
    const entry = purchaseOrderArray.find(entry => parseInt(entry.sNo) === idToFind);

    if (!entry) {
        return false;
    }
    $("#divAddUnitModal .modal-title").html("<i class='bx bxs-edit font-size-20 align-middle me-1'></i>&nbsp;Edit Item");
    $("#divAddUnitModal").show();
    // Clear previous validation states
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    $("#hdnSNo").val(entry.sNo);
    $("#hdnPurchaseOrderTransID").val(entry.PurchaseOrderTransID || 0);

    // Set item dropdown and related fields
    $("#ddlItem").val(entry.ItemID).change();

    // Set basic fields with null checks
    $("#txtQuantity").val(entry.Quantity || '');

    // Show/hide buttons
    $("#btnSaveItem").hide();
    $("#btnUpdateItem").show();

    // Focus on first field
    $("#ddlItem").focus();

    return false;
}
//Update the field in the table
function Update_PurchaseTrans(oData) {
    const index = purchaseOrderArray.findIndex(item => parseInt(item.sNo) === parseInt(oData.sNo));

    if (index === -1) {
        return false;
    }
    oData.sNo = parseInt(oData.sNo);
    purchaseOrderArray[index] = oData;
    // Update the configuration array
    purchaseOrderArray.forEach((order) => {
        if (order.sNo === oData.sNo) {
            order.ItemName = oData.ItemName;
            order.HSNCode = oData.HSNCode;
            order.UnitName = oData.UnitName;
            order.Quantity = oData.Quantity;
            order.StatusFlag = oData.StatusFlag;
        }
    });

    DisplayDataTable(purchaseOrderArray, false);
    $("#btnSaveItem").show();
    $("#btnUpdateItem").hide();
    ClearModuleFormFields();
    $.jGrowl("Item updated successfully!", { sticky: false, theme: 'success', life: jGrowlLife });
    return false;
}
function Delete_PurchaseOrderTrans(ID) {
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
            purchaseOrderArray.forEach((order) => {
                if (order.sNo == ID) {
                    var index = purchaseOrderArray.findIndex(record => record.sNo === ID);
                    if (order.PurchaseOrderTransID > 0)
                        order.StatusFlag = "D";
                    else
                        purchaseOrderArray.splice(index, 1);
                }
            });
            Swal.fire({
                title: "Deleted",
                text: "Your data deleted successfully!",
                icon: "success",
                confirmButtonColor: "#556ee6"
            });

            DisplayDataTable(purchaseOrderArray, false);
        } else {
            Swal.fire({
                title: "Cancelled",
                text: "Your data is safe :)",
                icon: "error"
            });
        }
    });

    ClearModuleFormFields();
    $("#ddlItem").focus();
    return false;
}
//---------------------------Purchase Order----------------------------------------------------------------------------------

$("#btnClose,#btnCloseWindow").on('click', function () {
    $("#divRecords").show();
    $("#divAddEdit").hide();

    getRecordList();
    ClearFormFields();
    return false;
});
$('#btnRefresh').on('click', function () {
    getRecordList();
    return false;
});
function ClearFormFields() {
    $("#divAddEdit .card-body :input").attr("disabled", false);
    $("#divAddUnitModal .modal-body :input").attr("disabled", false);
    $("#hdnPurchaseOrderID").val(0);
    $("#txtPoNo").val("");
    $("#txtPoDate").val("");
    $("#ddlSupplierType").val("0").change();
    $("#txtPoValue").val("");
    $("#spnLastUpdatedBy").empty();
    $("#spnLastUpdatedDate").empty();

    ClearModuleFormFields();
    purchaseOrderArray = []; divTableData
    $("#divTableData").empty();

    return false;
}
//save the Purchase Entry data
$("#btnSave,#btnUpdate").on('click', function () {
    let isValid = true;
    $('.invalid-feedback').remove();
    $('.form-control').removeClass('is-invalid');

    var PurchaseOrder = new Object();

    // Set ID and StatusFlag
    PurchaseOrder.PurchaseOrderID = 0;
    if (this.id == "btnUpdate" && $("#hdnPurchaseOrderID").val() > 0) {
        PurchaseOrder.PurchaseOrderID = $("#hdnPurchaseOrderID").val();
    }
    PurchaseOrder.StatusFlag = (PurchaseOrder.PurchaseOrderID === 0) ? "I" : "U";

    // Basic Details
    PurchaseOrder.PurchaseOrderNo = $('#txtPoNo').val();
    PurchaseOrder.sPurchaseOrderDate = $('#txtPoDate').val();
    PurchaseOrder.SupplierID = getValidatedID('#ddlSupplierType');
    PurchaseOrder.PurchaseOrderValue = getValidatedFloat("#txtPoValue");
    PurchaseOrder.PurchaseOrderStatusID = 1;

    // Transactions
    PurchaseOrder.PurchaseOrderTransList = purchaseOrderArray;

    // Validations
    if (!PurchaseOrder.PurchaseOrderNo) {
        $('#txtPoNo').addClass('is-invalid');
        $('#txtPoNo').after('<div class="invalid-feedback">Please Enter P.O No</div>');
        $('#txtPoNo').focus();
        return false;
    }

    if (!PurchaseOrder.sPurchaseOrderDate) {
        $('#txtPoDate').addClass('is-invalid');
        $('#txtPoDate').after('<div class="invalid-feedback">Please Select P.O Date</div>');
        $('#txtPoDate').focus();
        return false;
    }

    if (!PurchaseOrder.SupplierID) {
        $('#ddlSupplierType').addClass('is-invalid');
        $('#ddlSupplierType').after('<div class="invalid-feedback">Please Select Supplier</div>');
        $('#ddlSupplierType').focus();
        return false;
    }
    if (PurchaseOrder.PurchaseOrderValue == null || PurchaseOrder.PurchaseOrderValue == undefined) {
        $('#txtPoValue').addClass('is-invalid');
        $('#txtPoValue').after('<div class="invalid-feedback">Please Enter P.O Value</div>');
        $('#txtPoValue').focus();
        return false;
    }
    var count = 0;
    purchaseOrderArray.forEach(function (purchase) {
        if (purchase.StatusFlag != 'D') {
            count++;
        }
    });
    if (count == 0) {
        $.jGrowl("Kindly Enter Atlest one Item", { sticky: false, theme: 'warning', life: 3000 });
        return false;
    }

    if (isValid) {
        SaveandUpdatePurchaseEntry(PurchaseOrder);
    }

    return false;
});
function SaveandUpdatePurchaseEntry(PurchaseOrder) {
    if (ENABLE_VERBOSE_Logging) //console.log(PurchaseOrder);

    $.ajax({
        url: SaveUpdateDataUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(PurchaseOrder),
        success: function (response) {
            if (ENABLE_VERBOSE_Logging) console.log(response);

            if (response.dataResponse.Success) {
                if (PurchaseOrder.PurchaseOrderID == 0)
                    Swal.fire({ title: "Saved!", text: SaveSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });
                else if (PurchaseOrder.PurchaseOrderID > 0)
                    Swal.fire({ title: "Updated!", text: UpdateSuccessMessage, icon: "success", confirmButtonColor: "#556ee6" });

                $("#btnClose").click();
            }
            else if (!response.dataResponse.Success) {
                Swal.fire({ title: "Data already exists!", text: "", icon: "warning", confirmButtonColor: "#556ee6" });
            }
            else if (!response.dataResponse.Success && response.dataResponse.Error) {
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
            url: PurchaseOrderDataTableUrl,
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
                "width": "1%",
                "orderable": false
            },
            { "data": "PurchaseOrderNo", "orderable": true, "width": "10%" },
            { "data": "sPurchaseOrderDate", "orderable": true, "width": "10%" },
            { "data": "SupplierName", "orderable": true },
            {
                "data": "PurchaseOrderValue", "orderable": true, "width": "10%",
                "render": function (data, type, row) {
                    return `<i class="bx bx-rupee text-primary font-size-16"></i>${row.PurchaseOrderValue.toFixed(2)}`
                },
            },
            {
                "data": "PurchaseOrderStatus",
                "className": "text-center",
                "render": function (data, type, row) {
                    return `<span class="badge ${row.ColorCode}">${row.PurchaseOrderStatus}</span>`;
                },
                "width": "10%",
                "orderable": false
            },
            {
                data: null,
                "className": "text-center",
                bSortable: false,
                render: function (data, type, row) {
                    let actionButtons = '';
                    if (row.PurchaseOrderStatusID === 1) {
                        actionButtons = `
                                 <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Create Purchase Entry">
                                     <a href="javascript:void(0);" onclick="CreatePurcahseEntry(${row.PurchaseOrderID})" class="btn btn-sm btn-soft-primary" title="Create New Purchase Entry">
                                     <i class="bx bxs-purchase-tag"></i>
                                     </a>
                                 </li>` ;
                    }
                    else {
                        return SetAction(row.PurchaseOrderID);
                    }
                    return `<ul class="list-unstyled hstack gap-1 mb-0">${actionButtons}${SetAction(row.PurchaseOrderID)}</ul>`;
                },
                "width": "5%",
                "orderable": false
            }
        ]
    });
    $(".dataTables_paginate").addClass("pagination-rounded");
}

function EditData(ID, ViewFlag) {
    if (ENABLE_VERBOSE_Logging) //console.log(ID);
    ClearModuleFormFields();

    $.ajax({
        url: GetDataByIDUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: ID },
        success: function (response) {

            if (ViewFlag) {
                $("#divAddEdit .card-body :input").attr("disabled", true);
                $("#divCardTitle").html("<i class='fas fa-eye align-middle me-1'></i>View Purchase Order");
                $("#btnSave").hide();
                $("#btnUpdate").hide();
                $("#divAddUnitModal .modal-body :input").attr("disabled", true);
                $("#btnCloseWindow,#btnClose").attr("disabled", false);
            }
            else {
                $("#divCardTitle").html("<i class='fas fa-edit  me-1'></i>Edit Purchase Order");
                $("#btnSave").hide();
                $("#btnUpdate").show();
            }

            $("#divAddEdit").show();
            $("#divRecords").hide();

            var PurchaseOrder = response.dataResponse.Value;

            $("#hdnPurchaseOrderID").val(PurchaseOrder.PurchaseOrderID);
            $("#txtPoNo").val(PurchaseOrder.PurchaseOrderNo);
            $("#txtPoDate").val(PurchaseOrder.sPurchaseOrderDate);
            $("#ddlSupplierType").val(PurchaseOrder.SupplierID).change();
            $("#txtPoValue").val(PurchaseOrder.PurchaseOrderValue);

            purchaseOrderArray = [];

            PurchaseOrder.PurchaseOrderTransList.forEach((purchaseItem, index) => {
                var objTemp = new Object();

                // Basic fields
                objTemp.SNo = index + 1;
                objTemp.sNo = objTemp.SNo;
                objTemp.PurchaseOrderTransID = purchaseItem.PurchaseOrderTransID;
                objTemp.ItemID = purchaseItem.ItemID;
                objTemp.ItemCode = purchaseItem.ItemCode;
                objTemp.ItemName = purchaseItem.ItemName;
                objTemp.HSNCode = purchaseItem.HSNCode;
                objTemp.UnitName = purchaseItem.UnitName;
                objTemp.Quantity = purchaseItem.Quantity;
                objTemp.StatusFlag = "";
                purchaseOrderArray.push(objTemp);
            });
            DisplayDataTable(purchaseOrderArray, ViewFlag);
            $("#divRecordLog").show();
            $("#spnLastUpdatedBy").html("Last Updated By: " + PurchaseOrder.LastUpdatedByName);
            $("#spnLastUpdatedDate").html("Date: " + ISTtoLocalTime(PurchaseOrder.LastUpdatedDate));

        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(response);
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
            if (response.dataResponse.Success && response.dataResponse.Error == false) {
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
    $('#ddlItem').select2({ dropdownParent: $('#divAddUnitModal'), width: '100%' });
});

//$('#divAddEdit').on('shown.bs.modal', function () {
//    $('#ddlSupplierType').select2({ dropdownParent: $('#divAddEdit'), width: '100%' });
//});

//Added on 2025.02.04
function CreatePurcahseEntry(POID) {
    $.cookie("PurchaseOrderID", parseInt(POID));

    window.open("/Inventory/Purchase/PurchaseEntry"); // Opens in a new tab    
    return false;
}