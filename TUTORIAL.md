## 1. Overview

This tutorial covers most of the stuff you can do with Shibari framework, which includes implementing a model, binding it to views, and making data persistent.

We'll create a simplistic donut clicker game. Each time player clicks the donut, their highscore increases by one. The highscore is saved between sessions. Every ten clicks the donut changes its look. The player can change donut appearance manually by the dropdown menu. 

You know what's better than a donut clicker game? ~~An actual donut~~ Three donut clicker games! We'll do it in three different ways:

1. Using default BindableViews: It's a super simple method that's good for routine tasks.

2. Using custom BindableViews: Sometimes we have complex or weird views. You can extend BindableView the same way I did it for, say, SliderView. 

3. Using script which is not BindableView at all: Sometimes we need to access a model from our business logic. Sometimes we want to simply hardcode a reference to model's property. Sometimes we want a framework to be as non-invasive as possible. In such cases, you can call model properties directly.

## 2. Initial Setup

1.  Clone this repository into the Assets/Shibari subfolder of your project ~~or import Asset Store package (not yet published)~~.
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

## 3a. Using default BindableViews

### 3a.1. Preparing the model

Let's split our model in two different nodes: player node stores a score, and UI node has everything that views interact with.

Make two new classes:

```csharp
using Shibari;
using UnityEngine;

public class UiNode : Node
{
    //Marks property to be serialized to json.
    [SerializeValue]
    //It's good practice to store resource paths in separate json file.
    public AssignableValue<string> DonutSpritePath { get; } = new AssignableValue<string>();

    [SerializeValue]
    //The same applies to user-friendly text labels. 
    //If you need to support various languages, you can make separate localization node and feed different json files to it, depending on user's language.
    public AssignableValue<string> ScoreFormat { get; } = new AssignableValue<string>();

    //Marks property to be visible in the editor.
    [ShowInEditor]
    public CalculatedValue<string> ScoreLabel { get; }

    [ShowInEditor]
    public AssignableValue<int> CurrentDonut { get; } = new AssignableValue<int>();

    [SerializeValue, ShowInEditor]
    public AssignableValue<string[]> DonutTypes { get; } = new AssignableValue<string[]>();

    [ShowInEditor]
    public CalculatedValue<Sprite> CurrentDonutSprite { get; }

    public UiNode()
    {
        //Refer to "Grokking your model" chapter.
        CurrentDonutSprite = new CalculatedValue<Sprite>(() => Resources.Load<Sprite>(string.Format(DonutSpritePath, CurrentDonut)), DonutSpritePath, CurrentDonut);
        ScoreLabel = new CalculatedValue<string>(() => string.Format(ScoreFormat, RootNode.Instance.PlayerNode.Score), ScoreFormat, RootNode.Instance.PlayerNode.Score);
    }

    //Mark method to be visible in editor.
    [ShowInEditor]
    public void OnDonutClicked()
    {
        //BindableValue<T> has implicit conversion to type T, but doing the opposite does not seem possible. 
        //In a satisfactory way, at least.
        RootNode.Instance.PlayerNode.Score.Set(RootNode.Instance.PlayerNode.Score + 1);
    }

    //Assign new random donut if score is divisible by ten.
    public void OnPlayerScoreChanged()
    {
        if (RootNode.Instance.PlayerNode.Score % 10 == 0)
        {
            int nextDonut = Random.Range(0, DonutTypes.Get().Length);
            CurrentDonut.Set(nextDonut);
        }
    }

    public override void Initialize()
    {
        RootNode.Instance.PlayerNode.Score.OnValueChanged += OnPlayerScoreChanged;
        base.Initialize();
    }
}
```

```csharp
using Shibari;
using UnityEngine;

public class PlayerNode : Node
{
    [SerializeValue]
    public AssignableValue<int> Score { get; } = new AssignableValue<int>();

    //Save player node to PlayerPrefs.
    public void Save()
    {
        PlayerPrefs.SetString("donut_score", Serialize());
        //PlayerPrefs.Save() is pretty heavy. Usually, you don't want to call it every time something is changed.
        PlayerPrefs.Save();
    }

    //Load player node from PlayerPrefs.
    public void Load()
    {
        if (PlayerPrefs.HasKey("donut_score"))
            Deserialize(PlayerPrefs.GetString("donut_score"));
    }

    public override void Initialize()
    {
        //Save player node each time score has changed.
        Score.OnValueChanged += Save;
        base.Initialize();
    }
}
```

