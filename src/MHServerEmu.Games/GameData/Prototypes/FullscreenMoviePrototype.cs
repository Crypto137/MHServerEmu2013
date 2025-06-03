using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.GameData.Prototypes
{
    #region Enums

    [AssetEnum]
    public enum MovieType
    {
        None,
        Loading,
        TeleportFar,
        Cinematic,
    }

    #endregion

    public class FullscreenMoviePrototype : Prototype
    {
        public AssetId MovieName { get; protected set; }
        public bool Skippable { get; protected set; }
        public MovieType MovieType { get; protected set; }
        public bool ExitGameAfterPlay { get; protected set; }
        public LocaleStringId MovieTitle { get; protected set; }
        public AssetId Banter { get; protected set; }
    }

    public class KismetSequencePrototype : Prototype
    {
        public AssetId KismetSeqName { get; protected set; }
        public bool KismetSeqBlocking { get; protected set; }
        public bool AudioListenerAtCamera { get; protected set; }
    }
}
