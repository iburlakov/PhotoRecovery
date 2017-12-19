using System;

namespace PhotoRecovery.CLI
{
    public static class Extensions
    {
        public static T GetArgument<T>(this string[] @this, string name)
        {
            var index = Array.IndexOf(@this, name);
            if (index >= 0)
            {
                var valueIndex = index + 1;
                if (valueIndex < @this.Length)
                {
                    var valueStr = @this[valueIndex];

                    return (T)Convert.ChangeType(valueStr, typeof(T));
                }
                else
                {
                    throw new ArgumentException(string.Format("Could not get value for argument '{0}'", name));
                }
            }
            else
            {
                throw new ArgumentException(string.Format("Argument '{0}' is not passed", name));
            }
        }
    }
}
