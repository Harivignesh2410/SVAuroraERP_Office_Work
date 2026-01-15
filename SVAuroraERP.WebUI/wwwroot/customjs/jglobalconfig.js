var ActionAdd = '0', ActionUpdate = '0', ActionDelete = '0', ActionView = '0', ActionAccess = '0', ActionExport = '0';
var jGrowlLife = 3000;

var ENABLE_VERBOSE_Logging = true;

var SaveErrorMessage = "Unexpected error occured. Please contact Administrator";
var SaveSuccessMessage = "Data Saved Successfully";

var UpdateErrorMessage = "Unexpected error occured. Please contact Administrator";
var UpdateSuccessMessage = "Data Updated Successfully";

var DeleteErrorMessage = "Unexpected error occured. Please contact Administrator";
var DeleteSuccessMessage = "Data Deleted Successfully";

//Added on 2024.12.15
var _CWICloseWindowIcon = "far fa-window-close fa-lg bg-danger text-white";

var _CMAccessDeined = "You don't have permission. Please contact Administrator";

//Added on 2025.01.20
var _MaxSubStringLength = 20;

//Added on 2024.10.26
function ISTtoLocalTime(istDate) {
    // Create a Date object from the UTC timestamp
    const date = new Date(istDate);

    // Format the date to the desired format
    const formattedDate = date.toLocaleString("en-GB", {
        day: "2-digit",
        month: "short",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit",
        hour12: true,
    });

    return formattedDate;
}
function pLoadingSetup(dFlag) {
    if (dFlag) {
        $('.WebPageContent').removeClass("d-none");
    }
    else {
        $('.WebPageContent').addClass("d-none");
    }
}

function SetStatus(Flag) {
    if (Flag) {
        return '<span class="badge bg-success">Active</span>';
    }
    else {
        return '<span class="badge bg-danger">Inactive</span>';
    }
}

function SetModalAction(ID) {
    return `
                                                    <ul class="list-unstyled hstack gap-1 mb-0">
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                                                            <a href="javascript:void(0);" onclick="EditData(${ID}, true)" class="btn btn-sm btn-soft-primary" data-bs-toggle="modal" data-bs-target="#divAddEditModal">
                                                                <i class="mdi mdi-eye-outline"></i>
                                                            </a>
                                                        </li>
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Edit">
                                                            <a href="javascript:void(0);" onclick="EditData(${ID},false)"  class="btn btn-sm btn-soft-info" data-bs-toggle="modal" data-bs-target="#divAddEditModal">
                                                                <i class="mdi mdi-pencil-outline"></i>
                                                            </a>
                                                        </li>
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
                                                            <a href="javascript:void(0);" onclick="DeleteData('${ID}')" class="btn btn-sm btn-soft-danger">
                                                                <i class="mdi mdi-delete-outline"></i>
                                                            </a>
                                                        </li>
                                                    </ul>`;
}

function SetAction(ID) {
    return `
                                                    <ul class="list-unstyled hstack gap-1 mb-0 ">
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                                                            <a href="javascript:void(0);" onclick="EditData(${ID}, true)" class="btn btn-sm btn-soft-primary">
                                                                <i class="mdi mdi-eye-outline"></i>
                                                            </a>
                                                        </li>
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Edit">
                                                            <a href="javascript:void(0);" onclick="EditData(${ID},false)"  class="btn btn-sm btn-soft-info" >
                                                                <i class="mdi mdi-pencil-outline"></i>
                                                            </a>
                                                        </li>
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
                                                            <a href="javascript:void(0);" onclick="DeleteData('${ID}')" class="btn btn-sm btn-soft-danger">
                                                                <i class="mdi mdi-delete-outline"></i>
                                                            </a>
                                                        </li>
                                                    </ul>`;
}
function SetActionViewAndExportOnly(ID) {
    return `
                                                   <ul class="list-unstyled hstack gap-1 mb-0">
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                                                            <a href="javascript:void(0);" onclick="EditData(${ID}, true)" class="btn btn-sm btn-soft-primary">
                                                                <i class="mdi mdi-eye-outline"></i>
                                                            </a>
                                                        </li>
                                                        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
                                                            <a href="javascript:void(0);" onclick="PrintReport(${ID})" class="btn btn-sm btn-soft-primary">
                                                                <i class="bx bx-printer text-pink"></i>
                                                            </a>
                                                        </li>
                                                    </ul> `;
}
//Added on 2025.03.16
function GetStockStatus(statuscode) {
    var status = "";

    if (statuscode == "Stock Available")
        status = "<span class='badge bg-success'>" + statuscode + "</span>";
    else if (statuscode == "Insufficient Stock")
        status = "<span class='badge bg-danger'>" + statuscode + "</span>";

    return status;
}

var AluminiumCoil = 1;
var RRS = 2;
var Hologram = 3;
var BARCODE = 4;
var CARTONBOX = 5;
var HOTFOIL = 6;
var CAUTIONLABEL = 7;
var Rivets = 8;
var screw = 9;
var BLANKPLATE = 11;
var HOLOGRAMPLATE = 12;
var LASERNOPLATE = 13;
var NUMBERPLATE = 14;

var ColorNone = 8;
var SizeNone = 8;


