// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestContextWithFile.cs" company="WildGums">
//   Copyright (c) 2008 - 2015 WildGums. All rights reserved.
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
        [Option(' ', "", DisplayName = "fileName", HelpText = "The file name to start with")]
        public string FileName { get; set; }
        #endregion
    }
}