using System;
using System.Linq;

namespace YayysonParser.Internal.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsSupportedInYayyson(this Type type)
        {
            return new[]
            {
                typeof(Guid),
                typeof(DateTime),
                typeof(TimeSpan),
                typeof(int),
                typeof(float),
                typeof(string),
                typeof(bool)
            }
            .Contains(type);
        }
    }
}