/*
//==== DEV ======
var AluminiumCoil = 4;
var RRS = 5;
var Hologram = 6;
var BARCODE = 7;
var CARTONBOX = 8;
var HOTFOIL = 9;
var CAUTIONLABEL = 10;
var Rivets = 11;
var screw = 12;
var BLANKPLATE = 13;
var HOLOGRAMPLATE = 14;
var LASERNOPLATE = 15;
var NUMBERPLATE = 16;

var ColorNone = 4;
var SizeNone = 2;
*/

/*
Prod Global Config Settings

var AluminiumCoil = 1;
var RRS = 2;
var Hologram = 7;
var BARCODE = 13;
var CARTONBOX = 5;
var HOTFOIL = 3;
var CAUTIONLABEL = 6;
var Rivets = 4;
var screw = 12;
var BLANKPLATE = 8;
var HOLOGRAMPLATE = 9;
var LASERNOPLATE = 10;
var NUMBERPLATE = 11;

var ColorNone = 5;
var SizeNone = 5;



*/

//Common Validation Function 08.05.2025
function markInvalid(selector, message) {
    $(selector).addClass('is-invalid').focus();
    $.jGrowl(message, { sticky: false, theme: 'warning', life: jGrowlLife });
    return false;
}

//Common Confirm Delete Function  08.05.2025
function ConfirmDelete(id, url, _TOKEN, successMsg, errorMsg) {
    return new Promise(function (resolve, reject) {
        $.ajax({
            url: url,
            type: 'POST',
            headers: { "RequestVerificationToken": _TOKEN },
            contentType: 'application/json',
            data: JSON.stringify(id),
            success: function (response) {
                if (response && response.resultdata) {
                    var result = response.resultdata;
                    if (!result.Error && result.Success && result.ID > 0) {
                        Swal.fire({
                            title: "Deleted!",
                            text: successMsg,
                            icon: "success",
                            confirmButtonColor: "#556ee6"
                        });
                        resolve(true);
                    } else {
                        Swal.fire({
                            title: "Error",
                            text: result.Message,
                            icon: "warning",
                            confirmButtonColor: "#556ee6"
                        });
                        resolve(false);
                    }
                } else {
                    resolve(false);
                }
            },
            error: function (xhr) {
                Swal.fire({
                    title: "Error",
                    text: xhr.responseText || "Something went wrong!",
                    icon: "error",
                    confirmButtonColor: "#556ee6"
                });
                resolve(false);
            }
        });
    });
}
//Added on 2025/05/24 by HARIVIGNESH
function SetActionButtons(ID, _CMPermissions) {
    //console.log(_CMPermissions);

    // Create action buttons based on permissions
    const viewBtn = _CMPermissions.HasView ? `
        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
            <a href="javascript:void(0);" onclick="EditData(${ID}, true)" class="btn btn-sm btn-soft-primary">
                <i class="mdi mdi-eye-outline"></i>
            </a>
        </li>
    ` : '';

    const editBtn = _CMPermissions.HasEdit ? `
        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Edit">
            <a href="javascript:void(0);" onclick="EditData(${ID}, false)" class="btn btn-sm btn-soft-info">
                <i class="mdi mdi-pencil-outline"></i>
            </a>
        </li>
    ` : '';

    const deleteBtn = _CMPermissions.HasDelete ? `
        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
            <a href="javascript:void(0);" onclick="DeleteData('${ID}')" class="btn btn-sm btn-soft-danger">
                <i class="mdi mdi-delete-outline"></i>
            </a>
        </li>
    ` : '';

    return `
        <ul class="list-unstyled hstack gap-1 mb-0">
            ${viewBtn}
            ${editBtn}
            ${deleteBtn}
        </ul>`;
}
function getBrowserDimensions() {
    return {
        width: window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth,
        height: window.innerHeight || document.documentElement.clientHeight || document.body.clientHeight
    };
}
function InsertPageAccessAuditLog(_TOKEN, pagedata) {
    //console.log(pagedata);

    const dimensions = getBrowserDimensions();
    pagedata.BrowserPageWidth = dimensions.width;
    pagedata.BrowserPageHeight = dimensions.height;

    $.ajax({
        url: '/GlobalFunctions?handler=InsertPageAccessAuditLog',
        method: 'POST',
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        data: JSON.stringify(pagedata),
        success: function (data) {

        }
    });
}
function convertToShortMonthFormat(inputDate) {
    const shortMonths = ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
        "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];

    const parts = inputDate.split("/"); // ["09", "07", "2025"]
    const day = parts[0];
    const monthIndex = parseInt(parts[1], 10) - 1;
    const year = parts[2];

    return `${day}-${shortMonths[monthIndex]}-${year}`;
}
function GetOEMList(ddlOEM, OEMListUrl, _TOKEN) {
    $.ajax({
        url: OEMListUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            $('#' + ddlOEM).empty();

            $("#" + ddlOEM).append("<option value='0' selected='selected'>--Select OEM--</option>");
            $.each(data.result.Value, function (i, response) {
                $('#' + ddlOEM).append("<option value='" + response.HSRPUserID + "'>" + response.CompanyName + "</option>");
            });

            $("#" + ddlOEM).val(0).change();
        }
    });

    return false;
}
function GetEmbossingStationList(ddlEmbossingStation, EmbossingStationListUrl, _TOKEN) {
    $.ajax({
        url: EmbossingStationListUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            $('#' + ddlEmbossingStation).empty();

            $("#" + ddlEmbossingStation).append("<option value='0' selected='selected'>--Select Embossing Station--</option>");
            $.each(data.result.Value, function (i, response) {
                $('#' + ddlEmbossingStation).append(
                    "<option value='" + response.HSRPUserID + "'>" +
                    response.CompanyName + " - " + response.City +
                    "</option>"
                );
            });

            $("#" + ddlEmbossingStation).val(0).change();
        }
    });

    return false;
}
function GetDealerList(ddlDealer, DealerListUrl, _TOKEN) {
    $.ajax({
        url: DealerListUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            $('#' + ddlDealer).empty();

            $("#" + ddlDealer).append("<option value='0' selected='selected'>--Select Dealer--</option>");
            $.each(data.result.Value, function (i, response) {
                $('#' + ddlDealer).append("<option value='" + response.HSRPUserID + "'>" + response.CompanyName + "</option>");
            });

            $("#" + ddlDealer).val(0).change();
        }
    });

    return false;
}

