using Core.Models.Auth;
using Core.Models.Common;
using Infrustructure.Repositories.AuditFactory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace Infrustructure.Repositories.Data
{
    public class LeadContext : IdentityDbContext<ApplicationUser>
    {
        public LeadContext() { }
        public LeadContext(DbContextOptions<LeadContext> options)
            : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<AspNetServiceTypes> AspNetServiceTypes { get; set; } = default!;
        //For storing users actions in Audit Table
        public virtual DbSet<Audit> Audit { get; set; } = default!;
        private AuditTrailFactory? auditFactory = null;
        private readonly List<Audit> auditList = [];
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            auditList.Clear();
            auditFactory = new AuditTrailFactory(new HttpContextAccessor());

            var entityList = ChangeTracker.Entries().Where(p => p.State == EntityState.Deleted || p.State == EntityState.Modified);  //p.State == EntityState.Added || for insert statement
            foreach (var entity in entityList)
            {
                Audit? audit = await auditFactory.GetAudit(entity);
                bool isValid = true;

                var tableName = audit?.TableName ?? string.Empty;
                //won't go inside if state is added or Empty Modified
                if (audit is null || string.IsNullOrEmpty(tableName))
                {
                    isValid = false;
                }
                else if (entity.State == EntityState.Modified && string.IsNullOrWhiteSpace(audit?.NewData) && string.IsNullOrWhiteSpace(audit?.OldData))
                {
                    isValid = false;
                }
                else if (tableName.Contains("RequestCount", StringComparison.CurrentCultureIgnoreCase) || tableName.Contains("AspNet", StringComparison.CurrentCultureIgnoreCase))
                {
                    isValid = false;
                }

                if (isValid)
                {
                    auditList.Add(audit!);
                }
            }
            var retVal = await base.SaveChangesAsync(cancellationToken);
            if (auditList.Count > 0)
            {
                await Audit.AddRangeAsync(auditList, cancellationToken);
                await base.SaveChangesAsync(cancellationToken);
            }

            return retVal;
        }
    }
}
