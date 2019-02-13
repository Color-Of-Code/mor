namespace mor.Interfaces
{
    public interface IDiscTocEntry
    {
        int TrackNumber { get; }
        // logical block address
        int Lba { get; }
        // start time in s
        int StartTime { get; }
        // start time in frames
        int StartFrame { get; }
        int Mode { get; }
        int Format { get; }
        int Adr { get; }
        int Control { get; }
        int M { get; }
        int S { get; }
        int F { get; }
   }
}
