using System.Text.Json.Serialization;
using Gazillion;
using MHServerEmu.Games.GameData;

namespace MHServerEmu.Games.MTXStore.Catalogs
{
    public class CatalogEntry
    {
        public long SkuId { get; set; }
        public PrototypeGuid[] ItemPrototypeGuids { get; set; }
        public PrototypeId[] ItemPrototypeRuntimeIdsForClient { get; set; }
        public LocalizedCatalogEntry[] LocalizedEntries { get; set; }
        public CatalogEntryType Type { get; set; }
        public CatalogEntryTypeModifier[] TypeModifiers { get; set; }

        [JsonConstructor]
        public CatalogEntry(long skuId, PrototypeGuid[] itemPrototypeGuids, PrototypeId[] itemPrototypeRuntimeIdsForClient, LocalizedCatalogEntry[] localizedEntries,
            CatalogEntryType type, CatalogEntryTypeModifier[] typeModifiers)
        {
            SkuId = skuId;
            ItemPrototypeGuids = itemPrototypeGuids;
            ItemPrototypeRuntimeIdsForClient = itemPrototypeRuntimeIdsForClient;
            LocalizedEntries = localizedEntries;
            Type = type;
            TypeModifiers = typeModifiers;
        }

        public CatalogEntry(MarvelHeroesCatalogEntry entry)
        {
            SkuId = entry.SkuId;
            ItemPrototypeGuids = entry.ItemPrototypeGuidsList.Select(guid => (PrototypeGuid)guid).ToArray();
            ItemPrototypeRuntimeIdsForClient = entry.ItemPrototypeRuntimeIdsForClientList.Select(id => (PrototypeId)id).ToArray();
            LocalizedEntries = entry.LocalizedEntriesList.Select(localizedEntry => new LocalizedCatalogEntry(localizedEntry)).ToArray();
            Type = new(entry.Type);
            TypeModifiers = entry.TypeModifierList.Select(typeModifier => new CatalogEntryTypeModifier(typeModifier)).ToArray();
        }

        public MarvelHeroesCatalogEntry ToNetStruct()
        {
            return MarvelHeroesCatalogEntry.CreateBuilder()
                .SetSkuId(SkuId)
                .AddRangeItemPrototypeGuids(ItemPrototypeGuids.Select(guid => (ulong)guid))
                .AddRangeItemPrototypeRuntimeIdsForClient(ItemPrototypeRuntimeIdsForClient.Select(id => (ulong)id))
                .AddRangeLocalizedEntries(LocalizedEntries.Select(localizedEntry => localizedEntry.ToNetStruct()))
                .SetType(Type.ToNetStruct())
                .AddRangeTypeModifier(TypeModifiers.Select(typeModifier => typeModifier.ToNetStruct()))
                .Build();
        }
    }
}
