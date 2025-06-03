using System.Collections;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.Powers.Conditions;

namespace MHServerEmu.Games.Entities
{
    public class ConditionCollection : ISerialize, IEnumerable<Condition>
    {
        private readonly WorldEntity _owner;

        public ConditionCollection(WorldEntity owner)
        {
            _owner = owner;
        }

        public bool Serialize(Archive archive)
        {
            bool success = true;

            if (archive.IsTransient)
            {
                ulong repId = 0;

                Serializer.Transfer(archive, ref repId);    // RepId for the ConditionCollection itself
                Serializer.Transfer(archive, ref repId);    // RepId for RepMap<RepVar<ulong>, Condition>

                uint numConditions = 0;
                Serializer.Transfer(archive, ref numConditions);

                /* Condition structure
                ReplicationId
                RepVar<ulong>               - CreatorId?
                RepVar<ulong>               - UltimateCreatorId?
                RepVar<PrototypeDataRef>    - ConditionProtoRef?
                RepVar<byte>                - CreatorPowerIndex?
                RepVar<PrototypeDataRef>    - CreatorPowerRef?
                RepVar<AssetDataRef>        - OwnerAssetRef?
                RepVar<GameTime>            - StartTime?
                RepVar<ulong>
                RepVar<ulong>
                ReplicatedPropertyCollection
                RepVar<uint>                - CancelOnFlags?
                */
            }

            return success;
        }

        public IEnumerator<Condition> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
