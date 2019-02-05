
namespace mor.Fingerprint
{
    using System.IO;
    using System.Linq;
    using mor.Interfaces;

    // Method is explained here:
    // http://ftp.freedb.org/pub/freedb/misc/freedb_howto1.07.zip
    public class FreeDbDiscID : IDiscFingerprint
    {
        public string GetFingerprint(IDiscToc toc)
        {
            int cksum = toc.TrackEntries.Sum( entry => UpdateCddbSum(entry.StartTime));

            var trackCount = toc.EndTrack;
            var firstEntry = toc.TrackEntries.First();
            var totaltime = toc.LeadOutEntry.StartTime - firstEntry.StartTime;

            var s = new StringWriter();
            // discid
            uint id = ((uint)(cksum % 0xff) << 24) | (uint)(totaltime << 8) | (uint)trackCount;
            s.Write("{0:x8} ", id);

            // number of tracks
            s.Write(trackCount);

            // frame offsets
            foreach (var entry in toc.TrackEntries)
                s.Write(" {0}", entry.StartFrame);

            // length (in s) of disc
            s.Write(" {0}", toc.LeadOutEntry.StartTime);

            return s.ToString();
        }

        private static int UpdateCddbSum(int n)
        {
            int ret = 0;
            while (n > 0)
            {
                ret = ret + (n % 10);
                n = n / 10;
            }
            return ret;
        }
    }
}