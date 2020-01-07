# Outer Wilds Mod Loader

OWML is the mod loader and mod framework for Outer Wilds. It patches Outer Wilds to load mods, and provides mods with a framework to interact with the game. OWML is inspired by SMAPI for Stardew Valley.

## How it works

OWML does the following:
1. Displays the mods found in the Mods folder.
2. Patches the game to make it call the mod loader.
3. Starts the game which calls the mod loader.
4. The mod loader does this for each mod:
   1. Creates a new Unity game object containing the mod.
   2. Initializes the mod with a mod helper.

## Sample mods

One mod is included as an example. It's disabled by default, enable in manifest.json.

|Sample mod|Description|
|----------|-----------|
|OWML.EnableDebugMode|Enables the built-in debug mode in the game. Highlights: cycle through debug UIs with F1, warp to planets with the number keys, and explode the sun with the End key.|

## For players

1. Extract the OWML zip file anywhere you want.
2. Download mods and put them in the Mods folder, each mod in separate folders.
3. Run OWML.Launcher.exe.

## For modders

Refer to the sample mods for examples.

### Get started

1. Create a class library project targeting .Net Framework 3.5.
2. Install the [OWML Nuget package](https://www.nuget.org/packages/OWML/).
3. Reference the following files in {gamePath}\OuterWilds_Data\Managed:
    * Assembly-CSharp.dll
    * UnityEngine.CoreModule.dll
    * More Unity DLLs if needed
4. Inherit from ModBehaviour.
	
For more info, see [For modders](https://github.com/amazingalek/owml/wiki/For-modders).

## Configuration

OWML is configured by OWML.Config.json:

|Key|Description|
|---|-----------|
|gamePath|The path to the game files.|
|verbose|If this is true, errors from all of the game will be displayed and logged. Intended for modders only.|

Each mod is defined in a manifest.json file:

|Key|Description|
|---|-----------|
|filename|The filename of the DLL containing the ModBehaviour class.|
|author|The name of the author.|
|name|The name of the mod.|
|uniqueName|Usually {author}.{uniqueName}.|
|version|The version number.|
|owmlVersion|The version of OWML the mod was built for.|
|enabled|Whether or not the mod will be loaded.|

## Compatibility

* Tested with Outer Wilds 1.0.0, 1.0.2, 1.0.3 and 1.0.4.
* Currently Windows only.

## Feedback

I'll be working tightly with the mod community to improve OWML and aid in mod development. 
I'm Alek on the [Outer Wilds Discord](https://discord.gg/csKYR3w).

Feature requests, bug reports and PRs are welcome on GitHub.

Nexus page: https://www.nexusmods.com/outerwilds/mods/1

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
* HtmlAgilityPack: https://html-agility-pack.net/
* HtmlAgilityPack.CssSelector: https://github.com/hcesar/HtmlAgilityPack.CssSelector
