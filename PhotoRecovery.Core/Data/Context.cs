using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;

using NLog;

using PhotoRecovery.Core.Data.Models;

namespace PhotoRecovery.Core.Data
{
    [DbConfigurationType(typeof(DatabaseConfiguration))]
    public class Context : DbContext
    {
        private static ILogger log = LogManager.GetCurrentClassLogger();

        public Context() : base(new SQLiteConnection(@"Data Source = photoRecovery.db; Version=3;"), contextOwnsConnection: true)
        {
            Database.Log = log.Trace;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            Database.SetInitializer(new DatabaseInitializer<Context>());
        }

        public DbSet<Dir> Dirs { get; set; }
        
        public DbSet<File> Files { get; set; }

        public DbSet<RawFile> RawFiles { get; set; }
    }
}
