
namespace ServiceIntegrationTesting.TestClass;

using TestShared.TestClass;

[Collection("GlobalMongoTestCollection")]
public class TestClass_MongoDB : TestClassBase
{

    [Theory]
    [InlineData(nameof(TestService_MongoDBProxy.MongoDB_MultiEntitySaveDeleteMultiTimes_SaveAndDeleteSucc), 1000)]
    [InlineData(nameof(TestService_MongoDBProxy.MongoDB_SaveDeleteMultiTimes_QueryNotExistnSucc), 1000)]
    [InlineData(nameof(TestService_MongoDBProxy.MongoDB_DeleteSaveMultiTimes_QueryExistSucc), 1000)]
    internal async Task CallMethodTest(string methodName, int args = int.MinValue, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
    {
        await this.CallMethodByProxy<TestService_MongoDB, TestService_MongoDBProxy>(methodName, args, args2, args3, args4);
    }
}