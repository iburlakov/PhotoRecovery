using Models = PhotoRecovery.Core.Data.Models;

namespace PhotoRecovery.Core.Cache
{
    public class Dir
    {
        public Dir(Models.Dir dir, Dir parentDir = null)
        {
            this.Id = dir.Id;
            this.ParentId = dir.ParentId;
            this.Name = dir.Name;
            this.Path = parentDir == null ? dir.Name : System.IO.Path.Combine(parentDir.Path, dir.Name);
            this.Restored = dir.Restored ?? false;
        }

        public long Id { get; private set; }
        public long? ParentId { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public bool Restored { get; private set; }

        public override string ToString()
        {
            return this.Path;
        }
    }
}
