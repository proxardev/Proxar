
using FluentAssertions;
using Proxar.MongoDBExtension;
using Proxar.MongoDBExtension.Interfaces;
using Proxar.ServiceCore;
using Proxar.Tasks;
using Proxar.Timer;

namespace ServiceIntegrationTesting;

internal partial class TestService_MongoDB : ServiceBase
{

    private ushort GetMongoDBConfigId()
    {
        return GlobalTestCleanupFixture.GetMongoDBConfigId();
    }

    [ServiceMethod]
    private async ZFTask MongoDB_MultiEntitySaveDeleteMultiTimes_SaveAndDeleteSucc(int count)
    {
        var mgr = EntityRoot.GetRoot()
            .GetDatabaseStratrgyRootEntity(GetMongoDBConfigId())
            .AddComponent<DBEntityManager>();

        count.Should().NotBe(0);

        await mgr.LoadAllDBChild<TaskTest>();
        var list = mgr.GetAllChild<TaskTest>();
        list.Should().BeNullOrEmpty();

        for (int i = 0; i < count; i++)
        {
            var x = mgr.AddChild<TaskTest>();
            x.SetDirty();
        }

        MongoDBSaveContainerActorSingleton
                .Current
                .NowSaveAll();

        var taskTestIds = mgr.GetAllChild<TaskTest>()!
            .Select(x => x.Id)
            .ToList();
        taskTestIds.Sort();

        taskTestIds.Count.Should().Be(count);

        // 从内存移除
        foreach (var id in taskTestIds)
        {
            mgr.RemoveChildById(id);
        }
        list = mgr.GetAllChild<TaskTest>()!;
        list.Should().BeNullOrEmpty();


        // 执行重新加载、验证数量
        await mgr.LoadAllDBChild<TaskTest>();
        list = mgr.GetAllChild<TaskTest>()!;
        var loadIds = list.Select(x => x.Id).Order().ToList();



        var taskTestIds2 = mgr.GetAllChild<TaskTest>()!
            .Select(x => x.Id)
            .ToList();
        taskTestIds2.Sort();
        taskTestIds2.Should().Equal(taskTestIds);

        // 删除
        foreach (var item in list)
        {
            item.SetRemoveMongoDoc();
            item.SetDirty();
        }
        MongoDBSaveContainerActorSingleton.Current
            .NowSaveAll();

        foreach (var id in taskTestIds)
        {
            mgr.RemoveChildById(id);
        }
        list = mgr.GetAllChild<TaskTest>()!;
        list.Should().BeNullOrEmpty();

        //执行重新加载，验证删除情况
        await mgr.LoadAllDBChild<TaskTest>();
        list = mgr.GetAllChild<TaskTest>()!;
        list.Should().BeNullOrEmpty();


        TestResultHelper.SetFinishTest();
    }



    [ServiceMethod]
    private async ZFTask MongoDB_SaveDeleteMultiTimes_QueryNotExistnSucc(int count)
    {
        var configId = Mongo2GoHolder.ConfigId;
        var mgr = EntityRoot.GetRoot()
            .GetDatabaseStratrgyRootEntity(GetMongoDBConfigId())
            .AddComponent<DBEntityManager>();

        count.Should().NotBe(0);

        var id = 3;
        await mgr.LoadDBChild<TaskTest>(id);
        var taskTest = mgr.GetChild<TaskTest>(id);
        taskTest.Should().BeNull();

        // 多次保存、删除，最终删除
        for (int i = 0; i < count; i++)
        {
            taskTest = mgr.AddChildWithId<TaskTest>(id);
            taskTest.SetDirty();
            taskTest.NowSave();

            taskTest.SetRemoveMongoDoc();
            taskTest.SetDirty();
            taskTest.NowSave();
            mgr.RemoveChildById(id);
        }

        taskTest = await mgr.LoadDBChild<TaskTest>(id);
        taskTest.Should().BeNull();

        TestResultHelper.SetFinishTest();
    }

    [ServiceMethod]
    private async ZFTask MongoDB_DeleteSaveMultiTimes_QueryExistSucc(int count)
    {
        var configId = Mongo2GoHolder.ConfigId;
        var mgr = EntityRoot.GetRoot()
            .GetDatabaseStratrgyRootEntity(GetMongoDBConfigId())
            .AddComponent<DBEntityManager>();

        count.Should().NotBe(0);

        var id = 3;
        var taskTest = await mgr.LoadDBChild<TaskTest>(id);
        taskTest.Should().BeNull();

        // 多次保存、删除，最终保存

        taskTest = mgr.AddChildWithId<TaskTest>(id);
        taskTest.SetDirty();
        taskTest.NowSave();
        for (int i = 0; i < count; i++)
        {
            taskTest.SetRemoveMongoDoc();
            taskTest.SetDirty();
            taskTest.NowSave();

            taskTest.SetCancelRemoveMongoDoc();
            taskTest.SetDirty();
            taskTest.NowSave();
        }
        // 验证内存的,存在
        taskTest = mgr.GetChild<TaskTest>(id);
        taskTest.Should().NotBeNull();

        // 删除后验证不存在
        mgr.RemoveChildById(id);
        taskTest = mgr.GetChild<TaskTest>(id);
        taskTest.Should().BeNull();

        // 加载后验证，要存在
        taskTest = await mgr.LoadDBChild<TaskTest>(id);
        taskTest.Should().NotBeNull();

        // 删掉对象，清理数据
        taskTest.SetRemoveMongoDoc();
        taskTest.SetDirty();
        taskTest.NowSave();
        mgr.RemoveChildById(id);

        taskTest = await mgr.LoadDBChild<TaskTest>(id);
        taskTest.Should().BeNull();

        await TimerActorSingleton.Current.Delay(1000);


        TestResultHelper.SetFinishTest();
    }

}