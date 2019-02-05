
using System.Collections.Generic;

namespace mor.Interfaces
{
    public interface IDiscToc
    {
        int StartTrack { get; }
        int EndTrack { get; }
        IEnumerable<IDiscTocEntry> TrackEntries { get; }
        IDiscTocEntry LeadOutEntry { get; }
    }
}
