using Core.Models.Auth;
using Core.Models.Common;
using Core.Models.Lead;
using Core.Models.Menu;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories.Data;

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
        RegisterAutoIncludes(modelBuilder);
    }

    #region - User, Menu and Roles -
    public virtual DbSet<AspNetServiceTypes> AspNetServiceTypes { get; set; } = default!;
    public virtual DbSet<AspNetBusinessInfo> AspNetBusinessInfo { get; set; } = default!;
    public virtual DbSet<AspNetBusinessApiKeys> AspNetBusinessApiKeys { get; set; } = default!;
    public virtual DbSet<AspNetUserBusinessInfo> AspNetUserBusinessInfo { get; set; } = default!;
    public virtual DbSet<MenuItem> MenuItem { get; set; } = default!;
    public virtual DbSet<MenuToRole> MenuToRole { get; set; } = default!;
    #endregion

    #region - Lead -
    public virtual DbSet<DataTypes> DataTypes { get; set; } = default!;
    public virtual DbSet<FormDetails> FormDetails { get; set; } = default!;
    public virtual DbSet<FormValues> FormValues { get; set; } = default!;
    public virtual DbSet<BusinessSupportedFormId> BusinessSupportedFormId { get; set; } = default!;
    #endregion

    #region - Audit -
    public virtual DbSet<Audit> Audit { get; set; } = default!;
    private AuditTrailFactory? auditFactory = null;
    private readonly List<Audit> auditList = [];
    #endregion

    /// <summary>
    /// Author      : Sakibur Rahman
    /// Date        : 01 Jun 2025
    /// Description : Tracing modified data
    /// </summary>
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


    /// <summary>
    /// Md. Sakibur Rahman
    /// 21 Jun 2035
    /// </summary>
    private static void RegisterAutoIncludes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetBusinessInfo>()
           .Navigation(m => m.AspNetServiceTypes)
           .AutoInclude();

        modelBuilder.Entity<AspNetUserBusinessInfo>()
            .Navigation(m => m.AspNetBusinessInfo)
            .AutoInclude();
        modelBuilder.Entity<AspNetUserBusinessInfo>()
            .Navigation(m => m.User)
            .AutoInclude();

        modelBuilder.Entity<FormDetails>()
            .Navigation(m => m.DataTypes)
            .AutoInclude();

        modelBuilder.Entity<FormValues>()
            .Navigation(m => m.FormDetails)
            .AutoInclude();
        modelBuilder.Entity<FormValues>()
            .Navigation(m => m.AspNetBusinessInfo)
            .AutoInclude();
    }
}
