using System;
using CommandLine;
using mor.Fingerprint;
using mor.Hardware;

namespace mor
{
    class Program
    {
        public class Options
        {
            [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
            public bool Verbose { get; set; }

            [Option('d', "device", Required = false, HelpText = "Set the device.", Default = "/dev/cdrom")]
            public string Device { get; set; }
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       Run(o);
                   });
        }

        static void Run(Options o)
        {
            var toc = CdRom.ReadToc(o.Device);

            var freeDbDiscID = new FreeDbDiscID();
            Console.WriteLine("FreeDB DISC ID:\r\n {0}", freeDbDiscID.GetFingerprint(toc));

            var musicBrainzDiscID = new MusicBrainzDiscID();
            Console.WriteLine("MusicBrainz DISC ID:\r\n {0}", musicBrainzDiscID.GetFingerprint(toc));
        }
    }
}