function GetDealerListByOEMID(ddlDealer, DealerListUrl, _TOKEN, oemID) {
    $.ajax({
        url: DealerListUrl, // your API URL
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        data: { OEMID: oemID }, 
        success: function (data) {
            $('#' + ddlDealer).empty();
            $("#" + ddlDealer).append("<option value='0' selected='selected'>--Select Dealer--</option>");

            $.each(data.result.Value, function (i, response) {
                $('#' + ddlDealer).append("<option value='" + response.HSRPUserID + "'>" + response.CompanyName + "</option>");
            });

            $("#" + ddlDealer).val(0).change();
        }
    });

    return false;
}

function GetOrderTypeList(ddlOrderType, OrderTypeListUrl, _TOKEN) {
    $.ajax({
        url: OrderTypeListUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            $('#' + ddlOrderType).empty();

            $("#" + ddlOrderType).append("<option value='0' selected='selected'>--Select Order Type--</option>");
            $.each(data.result.Value, function (i, response) {
                $('#' + ddlOrderType).append("<option value='" + response.OrderTypeID + "'>" + response.OrderTypeName + "</option>");
            });

            $("#" + ddlOrderType).val(0).change();
        }
    });

    return false;
}

//Added on 2025/10/13 by HARIVIGNESH
function GetLaserNoStockStatusList(ddlStatus, LaserNoStockStatusListUrl, _TOKEN) {
    $.ajax({
        url: LaserNoStockStatusListUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            $('#' + ddlStatus).empty();

            $("#" + ddlStatus).append("<option value='0' selected='selected'>--Select Order Type--</option>");
            $.each(data.result.Value, function (i, response) {
                $('#' + ddlStatus).append("<option value='" + response.HSRPLaserNoStatusID + "'>" + response.LaserNoStatus + "</option>");
            });

            $("#" + ddlStatus).val(0).change();
        }
    });

    return false;
}
function GetVehiclePlateSizeList(ddlSize, VehiclePlateSizeListUrl, _TOKEN) {
    $.ajax({
        url: VehiclePlateSizeListUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            $('#' + ddlSize).empty();

            $("#" + ddlSize).append("<option value='0' selected='selected'>--Select Order Type--</option>");
            $.each(data.result.Value, function (i, response) {
                $('#' + ddlSize).append("<option value='" + response.VehiclePlateSizeID + "'>" + response.VehiclePlateSizeName + "</option>");
            });

            $("#" + ddlSize).val(0).change();
        }
    });

    return false;
}
function GetVehiclePlateColorList(ddlColor, VehiclePlateColorListUrl, _TOKEN) {
    $.ajax({
        url: VehiclePlateColorListUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            $('#' + ddlColor).empty();
            $('#' + ddlColor).append("<option value='0'>--Select Vehicle Plate Color--</option>");

            $.each(data.result?.Value || [], function (i, response) {
                $('#' + ddlColor).append(`<option value="${response.VehiclePlateColorID}">${response.VehiclePlateColorName}</option>`);
            });

            $('#' + ddlColor).val(0);
        }
    });

    return false;
}

function GetColorList(ddlColor, ColorListUrl, _TOKEN) {
    $.ajax({
        url: ColorListUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            $('#' + ddlColor).empty();

            $("#" + ddlColor).append("<option value='0' selected='selected'>--Select Color--</option>");
            $.each(data.result.Value, function (i, response) {
                $('#' + ddlColor).append("<option value='" + response.ColorID + "'>" + response.ColorName + "</option>");
            });

            $("#" + ddlColor).val(0).change();
        }
    });

    return false;
}
function GetSizeList(ddlSize, SizeListUrl, _TOKEN) {
    $.ajax({
        url: SizeListUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            $('#' + ddlSize).empty();

            $("#" + ddlSize).append("<option value='0' selected='selected'>--Select Size--</option>");
            $.each(data.result.Value, function (i, response) {
                $('#' + ddlSize).append("<option value='" + response.SizeID + "'>" + response.SizeName + "</option>");
            });

            $("#" + ddlSize).val(0).change();
        }
    });

    return false;
}
function GetApplicationList(ddlApplication, ApplicationListUrl, _TOKEN) {
    $.ajax({
        url: ApplicationListUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            $('#' + ddlApplication).empty();

            $("#" + ddlApplication).append("<option value='0' selected='selected'>--Select Application--</option>");
            $.each(data.result.Value, function (i, response) {
                $('#' + ddlApplication).append("<option value='" + response.ApplicationID + "'>" + response.ApplicationName + "</option>");
            });

            $("#" + ddlApplication).val(0).change();
        }
    });

    return false;
}

