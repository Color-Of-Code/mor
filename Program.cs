using System;
using mor.Fingerprint;
using mor.Hardware;

namespace mor
{
    class Program
    {
        static void Main(string[] args)
        {
            var device = "/dev/cdrom";
            var toc = CdRom.ReadToc(device);

            Console.WriteLine("{0} tracks", toc.EndTrack);
            foreach (var e in toc.TrackEntries)
                Console.WriteLine("{0} - Start Time {1}", e.TrackNumber, e.StartTime);
        }
    }
}
