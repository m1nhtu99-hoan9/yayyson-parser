using System;
using System.Diagnostics.CodeAnalysis;
using YayysonParser.Internal.Extensions;
using YayysonParser.Internal.Models;

namespace YayysonParser.Models
{
    public interface IUnaryFn<out TResult>
    {
        public TResult Invoke(object arg);

        public Type ParamType { get; }
    }

    public class UnaryFn<TResult> : BaseCustomFn, IUnaryFn<TResult>
    {
        public UnaryFn([NotNull] string fnName,
            [NotNull] Func<object, TResult> fn, 
            [NotNull] Type paramType)
            : base(fnName)
        {
            paramType.RaiseIfNotSupportedInYayyson(nameof(paramType));
            ParamType = paramType;

            _fn = fn ?? throw new ArgumentNullException(nameof(fn));
        }

        private readonly Func<object, TResult> _fn;

        public TResult Invoke(object arg)
        {
            if (arg?.GetType().IsInstanceOfType(ParamType) != true)
            {
                throw new InvalidOperationException($"'{Name}' is not called with an instance of [{ParamType}].");
            }

            return _fn.Invoke(arg);
        }

        public Type ParamType { get; private set; }
    }
}
