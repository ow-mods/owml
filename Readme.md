# Outer Wilds Mod Loader (OWML)

## Install:

1. Check that the path to the game is correct in OWML.Config.json. The default is "C:/Program Files (x86)/Outer Wilds".
2. Run OWML.Launcher.exe.

## How it works

The launcher does this:
1. Copy some files from the game which is needed by OWML:
   * Assembly-CSharp.dll
   * UnityEngine.CoreModule.dll
2. Patch the game file \Managed\Assembly-CSharp.dll usind dnpatch to make the game call the mod loader when it starts. 
3. Copy OWML files to the game folder, used by the mod loader.
3. Start the game.

The mod loader does this:
1. Get all mods from the Mods folder
2. Create a mod helper object with useful events, etc.
3. For each mod found in Mods:
   1. Create a new Unity game object
   2. Add the mod behaviour to the game object.
   3. Initialize the mod bevaiour with the mod helper.
   
## For players

Download mods and put them in the Mods folder.
Make sure each mod has its own folder in Mods.

## For modders

See sample mods for examples.

Make a new project with a class inheriting from ModBehaviour. This is a Unity monobehaviour, see Unity doc: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html

Your initial logic goes in Awake or Start (called by Unity). You'll have access to the mod helper at that time. Example:

~~~~
private void Start()
{
    ModHelper.Console.WriteLine("In some mod!");
}
~~~~

This will be called when the game starts (at the title menu) which might be too early for what you want to do. The mod helper contains events we can use to know when the game has properly loaded. Here we add an event for when the Flashlight class has loaded: 

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

Mod helper:

|Thing|What it does|
|-----|------------|
|Logger|Logs to file|
|Console|Prints to the console|
|Events|Allows listening to events, such as Awake or Start of monobehaviours. Uses HarmonyHelper.
|HarmonyHelper|Helper methods for Harmony, such as extending a method with another, and changing or removing the contents of a method. 

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

## Thanks to

* Outer Wilds, one of my favorite games ever: http://www.outerwilds.com
* SMAPI, the main inspiration for this project: https://smapi.io

Dependencies:
* dnpatch: https://github.com/ioncodes/dnpatch
* dnlib: https://github.com/0xd4d/dnlib
* Harmony: https://github.com/pardeike/Harmony
* Newtonsoft.Json for Unity: https://github.com/SaladLab/Json.Net.Unity3D