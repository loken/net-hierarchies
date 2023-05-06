using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Loken.Hierarchies.Data.MongoDB;

public class DbFixture : IDisposable
{
	static DbFixture()
	{
		DotEnv.Load();

		var config = new ConfigurationBuilder()
			.AddEnvironmentVariables()
			.Build();

		var username = config.GetSection("MONGO_INITDB_ROOT_USERNAME").Value;
		var password = config.GetSection("MONGO_INITDB_ROOT_PASSWORD").Value;
		var connectionString = $"mongodb://{username}:{password}@localhost:27017";

		var collection = new ServiceCollection();
		collection.AddSingleton<IMongoClient>(new MongoClient(connectionString));

		collection.BuildServiceProvider();

		var services = collection.BuildServiceProvider();

		HierarchyMongo.RegisterConventions();
		Client = services.GetRequiredService<IMongoClient>();
	}

	public DbFixture()
	{
	}

	protected ISet<string> DatabaseNames { get; } = new HashSet<string>();

	protected static IMongoClient Client { get; }

	public IMongoDatabase GetDatabase(string? name = default)
	{
		name ??= $"test_{Guid.NewGuid()}";
		DatabaseNames.Add(name);
		return Client.GetDatabase(name);
	}

	public void Dispose()
	{
		foreach (var dbName in DatabaseNames)
			Client.DropDatabase(dbName);
	}
}