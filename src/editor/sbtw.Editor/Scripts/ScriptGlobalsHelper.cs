// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace sbtw.Editor.Scripts
{
    public static class ScriptGlobalsHelper<T>
        where T : class
    {
        private static IReadOnlyDictionary<MethodInfo, Type> methods;
        private static IReadOnlyList<MemberInfo> members;
        private static IReadOnlyList<Type> types;

        public static IReadOnlyDictionary<string, object> GetValues(T source)
        {
            if (members == null)
            {
                members = typeof(T)
                    .GetMembers(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                    .Where(m => m is PropertyInfo || m is FieldInfo).ToList();
            }

            var values = new Dictionary<string, object>();

            foreach (var member in members)
            {
                if (member is PropertyInfo property)
                    values.Add(property.Name, property.GetValue(source));

                if (member is FieldInfo field)
                    values.Add(field.Name, field.GetValue(source));
            }

            return values;
        }

        public static IReadOnlyList<Delegate> GetMethods(T source)
        {
            if (methods == null)
            {
                var m = typeof(T)
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToList();

                var types = new Dictionary<MethodInfo, Type>();

                foreach (var method in m)
                {
                    var parameters = method.GetParameters().Select(p => p.ParameterType);
                    var type = method.ReturnType == typeof(void)
                        ? Expression.GetActionType(parameters.ToArray())
                        : Expression.GetFuncType(parameters.Append(method.ReturnType).ToArray());

                    types.Add(method, type);
                }

                methods = types;
            }

            var dels = new List<Delegate>();

            foreach ((var method, var type) in methods)
                dels.Add(method.CreateDelegate(type, source));

            return dels;
        }

        public static IReadOnlyList<Type> GetTypes(T source)
        {
            if (types != null)
                return types;

            var members = typeof(T).GetMember("TYPES", BindingFlags.Static | BindingFlags.Public);
            PropertyInfo prop = null;

            foreach (var m in members)
            {
                if (m is PropertyInfo p && p.CanRead && p.PropertyType.IsAssignableTo(typeof(IReadOnlyList<Type>)))
                {
                    prop = p;
                    break;
                }
            }

            if (prop == null)
                return Array.Empty<Type>();

            return types = prop.GetValue(source) as IReadOnlyList<Type>;
        }
    }
}
