using OWML.Common;

namespace OWML.Patcher
{
    public class SectorInfo : ISectorInfo
    {
        public int SectorStart { get; }

        public int SectorSize { get; }

        public int SectorEnd => SectorStart + SectorSize;

        public SectorInfo(int sectorStart, int sectorSize)
        {
            SectorStart = sectorStart;
            SectorSize = sectorSize;
        }
    }
}
