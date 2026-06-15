namespace MHServerEmu.Games.GameData
{
    public enum PrototypeFieldType
    {
        Invalid = -1,
        Int8,
        Int16,
        Int32,
        Int64,
        Bool,
        UInt16,
        UInt32,
        UInt64,
        Float32,
        Float64,
        Text,                   // utf-8 prefixed by int32 length
        Enum,
        SymbolicBitSet,         // byte array of flag enums (e.g. NaviContentFlags in NaviPatchEdgePrototype)
        FunctionPtr,
        PrototypeDataRef,
        AssetRef,
        AssetTypeRef,
        CurveRef,
        Vector3,
        Point3,
        IPoint3,
        Point2,
        IPoint2,
        Orientation,
        Matrix4,
        Transform3,
        Aabb,
        LocaleStringId,
        PrototypeGuid,
        ResourceName,           // same as text
        Mixin,
        Prototype,
        PrototypePtr,
        PrototypeRefPtr,        // V10_NOTE: Does not exist in the 1.10 client.
        VectorPrototypeDataRef,
        ListPrototypeDataRef,
        VectorAssetDataRef,     // V10_NOTE: Does not exist in the 1.10 client.
        ListAssetRef,
        ListAssetTypeRef,
        ListBool,
        ListEnum,
        ListInt8,
        ListInt16,
        ListInt32,
        ListInt64,
        ListFloat32,
        ListFloat64,
        ListString,
        ListPrototypePtr,       // "Lists of PrototypePtrs are not parsed as a standard prototype field"
        ListMixin,              // "Mixin lists are not parsed as a standard prototype field"
        VectorPrototypePtr,     // "Vectors of PrototypePtrs are not parsed as a standard prototype field"
        VectorPrototypeRefPtr,  // V10_NOTE: Does not exist in the 1.10 client.
        UnkType52,
        Vector,
        PropertyId,
        PropertyCollection,     // "Property collections are not parsed as a standard prototype field"
        PropertyList,
    }
}
