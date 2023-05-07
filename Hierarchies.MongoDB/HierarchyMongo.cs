using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

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

	/// <summary>
	/// Drop the database.
	/// </summary>
	public HierarchyMongo Drop()
	{
		_client.DropDatabase(_databaseName);
		return this;
	}

	/// <summary>
	/// Register conventions necessary for storing <see cref="HierarchyRelation{TId}"/>s with duplicate IDs.
	/// </summary>
	public static void RegisterConventions()
	{
		ConventionRegistry.Register("Loken_Hierarchies_Conventions", new ConventionPack
		{
			new NoIdMemberConvention(),
			new EnumRepresentationConvention(BsonType.String),
			new IgnoreExtraElementsConvention(true),
		}, t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(HierarchyRelation<>));
	}
}
