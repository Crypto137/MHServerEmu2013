namespace MHServerEmu.Games.GameData.Prototypes
{
    public class DownloadChunkPrototype : Prototype
    {
        public PrototypeId Chapter { get; protected set; }
        public PrototypeId[] Regions { get; protected set; }
    }

    public class DownloadChunksPrototype : Prototype
    {
        public PrototypeId[] Chunks { get; protected set; }
    }
}
