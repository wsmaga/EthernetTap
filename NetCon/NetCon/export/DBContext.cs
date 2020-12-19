using NetCon.export.entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
