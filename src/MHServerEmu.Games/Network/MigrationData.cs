using MHServerEmu.Core.Config;
using MHServerEmu.Core.Serialization;
using MHServerEmu.Games.Common;
using MHServerEmu.Games.Entities;
using MHServerEmu.Games.Entities.Avatars;
using MHServerEmu.Games.Entities.Inventories;
using MHServerEmu.Games.Entities.Items;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;

namespace MHServerEmu.Games.Network
{
    /// <summary>
    /// Contains player data that needs to persist on migration (going from region to region), but not when logging out.
    /// </summary>
    public class MigrationData
    {
        private static ulong CurrentPlayerDbGuid = 0x2000000000000001;
        private static uint PlayerNumber;

        private readonly Dictionary<PrototypeId, byte[]> _archivedAvatars = new();
        private readonly Dictionary<PrototypeId, List<ArchivedEntity>> _archivedItems = new();

        public ulong PlayerDbGuid { get; private set; }
        public string PlayerName { get; private set; }

        public PrototypeId AvatarProtoRef { get; private set; }

        public MigrationData()
        {
        }

        public void Initialize()
        {
            GameConfig config = ConfigManager.Instance.GetConfig<GameConfig>();

            PlayerDbGuid = CurrentPlayerDbGuid++;
            PlayerName = $"Player{++PlayerNumber}";

            string defaultAvatarName = ConfigManager.Instance.GetConfig<GameConfig>().DefaultAvatar;
            PrototypeId defaultAvatarProtoRef = GameDatabase.GetPrototypeRefByName(defaultAvatarName);
            if (DataDirectory.Instance.PrototypeIsA<AvatarPrototype>(defaultAvatarProtoRef))
                AvatarProtoRef = defaultAvatarProtoRef;
        }

        public void Update(PlayerConnection playerConnection)
        {
            Player player = playerConnection.Player;
            Avatar currentAvatar = player.CurrentAvatar;

            AvatarProtoRef = currentAvatar != null ? currentAvatar.PrototypeDataRef : PrototypeId.Invalid;

            _archivedAvatars.Clear();
            _archivedItems.Clear();

            // Save player
            ArchiveItems(player);

            // Save avatars
            foreach (Avatar avatar in new AvatarIterator(playerConnection.Player))
            {
                using Archive archive = new(ArchiveSerializeType.Database);
                Serializer.Transfer(archive, avatar);
                _archivedAvatars[avatar.PrototypeDataRef] = archive.AccessAutoBuffer().ToArray();

                ArchiveItems(avatar);
            }
        }

        public bool ChangePlayerName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return false;

            if (newName.Length > 10)
                return false;

            if (newName.StartsWith("Player"))
                return false;

            PlayerName = newName;
            return true;
        }

        public bool TryGetArchivedAvatar(PrototypeId avatarProtoRef, out byte[] archiveData)
        {
            return _archivedAvatars.TryGetValue(avatarProtoRef, out archiveData);
        }

        public bool TryGetArchivedItems(PrototypeId ownerProtoRef, out IReadOnlyList<ArchivedEntity> archivedItems)
        {
            bool found = _archivedItems.TryGetValue(ownerProtoRef, out List<ArchivedEntity> mutableList);
            archivedItems = found ? mutableList : null;
            return found;
        }

        private void ArchiveItems(Entity entity)
        {
            List<ArchivedEntity> archivedItems = new();

            PrototypeId entityProtoRef = entity.PrototypeDataRef;
            EntityManager entityManager = entity.Game.EntityManager;

            foreach (Inventory inventory in new InventoryIterator(entity))
            {
                PrototypeId inventoryProtoRef = inventory.PrototypeDataRef;

                foreach (var entry in inventory)
                {
                    Item item = entityManager.GetEntity<Item>(entry.Id);
                    if (item == null)
                        continue;

                    using Archive archive = new(ArchiveSerializeType.Database);
                    Serializer.Transfer(archive, item);

                    ArchivedEntity archivedItem = new(inventoryProtoRef, entry.Slot, item.PrototypeDataRef, archive.AccessAutoBuffer().ToArray());
                    archivedItems.Add(archivedItem);
                }
            }

            if (archivedItems.Count > 0)
                _archivedItems[entityProtoRef] = archivedItems;
        }

        public readonly struct ArchivedEntity(PrototypeId inventoryProtoRef, uint slot, PrototypeId entityProtoRef, byte[] archiveData)
        {
            public readonly PrototypeId InventoryProtoRef = inventoryProtoRef;
            public readonly uint Slot = slot;
            public readonly PrototypeId EntityProtoRef = entityProtoRef;
            public readonly byte[] ArchiveData = archiveData;
        }
    }
}
