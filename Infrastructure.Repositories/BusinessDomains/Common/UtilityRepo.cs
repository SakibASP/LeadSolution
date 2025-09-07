using Common.Extentions;
using Common.Utils.Constant;
using Core.Models.Common;
using Core.ViewModels.Request.Common;
using Dapper;
using Infrastructure.Interfaces.Common;
using Infrastructure.Repositories.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data;

namespace Infrastructure.Repositories.BusinessDomains.Common;


/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>

public sealed class UtilityRepo(LeadContext context,
    IDapperContext dapper, 
    IHttpContextAccessor httpContext) : IUtilityRepo, IAsyncDisposable
{
    private readonly LeadContext _context = context;
    private readonly IDapperContext _dapper = dapper;
    private readonly IHttpContextAccessor _httpContext = httpContext;

    private string CurrentUser => _httpContext.HttpContext?.User?.Identity?.Name ?? "N/A";
    private string RequestPath => _httpContext.HttpContext?.Request.Path.Value ?? "N/A";

    private static readonly Lock _lock = new();
    private readonly DateTime DaysAgo = DateTime.Now.ToBangladeshTime().AddDays(-15);

    private static DateTime _lastSystemLogCleanup = DateTime.MinValue;
    private static DateTime _lastApiLogCleanup = DateTime.MinValue;


    public async Task<IList<T>> GetDropdownListAsync<T>(DropdownRequest request)
    {
        var parameters = new DynamicParameters();
        parameters.Add("@Id", request.Id);
        parameters.Add("@Param1", request.Param1);
        parameters.Add("@Param2", request.Param2);
        parameters.Add("@Param3", request.Param3);
        parameters.Add("@Param4", request.Param4);

        using var connection = _dapper.CreateConnection();
        var result = await connection.QueryAsync<T>(
            Sp.usp_GetDropdownList,
            parameters,
            commandType: CommandType.StoredProcedure);

        return [.. result];
    }

    public async Task UpdateCountriesAsync(IList<RestCountry>? restCountries)
    {
        if (restCountries == null || restCountries.Count == 0) return; // No countries to update

        var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var c in restCountries)
            {
                // 🔹 Insert or Update Country
                Countries? dbCountry = await UpsertCountryAsync(c);

                // 🔹 Update Native Names
                if (dbCountry is not null)
                    await UpsertCountryNativeNameAsync(c, dbCountry);

                // 🔹 Update Currencies
                if (dbCountry is not null)
                    await UpsertCountryCurrencyAsync(c, dbCountry);
            }
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }

    }

    #region - logs -
    public async Task<IList<Logs>> GetSystemLogsAsync()
    {
        #region - Delete old logs in other thread if last cleanup was more than 1 hour ago -
        var now = DateTime.Now.ToBangladeshTime();
        if ((now - _lastSystemLogCleanup).TotalHours >= 1)
        {
            lock (_lock)
            {
                if ((now - _lastSystemLogCleanup).TotalHours >= 1)
                {
                    _lastSystemLogCleanup = now;

                    Thread th = new(() =>
                    {
                        try
                        {
                            using var db = new LeadContext(); // create a fresh DbContext

                            db.Logs
                              .Where(l => l.TimeStamp < DaysAgo)
                              .ExecuteDelete();
                        }
                        catch (Exception ex)
                        {
                            Log
                                .ForContext("UserName", CurrentUser)
                                .ForContext("Path", RequestPath)
                                .Error(ex, "Error bulk removing old system logs");
                        }
                    })
                    {
                        IsBackground = true
                    };
                    th.Start();
                }
            }
        }
        #endregion

        // Return the latest 1000 logs
        return await _context.Logs
            .AsNoTracking()
            .Where(x => x.TimeStamp > DaysAgo)
            .OrderByDescending(l => l.Id)
            .Take(1000)
            .ToListAsync();
    }

    public async Task<Logs> GetSystemLogByIdAsync(int id)
    {
        var result = await _context.Logs.FindAsync(id);
        return result is null ? throw new KeyNotFoundException($"Log with ID {id} not found.") : result;
    }

    public async Task<IList<RequestLogs>> GetApiLogsAsync()
    {
        #region - Delete old logs in other thread if last cleanup was more than 1 hour ago -
        var now = DateTime.Now.ToBangladeshTime();
        if ((now - _lastApiLogCleanup).TotalHours >= 1)
        {
            lock (_lock)
            {
                if ((now - _lastApiLogCleanup).TotalHours >= 1)
                {
                    _lastApiLogCleanup = now;

                    Thread th = new(() =>
                    {
                        try
                        {
                            using var db = new LeadContext(); // create a fresh DbContext

                            db.RequestLogs
                              .Where(l => l.CreatedDate < DaysAgo)
                              .ExecuteDelete();
                        }
                        catch (Exception ex)
                        {
                            Log
                                .ForContext("UserName", CurrentUser)
                                .ForContext("Path", RequestPath)
                                .Error(ex, "Error bulk removing old api logs");
                        }
                    })
                    {
                        IsBackground = true
                    };
                    th.Start();
                }
            }
        }
        #endregion

        // Return the latest 1000 logs
        return await _context.RequestLogs
                   .AsNoTracking()
                   .Where(x => x.CreatedDate > DaysAgo)
                   .OrderByDescending(l => l.Id)
                   .Take(500)
                   .ToListAsync();
    }

    public async Task<RequestLogs> GetApiLogByIdAsync(int id)
    {
        var result = await _context.RequestLogs.FindAsync(id);
        if (result is not null) result.UserId = _context.Users.Find(result.UserId)?.UserName ?? "Unauthorized";
        return result is null ? throw new KeyNotFoundException($"Log with ID {id} not found.") : result;
    }
    #endregion

    #region - private methods for country upsert -
    private async Task UpsertCountryCurrencyAsync(RestCountry c, Countries dbCountry)
    {
        if (c.Currencies != null)
        {
            foreach (var kv in c.Currencies)
            {
                var dbCurrency = await _context.Currencies
                    .FirstOrDefaultAsync(cur => cur.CountryId == dbCountry.Id && cur.CurrencyCode == kv.Key);

                if (dbCurrency == null)
                {
                    dbCurrency = new Currencies
                    {
                        CountryId = dbCountry.Id,
                        CurrencyCode = kv.Key,
                        CurrencyName = kv.Value.Name,
                        CurrencySymbol = kv.Value.Symbol
                    };
                    await _context.Currencies.AddAsync(dbCurrency);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    dbCurrency.CurrencyCode = kv.Key;
                    dbCurrency.CurrencyName = kv.Value.Name;
                    dbCurrency.CurrencySymbol = kv.Value.Symbol;
                    _context.Currencies.Update(dbCurrency);
                }

            }
        }
    }

    private async Task UpsertCountryNativeNameAsync(RestCountry c, Countries dbCountry)
    {
        if (c.Name?.NativeName != null)
        {
            foreach (var kv in c.Name.NativeName)
            {
                var dbNative = await _context.CountryNativeNames
                    .FirstOrDefaultAsync(n => n.CountryId == dbCountry.Id && n.LanguageCode == kv.Key);

                if (dbNative == null)
                {
                    dbNative = new CountryNativeNames
                    {
                        CountryId = dbCountry.Id,
                        LanguageCode = kv.Key,
                        Common = kv.Value.Common,
                        Official = kv.Value.Official
                    };
                    await _context.CountryNativeNames.AddAsync(dbNative);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    dbNative.LanguageCode = kv.Key;
                    dbNative.Common = kv.Value.Common;
                    dbNative.Official = kv.Value.Official;
                    _context.CountryNativeNames.Update(dbNative);
                }

            }
        }
    }

    private async Task<Countries?> UpsertCountryAsync(RestCountry c)
    {
        var dbCountry = await _context.Countries
                            .FirstOrDefaultAsync(x => x.Iso2Code == c.Cca2 || x.Iso3Code == c.Cca3);

        if (dbCountry == null)
        {
            dbCountry = new Countries()
            {
                CommonName = c.Name?.Common,
                OfficialName = c.Name?.Official,
                Iso2Code = c.Cca2,
                Iso3Code = c.Cca3,
                Region = c.Region,
                SubRegion = c.Subregion,
                FlagPng = c.Flags?.Png,
                FlagSvg = c.Flags?.Svg,
                FlagAlt = c.Flags?.Alt,
            };
            await _context.Countries.AddAsync(dbCountry);
            await _context.SaveChangesAsync();
        }
        else
        {
            dbCountry.CommonName = c.Name?.Common;
            dbCountry.OfficialName = c.Name?.Official;
            dbCountry.Iso2Code = c.Cca2;
            dbCountry.Iso3Code = c.Cca3;
            dbCountry.Region = c.Region;
            dbCountry.SubRegion = c.Subregion;
            dbCountry.FlagPng = c.Flags?.Png;
            dbCountry.FlagSvg = c.Flags?.Svg;
            dbCountry.FlagAlt = c.Flags?.Alt;
            _context.Countries.Update(dbCountry);
        }

        return dbCountry;
    }

    #endregion

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public async Task<int> GetDropdownIdByFormId(int formId)
    {
        var result = await _context.FormWiseDropdowns
            .AsNoTracking()
            .Where(f => f.FormId == formId)
            .Select(f => f.DropdownId)
            .FirstOrDefaultAsync();
        return result;
    }
}
