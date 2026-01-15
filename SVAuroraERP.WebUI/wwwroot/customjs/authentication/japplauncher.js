$(function () {
    pLoadingSetup(false);
    GetAppLauncherByUserID();
    pLoadingSetup(true);
});
function GetAppLauncherByUserID() {
    $("#divAppLauncher").empty();
    $.ajax({
        url: GetAppLauncherByUserIDUrl,
        type: 'GET',
        success: function (response) {
            if (response != null && response.data != null) {
                // Header details
                var headerDetails = ``;                
                response.data.forEach((item, index) => {
                    headerDetails += `
                         <div class="col-lg-2 col-6" data-id="add_${item.PageControlID}">
                             <a class="text-body" href="${item.PageURL}">
                                   <div class="card">
                                        <div class="card-body p-2 text-center">
                                            <img src="${item.PageIcon}" alt="" class="object-fit-fill avatar-sm">
                                             <h5 class="mt-4 mb-2 font-size-13">${item.PageName}</h5>
                                             <p class="mb-0 text-muted">${item.MenuDisplayName}</p>
                                         </div>
                                   </div>
                              </a>
                         </div>`;
                });
              
                // Append both sections to the container
                $("#divAppLauncher").append(headerDetails);

            } else {
               
                $("#divAppLauncher").empty();
            }
        },
        error: function (xhr, status, error) {
            if (ENABLE_VERBOSE_Logging) console.log(error);
            Swal.fire({ title: "Error", text: error.responseText, icon: "error", confirmButtonColor: "#556ee6" });
        }
    });
}