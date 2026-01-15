//Added on 2024.11.01
var gRoleConfiguration = [];
//Remove Privilege
const removedPrivileges = [];

$(function () {
    pLoadingSetup(false);
    ActionAdd = 1;
    ActionUpdate = 1;
    ActionDelete = 1;
    ActionView = 1;

    $("#divRecordList").show();
    $("#divAddEditView").hide();

    getRecordList();
    pLoadingSetup(true);
});

$('#btnRefresh').on('click', function () {
    getRecordList();
    return false;
});
$('#btnClose').on('click', function () {
    $("#divRecordList").show();
    $("#divAddEditView").hide();

    getRecordList();
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
            url: ListDataURL,
            "type": "GET",
            "data": function (d) {
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
                    return meta.row + 1; // Display row number (S. No.)
                },
                orderable: false, // Disable sorting for this column},
                "width": "5%",
                "orderable": false
            },
            { "data": "RoleName", "orderable": true },
            { "data": "Description", "orderable": true },
            {
                "data": null, "orderable": true, "width": "10%", "className": "text-center text-light",
                "render": function (data, type, row) {
                    return `<span class="${row.Colorcode}">${row.ApplicationName}</span>`
                },
            },
            {
                "data": "IsActive",
                "render": function (data, type, row) {
                    if (data) {
                        return '<span class="badge bg-success">Active</span>';
                    } else {
                        return '<span class="badge bg-danger">Inactive</span>';
                    }
                },
                "width": "10%",
                "orderable": false
            },
            {
                data: null,
                bSortable: false,
                render: function (data, type, row) {
                    return `<button type="button" class="btn btn-outline-info btn-sm btn-rounded waves-effect waves-light Configure" onclick="EditData(${row.RoleID})" title="Click here to Configure">
                                <i class="bx bxs-check-shield align-middle me-2 font-size-16"></i>Configure
                            </button>`;
                },
                "width": "5%",
                "orderable": false
            },
        ]
    });

    $(".dataTables_paginate").addClass("pagination-rounded");
}

$(".CloseButton").click(function () {
    $("#divRecordList").show();
    $("#divAddEditView").hide();

    $("#divPageAccessList").empty();
    return false;
});

