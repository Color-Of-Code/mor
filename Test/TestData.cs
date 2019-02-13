using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using mor.Interfaces;

namespace mor.Test
{

    internal class TestData
    {
        /// <summary>
        /// Read a Table of Contents from a file
        /// </summary>
        /// <param name="device">path to the file to read</param>
        /// <returns>A TOC</returns>
        public static IDiscToc ReadToc(string device)
        {
            var lines = File.ReadAllLines(device);
            FileDiscTocHeader tocHeader = null;
            foreach (var line in lines)
            {
                if (line.StartsWith("first:"))
                {
                    var r = new Regex(@"first:\s*(?<first>[\d]+)\s+last:?\s*(?<last>[\d]+)");
                    var groups = r.Match(line).Groups;
                    tocHeader = new FileDiscTocHeader(
                        int.Parse(groups["first"].Value),
                        int.Parse(groups["last"].Value));
                }
                if (line.StartsWith("track:"))
                {
                    var r = new Regex(@"track:\s*(?<track>[\d]+)\s+lba:\s*(?<lba>[\d]+)");
                    var m = r.Match(line);
                    if (m.Success)
                    {
                        var groups = m.Groups;
                        var track = int.Parse(groups["track"].Value);
                        var lba = int.Parse(groups["lba"].Value);
                        tocHeader.AddTocEntry(track, lba);
                    }
                    else
                    {
                        r = new Regex(@"track:\s*lout\s+lba:\s*(?<lba>[\d]+)");
                        var groups = r.Match(line).Groups;
                        var lba = int.Parse(groups["lba"].Value);
                        tocHeader.AddLeadOut(lba);
                    }
                }
            }
            return tocHeader;
        }

        private class FileDiscTocHeader : IDiscToc
        {
            public FileDiscTocHeader(int start, int end)
            {
                StartTrack = start;
                EndTrack = end;
            }

            public int StartTrack { get; }

            public int EndTrack { get; }

            public IEnumerable<IDiscTocEntry> TrackEntries => _entries;

            public IDiscTocEntry LeadOutEntry { get; private set; }

            private List<IDiscTocEntry> _entries = new List<IDiscTocEntry>();
            internal void AddTocEntry(int track, int lba)
            {
                var tocEntry = new FileDiscTocEntry(track, lba);
                _entries.Add(tocEntry);
            }

            internal void AddLeadOut(int lba)
            {
                LeadOutEntry = new FileDiscTocEntry(0xAA, lba);
            }
        }

        private class FileDiscTocEntry : IDiscTocEntry
        {
            public FileDiscTocEntry(int track, int lba)
            {
                TrackNumber = track;
                Lba = lba;
            }

            public int TrackNumber { get; }

            public int Lba { get; }

            public int Mode => 0;

            public int Format => 0;

            public int Adr => 1;

            public int Control => 0;

            // 2s offset = 150 frames
            public int StartFrame => (Lba + 150);
            // 75 frames / s
            public int StartTime => StartFrame / 75;

            public int M => StartTime / 60;
            public int S => StartTime % 60;
            public int F => StartFrame % 75;
        }
    }
}