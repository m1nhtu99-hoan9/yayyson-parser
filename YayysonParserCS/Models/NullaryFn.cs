using System;
using System.Diagnostics.CodeAnalysis;
using YayysonParser.Internal.Models;

namespace YayysonParser.Models
{
    public interface INullaryFn<out TResult>
    {
        public TResult Invoke();
    }

    public class NullaryFn<TResult> : BaseCustomFn, INullaryFn<TResult>
    {
        private readonly Func<TResult> _fn;

        public NullaryFn([NotNull] string fnName, 
            [NotNull] Func<TResult> fn) 
            : base(fnName)
        {
            _fn = fn ?? throw new ArgumentNullException(nameof(fn));
        }

        public TResult Invoke() => _fn.Invoke();
    }
}
