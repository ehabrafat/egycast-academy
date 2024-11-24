namespace EgycastApi.Community;
using System;
using System.ComponentModel.DataAnnotations;

public class ValidEnumAttribute : ValidationAttribute
{
    private readonly Type _enumType;

    public ValidEnumAttribute(Type enumType)
    {
        _enumType = enumType;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || !Enum.IsDefined(_enumType, value.ToString().ToUpper()))
        {
            return new ValidationResult($"{value} is not a valid value for {_enumType.Name}");
        }
        
        return ValidationResult.Success;
    }
}
