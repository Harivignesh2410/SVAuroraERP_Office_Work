namespace SVAuroraERP.Application.Interfaces.Persistance.Authentication
{
    public interface IRoleServiceRepository
    {
        List<LkupModule> GetModuleList();
        List<VRole> GetList();
        VRole? GetByID(int ID);
        Tuple<bool, bool> Save(Role role);
        Tuple<bool, bool> Update(Role role);
        Tuple<bool, bool> Delete(int ID, int UserID, long LoginAuditID);        
        List<RoleModule> GetRoleModuleByID(int RoleID);
        List<VRole> GetRoleByApplicationID(int ApplicationID);
        List<LkupModule> GetModuleListByApplicationID(int ApplicationID);
    }
}