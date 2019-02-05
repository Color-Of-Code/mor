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

            var freeDbDiscID = new FreeDbDiscID();
            Console.WriteLine("FreeDB DISC ID:\r\n {0}", freeDbDiscID.GetFingerprint(toc));

            var musicBrainzDiscID = new MusicBrainzDiscID();
            Console.WriteLine("MusicBrainz DISC ID:\r\n {0}", musicBrainzDiscID.GetFingerprint(toc));
        }
    }
}
