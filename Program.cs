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

            var discid = new FreeDbDiscID();
            Console.WriteLine("FreeDB DISC ID:\r\n {0}", discid.GetFingerprint(toc));
        }
    }
}
