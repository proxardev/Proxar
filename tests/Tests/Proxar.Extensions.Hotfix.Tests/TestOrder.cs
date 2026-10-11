//using Xunit.Sdk;
//using Xunit.v3;

//namespace Proxar.Extensions.Hotfix.Tests;

//public class TestOrder : ITestCaseOrderer
//{
//    public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(
//        IReadOnlyCollection<TTestCase> testCases)
//        where TTestCase : notnull, ITestCase
//    {
//        var ordered = testCases.ToList();

//        // 通过 tc.TestMethod.Method 获取 IMethodInfo，然后调用 GetCustomAttributes
//        var firstTest = ordered
//            .FirstOrDefault(tc =>
//                tc.TestMethod.Method.GetCustomAttributes(typeof(RunFirstAttribute), inherit: false).Any());

//        if (firstTest != null)
//        {
//            ordered.Remove(firstTest);
//            // 注意：Concat 需要 using System.Linq，并转换为 List 以匹配返回类型
//            return new[] { firstTest }.Concat(ordered).ToList();
//        }

//        return ordered;
//    }
//}