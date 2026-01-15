namespace SVAuroraERP.Application.Interfaces.Persistance.Authentication
{
    public interface IAppLauncherServiceRepository
    {
        List<VApplauncher> GetByUserID(int UserID);
        Tuple<bool, bool> Save(List<AppLauncher> AppLauncher);
        List<VRoleConfiguration> GetAppLauncherListByUserID(int UserID);
    }
}