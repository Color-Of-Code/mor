
namespace mor.Fingerprint
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using mor.Interfaces;

    // Method is explained here:
    // https://musicbrainz.org/doc/Disc_ID_Calculation
    public class MusicBrainzDiscID : IDiscFingerprint
    {
        public string GetFingerprint(IDiscToc toc)
        {
            // 16 150 19662 37515 67342 91142 110397 131050 146557 174465 190222 212852 231177 254500 273352 293707 315335 330572

            var encoding = System.Text.Encoding.ASCII;
            var ba = new byte[(1+1+4+99*4)*2];
            // First track number (normally one): 1 byte
            var s = String.Format("{0:X2}", (byte)toc.StartTrack);
            Array.Copy(encoding.GetBytes(s), 0, ba, 0, 2);
            // Last track number: 1 byte
            s = String.Format("{0:X2}", (byte)toc.EndTrack);
            Array.Copy(encoding.GetBytes(s), 0, ba, 2, 2);
            // Lead-out track offset: 4 bytes
            s = String.Format("{0:X8}", toc.LeadOutEntry.StartFrame);
            Array.Copy(encoding.GetBytes(s), 0, ba, 4, 8);
            // 99 frame offsets: 4 bytes for each track
            int i = 0;
            foreach (var e in toc.TrackEntries)
            {
                s = String.Format("{0:X8}", e.StartFrame);
                Console.WriteLine(e.StartFrame);
                Array.Copy(encoding.GetBytes(s), 0, ba, 12 + 8*i++, 8);
            }
            var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(ba);
            var id = Convert.ToBase64String(hash);
            
            // +, /, and = characters, all of which are special HTTP/URL characters.
            // To avoid the problems with dealing with that, MusicBrainz uses ., _, and -
            id = id.Replace('+', '.');
            id = id.Replace('/', '_');
            id = id.Replace('=', '-');
                            
            //_u.wZ2COf4ecexgk4GL5PlJDvgY- 16 150 19662 37515 67342 91142 110397 131050 146557 174465 190222 212852 231177 254500 273352 293707 315335 4407

            return id;
        }
    }
}