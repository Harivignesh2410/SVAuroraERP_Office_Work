namespace SVAuroraERP.Infrastructure.Repositories.Authentication
{
    public class MenuServiceRepository : IMenuServiceRepository
    {
        private readonly SVAuroraERPDbContext _dbcontext;
        public MenuServiceRepository(SVAuroraERPDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public List<LkupMenuGroup>? DrawMenuLayoutByRoleID(int RoleID)
        {
            var result = _dbcontext.VRoleConfiguration
                             .Where(vc => vc.RoleID == RoleID && vc.IsAccess)
                             .OrderBy(vc => vc.MenuGroupOrdinalNo)
                             .ThenBy(vc => vc.MenuOrdinalNo)
                             .ThenBy(vc => vc.PageOrdinalNo)
                             .Select(vc => new
                             {
                                 MenuGroupId = vc.MenuGroupID,
                                 MenuGroupName = vc.MenuGroupName,
                                 MenuGroupOrdinalNo = vc.MenuGroupOrdinalNo,
                                 MenuControlId = vc.MenuControlID,
                                 MenuControlName = vc.MenuName,
                                 MenuIcon = vc.MenuIcon,
                                 ModuleID = vc.ModuleID,
                                 MenuControlOrdinalNo = vc.MenuOrdinalNo,
                                 MenuControlDisplayName = vc.MenuDisplayName,
                                 PageControlId = vc.PageControlID,
                                 PageName = vc.PageName,
                                 PageURL = vc.PageURL,
                                 PageIcon = vc.PageIcon,
                                 PageOrdinalNo = vc.PageOrdinalNo
                             })
                             .ToList();

            var groupedResult = result
            .GroupBy(g => new { g.MenuGroupId, g.MenuGroupName, g.MenuGroupOrdinalNo })
            .Select(g => new LkupMenuGroup
            {
                MenuGroupID = g.Key.MenuGroupId,
                MenuGroupName = g.Key.MenuGroupName,
                OrdinalNo = g.Key.MenuGroupOrdinalNo,
                MenuControlList = g.GroupBy(mc => new { mc.MenuControlId, mc.MenuControlName, mc.MenuControlDisplayName, mc.MenuIcon, mc.ModuleID, mc.MenuControlOrdinalNo })
                                .Select(mc => new LkupMenuControl
                                {
                                    MenuControlID = mc.Key.MenuControlId,
                                    MenuName = mc.Key.MenuControlName,
                                    MenuDisplayName = mc.Key.MenuControlDisplayName,
                                    MenuGroupID = g.Key.MenuGroupId,
                                    MenuIcon = mc.Key.MenuIcon,
                                    ModuleID = mc.Key.ModuleID,
                                    OrdinalNo = mc.Key.MenuControlOrdinalNo, //Added Page Control Grouping on 2024.12.27
                                    PageControlList = mc.GroupBy(p => new { p.PageControlId, p.MenuControlId, p.PageName, p.PageIcon, p.PageURL, p.PageOrdinalNo })
                                    .Select(pc => new LkupPageControl
                                    {
                                        MenuControlID = pc.Key.MenuControlId,
                                        PageControlID = pc.Key.PageControlId,
                                        PageName = pc.Key.PageName,
                                        PageIcon = pc.Key.PageIcon,
                                        PageURL = pc.Key.PageURL,
                                        OrdinalNo = pc.Key.PageOrdinalNo
                                    }).ToList()
                                }).ToList()
            }).ToList();

            return groupedResult;
        }
    }
}