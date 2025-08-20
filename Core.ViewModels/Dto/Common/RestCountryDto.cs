public class RestCountry
{
    public Name Name { get; set; }
    public string Cca2 { get; set; }
    public string Cca3 { get; set; }
    public string Region { get; set; }
    public string Subregion { get; set; }
    public Flags Flags { get; set; }
    public Dictionary<string, CurrencyInfo> Currencies { get; set; }
}

public class Name
{
    public string Common { get; set; }
    public string Official { get; set; }
    public Dictionary<string, NativeName> NativeName { get; set; }
}

public class NativeName
{
    public string Official { get; set; }
    public string Common { get; set; }
}

public class Flags
{
    public string Png { get; set; }
    public string Svg { get; set; }
    public string Alt { get; set; }
}

public class CurrencyInfo
{
    public string Name { get; set; }
    public string Symbol { get; set; }
}
