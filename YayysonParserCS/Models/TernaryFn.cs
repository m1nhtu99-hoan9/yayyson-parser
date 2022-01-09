using System;
using System.Diagnostics.CodeAnalysis;
using YayysonParser.Internal.Extensions;
using YayysonParser.Internal.Models;

namespace YayysonParser.Models
{
    public interface ITernaryFn<out TResult>
    {
        public TResult Invoke(object arg1, object arg2, object arg3);

        public Type Param1Type { get; }

        public Type Param2Type { get; }

        public Type Param3Type { get; }
    }

    public class TernaryFn<TResult> : BaseCustomFn, ITernaryFn<TResult>
    {
        public TernaryFn([NotNull] string fnName,
            [NotNull] Func<object, object, object, TResult> fn,
            [NotNull] Type param1Type,
            [NotNull] Type param2Type,
            [NotNull] Type param3Type)
            : base(fnName)
        {
            param1Type.RaiseIfNotSupportedInYayyson(nameof(param1Type));
            param2Type.RaiseIfNotSupportedInYayyson(nameof(param2Type));
            param3Type.RaiseIfNotSupportedInYayyson(nameof(param3Type));

            Param1Type = param1Type;
            Param2Type = param2Type;
            Param3Type = param3Type;

            _fn = fn ?? throw new ArgumentNullException(nameof(fn));
        }

        private readonly Func<object, object, object, TResult> _fn;

        public TResult Invoke(object arg1, object arg2, object arg3)
        {
            if (arg1?.GetType().IsInstanceOfType(Param1Type) != true)
            {
                throw new InvalidOperationException($"'{Name}'`s first argument is not an instance of [{Param1Type}].");
            }

            if (arg2?.GetType().IsInstanceOfType(Param2Type) != true)
            {
                throw new InvalidOperationException($"'{Name}'`s second argument is not an instance of [{Param2Type}].");
            }

            if (arg3?.GetType().IsInstanceOfType(Param3Type) != true)
            {
                throw new InvalidOperationException($"'{Name}'`s third argument is not an instance of [{Param3Type}].");
            }

            return _fn.Invoke(arg1, arg2, arg3);
        }

        public Type Param1Type { get; private set; }

        public Type Param2Type { get; private set; }

        public Type Param3Type { get; private set; }
    }
}
