using MHServerEmu.Games.GameData;
using MHServerEmu.Games.Properties;

namespace MHServerEmu.Games.Powers.Conditions
{
    public class Condition
    {
        // V10_TODO: Just a stub for evals for now
        public PropertyCollection Properties { get; } = new();

        public Condition()
        {

        }

        public PrototypeId[] GetKeywords()
        {
            return Array.Empty<PrototypeId>();
        }
    }
}
