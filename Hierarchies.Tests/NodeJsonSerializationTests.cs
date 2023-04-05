using Newtonsoft.Json;

namespace Loken.Hierarchies;
public class NodeJsonSerializationTests
{
	[Fact]
	public void Stuff()
	{
		const string expectedJson = """
		{
		  "Children": [
		    {
		      "Children": null,
		      "Value": "a"
		    }
		  ],
		  "Value": "root"
		}
		""";

		var root = Node.Create("root").Attach(Node.Create("a"));
		var nodes = root.GetDescendants(true).ToArray();

		var actualJson = JsonConvert.SerializeObject(root, Formatting.Indented);
		Assert.Equal(expectedJson, actualJson);

		var deserializedRoot = JsonConvert.DeserializeObject<Node<string>>(actualJson)!;
		var deserializedNodes = deserializedRoot.GetDescendants(true).ToArray();
		Assert.Equal(nodes, deserializedNodes);
	}
}
