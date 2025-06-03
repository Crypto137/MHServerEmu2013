namespace MHServerEmu.Games.Populations
{
    [Flags]
    public enum MarkerSetOptions
    {
        None = 0,
        NoOffset = 1,
        Default = 2,
        SpawnMissionAssociated = 4,
        NoSpawnMissionAssociated = 8
    }
}
