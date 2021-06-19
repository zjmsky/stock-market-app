using System;
using Xunit;
using FluentAssertions;

namespace StockMarket.Integration.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            4.Should().Be(4);
        }
    }
}
