using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace PhysicallyFitPT.Domain.Validation;

/// <summary>
/// Custom validation attributes to prevent runtime errors
/// </summary>
public class EmailAddressValidationAttribute : ValidationAttribute
{
    private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);

    public override bool IsValid(object? value)
    {
        if (value is null or string { Length: 0 })
            return true; // Allow null/empty for optional fields

        return value is string email && EmailRegex.IsMatch(email);
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a valid email address";
    }
}

public class PhoneNumberValidationAttribute : ValidationAttribute
{
    private static readonly Regex PhoneRegex = new(@"^[\+]?[1-9][\d]{0,15}$", RegexOptions.Compiled);

    public override bool IsValid(object? value)
    {
        if (value is null or string { Length: 0 })
            return true; // Allow null/empty for optional fields

        if (value is not string phone)
            return false;

        // Remove common formatting characters
        phone = phone.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");
        return PhoneRegex.IsMatch(phone);
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a valid phone number";
    }
}

public class DateRangeValidationAttribute : ValidationAttribute
{
    private readonly DateTime _minDate;
    private readonly DateTime _maxDate;

    public DateRangeValidationAttribute(string minDate, string maxDate)
    {
        _minDate = DateTime.Parse(minDate);
        _maxDate = DateTime.Parse(maxDate);
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
            return true; // Allow null for optional fields

        if (value is DateTime date)
        {
            return date >= _minDate && date <= _maxDate;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be between {_minDate:yyyy-MM-dd} and {_maxDate:yyyy-MM-dd}";
    }
}

public class PercentageRangeAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null)
            return true; // Allow null for optional fields

        if (value is double percentage)
        {
            return percentage >= 0.0 && percentage <= 100.0;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be between 0 and 100";
    }
}

public class AngleMeasurementAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null)
            return true; // Allow null for optional fields

        if (value is int angle)
        {
            return angle >= 0 && angle <= 360;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be between 0 and 360 degrees";
    }
}

public class PainScaleAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        if (value is null or string { Length: 0 })
            return true; // Allow null/empty for optional fields

        if (value is string painValue && int.TryParse(painValue, out int pain))
        {
            return pain >= 0 && pain <= 10;
        }

        return false;
    }

    public override string FormatErrorMessage(string name)
    {
        return $"{name} must be a number between 0 and 10";
    }
}