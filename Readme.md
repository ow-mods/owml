# Outer Wilds Mod Loader

OWML is the mod loader and mod framework for Outer Wilds. It patches Outer Wilds to load mods, and provides mods with a framework to interact with the game. OWML is inspired by SMAPI for Stardew Valley.

## How it works

OWML does the following:
1. Patches the game to make it call the mod loader.
2. Starts the game.
3. The mod loader loads and initializes installed mods.

## Installation

With [Outer Wilds Mod Manager](https://github.com/Raicuparta/ow-mod-manager) (recommended):
1. Download the [latest release of the Mod Manager](https://github.com/Raicuparta/ow-mod-manager/releases).
2. Use the Mod Manager to install OWML and mods, and start the game.

With Vortex:
1. Download OWML and extract the zip file.
2. Put the OWML folder in ﻿the game's root folder, usually C:\Program Files\Epic Games\OuterWilds.
3. Install [the Vortex extension](https://www.nexusmods.com/site/mods/73/).
4. Use [Vortex](https://www.nexusmods.com/site/mods/1/) to install mods and start the game.

Manual install:
1. Download OWML and extract the zip file anywhere you want.
2. [Download mods](https://www.nexusmods.com/outerwilds) and put them in the Mods folder, each mod in a separate folder.
3. Start the game with OWML.Launcher.exe.

## Sample mod

One mod is included as an example. It's disabled by default, enable in manifest.json.

|Sample mod|Description|
|----------|-----------|
|OWML.EnableDebugMode|Enables the built-in debug mode in the game. Highlights: cycle through debug UIs with F1, warp to planets with the number keys, and explode the sun with the End key.|

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
|gamePath|The path to the game files. OWML will try to locate the game automatically, set this manually if needed.|
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
|dependencies|\[ Dependency mod 1 name, Dependency mod 2 name, etc. \]|

Each mod can be configured with an **optional** config.json file:

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

* Tested with all versions of Outer Wilds up to and including v1.0.6.
* Currently Windows only.

## Feedback

I'm working tightly with the mod community to improve OWML and aid in mod development. 
I'm Alek on the [Outer Wilds Discord](https://discord.gg/csKYR3w).

Feature requests, bug reports and PRs are welcome on GitHub.

[Nexus page](https://www.nexusmods.com/outerwilds/mods/1)

## Credits

* [Outer Wilds](http://www.outerwilds.com)
* [Outer Wilds on Discord](https://discord.gg/csKYR3w)
* [Outer Wilds on Reddit](https://www.reddit.com/r/outerwilds)
* Inspired by [SMAPI](https://smapi.io)
* Texture_Turtle for graphics on [Nexus page](https://www.nexusmods.com/outerwilds/mods/1)
* [TAImatem](https://github.com/TAImatem) and [Raicuparta](https://github.com/Raicuparta/) for research

Dependencies:
* [dnpatch](https://github.com/ioncodes/dnpatch)
* [dnlib](https://github.com/0xd4d/dnlib)
* [Harmony](https://github.com/pardeike/Harmony)
* [Json.Net.Unity3D](https://github.com/SaladLab/Json.Net.Unity3D)
* [ObjImporter](https://wiki.unity3d.com/index.php?title=ObjImporter)
* [NAudio-Unity](https://github.com/WulfMarius/NAudio-Unity)
* [BsDiff](https://github.com/LogosBible/bsdiff.net)
