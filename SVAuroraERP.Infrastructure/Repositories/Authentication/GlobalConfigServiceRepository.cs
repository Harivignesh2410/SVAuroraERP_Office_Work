namespace SVAuroraERP.Infrastructure.Repositories.Authentication
{
    public class GlobalConfigServiceRepository : IGlobalConfigServiceRepository
    {
        public readonly SVAuroraERPDbContext _dbcontext;
        public GlobalConfigServiceRepository(SVAuroraERPDbContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public async Task<GlobalConfig> GetGlobalConfig() => await _dbcontext.GlobalConfig.FirstOrDefaultAsync();
    }
}