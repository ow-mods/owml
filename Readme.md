# Outer Wilds Mod Loader

OWML is the mod loader and mod framework for Outer Wilds. It patches Outer Wilds to load mods, and provides mods a framework to interact with the game. OWML is inspired by SMAPI for Stardew Valley.

## How it works

OWML does the following:
1. Patches the game to make it call the mod loader.
2. Starts the game.
3. The mod loader loads and initializes installed mods.

## Installation

With Outer Wilds Mod Manager (recommended):
1. Download the Mod Manager from the [Outer Wilds Mods](https://outerwildsmods.com/) website.
2. Use the Mod Manager to install OWML and mods, and start the game.

Manual install:
1. Download OWML and extract the zip file anywhere you want.
2. [Download Outer Wilds mods](https://outerwildsmods.com/mods) and put them in the Mods folder, each mod in a separate folder.
3. Start the game with OWML.Launcher.exe.

## For modders

Refer to the sample mods in the source code for examples. These mods are not included in releases.

### Get started

1. Create a C# class library project targeting .Net Framework 4.0.
2. Install the [OWML Nuget package](https://www.nuget.org/packages/OWML/).
3. Reference the following files in {gamePath}\OuterWilds_Data\Managed:
    * Assembly-CSharp.dll
    * UnityEngine.CoreModule.dll
    * More Unity DLLs if needed
4. Inherit from ModBehaviour.
	
For more info, see [For modders](https://github.com/amazingalek/owml/wiki/For-modders).

## Configuration

OWML is configured in the in-game MODS menu, or in OWML.Config.json:

|Key|Description|
|---|-----------|
|gamePath|The path to the game files. OWML will try to locate the game automatically.|
|debugMode|If enabled, a lot more information is written to the console. Intended for developers.|
|combinationsBlockInput|If this is true, mod input combinations will block game input.|

Each mod is defined in a manifest.json file:

|Key|Description|
|---|-----------|
|filename|The filename of the DLL containing the ModBehaviour class.|
|author|The name of the author.|
|name|The name of the mod.|
|uniqueName|Usually {author}.{uniqueName}.|
|version|The version number.|
|owmlVersion|The version of OWML the mod was built for.|
|dependencies|Array of dependency names. Make sure to use the unique name.|

Each mod can be configured in the in-game MODS menu, or in config.json:

|Key|Description|
|---|-----------|
|enabled|Whether or not the mod will be loaded. Default: true.|
|requireVR|Whether or not the mod requires VR to work. Default: false.|
|settings|An object of mod-specific settings. Default: empty.|

Example:
~~~~
{
  "enabled": true,
  "requireVR": false,
  "settings": {
    "enableMusic": true,
    "foo": "bar",
    "lol": 1337
  }
}
~~~~

More info about config can be found [here](https://github.com/amazingalek/owml/wiki/For-modders#mod-config).

## Compatibility

* Compatible with Outer Wilds version 1.1.8 and above.

## Feedback

I'm working tightly with the mod community to improve OWML and aid in mod development. 
I'm Alek on the [Outer Wilds Discord](https://discord.gg/csKYR3w).

Feature requests, bug reports and PRs are welcome on GitHub.

## Credits

Authors:
* [AmazingAlek](https://github.com/amazingalek)
* [Raicuparta](https://github.com/Raicuparta/)
* [_nebula](https://github.com/misternebula)
* [TAImatem](https://github.com/TAImatem)

Special thanks to:
* [Outer Wilds](http://www.outerwilds.com)
* [Outer Wilds on Discord](https://discord.gg/csKYR3w)
* [Outer Wilds on Reddit](https://www.reddit.com/r/outerwilds)
* Inspired by (and some code from) [SMAPI](https://smapi.io)

Dependencies:
* [dnpatch](https://github.com/ioncodes/dnpatch)
* [dnlib](https://github.com/0xd4d/dnlib)
* [HarmonyX](https://github.com/BepInEx/HarmonyX)
* [Json.Net.Unity3D](https://github.com/SaladLab/Json.Net.Unity3D)
* [ObjImporter](https://wiki.unity3d.com/index.php?title=ObjImporter)
* [NAudio-Unity](https://github.com/WulfMarius/NAudio-Unity)
* [Gameloop.Vdf](https://github.com/shravan2x/Gameloop.Vdf)
