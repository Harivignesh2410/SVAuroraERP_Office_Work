namespace SVAuroraERP.Application.Interfaces.Persistance.Authentication
{
    public interface IHSRPConfigServiceRepository
    {
        DataResponse GetHSRPUser();
        DataResponse GetRole();
        public DataResponse Save(OEMConfig request);
        public DataResponse SaveRole(HSRPRoleConfig request);
        public DataResponse GetHSRPConfig();

    }
}
