using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Common.Utils.Helper
{
    public class MessageHelper<T>
    {
        public static string GenerateErrorMsg(PathString path, T? param, string? userName)
            => $"❌ Error ❌ [path: {path}] [param: {JsonSerializer.Serialize(param)}] [user : {userName}]";
    }
}
