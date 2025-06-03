using MHServerEmu.Games.GameData.Calligraphy.Attributes;

namespace MHServerEmu.Games.Properties.Evals
{
    [AssetEnum((int)Default)]
    public enum EvalContext
    {
        Default,
        Entity,
        EntityBehaviorBlackboard,
        Other,
        Condition,
        ConditionKeywords,
        Var1,
        Var2,
        Var3,
        Var4,
        Var5,
        MaxVars,
        LocalStack,
        CallerStack,
        Globals,
    }

    public enum GetEvalPropertyIdEnum
    {
        PropertyInfoEvalInput,
        Output,
        Input
    }

    public enum EvalOp
    {
        Invalid,
        And,
        Equals,
        GreaterThan,
        IsContextDataNull,
        LessThan,
        Not,
        Or,
        LoadAssetRef,
        LoadBool,
        LoadFloat,
        LoadInt,
        LoadProtoRef,
        LoadContextInt,
        LoadContextProtoRef,
        For,
        ForEachConditionInContext,
        ForEachProtoRefInContextRefList,
        IfElse,
        Scope,
        ExportError,
        LoadCurve,
        Add,
        Div,
        Exponent,
        Max,
        Min,
        Mult,
        Sub,
        AssignProp,
        AssignPropEvalParams,
        LoadProp,
        LoadPropContextParams,
        LoadPropEvalParams,
        RandomFloat,
        RandomInt,
        LoadEntityToContextVar,
        LoadConditionCollectionToContext,
    }

    public enum EvalReturnType
    {
        Error,
        Undefined,
        Int,
        Float,
        Bool,
        EntityId,
        RegionId,
        ProtoRef,
        AssetRef,
        PropertyCollectionPtr,
        PropertyId,
        ProtoRefListPtr,
        ProtoRefVectorPtr,
        ConditionCollectionPtr
    }
}
