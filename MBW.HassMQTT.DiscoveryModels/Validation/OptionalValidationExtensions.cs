#nullable enable

using System;
using FluentValidation;

namespace MBW.HassMQTT.DiscoveryModels.Validation;

internal static class OptionalValidationExtensions
{
    public static IRuleBuilderOptions<T, Optional<TValue?>> IsInEnumWhenSet<T, TValue>(
        this IRuleBuilder<T, Optional<TValue?>> ruleBuilder)
        where TValue : struct, System.Enum
    {
        return ruleBuilder.Must(optional =>
                !optional.IsSet ||
                !optional.Value.HasValue ||
                System.Enum.IsDefined(typeof(TValue), optional.Value.Value))
            .WithMessage("{PropertyName} has a range of values which does not include '{PropertyValue}'.");
    }

    public static IRuleBuilderOptions<T, Optional<TValue?>> MustWhenSet<T, TValue>(
        this IRuleBuilder<T, Optional<TValue?>> ruleBuilder,
        Func<TValue, bool> predicate)
        where TValue : struct
    {
        return ruleBuilder.Must(optional =>
            !optional.IsSet || !optional.Value.HasValue || predicate(optional.Value.Value));
    }
}
