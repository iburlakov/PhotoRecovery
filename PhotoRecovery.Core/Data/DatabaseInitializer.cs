using System.Data.Entity;
using System.Linq;

namespace PhotoRecovery.Core.Data
{
    public class DatabaseInitializer<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
    {
        public void InitializeDatabase(TContext context)
        {
            if (this.GetVersion(context) == 0)
            {
                // manually crete tables
                context.Database.ExecuteSqlCommand(@"
                    CREATE TABLE Dir(Id INTEGER PRIMARY KEY NOT NULL, Name TEXT NOT NULL, ParentId INTEGER);
                    CREATE TABLE File(Id INTEGER PRIMARY KEY NOT NULL, Name TEXT NOT NULL, ParentId INTEGER NOT NULL, Length INTEGER NOT NULL, Created INTEGER, Modified INTEGER, Taken INTEGER);
                    CREATE TABLE RawFile(Id INTEGER PRIMARY KEY NOT NULL, Path TEXT NOT NULL, Length INTEGER NOT NULL, Created INTEGER, Modified INTEGER, Taken INTEGER);
                    PRAGMA user_version = 1;");
            }

            // do migrations
        }

        private int GetVersion(TContext context)
        {
            return context.Database.SqlQuery<int>("PRAGMA user_version;").Single();
        }

        private void SetVersion(TContext context, int version)
        {
            context.Database.ExecuteSqlCommand("PRAGMA user_version = @p0;", version);
        }
    }
}