function EditData(id) {
    if (ENABLE_VERBOSE_Logging) console.log(id);

    $.ajax({
        url: GetRoleByIDDataUrl,
        type: 'GET',
        contentType: 'application/json',
        data: { ID: id },
        success: function (response) {
            var roledata = response.data;
            $("#hdnRoleID").val(roledata.RoleID);
            $("#hdnRoleModuleID").val(roledata.RoleModuleIDs);
            $("#divCardTitle").html("<i class='fas fa-edit me-2'></i>Configure " + roledata.RoleName);

            $("#divRecordList").hide();
            $("#divAddEditView").show();

            $("#divMenuLayout").empty();
            GetRoleConfigurationByRoleID(roledata.RoleID);
            GetMenuLayout($("#hdnRoleModuleID").val());
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);

            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}


$("#btnSave").on('click', function () {
    const groupedPrivileges = {};

    $('.privilege-checkbox:checked').each(function () {
        const pageId = $(this).data('page-id');
        const privilege = $(this).data('privilege');

        if (!groupedPrivileges[pageId]) {
            groupedPrivileges[pageId] = {
                PageControlID: pageId,
                RoleConfigurationID: 0,
                RoleID: parseInt($("#hdnRoleID").val()),
                IsAccess: false,
                IsAdd: false,
                IsEdit: false,
                IsDelete: false,
                IsView: false,
                IsExport: false
            };
        }

        // Mark the privilege as true for the PageID
        groupedPrivileges[pageId][privilege] = true;
    });

    // Convert the groupedPrivileges object to an array if needed
    const resultArray = Object.values(groupedPrivileges);
    SaveandUpdateRoleConfigration(resultArray);

    return false;
});

function groupRemovedPrivileges(data) {
    const grouped = {};
    data.forEach(item => {
        if (!grouped[item.pageId]) {
            grouped[item.pageId] = [];
        }
        grouped[item.pageId].push(item.privilege);
    });

    return Object.keys(grouped).map(pageId => ({
        pageId: parseInt(pageId),
        privileges: grouped[pageId]
    }));
}
function SaveandUpdateRoleConfigration(rolelist) {    
    $.ajax({
        type: 'POST',
        url: SaveUpdateDataUrl,
        headers: { "RequestVerificationToken": _TOKEN },
        contentType: 'application/json',
        dataType: "json",     
        data: JSON.stringify(rolelist),
        success: function (response) {
            console.log(response);
            if (response.success && !response.isExists) {
                Swal.fire({ title: "Updated!", text: response.message, icon: "success", confirmButtonColor: "#556ee6" });

                GetRoleConfigurationByRoleID($("#hdnRoleID").val());
            } else {
                $.jGrowl("Failed to save configuration.", { sticky: false, theme: 'error', life: jGrowlLife });
            }
        },
        error: function (error) {
            $.jGrowl("Error while saving configuration.", { sticky: false, theme: 'error', life: jGrowlLife });
        }
    });
}

//Added on 2024.12.15
function GetRoleConfigurationByRoleID(iRoleID) {
    gRoleConfiguration = [];

    if (iRoleID > 0) {
        $.ajax({
            url: GetRoleConfigurationbyRoleIDUrl, // Use the variable defined in Razor page
            type: 'get',
            headers: { "RequestVerificationToken": _TOKEN },
            data: { RoleID: iRoleID },
            async: false,
            success: function (data) {
                //console.log(data);
                $.each(data, function (index, roledata) {
                    var objTemp = new Object();
                    objTemp.sNO = index + 1;
                    objTemp.SNo = objTemp.sNO;
                    objTemp.RoleID = roledata.RoleID;
                    objTemp.RoleConfigurationID = roledata.RoleConfigurationID;
                    objTemp.MenuControlID = roledata.MenuControlID;
                    objTemp.MenuName = roledata.MenuName;
                    objTemp.PageControlID = roledata.PageControlID;
                    objTemp.PageName = roledata.PageName;
                    objTemp.IsAccess = roledata.IsAccess;
                    objTemp.IsView = roledata.IsView;
                    objTemp.IsAdd = roledata.IsAdd;
                    objTemp.IsEdit = roledata.IsEdit;
                    objTemp.IsDelete = roledata.IsDelete;
                    objTemp.IsExport = roledata.IsExport;

                    objTemp.StatusFlag = "";
                    gRoleConfiguration.push(objTemp);
                });
            }
        });
    }
}
function GetMenuLayout(RoleModuleIDs) {
    if (RoleModuleIDs == null || RoleModuleIDs.length < 0) return false;

    var ModuleIDs = RoleModuleIDs.split(",").map(Number);

    $("#divMenuLayout").empty();
    $.ajax({
        url: GetMenuLayoutUrl, // Use the variable defined in Razor page
        type: 'get',
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (data) {
            console.log(data);
            var _layout = "";

            _layout += "<div class='row'>";
            _layout += "<div class='col-md-2'>";
            _layout += "<div class='nav flex-column nav-pills' id='v-pills-tab-" + "1" + "' role='tablist' aria-orientation='vertical'>";
            var TabCount = 1;
            $.each(data, function (index, menuGroup) {
                //Tab Header
                $.each(menuGroup.MenuControlList, function (index, menuItem) {
                    if (ModuleIDs.includes(menuItem.ModuleID)) { //Allow only 
                        _layout += "<a class='nav-link mb-2 " + (TabCount == 1 ? "active" : "") + "' id='v-pills-" + menuItem.MenuControlID + "-tab'";
                        _layout += " data-bs-toggle='pill' href='#v-pills-" + menuItem.MenuControlID + "' role='tab'";
                        _layout += " aria-controls='v-pills-" + menuItem.MenuControlID + "' aria-selected='" + (TabCount == 1 ? "true" : "false") + "'";
                        _layout += (TabCount == 1 ? "" : " tabindex='-1'") + "><i class='" + menuItem.MenuIcon + " font-size-20 align-items-center me-2'></i>" + menuItem.MenuDisplayName + "</a>";

                        TabCount++;
                    }
                });
            });
            _layout += "</div>";
            _layout += "</div>";

            _layout += "<div class='col-md-10'>";
            _layout += "<div class='tab-content text-muted mt-4 mt-md-0' id='v-pills-" + "1" + "-tabcontent'>";
            var TabContent = 1;
            var sColorCode = "bg-secondary bg-gradient text-white";

            $.each(data, function (index, menuGroup) {
                $.each(menuGroup.MenuControlList, function (index, menuItem) {
                    if (ModuleIDs.includes(menuItem.ModuleID)) {
                        _layout += "<div class='tab-pane fade" + (TabContent == 1 ? " active show" : "") + "'";
                        _layout += " id='v-pills-" + menuItem.MenuControlID + "' role='tabpanel'";
                        _layout += " aria-labelledby='v-pills-" + menuItem.MenuControlID + "-tab'>";

                        _layout += "<div class='row'><div class='col-12'>";
                        _layout += " <div class='page-title-box d-sm-flex float-end'>";
                        _layout += " <div class='page-title-right'>";
                        _layout += " <ol class='breadcrumb m-0'>";
                        _layout += " <li class='breadcrumb-item'><a href='#'>Home</a></li>";
                        _layout += " <li class='breadcrumb-item'><a href='#'>" + menuGroup.MenuGroupName + "</a></li>";
                        _layout += " <li class='breadcrumb-item active'><a href='#'>" + menuItem.MenuDisplayName + "</a></li>";
                        _layout += " </ol>";
                        _layout += " </div>";
                        _layout += " </div>";
                        _layout += "</div></div>"; //row + col

                        _layout += " <div class='table-responsive'>";
                        //_layout += " <table id='rolePrivilegesTable" + menuItem.MenuControlID + "' class='table table-sm'>";
                        _layout += " <table id='rolePrivilegesTable' class='table table-bordered'>";
                        _layout += " <thead class=''><tr>";
                        _layout += " <th class='select-all-col " + sColorCode + "'>Select</th>";
                        _layout += " <th class='page-name-col " + sColorCode + "'>Page</th>";
                        _layout += " <th class='privilege-col " + sColorCode + "'>Access</th>";
                        _layout += " <th class='privilege-col " + sColorCode + "'>Create</th>";
                        _layout += " <th class='privilege-col " + sColorCode + "'>Update</th>";
                        _layout += " <th class='privilege-col " + sColorCode + "'>Delete</th>";
                        _layout += " <th class='privilege-col " + sColorCode + "'>View</th>";
                        _layout += " <th class='privilege-col " + sColorCode + "'>Export</th>";
                        _layout += " </tr><tbody>";
                        $.each(menuItem.PageControlList, function (index, page) {
                            _layout += "<tr>";

                            _layout += "<td class='text-center'>";
                            _layout += "<div class='form-check-inline form-check-success'>";
                            _layout += " <input type='checkbox' name='chkSelectAll' class='form-check-input select-all' id='chkSelectAll_" + page.PageControlID + "' data-page-id='" + page.PageControlID + "' />";
                            _layout += " <label class='custom-control-label' for='chkSelectAll_" + page.PageControlID + "'></label>";
                            _layout += "</div>";
                            _layout += "</td>";

                            _layout += "<td class='page-name'>" + page.PageName + "</td>";

                            //Access
                            _layout += "<td class='text-center'>";
                            _layout += "<div class='form-check-inline form-check-success'>";
                            _layout += " <input type='checkbox' name='chkPageAccess' class='form-check-input privilege-checkbox' id='chkPageAccess_" + page.PageControlID + "' data-privilege='IsAccess' data-page-id='" + page.PageControlID + "' />";
                            _layout += " <label class='custom-control-label' for='chkPageAccess_" + page.PageControlID + "'></label>";
                            _layout += "</div>";
                            _layout += "</td>";

                            //Create
                            _layout += "<td class='text-center'>";
                            _layout += "<div class='form-check-inline form-check-success'>";
                            _layout += " <input type='checkbox' name='chkPageCreate' class='form-check-input privilege-checkbox' id='chkPageCreate_" + page.PageControlID + "' data-privilege='IsAdd' data-page-id='" + page.PageControlID + "' />";
                            _layout += " <label class='custom-control-label' for='chkPageCreate_" + page.PageControlID + "'></label>";
                            _layout += "</div>";
                            _layout += "</td>";

                            //Update
                            _layout += "<td class='text-center'>";
                            _layout += "<div class='form-check-inline form-check-success'>";
                            _layout += " <input type='checkbox' name='chkPageUpdate' class='form-check-input privilege-checkbox' id='chkPageUpdate_" + page.PageControlID + "' data-privilege='IsEdit' data-page-id='" + page.PageControlID + "' />";
                            _layout += " <label class='custom-control-label' for='chkPageUpdate_" + page.PageControlID + "'></label>";
                            _layout += "</div>";
                            _layout += "</td>";

                            //Delete
                            _layout += "<td class='text-center'>";
                            _layout += "<div class='form-check-inline form-check-success'>";
                            _layout += " <input type='checkbox' name='chkPageDelete' class='form-check-input privilege-checkbox' id='chkPageDelete_" + page.PageControlID + "' data-privilege='IsDelete' data-page-id='" + page.PageControlID + "' />";
                            _layout += " <label class='custom-control-label' for='chkPageDelete_" + page.PageControlID + "'></label>";
                            _layout += "</div>";
                            _layout += "</td>";

                            //View
                            _layout += "<td class='text-center'>";
                            _layout += "<div class='form-check-inline form-check-success'>";
                            _layout += " <input type='checkbox' name='chkPageView' class='form-check-input privilege-checkbox' id='chkPageView_" + page.PageControlID + "' data-privilege='IsView' data-page-id='" + page.PageControlID + "' />";
                            _layout += " <label class='custom-control-label' for='chkPageView_" + page.PageControlID + "'></label>";
                            _layout += "</div>";
                            _layout += "</td>";

                            //Export
                            _layout += "<td class='text-center'>";
                            _layout += "<div class='form-check-inline form-check-success'>";
                            _layout += " <input type='checkbox' name='chkPageExport' class='form-check-input privilege-checkbox' id='chkPageExport_" + page.PageControlID + "' data-privilege='IsExport' data-page-id='" + page.PageControlID + "' />";
                            _layout += " <label class='custom-control-label' for='chkPageExport_" + page.PageControlID + "'></label>";
                            _layout += "</div>";
                            _layout += "</td>";

                            _layout += "</tr>";
                        });
                        _layout += "</tbody></table></div>";
                        _layout += "</div>";
                        TabContent++;
                    }
                });
            });

            _layout += "</div>";
            _layout += "</div></div>"; //row close

            $("#divMenuLayout").html(_layout);

            $('.select-all').on('change', function () {
                const pageId = $(this).data('page-id');
                const isChecked = $(this).is(':checked');
                $(`.privilege-checkbox[data-page-id="${pageId}"]`).prop('checked', isChecked);
            });

            // Privilege Checkbox Change
            $('.privilege-checkbox').on('change', function () {
                const pageId = $(this).data('page-id');
                const allChecked = $(`.privilege-checkbox[data-page-id="${pageId}"]:checked`).length ===
                    $(`.privilege-checkbox[data-page-id="${pageId}"]`).length;
                $(`.select-all[data-page-id="${pageId}"]`).prop('checked', allChecked);
            });

            // Capture unchecked privileges
            $('.privilege-checkbox').change(function () {
                const pageId = $(this).data('page-id');
                const privilege = $(this).data('privilege');

                if (!$(this).is(':checked')) {
                    removedPrivileges.push({ pageId, privilege });
                }
            });

            // Iterate through gRoleConfiguration and enable checkboxes based on access
            gRoleConfiguration.forEach(function (page) {
                // Check and set "Access" permission
                if (page.IsAccess) {
                    $("#chkPageAccess_" + page.PageControlID).prop("checked", true);
                }

                // Check and set "Create" permission
                if (page.IsAdd) {
                    $("#chkPageCreate_" + page.PageControlID).prop("checked", true);
                }

                // Check and set "Update" permission
                if (page.IsEdit) {
                    $("#chkPageUpdate_" + page.PageControlID).prop("checked", true);
                }

                // Check and set "View" permission
                if (page.IsView) {
                    $("#chkPageView_" + page.PageControlID).prop("checked", true);
                }

                // Check and set "Delete" permission
                if (page.IsDelete) {
                    $("#chkPageDelete_" + page.PageControlID).prop("checked", true);
                }

                // Check and set "Export" permission
                if (page.IsExport) {
                    $("#chkPageExport_" + page.PageControlID).prop("checked", true);
                }

                if (page.IsAccess && page.IsAdd && page.IsEdit && page.IsView && page.IsDelete && page.IsExport)
                    $("#chkSelectAll_" + page.PageControlID).prop("checked", true);
            });
        }
    });
}

$('#searchBox').on('keyup', function () {
    var searchValue = $(this).val().toLowerCase(); // Get search text
    $('#rolePrivilegesTable tbody tr').filter(function () {
        $(this).toggle($(this).find('.page-name').text().toLowerCase().indexOf(searchValue) > -1);
    });
});