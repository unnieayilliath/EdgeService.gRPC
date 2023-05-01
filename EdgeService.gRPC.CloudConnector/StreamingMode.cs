using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EdgeService.gRPC.CloudConnector
{
    internal class StreamingMode
    {
        public static string Unary = "unary";
        public static string ClientStreaming = "client";
        public static string BiDirectional = "bidirectional";
    }
}