Finally, modify RootNode class:

```csharp
using Shibari;
using UnityEngine;

public class RootNode : Node
{

    //Marks node's contents visible in the editor. By default, node is hidden even if it has properties marked with ShowInEditor attribute. It's made this way for consistency reasons.
    [ShowInEditor]
    public UiNode UiNode { get; private set; }

    //We don't need player node to be visible in editor. 
    public PlayerNode PlayerNode { get; private set; }

    //Simple property to encapsulate explicit convertion from Node to RootNode.
    public static RootNode Instance { get { return (RootNode)Model.RootNode; } }

    public override void Initialize()
    {
        //We can't use auto-implemented properties in our RootData or instantiate nodes in RootNode constructor. 
        //If we do so, instance of RootNode would not be yet assigned to a Model.RootNode at the moment of initialization of our UiData.
        //Refer to "Grokking your model" chapter for further read.
        PlayerNode = new PlayerNode();
        UiNode = new UiNode();

        base.Initialize();

        //Model is ready to use.
        
        //Load UI node values from json file.
        UiNode.Deserialize(Resources.Load<TextAsset>("UiNode").text);
        
        PlayerNode.Load();
    }
}
```

### 3a.2. Prepare resources

Create new text file in ``Assets/Resources`` folder and name it "UiNode.json":

```json
{
  "DonutSpritePath": "Donuts/{0}",
  "ScoreFormat": "Score: {0}",
  "DonutTypes": ["No topping", "Chocolate topping", "Stuffed donut"]
}
```

Create three programmer art donut sprites in ``Assets/Resources/Donut`` folder. Or skip this step, if you are fine with white square donuts.

### 3a.3. Prepare scene

* Create new Unity scene.
* Add Canvas to it (right click inside Hierarchy view, then pick UI/Canvas option).
* Add UI/Text to your canvas, then add TextView component to it. Pick ``UiNode/ScoreLabel`` in it's inspector.
* Make an UI/Button and add ButtonView component. Pick ``UiNode/OnDonutClicked()``. Then add ImageView and pick ``UiNode/CurrentDonutSprite``.
* Create UI/Dropdown and add DropdownView component to it. Pick ``UiNode/CurrentDonut`` and ``UiNode/DonutTypes``.

### 3a.4. Enjoy

Our little game is ready to be played.

## 3b. 

## 3c.

## 4. Grokking your model

Your model is basically an object of the type you specify in ``Settings/Shibari`` menu. It extends Shibari.Node class and consists of bindable properties, handler methods and child nodes with similar structure. You can think about model as a [directed rooted tree](https://en.wikipedia.org/wiki/Tree_(graph_theory)#Rooted_tree).

Model initialization happens [right before](https://docs.unity3d.com/ScriptReference/RuntimeInitializeLoadType.BeforeSceneLoad.html) your first scene loads. It consists of following steps:

1. Root node instance is created.
2. Root node instance is assigned to Shibari.Model.RootNode property.
3. Shibari.Model.RootNode.Initialize() is called.
4. Shibari.Node.Initialize() method caches references to it's bindable properties, handler methods, and child nodes. Then it recursively calls Initialize() method on all Node properties.
5. Model is ready to use now.

At the moment of BindableData.Initialize() execution, all of object's BindableValue and BindableData properties have to be assigned. You can do it with auto-implemented properties, in a constructor, or in overriden Initialize() method, before base method is called.

Most likely, you will use CalculatedValue properties. They could refer to other properties of the same or another object of your model. These properties should be assigned prior to CalculatedValue's constructor invocation. 

There are several things to consider:

1. You can't reference object's member from [auto-implemented property](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties). This means that that you cannot auto-implement CalculatedValue if it references BindableValue of the same node. Assign it in object's constructor instead.
2. You have to create your node objects in a strict order, from referenced object to referencing one. The messier your model interactions are, the harder it will be to figure out the right order. And it could be harder to implement if your model hierarchy is too deep.
3. Avoid recursive hierarchy *(type A has property of type B which has property of type A...)* since there is no way to stop a recursion.
4. Assign all of RootNode object's properties in it's Initialize() method, before base.Initialize() call. It guarantees that Model.RootNode is assigned and can be used by CalculatedValue constructors.