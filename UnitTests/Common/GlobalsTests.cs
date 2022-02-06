using Eraware.Modules.SummitApiDemo.Common;
using System;
using Xunit;

namespace UnitTests.Common
{
    public class GlobalsTests
    {
        [Fact]
        public void ModulePrefixIsValid()
        {
            var modulePrefix = Globals.ModulePrefix;
            Assert.Equal(expected: "Era_SummitApiDemo_", modulePrefix);
        }
    }
}
