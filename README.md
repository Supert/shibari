# Shibari
Unity3d data binding framework with strong typing support.

# Minimal setup
1. Set Player Settings/Api Compatibility Level to Experimental (.NET 4.6 Equivalent).
2. Set dependencies:
    1. [ClassTypeReference](https://github.com/rotorz/unity3d-class-type-reference)
    2. [JsonNET](https://www.newtonsoft.com/json)
3. Create ShibariRootNode.cs with following contents:
```csharp
using Shibari;

//You can name it to your liking.
public class ShibariRootNode : BindableData
{
    
}
```
4. Pick ShibariRootNode as a root node in Settings/Shibari menu.
