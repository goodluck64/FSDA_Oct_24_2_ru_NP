using System.Net;

namespace HTTPServer;

public static class HttpStatusCodeExtensions
{
    public static int ToInt32(this HttpStatusCode source) => (int)source;
}