
using System.Collections.Generic;

namespace mor.Interfaces
{
    // there are two ways to address a place on the disc
    // MSF or LBA
    
    public struct MsfAddress
    {
        private byte minute;
        private byte second;
        private byte frame;     // 1/75 s
    };

    public class DiscAddress
    {
        public DiscAddress(int lba)
        {
            Lba = lba;
        }
        public MsfAddress Msf { get; }
        public int Lba { get; }
    }
}
