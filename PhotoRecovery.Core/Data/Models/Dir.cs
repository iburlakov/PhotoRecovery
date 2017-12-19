namespace PhotoRecovery.Core.Data.Models
{
    public class Dir
    {
        public long Id { get; set; }
        public long? ParentId {get;  set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("{0}[Id: {1}, ParentId: {2}]", this.Name, this.Id, this.ParentId?.ToString() ?? "NULL");
        }
    }
}
