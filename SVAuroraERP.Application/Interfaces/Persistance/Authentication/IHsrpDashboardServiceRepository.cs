namespace SVAuroraERP.Application.Interfaces.Persistance.Authentication
{
    public interface IHsrpDashboardServiceRepository
    {
        Task<DataResponse> GetHsrpDashboardAsync(HsrpDashboardRequest request);
    }
}
