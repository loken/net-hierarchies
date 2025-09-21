namespace Loken.Hierarchies;

public class NodeBrandingTests
{
	// Basic branding functionality tests
	[Fact]
	public void NodeBrand_WithUndefined_Throws()
	{
		var a = Nodes.Create("A");

		// In C#, we can't pass undefined, but we can test with null for similar behavior
		Assert.Throws<ArgumentNullException>(() => a.Brand(null!));
	}

	[Fact]
	public void NodeBrand_TwiceWithoutDebranding_Throws()
	{
		var a = Nodes.Create("A");

		a.Brand("first-brand");

		Assert.Throws<InvalidOperationException>(() => a.Brand("second-brand"));
	}

	[Fact]
	public void NodeBrand_AndDebrandCycle_Works()
	{
		var a = Nodes.Create("A");

		// Should be able to brand and debrand
		var debrand = a.Brand("some-brand");
		Assert.True(a.IsBranded);
		debrand();
		Assert.False(a.IsBranded);

		// Should be able to brand again after debranding
		var debrand2 = a.Brand("new-brand");
		Assert.True(a.IsBranded);
		debrand2();
		Assert.False(a.IsBranded);
	}

	[Fact]
	public void Debranding_Twice_IsSafe()
	{
		var a = Nodes.Create("A");

		var debrand = a.Brand("some-brand");

		debrand();
		debrand(); // Should not throw
	}

	// Brand value type tests
	[Fact]
	public void NodeBrand_WithString_Works()
	{
		var a = Nodes.Create("A");

		var debrand = a.Brand("test-brand");
		Assert.True(a.IsBranded);

		debrand();
		Assert.False(a.IsBranded);
	}

	[Fact]
	public void NodeBrand_WithEmptyString_Works()
	{
		var a = Nodes.Create("A");

		var debrand = a.Brand("");
		Assert.True(a.IsBranded);

		debrand();
		Assert.False(a.IsBranded);
	}

	[Fact]
	public void NodeBrand_WithNumber_Works()
	{
		var a = Nodes.Create("A");

		var debrand = a.Brand(42);
		Assert.True(a.IsBranded);

		debrand();
		Assert.False(a.IsBranded);
	}

	[Fact]
	public void NodeBrand_WithObject_Works()
	{
		var a = Nodes.Create("A");
		var brandObject = new { name = "test-brand" };

		var debrand = a.Brand(brandObject);
		Assert.True(a.IsBranded);

		debrand();
		Assert.False(a.IsBranded);
	}

	// Brand compatibility tests
	[Fact]
	public void IsBrandCompatible_UnbrandedNodes_AreCompatible()
	{
		var a = Nodes.Create("A");
		var b = Nodes.Create("B");

		Assert.True(a.IsBrandCompatible(b));
		Assert.True(b.IsBrandCompatible(a));
	}

	[Fact]
	public void IsBrandCompatible_SameBrandNodes_AreCompatible()
	{
		var a = Nodes.Create("A");
		var b = Nodes.Create("B");

		a.Brand("same-brand");
		b.Brand("same-brand");

		Assert.True(a.IsBrandCompatible(b));
		Assert.True(b.IsBrandCompatible(a));
	}

	[Fact]
	public void IsBrandCompatible_DifferentBrandNodes_AreIncompatible()
	{
		var a = Nodes.Create("A");
		var b = Nodes.Create("B");

		a.Brand("brand-a");
		b.Brand("brand-b");

		Assert.False(a.IsBrandCompatible(b));
		Assert.False(b.IsBrandCompatible(a));
	}

	[Fact]
	public void IsBrandCompatible_BrandedAndUnbrandedNodes_AreIncompatible()
	{
		var a = Nodes.Create("A");
		var b = Nodes.Create("B");

		a.Brand("some-brand");
		// b remains unbranded

		Assert.False(a.IsBrandCompatible(b));
		Assert.False(b.IsBrandCompatible(a));
	}

	[Fact]
	public void NodeAttach_UnbrandedNodes_Works()
	{
		var a = Nodes.Create("A");
		var b = Nodes.Create("B");

		// Should work fine - both unbranded
		a.Attach(b);

		Assert.Equal(a, b.Parent);
		Assert.Contains(b, a.Children);
	}

	[Fact]
	public void NodeAttach_SameBrandNodes_Works()
	{
		var a = Nodes.Create("A");
		var b = Nodes.Create("B");

		a.Brand("same-brand");
		b.Brand("same-brand");

		// Should work fine - same brand
		a.Attach(b);

		Assert.Equal(a, b.Parent);
		Assert.Contains(b, a.Children);
	}

	[Fact]
	public void NodeAttach_UnbrandedAndBranded_Throws()
	{
		var a = Nodes.Create("A");
		var b = Nodes.Create("B");

		a.Brand("some-brand");

		Assert.Throws<InvalidOperationException>(() => a.Attach(b));
		Assert.Throws<InvalidOperationException>(() => b.Attach(a));
	}

	[Fact]
	public void NodeAttach_DifferentBrands_Throws()
	{
		var a = Nodes.Create("A");
		var b = Nodes.Create("B");

		a.Brand("some-brand");
		b.Brand("other-brand");

		Assert.Throws<InvalidOperationException>(() => a.Attach(b));
		Assert.Throws<InvalidOperationException>(() => b.Attach(a));
	}

	[Fact]
	public void NodeDetach_Branded_Throws()
	{
		var a = Nodes.Create("A");
		var b = Nodes.Create("B");

		a.Brand("some-brand");
		b.Brand("some-brand");

		a.Attach(b);

		Assert.Throws<InvalidOperationException>(() => a.Detach(b));
	}

	[Fact]
	public void NodeDetach_AfterDebranding_Works()
	{
		var a = Nodes.Create("A");
		var b = Nodes.Create("B");

		var debrandA = a.Brand("some-brand");
		var debrandB = b.Brand("some-brand");

		a.Attach(b);

		// We only need to debrand the node we're going to detach,
		// not the node we're detaching from!
		debrandB();

		a.Detach(b);
	}
}
