
using System;
using System.Runtime.InteropServices;
using mor.Interfaces;

namespace mor.Hardware
{
    // OS independent part of the Toc extraction
    public class CdRom
    {
        public static IDiscToc ReadToc(string device)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return CdRomUnix.ReadToc(device);
            throw new NotImplementedException("OsPlatform not supported");
        }
    }
}
 