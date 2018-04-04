## 1. Overview

This tutorial covers most of the stuff you can do with Shibari framework, which includes implementing a model, binding it to views, and making data persistent.

We'll create simplistic donut clicker game. Each time player clicks the donut, his highscore increases by one. Highscore is saved between sessions. Every ten clicks, donut changes it's look. Player can change donut appearence manually by the dropdown menu. 

You know what's better than donut clicker game? ~~An actual donut~~ Three donut clicker games! We'll do it in three different ways:

1. Using default BindableViews: It's super simple method that's good for routine tasks.

2. Using custom BindableViews: Sometimes we have complex or weird views. You can extend BindableView the same way I did it for, say, SliderView. 

3. Using script which is not BindableView at all: Sometimes we need to access model from our business logic. Sometimes we want to simply hardcode reference to model's property. Sometimes we want a framework to be as non-invasive as possible. In such cases, you can call model properties directly.

## 2. Initial Setup

1.  Clone this repository into Assets/Shibari subfolder of your project ~~or import Asset Store package (not yet published)~~.
2.  Set ``Player Settings/Api Compatibility Level`` to ``Experimental (.NET 4.6 Equivalent)``.
3.  Add required dependencies to your project:
    * [JsonNET](https://www.newtonsoft.com/json), or [package from Wanzyee studio](https://assetstore.unity.com/packages/tools/input-management/json-net-converters-simple-compatible-solution-58621), or [package from ParentElement, LLC](https://assetstore.unity.com/packages/tools/input-management/json-net-for-unity-11347)
4.  Create a new class inherited from Shibari.Node:
```csharp
using Shibari;

public class RootNode : Node
{
    
}
```
5.  Pick it as a root node in ``Settings/Shibari`` menu.

## 3a. 

## 3b. 

## 3c.

## 4. Grokking your model

Your model is basically an object of type you specify in ``Settings/Shibari`` menu. It extends Shibari.Node class and consist of bindable properties, handler methods and child nodes with similar structure. You can think about model as a [directed rooted tree](https://en.wikipedia.org/wiki/Tree_(graph_theory)#Rooted_tree).

Model initialization happens [right before](https://docs.unity3d.com/ScriptReference/RuntimeInitializeLoadType.BeforeSceneLoad.html) your first scene loads. It consists of following steps:

1. Root node instance is created.
2. Root node instance is assigned to Shibari.Model.RootNode property.
3. Shibari.Model.RootNode.Initialize() is called.
4. Shibari.Node.Initialize() method caches references to it's bindable properties, handler methods, and child nodes. Then it recursively calls Initialize() method on all Node properties.
5. Model is ready to use now.

At the moment of BindableData.Initialize() execution, all of object's BindableValue and BindableData properties have to be assigned. You can do it with auto-implemented properties, in a constructor, or in overriden Initialize() method, before base method is called.

Most likely, you will use CalculatedValue properties. They could refer to other properties of the same or another object of your model. These properties should be assigned prior to CalculatedValue's constructor invocation. 

There are several things to consider:

1. You can't reference object's member from [auto-implemented property](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties). It means that you cannot auto-implement CalculatedValue if it references BindableValue of the same node. Assign it in object's constructor instead.
2. You have to create your node objects in a strict order, from referenced object to referencing one. The messier your model interactions are, the harder it will be to figure out the right order. And it could be harder to implement if your model hierarchy is too deep.
3. Avoid recursive hierarchy *(type A has property of type B which has property of type A...)* since there is no way to stop a recursion.
4. Assign all of RootNode object's properties in it's Initialize() method, before base.Initialize() call. It guarantees that Model.RootNode is assigned and can be used by CalculatedValue constructors.