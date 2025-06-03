using MHServerEmu.Core.Extensions;
using MHServerEmu.Games.GameData.Calligraphy.Attributes;
using MHServerEmu.Games.Properties;
using MHServerEmu.Games.Properties.Evals;
using System.Text;

namespace MHServerEmu.Games.GameData.Prototypes
{
    public class EvalNodeVisitor
    {
        public virtual void Visit(EvalPrototype visitor) { }
    }

    public class EvalPrototype : Prototype
    {
        //---

        [DoNotCopy]
        public EvalOp Op { get; protected set; }

        public virtual void Visit(EvalNodeVisitor visitor)
        {
            visitor.Visit(this);
        }

        public virtual string ExpressionString()
        {
            return string.Empty;
        }
    }

    public class ExportErrorPrototype : EvalPrototype
    {
        //---
        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.ExportError;
        }

        public override string ExpressionString()
        {
            return "ExportError";
        }
    }

    public class AssignPropPrototype : EvalPrototype
    {
        public EvalContext Context { get; protected set; }
        public PropertyId Prop { get; protected set; }
        public EvalPrototype Eval { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.AssignProp;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Eval?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            PropertyEnum propertyEnum = Prop.Enum;

            if (propertyEnum == PropertyEnum.Invalid)
                return $"!PropError! = {(Eval != null ? Eval.ExpressionString() : "NULL")}";

            PropertyInfo info = GameDatabase.PropertyInfoTable.LookupPropertyInfo(propertyEnum);
            string propertyName = info.BuildPropertyName(Prop);

            return string.Format("{0} = {1}",
                string.IsNullOrWhiteSpace(propertyName) == false ? propertyName : "!PropError!",
                Eval != null ? Eval.ExpressionString() : "NULL");
        }
    }

    public class AssignPropEvalParamsPrototype : EvalPrototype
    {
        public EvalContext Context { get; protected set; }
        public EvalPrototype Param0 { get; protected set; }
        public EvalPrototype Param1 { get; protected set; }
        public EvalPrototype Param2 { get; protected set; }
        public EvalPrototype Param3 { get; protected set; }
        public PrototypeId Prop { get; protected set; }
        public EvalPrototype Eval { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.AssignPropEvalParams;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Param0?.Visit(visitor);
            Param1?.Visit(visitor);
            Param2?.Visit(visitor);
            Param3?.Visit(visitor);
            Eval?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            PropertyEnum propertyEnum = GameDatabase.PropertyInfoTable.GetPropertyEnumFromPrototype(Prop);

            if (propertyEnum == PropertyEnum.Invalid)
                return $"!PropError! = {(Eval != null ? Eval.ExpressionString() : "NULL")}";

            PropertyInfo info = GameDatabase.PropertyInfoTable.LookupPropertyInfo(propertyEnum);
            return string.Format("{0}({1},{2},{3},{4}) = {5}",
                info.PropertyName,
                Param0?.ExpressionString(),
                Param1?.ExpressionString(),
                Param2?.ExpressionString(),
                Param3?.ExpressionString(),
                Eval != null ? Eval.ExpressionString() : "NULL");
        }
    }

    public class LoadPropPrototype : EvalPrototype
    {
        public EvalContext Context { get; protected set; }
        public PropertyId Prop { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadProp;
        }

        public override string ExpressionString()
        {
            PropertyEnum propertyEnum = Prop.Enum;

            if (propertyEnum == PropertyEnum.Invalid)
                return "!PropError!";

            PropertyInfo info = GameDatabase.PropertyInfoTable.LookupPropertyInfo(propertyEnum);
            return info.BuildPropertyName(Prop);
        }
    }

    public class LoadPropContextParamsPrototype : EvalPrototype
    {
        public EvalContext PropertyCollectionContext { get; protected set; }
        public PrototypeId Prop { get; protected set; }
        public EvalContext PropertyIdContext { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadPropContextParams;
        }

        public override string ExpressionString()
        {
            PropertyEnum propertyEnum = GameDatabase.PropertyInfoTable.GetPropertyEnumFromPrototype(Prop);

            if (propertyEnum == PropertyEnum.Invalid)
                return "!PropError!";

            PropertyInfo info = GameDatabase.PropertyInfoTable.LookupPropertyInfo(propertyEnum);
            return $"{info.PropertyName}(<PropColContext[{PropertyCollectionContext}], PropIdContext[{PropertyIdContext}]>)";
        }
    }

    public class LoadPropEvalParamsPrototype : EvalPrototype
    {
        public EvalContext Context { get; protected set; }
        public EvalPrototype Param0 { get; protected set; }
        public EvalPrototype Param1 { get; protected set; }
        public EvalPrototype Param2 { get; protected set; }
        public EvalPrototype Param3 { get; protected set; }
        public PrototypeId Prop { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadPropEvalParams;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Param0?.Visit(visitor);
            Param1?.Visit(visitor);
            Param2?.Visit(visitor);
            Param3?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            PropertyEnum propertyEnum = GameDatabase.PropertyInfoTable.GetPropertyEnumFromPrototype(Prop);

            if (propertyEnum == PropertyEnum.Invalid)
                return "!PropError!";

            PropertyInfo info = GameDatabase.PropertyInfoTable.LookupPropertyInfo(propertyEnum);
            return string.Format("{0}({1},{2},{3},{4})",
                info.PropertyInfoName,
                Param0?.ExpressionString(),
                Param1?.ExpressionString(),
                Param2?.ExpressionString(),
                Param3?.ExpressionString());
        }
    }

    public class LoadBoolPrototype : EvalPrototype
    {
        public bool Value { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadBool;
        }

        public override string ExpressionString()
        {
            return $"{Value}";
        }
    }

    public class LoadIntPrototype : EvalPrototype
    {
        public int Value { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadInt;
        }

        public override string ExpressionString()
        {
            return $"{Value}";
        }
    }

    public class LoadFloatPrototype : EvalPrototype
    {
        public float Value { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadFloat;
        }

        public override string ExpressionString()
        {
            return $"{Value}";
        }
    }

    public class LoadCurvePrototype : EvalPrototype
    {
        public CurveId Curve { get; protected set; }
        public EvalPrototype Index { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadCurve;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Index?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("( {0}[{1}] )",
                GameDatabase.GetCurveName(Curve),
                Index != null ? Index.ExpressionString() : "NULL");
        }
    }

    public class LoadAssetRefPrototype : EvalPrototype
    {
        public AssetId Value { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadAssetRef;
        }

        public override string ExpressionString()
        {
            return $"({GameDatabase.GetAssetName(Value)})";
        }
    }

    public class LoadProtoRefPrototype : EvalPrototype
    {
        public PrototypeId Value { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadProtoRef;
        }

        public override string ExpressionString()
        {
            return $"({GameDatabase.GetPrototypeName(Value)})";
        }
    }

    public class LoadContextIntPrototype : EvalPrototype
    {
        public EvalContext Context { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadContextInt;
        }

        public override string ExpressionString()
        {
            return $"(context<int>[{Context}])";
        }
    }

    public class LoadContextProtoRefPrototype : EvalPrototype
    {
        public EvalContext Context { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadContextProtoRef;
        }

        public override string ExpressionString()
        {
            return $"(context<protoref>[{Context}])";
        }
    }

    public class AddPrototype : EvalPrototype
    {
        public EvalPrototype Arg1 { get; protected set; }
        public EvalPrototype Arg2 { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.Add;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Arg1?.Visit(visitor);
            Arg2?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("( {0} + {1} )",
                Arg1 != null ? Arg1.ExpressionString() : "NULL",
                Arg2 != null ? Arg2.ExpressionString() : "NULL");
        }
    }

    public class SubPrototype : EvalPrototype
    {
        public EvalPrototype Arg1 { get; protected set; }
        public EvalPrototype Arg2 { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.Sub;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Arg1?.Visit(visitor);
            Arg2?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("( {0} - {1} )",
                Arg1 != null ? Arg1.ExpressionString() : "NULL",
                Arg2 != null ? Arg2.ExpressionString() : "NULL");
        }
    }

    public class MultPrototype : EvalPrototype
    {
        public EvalPrototype Arg1 { get; protected set; }
        public EvalPrototype Arg2 { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.Mult;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Arg1?.Visit(visitor);
            Arg2?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("( {0} * {1} )",
                Arg1 != null ? Arg1.ExpressionString() : "NULL",
                Arg2 != null ? Arg2.ExpressionString() : "NULL");
        }
    }

    public class DivPrototype : EvalPrototype
    {
        public EvalPrototype Arg1 { get; protected set; }
        public EvalPrototype Arg2 { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.Div;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Arg1?.Visit(visitor);
            Arg2?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("( {0} / {1} )",
                Arg1 != null ? Arg1.ExpressionString() : "NULL",
                Arg2 != null ? Arg2.ExpressionString() : "NULL");
        }
    }

    public class ExponentPrototype : EvalPrototype
    {
        public EvalPrototype BaseArg { get; protected set; }
        public EvalPrototype ExpArg { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.Exponent;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            BaseArg?.Visit(visitor);
            ExpArg?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("( {0}^{1} )",
                BaseArg != null ? BaseArg.ExpressionString() : "NULL",
                ExpArg != null ? ExpArg.ExpressionString() : "NULL");
        }
    }

    public class ScopePrototype : EvalPrototype
    {
        public EvalPrototype[] Scope { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.Scope;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            if (Scope.HasValue())
                foreach (var node in Scope)
                    node?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            if (Scope.IsNullOrEmpty())
                return "{ ..Empty.. }";

            StringBuilder sb = new();
            sb.Append('{');
            foreach (EvalPrototype scopeEval in Scope)
                sb.Append($" {scopeEval.ExpressionString()};");
            sb.Append(" }");
            return sb.ToString();
        }
    }

    public class GreaterThanPrototype : EvalPrototype
    {
        public EvalPrototype Arg1 { get; protected set; }
        public EvalPrototype Arg2 { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.GreaterThan;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Arg1?.Visit(visitor);
            Arg2?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("( {0} > {1} )",
                Arg1 != null ? Arg1.ExpressionString() : "NULL",
                Arg2 != null ? Arg2.ExpressionString() : "NULL");
        }
    }

    public class LessThanPrototype : EvalPrototype
    {
        public EvalPrototype Arg1 { get; protected set; }
        public EvalPrototype Arg2 { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LessThan;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Arg1?.Visit(visitor);
            Arg2?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("( {0} < {1} )",
                Arg1 != null ? Arg1.ExpressionString() : "NULL",
                Arg2 != null ? Arg2.ExpressionString() : "NULL");
        }
    }

    public class EqualsPrototype : EvalPrototype
    {
        public EvalPrototype Arg1 { get; protected set; }
        public EvalPrototype Arg2 { get; protected set; }
        public float Epsilon { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.Equals;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Arg1?.Visit(visitor);
            Arg2?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("( {0} == {1} )",
                Arg1 != null ? Arg1.ExpressionString() : "NULL",
                Arg2 != null ? Arg2.ExpressionString() : "NULL");
        }
    }

    public class AndPrototype : EvalPrototype
    {
        public EvalPrototype Arg1 { get; protected set; }
        public EvalPrototype Arg2 { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.And;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Arg1?.Visit(visitor);
            Arg2?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("( {0} && {1} )",
                Arg1 != null ? Arg1.ExpressionString() : "NULL",
                Arg2 != null ? Arg2.ExpressionString() : "NULL");
        }
    }

    public class OrPrototype : EvalPrototype
    {
        public EvalPrototype Arg1 { get; protected set; }
        public EvalPrototype Arg2 { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.Or;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Arg1?.Visit(visitor);
            Arg2?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("( {0} || {1} )",
                Arg1 != null ? Arg1.ExpressionString() : "NULL",
                Arg2 != null ? Arg2.ExpressionString() : "NULL");
        }
    }

    public class NotPrototype : EvalPrototype
    {
        public EvalPrototype Arg { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.Not;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Arg?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return $"!({(Arg != null ? Arg.ExpressionString() : "NULL")})";
        }
    }

    public class IsContextDataNullPrototype : EvalPrototype
    {
        public EvalContext Context { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.IsContextDataNull;
        }

        public override string ExpressionString()
        {
            return $"IsNULL({Context})";
        }
    }

    public class IfElsePrototype : EvalPrototype
    {
        public EvalPrototype Conditional { get; protected set; }
        public EvalPrototype EvalIf { get; protected set; }
        public EvalPrototype EvalElse { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.IfElse;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Conditional?.Visit(visitor);
            EvalIf?.Visit(visitor);
            EvalElse?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("( {0} ? {1} : {2} )",
                Conditional != null ? Conditional.ExpressionString() : "NULL",
                EvalIf != null ? EvalIf.ExpressionString() : "NULL",
                EvalElse != null ? EvalElse.ExpressionString() : "NULL");
        }
    }

    public class MaxPrototype : EvalPrototype
    {
        public EvalPrototype Arg1 { get; protected set; }
        public EvalPrototype Arg2 { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.Max;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Arg1?.Visit(visitor);
            Arg2?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("Max({0}, {1})",
                Arg1 != null ? Arg1.ExpressionString() : "NULL",
                Arg2 != null ? Arg2.ExpressionString() : "NULL");
        }
    }

    public class MinPrototype : EvalPrototype
    {
        public EvalPrototype Arg1 { get; protected set; }
        public EvalPrototype Arg2 { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.Min;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            Arg1?.Visit(visitor);
            Arg2?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return string.Format("Min({0}, {1})",
                Arg1 != null ? Arg1.ExpressionString() : "NULL",
                Arg2 != null ? Arg2.ExpressionString() : "NULL");
        }
    }

    public class RandomFloatPrototype : EvalPrototype
    {
        public float Max { get; protected set; }
        public float Min { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.RandomFloat;
        }

        public override string ExpressionString()
        {
            return $"RandFloat( {Min}:{Max} )";
        }
    }

    public class RandomIntPrototype : EvalPrototype
    {
        public int Max { get; protected set; }
        public int Min { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.RandomInt;
        }

        public override string ExpressionString()
        {
            return $"RandInt( {Min}:{Max} )";
        }
    }

    public class ForPrototype : EvalPrototype
    {
        public EvalPrototype LoopAdvance { get; protected set; }
        public EvalPrototype LoopCondition { get; protected set; }
        public EvalPrototype LoopVarInit { get; protected set; }
        public EvalPrototype PostLoop { get; protected set; }
        public EvalPrototype PreLoop { get; protected set; }
        public EvalPrototype[] ScopeLoopBody { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.For;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            LoopAdvance?.Visit(visitor);
            LoopCondition?.Visit(visitor);
            LoopVarInit?.Visit(visitor);
            PostLoop?.Visit(visitor);
            PreLoop?.Visit(visitor);
            if (ScopeLoopBody.HasValue())
                foreach (var node in ScopeLoopBody)
                    node?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            StringBuilder sb = new();

            if (ScopeLoopBody != null)
            {
                foreach (EvalPrototype evalProto in ScopeLoopBody)
                    sb.Append($" {evalProto.ExpressionString()};");
            }

            return string.Format("{{ Pre({0}) For ({1}; {2}; {3}) {{{4}}} Post({5}) }}",
                PreLoop?.ExpressionString(),
                LoopVarInit?.ExpressionString(),
                LoopCondition?.ExpressionString(),
                LoopAdvance?.ExpressionString(),
                sb.ToString(),
                PostLoop?.ToString());
        }
    }

    public class ForEachConditionInContextPrototype : EvalPrototype
    {
        public EvalPrototype PostLoop { get; protected set; }
        public EvalPrototype PreLoop { get; protected set; }
        public EvalPrototype[] ScopeLoopBody { get; protected set; }
        public EvalPrototype LoopConditionPreScope { get; protected set; }
        public EvalPrototype LoopConditionPostScope { get; protected set; }
        public EvalContext ConditionCollectionContext { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.ForEachConditionInContext;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            LoopConditionPreScope?.Visit(visitor);
            LoopConditionPostScope?.Visit(visitor);
            PostLoop?.Visit(visitor);
            PreLoop?.Visit(visitor);
            if (ScopeLoopBody.HasValue())
                foreach (var node in ScopeLoopBody)
                    node?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            StringBuilder sb = new();

            if (ScopeLoopBody != null)
            {
                foreach (EvalPrototype evalProto in ScopeLoopBody)
                    sb.Append($" {evalProto.ExpressionString()};");
            }

            return string.Format("{{ Pre({0}) Foreach (Condition in context [{1}], LoopCondition={{{2}}}) {{{3} if(LoopConditionPostScope{{{4}}}){{break}}}} Post({5}) }}",
                PreLoop?.ExpressionString(),
                ConditionCollectionContext,
                LoopConditionPreScope?.ExpressionString(),
                sb.ToString(),
                LoopConditionPostScope?.ExpressionString(),
                PostLoop?.ExpressionString());
        }
    }

    public class ForEachProtoRefInContextRefListPrototype : EvalPrototype
    {
        public EvalPrototype PostLoop { get; protected set; }
        public EvalPrototype PreLoop { get; protected set; }
        public EvalPrototype[] ScopeLoopBody { get; protected set; }
        public EvalPrototype LoopCondition { get; protected set; }
        public EvalContext ProtoRefListContext { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.ForEachProtoRefInContextRefList;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            LoopCondition?.Visit(visitor);
            PostLoop?.Visit(visitor);
            PreLoop?.Visit(visitor);
            if (ScopeLoopBody.HasValue())
                foreach (var node in ScopeLoopBody)
                    node?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            StringBuilder sb = new();

            if (ScopeLoopBody != null)
            {
                foreach (EvalPrototype evalProto in ScopeLoopBody)
                    sb.Append($" {evalProto.ExpressionString()};");
            }

            return string.Format("{{ Pre({0}) Foreach (ProtoRef in context [{1}], LoopCondition={{{2}}}) {{{3}}} Post({4}) }}",
                PreLoop?.ExpressionString(),
                ProtoRefListContext,
                LoopCondition?.ExpressionString(),
                sb.ToString(),
                PostLoop?.ExpressionString());
        }
    }

    public class LoadEntityToContextVarPrototype : EvalPrototype
    {
        public EvalContext Context { get; protected set; }
        public EvalPrototype EntityId { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadEntityToContextVar;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            EntityId?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return $"LoadEntityToContextVar(Context=[{Context}], EntityId={{{(EntityId != null ? EntityId.ExpressionString() : "!NULL Eval!")}}})";
        }
    }

    public class LoadConditionCollectionToContextPrototype : EvalPrototype
    {
        public EvalContext Context { get; protected set; }
        public EvalPrototype EntityId { get; protected set; }

        //---

        public override void PostProcess()
        {
            base.PostProcess();
            Op = EvalOp.LoadConditionCollectionToContext;
        }

        public override void Visit(EvalNodeVisitor visitor)
        {
            base.Visit(visitor);
            EntityId?.Visit(visitor);
        }

        public override string ExpressionString()
        {
            return $"LoadConditionCollectionToContext(Context=[{Context}], EntityId={{{(EntityId != null ? EntityId.ExpressionString() : "!NULL Eval!")}}})";
        }
    }
}
