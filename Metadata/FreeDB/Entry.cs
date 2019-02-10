using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public List<Track> Tracks { get; }

        public string ExtendedData { get; set; }

        public string PlayOrder { get; set; }

        public int NumberOfTracks => Tracks.Count();

        public Entry(IEnumerable<String> data)
        {
            Tracks = new List<Track>();
            if (!Parse(data))
            {
                throw new Exception("Unable to Parse Entry.");
            }
        }

        private bool Parse(IEnumerable<String> data)
        {
            var r = new Regex(@"^(?<field>[A-Z]+)(?<id>[0-9]+)?=(?<value>.*)$");
            foreach (var line in data)
            {
                // ignore comments
                if (line.StartsWith('#'))
                    continue;

                var m = r.Match(line);
                if (!m.Success) // couldn't find equal sign have no clue what the data is
                    continue;
                string field = m.Groups["field"].Value;
                string value = m.Groups["value"].Value;
                switch (field)
                {
                    case "DISCID":
                        Discid = value;
                        break;
                    case "DTITLE": // artist / title
                        {
                            Artist = value;
                            //split the title and artist from DTITLE;
                            // see if we have a slash
                            int slash = Artist.IndexOf(" / ");
                            if (slash == -1)
                            {
                                Title = Artist;
                            }
                            else
                            {
                                string titleArtist = Artist;
                                Artist = titleArtist.Substring(0, slash);
                                slash += 3; // move past " / "
                                Title = titleArtist.Substring(slash);
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
                            //may need to concatenate track info
                            if (trackNumber < Tracks.Count())
                                Tracks[trackNumber].Title += value;
                            else
                                Tracks.Add(new Track(trackNumber, value));
                        }
                        break;
                    case "EXTT":
                        {
                            int trackNumber = int.Parse(m.Groups["id"].Value);
                            if (trackNumber < 0 || trackNumber > Tracks.Count() - 1)
                                continue;

                            Tracks[trackNumber].ExtendedData += value;
                        }
                        break;
                    default:
                        throw new Exception($"Unrecognized field {field}");
                }
            }
            return true;
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            s.Append("Title: ");
            s.Append(Title);
            s.Append("\n");
            s.Append("Artist: ");
            s.Append(Artist);
            s.Append("\n");
            s.Append("Discid: ");
            s.Append(Discid);
            s.Append("\n");
            s.Append("Genre: ");
            s.Append(Genre);
            s.Append("\n");
            s.Append("Year: ");
            s.Append(Year);
            s.Append("\n");
            s.Append("Tracks:");
            foreach (var track in Tracks)
            {
                s.Append("\n");
                s.Append($"{track.Number} - {track.Title}");
            }
            return s.ToString();
        }
    }
}
