// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestContextWithFile.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine.Tests.Context
{
    public class TestContext : ContextBase
    {
        public TestContext()
        {
            StringSwitch = string.Empty;
        }

        #region Properties
        [Option('b', "bool", AcceptsValue = false, HelpText = "Some boolean switch")]
        public bool BooleanSwitch { get; set; }

        [Option('i', "int", HelpText = "Some integer switch")]
        public int IntegerSwitch { get; set; }

        [Option('s', "string", HelpText = "Some string switch")]
        public string StringSwitch { get; set; }
        #endregion
    }
}