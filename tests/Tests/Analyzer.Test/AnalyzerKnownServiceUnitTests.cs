using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proxar.Abstractions.Service;
using Proxar.ServiceCore;
using System;

namespace Analyzer.Test
{

    [TestClass]
    public class AnalyzerKnownServiceUnitTests : AnalyzerAttributeIdUnitTests
    {
        protected override DiagnosticDescriptor RuleRangeError => Rules.ErrorRule_1012;

        protected override DiagnosticDescriptor RuleRepeatId => Rules.ErrorRule_1011;

        protected override string AttributeName => nameof(KnownServiceAttribute);

        protected override int MinId => ServiceConst.MinKnownServiceId;

        protected override int MaxId => ServiceConst.MaxKnownServiceId;

        protected override int AutoIdStart => ServiceConst.AutoKnownServiceIdStart;

        protected override int AutoIdEnd => ServiceConst.AutoKnownServiceIdEnd;

        protected override Action<CustomTest> ExtInitAction => null;
    }
}