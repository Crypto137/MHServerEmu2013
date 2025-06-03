using MHServerEmu.Core.Serialization;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.Entities;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.Missions
{
    [AssetEnum((int)Invalid)]
    public enum MissionState
    {
        Invalid,
        Inactive,
        Available,
        Active,
        Completed,
        Failed,
    }

    public class Mission : ISerialize
    {
        private MissionState _state = MissionState.Active;
        private TimeSpan _timeExpireCurrentState;
        private PrototypeId _prototypeDataRef;
        private int _lootSeed;
        private SortedSet<ulong> _participants = new();

        public MissionState State { get => _state; }
        public PrototypeId PrototypeDataRef { get => _prototypeDataRef; }
        public MissionManager MissionManager { get; }
        public Game Game { get; }

        public Mission(MissionManager missionManager, PrototypeId missionRef)
        {
            MissionManager = missionManager;
            Game = missionManager.Game;

            _prototypeDataRef = missionRef;

            if (missionManager.Owner is Player player)
                _participants.Add(player.Id);
        }

        public override string ToString()
        {
            return PrototypeDataRef.GetName();
        }

        public bool Serialize(Archive archive)
        {
            bool success = true;

            int state = (int)_state;
            success &= Serializer.Transfer(archive, ref state);
            _state = (MissionState)state;

            success &= Serializer.Transfer(archive, ref _timeExpireCurrentState);
            success &= Serializer.Transfer(archive, ref _prototypeDataRef);

            // V10_TODO: 1.10 has a Map<ulong, ItemSpec> here with a currently unknown key (index? id?)
            ulong numItemSpecs = 0;
            success &= Serializer.Transfer(archive, ref numItemSpecs);

            success &= Serializer.Transfer(archive, ref _lootSeed);

            if (archive.IsReplication)
            {
                // V10_NOTE: No suspension status in 1.10 
                success &= SerializeObjectives(archive);
                success &= Serializer.Transfer(archive, ref _participants);
            }

            return success;
        }

        private bool SerializeObjectives(Archive archive)
        {
            bool success = true;

            // TODO
            ulong numObjectives = 0;
            success &= Serializer.Transfer(archive, ref numObjectives);

            return success;

        }
    }
}
