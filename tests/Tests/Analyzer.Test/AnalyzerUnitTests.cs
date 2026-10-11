using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace Analyzer.Test
{


    [TestClass]
    public class AnalyzerServiceMethodUnitTest : AnalyzerUnitTestBase
    {


        [TestMethod]
        public async Task ServiceMethod_SameProto_Error()
        {
            var test = @"
using Proxar.Abstractions.Service.Interfaces;
using Proxar.ServiceCore;

class TestService:IService
{
    [|[ServiceMethod(1)]
    private void TestMethod1(long serviceId)
    {
    }|]

    [|[ServiceMethod(1)]
    private void TestMethod12(long serviceId)
    {
    }|]
}
";
            await VerifyAnalyzerAsync(test, Rules.ErrorRule_0003);
        }


        [TestMethod]
        public async Task ServiceMethod_SameProto2_Error()
        {
            var test = @"
using Proxar.Abstractions.Service.Interfaces;
using Proxar.ServiceCore;

class TestService:IService
{

    [|[ServiceMethod]
    private void TestAutoProtoMethod(long serviceId)
    {
    }|]

    [|[ServiceMethod]
    private void TestAutoProtoMethod(string serviceId)
    {
    }|]
}
";
            await VerifyAnalyzerAsync(test, Rules.ErrorRule_0003);
        }

        [TestMethod]
        public async Task ServiceMethod_AppointNowAllowProto_Error()
        {
            var test = @"
using Proxar.Abstractions.Service.Interfaces;
using Proxar.ServiceCore;
using Proxar.Abstractions.Service;

class TestService:IService
{
    [|[ServiceMethod(2000)]
    private void TestMethod1(long serviceId)
    {
    }|]

}
";
            await VerifyAnalyzerAsync(test, Rules.ErrorRule_0006);
        }


    }
}