function GetCourierList(ddlCourier, CourierListUrl, _TOKEN) {
    $.ajax({
        url: CourierListUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            $('#' + ddlCourier).empty();

            $("#" + ddlCourier).append("<option value='0' selected='selected'>--Select Courier--</option>");
            $.each(data.result.Value, function (i, response) {
                $('#' + ddlCourier).append("<option value='" + response.CourierID + "'>" + response.CourierName + "</option>");
            });

            $("#" + ddlCourier).val(0).change();
        }
    });

    return false;
}
function PrintReportByID(ReportURL, FilterByIDColumnName, FilterByIDValue) {
    // Show loading indicator
    Swal.fire({
        title: 'Generating PDF...',
        text: 'Please wait while the Report is being prepared.',
        allowOutsideClick: false,
        showConfirmButton: false,
        willOpen: () => {
            Swal.showLoading();
        }
    });

    $.ajax({
        url: ReportURL,
        type: 'GET',
        data: { [FilterByIDColumnName]: FilterByIDValue },
        xhrFields: {
            responseType: 'blob'
        },
        success: function (data, status, xhr) {
            Swal.close(); // Close loading dialog

            // Create blob from PDF data
            var blob = new Blob([data], { type: 'application/pdf' });

            // Extract custom filename from Content-Disposition header
            var fileName = "DummyFileName.pdf"; // Default filename
            var disposition = xhr.getResponseHeader('Content-Disposition');

            if (disposition) {
                var matches = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/.exec(disposition);
                if (matches != null && matches[1]) {
                    fileName = matches[1].replace(/['"]/g, '');
                }
            }

            // Create object URL for the blob
            var blobUrl = URL.createObjectURL(blob);

            // Method 1: Open in new tab/window (for viewing)
            var newWindow = window.open(blobUrl, '_blank');
            if (newWindow) {
                newWindow.focus();

                // Clean up the blob URL after the window loads
                newWindow.onload = function () {
                    setTimeout(() => {
                        URL.revokeObjectURL(blobUrl);
                    }, 1000);
                };
            }

            // Method 2: Trigger download with custom filename
            var downloadLink = document.createElement('a');
            downloadLink.href = blobUrl;
            downloadLink.download = fileName;
            downloadLink.style.display = 'none';

            // Append to body, click, and remove
            document.body.appendChild(downloadLink);
            downloadLink.click();
            document.body.removeChild(downloadLink);

            // Clean up blob URL after download
            setTimeout(() => {
                URL.revokeObjectURL(blobUrl);
            }, 1000);
        },
        error: function (xhr, status, error) {
            Swal.close(); // Close loading dialog

            var errorMessage = "Could not generate PDF.";

            // Try to extract error message from response
            if (xhr.response) {
                try {
                    var reader = new FileReader();
                    reader.onload = function () {
                        var responseText = reader.result;

                        // Try to parse as JSON first
                        try {
                            var errorObj = JSON.parse(responseText);
                            errorMessage = errorObj.message || errorObj.error || responseText;
                        } catch (e) {
                            // If not JSON, use as plain text
                            errorMessage = responseText;
                        }

                        Swal.fire({
                            title: "Error",
                            text: errorMessage,
                            icon: "error",
                            confirmButtonColor: "#556ee6"
                        });
                    };

                    var errorBlob = new Blob([xhr.response], { type: 'text/plain' });
                    reader.readAsText(errorBlob);
                } catch (e) {
                    Swal.fire({
                        title: "Error",
                        text: `Failed to generate PDF. Status: ${xhr.status}`,
                        icon: "error",
                        confirmButtonColor: "#556ee6"
                    });
                }
            } else {
                Swal.fire({
                    title: "Error",
                    text: errorMessage,
                    icon: "error",
                    confirmButtonColor: "#556ee6"
                });
            }
        }
    });
}
function GetGlobalRoleIDByPageID(PageID, ddlApplication, ddlUserRole) {
    $.ajax({
        url: GetGlobalRoleIDByPageIDUrl,
        type: 'GET',
        data: { PageID: PageID },
        success: function (data) {
            if (data.result.Value) {
                var data = data.result.Value;
                $("#" + ddlApplication).val(data.ApplicationID).change();
                $("#" + ddlUserRole).val(data.RoleID).change();

                $("#" + ddlApplication).prop("disabled", true);
                $("#" + ddlUserRole).prop("disabled", true);
            }
        },
        error: function (xhr, status, error) {
            Swal.close(); // Close loading dialog

            var errorMessage = "Could not generate PDF.";

            // Try to extract error message from response
            if (xhr.response) {
                try {
                    var reader = new FileReader();
                    reader.onload = function () {
                        var responseText = reader.result;

                        // Try to parse as JSON first
                        try {
                            var errorObj = JSON.parse(responseText);
                            errorMessage = errorObj.message || errorObj.error || responseText;
                        } catch (e) {
                            // If not JSON, use as plain text
                            errorMessage = responseText;
                        }

                        Swal.fire({
                            title: "Error",
                            text: errorMessage,
                            icon: "error",
                            confirmButtonColor: "#556ee6"
                        });
                    };

                    var errorBlob = new Blob([xhr.response], { type: 'text/plain' });
                    reader.readAsText(errorBlob);
                } catch (e) {
                    Swal.fire({
                        title: "Error",
                        text: `Failed to generate PDF. Status: ${xhr.status}`,
                        icon: "error",
                        confirmButtonColor: "#556ee6"
                    });
                }
            } else {
                Swal.fire({
                    title: "Error",
                    text: errorMessage,
                    icon: "error",
                    confirmButtonColor: "#556ee6"
                });
            }
        }
    });
}
function GetGlobalPageList(roleID, ddlPageList) {
    $.ajax({
        url: GlobalPageControlByRoleIDURL,
        type: 'GET',
        data: { RoleID: roleID },
        async: false,
        success: function (data) {
            if (data) {
                $('#' + ddlPageList).empty();

                $("#" + ddlPageList).append("<option value='0' selected='selected'>--Select Landing Page--</option>");
                $.each(data, function (i, result) {
                    $('#' + ddlPageList).append('<option value="' + result.Value + '">' + result.Text + '</option>');
                });

            }
        },
        error: function (xhr, status, error) {
            Swal.close(); // Close loading dialog

            var errorMessage = "Could not generate PDF.";

            // Try to extract error message from response
            if (xhr.response) {
                try {
                    var reader = new FileReader();
                    reader.onload = function () {
                        var responseText = reader.result;

                        // Try to parse as JSON first
                        try {
                            var errorObj = JSON.parse(responseText);
                            errorMessage = errorObj.message || errorObj.error || responseText;
                        } catch (e) {
                            // If not JSON, use as plain text
                            errorMessage = responseText;
                        }

                        Swal.fire({
                            title: "Error",
                            text: errorMessage,
                            icon: "error",
                            confirmButtonColor: "#556ee6"
                        });
                    };

                    var errorBlob = new Blob([xhr.response], { type: 'text/plain' });
                    reader.readAsText(errorBlob);
                } catch (e) {
                    Swal.fire({
                        title: "Error",
                        text: `Failed to generate PDF. Status: ${xhr.status}`,
                        icon: "error",
                        confirmButtonColor: "#556ee6"
                    });
                }
            } else {
                Swal.fire({
                    title: "Error",
                    text: errorMessage,
                    icon: "error",
                    confirmButtonColor: "#556ee6"
                });
            }
        }
    });
}
function HsrpGroupAction(ID) {
    return `
        <div class="btn-group-vertical" role="group" aria-label="Vertical button group">
            <div class="btn-group" role="group">
                <button id="btnGroupVerticalDrop${ID}" type="button"
                    class="btn btn-sm btn-outline-pink dropdown-toggle"
                    data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="bx bx-list-check me-1"></i> Actions
                </button>
                <ul class="dropdown-menu shadow-sm" aria-labelledby="btnGroupVerticalDrop${ID}">
                 <li>
                        <a class="dropdown-item d-flex align-items-center btn-assign" href="javascript:;" data-orderid="${ID}">
                            <i class=" bx bx-chevrons-right text-primary me-2"></i> Assign
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-update" href="javascript:;" data-orderid="${ID}">
                            <i class="bx bx-pencil text-info me-2"></i> Update
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-print-order" href="javascript:;" onclick="PrintOrderReport(${ID})">
                            <i class="bx bx-printer text-success me-2"></i> Print Order
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-print-tlp" href="javascript:;" onclick="PrintTLPSticker(${ID})">
                            <i class="bx bx-file text-warning me-2"></i> Print TLP
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-track-status" href="javascript:;" data-orderid="${ID}">
                            <i class="bx bx-map text-danger me-2"></i> Track Status
                        </a>
                    </li>
                   
                </ul>
            </div>
        </div>`;
}

function HsrpGroupViewAction(ID) {
    return `
        <div class="btn-group-vertical" role="group" aria-label="Vertical button group">
            <div class="btn-group" role="group">
                <button id="btnGroupVerticalDrop${ID}" type="button"
                    class="btn btn-sm btn-outline-pink dropdown-toggle"
                    data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="bx bx-list-check me-1"></i> Actions
                </button>
                <ul class="dropdown-menu shadow-sm" aria-labelledby="btnGroupVerticalDrop${ID}">
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-print-order" href="javascript:;" onclick="PrintOrderReport(${ID})">
                            <i class="bx bx-printer text-success me-2"></i> Print Order
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-print-tlp" href="javascript:;" onclick="PrintTLPSticker(${ID})">
                            <i class="bx bx-file text-warning me-2"></i> Print TLP
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-track-status" href="javascript:;" data-orderid="${ID}">
                            <i class="bx bx-map text-danger me-2"></i> Track Status
                        </a>
                    </li>
                   
                </ul>
            </div>
        </div>`;
}
function GetRectificaionReason(ddlRectify, GetRectificaionReasonUrl, _TOKEN) {
    $.ajax({
        url: GetRectificaionReasonUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            $('#' + ddlRectify).empty();
            $('#' + ddlRectify).append("<option value='0'>--Select Laser Serial No--</option>");

            $.each(data.result?.Value || [], function (i, response) {
                $('#' + ddlRectify).append(`<option value="${response.HSRPOrderRectificationReasoniID}">${response.HSRPOrderRectificationReason}</option>`);
            });

            $('#' + ddlRectify).val(0);
        }
    });

    return false;
}
function getOrderHistory(HSrpID) {
    $.ajax({
        url: TimeLineListDataUrl,
        type: "GET",
        headers: { "RequestVerificationToken": _TOKEN },
        data: { orderId: HSrpID },

        success: function (response) {

            if (!response || !response.result || !response.result.Value) return;

            const data = response.result.Value;

            if (data.length === 0) return;

            const timelineContainer = $('.status-timeline');
            timelineContainer.empty();

            // ⭐ Read dynamic values from first row
            const completedStatusID = data[0].CompletedStatusID;
            const nextPendingStatusID = data[0].NextPendingStatusID;
            const nextPendingDescription = data[0].NextPendingDescription;
            const nextPendingIconCode = data[0].NextPendingIconCode;

            // ⭐ Add next pending status if exists
            if (nextPendingStatusID) {
                data.push({
                    HSRPOrderStatusLogID: 0,     // not needed
                    OrderID: HSrpID,
                    OrderStatusID: nextPendingStatusID,
                    Description: nextPendingDescription,
                    IconCode: nextPendingIconCode,
                    LastUpdatedBy: "",
                    LastUpdateDate: "",
                    IsFuture: true      
                });
            }

            // ⭐ Sort final data by OrderStatusID ASC
            // Sort by OrderStatusID in DESC
            data.sort((a, b) => b.OrderStatusID - a.OrderStatusID);

            data.forEach(item => {

                // ⭐ Completed Status
                let iconHtml = '<i class="bx bxs-check-circle text-success font-size-18"></i>';

                // ⭐ Pending / Future Stage
                if (item.IsFuture) {
                    iconHtml = '<i class="bx bxs-right-arrow-circle bx-fade-right text-primary font-size-18"></i>';
                }

                // ⭐ Active = next pending status
                const isActive = item.OrderStatusID == nextPendingStatusID;

                const date = item.LastUpdateDate ? ISTtoLocal(item.LastUpdateDate) : '';

                const li = $(`
                    <li class="event-list ${isActive ? 'active' : ''}">
                        <div class="event-timeline-dot">
                            ${iconHtml}
                        </div>

                        <div class="d-flex align-items-start mt-3 mb-2">
                            <div class="flex-shrink-0 me-2">

                                <h5 class="font-size-16 mb-0 d-flex align-items-center">
                                    <i class="${item.IconCode} font-size-18"></i>
                                    <span class="ms-3 fw-semibold">${item.Description}</span>
                                    ${isActive ? `
                                  <span style="background:#ff4fa3; color:white; padding:2px 6px; border-radius:4px; font-size:12px;" class="ms-3">
                                        Current Status
                                    </span>
                                ` : ''}
                                </h5>
                                <h6 class="text-primary ms-4 mt-2 fw-normal">${date}</h6>
                            </div>
                        </div>
                    </li>
                `);

                timelineContainer.append(li);

            });
        },
        error: function (xhr) {
            console.error("Error loading timeline", xhr);
        }
    });
}
function getInvoiceDetails(orderId) {
    $.ajax({
        url: InvoiceDetailsUrl,
        type: "GET",
        headers: { "RequestVerificationToken": _TOKEN },
        data: { orderId: orderId },

        success: function (response) {

            if (!response || !response.result.Success ||
                !response.result.Value || response.result.Value.length === 0) {

                setField("InvoiceNo", "-");
                setField("sInvoiceDate", "-");
                setField("InvoiceNetAmount", "-");
                return;
            }

            const item = response.result.Value[0];  

            setField("InvoiceNo", item.InvoiceNo ?? "-");
            setField("sInvoiceDate", item.sInvoiceDate ?? "-");
            setField("InvoiceNetAmount", item.Amount ?? "-");
        },

        error: function (xhr) {
            console.error("Error loading invoice details", xhr);
        }
    });
}
function setField(fieldName, value) {
    const el = $(`[data-field='${fieldName}']`);

    if (el.is("a")) {
        if (!value || value === "-") {
            el.attr("href", "#");
            el.text("No POD");
            el.removeClass("image-popup text-primary")
                .addClass("text-muted");
        } else {
            el.attr("href", value);
            el.text("View POD");
            el.addClass("image-popup text-primary")
                .removeClass("text-muted");
        }
        return;
    }
    el.text(value || "-");
}
function getShipmentAndDeliveryDetails(orderId) {
    $.ajax({
        url: ShipmentAndDeliveryDetailsUrl,
        type: "GET",
        headers: { "RequestVerificationToken": _TOKEN },
        data: { orderId: orderId },

        success: function (response) {

            if (!response || !response.result.Success ||
                !response.result.Value || response.result.Value.length === 0) {

                setField("ModeOfTransport", "-");
                setField("CourierName", "-");
                setField("CollectingPerson", "-");
                setField("ConsignmentDetails", "-");
                setField("ShipmentDate", "-");
                setField("sDeliveredDate", "-");
                setField("DocketNo", "-");
                return;
            }

            const item = response.result.Value[0];  

            setField("ModeOfTransport", item.ModeOfTransport ?? "-");
            setField("CourierName", item.CourierName ?? "-");
            setField("CollectingPerson", item.CollectingPerson ?? "-");
            setField("ConsignmentDetails", item.ConsignmentDetails ?? "-");
            setField("ShipmentDate", item.ShipmentDate ?? "-");
            setField("sDeliveredDate", item.sDeliveredDate ?? "-");
            setField("DocketNo", item.DocketNo ?? "-");
            setField("UploadImageUrl", item.UploadImageUrl ?? "-");
            $('.image-popup').magnificPopup({
                type: 'image',
                closeOnContentClick: true,
                mainClass: 'mfp-img-mobile',
                image: {
                    verticalFit: true
                }
            });
        },

        error: function (xhr) {
            console.error("Error loading invoice details", xhr);
        }
    });
}
// 2025.12.04
var _CMAccessDeined = "You don't have permission. Please contact Administrator";
function HsrpGroupActionForDeliveryAcknowledgementOrders(ID) {
    return `
        <div class="btn-group-vertical" role="group" aria-label="Vertical button group">
            <div class="btn-group" role="group">
                <button id="btnGroupVerticalDrop${ID}" type="button"
                    class="btn btn-sm btn-outline-pink dropdown-toggle"
                    data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="bx bx-list-check me-1"></i> Actions
                </button>
                <ul class="dropdown-menu shadow-sm" aria-labelledby="btnGroupVerticalDrop${ID}">
                    <li>
                       <a class="dropdown-item d-flex align-items-center btn-assign" href="javascript:;" onclick="GetHSRPDataByID(${ID}, false)">
                          <i class="bx bx-search-alt text-primary me-2"></i> Fitment Upload
                       </a>
                    </li>
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-print-order" href="javascript:;" onclick="PrintOrderReport(${ID})">
                            <i class="bx bx-printer text-success me-2"></i> Print Order
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-print-tlp" href="javascript:;" onclick="PrintTLPSticker(${ID})">
                            <i class="bx bx-file text-warning me-2"></i> Print TLP
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-track-status" href="javascript:;" data-orderid="${ID}">
                            <i class="bx bx-map text-danger me-2"></i> Track Status
                        </a>
                    </li>                   
                </ul>
            </div>
        </div>`;
}
function GetOEMByEmbossingStationList(ddlOEMFilter, OEMListByEmbossingStationUrl, _TOKEN, USERID) {
    $.ajax({
        url: OEMListByEmbossingStationUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        data: { USERID: USERID }, 
        success: function (data) {
            $('#' + ddlOEMFilter).empty();

            $("#" + ddlOEMFilter).append("<option value='0' selected='selected'>--Select OEM--</option>");
            $.each(data.result, function (i, response) {
                $('#' + ddlOEMFilter).append(
                    "<option value='" + response.HSRPUserID + "'>" +
                    response.CompanyName + " - " + response.City +
                    "</option>"
                );
            });

            $("#" + ddlOEMFilter).val(0).change();
        }
    });
    return false;
}
function GetEmbossingStationByUser(ddlEmbossingStationFilter, EmbossingStationByUserUrl, _TOKEN, USERID) {
    $.ajax({
        url: EmbossingStationByUserUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        data: { USERID: USERID },
        success: function (data) {
            $('#' + ddlEmbossingStationFilter).empty();
            $("#" + ddlEmbossingStationFilter).append("<option value='0' selected='selected'>--Select EmbossinStation--</option>");
            $.each(data.result, function (i, response) {
                $('#' + ddlEmbossingStationFilter).append(
                    "<option value='" + response.HSRPUserID + "'>" +
                    response.CompanyName + " - " + response.City +
                    "</option>"
                  
                );
            });
            $("#" + ddlEmbossingStationFilter).val(0).change();
        }
    });
    return false;
}  


function HsrpGroupActionForViewDeliveryAcknowledgementOrders(ID) {
    return `
        <div class="btn-group-vertical" role="group" aria-label="Vertical button group">
            <div class="btn-group" role="group">
                <button id="btnGroupVerticalDrop${ID}" type="button"
                    class="btn btn-sm btn-outline-pink dropdown-toggle"
                    data-bs-toggle="dropdown" aria-expanded="false">
                    <i class="bx bx-list-check me-1"></i> Actions
                </button>
                <ul class="dropdown-menu shadow-sm" aria-labelledby="btnGroupVerticalDrop${ID}">               
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-print-order" href="javascript:;" onclick="PrintOrderReport(${ID})">
                            <i class="bx bx-printer text-success me-2"></i> Print Order
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-print-tlp" href="javascript:;" onclick="PrintTLPSticker(${ID})">
                            <i class="bx bx-file text-warning me-2"></i> Print TLP
                        </a>
                    </li>
                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-track-status" href="javascript:;" data-orderid="${ID}">
                            <i class="bx bx-map text-danger me-2"></i> Track Status
                        </a>
                    </li>                   
                </ul>
            </div>
        </div>`;
}

function GetPartNumberByOEM(CONTROL, URL, TOKEN, OEMID) {

    $.ajax({
        url: URL,
        type: 'GET',
        headers: { "RequestVerificationToken": TOKEN },
        data: { OEMID: OEMID },
        async:false,
        success: function (response) {

            $('#' + CONTROL).empty();
            $('#' + CONTROL).append("<option value='0'>-- Select Part Number --</option>");

            if (response && response.result && response.result.Value) {
                $.each(response.result.Value, function (i, item) {
                    $('#' + CONTROL).append(
                        `<option value="${item.HSRPPartNumberID}">
                            ${item.PartNumber}
                        </option>`
                    );
                });
            }

            $('#' + CONTROL).trigger("change");
        },
        error: function (xhr) {
            Swal.fire("Error", xhr.responseText, "error");
        }
    });
}

function HsrpGroupActionForAllOrder(ID, OrderStatusID) {

    let updateButton = '';

    // Show Update ONLY if status is NOT 9
    if (OrderStatusID !== 9) {
        updateButton = `
            <li>
                <a class="dropdown-item d-flex align-items-center btn-update"
                   href="javascript:;" data-orderid="${ID}">
                    <i class="bx bx-pencil text-info me-2"></i> Update
                </a>
            </li>`;
    }

    return `
        <div class="btn-group-vertical" role="group">
            <div class="btn-group" role="group">
                <button type="button"
                    class="btn btn-sm btn-outline-pink dropdown-toggle"
                    data-bs-toggle="dropdown">
                    <i class="bx bx-list-check me-1"></i> Actions
                </button>

                <ul class="dropdown-menu shadow-sm">

                    ${updateButton}

                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-print-order"
                           href="javascript:;" onclick="PrintOrderReport(${ID})">
                            <i class="bx bx-printer text-success me-2"></i> Print Order
                        </a>
                    </li>

                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-print-tlp"
                           href="javascript:;" onclick="PrintTLPSticker(${ID})">
                            <i class="bx bx-file text-warning me-2"></i> Print TLP
                        </a>
                    </li>

                    <li>
                        <a class="dropdown-item d-flex align-items-center btn-track-status"
                           href="javascript:;" data-orderid="${ID}">
                            <i class="bx bx-map text-danger me-2"></i> Track Status
                        </a>
                    </li>

                </ul>
            </div>
        </div>`;
}

function GetEmbossingStationByHSRPOnlineOrderID(ddlEmbossingStation, EmbossingStationByHSRPOnlineOrderIDUrl, _TOKEN,OnlineOrderID) {
    $.ajax({
        url: EmbossingStationByHSRPOnlineOrderIDUrl,
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        data: { OnlineOrderID: OnlineOrderID }, 
        success: function (data) {
            $('#' + ddlEmbossingStation).empty();

            $("#" + ddlEmbossingStation).append("<option value='0' selected='selected'>--Select Embossing Station--</option>");
            $.each(data.result, function (i, response) {
                $('#' + ddlEmbossingStation).append(
                    "<option value='" + response.EmbossingStationID + "'>" +
                    response.EmbossingStationName +
                    "</option>"
                );
            });

            $("#" + ddlEmbossingStation).val(0).change();
        }
    });

    return false;
}


function SetActionButtonsForUser(ID, _CMPermissions) {
    //console.log(_CMPermissions);

    // Create action buttons based on permissions
    const viewBtn = _CMPermissions.HasView ? `
        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="View">
            <a href="javascript:void(0);" onclick="EditData(${ID}, true)" class="btn btn-sm btn-soft-primary">
                <i class="mdi mdi-eye-outline"></i>
            </a>
        </li>
    ` : '';

    const editBtn = _CMPermissions.HasEdit ? `
        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Edit">
            <a href="javascript:void(0);" onclick="EditData(${ID}, false)" class="btn btn-sm btn-soft-info">
                <i class="mdi mdi-pencil-outline"></i>
            </a>
        </li>
    ` : '';

    const deleteBtn = _CMPermissions.HasDelete ? `
        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Delete">
            <a href="javascript:void(0);" onclick="DeleteData('${ID}')" class="btn btn-sm btn-soft-danger">
                <i class="mdi mdi-delete-outline"></i>
            </a>
        </li>
    ` : '';

    // Show Change Password button only for ManageUser page (where ChangeUserPassword function exists)
    // and only for super admin (has Add, Edit, and Delete permissions)
    const changePasswordBtn = (typeof ChangeUserPassword !== 'undefined' && _CMPermissions.HasAdd && _CMPermissions.HasEdit && _CMPermissions.HasDelete) ? `
        <li data-bs-toggle="tooltip" data-bs-placement="top" data-bs-title="Change Password">
            <a href="javascript:void(0);" onclick="ChangeUserPassword(${ID})" class="btn btn-sm btn-soft-warning">
                <i class="mdi mdi-key-change"></i>
            </a>
        </li>
    ` : '';

    return `
        <ul class="list-unstyled hstack gap-1 mb-0">
            ${viewBtn}
            ${editBtn}
            ${deleteBtn}
            ${changePasswordBtn}
        </ul>`;
}
