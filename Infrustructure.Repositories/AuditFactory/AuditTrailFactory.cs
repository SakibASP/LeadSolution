using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Core.Models.Common;
using Common.Utils.Helper;

namespace Infrustructure.Repositories.AuditFactory
{
    /// <summary>
    /// Factory class for creating Audit objects based on entity changes.
    /// </summary>
    public class AuditTrailFactory(IHttpContextAccessor httpContext)
    {
        // HTTP context accessor for retrieving user and request information
        private readonly IHttpContextAccessor _httpContext = httpContext;

        /// <summary>
        /// Generates an Audit object for the given entity entry if it should be audited.
        /// </summary>
        /// <param name="entry">The EntityEntry representing the entity change.</param>
        /// <returns>An Audit object with relevant audit information, or null if not applicable.</returns>
        public async Task<Audit?> GetAudit(EntityEntry entry)
        {
            Audit? audit = new();
            try
            {
                string tableName = entry.Entity.GetType().Name;
                // Determine if the entity should be audited based on its table name
                bool shouldAudit = !tableName.Contains("AspNet", StringComparison.CurrentCultureIgnoreCase) && !tableName.Contains("RequestCount", StringComparison.CurrentCultureIgnoreCase)
                    && !tableName.Contains("ApplicationUser", StringComparison.CurrentCultureIgnoreCase) && !tableName.Contains("IdentityUser", StringComparison.CurrentCultureIgnoreCase)
                    && !string.IsNullOrEmpty(tableName);

                if (shouldAudit)
                {
                    /*
                    // Example code for retrieving IP address and MAC address (not used in current implementation)
                    string ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if ((String.IsNullOrEmpty(ipAddress)))
                        ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    if ((String.IsNullOrEmpty(ipAddress)))
                        ipAddress = HttpContext.Current.Request.UserHostAddress;

                    // Getting Physical Mac Address of hosted device
                    HashSet<string> macSet = [];
                    NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                    foreach (NetworkInterface networkInterface in networkInterfaces)
                    {
                        PhysicalAddress macAddress = networkInterface.GetPhysicalAddress();
                        byte[] bytes = macAddress.GetAddressBytes();
                        string macAddressString = BitConverter.ToString(bytes);
                        macAddressString = macAddressString.Replace("-", ":");
                        macSet.Add(macAddressString);
                    }
                    string physicalMac = string.Join(" || ", macSet);
                    */

                    if (_httpContext.HttpContext is not null)
                    {
                        // User's IP address
                        string? ipAddress = _httpContext.HttpContext.Connection.RemoteIpAddress?.ToString();
                        // User's ID from claims, fallback to default if not found
                        audit.UserId = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "TakeSakibsIdAsNullUser";
                        // Model class name based on database table
                        audit.TableName = tableName;
                        // Bangladesh standard current time
                        audit.UpdateDate = TimeHelper.GetCurrentBangladeshTime();
                        // User's operating system from User-Agent header
                        audit.OperatingSystem = GetOperatingSystem(_httpContext.HttpContext!.Request.Headers.UserAgent.ToString());
                        // Truncate IP address to first 256 characters
                        audit.IPAddress = ipAddress?[..Math.Min(ipAddress.Length, 256)];
                        // URL path from which the request originated
                        audit.AreaAccessed = _httpContext.HttpContext.Request.Path;
                        // Get primary key value of the table
                        PropertyValues? dbValus = await entry.GetDatabaseValuesAsync();
                        audit.KeyValue = GetKeyValue(dbValus);

                        // If entity is deleted, populate old data and set action to Delete
                        if (entry.State == EntityState.Deleted)
                        {
                            var oldValues = new StringBuilder();
                            SetDeletedProperties(dbValus, oldValues);
                            audit.OldData = oldValues.ToString();
                            audit.Actions = AuditActions.Delete.ToString();
                        }
                        // If entity is modified, populate old and new data and set action to Update
                        else if (entry.State == EntityState.Modified)
                        {
                            var oldValues = new StringBuilder();
                            var newValues = new StringBuilder();
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
                // Exception handling can be implemented here (e.g., logging)
            }
            return audit;
        }

        /// <summary>
        /// Populates oldData with property values for a deleted entity.
        /// </summary>
        /// <param name="dbValues">Database property values.</param>
        /// <param name="oldData">StringBuilder to append old data.</param>
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

            // Remove trailing separator if present
            if (oldData.Length > 0)
                _ = oldData.Remove(oldData.Length - 3, 3);
        }

        /// <summary>
        /// Populates oldData and newData with property values for a modified entity.
        /// </summary>
        /// <param name="entry">EntityEntry representing the entity.</param>
        /// <param name="dbValues">Database property values.</param>
        /// <param name="oldData">StringBuilder to append old data.</param>
        /// <param name="newData">StringBuilder to append new data.</param>
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
                        // Compare old and new values, append if changed
                        bool unchnaged = oldVal.ToString()!.Equals(newVal.ToString(), StringComparison.CurrentCultureIgnoreCase);
                        if (!unchnaged)
                        {
                            newData.AppendFormat("{0}={1} || ", propertyName.Name, newVal);
                            oldData.AppendFormat("{0}={1} || ", propertyName.Name, oldVal);
                        }
                    }
                }
            }

            // Remove trailing separator if present
            if (oldData.Length > 0)
                _ = oldData.Remove(oldData.Length - 3, 3);
            if (newData.Length > 0)
                _ = newData.Remove(newData.Length - 3, 3);
        }

        /// <summary>
        /// Retrieves the primary key value from the property values.
        /// </summary>
        /// <param name="dbValues">Database property values.</param>
        /// <returns>Primary key value as long, or 0 if not found.</returns>
        public static long? GetKeyValue(PropertyValues? dbValues)
        {
            string _key = dbValues?.EntityType.FindPrimaryKey()?.Properties.FirstOrDefault()?.Name ?? "AutoId";
            object? keyValue = dbValues?[_key];
            long id = 0;
            if (keyValue != null)
            {
                id = Convert.ToInt64(keyValue);
            }
            // Alternative approach for retrieving key value from ObjectStateEntry (commented out)
            // var objectStateEntry = ((IObjectContextAdapter)_context).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
            // if (objectStateEntry.EntityKey.EntityKeyValues != null)
            //     id = Convert.ToInt64(objectStateEntry.EntityKey.EntityKeyValues[0].Value);

            return id;
        }

        /// <summary>
        /// Extracts the operating system name from the user agent string.
        /// </summary>
        /// <param name="userAgent">User agent string from request headers.</param>
        /// <returns>Operating system name as string.</returns>
        private static string GetOperatingSystem(string userAgent)
        {
            // Basic logic to extract operating system from user agent string
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

    /// <summary>
    /// Enum representing possible audit actions.
    /// </summary>
    public enum AuditActions
    {
        Update,
        Delete
    }
}
