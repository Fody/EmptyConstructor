using System.Xml.Linq;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class ConfigReaderTests
{
    [Test]
    public void ExcludeNamespacesNode()
    {
        var xElement = XElement.Parse(@"
<EmptyConstructor>
    <ExcludeNamespaces>
Foo
Bar
Foo.Bar
    </ExcludeNamespaces>
</EmptyConstructor>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.AreEqual("Foo", moduleWeaver.ExcludeNamespaces[0]);
        Assert.AreEqual("Bar", moduleWeaver.ExcludeNamespaces[1]);
        Assert.AreEqual("Foo.Bar", moduleWeaver.ExcludeNamespaces[2]);
    }

    [Test]
    public void VisibilityFamily()
    {
        var xElement = XElement.Parse("<EmptyConstructor Visibility='family'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.AreEqual(MethodAttributes.Family, moduleWeaver.Visibility);
    }

    [Test]
    public void VisibilityDefault()
    {
        var xElement = XElement.Parse("<EmptyConstructor/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.AreEqual(MethodAttributes.Public, moduleWeaver.Visibility);
    }

    [Test]
    public void VisibilityPublic()
    {
        var xElement = XElement.Parse("<EmptyConstructor Visibility='public'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.AreEqual(MethodAttributes.Public, moduleWeaver.Visibility);
    }

    [Test]
    public void MakeExistingEmptyConstructorsVisible_Default()
    {
        var xElement = XElement.Parse("<EmptyConstructor/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.IsFalse(moduleWeaver.MakeExistingEmptyConstructorsVisible);
    }

    [Test]
    public void MakeExistingEmptyConstructorsVisible_False()
    {
        var xElement = XElement.Parse("<EmptyConstructor MakeExistingEmptyConstructorsVisible='False'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.IsFalse(moduleWeaver.MakeExistingEmptyConstructorsVisible);
    }

    [Test]
    public void MakeExistingEmptyConstructorsVisible_True()
    {
        var xElement = XElement.Parse("<EmptyConstructor MakeExistingEmptyConstructorsVisible='True'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.IsTrue(moduleWeaver.MakeExistingEmptyConstructorsVisible);
    }

    [Test]
    public void ExcludeNamespacesAttribute()
    {
        var xElement = XElement.Parse(@"
<EmptyConstructor ExcludeNamespaces='Foo|Bar'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.AreEqual("Foo", moduleWeaver.ExcludeNamespaces[0]);
        Assert.AreEqual("Bar", moduleWeaver.ExcludeNamespaces[1]);
    }

    [Test]
    public void ExcludeNamespacesCombined()
    {
        var xElement = XElement.Parse(@"
<EmptyConstructor  ExcludeNamespaces='Foo'>
    <ExcludeNamespaces>
Bar
    </ExcludeNamespaces>
</EmptyConstructor>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.AreEqual("Foo", moduleWeaver.ExcludeNamespaces[0]);
        Assert.AreEqual("Bar", moduleWeaver.ExcludeNamespaces[1]);
    }

    [Test]
    public void IncludeNamespacesNode()
    {
        var xElement = XElement.Parse(@"
<EmptyConstructor>
    <IncludeNamespaces>
Foo
Bar
Foo.Bar
    </IncludeNamespaces>
</EmptyConstructor>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.AreEqual("Foo", moduleWeaver.IncludeNamespaces[0]);
        Assert.AreEqual("Bar", moduleWeaver.IncludeNamespaces[1]);
        Assert.AreEqual("Foo.Bar", moduleWeaver.IncludeNamespaces[2]);
    }

    [Test]
    public void IncludeNamespacesAttribute()
    {
        var xElement = XElement.Parse(@"
<EmptyConstructor IncludeNamespaces='Foo|Bar'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.AreEqual("Foo", moduleWeaver.IncludeNamespaces[0]);
        Assert.AreEqual("Bar", moduleWeaver.IncludeNamespaces[1]);
    }

    [Test]
    public void IncludeAndExcludeNamespacesAttribute()
    {
        var xElement = XElement.Parse(@"
<EmptyConstructor IncludeNamespaces='Bar' ExcludeNamespaces='Foo'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        var exception = Assert.Throws<WeavingException>(() => moduleWeaver.ReadConfig());
        Assert.AreEqual("Either configure IncludeNamespaces OR ExcludeNamespaces, not both.",exception.Message);
    }

    [Test]
    public void IncludeNamespacesCombined()
    {
        var xElement = XElement.Parse(@"
<EmptyConstructor  IncludeNamespaces='Foo'>
    <IncludeNamespaces>
Bar
    </IncludeNamespaces>
</EmptyConstructor>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.AreEqual("Foo", moduleWeaver.IncludeNamespaces[0]);
        Assert.AreEqual("Bar", moduleWeaver.IncludeNamespaces[1]);
    }
}