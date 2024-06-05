---
Title: Creating Mod Settings
Sort_Priority: 75
---

# Creating Mod Settings

This guide outlines how to use OWML's config system to display mod settings in the settings menu.

## Editing default-config.json

First things first, you're going to need to define your settings, open up `default-config.json`, and add an object called `settings`.

```json
{
    "$schema": "https://raw.githubusercontent.com/ow-mods/owml/master/schemas/config_schema.json",
    "enabled": true,
    "settings": {
     
    }
}
```

The settings object is comprised of key-value pairs where the key is the ID of the setting and the value is the default value of that setting.  

For example, if I wanted a check box called "Party Mode", I would do:

```json
{
    "$schema": "https://raw.githubusercontent.com/ow-mods/owml/master/schemas/config_schema.json",
    "enabled": true,
    "settings": {
        "Party Mode": true
    }
}
```

You can also do other data types like numbers and strings

```json
{
    "$schema": "https://raw.githubusercontent.com/ow-mods/owml/master/schemas/config_schema.json",
    "enabled": true,
    "settings": {
        "Number Of Ducks": 5,
        "Favorite Food": "Marshmallows"
    }
}
```

## Complex Settings

Complex settings use JSON objects to allow for more customization. You can use it to add selectors:

```json
{
    "$schema": "https://raw.githubusercontent.com/ow-mods/owml/master/schemas/config_schema.json",
    "enabled": true,
    "settings": {
        "Favorite Color": {
            "type": "selector",
            "value": "Green",
            "options": [
                "Purple",
                "Green",
                "Wrong >::("
            ]
        }
    }
}
```

And sliders as well:

```json
{
    "$schema": "https://raw.githubusercontent.com/ow-mods/owml/master/schemas/config_schema.json",
    "enabled": true,
    "settings": {
        "Bumpscosity": {
            "type": "slider",
            "min": 0,
            "max": 1000,
            "value": 1
        }
    }
}
```

You can also separate your options with separators:

```json
{
    "$schema": "https://raw.githubusercontent.com/ow-mods/owml/master/schemas/config_schema.json",
    "enabled": true,
    "settings": {
        "My Cool Value": 50,
        "My Separator": {
            "type": "separator"
        },
        "My Other Cool Value But It's Below The Separator ::D": "Burger"
    }
}
```

For a full list of the types of inputs, check [our schema](https://raw.githubusercontent.com/ow-mods/owml/master/schemas/config_schema.json).

## Getting Values In C\#

Now that we have these values defined, we can use `ModHelper.Config.GetSettingsValue` to grab our options.

```csharp
public class MyMod : ModBehaviour {
    public void Start() {
        var partyMode = ModHelper.Config.GetSettingsValue<bool>("Party Mode");

        var numberOfDucks = ModHelper.Config.GetSettingsValue<int>("Number Of Ducks");
        var favoriteFood = ModHelper.Config.GetSettingsValue<string>("Favorite Food");

        var favoriteColor = ModHelper.Config.GetSettingsValue<string>("Favorite Color");

        // if (favoriteColor == "Wrong >::(") Application.Quit();
    }
}
```

If you want to listen for changes to your mod's config, you can override the `Configure` method, this will be passed the new config data.

```csharp
public class MyMod : ModBehaviour {
    public override void Configure(IModConfig config) {
        var newFavorite = config.GetSettingsValue<string>("Favorite Food");
        ModHelper.Console.WriteLine($"You changed your favorite food to: {newFavorite}!");
    }
}
```

## Config Updates

Something important to note is that when the manager pulls and update for your mod, the `config.json` file is preserved. The issue with this is menus are generated from the `config.json` file. When changing options like slider minimums and maximums or choices, you may want to create a new property rather than edit an existing one to make sure the UI is correct.

## Translations

Mod config options can be translated in the same way that [New Horizons mods do translations](https://nh.outerwildsmods.com/guides/translation/). First, you need to add a folder named `translations` in the root directory of your mod.

There are 12 supported languages in Outer Wilds: `english`, `spanish_la`, `german`, `french`, `italian`, `polish`, `portuguese_br`, `japanese`, `russian`, `chinese_simple`, `korean`, and `turkish`.

In the `translations` folder you can put json files with the name of the language you want to translate for. This file will contain a single dictionary named `UIDictionary` which will have key-value pairs for the translation.

```json
{
    "UIDictionary": {
        "settingTitle": "My Translated Title",
        "settingTooltip": "My Translated Tooltip"
    }
}
```

If your mod already uses New Horizons and supports translations, these values are added directly into the same translation files that NH uses.
