using System;

namespace mor.FreeDB
{
    public class Track
    {
        public string ExtendedData { get; set; }
        public string Title { get; set; }
        public int Number { get; set; }

        public Track(int number, string title = null, string extendedData = null)
        {
            Number = number;
            Title = title;
            ExtendedData = extendedData;
        }
    }
}
