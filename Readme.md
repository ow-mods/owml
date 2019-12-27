# Outer Wilds Mod Loader

OWML makes mod development for Outer Wilds much easier, and lets us use many mods simultaneously. Hopefully this will encourage many people to make mods for this amazing game. OWML behaves similarly to SMAPI for Stardew Valley.

## How it works

OWML does the following:
1. Shows the mods found in the Mods folder.
2. Patches the game to make it call the mod loader. 
3. Starts the game which calls the mod loader.
4. The mod loader does this for each mod:
   1. Creates a new Unity game object containing the mod behaviour.
   2. Initializes the mod behaviour with a mod helper.
   
## Sample mods

Some mods are included as examples/inspiration. **They are all disabled by default. Enable in manifest.json.**

|Sample mod|Description|
|----------|-----------|
|OWML.EnableDebugMode|Enables the built-in debug mode in the game. Highlights: cycle through debug UIs with F1, warp to planets with the number keys, and explode the sun with the End key.|
|OWML.TestMod|Blows up the sun as soon as the player wakes up.|
|OWML.LoadCustomAssets|Showcases loading of custom 3D objects and audio. Click the left mouse button to shoot rubber ducks.|

## For players

1. Extract the OWML zip file anywhere you want.
2. Check that the path to the game is correct in OWML.Config.json.
3. Download mods and put them in the Mods folder. Make sure each mod has its own folder in Mods.
4. Run OWML.Launcher.exe.

## For modders

Refer to the sample mods for examples.

### Get started

