namespace SVAuroraERP.Application.Interfaces.Persistance.Authentication
{
    public interface IGlobalConfigServiceRepository
    {
        Task<GlobalConfig> GetGlobalConfig();
    }
}