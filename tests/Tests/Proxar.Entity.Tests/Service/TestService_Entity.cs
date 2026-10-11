using FluentAssertions;
using Proxar.ServiceCore;
using Proxar.Utilities;

namespace ServiceIntegrationTesting;

internal partial class TestService_Entity : ServiceBase
{

    #region MulTestEntityCreateTest
    [ServiceMethod]
    public void Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory1()
    {
        var list = RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var entity = EntityRoot.GetRoot().AddChildWithId<TestEntity_CreateWith1Args, int>(id, data1);
                return (id, data1, entity);
            })
            .ToList();
        list.AddRange(
            RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var entity = EntityRoot.GetRoot().AddChildWithId(id, TestEntity_CreateWith1Args.FractoryCreateEntity, data1);
                return (id, data1, entity);
            })
            .ToList()
            );

        list.Should().NotBeNullOrEmpty();
        foreach (var (id, data1, entity) in list)
        {
            entity.Id.Should().Be(id);
            entity.Data1.Should().Be(data1);
        }

        foreach (var (id, data1, entity) in list)
        {
            entity.Dispose();
        }

        foreach (var (id, data1, entity) in list)
        {
            entity.InstanceId.Should().Be(0);
            entity.IsDisposed().Should().BeTrue();
            EntityRoot.GetRoot().GetChild<EntityBase>(id).Should().BeNull();
        }
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod]
    public void Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory2()
    {
        var list = RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId<TestEntity_CreateWith2Args, int, string>(id, data1, data2);
                return (id, data1, data2, entity);
            })
            .ToList();
        list.AddRange(
            RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId(id, TestEntity_CreateWith2Args.FractoryCreateEntity, data1, data2);
                return (id, data1, data2, entity);
            })
            .ToList()
            );

        list.Should().NotBeNullOrEmpty();
        foreach (var (id, data1, data2, entity) in list)
        {
            entity.Id.Should().Be(id);
            entity.Data1.Should().Be(data1);
            entity.Data2.Should().Be(data2);
        }

        foreach (var (id, data1, data2, entity) in list)
        {
            entity.Dispose();
        }

        foreach (var (id, data1, data2, entity) in list)
        {
            entity.InstanceId.Should().Be(0);
            entity.IsDisposed().Should().BeTrue();
            EntityRoot.GetRoot().GetChild<EntityBase>(id).Should().BeNull();
        }
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod]
    public void Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory3()
    {
        var list = RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId<TestEntity_CreateWith3Args, int, int, string>(id, data1, data2, data3);
                return (id, data1, data2, data3, entity);
            })
            .ToList();
        list.AddRange(
            RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId(id, TestEntity_CreateWith3Args.FractoryCreateEntity, data1, data2, data3);
                return (id, data1, data2, data3, entity);
            })
            .ToList()
            );

        list.Should().NotBeNullOrEmpty();
        foreach (var (id, data1, data2, data3, entity) in list)
        {
            entity.Id.Should().Be(id);
            entity.Data1.Should().Be(data1);
            entity.Data2.Should().Be(data2);
            entity.Data3.Should().Be(data3);
        }

        foreach (var (id, data1, data2, data3, entity) in list)
        {
            entity.Dispose();
        }

        foreach (var (id, data1, data2, data3, entity) in list)
        {
            entity.InstanceId.Should().Be(0);
            entity.IsDisposed().Should().BeTrue();
            EntityRoot.GetRoot().GetChild<EntityBase>(id).Should().BeNull();
        }
        TestResultHelper.SetFinishTest();
    }


    [ServiceMethod]
    public void Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory4()
    {
        var list = RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random().ToString();
                var data4 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId<TestEntity_CreateWith4Args, int, int, string, string>(id, data1, data2, data3, data4);
                return (id, data1, data2, data3, data4, entity);
            })
            .ToList();
        list.AddRange(
            RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random().ToString();
                var data4 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId(id, TestEntity_CreateWith4Args.FractoryCreateEntity, data1, data2, data3, data4);
                return (id, data1, data2, data3, data4, entity);
            })
            .ToList()
            );

        list.Should().NotBeNullOrEmpty();
        foreach (var (id, data1, data2, data3, data4, entity) in list)
        {
            entity.Id.Should().Be(id);
            entity.Data1.Should().Be(data1);
            entity.Data2.Should().Be(data2);
            entity.Data3.Should().Be(data3);
            entity.Data4.Should().Be(data4);
        }

        foreach (var (id, data1, data2, data3, data4, entity) in list)
        {
            entity.Dispose();
        }

        foreach (var (id, data1, data2, data3, data4, entity) in list)
        {
            entity.InstanceId.Should().Be(0);
            entity.IsDisposed().Should().BeTrue();
            EntityRoot.GetRoot().GetChild<EntityBase>(id).Should().BeNull();
        }
        TestResultHelper.SetFinishTest();
    }


    [ServiceMethod]
    public void Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory5()
    {
        var list = RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random();
                var data4 = RandomHelper.Random().ToString();
                var data5 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId<TestEntity_CreateWith5Args, int, int, int, string, string>(id, data1, data2, data3, data4, data5);
                return (id, data1, data2, data3, data4, data5, entity);
            })
            .ToList();
        list.AddRange(
            RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random();
                var data4 = RandomHelper.Random().ToString();
                var data5 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId(id, TestEntity_CreateWith5Args.FractoryCreateEntity, data1, data2, data3, data4, data5);
                return (id, data1, data2, data3, data4, data5, entity);
            })
            .ToList()
            );

        list.Should().NotBeNullOrEmpty();
        foreach (var (id, data1, data2, data3, data4, data5, entity) in list)
        {
            entity.Id.Should().Be(id);
            entity.Data1.Should().Be(data1);
            entity.Data2.Should().Be(data2);
            entity.Data3.Should().Be(data3);
            entity.Data4.Should().Be(data4);
            entity.Data5.Should().Be(data5);
        }

        foreach (var (id, data1, data2, data3, data4, data5, entity) in list)
        {
            entity.Dispose();
        }

        foreach (var (id, data1, data2, data3, data4, data5, entity) in list)
        {
            entity.InstanceId.Should().Be(0);
            entity.IsDisposed().Should().BeTrue();
            EntityRoot.GetRoot().GetChild<EntityBase>(id).Should().BeNull();
        }
        TestResultHelper.SetFinishTest();
    }


    [ServiceMethod]
    public void Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory6()
    {
        var list = RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random();
                var data4 = RandomHelper.Random().ToString();
                var data5 = RandomHelper.Random().ToString();
                var data6 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId<TestEntity_CreateWith6Args, int, int, int, string, string, string>(id, data1, data2, data3, data4, data5, data6);
                return (id, data1, data2, data3, data4, data5, data6, entity);
            })
            .ToList();
        list.AddRange(
            RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random();
                var data4 = RandomHelper.Random().ToString();
                var data5 = RandomHelper.Random().ToString();
                var data6 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId(id, TestEntity_CreateWith6Args.FractoryCreateEntity, data1, data2, data3, data4, data5, data6);
                return (id, data1, data2, data3, data4, data5, data6, entity);
            })
            .ToList()
            );

        list.Should().NotBeNullOrEmpty();
        foreach (var (id, data1, data2, data3, data4, data5, data6, entity) in list)
        {
            entity.Id.Should().Be(id);
            entity.Data1.Should().Be(data1);
            entity.Data2.Should().Be(data2);
            entity.Data3.Should().Be(data3);
            entity.Data4.Should().Be(data4);
            entity.Data5.Should().Be(data5);
            entity.Data6.Should().Be(data6);
        }

        foreach (var (id, data1, data2, data3, data4, data5, data6, entity) in list)
        {
            entity.Dispose();
        }

        foreach (var (id, data1, data2, data3, data4, data5, data6, entity) in list)
        {
            entity.InstanceId.Should().Be(0);
            entity.IsDisposed().Should().BeTrue();
            EntityRoot.GetRoot().GetChild<EntityBase>(id).Should().BeNull();
        }
        TestResultHelper.SetFinishTest();
    }


    [ServiceMethod]
    public void Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory7()
    {
        var list = RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random();
                var data4 = RandomHelper.Random();
                var data5 = RandomHelper.Random().ToString();
                var data6 = RandomHelper.Random().ToString();
                var data7 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId<TestEntity_CreateWith7Args, int, int, int, int, string, string, string>(id, data1, data2, data3, data4, data5, data6, data7);
                return (id, data1, data2, data3, data4, data5, data6, data7, entity);
            })
            .ToList();
        list.AddRange(
            RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random();
                var data4 = RandomHelper.Random();
                var data5 = RandomHelper.Random().ToString();
                var data6 = RandomHelper.Random().ToString();
                var data7 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId(id, TestEntity_CreateWith7Args.FractoryCreateEntity, data1, data2, data3, data4, data5, data6, data7);
                return (id, data1, data2, data3, data4, data5, data6, data7, entity);
            })
            .ToList()
            );

        list.Should().NotBeNullOrEmpty();
        foreach (var (id, data1, data2, data3, data4, data5, data6, data7, entity) in list)
        {
            entity.Id.Should().Be(id);
            entity.Data1.Should().Be(data1);
            entity.Data2.Should().Be(data2);
            entity.Data3.Should().Be(data3);
            entity.Data4.Should().Be(data4);
            entity.Data5.Should().Be(data5);
            entity.Data6.Should().Be(data6);
            entity.Data7.Should().Be(data7);
        }

        foreach (var (id, data1, data2, data3, data4, data5, data6, data7, entity) in list)
        {
            entity.Dispose();
        }

        foreach (var (id, data1, data2, data3, data4, data5, data6, data7, entity) in list)
        {
            entity.InstanceId.Should().Be(0);
            entity.IsDisposed().Should().BeTrue();
            EntityRoot.GetRoot().GetChild<EntityBase>(id).Should().BeNull();
        }
        TestResultHelper.SetFinishTest();
    }


    [ServiceMethod]
    public void Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory8()
    {
        var list = RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random();
                var data4 = RandomHelper.Random();
                var data5 = RandomHelper.Random().ToString();
                var data6 = RandomHelper.Random().ToString();
                var data7 = RandomHelper.Random().ToString();
                var data8 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId<TestEntity_CreateWith8Args, int, int, int, int, string, string, string, string>(id, data1, data2, data3, data4, data5, data6, data7, data8);
                return (id, data1, data2, data3, data4, data5, data6, data7, data8, entity);
            })
            .ToList();
        list.AddRange(
            RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random();
                var data4 = RandomHelper.Random();
                var data5 = RandomHelper.Random().ToString();
                var data6 = RandomHelper.Random().ToString();
                var data7 = RandomHelper.Random().ToString();
                var data8 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId(id, TestEntity_CreateWith8Args.FractoryCreateEntity, data1, data2, data3, data4, data5, data6, data7, data8);
                return (id, data1, data2, data3, data4, data5, data6, data7, data8, entity);
            })
            .ToList()
            );

        list.Should().NotBeNullOrEmpty();
        foreach (var (id, data1, data2, data3, data4, data5, data6, data7, data8, entity) in list)
        {
            entity.Id.Should().Be(id);
            entity.Data1.Should().Be(data1);
            entity.Data2.Should().Be(data2);
            entity.Data3.Should().Be(data3);
            entity.Data4.Should().Be(data4);
            entity.Data5.Should().Be(data5);
            entity.Data6.Should().Be(data6);
            entity.Data7.Should().Be(data7);
            entity.Data8.Should().Be(data8);
        }

        foreach (var (id, data1, data2, data3, data4, data5, data6, data7, data8, entity) in list)
        {
            entity.Dispose();
        }

        foreach (var (id, data1, data2, data3, data4, data5, data6, data7, data8, entity) in list)
        {
            entity.InstanceId.Should().Be(0);
            entity.IsDisposed().Should().BeTrue();
            EntityRoot.GetRoot().GetChild<EntityBase>(id).Should().BeNull();
        }
        TestResultHelper.SetFinishTest();
    }


    [ServiceMethod]
    public void Entity_CreateChildWithIdEnDestory_EntityIdShouldSameAndLateDestory9()
    {
        var list = RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random();
                var data4 = RandomHelper.Random();
                var data5 = RandomHelper.Random();
                var data6 = RandomHelper.Random().ToString();
                var data7 = RandomHelper.Random().ToString();
                var data8 = RandomHelper.Random().ToString();
                var data9 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId<TestEntity_CreateWith9Args, int, int, int, int, int, string, string, string, string>(id, data1, data2, data3, data4, data5, data6, data7, data8, data9);
                return (id, data1, data2, data3, data4, data5, data6, data7, data8, data9, entity);
            })
            .ToList();
        list.AddRange(
            RangeHelper.Range(100)
            .Select(x =>
            {
                var id = EntityBase.NewId();
                var data1 = RandomHelper.Random();
                var data2 = RandomHelper.Random();
                var data3 = RandomHelper.Random();
                var data4 = RandomHelper.Random();
                var data5 = RandomHelper.Random();
                var data6 = RandomHelper.Random().ToString();
                var data7 = RandomHelper.Random().ToString();
                var data8 = RandomHelper.Random().ToString();
                var data9 = RandomHelper.Random().ToString();
                var entity = EntityRoot.GetRoot().AddChildWithId(id, TestEntity_CreateWith9Args.FractoryCreateEntity, data1, data2, data3, data4, data5, data6, data7, data8, data9);
                return (id, data1, data2, data3, data4, data5, data6, data7, data8, data9, entity);
            })
            .ToList()
            );

        list.Should().NotBeNullOrEmpty();
        foreach (var (id, data1, data2, data3, data4, data5, data6, data7, data8, data9, entity) in list)
        {
            entity.Id.Should().Be(id);
            entity.Data1.Should().Be(data1);
            entity.Data2.Should().Be(data2);
            entity.Data3.Should().Be(data3);
            entity.Data4.Should().Be(data4);
            entity.Data5.Should().Be(data5);
            entity.Data6.Should().Be(data6);
            entity.Data7.Should().Be(data7);
            entity.Data8.Should().Be(data8);
            entity.Data9.Should().Be(data9);
        }

        foreach (var (id, data1, data2, data3, data4, data5, data6, data7, data8, data9, entity) in list)
        {
            entity.Dispose();
        }

        foreach (var (id, data1, data2, data3, data4, data5, data6, data7, data8, data9, entity) in list)
        {
            entity.InstanceId.Should().Be(0);
            entity.IsDisposed().Should().BeTrue();
            EntityRoot.GetRoot().GetChild<EntityBase>(id).Should().BeNull();
        }
        TestResultHelper.SetFinishTest();
    }
    #endregion


    [ServiceMethod]
    private void TestEntity_Create_NotNull()
    {
        var testEntity = EntityRoot.GetRoot().AddChild<TestEntity>();
        var testEntityChildList = new List<TestEntity>();
        testEntity.Should().NotBeNull();
        TestResultHelper.SetFinishTest();
    }


    private List<TestEntity> CreateChainEntityChildList(int count = 1000)
    {
        var testEntity = EntityRoot.GetRoot().AddChild<TestEntity>();
        var listEntity = new List<TestEntity>();
        listEntity.Add(testEntity);
        foreach (var _ in RangeHelper.Range(1000))
        {
            testEntity = testEntity.AddChild<TestEntity>();
            listEntity.Add(testEntity);
        }
        return listEntity;
    }


    [ServiceMethod]
    private void Entity_CreateChildChain_ShouldNotNull()
    {

        var listEntity = CreateChainEntityChildList();

        listEntity.Should().NotBeNullOrEmpty();
        foreach (var checkTestEntity in listEntity)
        {
            checkTestEntity.Should().NotBeNull();
            checkTestEntity.InstanceId.Should().NotBe(0);
            checkTestEntity.Id.Should().NotBe(0);
        }
        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod]
    private void Entity_DestoryEntity_EntityAndChildShouldDispose()
    {

        var listEntity = CreateChainEntityChildList();
        var first = listEntity.First();
        var id = first.Id;
        first.Dispose();

        var disposeTestEntity = EntityRoot.GetRoot().GetChild<TestEntity>(id);
        disposeTestEntity.Should().BeNull();
        listEntity.Should().NotBeNullOrEmpty();
        foreach (var checkTestEntity in listEntity)
        {
            checkTestEntity.Should().NotBeNull();
            checkTestEntity.IsDisposed().Should().BeTrue();
            checkTestEntity.InstanceId.Should().Be(0);
        }

        TestResultHelper.SetFinishTest();
    }

}