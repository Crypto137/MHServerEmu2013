﻿using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.Network;
using MHServerEmu.Games.Properties;

namespace MHServerEmu.Games.GameData.Prototypes
{
    public class PropertyInfoPrototype : Prototype
    {
        public sbyte Version { get; protected set; }
        public AggregationMethod AggMethod { get; protected set; }
        public float Min { get; protected set; }
        public float Max { get; protected set; }
        public DatabasePolicy ReplicateToDatabase { get; protected set; }
        public bool ReplicateToProximity { get; protected set; }
        public bool ReplicateToParty { get; protected set; }
        public bool ReplicateToOwner { get; protected set; }
        public bool ReplicateToDiscovery { get; protected set; }
        public bool ReplicateForTransfer { get; protected set; }
        public PropertyDataType Type { get; protected set; }
        public PropertyFormulaPrototype Formula { get; protected set; }
        public float CurveDefault { get; protected set; }
        public bool ReplicateToDatabaseAllowedOnItems { get; protected set; }
        public bool ClientOnly { get; protected set; }
        public bool SerializeEntityToPowerPayload { get; protected set; }
        public bool SerializePowerToPowerPayload { get; protected set; }
        public PrototypeId TooltipText { get; protected set; }
        public bool TruncatePropertyValueToInt { get; protected set; }
        public EvalPrototype Eval { get; protected set; }
        public bool EvalAlwaysCalculates { get; protected set; }
        public bool SerializeConditionSrcToCondition { get; protected set; }

        //---

        [DoNotCopy]
        public AOINetworkPolicyValues RepNetwork { get; private set; } = AOINetworkPolicyValues.AOIChannelNone;
        [DoNotCopy]
        public bool ShouldClampValue { get => Min != 0f || Max != 0f; }

        public override void PostProcess()
        {
            base.PostProcess();

            // Reconstruct AOI network policy
            if (ReplicateToProximity)
                RepNetwork |= AOINetworkPolicyValues.AOIChannelProximity;

            if (ReplicateToParty)
                RepNetwork |= AOINetworkPolicyValues.AOIChannelParty;

            if (ReplicateToOwner)
                RepNetwork |= AOINetworkPolicyValues.AOIChannelOwner;

            if (ReplicateToDiscovery)
                RepNetwork |= AOINetworkPolicyValues.AOIChannelDiscovery;
        }
    }
}
