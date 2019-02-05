
using System.Collections.Generic;

namespace mor.Interfaces
{
    public interface IDiscTocEntry
    {
        int TrackNumber { get; }
        DiscAddress Address { get; }
        int Mode { get; }
        int Format { get; }
        int Adr { get; }
        int Control { get; }
   }
}
