
using TestShared.TestClass;
namespace ServiceIntegrationTesting.TestClass;

public class TestClass_TestEntity : TestClassBase
{
    [Theory]
    [InlineData(nameof(TestService_EntityProxy.Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory1))]
    [InlineData(nameof(TestService_EntityProxy.Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory2))]
    [InlineData(nameof(TestService_EntityProxy.Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory3))]
    [InlineData(nameof(TestService_EntityProxy.Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory4))]
    [InlineData(nameof(TestService_EntityProxy.Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory5))]
    [InlineData(nameof(TestService_EntityProxy.Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory6))]
    [InlineData(nameof(TestService_EntityProxy.Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory7))]
    [InlineData(nameof(TestService_EntityProxy.Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory8))]
    [InlineData(nameof(TestService_EntityProxy.Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory9))]


    [InlineData(nameof(TestService_EntityProxy.TestEntity_Create_NotNull))]

    [InlineData(nameof(TestService_EntityProxy.Entity_CreateChildChain_ShouldNotNull))]
    [InlineData(nameof(TestService_EntityProxy.Entity_DestoryEntity_EntityAndChildShouldDispose))]



    [InlineData(nameof(TestService_ComponentProxy.Component_AddComponent_Succ))]
    [InlineData(nameof(TestService_ComponentProxy.Component_IterCreateAndRemove_ShouldIterExistAndNotExist))]
    [InlineData(nameof(TestService_ComponentProxy.Component_IterChainCreate_ShouldNotNull))]
    [InlineData(nameof(TestService_ComponentProxy.Component_DestoryChainParent_ShouldAllChainChildDisposeAndNull))]

    internal async Task CallMethodTest(string methodName, int args = int.MinValue, int args2 = int.MinValue,
        int args3 = int.MinValue,
        int args4 = int.MinValue
        )
    {
        await this.CallMethodByProxy<TestService_Entity, TestService_EntityProxy>(methodName, args, args2, args3, args4);
    }
}