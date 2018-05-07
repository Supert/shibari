## Shibari

Shibari is Unity3d data binding framework with strong typing support.

![Editor view](screenshot_editor.png?raw=true "Editor")

![Source code sample](screenshot_source.png?raw=true "Source")

## Minimal setup

1.  Clone this repository into the ``Assets/Shibari`` subfolder of your project [or import Asset Store package](https://assetstore.unity.com/packages/templates/systems/shibari-114989).
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

## Examples

You can find a showcase project for my framework [here](https://github.com/Supert/village-keeper). It shows everything my framework is capable of and is close enough to a project you could make in real world of game development.

## Tutorial

[Tutorial](TUTORIAL.md) is available. It's the quickest way to get used to Shibari framework.  

## Default UI Classes

### BindableHandlerView : MonoBehaviour

Base abstract class for View-to-Model connection. It binds to a method specified in Unity Editor and calls it when specified conditions are met (for example, ButtonView calls it's method when button is clicked).

#### ButtonView

BindableHandlerView which calls it's method when button is clicked.

### BindableView : MonoBehaviour

Base abstract class for Model-to-View and both-ways connection.

#### DropdownView

Populates UnityEngine.UI.Dropdown view with contents of the second BoundValue and binds index of selected item to the first one.

#### EnabledView

Sets GameObject active or inactive depending on it's boolean BoundValue.

#### FillerBindableView

Sets Image.fillAmount to it's float BoundValue.

#### ImageBindableView

Sets Image.Sprite to it's Sprite BoundValue.

#### InputView

Provides two-way connection between InputField and string BoundValue.

#### InteractableView

Sets Selectable.interactable to boolean BoundValue.

#### SelectableSpritesView

Sets Selectable sprites to SelectableSprites BoundValue.

#### SliderView

Provides two-way connection between Slider and float BoundValue.

#### TextBindableView

Sets Text.text to string BoundValue.

#### ToggleView

Provides two-way connection between Toggle.isOn and boolean BoundValue.
