using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Threading.Tasks;

namespace Analyzer.Test
{

    public class CustomTest : CSharpAnalyzerTest<ServiceMethodDeclarationAnalyzer, DefaultVerifier>
    {
        private DiagnosticDescriptor diagnosticDescriptor;

        protected override string DefaultTestProjectName => "Analyzer.Test";

        public CustomTest(DiagnosticDescriptor diagnosticDescriptor)
        {
            this.diagnosticDescriptor = diagnosticDescriptor;
        }

        protected override DiagnosticDescriptor GetDefaultDiagnostic(DiagnosticAnalyzer[] analyzers)
        {
            return diagnosticDescriptor;
        }
    }

    public class AnalyzerUnitTestBase
    {
        protected static async Task VerifyAnalyzerAsync(string source, DiagnosticDescriptor diagnosticDescriptor, Action<CustomTest> extInitAction = null)
        {

            var test = new CustomTest(diagnosticDescriptor)
            {
                ReferenceAssemblies = ReferenceAssemblies.Net.Net100
            };
            test.TestState.Sources.Add(SourceText.From(source));
            var assembly = typeof(Proxar.Abstractions.Service.Interfaces.IService).Assembly;
            test.TestState.AdditionalReferences.Add(MetadataReference.CreateFromFile(assembly.Location));
            extInitAction?.Invoke(test);
            await test.RunAsync();
        }
    }
}