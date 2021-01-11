using NetCon.export.entities;
using System.Data.Entity;

namespace NetCon.export
{
    class DBContext : DbContext
    {
        public DbSet<Target> Targets { get; set; }
        public DbSet<TargetName> TargetNames { get; set; }
        public DbSet<VariableEvent> VariableEvents { get; set; }

        public DBContext(string connectionString)
            : base(connectionString) { }
    }
}
