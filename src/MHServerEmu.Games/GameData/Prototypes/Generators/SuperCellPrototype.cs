using MHServerEmu.Core.Extensions;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes.Generators
{
    public class SuperCellEntryPrototype : Prototype
    {
        public sbyte X { get; protected set; }
        public sbyte Y { get; protected set; }
        public AssetId Cell { get; protected set; }

        //---

        [DoNotCopy]
        public Point2 Offset { get => new(X, Y); }
    }

    public class SuperCellPrototype : Prototype
    {
        public SuperCellEntryPrototype[] Entries { get; protected set; }

        //---

        [DoNotCopy]
        public Point2 Max { get; private set; }

        public override void PostProcess()
        {
            base.PostProcess();

            Max = new(-1, -1);
            if (Entries.HasValue())
                foreach (SuperCellEntryPrototype superCellEntry in Entries)
                    if (superCellEntry != null)
                        Max = new(Math.Max(Max.X, superCellEntry.X), Math.Max(Max.Y, superCellEntry.Y));
        }

        public bool ContainsCell(PrototypeId cellRef)
        {
            if (Entries.HasValue())
                foreach (var entryProto in Entries)
                    if (entryProto != null && GameDatabase.GetDataRefByAsset(entryProto.Cell) == cellRef)
                        return true;
            return false;
        }
    }

}
