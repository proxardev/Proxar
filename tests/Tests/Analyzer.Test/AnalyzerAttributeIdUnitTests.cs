using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Analyzer.Test
{
    public abstract class AnalyzerAttributeIdUnitTests : AnalyzerUnitTestBase
    {
        protected abstract DiagnosticDescriptor RuleRangeError { get; }
        protected abstract DiagnosticDescriptor RuleRepeatId { get; }
        protected abstract string AttributeName { get; }

        protected abstract int MinId { get; }
        protected abstract int MaxId { get; }
        protected abstract int AutoIdStart { get; }
        protected abstract int AutoIdEnd { get; }

        protected abstract Action<CustomTest> ExtInitAction { get; }

        [TestMethod]
        public async Task Service_UseNotAllowId_Error()
        {
            var range = Enumerable.Range(AutoIdStart, Math.Min(100, AutoIdEnd - AutoIdStart + 1));
            foreach (var i in range)
            {

                var test = $@"
using Proxar.Abstractions.Service.Interfaces;
using Proxar.ServiceCore;
using Proxar.Abstractions.Service;

[|[{AttributeName}({i})]
class TestService:IService
{{
}}|]

";
                await VerifyAnalyzerAsync(test, RuleRangeError, ExtInitAction);
            }
        }
        [TestMethod]
        public async Task Service_UseInvalidId_Error()
        {

            var range = Enumerable.Range(MinId - 10, 10)
                .Concat(Enumerable.Range(MaxId + 1, 10));
            foreach (var i in range)
            {
                var test = $@"
using Proxar.Abstractions.Service.Interfaces;
using Proxar.ServiceCore;
using Proxar.Abstractions.Service;

[|[{AttributeName}({i})]
class TestService:IService
{{
}}|]

";
                await VerifyAnalyzerAsync(test, RuleRangeError, ExtInitAction);
            }
        }

        [TestMethod]
        public async Task Service_IdRepeat_Error()
        {
            var test = $@"
using Proxar.Abstractions.Service.Interfaces;
using Proxar.ServiceCore;
using Proxar.Abstractions.Service;

[|[|[{AttributeName}(1)]
class TestService:IService
{{
}}|]|]


[|[|[{AttributeName}(1)]
class TestService2:IService
{{
}}|]|]
";
            await VerifyAnalyzerAsync(test, RuleRepeatId, ExtInitAction);
        }



        [TestMethod]
        public async Task Service_Auto_NotSameNamespace_ButSameName_NotError()
        {
            var test = $@"
using Proxar.Abstractions.Service.Interfaces;
using Proxar.ServiceCore;
using Proxar.Abstractions.Service;

namespace Test1
{{
    [{AttributeName}]
    class TestService:IService
    {{
    }}
}}

namespace Test2
{{
    [{AttributeName}]
    class TestService:IService
    {{
    }}
}}
";
            await VerifyAnalyzerAsync(test, RuleRepeatId, ExtInitAction);
        }

    }
}