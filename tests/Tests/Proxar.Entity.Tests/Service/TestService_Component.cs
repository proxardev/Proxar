using FluentAssertions;
using Proxar.ServiceCore;
using Proxar.Utilities;

namespace ServiceIntegrationTesting;

internal partial class TestService_Component : ServiceBase
{
    [ServiceMethod]
    private void Component_AddComponent_Succ()
    {
        EntityRoot.GetRoot().GetComponent<EntityBase>()?.Dispose();

        var entityBase = EntityRoot.GetRoot().AddComponent<EntityBase>();
        entityBase.Should().NotBeNull();
        entityBase.IsDisposed().Should().BeFalse();

        entityBase.Dispose();

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod]
    private void Component_IterCreateAndRemove_ShouldIterExistAndNotExist()
    {
        EntityRoot.GetRoot().GetComponent<EntityBase>()?.Dispose();
        int count = 1000;
        var list = RangeHelper.Range(count)
            .Select(
            x =>
            {
                var component = EntityRoot.GetRoot().AddComponent<EntityBase>();
                var instId = component.InstanceId;
                var id = component.Id;

                component.Should().NotBeNull();
                component.IsDisposed().Should().BeFalse();

                component.Dispose();

                component = EntityRoot.GetRoot().GetComponent<EntityBase>();
                component.Should().BeNull();


                return (instId, id);
            }
            )
            .ToList();

        list.Should().NotBeNullOrEmpty();
        list.Count.Should().Be(count);

        var instIdList = list
            .Select(x => x.instId)
            .ToList();
        var idList = list
            .Select(x => x.id)
            .ToList();

        instIdList.ToHashSet().Count.Should().Be(instIdList.Count);
        idList.ToHashSet().Count.Should().Be(1);
        idList.First().Should().Be(EntityRoot.GetRoot().Id);


        EntityRoot.GetRoot().GetComponent<EntityBase>().Should().BeNull();

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod]
    private void Component_IterChainCreate_ShouldNotNull()
    {
        EntityRoot.GetRoot().GetComponent<EntityBase>()?.Dispose();
        EntityBase? component = EntityRoot.GetRoot();
        int count = 1000;
        var list = RangeHelper.Range(count)
            .Select(
            x =>
            {
                component = component.AddComponent<EntityBase>();
                var instId = component.InstanceId;
                var id = component.Id;

                component.Should().NotBeNull();
                component.IsDisposed().Should().BeFalse();


                return (instId, id);
            }
            )
            .ToList();

        list.Should().NotBeNullOrEmpty();
        list.Count.Should().Be(count);

        var instIdList = list
            .Select(x => x.instId)
            .ToList();
        var idList = list
            .Select(x => x.id)
            .ToList();

        instIdList.ToHashSet().Count.Should().Be(instIdList.Count);
        idList.ToHashSet().Count.Should().Be(1);
        idList.First().Should().Be(EntityRoot.GetRoot().Id);

        EntityRoot.GetRoot().GetComponent<EntityBase>()!.Dispose();

        TestResultHelper.SetFinishTest();

    }

    [ServiceMethod]
    private void Component_DestoryChainParent_ShouldAllChainChildDisposeAndNull()
    {
        EntityRoot.GetRoot().GetComponent<EntityBase>()?.Dispose();
        EntityBase? component = EntityRoot.GetRoot();
        int count = 1000;
        var list = RangeHelper.Range(count)
            .Select(
            x =>
            {
                component = component.AddComponent<EntityBase>();
                var instId = component.InstanceId;
                var id = component.Id;

                component.Should().NotBeNull();
                component.IsDisposed().Should().BeFalse();


                return component;
            }
            )
            .ToList();

        list.Should().NotBeNullOrEmpty();
        list.Count.Should().Be(count);

        EntityRoot.GetRoot().GetComponent<EntityBase>()!.Dispose();

        foreach (var entity in list)
        {
            entity.IsDisposed().Should().BeTrue();
        }


        EntityRoot.GetRoot().GetComponent<EntityBase>().Should().BeNull();

        TestResultHelper.SetFinishTest();
    }
}