using System;

namespace PhotoRecovery.Core.Data.Models
{
    public class File
    {
        public long Id { get; set; }
        public long ParentId { get; set; }
        public string Name { get; set; }
        public long Length { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Taken { get; set; }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
