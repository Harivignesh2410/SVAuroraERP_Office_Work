namespace SVAuroraERP.Application.Interfaces.Persistance.Authentication
{
    public interface IMenuServiceRepository
    {
        List<LkupMenuGroup>? DrawMenuLayoutByRoleID(int RoleID);
    }
}