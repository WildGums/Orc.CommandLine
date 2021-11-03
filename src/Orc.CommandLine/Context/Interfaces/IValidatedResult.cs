namespace Orc.CommandLine
{
    using Catel.Data;

    public interface IValidatedResult : IContext
    {
        public IValidationContext ValidationContext { get; set; }
    }
}
