using Gazillion;
using MHServerEmu.Core.Extensions;
using MHServerEmu.Core.Helpers;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.Memory;
using MHServerEmu.Games.Entities;
using MHServerEmu.Games.Entities.Inventories;
using MHServerEmu.Games.Entities.Items;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;
using MHServerEmu.Games.MTXStore.Catalogs;

namespace MHServerEmu.Games.MTXStore
{
    public class CatalogManager
    {
        private static readonly Logger Logger = LogManager.CreateLogger();
        private static readonly string MTXStoreDataDirectory = Path.Combine(FileHelper.DataDirectory, "Game", "MTXStore");

        private readonly Catalog _catalog = new();

        public static CatalogManager Instance { get; } = new();

        private CatalogManager() { }

        public bool Initialize()
        {
            if (_catalog.Count != 0)
                return false;

            _catalog.Initialize();
            LoadEntries();

            return true;
        }

        public void LoadEntries()
        {
            lock (_catalog)
            {
                _catalog.ClearEntries();

                // V10_REMOVEME
                _catalog.AddEntries(TEMP_GenerateCatalogEntries());

                foreach (string filePath in FileHelper.GetFilesWithPrefix(MTXStoreDataDirectory, "Catalog", "json"))
                {
                    CatalogEntry[] entries = FileHelper.DeserializeJson<CatalogEntry[]>(filePath);
                    _catalog.AddEntries(entries);
                    Logger.Trace($"Parsed catalog entries from {Path.GetFileName(filePath)}");
                }

                Logger.Info($"Loaded {_catalog.Count} store catalog entries");
            }
        }

        private static CatalogEntry[] TEMP_GenerateCatalogEntries()
        {
            List<CatalogEntry> entries = new();

            long currentSkuId = 10000000;

            foreach (PrototypeId avatarProtoRef in DataDirectory.Instance.IteratePrototypesInHierarchy<AvatarPrototype>(PrototypeIterateFlags.NoAbstractApprovedOnly))
            {
                CatalogEntry entry = TEMP_GenerateCatalogEntry(ref currentSkuId, avatarProtoRef, 600, "Hero", 0);
                entries.Add(entry);
            }

            foreach (PrototypeId costumeProtoRef in DataDirectory.Instance.IteratePrototypesInHierarchy<CostumePrototype>(PrototypeIterateFlags.NoAbstractApprovedOnly))
            {
                CatalogEntry entry = TEMP_GenerateCatalogEntry(ref currentSkuId, costumeProtoRef, 1000, "Costume", 1);
                entries.Add(entry);
            }

            PrototypeId[] items = [
                (PrototypeId)11922313247660511208,  // Pet001OldLace
                (PrototypeId)14065705819888423186,  // Pet002IronManMk1
                (PrototypeId)2488815647894081414,   // Pet004Herbie
                (PrototypeId)6091493728158291712,   // CosmicKey
                (PrototypeId)10473154902158743741,  // UnstableMolecules
                (PrototypeId)9317172445828291409,   // RespecSingleUse
                (PrototypeId)11258256079380027999,  // ShopBoostExp1
                (PrototypeId)4454097736648432738,   // ShopBoostItemFind1
#if !BUILD_1_10_0_69
                (PrototypeId)6736197208452963392,   // ShopSummonCrafter
                (PrototypeId)4932589029459303292,   // ShopSummonStash
                (PrototypeId)13948859884612492703,  // ShopSummonVendorJunk
#endif
            ];

            foreach (PrototypeId itemProtoRef in items)
            {
                CatalogEntry entry = TEMP_GenerateCatalogEntry(ref currentSkuId, itemProtoRef, 100, "Boost", 5);
                entries.Add(entry);
            }

            PrototypeId[] chests = [
                (PrototypeId)540300255616311629,    // MysteryChestFortuneCard
            ];

            foreach (PrototypeId itemProtoRef in chests)
            {
                CatalogEntry entry = TEMP_GenerateCatalogEntry(ref currentSkuId, itemProtoRef, 100, "Chest", 5);
                entries.Add(entry);
            }

            Logger.Debug($"Generated {entries.Count} catalog entries");
            return entries.ToArray();
        }

