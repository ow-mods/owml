---
Title: Getting Started
Sort_Priority: 90
---

# Getting Started

This page will outline how to get a working mod that will simply log a message to the console when the game starts.

For this guide we'll assume you already have the [Outer Wilds Mod Manager](https://github.com/ow-mods/ow-mod-manager){ target="_blank" } installed.

## Choosing an IDE

An IDE will help provide the ability to create, edit, and build your mod.

The recommended IDE for modding is [Visual Studio](https://visualstudio.microsoft.com/){ target="_blank" }, however, stuff like Rider and VSCode can also work. This tutorial will assume you're using Visual Studio.

## Installing Visual Studio

Head to the [Visual Studio downloads page](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&channel=Release&version=VS2022&source=VSLandingPage&cid=2030&passive=false){ target="_blank" } and select Community 2022, after downloading and launching the installer, follow instructions until you reach this screen:

![Select the module in the installer]({{ "images/getting_started/dotnetinstall.webp"|static }})

We want the "Desktop development with .NET" module, this will provide us with the tools we need to build the mod.

After installing Visual Studio, launch it once and then close it, this will ensure certain files are generated.

## Installing the Template

We provide a template to make creating mods easier, this template will handle renaming files and changing the manifest.

Open up the windows search box and search for "Developer Command Line for Visual Studio 2022", it should look like this:

![Select the developer command line]({{ "images/getting_started/devconsole.webp"|static }})

To install the template, run the following command:

```sh
dotnet new --install Bwc9876.OuterWildsModTemplate
```

This will give an output similar to this:

```txt
--------------------------------------------------------------------------------------
The following template packages will be installed:
   Bwc9876.OuterWildsModTemplate

Success: Bwc9876.OuterWildsModTemplate::1.3.0 installed the following templates:
Template Name    Short Name     Language  Tags
---------------  -------------  --------  -------
Outer Wilds Mod  OuterWildsMod  [C#]      Library
```

Once installed, you can close the developer command prompt and re-open Visual Studio.

## Using the Template

Now that we have the template installed, open Visual Studio and select "New Project" from the welcome screen. Then search "Outer Wilds" and the template should appear in the list.

Set the project name to the name of your mod, **please note this should NOT have spaces or special characters in it**.  The standard casing for projects is PascalCase, which involves capitalizing the start of every word and removing spaces.

On the next screen, set the author name to the name you want to appear in the manager and on the website, **this should also not contain spaces**

!!! alert-danger "Important Note"
    Ensure that the "Put Solution and Project in same directory" is unchecked,
    the pre-built Action for releasing mods depends on this.

Finally, click "Create Project"

## General Mod Layout

The general layout of an Outer Wilds mod is as follows:

### manifest.json

This file contains metadata about your mod, such as its name, author, and version.

### {YourProjectName}.cs

This file should have been renamed to your project name, it acts as the entry point for the mod.

### default-config.json

This file is used by OWML to generate the settings menu for your mod, we'll go over it in another guide

### {YourProjectName}.csproj

This file tells Visual Studio about your project, it determines stuff like dependencies and versions, you shouldn't need to touch this.

## The ModBehaviour File

Double-click {YourProjectName}.cs, and it should open up in the main editor pane.

This file should contain a class that has the same name as your project and some methods within that class.

The class this class inherits from is `ModBehaviour`, which is a special `MonoBehaviour` that not only marks a class as the entry-point for a mod, but also provides various utilities and overridable methods.

We'll focus on `Start()`. In this method we do two things:

1. We output a message to the console alerting the user that the mod has loaded
2. We subscribe to the scene loaded event to output a message to the log when the SolarSystem scene is loaded.

You may have noticed we use the ModHelper field to achieve console output, ModHelper is a collection of utilities your mod can use to do a variety of things. It's covered in the "Mod Helper" section of the site.

!!! alert-warning "Warning"
    There's only one `ModBehaviour` allowed per-mod, to add more components, you'll need to use `AddComponent<>` within your mod behaviour class.

## Building The Mod

Now that we know what the mod *should* do, let's make sure it does it. Building your mod should be as simple as pressing "Solution -> Build Solution" in the menu bar, if you get an error involving Visual Studio not being able to find a path, please see the section below, otherwise, skip to "Running The Mod"

### Fixing .csproj.user

Your mod contains a special file called {YourProjectName}.csproj.user, this file tells Visual Studio where to build the mod, if you've installed the manager in a non-standard location, this file will be incorrect. To fix this, open the manager and select settings, then copy the path in the "OWML Path" field. Copy and paste this value between the `<OutputPath>` and `</OutputPath>`, and add `\Mods` to the end of the path. Then open up your manifest file and copy the `uniqueName` field (don't include the quotes). Paste this value preceded by a `\` at the end of the path.

For example, if my mod's uniqueName is `Bwc9876.CoolMod`, my file would look like this:

```xml
<OutputPath>C:\MyCoolFolder\DifferentManagerInstallFolderBcImAHacker\Mods\Bwc9876.CoolMod</OutputPath>
```

## Running The Mod

Now the mod should have appeared in your mod manager at the very bottom, notice how the download count is a dash.

Your mod should now be ready to run!

Click start game and wait for the title screen to load in. Now search your manager logs (there's a search box) for a message along the lines of "My mod {YourProjectName} is loaded!".  This means your mod was loaded successfully! You can also try loading into the main game and checking the logs for another message from your mod.

## Getting Line Numbers

When developing your mod you may want to get line numbers in your stack trace. To do so, [download this dll file]({{ "mono-2.0-bdwgc.dll"|static }}){target="_blank"}, and have it replace the one with the same name within `MonoBleedingEdge/EmbedRuntime` of the game's files. Doing this will degrade performance slightly, but will allow all mods that use the `Portable` debug type to have line numbers. If you've used the template to create your mod, simply build with the "Debug" release candidate to have DebugType set to portable.

## Next Steps

You've successfully created and built your first Outer Wilds mod, moving forward may require a bit of knowledge in unity and will depend on what exactly you're trying to do. You may want to read the following guides to get an idea of how to make your mod:

- [Patching the game with HarmonyX]({{ "Patching"|route }})
- [Interacting With Other Mods via APIs]({{ "Creating APIs"|route }})
- [Creating custom mod settings]({{ "Creating Mod Settings"|route }})
- [Publishing your mod]({{ "Publishing Your Mod"|route }})

These guides will provide information on how to use various aspects of OWML, but they won't cover everything. We also have a few other sites with information on different parts of OW modding.

- Learn Outer Wild's internal workings on the [Outer Wilds Unity Wiki](https://github.com/ow-mods/outer-wilds-unity-wiki/wiki){ target="_blank" }
- Learn how to create custom content easily with [New Horizons](https://nh.outerwildsmods.com){ target="_blank" }

If you ever need help, or even just want to chat about modding, feel free to [join our Discord](https://discord.gg/wusTQYbYTc){ target="_blank" }. There's almost always someone available to help.
