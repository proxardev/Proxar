using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proxar.Abstractions.Service;
using Proxar.ServiceCore;
using System;
using System.Threading.Tasks;

namespace Analyzer.Test
{

    [TestClass]
    public class AnalyzerExternalProxyUnitTests : AnalyzerAttributeIdUnitTests
    {
        protected override DiagnosticDescriptor RuleRangeError => Rules.ErrorRule_1004;

        protected override DiagnosticDescriptor RuleRepeatId => Rules.ErrorRule_1003;

        protected override string AttributeName => nameof(ServiceExportAttribute);

        protected override int MinId => ServiceConst.MinProxyId;

        protected override int MaxId => ServiceConst.MaxProxyId;

        protected override int AutoIdStart => ServiceConst.AutoProxyIdStart;

        protected override int AutoIdEnd => ServiceConst.AutoProxyIdEnd;

        protected override Action<CustomTest> ExtInitAction => (test) =>
        {
            string config = $@"is_global = true
build_property.{ServiceInfo.ModuleExternalProxyPreIdField} = 1";

            test.TestState.AnalyzerConfigFiles.Add(("/.globalconfig", config));
        };

        [TestMethod]
        public async Task ExternalService_UseServiceExportAndNotDefineModuleId_Error()
        {
            var test = @"
using Proxar.Abstractions.Service.Interfaces;
using Proxar.ServiceCore;
using Proxar.Abstractions.Service;

[|[ServiceExport(1)]
class TestService:IService
{
}|]

";

            Action<CustomTest> extInitAction = (test) =>
            {
                string config = $@"is_global = true
build_property.{ServiceInfo.ModuleExternalProxyPreIdField} = 1";

                test.TestState.AnalyzerConfigFiles.Add(("/.globalconfig", config));
            };
            await VerifyAnalyzerAsync(test, Rules.ErrorRule_1001);
        }
    }
}