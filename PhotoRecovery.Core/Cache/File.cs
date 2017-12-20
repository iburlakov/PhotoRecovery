using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Models = PhotoRecovery.Core.Data.Models;

namespace PhotoRecovery.Core.Cache
{
    public class File
    {
        public File(Models.File file, Dir parentDir)
        {
            this.Id = file.Id;
            this.Name = file.Name;
            this.Path = System.IO.Path.Combine(parentDir.Path, file.Name);
            this.Length = file.Length;
            this.Created = file.Created;
            this.Modified = file.Modified;
            this.Taken = file.Taken;
        }

        public long Id { get; private set; }
        public long ParentId { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public long Length { get; private set; }
        public DateTime? Created { get; private set; }
        public DateTime? Modified { get; private set; }
        public DateTime? Taken { get; private set; }

        public override string ToString()
        {
            return this.Path;
        }
    }
}
