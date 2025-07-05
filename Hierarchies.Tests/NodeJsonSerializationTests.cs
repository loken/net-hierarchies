using Newtonsoft.Json;

namespace Loken.Hierarchies;
public class NodeJsonSerializationTests
{
	[Fact]
	public void JsonConvert_BothWays_Works()
	{
		const string expectedJson = """
		{
		  "Children": [
		    {
		      "Children": null,
		      "Item": "a"
		    }
		  ],
		  "Item": "root"
		}
		""";

		var root = Nodes.Create("root").Attach(Nodes.Create("a"));
		var nodes = root.GetDescendants(true).ToArray();

		var actualJson = JsonConvert.SerializeObject(root, Formatting.Indented);
		Assert.Equal(expectedJson, actualJson);

		var deserializedRoot = JsonConvert.DeserializeObject<Node<string>>(actualJson)!;
		var deserializedNodes = deserializedRoot.GetDescendants(true).ToArray();
		Assert.Equal(nodes, deserializedNodes);
	}
}
