namespace Orc.CommandLine
{
    using Catel.Data;

    public interface IResult : IContext
    {
        #region Properties
        ValidationContext ValidationContext { get; set; }
        #endregion
    }
}
