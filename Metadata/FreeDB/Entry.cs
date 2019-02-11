using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace mor.FreeDB
{
    public class Entry
    {
        public string Discid { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public string Year { get; set; }

        public string Genre { get; set; }

        public IEnumerable<Track> Tracks { get; private set; }

        public string ExtendedData { get; set; }

        public string PlayOrder { get; set; }

        public int NumberOfTracks => Tracks.Count();

        public Entry(IEnumerable<String> data)
        {
            Parse(data);
        }

        private void Parse(IEnumerable<String> data)
        {
            var tracks = new Dictionary<int, Track>();
            var r = new Regex(@"^(?<field>[A-Z]+)(?<id>[0-9]+)?=(?<value>.*)$");
            var at = new Regex(@"^\s*(?<artist>.*?)\s+/\s+(?<title>.*)$");

            // ignore comments
            foreach (var line in data.Where(x => !x.StartsWith('#')))
            {
                var m = r.Match(line);
                if (!m.Success) // couldn't find equal sign have no clue what the data is
                    continue;
                string field = m.Groups["field"].Value;
                string value = m.Groups["value"].Value.Trim();
                switch (field)
                {
                    case "DISCID":
                        Discid = value;
                        break;
                    case "DTITLE": // artist / title
                        {
                            var atm = at.Match(value);
                            if (atm.Success)
                            {
                                Artist = atm.Groups["artist"].Value;
                                Title = atm.Groups["title"].Value;
                            }
                            else
                            {
                                Artist = value;
                                Title = Artist;
                            }
                        }
                        break;
                    case "DYEAR":
                        Year = value;
                        break;
                    case "DGENRE":
                        Genre = value;
                        break;
                    case "EXTD":
                        // may be more than one - just concatenate them
                        ExtendedData += value;
                        break;
                    case "PLAYORDER":
                        PlayOrder += value;
                        break;
                    case "TTITLE":
                        {
                            int trackNumber = int.Parse(m.Groups["id"].Value);
                            tracks[trackNumber] = new Track(trackNumber+1, value);
                        }
                        break;
                    case "EXTT":
                        {
                            int trackNumber = int.Parse(m.Groups["id"].Value);
                            tracks[trackNumber].ExtendedData += value;
                        }
                        break;
                    default:
                        throw new Exception($"Unrecognized field {field}");
                }
            }
            Tracks = tracks.Values.OrderBy(x=> x.Number);
        }
    }
}
