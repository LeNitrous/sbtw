// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace sbtw.Editor.Scripts
{
    public abstract class BuiltInScript : DynamicObject, IScript, INamedScript
    {
        public string Name => GetType().Name.Replace("Script", string.Empty);

        private readonly List<IProperty> properties = new List<IProperty>();

        protected abstract void Perform(dynamic context);

        public Task ExecuteAsync(CancellationToken token = default)
        {
            Perform(this);
            return Task.CompletedTask;
        }

        public void Reset() => properties.Clear();

        public sealed override IEnumerable<string> GetDynamicMemberNames()
            => properties.Select(p => p.Name);

        public void RegisterFunction(Delegate del)
            => properties.Add(new FunctionProperty { Name = del.Method.Name, Delegate = del });

        public void RegisterVariable(string name, object value)
            => properties.Add(new ValueProperty { Name = name, Value = value });

        public void RegisterType(Type type)
        {
        }

        public sealed override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var prop = properties.FirstOrDefault(p => p.Name == binder.Name);

            if (prop is ValueProperty value)
            {
                result = value.Value;
                return true;
            }

            if (prop is FunctionProperty function)
            {
                result = function.Delegate;
                return true;
            }

            return base.TryGetMember(binder, out result);
        }

        public sealed override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var prop = properties.FirstOrDefault(p =>
                p is FunctionProperty func
                    && func.Name == binder.Name
                    && func.ParameterTypes.Count() == args.Length
                    && func.ParameterTypes.SequenceEqual(args.Select(a => a.GetType()))
            );

            if (prop is FunctionProperty func)
            {
                result = func.Invoke(args);
                return true;
            }

            return base.TryInvokeMember(binder, args, out result);
        }

        public sealed override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var prop = properties.FirstOrDefault(p => p.Name == binder.Name);

            if (prop != null)
                properties.Remove(prop);

            if (value is Delegate del)
            {
                properties.Add(new FunctionProperty { Name = binder.Name, Delegate = del });
                return true;
            }

            properties.Add(new ValueProperty { Name = binder.Name, Value = value });
            return true;
        }

        public sealed override DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return base.GetMetaObject(parameter);
        }

        public sealed override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            return base.TryBinaryOperation(binder, arg, out result);
        }

        public sealed override bool TryConvert(ConvertBinder binder, out object result)
        {
            return base.TryConvert(binder, out result);
        }

        public sealed override bool TryCreateInstance(CreateInstanceBinder binder, object[] args, [NotNullWhen(true)] out object result)
        {
            return base.TryCreateInstance(binder, args, out result);
        }

        public sealed override bool TryDeleteIndex(DeleteIndexBinder binder, object[] indexes)
        {
            return base.TryDeleteIndex(binder, indexes);
        }

        public sealed override bool TryDeleteMember(DeleteMemberBinder binder)
        {
            return base.TryDeleteMember(binder);
        }

        public sealed override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return base.TryGetIndex(binder, indexes, out result);
        }

        public sealed override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            return base.TryInvoke(binder, args, out result);
        }

        public sealed override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return base.TrySetIndex(binder, indexes, value);
        }

        public sealed override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            return base.TryUnaryOperation(binder, out result);
        }

        private interface IProperty : IEquatable<IProperty>
        {
            string Name { get; }
        }

        private class FunctionProperty : IProperty
        {
            public string Name { get; set; }
            public Delegate Delegate { get; set; }
            public Type ReturnType => Delegate.Method.ReturnType;
            public IEnumerable<Type> ParameterTypes => Delegate.Method.GetParameters().Select(p => p.ParameterType);

            public object Invoke(params object[] args)
                => Delegate.Method.Invoke(Delegate.Target, args);

            public bool Equals(IProperty other)
                => other is FunctionProperty otherFunction
                    && otherFunction.Name.Equals(Name)
                    && otherFunction.ReturnType.Equals(ReturnType)
                    && otherFunction.ParameterTypes.SequenceEqual(ParameterTypes);
        }

        private class ValueProperty : IProperty
        {
            public string Name { get; set; }
            public object Value { get; set; }
            public Type Type => Value.GetType();

            public bool Equals(IProperty other)
                => other is ValueProperty otherValue
                    && otherValue.Name.Equals(Name)
                    && otherValue.Value.Equals(Value);
        }
    }
}
