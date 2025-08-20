using Core.Models.Common;
using Infrastructure.Interfaces.Common;
using Infrastructure.Repositories.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.BusinessDomains.Common;


/// <summary>
/// Author: Md. Sakibur Rahman
/// </summary>

public sealed class CountryRepo(LeadContext context) : ICountryRepo, IAsyncDisposable
{
    private readonly LeadContext _context = context;

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

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        GC.SuppressFinalize(this);
    }

}
