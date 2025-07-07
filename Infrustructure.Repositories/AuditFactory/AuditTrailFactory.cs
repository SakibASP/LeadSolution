using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Core.Models.Common;

namespace Infrustructure.Repositories.AuditFactory
{
    public class AuditTrailFactory(IHttpContextAccessor httpContext)
    {
        private static readonly TimeZoneInfo bdTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
        //private readonly DbContext _context = context;
        private readonly IHttpContextAccessor _httpContext = httpContext; //new HttpContextAccessor();

        public async Task<Audit?> GetAudit(EntityEntry entry)
        {
            Audit? audit = new();
            try
            {
                string tableName = entry.Entity.GetType().Name;
                bool shouldAudit = !tableName.Contains("AspNet", StringComparison.CurrentCultureIgnoreCase) && !tableName.Contains("RequestCount", StringComparison.CurrentCultureIgnoreCase)
                    && !tableName.Contains("ApplicationUser", StringComparison.CurrentCultureIgnoreCase) && !tableName.Contains("IdentityUser", StringComparison.CurrentCultureIgnoreCase)
                    && !string.IsNullOrEmpty(tableName);

                if (shouldAudit)
                {
                    /***
                    string ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if ((String.IsNullOrEmpty(ipAddress)))
                        ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    if ((String.IsNullOrEmpty(ipAddress)))
                        ipAddress = HttpContext.Current.Request.UserHostAddress;

                    Getting Physical Mac Address of hosted device
                    HashSet<string> macSet = [];
                    NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                    foreach (NetworkInterface networkInterface in networkInterfaces)
                    {
                        PhysicalAddress macAddress = networkInterface.GetPhysicalAddress();
                        //physicalMac = physicalMac + "__" + macAddress.ToString();
                        byte[] bytes = macAddress.GetAddressBytes();
                        string macAddressString = BitConverter.ToString(bytes);
                        macAddressString = macAddressString.Replace("-", ":");
                        macSet.Add(macAddressString);
                    }
                    string physicalMac = string.Join(" || ", macSet);
                    ***/

                    if (_httpContext.HttpContext is not null)
                    {
                        //User's ip address
                        string? ipAddress = _httpContext.HttpContext.Connection.RemoteIpAddress?.ToString();
                        //User's id
                        audit.UserId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "TakeSakibsIdAsNullUser";
                        //model class name based on database table 
                        audit.TableName = tableName;
                        //Bangladesh standard current time
                        audit.UpdateDate = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, bdTimeZone);
                        //User's operating system
                        audit.OperatingSystem = GetOperatingSystem(_httpContext.HttpContext!.Request.Headers.UserAgent.ToString()); ;
                        //re-organizing User's ip address [taking first 256 chars]
                        audit.IPAddress = ipAddress?[..Math.Min(ipAddress.Length, 256)];
                        //url path [from which menu the request came from]
                        audit.AreaAccessed = _httpContext.HttpContext.Request.Path;
                        //getting primary key value of the table
                        PropertyValues? dbValus = await entry.GetDatabaseValuesAsync();
                        audit.KeyValue = GetKeyValue(dbValus);

                        //entry in deleted
                        if (entry.State == EntityState.Deleted)
                        {
                            var oldValues = new StringBuilder();
                            //populating deleted properties values with column name
                            SetDeletedProperties(dbValus, oldValues);
                            audit.OldData = oldValues.ToString();
                            audit.Actions = AuditActions.Delete.ToString();
                        }
                        //entry is modified
                        else if (entry.State == EntityState.Modified)
                        {
                            var oldValues = new StringBuilder();
                            var newValues = new StringBuilder();
                            //populating modified properties values with column name [previous and current data]
                            SetModifiedProperties(entry, dbValus, oldValues, newValues);
                            audit.OldData = oldValues.ToString();
                            audit.NewData = newValues.ToString();
                            audit.Actions = AuditActions.Update.ToString();
                        }
                    }
                }
            }
            catch
            {
                //Write codes for exception here
            }
            return audit;
        }

        private static void SetDeletedProperties(PropertyValues? dbValues, StringBuilder oldData)
        {
            if (dbValues is not null)
            {
                foreach (var propertyName in dbValues.Properties)
                {
                    var oldVal = dbValues[propertyName.Name];
                    if (oldVal is not null)
                    {
                        oldData.AppendFormat("{0}={1} || ", propertyName.Name, oldVal);
                    }
                }
            }

            if (oldData.Length > 0)
                _ = oldData.Remove(oldData.Length - 3, 3);
        }

        private static void SetModifiedProperties(EntityEntry entry, PropertyValues? dbValues, StringBuilder oldData, StringBuilder newData)
        {
            if (dbValues is not null)
            {
                foreach (var propertyName in dbValues.Properties)
                {
                    var oldVal = dbValues[propertyName.Name];
                    var newVal = entry.CurrentValues[propertyName.Name];
                    if (oldVal is not null && newVal is not null)
                    {
                        bool unchnaged = oldVal.ToString()!.Equals(newVal.ToString(), StringComparison.CurrentCultureIgnoreCase);
                        if (!unchnaged)
                        {
                            newData.AppendFormat("{0}={1} || ", propertyName.Name, newVal);
                            oldData.AppendFormat("{0}={1} || ", propertyName.Name, oldVal);
                        }
                    }
                }
            }

            if (oldData.Length > 0)
                _ = oldData.Remove(oldData.Length - 3, 3);
            if (newData.Length > 0)
                _ = newData.Remove(newData.Length - 3, 3);
        }

        public static long? GetKeyValue(PropertyValues? dbValues)
        {
            string _key = dbValues?.EntityType.FindPrimaryKey()?.Properties.FirstOrDefault()?.Name ?? "AutoId";
            object? keyValue = dbValues?[_key];
            long id = 0;
            if (keyValue != null)
            {
                id = Convert.ToInt64(keyValue);
            }
            //var objectStateEntry = ((IObjectContextAdapter)_context).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            //if (objectStateEntry.EntityKey.EntityKeyValues != null)
            //    id = Convert.ToInt64(objectStateEntry.EntityKey.EntityKeyValues[0].Value);

            return id;
        }

        private static string GetOperatingSystem(string userAgent)
        {
            // Logic to extract operating system from user agent string
            // You can use a library like UserAgentUtils or implement custom logic
            // For simplicity, let's assume a basic implementation
            if (userAgent.Contains("Android", StringComparison.CurrentCultureIgnoreCase))
            {
                // If "Android" is found in the User-Agent string, it's likely an Android device
                return "Android";
            }
            else if (userAgent.Contains("Windows", StringComparison.CurrentCultureIgnoreCase))
            {
                return "Windows";
            }
            else if (userAgent.Contains("Mac OS", StringComparison.CurrentCultureIgnoreCase))
            {
                return "macOS";
            }
            else if (userAgent.Contains("Linux", StringComparison.CurrentCultureIgnoreCase))
            {
                return "Linux";
            }
            else if (userAgent.Contains("Google-Safety", StringComparison.CurrentCultureIgnoreCase) || userAgent.Equals("Google", StringComparison.CurrentCultureIgnoreCase))
            {
                return "Google";
            }
            else if (userAgent.Contains("facebookexternalhit", StringComparison.CurrentCultureIgnoreCase))
            {
                return "Facebook";
            }
            else
            {
                return "Unknown";
            }
        }
    }

    public enum AuditActions
    {
        Update,
        Delete
    }
}
