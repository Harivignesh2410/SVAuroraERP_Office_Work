namespace SVAuroraERP.Application.Interfaces.Persistance.Authentication
{
    public interface IRoleConfigurationServiceRepository
    {
        List<VRoleConfiguration> GetRoleConfigurationByRoleID(int RoleID);
        Tuple<bool, bool> SaveChanges(List<RoleConfiguration> role);
        List<LkupMenuGroup>? GetMenuLayout();
        List<VRoleConfiguration> GetRoleConfigurationByUserID(int UserID);
    }
}