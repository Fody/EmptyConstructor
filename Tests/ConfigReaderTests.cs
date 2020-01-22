using System.Xml.Linq;
using Fody;
using Mono.Cecil;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class ConfigReaderTests :
    VerifyBase
{
    [Fact]
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
        Assert.Equal("Foo", moduleWeaver.ExcludeNamespaces[0]);
        Assert.Equal("Bar", moduleWeaver.ExcludeNamespaces[1]);
        Assert.Equal("Foo.Bar", moduleWeaver.ExcludeNamespaces[2]);
    }

    [Fact]
    public void VisibilityFamily()
    {
        var xElement = XElement.Parse("<EmptyConstructor Visibility='family'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.Equal(MethodAttributes.Family, moduleWeaver.Visibility);
    }

    [Fact]
    public void VisibilityDefault()
    {
        var xElement = XElement.Parse("<EmptyConstructor/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.Equal(MethodAttributes.Public, moduleWeaver.Visibility);
    }

    [Fact]
    public void VisibilityPublic()
    {
        var xElement = XElement.Parse("<EmptyConstructor Visibility='public'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.Equal(MethodAttributes.Public, moduleWeaver.Visibility);
    }

    [Fact]
    public void MakeExistingEmptyConstructorsVisible_Default()
    {
        var xElement = XElement.Parse("<EmptyConstructor/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.False(moduleWeaver.MakeExistingEmptyConstructorsVisible);
    }

    [Fact]
    public void MakeExistingEmptyConstructorsVisible_False()
    {
        var xElement = XElement.Parse("<EmptyConstructor MakeExistingEmptyConstructorsVisible='False'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.False(moduleWeaver.MakeExistingEmptyConstructorsVisible);
    }

    [Fact]
    public void MakeExistingEmptyConstructorsVisible_True()
    {
        var xElement = XElement.Parse("<EmptyConstructor MakeExistingEmptyConstructorsVisible='True'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.True(moduleWeaver.MakeExistingEmptyConstructorsVisible);
    }

    [Fact]
    public void PreserveInitializers_Default()
    {
        var xElement = XElement.Parse("<EmptyConstructor/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.True(moduleWeaver.PreserveInitializers);
    }

    [Fact]
    public void PreserveInitializers_False()
    {
        var xElement = XElement.Parse("<EmptyConstructor PreserveInitializers='False'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.False(moduleWeaver.PreserveInitializers);
    }

    [Fact]
    public void PreserveInitializers_True()
    {
        var xElement = XElement.Parse("<EmptyConstructor PreserveInitializers='True'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.True(moduleWeaver.PreserveInitializers);
    }

    [Fact]
    public void ExcludeNamespacesAttribute()
    {
        var xElement = XElement.Parse(@"
<EmptyConstructor ExcludeNamespaces='Foo|Bar'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.Equal("Foo", moduleWeaver.ExcludeNamespaces[0]);
        Assert.Equal("Bar", moduleWeaver.ExcludeNamespaces[1]);
    }

    [Fact]
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
        Assert.Equal("Foo", moduleWeaver.ExcludeNamespaces[0]);
        Assert.Equal("Bar", moduleWeaver.ExcludeNamespaces[1]);
    }

    [Fact]
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
        Assert.Equal("Foo", moduleWeaver.IncludeNamespaces[0]);
        Assert.Equal("Bar", moduleWeaver.IncludeNamespaces[1]);
        Assert.Equal("Foo.Bar", moduleWeaver.IncludeNamespaces[2]);
    }

    [Fact]
    public void IncludeNamespacesAttribute()
    {
        var xElement = XElement.Parse(@"
<EmptyConstructor IncludeNamespaces='Foo|Bar'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        moduleWeaver.ReadConfig();
        Assert.Equal("Foo", moduleWeaver.IncludeNamespaces[0]);
        Assert.Equal("Bar", moduleWeaver.IncludeNamespaces[1]);
    }

    [Fact]
    public void IncludeAndExcludeNamespacesAttribute()
    {
        var xElement = XElement.Parse(@"
<EmptyConstructor IncludeNamespaces='Bar' ExcludeNamespaces='Foo'/>");
        var moduleWeaver = new ModuleWeaver { Config = xElement };
        var exception = Assert.Throws<WeavingException>(() => moduleWeaver.ReadConfig());
        Assert.Equal("Either configure IncludeNamespaces OR ExcludeNamespaces, not both.",exception.Message);
    }

    [Fact]
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
        Assert.Equal("Foo", moduleWeaver.IncludeNamespaces[0]);
        Assert.Equal("Bar", moduleWeaver.IncludeNamespaces[1]);
    }

    public ConfigReaderTests(ITestOutputHelper output) :
        base(output)
    {
    }
}