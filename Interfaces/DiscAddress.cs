
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

    public struct LbaAddress
    {
        private int lba;
    };

    public class DiscAddress
    {
        MsfAddress MsfAddress { get; }
        LbaAddress LbaAddress { get; }
    }
}
