namespace MHServerEmu.Games.GameData.Prototypes
{
    public class PlayerPrototype : EntityPrototype
    {
        public PrototypeId[] StartingAvatarLibrary { get; protected set; }
        public float CameraFacingDirX { get; protected set; }
        public float CameraFacingDirY { get; protected set; }
        public float CameraFacingDirZ { get; protected set; }
        public float CameraFOV { get; protected set; }
        public float CameraZoomDistance { get; protected set; }
        public float ResurrectWaitTime { get; protected set; }
        public int NumConsumableSlots { get; protected set; }
        public PrototypeId[] StartingPlayableAvatars { get; protected set; }
        public AbilityAssignmentPrototype[] StartingEmotes { get; protected set; }
        public bool AutoSelectStartingAvatar { get; protected set; }
        public int StartingCredits { get; protected set; }
        public EntityInventoryAssignmentPrototype[] StashInventories { get; protected set; }
        public int MaxDroppedItems { get; protected set; }
    }
}
