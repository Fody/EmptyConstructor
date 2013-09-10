![Icon](https://raw.github.com/Fody/EmptyConstructor/master/Icons/package_icon.png)

## This is an add-in for [Fody](https://github.com/Fody/Fody/) 

Adds an empty constructor to classes even if you have a non-empty one defined.

[Introduction to Fody](http://github.com/Fody/Fody/wiki/SampleUsage)

## Nuget

Nuget package http://nuget.org/packages/EmptyConstructor.Fody 

To Install from the Nuget Package Manager Console 
    
    PM> Install-Package EmptyConstructor.Fody


# Configuration Options

## Exclude types with an Attribute

If for some reason you want to skip a specific class you can mark it with a `DoNotVirtualizeAttribute`. 

Since no reference assembly is shipped with Virtuosity. Just add the below class to your assembly. Namespace does not matter.

    public class DoNotVirtualizeAttribute : Attribute
    {
    }

So your class will look like this

    [DoNotVirtualize]
    public class ClassToSkip
    {
        ...
    }

## Include or exclude namespaces
 
These config options are access by modifying the `EmptyConstructor` node in FodyWeavers.xml 
 
### ExcludeNamespaces

A list of namespaces to exclude.

Can not be defined with `IncludeNamespaces`.

Can take two forms. 

As an element with items delimited by a newline.

    <EmptyConstructor>
        <ExcludeNamespaces>
            Foo
            Bar
        </ExcludeNamespaces>
    </EmptyConstructor>
    
Or as a attribute with items delimited by a pipe `|`.

    <EmptyConstructor ExcludeNamespaces='Foo|Bar'/>
    
        
## IncludeNamespaces

A list of namespaces to include.

Can not be defined with `ExcludeNamespaces`.

Can take two forms. 

As an element with items delimited by a newline.

    <EmptyConstructor>
        <IncludeNamespaces>
            Foo
            Bar
        </IncludeNamespaces>
    </EmptyConstructor>
    
Or as a attribute with items delimited by a pipe `|`.

    <EmptyConstructor IncludeNamespaces='Foo|Bar'/>

## Icon

Icon courtesy of [The Noun Project](http://thenounproject.com)



