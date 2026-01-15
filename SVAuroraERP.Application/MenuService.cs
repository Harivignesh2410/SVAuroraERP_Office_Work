namespace SVAuroraERP.Application
{
    public class MenuService
    {
        private readonly IMenuServiceRepository _menuRepository;

        public MenuService(IMenuServiceRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public List<LkupMenuGroup>? DrawMenuLayoutByRoleID(int RoleID)
        {
            return _menuRepository.DrawMenuLayoutByRoleID(RoleID);
        }
    }
}