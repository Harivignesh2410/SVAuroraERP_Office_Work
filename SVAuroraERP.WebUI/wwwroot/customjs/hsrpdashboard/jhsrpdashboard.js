$(function () {
    pLoadingSetup(false);
    getHsrpDashboardData();
    pLoadingSetup(true);
});
function getHsrpDashboardData() {

    $.ajax({
        url: SummaryListUrl,
        type: "POST",
        headers: { "RequestVerificationToken": _TOKEN },
        success: function (response) {

            if (!response || !response.data) {
                Swal.fire("Error", "No dashboard data found", "error");
                return;
            }

            $(".WebPageContent").removeClass("d-none");

            renderSummaryTable(response.data.Summary);
            renderOEMOrdersTable(response.data.OEMOrders);
            renderOnlineOrdersTable(response.data.OnlineOrders);
        },
        error: function () {
            Swal.fire("Error", "Failed to load dashboard", "error");
        }
    });
}
const oemStatusIcons = {
    "Ready for Processing": "fas fa-cogs text-info",
    "Laser No. Assigned": "fas fa-bolt text-warning",
    "Job Card Generated": "fas fa-spinner text-primary",
    "Quality Processing": "fas fa-clipboard-check text-success",
    "QC Completed": "fas fa-check-square text-warning",
    "Invoice Generated": "fas fa-file-invoice text-info",
    "Dispatched": "fas fa-shopping-bag text-danger",
    "Delivered": "fas fa-truck text-success",
    "Vahan API Submitted": "fas fa-motorcycle text-warning",
    "Rejected Quality Processing": "fas fa-spinner text-primary",
    "Fitted Orders": "fas fa-clipboard-check text-success",
    "Cancelled Orders": "fas fa-window-close text-danger",
    "Fixation Re-upload Orders": "fas fa-file-upload text-danger",
    "Fixation Re-Uploaded": "fas fa-image text-info"
};

function renderSummaryTable(data) {

    data.forEach(item => {

        if (item.OrderType === "OEM Orders") {
            $("#summaryOEM").text(item.TotalOrders);
        }
        else if (item.OrderType === "Online Orders") {
            $("#summaryOnline").text(item.TotalOrders);
        }
        else if (item.OrderType === "Grand Total") {
            $("#summaryTotal").text(item.TotalOrders);
        }
    });
}
function renderOEMOrdersTable(data) {

    let container = $("#oemOrderCards");
    container.empty();

    data
        .sort((a, b) => a.OrdinalNo - b.OrdinalNo)
        .forEach(item => {
            //console.log(item);
            let iconClass = oemStatusIcons[item.Description] || "fas fa-box text-muted";

            /*
            container.append(`
                <div class="col-xl-3 col-lg-3 col-md-4 col-sm-6">
                    <div class="card shadow-sm text-center h-100 cursor-pointer"
                         onclick="redirectToOrderList(${item.OrderStatusID})">
                        <div class="card-body">
                            <div class="mb-2">
                                <i class="${iconClass} fa-2x"></i>
                            </div>
                            <h6 class="fw-bold mb-1 text-primary">${item.Description}</h6>
                            <h3 class="fw-bold mb-0">${item.OrderCount}</h3>
                        </div>
                    </div>
                </div>
            `);
            */

            container.append(`<div class="col-xl-3 col-lg-3 col-md-4 col-sm-6 cursor-pointer">
                    <a href="javascript:void(0);">
                        <div class="card shadow-sm" onclick="redirectToOrderList(${item.OrderStatusID})">
	                        <div class="card-body">
		                        <div class="d-flex flex-wrap">
			                        <div>
				                        <p class="text-muted mb-1">${item.Description}</p>
				                        <h4 class="mb-3 text-primary">${item.OrderCount}</h4>				
			                        </div>
			                        <div class="ms-auto align-self-end">
				                        <i class="${iconClass} display-4"></i>
			                        </div>
		                        </div>
	                        </div>
                        </div>
                    </a>
                 </div>
            `);
        });
}
function renderOnlineOrdersTable(data) {

    let container = $("#OnlineOrderCards");
    container.empty();

    data
        .sort((a, b) => a.OrdinalNo - b.OrdinalNo)
        .forEach(item => {

            let iconClass = oemStatusIcons[item.Description] || "fas fa-box text-muted";

            container.append(`
                <div class="col-xl-3 col-lg-3 col-md-4 col-sm-6">
                     <div class="card shadow-sm text-center h-100 cursor-pointer"
                         onclick="redirectToOrderList(${item.OrderStatusID})">
                        <div class="card-body">
                            <div class="mb-2">
                                <i class="${iconClass} fa-2x"></i>
                            </div>
                            <h6 class="fw-bold mb-1">${item.Description}</h6>
                            <h3 class="fw-bold text-primary mb-0">${item.OrderCount}</h3>
                        </div>
                    </div>
                </div>
            `);
        });
}
function redirectToOrderList(orderStatusID) {
    var url = GetRedirectUrl + "&OrderStatusID=" + encodeURIComponent(orderStatusID);
    window.location.href = url;
}