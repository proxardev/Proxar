using Mongo2Go;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Proxar.MongoDBExtension;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace ServiceIntegrationTesting;



public static class Mongo2GoHolder
{
    public static ushort ConfigId = 0;
    public static ConcurrentDictionary<string, (MongoDbRunner, ushort)> mongoDbRunners = new ConcurrentDictionary<string, (MongoDbRunner, ushort)>();


    [ModuleInitializer]
    public static void InitMongodb()
    {
        ConventionPack conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
        ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);

    }

    public static void Dispose()
    {
    }

    public static ushort CheckRegisterMongoDBRunner(string name)
    {
        var (runner, configId) = mongoDbRunners.GetOrAdd(name, AddMongoDBRunner);

        return configId;
    }

    private static (MongoDbRunner, ushort) AddMongoDBRunner(string name)
    {

        var runner = MongoDbRunner.Start();
        var url = runner.ConnectionString;
        //var url = "mongodb://127.0.0.1:27017";
        var mongoClient = new MongoClient(url);
        var config = new DatabaseConfig(mongoClient, url, "IntegrationTesting");
        var configId = DatabaseConfigManagerSingleton.Instance
        .AddConfig(config);
        return (runner, configId);
    }

    public static void Dispose(string name)
    {
        if (mongoDbRunners.TryRemove(name, out var info))
        {
            var (runner, configId) = info;
            runner?.Dispose();
        }
    }
}