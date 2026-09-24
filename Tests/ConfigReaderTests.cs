using System.Xml.Linq;
using Fody;
using Mono.Cecil;

public class ConfigReaderTests
{
    [Test]
    public async Task ExcludeNamespacesNode()
    {
        var xElement = XElement.Parse(
            """

            <EmptyConstructor>
                <ExcludeNamespaces>
            Foo
            Bar
            Foo.Bar
                </ExcludeNamespaces>
            </EmptyConstructor>
            """);
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.ExcludeNamespaces[0]).IsEqualTo("Foo");
        await Assert.That(moduleWeaver.ExcludeNamespaces[1]).IsEqualTo("Bar");
        await Assert.That(moduleWeaver.ExcludeNamespaces[2]).IsEqualTo("Foo.Bar");
    }

    [Test]
    public async Task VisibilityFamily()
    {
        var xElement = XElement.Parse("<EmptyConstructor Visibility='family'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.Visibility).IsEqualTo(MethodAttributes.Family);
    }

    [Test]
    public async Task VisibilityDefault()
    {
        var xElement = XElement.Parse("<EmptyConstructor/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.Visibility).IsEqualTo(MethodAttributes.Public);
    }

    [Test]
    public async Task VisibilityPublic()
    {
        var xElement = XElement.Parse("<EmptyConstructor Visibility='public'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.Visibility).IsEqualTo(MethodAttributes.Public);
    }

    [Test]
    public async Task MakeExistingEmptyConstructorsVisible_Default()
    {
        var xElement = XElement.Parse("<EmptyConstructor/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.MakeExistingEmptyConstructorsVisible).IsFalse();
    }

    [Test]
    public async Task MakeExistingEmptyConstructorsVisible_False()
    {
        var xElement = XElement.Parse("<EmptyConstructor MakeExistingEmptyConstructorsVisible='False'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.MakeExistingEmptyConstructorsVisible).IsFalse();
    }

    [Test]
    public async Task MakeExistingEmptyConstructorsVisible_True()
    {
        var xElement = XElement.Parse("<EmptyConstructor MakeExistingEmptyConstructorsVisible='True'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.MakeExistingEmptyConstructorsVisible).IsTrue();
    }

    [Test]
    public async Task PreserveInitializers_Default()
    {
        var xElement = XElement.Parse("<EmptyConstructor/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.PreserveInitializers).IsFalse();
    }

    [Test]
    public async Task PreserveInitializers_False()
    {
        var xElement = XElement.Parse("<EmptyConstructor PreserveInitializers='False'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.PreserveInitializers).IsFalse();
    }

    [Test]
    public async Task PreserveInitializers_True()
    {
        var xElement = XElement.Parse("<EmptyConstructor PreserveInitializers='True'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.PreserveInitializers).IsTrue();
    }

    [Test]
    public async Task ExcludeNamespacesAttribute()
    {
        var xElement = XElement.Parse(
            "<EmptyConstructor ExcludeNamespaces='Foo|Bar'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.ExcludeNamespaces[0]).IsEqualTo("Foo");
        await Assert.That(moduleWeaver.ExcludeNamespaces[1]).IsEqualTo("Bar");
    }

    [Test]
    public async Task ExcludeNamespacesCombined()
    {
        var xElement = XElement.Parse(
            """
            <EmptyConstructor  ExcludeNamespaces='Foo'>
                <ExcludeNamespaces>
            Bar
                </ExcludeNamespaces>
            </EmptyConstructor>
            """);
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.ExcludeNamespaces[0]).IsEqualTo("Foo");
        await Assert.That(moduleWeaver.ExcludeNamespaces[1]).IsEqualTo("Bar");
    }

    [Test]
    public async Task IncludeNamespacesNode()
    {
        var xElement = XElement.Parse(
            """
            <EmptyConstructor>
                <IncludeNamespaces>
            Foo
            Bar
            Foo.Bar
                </IncludeNamespaces>
            </EmptyConstructor>
            """);
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.IncludeNamespaces[0]).IsEqualTo("Foo");
        await Assert.That(moduleWeaver.IncludeNamespaces[1]).IsEqualTo("Bar");
        await Assert.That(moduleWeaver.IncludeNamespaces[2]).IsEqualTo("Foo.Bar");
    }

    [Test]
    public async Task IncludeNamespacesAttribute()
    {
        var xElement = XElement.Parse(
            "<EmptyConstructor IncludeNamespaces='Foo|Bar'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.IncludeNamespaces[0]).IsEqualTo("Foo");
        await Assert.That(moduleWeaver.IncludeNamespaces[1]).IsEqualTo("Bar");
    }

    [Test]
    public async Task IncludeAndExcludeNamespacesAttribute()
    {
        var xElement = XElement.Parse(
            "<EmptyConstructor IncludeNamespaces='Bar' ExcludeNamespaces='Foo'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        var exception = await Assert.That(() => moduleWeaver.ReadConfig()).Throws<WeavingException>();
        await Assert.That(exception!.Message).IsEqualTo("Either configure IncludeNamespaces OR ExcludeNamespaces, not both.");
    }

    [Test]
    public async Task IncludeNamespacesCombined()
    {
        var xElement = XElement.Parse(
            """
            <EmptyConstructor  IncludeNamespaces='Foo'>
                <IncludeNamespaces>
            Bar
                </IncludeNamespaces>
            </EmptyConstructor>
            """);
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        await Assert.That(moduleWeaver.IncludeNamespaces[0]).IsEqualTo("Foo");
        await Assert.That(moduleWeaver.IncludeNamespaces[1]).IsEqualTo("Bar");
    }
}