using Gazillion;
using MHServerEmu.Core.Memory;
using MHServerEmu.Core.VectorMath;
using MHServerEmu.Games.Entities.Avatars;
using MHServerEmu.Games.Entities.Inventories;
using MHServerEmu.Games.Entities.Items;
using MHServerEmu.Games.Entities;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.Network;
using MHServerEmu.Games.Properties;
using MHServerEmu.Games.Regions;

namespace MHServerEmu.Games.Commands
{
    public partial class CommandManagerMini
    {

        [Command("commands", "Prints available commands.")]
        private string PrintCommandList(string[] @params, PlayerConnection invoker)
        {
            foreach (var kvp in _commandDict)
                invoker.SendSystemChatMessage($"!{kvp.Key.Name} - {kvp.Key.Description}");

            return string.Empty;
        }

        [Command("avatar", "Switches the current avatar.")]
        private string SwitchAvatar(string[] @params, PlayerConnection invoker)
        {
            if (@params.Length == 0) return "Invalid parameters.";

            var matches = GameDatabase.SearchPrototypes(@params[0], DataFileSearchFlags.NoMultipleMatches | DataFileSearchFlags.CaseInsensitive, BlueprintId.Invalid,
                typeof(AvatarPrototype));

            if (matches.Any() == false)
                return $"No valid matches for {@params[0]}";

            PrototypeId avatarProtoRef = matches.First();

            Player player = invoker.Player;

            Avatar avatar = player.GetAvatar(avatarProtoRef);
            if (avatar == null)
                return $"Avatar {avatarProtoRef.GetName()} is not available";

            Region region = player.CurrentAvatar.Region;
            Vector3 position = player.CurrentAvatar.RegionLocation.Position;
            Orientation orientation = player.CurrentAvatar.RegionLocation.Orientation;

            Inventory avatarInPlay = invoker.Player.GetInventory(InventoryConvenienceLabel.AvatarInPlay);
            avatar.ChangeInventoryLocation(avatarInPlay, 0);
            avatar.EnterWorld(region, position, orientation);

            return $"Switched to {avatarProtoRef.GetName()}";
        }

        [Command("tower", "Return to Avengers Tower.")]
        private string ReturnToAvengersTower(string[] @params, PlayerConnection invoker)
        {
            invoker.MoveToTarget((PrototypeId)15718422430560164872);
            return string.Empty;
        }

        [Command("spawn", "Spawns an entity with the provided data file path (relative from the entity folder and without the file extension).")]
        private string SpawnEntity(string[] @params, PlayerConnection invoker)
        {
            if (@params.Length == 0)
                return "Invalid parameters.";

            /*
            string path = $"Entity/{@params[0]}.prototype";

            WorldEntity worldEntity = new(invoker.Game);
            worldEntity.Id = invoker.Game.NextEntityId;
            worldEntity.PrototypeDataRef = (PrototypeId)HashHelper.HashPath(path.ToCalligraphyPath());
            worldEntity.Properties[PropertyEnum.Health] = 100f;
            worldEntity.Properties[PropertyEnum.HealthMaxOther] = 100f;

            using (Archive archive = new(ArchiveSerializeType.Replication, (ulong)AOINetworkPolicyValues.AOIChannelProximity))
            {
                worldEntity.Serialize(archive);
                byte padding = 0;
                for (int i = 0; i < 256; i++)
                    archive.Transfer(ref padding);

                invoker.SendMessage(NetMessageEntityCreate.CreateBuilder()
                    .SetIdEntity(worldEntity.Id)
                    .SetPrototypeId((ulong)worldEntity.PrototypeDataRef)
                    .SetDbId(0)
                    .SetInterestPolicies((uint)AOINetworkPolicyValues.AOIChannelProximity)
                    .SetPosition(invoker.Player.CurrentAvatar.RegionLocation.Position.ToNetStructPoint3())
                    .SetOrientation(invoker.Player.CurrentAvatar.RegionLocation.Orientation.ToNetStructPoint3())
                    .SetArchiveData(archive.ToByteString())
                    .Build());
            }
            */

            return "Disabled";
        }

