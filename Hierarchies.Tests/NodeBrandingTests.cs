namespace Loken.Hierarchies;

public class NodeBrandingTests
{
	[Fact]
	public void Attach_UnbrandedAndBranded_Throws()
	{
		var a = Node.Create("A");
		var b = Node.Create("B");

		a.Brand("some-brand");

		Assert.Throws<ArgumentException>("nodes", () => a.Attach(b));
		Assert.Throws<ArgumentException>("nodes", () => b.Attach(a));
	}

	[Fact]
	public void Attach_DifferentBrands_Throws()
	{
		var a = Node.Create("A");
		var b = Node.Create("B");

		a.Brand("some-brand");
		b.Brand("other-brand");

		Assert.Throws<ArgumentException>("nodes", () => a.Attach(b));
		Assert.Throws<ArgumentException>("nodes", () => b.Attach(a));
	}

	[Fact]
	public void Detach_Branded_Throws()
	{
		var a = Node.Create("A");
		var b = Node.Create("B");

		a.Brand("some-brand");
		b.Brand("some-brand");

		a.Attach(b);

		Assert.Throws<InvalidOperationException>(() => a.Detach(b));
	}

	[Fact]
	public void DetachDelegate_Branded_Works()
	{
		var a = Node.Create("A");
		var b = Node.Create("B");

		var debrandA = a.Brand("some-brand");
		var debrandB = b.Brand("some-brand");

		a.Attach(b);

		// We only need to debrand the node we're going to detach,
		// not the node we're detaching from!
		debrandB();

		a.Detach(b);
	}
}
