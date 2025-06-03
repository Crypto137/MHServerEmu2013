using Gazillion;
using MHServerEmu.Core.System.Time;
using MHServerEmu.Games.GameData;
using MHServerEmu.Games.GameData.Prototypes;

namespace MHServerEmu.Games.MTXStore
{
    public class CatalogManager
    {
        private readonly Dictionary<long, PrototypeId> _skuDict = new();

        private NetMessageCatalogItems _cachedCatalogItems = null;

        public CatalogManager()
        {
        }

        public NetMessageCatalogItems GetCatalogItems()
        {
            if (_cachedCatalogItems == null)
                CacheCatalogItems();

            return _cachedCatalogItems;
        }

        public PrototypeId GetItemProtoRefForSku(long skuId)
        {
            if (_skuDict.TryGetValue(skuId, out PrototypeId itemProtoRef) == false)
                return PrototypeId.Invalid;

            return itemProtoRef;
        }

        private void CacheCatalogItems()
        {
            _skuDict.Clear();

            var builder = NetMessageCatalogItems.CreateBuilder()
                .SetTimestamp((long)Clock.UnixTime.TotalSeconds);

            long currentSkuId = 1;

            foreach (PrototypeId avatarProtoRef in DataDirectory.Instance.IteratePrototypesInHierarchy<AvatarPrototype>(PrototypeIterateFlags.NoAbstractApprovedOnly))
            {
                string avatarName = Path.GetFileNameWithoutExtension(avatarProtoRef.GetName());
                _skuDict.Add(currentSkuId, avatarProtoRef);

                builder.AddEntries(MarvelHeroesCatalogEntry.CreateBuilder()
                    .SetSkuId(currentSkuId++)
                    .AddItemPrototypeGuids(0)
                    .AddItemPrototypeRuntimeIdsForClient((ulong)avatarProtoRef)
                    .AddLocalizedEntries(MHLocalizedCatalogEntry.CreateBuilder()
                        .SetLanguageId("en_us")
                        .SetDescription(avatarName)
                        .SetTitle(avatarName)
                        .SetReleaseDate(string.Empty)
                        .SetItemPrice(450))
                    .SetType(MHCatalogEntryType.CreateBuilder()
                        .SetName("Hero")
                        .SetOrder(0)));
            }

            foreach (PrototypeId costumeProtoRef in DataDirectory.Instance.IteratePrototypesInHierarchy<CostumePrototype>(PrototypeIterateFlags.NoAbstractApprovedOnly))
            {
                string costumeName = Path.GetFileNameWithoutExtension(costumeProtoRef.GetName());
                _skuDict.Add(currentSkuId, costumeProtoRef);

                builder.AddEntries(MarvelHeroesCatalogEntry.CreateBuilder()
                    .SetSkuId(currentSkuId++)
                    .AddItemPrototypeGuids(0)
                    .AddItemPrototypeRuntimeIdsForClient((ulong)costumeProtoRef)
                    .AddLocalizedEntries(MHLocalizedCatalogEntry.CreateBuilder()
                        .SetLanguageId("en_us")
                        .SetDescription(costumeName)
                        .SetTitle(costumeName)
                        .SetReleaseDate(string.Empty)
                        .SetItemPrice(450))
                    .SetType(MHCatalogEntryType.CreateBuilder()
                        .SetName("Costume")
                        .SetOrder(1)));
            }

            builder.AddUrls(MHLocalizedCatalogUrls.CreateBuilder()
                .SetLocaleId("en_us")
                .SetStoreHomePageUrl("http://localhost/store")
                .AddStoreBannerPageUrls(MHBannerUrl.CreateBuilder()
                    .SetType("Home")
                    .SetUrl("http://localhost/store/images/banner.png"))
                .AddStoreBannerPageUrls(MHBannerUrl.CreateBuilder()
                    .SetType("Heroes")
                    .SetUrl("http://localhost/store/images/banner.png"))
                .AddStoreBannerPageUrls(MHBannerUrl.CreateBuilder()
                    .SetType("Costumes")
                    .SetUrl("http://localhost/store/images/banner.png"))
                .AddStoreBannerPageUrls(MHBannerUrl.CreateBuilder()
                    .SetType("Boosts")
                    .SetUrl("http://localhost/store/images/banner.png"))
                .AddStoreBannerPageUrls(MHBannerUrl.CreateBuilder()
                    .SetType("Chests")
                    .SetUrl("http://localhost/store/images/banner.png"))
                .AddStoreBannerPageUrls(MHBannerUrl.CreateBuilder()
                    .SetType("Specials")
                    .SetUrl("http://localhost/store/images/banner.png"))
                .SetStoreRealMoneyUrl("http://localhost/store/gs-bundles.html"));

            _cachedCatalogItems = builder.Build();
        }
    }
}
