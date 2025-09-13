using Microsoft.AspNetCore.Http;

namespace Common.Utils.Helper;

public static class IpAddressHelper
{
    public static string GetClientIpAddress(HttpContext? context)
    {
        if (context == null)
            return string.Empty;

        // 1️⃣ Check X-Forwarded-For header (proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            // X-Forwarded-For may contain multiple IPs, first one is client
            var ip = forwardedFor.Split(',').FirstOrDefault();
            if (!string.IsNullOrEmpty(ip))
                return ip.Trim();
        }

        // 2️⃣ Fall back to RemoteIpAddress
        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp != null)
        {
            // Handle IPv6 mapped IPv4 (e.g., "::ffff:192.168.1.10")
            return remoteIp.MapToIPv4().ToString();
        }

        return string.Empty;
    }
}
