using ServiceIntegrationTesting;


[CollectionDefinition("GlobalMongoTestCollection", DisableParallelization = true)]
public class GlobalTestCollection : ICollectionFixture<GlobalTestCleanupFixture>
{
}


public class GlobalTestCleanupFixture : IDisposable
{
    public const string MongoDBRunnerName = "TestMongoDB";

    public GlobalTestCleanupFixture()
    {
    }

    public static ushort GetMongoDBConfigId()
    {
        return Mongo2GoHolder.CheckRegisterMongoDBRunner(MongoDBRunnerName);
    }


    public void Dispose()
    {
        // ÄãµÄÇåÀíÂß¼­
        Mongo2GoHolder.Dispose(MongoDBRunnerName);
    }
}