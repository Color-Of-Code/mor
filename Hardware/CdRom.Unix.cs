using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using mor.Interfaces;

namespace mor.Hardware
{
    internal class CdRomUnix
    {
        #region Defs from Linux Kernel
        // https://github.com/torvalds/linux/blob/master/include/uapi/linux/cdrom.h
        /* Some generally useful CD-ROM information -- mostly based on the above */
        
        /* Read TOC header (struct cdrom_tochdr) */
        private static int CDROMREADTOCHDR = 0x5305;
        /* Read TOC entry  (struct cdrom_tocentry) */
        private static int CDROMREADTOCENTRY = 0x5306;
        /* CD-ROM address types (cdrom_tocentry.cdte_format) */
        private static byte CDROM_LBA = 0x01; /* "logical block": first frame is #0 */
        /* The leadout track is always 0xAA, regardless of # of tracks on disc */
        private static byte CDROM_LEADOUT = 0xAA;

        /* Address in MSF format */
        [StructLayout(LayoutKind.Sequential, Size = 3)]
        internal struct cdrom_msf0
        {
            public byte minute;
            public byte second;
            public byte frame;
        };
        /* Address in either MSF or logical format */
        [StructLayout(LayoutKind.Explicit, Size = 4)]
        internal struct cdrom_addr
        {
            [FieldOffset(0)]
            public cdrom_msf0 msf;
            [FieldOffset(0)]
            public int lba;
        };
        /* This struct is used by the CDROMREADTOCHDR ioctl */
        [StructLayout(LayoutKind.Sequential, Size = 2)]
        internal struct cdrom_tochdr
        {
            public byte cdth_trk0; /* start track */
            public byte cdth_trk1; /* end track */
        }
        internal class  LinuxDiscTocHeader : IDiscToc
        {
            public LinuxDiscTocHeader()
            {

                LinuxTocHeader = new cdrom_tochdr();
            }

            internal cdrom_tochdr LinuxTocHeader;

            public int StartTrack => LinuxTocHeader.cdth_trk0;

            public int EndTrack => LinuxTocHeader.cdth_trk1;

            internal void SetTrackInfo(IEnumerable<IDiscTocEntry> tracks, IDiscTocEntry leadout)
            {
                TrackEntries = tracks;
                LeadOutEntry = leadout;
            }
            public IEnumerable<IDiscTocEntry> TrackEntries { get; private set; }

            public IDiscTocEntry LeadOutEntry { get; private set; }
        };

        /* This struct is used by the CDROMREADTOCENTRY ioctl */
        [StructLayout(LayoutKind.Sequential)]
        internal struct cdrom_tocentry
        {
            public byte cdte_track;
            public byte cdte_adr_ctrl; //cdte_adr:4; cdte_ctrl :4;
            public byte cdte_format;
            public cdrom_addr cdte_addr;
            public byte cdte_datamode;
        }

        internal class LinuxDiscTocEntry : IDiscTocEntry
        {
            public LinuxDiscTocEntry()
            {
                LinuxTocEntry = new cdrom_tocentry();
            }

            internal cdrom_tocentry LinuxTocEntry;

            public int TrackNumber => LinuxTocEntry.cdte_track;

            public int Lba => LinuxTocEntry.cdte_addr.lba;

            public int Mode => LinuxTocEntry.cdte_datamode;

            public int Format => throw new NotImplementedException();

            public int Adr => throw new NotImplementedException();

            public int Control => throw new NotImplementedException();

            // 2s offset = 150 frames
            public int StartFrame => (Lba + 150);
            // 75 frames / s
            public int StartTime => StartFrame / 75;
        };
        #endregion

        [DllImport("libc.so.6", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int NativeIoctl(int fd, int request, ref cdrom_tochdr data);
        [DllImport("libc.so.6", EntryPoint = "ioctl", SetLastError = true)]
        private static extern int NativeIoctl(int fd, int request, ref cdrom_tocentry data);
        [DllImport("libc.so.6", EntryPoint = "open", SetLastError = true)]
        private static extern int NativeOpen(string fileName, int mode);
 
        [DllImport("libc.so.6", EntryPoint = "read", SetLastError = true)]
        private static extern int NativeRead(int handle, byte[] data, int length);

        [DllImport("libc.so.6", EntryPoint = "close", SetLastError = true)]
        private static extern int NativeClose(int handle);

        private static int OpenReadOnly(string filename)
        {
            int mode = 0x400; // O_RDONLY | O_NONBLOCK
            int handle = NativeOpen(filename, mode);
            if (handle < 0)
            {
                int error = Marshal.GetLastWin32Error();
                if (error == 123) // ENOMEDIUM No medium found
                    throw new FileNotFoundException("No medium found");
                throw new IOException("open() failed");
            }
            return handle;
        }

        private static int Close(int handle)
        {
            return NativeClose(handle);
        }

        private static void ControlReadTocHeader(int handle, ref cdrom_tochdr header)
        {
            if (NativeIoctl(handle, CDROMREADTOCHDR, ref header)<0)
                throw new System.IO.InvalidDataException("IO/Control TOC header");
        }
        private static void ControlReadTocEntry(int handle, ref cdrom_tocentry entry)
        {
            if (NativeIoctl(handle, CDROMREADTOCENTRY, ref entry)<0)
                throw new System.IO.InvalidDataException("IO/Control TOC entry");
        }

        private static IDiscToc ReadTocInternal(int handle)
        {
            var tocHeader = new LinuxDiscTocHeader();
            ControlReadTocHeader(handle, ref tocHeader.LinuxTocHeader);

            var trackCount = tocHeader.EndTrack;

            var tocEntries = new List<IDiscTocEntry>(trackCount);
            for (byte i = 0; i < trackCount; i++)
            {
                var tocEntry = new LinuxDiscTocEntry();
                tocEntry.LinuxTocEntry.cdte_track = (byte)(i + 1); // first is 1
                tocEntry.LinuxTocEntry.cdte_format = CDROM_LBA;
                ControlReadTocEntry(handle, ref tocEntry.LinuxTocEntry);
                tocEntries.Add(tocEntry);
            }
            var leadoutEntry = new LinuxDiscTocEntry();
            leadoutEntry.LinuxTocEntry.cdte_track = CDROM_LEADOUT;
	        leadoutEntry.LinuxTocEntry.cdte_format = CDROM_LBA;
            ControlReadTocEntry(handle, ref leadoutEntry.LinuxTocEntry);
            tocHeader.SetTrackInfo(tocEntries, leadoutEntry);
            return tocHeader;
        }

        internal static IDiscToc ReadToc(string filename)
        {
            int handle = 0;
            try
            {
                handle = OpenReadOnly(filename);
                return ReadTocInternal(handle);
            }
            finally
            {
                if (handle > 0)
                    NativeClose(handle);
            }
        }
    }
}
