var LoadingDataArray = [];
$(function () {
    pLoadingSetup(false);
    const urlParams = new URLSearchParams(window.location.search);
    const orderID = urlParams.get("orderId");
    const returnUrl = urlParams.get("returnUrl");

    // store return url safely
    if (returnUrl) {
        sessionStorage.setItem("UpdateOrder_ReturnUrl", returnUrl);
    }

    $("#hdnHSRPOrderID").val(orderID);

    $("#txtRegistrationDate").datetimepicker({
        pickTime: false,
        useCurrent: true,
        format: 'DD/MM/YYYY',
        minYear: 1951,
    });

    $(".ClearDate").click(function () { var x = $(this).prev().attr('id'); $("#" + x).val(""); });
    $(".SelectDate").click(function () { var x = $(this).next().attr('id'); $("#" + x).focus(); });



    $("#divAddEdit .card-body")
        .find("input:not([type='checkbox'])")
        .prop("disabled", true);
    $("#divddlfront, #divddlRear").hide();

    if (orderID) {
        LoadOrderDetails(orderID);
    }
    GetRectificaionReason("ddlRectification", GetRectificaionReasonUrl, _TOKEN);

    $("#btnRectifyOrder").hide();
    $("#btnUpdateOrder").show();
    $("#ddlRectification").attr("disabled", true);
    $("#txtRemarks").attr("disabled", true);
    resetPlateControls();

    

    pLoadingSetup(true);
});
function resetPlateControls() {
    // Front
    $("#chkFront").prop("checked", false).prop("disabled", true);
    $("#divddlfront").hide();
    $("#divtxtfront").show();

    // Rear
    $("#chkRear").prop("checked", false).prop("disabled", true);
    $("#divddlRear").hide();
    $("#divtxtRear").show();
}

$("#ddlRectification").on("change", function () {
    const rectificationID = parseInt($(this).val());
    resetPlateControls();
    switch (rectificationID) {
        case 1:
            $("#chkFront").prop("disabled", false);
            break;
        case 2:
            $("#chkRear").prop("disabled", false);
            break;
        case 3:
            $("#chkFront").prop("disabled", false);
            $("#chkRear").prop("disabled", false);
            break;
        case 4:
        default:
            break;
    }
});


// Front checkbox
$("#chkFront").change(function () {
    if ($(this).is(":checked")) {
        $("#divtxtfront").hide();
        $("#divddlfront").show();
    } else {
        $("#divddlfront").hide();
        $("#divtxtfront").show();
    }
});

// Rear checkbox
$("#chkRear").change(function () {
    if ($(this).is(":checked")) {
        $("#divtxtRear").hide();
        $("#divddlRear").show();
    } else {
        $("#divddlRear").hide();
        $("#divtxtRear").show(); 
    }
});

