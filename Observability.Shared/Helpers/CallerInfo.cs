using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Observability.Shared.Helpers
{
    public static class CallerInfo
    {
        public static (string Source, string Operation) GetCallerClassAndMethod(
            [CallerMemberName] string member = "",
            [CallerFilePath] string file = "")
        {
            var source = Path.GetFileNameWithoutExtension(file);
            return (source, member);
        }
    }
}
