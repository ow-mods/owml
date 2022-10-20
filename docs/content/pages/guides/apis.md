---
Title: Creating APIs
Sort_Priority: 35
---

# Creating APIs

To allow for easy interoperability between mods, OWML provides an API system where mods can provide and consume APIs from eachother easily.

## Creating an API

To create an API start by making an interface with all the methods your api will have. 

```csharp
public interface IMyCoolApi {
    public string Echo(string input);
}
```

Now create a class that implements the API

```csharp
public class MyCoolApi : IMyCoolApi {
    public string Echo(string input) => input;
}
```

Finally, override the `GetApi` method and have it return an instance of your API

```csharp
public class MyCoolMod : ModBehaviour {
    public override object GetApi() {
        return new MyCoolApi();
    }
}
```

Your mod now provides the API to consumers!

## Consuming APIs

First, define the interface for the API, usually the mod will have it available somewhere to copy and paste.

```csharp
public interface IMyCoolApi {
    public string Echo(string input);
}
```

Now, use `ModHelper.Interaction.TryGetModApi` to obtain the API for the mod.

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        var myApi = ModHelper.TryGetModApi<IMyCoolApi>("Bwc9876.MyCoolMod");
        ModHelper.Console.WriteLine(myApi.Echo("Hello, World!"));
    }
}
```

