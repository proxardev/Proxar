using Proxar.Entity;
using Proxar.Entity.Interfaces;

namespace ServiceIntegrationTesting;

public class TestEntity_CreateWith1Args : EntityBase, IFactoryEntity<TestEntity_CreateWith1Args, int>
{
    public int Data1;

    public TestEntity_CreateWith1Args(int data1) : base()
    {
        Data1 = data1;
    }

    public static TestEntity_CreateWith1Args FractoryCreateEntity(int args1)
    {
        return new TestEntity_CreateWith1Args(args1);
    }
}

public class TestEntity_CreateWith2Args : EntityBase, IFactoryEntity<TestEntity_CreateWith2Args, int, string>
{
    public int Data1;
    public string Data2;

    public TestEntity_CreateWith2Args(int data1, string data2) : base()
    {
        Data1 = data1;
        Data2 = data2;
    }

    public static TestEntity_CreateWith2Args FractoryCreateEntity(int args1, string args2)
    {
        return new TestEntity_CreateWith2Args(args1, args2);
    }
}

public class TestEntity_CreateWith3Args : EntityBase, IFactoryEntity<TestEntity_CreateWith3Args, int, int, string>
{
    public int Data1;
    public int Data2;
    public string Data3;

    public TestEntity_CreateWith3Args(int data1, int data2, string data3) : base()
    {
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
    }

    public static TestEntity_CreateWith3Args FractoryCreateEntity(int args1, int args2, string args3)
    {
        return new TestEntity_CreateWith3Args(args1, args2, args3);
    }
}

public class TestEntity_CreateWith4Args : EntityBase, IFactoryEntity<TestEntity_CreateWith4Args, int, int, string, string>
{
    public int Data1;
    public int Data2;
    public string Data3;
    public string Data4;

    public TestEntity_CreateWith4Args(int data1, int data2, string data3, string data4) : base()
    {
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
    }

    public static TestEntity_CreateWith4Args FractoryCreateEntity(int args1, int args2, string args3, string args4)
    {
        return new TestEntity_CreateWith4Args(args1, args2, args3, args4);
    }
}

public class TestEntity_CreateWith5Args : EntityBase, IFactoryEntity<TestEntity_CreateWith5Args, int, int, int, string, string>
{
    public int Data1;
    public int Data2;
    public int Data3;
    public string Data4;
    public string Data5;

    public TestEntity_CreateWith5Args(int data1, int data2, int data3, string data4, string data5) : base()
    {
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
        Data5 = data5;
    }

    public static TestEntity_CreateWith5Args FractoryCreateEntity(int args1, int args2, int args3, string args4, string args5)
    {
        return new TestEntity_CreateWith5Args(args1, args2, args3, args4, args5);
    }
}

public class TestEntity_CreateWith6Args : EntityBase, IFactoryEntity<TestEntity_CreateWith6Args, int, int, int, string, string, string>
{
    public int Data1;
    public int Data2;
    public int Data3;
    public string Data4;
    public string Data5;
    public string Data6;

    public TestEntity_CreateWith6Args(int data1, int data2, int data3, string data4, string data5, string data6) : base()
    {
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
        Data5 = data5;
        Data6 = data6;
    }

    public static TestEntity_CreateWith6Args FractoryCreateEntity(int args1, int args2, int args3, string args4, string args5, string args6)
    {
        return new TestEntity_CreateWith6Args(args1, args2, args3, args4, args5, args6);
    }
}

public class TestEntity_CreateWith7Args : EntityBase, IFactoryEntity<TestEntity_CreateWith7Args, int, int, int, int, string, string, string>
{
    public int Data1;
    public int Data2;
    public int Data3;
    public int Data4;
    public string Data5;
    public string Data6;
    public string Data7;

    public TestEntity_CreateWith7Args(int data1, int data2, int data3, int data4, string data5, string data6, string data7) : base()
    {
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
        Data5 = data5;
        Data6 = data6;
        Data7 = data7;
    }

    public static TestEntity_CreateWith7Args FractoryCreateEntity(int args1, int args2, int args3, int args4, string args5, string args6, string args7)
    {
        return new TestEntity_CreateWith7Args(args1, args2, args3, args4, args5, args6, args7);
    }
}

public class TestEntity_CreateWith8Args : EntityBase, IFactoryEntity<TestEntity_CreateWith8Args, int, int, int, int, string, string, string, string>
{
    public int Data1;
    public int Data2;
    public int Data3;
    public int Data4;
    public string Data5;
    public string Data6;
    public string Data7;
    public string Data8;

    public TestEntity_CreateWith8Args(int data1, int data2, int data3, int data4, string data5, string data6, string data7, string data8) : base()
    {
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
        Data5 = data5;
        Data6 = data6;
        Data7 = data7;
        Data8 = data8;
    }

    public static TestEntity_CreateWith8Args FractoryCreateEntity(int args1, int args2, int args3, int args4, string args5, string args6, string args7, string args8)
    {
        return new TestEntity_CreateWith8Args(args1, args2, args3, args4, args5, args6, args7, args8);
    }
}

public class TestEntity_CreateWith9Args : EntityBase, IFactoryEntity<TestEntity_CreateWith9Args, int, int, int, int, int, string, string, string, string>
{
    public int Data1;
    public int Data2;
    public int Data3;
    public int Data4;
    public int Data5;
    public string Data6;
    public string Data7;
    public string Data8;
    public string Data9;

    public TestEntity_CreateWith9Args(int data1, int data2, int data3, int data4, int data5, string data6, string data7, string data8, string data9) : base()
    {
        Data1 = data1;
        Data2 = data2;
        Data3 = data3;
        Data4 = data4;
        Data5 = data5;
        Data6 = data6;
        Data7 = data7;
        Data8 = data8;
        Data9 = data9;
    }

    public static TestEntity_CreateWith9Args FractoryCreateEntity(int args1, int args2, int args3, int args4, int args5, string args6, string args7, string args8, string args9)
    {
        return new TestEntity_CreateWith9Args(args1, args2, args3, args4, args5, args6, args7, args8, args9);
    }
}