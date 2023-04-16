namespace Loken.Hierarchies.Data.MongoDB;

/// <summary>
/// Provides easy access to a <see cref="Database"/> to use for
/// storing <see cref="HierarchyRelation{TId}"/>s for various concepts.
/// </summary>
public class HierarchyMongo
{
	private readonly IMongoClient _client;
	private readonly string _databaseName;

	public IMongoDatabase Database => _client.GetDatabase(_databaseName);

	public HierarchyMongo(IMongoClient client, string databaseName)
	{
		_client = client;
		_databaseName = databaseName;
	}

	public HierarchyMongo Drop()
	{
		_client.DropDatabase(_databaseName);
		return this;
	}
}
