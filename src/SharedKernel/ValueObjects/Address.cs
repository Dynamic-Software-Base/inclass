namespace SharedKernel.ValueObjects;

public record Address
{
    private const int MinimumBuildingNumberValue = 1;
    public string StreetAddress { get; init; }
    public string? BuildingNumber { get; init; }
    public string? Apartment { get; init; }
    public string City { get; init; }
    public string Province { get; init; }
    public string Region { get; init; }
    public string PostalCode { get; init; }

    public Coordinates? Coordinates { get; init; }

    private Address (){}

    private Address(
        string streetAddress,
        string? buildingNumber,
        string? apartment,
        string city,
        string province,
        string region,
        string postalCode,
        Coordinates? coordinates
    )
    {
        StreetAddress = streetAddress;
        BuildingNumber = buildingNumber;
        Apartment = apartment;
        City = city;
        Province = province;
        Region = region;
        PostalCode = postalCode;
        Coordinates = coordinates;
    }

    public static ErrorOr<Address> Create(
        string streetAddress,
        string? buildingNumber,
        string? apartment, // should be a number ?
        string city,
        string province,
        string region,
        string postalCode,
        Coordinates? coordinates
        )
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(streetAddress))
        {
            errors.Add(DomainErrors.Required(nameof(streetAddress)));
        }
#pragma warning disable CA1305
        if (!string.IsNullOrWhiteSpace(buildingNumber ) && buildingNumber.All(Char.IsDigit) && int.Parse(buildingNumber) >=MinimumBuildingNumberValue )
#pragma warning restore CA1305
        {
            errors.Add(DomainErrors.AddressError.MinimumLength(nameof(buildingNumber), MinimumBuildingNumberValue));
        }
        if (string.IsNullOrWhiteSpace(city))
        {
            errors.Add(DomainErrors.Required(nameof(city)));
        }
        if (string.IsNullOrWhiteSpace(region))
        {
            errors.Add(DomainErrors.Required(nameof(region)));
        }
        if (string.IsNullOrWhiteSpace(postalCode))
        {
            errors.Add(DomainErrors.Required(nameof(postalCode)));
        }

        if (!IsValidPostalCode(postalCode))
        {
            errors.Add(DomainErrors.AddressError.InvalidPostalCode);
        }

        if (!IsValidRegion(region))
        {
            errors.Add(DomainErrors.AddressError.InvalidRegion(region));
        }
        if (!IsValidProvince(province))
        {
            errors.Add(DomainErrors.AddressError.InvalidRegion(province));
        }
        if (!IsValidCity(city))
        {
            errors.Add(DomainErrors.AddressError.InvalidCity(city));
        }
        if (errors.Any())
        {
            return errors;
        }

        return new Address(streetAddress, buildingNumber, apartment, city, province, region, postalCode, coordinates);
    }

    private static bool IsValidPostalCode(string postalCode)
    {
        if (postalCode.Length != 5 || !postalCode.All(Char.IsDigit))
        {
            return false;
        }

        return true;
    }

    private static bool IsValidCity(string city)
    {
        string[] cities =
        [
            "Tanger",
            "Tetouan"
        ];

        return  cities.Contains(city);
    }
    private static bool IsValidRegion(string region)
    {
        string[] regions =
        [
            "Tanger-Tétouan-Al Hoceïma",
            "L'Oriental",
            "Fès-Meknès",
            "Rabat-Salé-Kénitra",
            "Béni Mellal-Khénifra",
            "Casablanca-Settat",
            "Marrakech-Safi",
            "Drâa-Tafilalet",
            "Souss-Massa",
            "Guelmim-Oued Noun",
            "Laâyoune-Sakia El Hamra",
            "Dakhla-Oued Ed-Dahab"
        ];

        return regions.Contains(region);
    }

    private static bool IsValidProvince(string province)
    {
        string[] provinces =
            [
            "Tanger-Assilah", "M'diq-Fnideq", "Tétouan", "Fahs-Anjra", "Larache",
            "Al Hoceïma", "Chefchaouen", "Ouezzane",
            "Oujda-Angad", "Nador", "Driouch", "Jerada", "Berkane", "Taourirt", "Guercif",
            "Fès", "Meknès", "El Hajeb", "Ifrane", "Moulay Yacoub", "Sefrou",
            "Boulemane", "Taounate", "Taza",
            "Rabat", "Salé", "Skhirat-Témara", "Kénitra", "Khémisset", "Sidi Kacem", "Sidi Slimane",
            "Casablanca", "Mohammedia", "El Jadida", "Nouaceur", "Médiouna",
            "Benslimane", "Berrechid", "Settat", "Sidi Bennour",
            "Marrakech", "Chichaoua", "Al Haouz", "El Kelâa des Sraghna", "Essaouira",
            "Rehamna", "Safi", "Youssoufia",
            "Béni Mellal", "Azilal", "Fquih Ben Salah", "Khénifra", "Khouribga",
            "Agadir Ida-Ou-Tanane", "Inezgane-Aït Melloul", "Chtouka-Aït Baha",
            "Taroudant", "Tiznit", "Tata", "Guelmim", "Assa-Zag", "Tan-Tan", "Sidi Ifni",
            "Laâyoune", "Boujdour", "Tarfaya", "Es-Semara",
            "Oued Ed-Dahab", "Aousserd",
            "Errachidia", "Ouarzazate", "Midelt", "Tinghir", "Zagora"
        ];
        return provinces.Contains(province);
    }



    public Address WithCoordinates(Coordinates coordinates) =>
        new Address(StreetAddress, BuildingNumber, Apartment, City, Province, Region, PostalCode, coordinates);


    private string GetFullAddress()
    {
        var parts = new List<string>();

        if (!string.IsNullOrWhiteSpace(BuildingNumber))
        {
            parts.Add(BuildingNumber);
        }

        parts.Add(StreetAddress);
        if (!string.IsNullOrWhiteSpace(Apartment))
        {
            parts.Add($"Apt {Apartment}");
        }

        parts.Add(City);
        parts.Add(Province);
        parts.Add(Region);
        parts.Add(PostalCode);

        return string.Join(", ", parts);
    }

    public override string ToString() => GetFullAddress();

}
