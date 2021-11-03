namespace Orc.CommandLine
{
    using System.Collections.Generic;
    using Catel.Data;

    public interface IResult : IContext
    {
        #region Properties
        ValidationContext ValidationContext { get; set; }
        #endregion
    }
}
