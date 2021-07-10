using FluentAssertions;
using System;
using Xunit;

namespace webApp.Test
{
    public class EnsureTestsCanRun
    {

        [Fact]
        public void EnsureTestsCanPass()
        {
            bool checkVar = true;

            checkVar.Should().Be(true);
        }
    }
}
