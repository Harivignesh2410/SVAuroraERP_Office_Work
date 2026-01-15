namespace SVAuroraERP.Infrastructure.Persistence
{
    public class SVAuroraERPLogDbContext(DbContextOptions<SVAuroraERPLogDbContext> options) : DbContext(options)
    {
        public DbSet<LoginAudit> LoginAuditInfo { get; set; }
        public DbSet<Project> Project { get; set; }
        public DbSet<ErrorLog> ErrorLog { get; set; }
        public DbSet<PageAccessAudit> PageAccessAudit { get; set; }
        public DbSet<ActionLog> ActionLog { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}