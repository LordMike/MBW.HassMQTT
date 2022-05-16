#nullable enable

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace MBW.HassMQTT.DiscoveryModels.Helpers;

internal static class ReflectionHelpers
{
    public static PropertyInfo GetProperty<TType, TProperty>(this Expression<Func<TType, TProperty>> expression)
    {
        MemberExpression member = expression.Body as MemberExpression;
        if (member == null)
            throw new ArgumentException($"Expression '{expression}' refers to a method, not a property.");

        PropertyInfo propInfo = member.Member as PropertyInfo;
        if (propInfo == null)
            throw new ArgumentException($"Expression '{expression}' refers to a field, not a property.");

        return propInfo;
    }

    public static Expression<Func<TType, TProperty>> GetPropertyExpression<TType, TProperty>(this Type type, PropertyInfo property)
    {
        ParameterExpression pe = Expression.Parameter(typeof(TType), "x");
        MemberExpression member = Expression.Property(pe, property);

        Expression<Func<TType, TProperty>> propExpression = Expression.Lambda<Func<TType, TProperty>>(member, pe);

        return propExpression;
    }

    public static Expression<Func<TType, bool>> GetNotNullExpression<TType>(this Type type, PropertyInfo property)
    {
        ParameterExpression pe = Expression.Parameter(typeof(TType), "x");
        MemberExpression member = Expression.Property(pe, property);

        BinaryExpression nullCompare = Expression.NotEqual(Expression.Constant(null), member);
        Expression<Func<TType, bool>> notNullExpression = Expression.Lambda<Func<TType, bool>>(nullCompare, pe);

        return notNullExpression;
    }

    public static bool IsNullable(this PropertyInfo property) =>
        IsNullableHelper(property.PropertyType, property.DeclaringType, property.CustomAttributes);

    public static bool IsNullable(this FieldInfo field) =>
        IsNullableHelper(field.FieldType, field.DeclaringType, field.CustomAttributes);

    public static bool IsNullable(this ParameterInfo parameter) =>
        IsNullableHelper(parameter.ParameterType, parameter.Member, parameter.CustomAttributes);

    private static bool IsNullableHelper(Type memberType, MemberInfo? declaringType, IEnumerable<CustomAttributeData> customAttributes)
    {
        if (memberType.IsValueType)
            return Nullable.GetUnderlyingType(memberType) != null;

        var nullable = customAttributes
            .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute");
        if (nullable != null && nullable.ConstructorArguments.Count == 1)
        {
            var attributeArgument = nullable.ConstructorArguments[0];
            if (attributeArgument.ArgumentType == typeof(byte[]))
            {
                var args = (ReadOnlyCollection<CustomAttributeTypedArgument>)attributeArgument.Value!;
                if (args.Count > 0 && args[0].ArgumentType == typeof(byte))
                {
                    return (byte)args[0].Value! == 2;
                }
            }
            else if (attributeArgument.ArgumentType == typeof(byte))
            {
                return (byte)attributeArgument.Value! == 2;
            }
        }

        for (var type = declaringType; type != null; type = type.DeclaringType)
        {
            var context = type.CustomAttributes
                .FirstOrDefault(x => x.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute");
            if (context != null &&
                context.ConstructorArguments.Count == 1 &&
                context.ConstructorArguments[0].ArgumentType == typeof(byte))
            {
                return (byte)context.ConstructorArguments[0].Value! == 2;
            }
        }

        // Couldn't find a suitable attribute
        return false;
    }
}