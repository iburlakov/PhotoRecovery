using System.Data.Entity;
using System.Linq;

namespace PhotoRecovery.Core.Data
{
    public class DatabaseInitializer<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        public void InitializeDatabase(TContext context)
        {
            context.Database.CreateIfNotExists();

            switch (this.GetVersion(context))
            {
                // manually crete tables
                case 0:
                    context.Database.ExecuteSqlCommand(@"
                        CREATE TABLE Dir(Id INTEGER PRIMARY KEY NOT NULL, Name TEXT NOT NULL, ParentId INTEGER, Restored INTEGER);
                        CREATE TABLE File(Id INTEGER PRIMARY KEY NOT NULL, Name TEXT NOT NULL, ParentId INTEGER NOT NULL, Length INTEGER NOT NULL, Created INTEGER, Modified INTEGER, Taken INTEGER);
                        CREATE TABLE RawFile(Id INTEGER PRIMARY KEY NOT NULL, Path TEXT NOT NULL, Length INTEGER NOT NULL, Created INTEGER, Modified INTEGER, Taken INTEGER);
                        PRAGMA user_version = 1;");
                    break;

                // migrations
                case 1:
                    context.Database.ExecuteSqlCommand(@"
                        ALTER TABLE Dir ADD COLUMN Restored INTEGER;
                         PRAGMA user_version = 2;");
                    break;
            }
        }

        private int GetVersion(TContext context)
        {
            return context.Database.SqlQuery<int>("PRAGMA user_version;").Single();
        }
    }
}
