
namespace mor.Fingerprint
{
    using System;
    using System.Security.Cryptography;
    using mor.Interfaces;

    // Method is explained here:
    // https://musicbrainz.org/doc/Disc_ID_Calculation
    public class MusicBrainzDiscID : IDiscFingerprint
    {
        public string GetFingerprint(IDiscToc toc)
        {
            var encoding = System.Text.Encoding.ASCII;
            var ba = new byte[(1 + 1 + 4 + 99 * 4) * 2];
            // First track number (normally one): 1 byte
            var s = String.Format("{0:X2}", (byte)toc.StartTrack);
            Array.Copy(encoding.GetBytes(s), 0, ba, 0, 2);
            // Last track number: 1 byte
            s = String.Format("{0:X2}", (byte)toc.EndTrack);
            Array.Copy(encoding.GetBytes(s), 0, ba, 2, 2);
            // in sum we have 100 offsets, Lead out + 99 tracks            
            var offsets = new int[100];
            offsets[0] = toc.LeadOutEntry.StartFrame;
            int i = 1;
            foreach (var e in toc.TrackEntries)
            {
                offsets[i++] = e.StartFrame;
            }
            // Add the hex representation of all these
            for (i = 0; i < 100; i++)
            {
                s = String.Format("{0:X8}", offsets[i]);
                Array.Copy(encoding.GetBytes(s), 0, ba, 4 + 8 * i, 8);
            }

            var sha1 = new SHA1Managed();
            var hash = sha1.ComputeHash(ba);
            var id = Convert.ToBase64String(hash);

            // +, /, and = characters, all of which are special HTTP/URL characters.
            // To avoid the problems with dealing with that, MusicBrainz uses ., _, and -
            id = id.Replace('+', '.');
            id = id.Replace('/', '_');
            id = id.Replace('=', '-');

            return id;
        }
    }
}