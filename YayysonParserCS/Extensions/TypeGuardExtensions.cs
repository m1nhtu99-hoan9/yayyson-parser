using System;

namespace YayysonParser.Internal.Extensions
{
    public static class TypeGuardExtensions
    {
        public static void RaiseIfNotSupportedInYayyson(this Type type,
            string argName = null)
        {
            var argName_ = string.IsNullOrWhiteSpace(argName) ? nameof(type) : argName;

            if (null == type)
            {
                throw new ArgumentNullException(argName_);
            }

            if (!type.IsSupportedInYayyson())
            {
                throw new ArgumentException($"'{argName_}' refers to a type not supported in Yayyson: [{type.FullName}].");
            }
        }
    }
}
