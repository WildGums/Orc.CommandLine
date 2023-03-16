namespace Orc.CommandLine.Tests
{
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using PublicApiGenerator;
    using VerifyNUnit;

        PublicApiApprover.ApprovePublicApi(assembly);
    }

    internal static class PublicApiApprover
    {
        [Test, MethodImpl(MethodImplOptions.NoInlining)]
        public async Task Orc_CommandLine_HasNoBreakingChanges_Async()
        {
            var assembly = typeof(CommandLineParser).Assembly;

            await PublicApiApprover.ApprovePublicApiAsync(assembly);
        }
    }

    internal class AssemblyPathNamer : UnitTestFrameworkNamer
    {
        private readonly string _name;

        public AssemblyPathNamer(string assemblyPath)
        {
            public static async Task ApprovePublicApiAsync(Assembly assembly)
            {
                var publicApi = ApiGenerator.GeneratePublicApi(assembly, new ApiGeneratorOptions());
                await Verifier.Verify(publicApi);
            }
        }
    }
}
