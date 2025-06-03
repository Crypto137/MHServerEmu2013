using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.Entities;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes.Missions;
using MHServerEmu.Games.Regions;

namespace MHServerEmu.Games.Missions
{
    public class MissionManager : ISerialize
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private readonly Dictionary<PrototypeId, Mission> _missionDict = new();

        public Game Game { get; }
        public IMissionManagerOwner Owner { get; }

        public MissionManager(Game game, IMissionManagerOwner owner)
        {
            Game = game;
            Owner = owner;
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public bool Serialize(Archive archive)
        {
            bool success = true;
            // PlayerDbGuid serialization for persistent archives
            success &= SerializeMissions(archive);
            return success;
        }

        public bool InitializeForPlayer(Player player, Region region)
        {
            // V10_TODO
            foreach (PrototypeId missionProtoRef in DataDirectory.Instance.IteratePrototypesInHierarchy<MissionPrototype>(PrototypeIterateFlags.NoAbstractApprovedOnly))
            {
                MissionPrototype missionProto = missionProtoRef.As<MissionPrototype>();
                if (ShouldCreateMission(missionProto) == false)
                    continue;

                CreateMissionByDataRef(missionProtoRef);
            }

            return true;
        }

        /// <summary>
        /// Returns <see langword="true"/> if the provided <see cref="MissionPrototype"/> is valid for this <see cref="MissionManager"/> instance.
        /// </summary>
        public bool ShouldCreateMission(MissionPrototype missionProto)
        {
            // V10_TODO
            if (missionProto == null)
                return false;

            if (missionProto is OpenMissionPrototype)
                return false;

            return missionProto.ApprovedForUse();
        }

        private Mission CreateMissionByDataRef(PrototypeId missionRef)
        {
            var mission = CreateMission(missionRef);
            if (mission == null) return null;

            InsertMission(mission);
            
            // V10_TODO: Everything else

            return mission;
        }

        private Mission CreateMission(PrototypeId missionRef)
        {
            return new(this, missionRef);
        }

        private Mission InsertMission(Mission mission)
        {
            if (mission == null) return null;
            _missionDict.Add(mission.PrototypeDataRef, mission);
            return mission;
        }

        private bool SerializeMissions(Archive archive)
        {
            bool success = true;

            ulong numMissions = (ulong)_missionDict.Count;
            success &= Serializer.Transfer(archive, ref numMissions);

            // NOTE: Missions need to be packed with Serializer.Transfer() and NOT mission.Serialize()
            // for us to be able to skip invalid / deprecated / disabled missions.

            if (archive.IsPacking)
            {
                foreach (var kvp in _missionDict)
                {
                    Mission mission = kvp.Value;

                    ulong guid = (ulong)GameDatabase.GetPrototypeGuid(kvp.Key);
                    success &= Serializer.Transfer(archive, ref guid);

                    // skipping persistent stuff
                    
                    success &= Serializer.Transfer(archive, ref mission);
                }
            }
            else
            {
                for (ulong i = 0; i < numMissions; i++)
                {
                    ulong guid = 0;
                    success &= Serializer.Transfer(archive, ref guid);

                    PrototypeId missionRef = GameDatabase.GetDataRefByPrototypeGuid((PrototypeGuid)guid);

                    // skipping persistent stuff

                    // Restore the mission if nothing is wrong with it
                    Mission mission = CreateMission(missionRef);
                    success &= Serializer.Transfer(archive, ref mission);

                    InsertMission(mission);
                }
            }

            return success;
        }
    }
}
