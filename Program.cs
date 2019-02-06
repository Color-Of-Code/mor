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

            [Option("toc", Required = false, HelpText = "Dump table of contents.")]
            public bool PrintToc { get; set; }

            [Option("freedb-id", Required = false, HelpText = "Dump Free DB disc id.")]
            public bool PrintFreeDbDiscID { get; set; }
            [Option("musicbrainz-id", Required = false, HelpText = "Dump MusicBrainz disc id.")]
            public bool PrintMusicBrainzDiscID { get; set; }
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
            if (o.PrintToc)
            {
                Console.WriteLine($"first: {toc.StartTrack}");
                Console.WriteLine($"last: {toc.EndTrack}");
                foreach (var e in toc.TrackEntries)
                {
                    Console.WriteLine("track: {0,3} lba: {1,9} {5:00}:{6:00}:{7:00} adr: {2} control: {3} mode: {4}",
                        e.TrackNumber, e.Lba, e.Adr, e.Control, e.Mode, e.M, e.S, e.F);
                }
                var l = toc.LeadOutEntry;
                Console.WriteLine("track:lout lba: {0,9} {3:00}:{4:00}:{5:00} adr: {1} control: {2}",
                    l.Lba, l.Adr, l.Control, l.M, l.S, l.F);
            }

            if (o.PrintFreeDbDiscID)
            {
                var freeDbDiscID = new FreeDbDiscID();
                Console.WriteLine("FreeDB DISC ID:\r\n {0}", freeDbDiscID.GetFingerprint(toc));
            }

            if (o.PrintMusicBrainzDiscID)
            {
                var musicBrainzDiscID = new MusicBrainzDiscID();
                Console.WriteLine("MusicBrainz DISC ID:\r\n {0}", musicBrainzDiscID.GetFingerprint(toc));
            }
        }
    }
}
