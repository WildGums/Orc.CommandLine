// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestContextWithFile.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine.Tests.Context
{
    public class TestContextWithMandatoryOption : TestContext
    {
        public TestContextWithMandatoryOption()
        {
            RequiredStringSwitch = string.Empty;
        }

        #region Properties
        [Option('r', "required", IsMandatory = true, HelpText = "Some required string switch")]
        public string RequiredStringSwitch { get; set; }
        #endregion
    }
}