        private static CatalogEntry TEMP_GenerateCatalogEntry(ref long currentSkuId, PrototypeId itemProtoRef, long price, string type, int order)
        {
            string calligraphyPath = itemProtoRef.GetName().ToCalligraphyPath();

            return new(
                currentSkuId++,
                [0],
                [itemProtoRef],
                [new("en_us", calligraphyPath, calligraphyPath, string.Empty, price)],
                new(type, order),
                []
            );
        }

        #region Message Handling

        public bool OnGetCatalog(Player player, NetMessageGetCatalog getCatalog)
        {
            // Send the catalog only if the client is out of date.
            TimeSpan clientTimestamp = TimeSpan.FromSeconds(getCatalog.Timestamp);

            lock (_catalog)
            {
                if (clientTimestamp != _catalog.Timestamp)
                    player.SendMessage(_catalog.ToProtobuf());
            }

            return true;
        }

        public bool OnGetCurrencyBalance(Player player)
        {
            player.SendMessage(NetMessageGetCurrencyBalanceResponse.CreateBuilder()
                .SetCurrencyBalance(player.GazillioniteBalance)
                .Build());

            return true;
        }

        public bool OnBuyItemFromCatalog(Player player, NetMessageBuyItemFromCatalog buyItemFromCatalog)
        {
            if (buyItemFromCatalog.HasSkuId == false)
                return Logger.WarnReturn(false, $"OnBuyItemFromCatalog(): No SkuId received from player [{player}]");

            long skuId = buyItemFromCatalog.SkuId;
            long clientPrice = buyItemFromCatalog.ItemUnitPrice;

            // In normal non-gift purchases the buyer is the recipient
            BuyItemResultErrorCodes result = BuyItem(player, skuId, clientPrice);

            player.SendMessage(NetMessageBuyItemFromCatalogResponse.CreateBuilder()
                .SetDidSucceed(result == BuyItemResultErrorCodes.BUY_RESULT_ERROR_SUCCESS)
                .SetCurrentCurrencyBalance(player.GazillioniteBalance)
                .SetErrorcode(result)
                .SetSkuId(skuId)
                .Build());

            return true;
        }

        #endregion

        private BuyItemResultErrorCodes BuyItem(Player player, long skuId, long clientPrice)
        {
            // Validate the order
            CatalogEntry entry = null;

            lock (_catalog)
                entry = _catalog.GetEntry(skuId);

            if (entry == null || entry.ItemPrototypeRuntimeIdsForClient.IsNullOrEmpty() || entry.LocalizedEntries.IsNullOrEmpty())
                return BuyItemResultErrorCodes.BUY_RESULT_ERROR_UNKNOWN;

            // V10_TODO: proper purchasing
            Game game = Game.Current;

            Prototype itemProto = entry.ItemPrototypeRuntimeIdsForClient[0].As<Prototype>();
            if (itemProto is not ItemPrototype)
                return BuyItemResultErrorCodes.BUY_RESULT_ERROR_UNKNOWN;

            Inventory generalInv = player.GetInventory(InventoryConvenienceLabel.General);

            PrototypeId itemProtoRef = itemProto.DataRef;
            PrototypeId rarityProtoRef = (PrototypeId)10195041726035595077;
            int itemLevel = 1;
            int seed = game.Random.Next();

            ItemSpec itemSpec = new(itemProtoRef, rarityProtoRef, itemLevel, 0, null, seed, PrototypeId.Invalid);

            using EntitySettings settings = ObjectPoolManager.Instance.Get<EntitySettings>();
            settings.EntityRef = itemProtoRef;
            settings.ItemSpec = itemSpec;
            settings.InventoryLocation = new(generalInv.OwnerId, generalInv.PrototypeDataRef);
            Item item = game.EntityManager.CreateEntity(settings) as Item;

            return BuyItemResultErrorCodes.BUY_RESULT_ERROR_SUCCESS;
        }
    }
}
