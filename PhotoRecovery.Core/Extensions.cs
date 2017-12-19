using System;
using System.Collections.Generic;

namespace PhotoRecovery.Core
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> forEachAction)
        {
            foreach (var item in @this)
            {
                forEachAction(item);
            }
        }
    }
}
