using System;
using MBW.HassMQTT.DiscoveryModels.Helpers;
using Xunit;

namespace MBW.HassMQTT.Tests;

public class ComparisonHelperTests
{
    [Fact]
    public void SameValueTests()
    {
        DateTime dt = new DateTime(2021, 10, 11, 18, 33, 12, DateTimeKind.Utc);
        DateTime dt2 = new DateTime(2021, 10, 11, 18, 33, 12, DateTimeKind.Utc);
        Assert.True(ComparisonHelper.IsSameValue(dt, dt2));

        DateTime dt3 = new DateTime(2021, 10, 11, 18, 33, 11, DateTimeKind.Utc);
        Assert.False(ComparisonHelper.IsSameValue(dt, dt3));

        Assert.True(ComparisonHelper.IsSameValue(50, 50));
        Assert.False(ComparisonHelper.IsSameValue(50, 51));

        Assert.True(ComparisonHelper.IsSameValue(1152921504606846976L, 1152921504606846976L));
        Assert.False(ComparisonHelper.IsSameValue(1152921504606846977L, 1152921504606846975L));
    }
}