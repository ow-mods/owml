# Outer Wilds Mod Loader

OWML makes mod developent for Outer Wilds (hopefully!) much easier, and makes us able to use many mods simultaneously. Hopefully this will encourage many people to make mods for this amazing game.

## How it works

The launcher does this:
1. Copies some files from the game which is needed by OWML.
2. Shows the mods found in the Mods folder.
3. Patches the game to make the game call the mod loader. 
4. Copies OWML files to the game folder, used by the mod loader.
5. Starts the game.

The mod loader does this:
1. Gets all mods from the Mods folder.
2. Creates a mod helper with useful events, etc.
3. For each mod:
   1. Creates a new Unity game object.
   2. Adds the mod behaviour to the game object.
   3. Initializes the mod behaviour with the mod helper.
   
## Sample mods

Two mods are included as examples/inspiration:

|Sample mod|Description|
|----------|-----------|
|OWML.EnableDebugMode|This enables the built-in debug mode in the game. It allows you to do some fun stuff by pressing certain keys, such as exploding the sun with the End key, and cycling through various debug UIs with F1.|
|OWML.TestMod|This blows up the sun right away. Disabled by default (in manifest.json).|

## For players

1. Check that the path to the game is correct in OWML.Config.json.
2. Download mods and put them in the Mods folder. Make sure each mod has its own folder in Mods.
3. Run OWML.Launcher.exe **as administrator**. If you get tired of running as admin, give full control to all game files to all users.

## For modders

See the sample mods for examples.

Make a new project and reference the following files:
* OWML:
  * OWML.Common.dll
  * OWML.Events.dll
* {gamePath}\OuterWilds_Data\Managed:
  * Assembly-CSharp.dll
  * UnityEngine.CoreModule.dll

Inherit from ModBehaviour. This is a Unity MonoBehaviour, see Unity doc: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html

Your initial logic goes in Awake or Start (called by Unity). You'll have access to the mod helper at that time. Example:

~~~~
public class TestMod : ModBehaviour
{
    private void Start()
    {
        ModHelper.Console.WriteLine($"In {nameof(TestMod)}!");
    }
}
~~~~

The mod helper contains useful helper classes:

|Helper|What it does|
|------|------------|
|Logger|Logs to file: Logs\OWML.Log.txt|
|Console|Prints to the console (via Logs\OWML.Output.txt...)|
|Events|Allows listening to events, such as Awake and Start of MonoBehaviours. Uses HarmonyHelper.|
|HarmonyHelper|Helper methods for Harmony, such as extending a method with another, and changing or removing the contents of a method.|
|?|More to come!|

This will be called when the game starts (at the title menu), which might be too early for what you want to do. The mod helper contains events we can use to know when certain behaviours start. Here we add an event for when  Flashlight has loaded, which is after the player has "woken up":

~~~~
private void Start()
{
	ModHelper.Events.AddStartEvent<Flashlight>();
	ModHelper.Events.OnStart += OnStart;
}

private void OnStart(MonoBehaviour behaviour)
{
	ModHelper.Console.WriteLine("Behaviour name: " + behaviour.name);
	if (behaviour.GetType() == typeof(Flashlight))
	{
		ModHelper.Console.WriteLine("Flashlight has started!");
	}
}
~~~~

Add a manifest file called manifest.json. Example:

~~~~
{
  "filename": "OWML.EnableDebugMode.dll",
  "author": "Alek",
  "name": "EnableDebugMode",
  "uniqueName": "Alek.EnableDebugMode",
  "version": "0.1",
  "enabled": true
}
~~~~

## Configuration

OWML is configured by OWML.Config.json:

|Key|Description|
|---|-----------|
|gamePath|The path to the game. This must be correct for anything to work. Default: "C:/Program Files (x86)/Outer Wilds"|

Each mod is defined in a manifest.json file:

|Key|Description|
|---|-----------|
|filename|The filename of the DLL containing the ModBehaviour class.|
|author|The name of the author.|
|name|The name of the mod.|
|uniqueName|Usually {author}.{uniqueName}.|
|version|The version number.|
|enabled|Whether or not the mod will be loaded.|

## Compatibility

* Tested with Outer Wilds 1.0.0, 1.0.2 and 1.0.3.
* Currently Windows only.

## Credits

* Outer Wilds: http://www.outerwilds.com
* Outer Wilds on Discord: https://discord.gg/csKYR3w
* Outer Wilds on Reddit: https://www.reddit.com/r/outerwilds
* SMAPI, the main inspiration for this project: https://smapi.io

Dependencies:
* dnpatch for patching DLL files: https://github.com/ioncodes/dnpatch
  * uses dnlib: https://github.com/0xd4d/dnlib
* Harmony for patching DLLs in memory: https://github.com/pardeike/Harmony
* Newtonsoft.Json for Unity: https://github.com/SaladLab/Json.Net.Unity3D

## Feedback

* I'm Alek on the Outer Wilds Discord: https://discord.gg/csKYR3w
* Feature requests: make an issue on Github. I also welcome PRs!