#if NET35 || PORTABLE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Threading
{
    internal static class ExecutionContextEx
    {
        public static void Dispose(this ExecutionContext context)
        {
        }
    }
}

#endif
