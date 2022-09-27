---
Title: Interaction
---

# ModHelper.Interaction

Provides methods to check for and interact with other mods.

## GetMods

Gets a list of all other mods that are installed and enabled.

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        var modNames = String.Join(", ", ModHelper.Interactions.GetMods().Select(m => m.ModHelper.Manifest.Name));
        ModHelper.Console.WriteLine($"Installed Mods: {modNames}.", MessageType.Info);
    }
}
```

## GetDependants

Gets a list of all mods that are dependent on the mod with the given uniqueName

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        var modNames = String.Join(", ", ModHelper.Interactions.GetDependants("Bwc9876.MyCoolMod").Select(m => m.ModHelper.Manifest.Name));
        ModHelper.Console.WriteLine($"My Dependants Are: {modNames}.", MessageType.Info);
    }
}
```

## GetDependencies

Gets a list of all mods that the mod with the given uniqueName depends upon.

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        var modNames = String.Join(", ", ModHelper.Interactions.GetDependencies("Bwc9876.MyCoolMod").Select(m => m.ModHelper.Manifest.Name));
        ModHelper.Console.WriteLine($"My Dependencies Are: {modNames}.", MessageType.Info);
    }
}
```

## TryGetMod

Attempts to get the `IModBehaviour` of the mod with the given unique name, returns `null` if the mod couldn't be found.

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        var newHorizons = ModHelper.Interaction.TryGetMod("xen.NewHorizons");
        ModHelper.Console.WriteLine($"New Horizons' version is: {newHorizons?.ModHelper?.Manifest?.Version ?? "NOT INSTALLED"}!");
    }
}
```

## TryGetModApi&lt;T&gt;

Attempts to get the API for the mod with the given unique name. You must define the mod's API as an interface and pass that in as the `T`. Returns `null` if the API couldn't be loaded.

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        // INewHorizons is assumed to be defined somewhere else
        var newHorizonsApi = ModHelper.Interaction.TryGetModApi<INewHorizons>();
        if (newHorizonsApi != null) {
            newHorizonsApi.LoadConfigs(this);
        }
    }
}
```

## ModExists

Checks if there exists a mod with the given uniqueName is installed and enabled.

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        var timeSaverOn = ModHelper.Interaction.ModExists("Bwc9876.TimeSaver");
        ModHelper.Console.WriteLine(timeSaverOn ? "Thank you for using TimeSaver ::)" : "No TimeSaver ::(", timeSaverOn? MessageType.Success : MessageType.Fatal);
    }
}
```
