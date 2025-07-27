using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Common.Utils.Helper;

public class MessageHelper
{
    public static string GenerateErrorMsg(PathString path, dynamic? param, string? userName)
    {
        var serializedParam = param != null ? $"[param: {JsonSerializer.Serialize(param)}]" : "";
        return $"❌ Error ❌ [path: {path}] {serializedParam} [user: {userName}]";
    }

}
