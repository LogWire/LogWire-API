using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace LogWire.API.Utils
{
    public static class HeaderUtils
    {

        public static bool HasHeader(this HttpRequest request, string header)
        {
            return request.Headers.ContainsKey(header);
        }

    }
}
