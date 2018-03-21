# Shibari
Unity3d data binding framework with strong typing support.

![Editor view](screenshot_editor.png?raw=true "Editor")

![Source code sample](screenshot_source.png?raw=true "Source")

# Minimal setup
1. Set ``Player Settings/Api Compatibility Level`` to ``Experimental (.NET 4.6 Equivalent)``.
2. Add required dependencies to your project:
    1. [ClassTypeReference](https://github.com/rotorz/unity3d-class-type-reference)
    2. [JsonNET](https://www.newtonsoft.com/json)
3. Create a new class inherited from Shibari.BindableData:
```csharp
using Shibari;

//You can name it to your liking.
public class ShibariRootNode : BindableData
{
    
}
```
4. Pick it as a root node in ``Settings/Shibari`` menu.

#Examples
You can find a showcase project for my framework [here](https://github.com/Supert/village-keeper). It shows everything my framework is capable of and is close enough to a project you could make in real world of game development.

# Attributes

## SerializeValue

Marks AssignableValue property as serializable to and from json. Supports classes inherited from AssignableValue.

## ShowInEditor

Marks BindableData and BindableValue properties to be shown in the editor of BindableView. Also marks methods to be shown in the editor if BindableHandlerView.

### Supported method parameters

1. ()
2. (BindableHandlerView)
3. (BindableHandlerView, string)

# Classes

## Model

Static class. Initializes RootNode of type specified in Shibari/Settings menu.

### static BindableData RootNode

Returns RootNode of your model. Due to technical limitations it returns your root node as an object of base type BindableData. For your convenience, I recommend you to encapsulate it somewhere in your project. For example:

```csharp
public static YourBindableData RootNode
{
    get
    {
        return (YourBindableData) Model.RootNode;
    }
}
```

## BindableData

BindableData is a base class for your model nodes. Add another BindableData properties to that class to create tree structure of your model. Add BindableValue properties to store and use values. Mark your BindableData properties with ShowInEditor attribute to it contents visible in editor.

## BindableValue``<TValue>``

Abstract class of a value stored in a BindableData.

### public TValue Get()

Returns stored value.

### public event Action OnValueChanged

Event that rises when value has changed.

## AssignableValue``<TValue>``

BindableValue``<TValue>`` which allows value to be assigned.

### public void Set(TValue value)

Sets value of AssignableValue``<TValue>``.

## CalculatedValue``<TValue>``.

BindableValue``<TValue>`` that subscribes to other BindableValues. When one of these values changes, stored value changes, too.

### public CalculatedValue(Func``<TValue>`` calculateValueFunction, IEnumerable``<IBindable>`` subscribeTo)
Constructor. Binds to values specified in subscribeTo parameter. When one of these values changes, CalculatedValue sets value to a result of calculateValueFunction.

# UI Classes

## BindableHandlerView : MonoBehaviour

Base abstract class for View-to-Model connection. It binds to a method specified in Unity Editor and calls it when specified conditions are met (for example, ButtonView calls it's method when button is clicked).

### protected virtual void Invoke()
Call it to execute a binded method.

## ButtonView : BindableHandlerView

BindableHandlerView which calls it's method when button is clicked.

## BindableView : MonoBehaviour

Base abstract class for Model-to-View and both-ways connection.

### public abstract BindableValueRestraint[] BindableValueRestraints { get; }

Implement to specify number of binded values, type of their contents and if they should be assignable or not.

### protected BindableValueInfo[] BindedValues { get; }

Return list of BindableValues binded to view. Specify them in view's editor.

### protected abstract void OnValueChanged()

This method is called when one of the binded values has changed.

## DropdownView : BindableView

Populates UnityEngine.UI.Dropdown view with contents of the second BindedValue and binds index of selected item to the first one.

## EnabledView : BindableView

Sets GameObject active or inactive depending on it's boolean BindedValue.

## FillerBindableView : BindableView

Sets Image.fillAmount to it's float BindedValue.

## ImageBindableView : BindableView

Sets Image.Sprite to it's Sprite BindedValue.

## InputView : BindableView

Provides two-way connection between InputField and string BindedValue.

## InteractableView : BindableView

Sets Selectable.interactable to boolean BindedValue.

## SelectableSpritesView : BindableView

Sets Selectable sprites to SelectableSprites BindedValue.

## SliderView

Provides two-way connection between Slider and float BindedValue.

## TextBindableView

Sets Text.text to string BindedValue.

## ToggleView

Provides two-way connection between Toggle.isOn and boolean BindedValue.
