using System;
using System.Diagnostics.CodeAnalysis;
using YayysonParser.Internal.Extensions;
using YayysonParser.Internal.Models;

namespace YayysonParser.Models
{
    public interface IBinaryFn<out TResult>
    {
        public TResult Invoke(object arg1, object arg2);

        public Type Param1Type { get; }

        public Type Param2Type { get; }
    }

    public class BinaryFn<TResult> : BaseCustomFn, IBinaryFn<TResult>
    {
        public BinaryFn([NotNull] string fnName,
            [NotNull] Func<object, object, TResult> fn,
            [NotNull] Type param1Type,
            [NotNull] Type param2Type)
            : base(fnName)
        {
            param1Type.RaiseIfNotSupportedInYayyson(nameof(param1Type));
            param2Type.RaiseIfNotSupportedInYayyson(nameof(param2Type));

            Param1Type = param1Type;
            Param2Type = param2Type;

            _fn = fn ?? throw new ArgumentNullException(nameof(fn));
        }

        private readonly Func<object, object, TResult> _fn;

        public TResult Invoke(object arg1, object arg2)
        {
            if (arg1?.GetType().IsInstanceOfType(Param1Type) != true)
            {
                throw new InvalidOperationException($"'{Name}'`s first argument is not an instance of [{Param1Type}].");
            }

            if (arg2?.GetType().IsInstanceOfType(Param2Type) != true)
            {
                throw new InvalidOperationException($"'{Name}'`s second argument is not an instance of [{Param2Type}].");
            }

            return _fn.Invoke(arg1, arg2);
        }

        public Type Param1Type { get; private set; }

        public Type Param2Type { get; private set; }
    }
}
