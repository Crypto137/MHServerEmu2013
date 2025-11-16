using Gazillion;
using MHServerEmu.Core.Config;
using MHServerEmu.Core.Extensions;
using MHServerEmu.Core.Logging;
using MHServerEmu.Core.System.Time;

namespace MHServerEmu.Games.MTXStore.Catalogs
{
    public class Catalog
    {
        private static readonly Logger Logger = LogManager.CreateLogger();

        private readonly Dictionary<long, CatalogEntry> _entries = new();
        private readonly LocalizedCatalogUrls _urls = new();    // Make this a collection if we ever implement locales other than en_us

        private NetMessageCatalogItems _cachedProtobuf;

        public TimeSpan Timestamp { get; private set; }
        
        public int Count { get => _entries.Count; }

        public Catalog() { }

        public void Initialize()
        {
            var config = ConfigManager.Instance.GetConfig<MTXStoreConfig>();

            _urls.LocaleId = "en_us";
            _urls.StoreHomePageUrl = config.HomePageUrl;
            _urls.StoreBannerPageUrls[(int)StoreBannerPage.Home]     = new() { Type = "Home",     Url = config.HomeBannerPageUrl };
            _urls.StoreBannerPageUrls[(int)StoreBannerPage.Heroes]   = new() { Type = "Heroes",   Url = config.HeroesBannerPageUrl };
            _urls.StoreBannerPageUrls[(int)StoreBannerPage.Costumes] = new() { Type = "Costumes", Url = config.CostumesBannerPageUrl };
            _urls.StoreBannerPageUrls[(int)StoreBannerPage.Boosts]   = new() { Type = "Boosts",   Url = config.BoostsBannerPageUrl };
            _urls.StoreBannerPageUrls[(int)StoreBannerPage.Chests]   = new() { Type = "Chests",   Url = config.ChestsBannerPageUrl };
            _urls.StoreBannerPageUrls[(int)StoreBannerPage.Specials] = new() { Type = "Specials", Url = config.SpecialsBannerPageUrl };
            _urls.StoreRealMoneyUrl = config.RealMoneyUrl;
        }

        public CatalogEntry GetEntry(long skuId)
        {
            if (_entries.TryGetValue(skuId, out CatalogEntry entry) == false)
                return null;

            return entry;
        }

        public void AddEntries(CatalogEntry[] newEntries)
        {
            if (newEntries.IsNullOrEmpty())
                return;

            // Overwrite entries with the same skuId
            foreach (CatalogEntry entry in newEntries)
            {
                long skuId = entry.SkuId;

                if (_entries.ContainsKey(skuId))
                    Logger.Trace($"Overriding SKU {skuId}");

                _entries[skuId] = entry;
            }

            FlagDirty();
        }

        public void ClearEntries()
        {
            _entries.Clear();

            FlagDirty();
        }

        public NetMessageCatalogItems ToProtobuf()
        {
            if (_cachedProtobuf == null)
            {
                _cachedProtobuf = NetMessageCatalogItems.CreateBuilder()
                    .SetTimestamp((long)Timestamp.TotalSeconds)
                    .AddRangeEntries(_entries.Values.Select(entry => entry.ToNetStruct()))
                    .AddUrls(_urls.ToNetStruct())
                    .Build();
            }

            return _cachedProtobuf;
        }

        private void FlagDirty()
        {
            // Microseconds are zero in our dump, so round down to milliseconds.
            Timestamp = TimeSpan.FromMilliseconds((long)Clock.UnixTime.TotalMilliseconds);

            // null out the cache, it will be rebuilt when the catalog is requested again.
            _cachedProtobuf = null;
        }
    }
}
