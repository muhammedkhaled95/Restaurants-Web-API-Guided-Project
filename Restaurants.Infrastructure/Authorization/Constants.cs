namespace Restaurants.Infrastructure.Authorization;

public static class PolicyNames
{
    public const string HasNationality = "HasNationality";
    public const string AtLeast20YearsOfAge = "AtLeast20YearsOfAge";
    public const string CreatedAtLeastTwoRestaurants = "CreatedAtLeastTwoRestaurants";
}

public static class AppClaimTypes
{
    public const string Nationality = "Nationality";
    public const string DateOfBirth = "DateOfBirth";
}