1. Create a class library project targeting .Net Framework 3.5.
2. Install the [OWML Nuget package](https://www.nuget.org/packages/OWML/).
3. Reference the following files in {gamePath}\OuterWilds_Data\Managed:
    * Assembly-CSharp.dll
    * UnityEngine.CoreModule.dll
    * More Unity DLLs if needed

Inherit from ModBehaviour. You can have any number of classes/projects you want, but only one ModBehaviour per mod.

### Magic methods

ModBehaviour is a Unity MonoBehaviour. Unity will call whichever magic Unity methods are on there, such as Awake, Start, Update and FixedUpdate. See the [Unity doc](https://docs.unity3d.com/ScriptReference/MonoBehaviour.html) for more info.

Your initial logic goes in Start, called by Unity. You'll have access to the mod helper at that time. Example:

~~~~
public class TestMod : ModBehaviour
{
    private void Start()
    {
        ModHelper.Console.WriteLine($"In {nameof(TestMod)}!");
    }
}
~~~~

### Mod helper

The mod helper contains useful helper classes:

|Helper|Description|
|------|-----------|
|Logger|Logs to file: Logs\OWML.Log.txt|
|Console|Prints to the console (via Logs\OWML.Output.txt...)|
|Events|Allows listening to events, such as Awake and Start of MonoBehaviours. Uses HarmonyHelper.|
|HarmonyHelper|Helper methods for Harmony, such as extending a method with another, and changing or removing the contents of a method.|
|Assets|Loads custom 3D objects and audio.|
|Storage|Save and load data.|
|?|More to come!|

Note: ModHelper can not be used in Awake, it's not initialized at that time.

### Events

Start in your ModBehaviour will be called when the game starts (at the title menu), which is usually too early for what you want to do. The mod helper contains events we can use to know when certain things happen. Here we add an event for when Flashlight has started, which is after the player has "woken up":

~~~~
private void Start()
{
	ModHelper.Events.AddEvent<Flashlight>(Events.AfterStart);
	ModHelper.Events.OnEvent += OnEvent;
}

private void OnEvent(MonoBehaviour behaviour, Events ev)
{
	ModHelper.Console.WriteLine("Behaviour name: " + behaviour.name);
	if (behaviour.GetType() == typeof(Flashlight) && ev == Events.AfterStart)
	{
		ModHelper.Console.WriteLine("Flashlight has started!");
	}
}
~~~~

### Load custom assets

ModHelper.Assets lets you load assets at runtime. See the sample mod OWML.LoadCustomAssets.

Put your custom assets in your mod folder, then load them like this:
~~~~
var duckAsset = ModHelper.Assets.Load3DObject("duck.obj", "duck.png");
duckAsset.OnLoaded += duck => ...

var audioAsset = ModHelper.Assets.LoadAudio("blaster-firing.wav");
audioAsset.OnLoaded += audioSource => ...
~~~~

It's recommended to load custom assets at the start of the game, then copy/play when needed, like this:
~~~~
var duckCopy = Instantiate(_duck);
_audio.Play();
~~~~

Supported asset types:

|Asset type|Compatibility|
|----------|-------------|
|3D models|.obj is supported, using [this script](https://wiki.unity3d.com/index.php?title=ObjImporter).|
|Images|Most formats are supported, using Unity's [WWW](https://docs.unity3d.com/ScriptReference/WWW.html).|
|Audio|Wav is supported using [WWW](https://docs.unity3d.com/ScriptReference/WWW.html). Mp3 is supported using [NAudio-Unity](https://github.com/WulfMarius/NAudio-Unity).|

Support for more asset types is in progress.

### Load and save data

Put a JSON file in your mod folder and load and save it like so:
~~~~
var saveFile = ModHelper.Storage.Load<SaveFile>("savefile.json");
...
ModHelper.Storage.Save(saveFile, "savefile.json");
~~~~

### Tips and tricks

#### Multiple MonoBehaviours

Your mod can contain more MonoBehaviors which can be added dynamically:
~~~~
AddComponent<SomeBehaviour>();
~~~~

#### Listen for inputs
~~~~
private void Update()
{
	if (Input.GetKeyDown(DebugKeyCode.cycleGUIMode))
	{
		ModHelper.Console.WriteLine("F1 pressed!");
	}
}
~~~~

#### Decompile the game

Use [dnSpy](https://github.com/0xd4d/dnSpy) to browse the game code and learn how the game ticks. Open {gamePath}\OuterWilds_Data\Managed\Assembly-CSharp.dll in dnSpy.

#### Reflection

Change private variables with [reflection](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/reflection). Example:
~~~~
typeof(GUIMode).GetAnyField("_renderMode").SetValue(null, _renderValue);
~~~~

OWML.Events.dll contains extension methods for easy reflection. Get and set private variables like this:

~~~~
var foo = behaviour.GetValue<string>("_foo");
behaviour.SetValue<string>("_bar", foo);
~~~~

#### Patch game methods

Modify existing game methods with [Harmony](https://github.com/pardeike/Harmony). The mod helper contains a wrapper for Harmony, making some of the functionality easy to use. See the source code of HarmonyHelper and ModEvents. Here we remove the contents of DebugInputManagers Awake method which makes sure the debug mode isn't disabled:
~~~~
ModHelper.HarmonyHelper.EmptyMethod<DebugInputManager>("Awake");
~~~~

#### Store extra info about objects

Unity takes care of comparing MonoBehaviours, so you can have a Dictionary with MonoBehaviours as keys, and whatever extra info as the values:

~~~~
var someBehaviour = GetComponent<SomeBehaviour>();
var dict = new Dictionary<MonoBehaviour>();
dict.Add(someBehaviour, 1337);
var num = dict[someBehaviour];
~~~~

### Manifest

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
|gamePath|The path to the game files. This must be correct for anything to work. Default: "C:/Program Files (x86)/Outer Wilds"|
|verbose|If this is true, errors from all of the game will be displayed and logged. Intended for modders only.|

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

## Feedback

I'll be working tightly with the mod community to improve OWML and aid in mod development. 
I'm Alek on the [Outer Wilds Discord](https://discord.gg/csKYR3w).

Feature requests, bug reports and PRs are welcome on GitHub.

## Credits

* Outer Wilds: http://www.outerwilds.com
* Outer Wilds on Discord: https://discord.gg/csKYR3w
* Outer Wilds on Reddit: https://www.reddit.com/r/outerwilds
* SMAPI, the main inspiration for this project: https://smapi.io
* Texture_Turtle for graphics on Nexus Mods page.

Dependencies:
* dnpatch for patching DLL files: https://github.com/ioncodes/dnpatch
  * Uses dnlib: https://github.com/0xd4d/dnlib
* Harmony for patching DLLs in memory: https://github.com/pardeike/Harmony
* Newtonsoft.Json for Unity: https://github.com/SaladLab/Json.Net.Unity3D
* ObjImporter: https://wiki.unity3d.com/index.php?title=ObjImporter
* NAudio-Unity: https://github.com/WulfMarius/NAudio-Unity
