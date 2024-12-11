---
Title: Prepatchers
Sort_Priority: 33
---

# Prepatchers

In certain contexts you may need to edit game files before start. This is where
prepatchers come in. Prepatchers are run by OWML directly before the game starts, allowing you to modify game files you would otherwise not be able to.

To create a prepatcher you'll need a separate project from your mod. This can be done by creating a new project in your solution with the executable type, it should automatically build to your mod folder.

Now in your mod manifest you need to set the `patcher` field to the path of the executable relative to the root of the
mod folder.

## Creating A Prepatcher

A prepatcher is a simple console app that OWML executes, it's only passed the location of your mod folder.
However, it is possible to get the game's location by doing `AppDomain.CurrentDomain.BaseDirectory`:

```csharp
public static void Main(string[] args)
{
    var modPath = args.Length > 0 ? args[0] : ".";
    var gamePath = AppDomain.CurrentDomain.BaseDirectory;

    Console.WriteLine($"Mod Path: {modPath}");
    Console.WriteLine($"Game Path: {gamePath}");

    // DoStuff(modPath, gamePath);
}
```

Keep in mind the `ModHelper` is not available in a prepatcher.
You'll need to have the prepatcher include libraries like Newtonsoft.Json to read and write JSON files.

As an example, Quantum Space Buddies [utilizes a pre-patcher](https://github.com/qsb-dev/quantum-space-buddies/tree/master/QSBPatcher). You can use this as an example. Notice how it also needs to [set this as `patcher` in its manifest](https://github.com/qsb-dev/quantum-space-buddies/blob/b2e55d61e97b2e9c05b8b6e69cb349c57b59aa93/QSB/manifest.json#L18) in order for OWML to know to run it.

### Logging

Due to a lack of `ModHelper`, you'll need to use `Console.WriteLine` to log information.
This **will not output to the manager window**. To test prepatchers, we recommend you launch `OWML.Launcher.exe` in a
terminal directly to properly see stdout.

If a prepatcher errors it *should usually* be outputted to the manage window as OWML is setup to catch and
log any exceptions thrown by the prepatcher.

### Warnings

Due to the nature of prepatchers, the manager cannot undo changes made by them. This means the game will continue to be modified even if the
mod is uninstalled or disabled.

The manager will try it's best to warn the user of this. If your mod has prepatcher and
is disabled or uninstalled the manager will show a dialog explaining that
your mod has modified game files in an irreversible way and encourages them to validate the game files.
