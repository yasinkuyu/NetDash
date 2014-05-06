namespace Insya.NetDash.Models
{
    public class Disk
    {
        public string Name { get; set; }
        public string Volume { get; set; }
        public ulong Total { get; set; }
        public  ulong Used { get; set; }
        public ulong Free { get; set; }
        public int PerUsed { get; set; }

        public Disk(string name, string volume, ulong total, ulong used, ulong free, int perUsed)
        {
            Name = name;
            Volume = volume;
            Total = total;
            Used = used;
            Free = free;
            PerUsed = perUsed;
        }
    }

}