using System;
using System.Diagnostics.CodeAnalysis;

namespace YayysonParser.Internal.Models
{
    public class BaseCustomFn 
    {
        protected string Name { get; }
        
        public BaseCustomFn([NotNull] string fnName)
        {
            if (string.IsNullOrWhiteSpace(fnName))
            {
                throw new ArgumentException(nameof(fnName));
            }

            Name = fnName;
        }
    }
}
