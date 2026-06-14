namespace LoanHub.Api.Common;

public static class DateHelpers
{
    public static int AgeInYears(DateOnly birthDate, DateOnly asOf)
    {
        var age = asOf.Year - birthDate.Year;
        if (birthDate > asOf.AddYears(-age))
        {
            age--;
        }

        return age;
    }
}
