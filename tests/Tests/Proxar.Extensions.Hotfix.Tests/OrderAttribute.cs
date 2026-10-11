//using System.Reflection;
//using Xunit.Sdk;


//namespace Proxar.Extensions.Hotfix.Tests;



//[AttributeUsage(AttributeTargets.Method)]
//public sealed class OrderAttribute(int priority) : Attribute
//{
//    public int Priority { get; } = priority;
//}

//internal sealed class TestOrder : ITestCaseOrderer
//{
//    public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(
//        IReadOnlyCollection<TTestCase> testCases)
//        where TTestCase : notnull, ITestCase
//    {
//        var first = new List<TTestCase>();
//        var rest = new List<TTestCase>();

//        foreach (var testCase in testCases)
//        {
//            if (testCase.TestMethod is XunitTestMethod xm)
//            {
//                var attr = xm.Method.GetCustomAttribute<OrderAttribute>();
//                (attr is null ? rest : first).Add(testCase);
//            }
//            else
//            {
//                rest.Add(testCase);
//            }
//        }

//        return first.Concat(rest).ToList().AsReadOnly();
//    }
//}