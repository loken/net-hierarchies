namespace Loken.Hierarchies;

public class NodeChildrenTamperingTests
{
	[Fact]
	public void Children_IListMutation_ThrowsNotSupportedException()
	{
		// Arrange
		var parent = new Node<string> { Item = "Parent" };
		var child1 = new Node<string> { Item = "Child1" };
		var child2 = new Node<string> { Item = "Child2" };
		parent.Attach(child1, child2);
		var children = parent.Children;

		// Act & Assert: All mutation attempts via IList should throw
		var list = children as IList<Node<string>>;
		var malicious = new Node<string> { Item = "Malicious" };
		Assert.NotNull(list);
		Assert.True(list.IsReadOnly, "IList should be read-only");
		Assert.Throws<NotSupportedException>(() => list.Add(malicious));
		Assert.Throws<NotSupportedException>(() => list.Insert(0, malicious));
		Assert.Throws<NotSupportedException>(() => list.Remove(child1));
		Assert.Throws<NotSupportedException>(() => list.RemoveAt(0));
		Assert.Throws<NotSupportedException>(() => list.Clear());
		Assert.Throws<NotSupportedException>(() => list[0] = malicious);
	}
}