        [Command("giveitem", "Add the specified item to the inventory.")]
        private string GiveItem(string[] @params, PlayerConnection invoker)
        {
            if (@params.Length == 0)
                return "Invalid parameters.";

            var matches = GameDatabase.SearchPrototypes(@params[0], DataFileSearchFlags.NoMultipleMatches | DataFileSearchFlags.CaseInsensitive, BlueprintId.Invalid,
                typeof(ItemPrototype));

            if (matches.Any() == false)
                return $"No valid matches for {@params[0]}";

            Inventory generalInv = invoker.Player.GetInventory(InventoryConvenienceLabel.General);

            PrototypeId itemProtoRef = matches.First();
            PrototypeId rarityProtoRef = (PrototypeId)10195041726035595077;
            int itemLevel = 1;
            int seed = Game.Current.Random.Next();

            ItemSpec itemSpec = new(itemProtoRef, rarityProtoRef, itemLevel, 0, null, seed, PrototypeId.Invalid);

            using EntitySettings settings = ObjectPoolManager.Instance.Get<EntitySettings>();
            settings.EntityRef = itemProtoRef;
            settings.ItemSpec = itemSpec;
            settings.InventoryLocation = new(generalInv.OwnerId, generalInv.PrototypeDataRef);
            Item item = invoker.Game.EntityManager.CreateEntity(settings) as Item;

            return $"Created item {item.PrototypeDataRef.GetName()}";
        }

        [Command("position", "Prints current position and orientation.")]
        private string PrintPosition(string[] @params, PlayerConnection invoker)
        {
            return invoker.Player.CurrentAvatar.RegionLocation.ToString();
        }

        [Command("tp", "Teleports to the specified coordinates (tp x:+1000 (relative to current position, tp x100 y500 z10 (absolute position)).")]
        private string Teleport(string[] @params, PlayerConnection invoker)
        {
            if (@params.Length == 0) return "Invalid arguments.";

            Avatar avatar = invoker.Player.CurrentAvatar;

            float x = 0f, y = 0f, z = 0f;
            foreach (string param in @params)
            {
                switch (param[0])
                {
                    case 'x':
                        if (float.TryParse(param.AsSpan(1), out x) == false) x = 0f;
                        break;

                    case 'y':
                        if (float.TryParse(param.AsSpan(1), out y) == false) y = 0f;
                        break;

                    case 'z':
                        if (float.TryParse(param.AsSpan(1), out z) == false) z = 0f;
                        break;

                    default:
                        return $"Invalid parameter: {param}";
                }
            }

            Vector3 teleportPoint = new(x, y, z);

            if (@params.Length < 3)
                teleportPoint += avatar.RegionLocation.Position;

            avatar.ChangeRegionPosition(teleportPoint, null, ChangePositionFlags.Teleport);

            return $"Teleporting to {teleportPoint.ToStringNames()}.";
        }

        [Command("startraftride", "Starts the funicular scripted sequence in the Raft.")]
        private string StartRaftFunicularRide(string[] @params, PlayerConnection invoker)
        {
            if (invoker.AOI.Region.PrototypeDataRef != (PrototypeId)9473360989573354222)
                return "You must be in the Raft to invoke this command.";

            invoker.PlayKismetSeq(9329157849119332306);

            return string.Empty;
        }

        [Command("damage", "Damages the current avatar.")]
        private string Damage(string[] @params, PlayerConnection invoker)
        {
            Avatar avatar = invoker.Player.CurrentAvatar;

            float health = avatar.Properties[PropertyEnum.Health];
            if (health > 0f)
            {
                health = Math.Max(health - 100f, 0f);
                avatar.Properties[PropertyEnum.Health] = health;

                if (health == 0f)
                {
                    invoker.SendMessage(NetMessageEntityKill.CreateBuilder()
                        .SetIdEntity(avatar.Id)
                        .SetIdKillerEntity(avatar.Id)
                        .SetKillFlags(0)
                        .Build());
                }
            }

            return "Taken damage.";
        }

        [Command("name", "Changes name to the provided value.")]
        private string Name(string[] @params, PlayerConnection invoker)
        {
            if (@params.Length == 0)
                return "Invalid parameters.";

            if (invoker.MigrationData.ChangePlayerName(@params[0]) == false)
                return $"Failed to change name to '{@params[0]}'.";

            invoker.MoveToTarget(GameDatabase.GlobalsPrototype.DefaultStartTarget);
            return string.Empty;
        }

        [Command("test", "Runs test code.")]
        private string Test(string[] @params, PlayerConnection invoker)
        {
            return "Test command invoked.";
        }
    }
}
