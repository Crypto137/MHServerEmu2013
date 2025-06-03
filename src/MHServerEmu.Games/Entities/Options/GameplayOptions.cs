using MHServerEmu.Core.Serialization;
using MHServerEmu.Games.Common;

namespace MHServerEmu.Games.Entities.Options
{
    public class GameplayOptions : ISerialize
    {
        private bool _autoPartyEnabled;
        private bool _preferLowPopulationRegions;

        public bool AutoPartyEnabled { get => _autoPartyEnabled; set => _autoPartyEnabled = value; }
        public bool PreferLowPopulationRegions { get => _preferLowPopulationRegions; set => _preferLowPopulationRegions = value; }

        public GameplayOptions()
        {
        }

        public override string ToString()
        {
            return $"{nameof(AutoPartyEnabled)}={AutoPartyEnabled}, {nameof(PreferLowPopulationRegions)}={PreferLowPopulationRegions}";
        }

        public bool Serialize(Archive archive)
        {
            bool success = true;
            success &= Serializer.Transfer(archive, ref _autoPartyEnabled);
            success &= Serializer.Transfer(archive, ref _preferLowPopulationRegions);
            return success;
        }
    }
}
