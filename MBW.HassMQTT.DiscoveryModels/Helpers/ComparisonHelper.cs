﻿using System;
using System.Collections;

namespace MBW.HassMQTT.DiscoveryModels.Helpers;

public static class ComparisonHelper
{
    public static bool IsSameValue(object a, object b)
    {
        if (ReferenceEquals(a, b))
            return true;

        if (a == null || b == null)
            return false;

        if (a.GetType() != b.GetType())
            throw new InvalidOperationException($"Attempted to compare two objects of differing types: {a.GetType().FullName} and {b.GetType().FullName}");

        // Arrays
        if (a.GetType().IsArray)
            return IsSameValues((Array)a, (Array)b);

        // Other values
        switch (a)
        {
            case int asInt:
                return asInt == (int)b;
            case string asString:
                return asString.Equals((string)b, StringComparison.Ordinal);
            case float asFloat:
                return Math.Abs(asFloat - (float)b) < float.Epsilon;
            case double asDouble:
                return Math.Abs(asDouble - (double)b) < double.Epsilon;
            case DateTime asDateTime:
                return asDateTime.Equals((DateTime)b);
            case DateTimeOffset asDateTimeOffset:
                return asDateTimeOffset.Equals((DateTimeOffset)b);
            default:
                return Comparer.DefaultInvariant.Compare(a, b) == 0;
        }
    }

    private static bool IsSameValues(Array a, Array b)
    {
        if (a.Length != b.Length)
            return false;

        for (int i = 0; i < a.Length; i++)
        {
            object valA = a.GetValue(i);
            object valB = b.GetValue(i);

            if (!IsSameValue(valA, valB))
                return false;
        }

        return true;
    }
}