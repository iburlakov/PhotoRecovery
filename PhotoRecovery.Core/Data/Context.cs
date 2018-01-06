using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

using System.Data.SQLite;

using NLog;

using PhotoRecovery.Core.Data.Models;
using System.Linq;

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

        public void Vacuum()
        {
            this.Database.ExecuteSqlCommand(TransactionalBehavior.DoNotEnsureTransaction, "VACUUM;");
        }

        public string GetStatString()
        {
            return string.Format("Database has {0} dirs with {1} files, {2} raw files and {3} restored dirs.",
                this.Dirs.Count(),
                this.Files.Count(),
                this.RawFiles.Count(),
                this.Dirs.Count(d => d.Restored.HasValue && d.Restored.Value));
        }

        public DbSet<Dir> Dirs { get; set; }
        
        public DbSet<File> Files { get; set; }

        public DbSet<RawFile> RawFiles { get; set; }
    }
}
