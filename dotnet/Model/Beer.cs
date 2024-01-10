namespace sk_webapi;

public class Beer
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string NameDisplay { get; set; }
    public string Description { get; set; }
    public string Abv { get; set; }
    public string Ibu { get; set; }
    public int GlasswareId { get; set; }
    public int AvailableId { get; set; }
    public int StyleId { get; set; }
    public string IsOrganic { get; set; }
    public string IsRetired { get; set; }
    public Labels Labels { get; set; }
    public string Status { get; set; }
    public string StatusDisplay { get; set; }
    public string CreateDate { get; set; }
    public string UpdateDate { get; set; }
    public Glass Glass { get; set; }
    public Available Available { get; set; }
    public Style Style { get; set; }
    public List<Brewery> Breweries { get; set; }
}

public class Labels
{
    public string Icon { get; set; }
    public string Medium { get; set; }
    public string Large { get; set; }
    public string ContentAwareIcon { get; set; }
    public string ContentAwareMedium { get; set; }
    public string ContentAwareLarge { get; set; }
}

public class Glass
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CreateDate { get; set; }
}

public class Available
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class Style
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Description { get; set; }
    public string IbuMin { get; set; }
    public string IbuMax { get; set; }
    public string AbvMin { get; set; }
    public string AbvMax { get; set; }
    public string SrmMin { get; set; }
    public string SrmMax { get; set; }
    public string OgMin { get; set; }
    public string FgMin { get; set; }
    public string FgMax { get; set; }
    public string CreateDate { get; set; }
    public string UpdateDate { get; set; }
}

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string CreateDate { get; set; }
}

public class Brewery
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string NameShortDisplay { get; set; }
    public string Description { get; set; }
    public string Website { get; set; }
    public string Established { get; set; }
    public string IsOrganic { get; set; }
    public Images Images { get; set; }
    public string Status { get; set; }
    public string StatusDisplay { get; set; }
    public string CreateDate { get; set; }
    public string UpdateDate { get; set; }
    public string IsMassOwned { get; set; }
    public string IsInBusiness { get; set; }
    public BrewersAssociation BrewersAssociation { get; set; }
    public string IsVerified { get; set; }
    public List<Location> Locations { get; set; }
}

public class Images
{
    public string Icon { get; set; }
    public string Medium { get; set; }
    public string Large { get; set; }
    public string SquareMedium { get; set; }
    public string SquareLarge { get; set; }
}

public class BrewersAssociation
{
    public string BrewersAssocationId { get; set; }
    public string IsCertifiedCraftBrewer { get; set; }
}

public class Location
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string StreetAddress { get; set; }
    public string Locality { get; set; }
    public string Region { get; set; }
    public string PostalCode { get; set; }
    public string Phone { get; set; }
    public string Website { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string IsPrimary { get; set; }
    public string InPlanning { get; set; }
    public string IsClosed { get; set; }
    public string OpenToPublic { get; set; }
    public string LocationType { get; set; }
    public string LocationTypeDisplay { get; set; }
    public string CountryIsoCode { get; set; }
    public string YearOpened { get; set; }
    public string Status { get; set; }
    public string StatusDisplay { get; set; }
    public string CreateDate { get; set; }
    public string UpdateDate { get; set; }
    public HoursOfOperationExplicit HoursOfOperationExplicit { get; set; }
    public string HoursOfOperationExplicitString { get; set; }
    public Country Country { get; set; }
}

public class HoursOfOperationExplicit
{
    public List<HoursOfOperationExplicitItem> Thu { get; set; }
    public List<HoursOfOperationExplicitItem> Fri { get; set; }
    public List<HoursOfOperationExplicitItem> Sat { get; set; }
    public List<HoursOfOperationExplicitItem> Sun { get; set; }
    public List<HoursOfOperationExplicitItem> Mon { get; set; }
    public List<HoursOfOperationExplicitItem> Tue { get; set; }
    public List<HoursOfOperationExplicitItem> Wed { get; set; }
}

public class HoursOfOperationExplicitItem
{
    public string StartTime { get; set; }
    public string EndTime { get; set; }
}

public class Country
{
    public string IsoCode { get; set; }
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public string IsoThree { get; set; }
    public int NumberCode { get; set; }
    public string CreateDate { get; set; }
}