$("#chkRectify").change(function () {
    if ($(this).is(":checked")) {
        $("#btnRectifyOrder").show();
        $("#btnUpdateOrder").hide();
        $("#ddlRectification").attr("disabled", false);
        $("#txtRemarks").attr("disabled", false);
    } else {
        $("#btnRectifyOrder").hide();
        $("#btnUpdateOrder").show();
        $("#ddlRectification").attr("disabled", true);
        $("#txtRemarks").attr("disabled", true);
    }
});
function LoadOrderDetails(orderID) {
    $.ajax({
        url: GetOrderDetailsByIdUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        data: { ID: orderID },
        success: function (result) {
            if (result.Data.Success && result.Data.Value) {

                const data = result.Data.Value;
                const userTypeId = result.UserTypeID;  
                LoadingDataArray = data;

                $("#divCardTitle").html(
                    `<i class='fas fa-edit me-1'></i> Update Orders 
             &nbsp;&nbsp;<span class="badge ${data.ColorCode}">${data.Description}</span>`
                );
                $("#chkFront").prop("checked", false);
                $("#divddlfront").hide();
                $("#divtxtfront").show();

                $("#chkRear").prop("checked", false);

                $("#divddlRear").hide();
                $("#divtxtRear").show(); 

                
                $("#txtRegistrationDate").prop("disabled", true);
                $("#txtChassisNumber").prop("disabled", true);
                $("#txtEngineNumber").prop("disabled", true);

                if (userTypeId === 1) {
                    $("#txtRegistrationDate").prop("disabled", false);
                    $("#txtChassisNumber").prop("disabled", false);
                    $("#txtEngineNumber").prop("disabled", false);
                }

                $("#txtOrderNumber").val(data.OrderNo);
                $("#txtOrderDate").val(data.sOrderDate);
                $("#txtPONumber").val(data.DealerPONo);
                $("#txtSONumber").val(data.DealerSONo);
                $("#txtRegistrationNumber").val(data.RegNo);
                $("#txtRegistrationDate").val(data.sRegDate);
                $("#txtChassisNumber").val(data.ChasisNo);
                $("#txtEngineNumber").val(data.EngineNo);

                $("#txtFrontNo").val(data.FrontLaserSerialNo + " (" + data.FrontPlateDimension + ")");
                $("#txtRearNo").val(data.RearLaserSerialNo + " (" + data.RearPlateDimension + ")");

                $("#txtRemarks").val(data.Remarks || "");

                GetLaserNoByPartNo(data.PartNo);

            } else {
                Swal.fire("Not Found", "Record not found.", "warning");
            }
        },
        error: function () {
            Swal.fire("Error", "Failed to load order data.", "error");
        }
    });
}
function GetLaserNoByPartNo(PartNo) {
    $.ajax({
        url: GetLaserNoByPartNoURL, 
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        data: { PartNo: PartNo},
        success: function (result) {
            if (result.Success && result.Value) {
               
                LoadLaserSerialNoDropDown("ddlFront", result.Value.FrontLaserNoData);
                LoadLaserSerialNoDropDown("ddlRear", result.Value.RearLaserNoData);

            } else {
                Swal.fire("Not Found", "Record not found.", "warning");
            }
        },
        error: function () {
            Swal.fire("Error", "Failed to load order data.", "error");
        }
    });
}
function LoadLaserSerialNoDropDown(ddldropdown, data) {
    $('#' + ddldropdown).empty();
    $('#' + ddldropdown).append("<option value='0'>--Select Laser Serial No--</option>");

    $.each(data || [], function (i, response) {
        $('#' + ddldropdown).append(`<option value="${response.PK_HSRPLaserNoStockID}">${response.SerialNo}(${response.Dimension})</option>`);
    });

    $('#' + ddldropdown).val(0);
}

$("#btnRectifyOrder").on('click', function () {

    var RectifyOrderData = new Object();

    RectifyOrderData.RectifyLaserPlateID = 0;
    RectifyOrderData.HSRPOrderID = LoadingDataArray.HSRPOrderID;
    RectifyOrderData.HSRPOrderRectificationReasonID = $('#ddlRectification').val();
    RectifyOrderData.FrontLaserNoPlateID = $("#ddlFront").val();
    RectifyOrderData.RearLaserNoPlateID = $("#ddlRear").val();
    RectifyOrderData.Remarks = $('#txtRemarks').val();


    if (!RectifyOrderData.HSRPOrderRectificationReasonID || RectifyOrderData.HSRPOrderRectificationReasonID === "0") return markInvalid("#ddlRectification", " Please Select Rectification Reason");
    //if ($("#txtFrontNo").val() == 'Not Assigned ()' || $("#txtRearNo").val() == 'Not Assigned ()')  return markInvalid("#ddlFront", " Please change the Laser Plate No");

    const reasonID = parseInt(RectifyOrderData.HSRPOrderRectificationReasonID);

    if (!reasonID || reasonID === 0) {
        return markInvalid("#ddlRectification", "Please select Rectification Reason");
    }

    if ((reasonID === 1 || reasonID === 3) && $("#ddlFront").val()== 0){
            return markInvalid("#ddlFront", "Please change Front Laser Plate No");
    }

    if ((reasonID === 2 || reasonID === 3) && $("#ddlRear").val() ==0) {
            return markInvalid("#ddlRear", "Please change Rear Laser Plate No");
    }

    SaveandUpdateRectifyOrder(RectifyOrderData);

    return false;
});



