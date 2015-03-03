// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestContextWithFile.cs" company="Wild Gums">
//   Copyright (c) 2008 - 2015 Wild Gums. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------


namespace Orc.CommandLine.Tests.Context
{
    public class TestContextWithFile : TestContext
    {
        public TestContextWithFile()
        {
            FileName = string.Empty;
        }

        #region Properties
        [Option(' ', "", HelpText = "The file name to start with")]
        public string FileName { get; set; }
        #endregion
    }
}