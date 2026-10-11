
using Proxar.Entity;
using Proxar.Entity.Interfaces;

namespace ServiceIntegrationTesting;



public class TestEntity : EntityBase
{

}

public class TestEntity_CreateWithInt : EntityBase, IFactoryEntity<TestEntity_CreateWithInt, int>
{
    public TestEntity_CreateWithInt(int key) : base()
    {

    }

    public static TestEntity_CreateWithInt FractoryCreateEntity(int args1)
    {

        return new TestEntity_CreateWithInt(args1);
    }
}


public class TestEntity_CreateWithString : EntityBase, IFactoryEntity<TestEntity_CreateWithString, string>
{
    public TestEntity_CreateWithString(string key)
    {
    }

    public static TestEntity_CreateWithString FractoryCreateEntity(string args1)
    {
        return new TestEntity_CreateWithString(args1);
    }
}


public class TestEntity_CreateWith2Int : EntityBase, IFactoryEntity<TestEntity_CreateWith2Int, int, int>
{
    public int Data1;
    public int Data2;

    public TestEntity_CreateWith2Int(int data1, int data2) : base()
    {
        Data1 = data1;
        Data2 = data2;
    }

    public static TestEntity_CreateWith2Int FractoryCreateEntity(int args1, int args2)
    {
        return new TestEntity_CreateWith2Int(args1, args2);
    }
}