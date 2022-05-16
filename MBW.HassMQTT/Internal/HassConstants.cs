using System;

namespace MBW.HassMQTT.Internal;

internal static class HassConstants
{
    public static readonly string DatetimeIso8601 = @"yyyy-MM-ddTHH\:mm\:ss.fffffffzzz";

    public static string ToIso8601(this DateTime dateTime) => dateTime.ToString(DatetimeIso8601);
    public static string ToIso8601(this DateTimeOffset dateTime) => dateTime.ToString(DatetimeIso8601);
}