$("#btnCloseWindow").on("click", function () {

    let returnUrl = sessionStorage.getItem("UpdateOrder_ReturnUrl");

    if (returnUrl) {
        sessionStorage.removeItem("UpdateOrder_ReturnUrl");
        window.location.href = decodeURIComponent(returnUrl);
    } else {
        // fallback
        window.history.back();
    }
});


function SaveandUpdateRectifyOrder(RectifyOrderData) {
        $.ajax({
            url: SaveandUpdateRectifyOrderUrl,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(RectifyOrderData),
            success: function (response) {
                    if (response.Success && !response.Error) {
                            Swal.fire({ title: "Saved!", text: "Rectify Order Saved Successfully", icon: "success", confirmButtonColor: "#556ee6" });
                        LoadOrderDetails(RectifyOrderData.HSRPOrderID);
                      
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


$("#btnUpdateOrder").click(function () {
    var LaserNoUpdateRequestData = new Object();

    LaserNoUpdateRequestData.HSRPOrderID = LoadingDataArray.HSRPOrderID;
    LaserNoUpdateRequestData.FrontLaserNoPlateID = 0;
    LaserNoUpdateRequestData.RearLaserNoPlateID = 0;
    LaserNoUpdateRequestData.sOrderDate = $("#txtRegistrationDate").val();
    LaserNoUpdateRequestData.ChassisNumber = $("#txtChassisNumber").val();
    LaserNoUpdateRequestData.EngineNumber = $("#txtEngineNumber").val();

    if (LaserNoUpdateRequestData.sOrderDate == "") return markInvalid("#txtRegistrationDate", " Please Select Registration Date");
    if (LaserNoUpdateRequestData.ChassisNumber == "") return markInvalid("#txtChassisNumber", " Please Enter Chassis Number");
    if (LaserNoUpdateRequestData.EngineNumber == "") return markInvalid("#txtEngineNumber", " Please Enter Engine Number");


    if ($("#chkFront").is(":checked")) { LaserNoUpdateRequestData.FrontLaserNoPlateID = $('#ddlFront').val(); }
    if ($("#chkRear").is(":checked")) { LaserNoUpdateRequestData.RearLaserNoPlateID = $('#ddlRear').val(); }

   // if (LaserNoUpdateRequestData.FrontLaserNoPlateID == "0" && LaserNoUpdateRequestData.RearLaserNoPlateID == "0") return markInvalid("#ddlFront", " Please change the Laser Plate No");

    SaveLaserNoForOrder(LaserNoUpdateRequestData);
});


function SaveLaserNoForOrder(LaserNoUpdateRequestData) {
    $.ajax({
        url: SaveLaserNoForOrderUrl,
        type: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(LaserNoUpdateRequestData),
        success: function (response) {
            if (response.Success && !response.Error) {
                Swal.fire({ title: "Saved!", text: "Order Updated Successfully", icon: "success", confirmButtonColor: "#556ee6" });
                LoadOrderDetails(LaserNoUpdateRequestData.HSRPOrderID);
            }
            else
                Swal.fire({ title: "Error", text: response.message, icon: "error", confirmButtonColor: "#556ee6" });
        },
        error: function () {
            Swal.fire("Error", "Failed to update order.", "error");
        },
        complete: function () {
            $("#btnSaveUpdate").prop("disabled", false).text("Update");
        }
    });
}