---
title: Mod APIs
description: Expose your mods behavior for use by other mods
---

To allow for easy interoperability between mods, OWML provides an API system where mods can provide and consume APIs from eachother easily.

## Creating an API

To create an API start by making an interface with all the methods your API will have. 

```csharp title="IMyCoolApi.cs"
public interface IMyCoolApi {
    public string Echo(string input);
}
```

Now create a class that implements the API

```csharp title="MyCoolApi.cs"
public class MyCoolApi : IMyCoolApi {
    public string Echo(string input) => input;
}
```

Finally, override the `GetApi` method and have it return an instance of your API

```csharp title="MyCoolMod.cs"
public class MyCoolMod : ModBehaviour {
    public override object GetApi() {
        return new MyCoolApi();
    }
}
```

Your mod now provides the API to consumers!

## Consuming APIs

First, define the interface for the API, usually the mod will have it available somewhere to copy and paste.

```csharp title="IAnotherModApi.cs"
public interface IAnotherModApi {
    public string Echo(string input);
}
```

Now, use `ModHelper.Interaction.TryGetModApi` to obtain the API for the mod. For example if I wanted to get the API for the mod `Bwc9876.MyCoolMod`, I would make the following call to `TryGetModApi`.

```csharp title="MyCoolMod.cs" "Bwc9876.MyCoolMod" "IAnotherModApi"
public class MyCoolMod : ModBehaviour {
    public void Start() {
        var myApi = ModHelper.Interaction.TryGetModApi<IAnotherModApi>("Bwc9876.MyCoolMod");
        ModHelper.Console.WriteLine(myApi.Echo("Hello, World!"));
    }
}
```


