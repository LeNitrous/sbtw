// Copyright (c) 2021 Nathan Alo. Licensed under MIT License.
// See LICENSE in the repository root for more details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace sbtw.Editor.Scripts
{
    /// <summary>
    /// Represents scripts of other languages that require external files for execution.
    /// </summary>
    public abstract class FileBasedScript : Script
    {
        /// <summary>
        /// The full path for this script.
        /// </summary>
        public readonly string Path;

        public FileBasedScript(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Compiles this script.
        /// </summary>
        public void Compile() => CompileAsync().Wait();

        /// <summary>
        /// Compiles this script asynchronously.
        /// </summary>
        public abstract Task CompileAsync();

        /// <summary>
        /// Exposes a delegate to an external runtime.
        /// </summary>
        /// <param name="del">The delegate to expose.</param>
        public abstract void RegisterDelegate(Delegate del);

        /// <summary>
        /// Exposes a member to an external runtime.
        /// </summary>
        /// <param name="member">The member name to expose.</param>
        /// <param name="value">The value to expose.</param>
        public abstract void RegisterMember(string name, object value);

        /// <summary>
        /// Exposes a type to an external runtime.
        /// </summary>
        /// <param name="type">The type to expose.</param>
        public abstract void RegisterType(Type type);

        static FileBasedScript()
        {
            var typeMap = new List<(Type, MethodInfo)>();
            var methods = typeof(Script)
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<ScriptVisibleAttribute>() != null);

            foreach (var method in methods)
            {
                var parameters = method.GetParameters().Select(p => p.ParameterType);
                var type = method.ReturnType == typeof(void)
                    ? Expression.GetActionType(parameters.ToArray())
                    : Expression.GetFuncType(parameters.Append(method.ReturnType).ToArray());

                typeMap.Add((type, method));
            }

            METHODS = typeMap;
        }

        public static readonly IReadOnlyList<(Type, MethodInfo)> METHODS;
        public static readonly IReadOnlyList<Type> TYPES = Type.EmptyTypes;
    }
}
