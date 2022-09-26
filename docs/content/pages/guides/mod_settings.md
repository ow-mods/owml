---
Title: Creating Mod Settings
Sort_Priority: 75
---

# Creating Mod Settings

This guide outlines how to use OWML's config system to display mod settings in the settings menu.

## Editing default-config.json

First things first, you're going to need to define your settings, open up `default-config.json` and add an object called `settings`.

```json
{
    "enabled": true,
    "settings" {
     
    }
}
```

The settings object is comprised of key-value pairs where the key is the ID of the setting and the value is default value of that setting.  

For example, if I wanted a check box called "Party Mode", I would do:

```json
{
    "enabled": true,
    "settings": {
        "Party Mode": true
    }
}
```

You can also do other data-types like numbers and strings

```json
{
    "enabled": true,
    "settings": {
        "Number Of Ducks": 5,
        "Favorite Food": "Marshmallows"
    }
}
```

You can even make a selection field (where you can only select specific values) using arrays

```json
{
    "enabled": true,
    "settings": {
        "Favorite Color": ["Purple", "Green", "Wrong >::("]
    }
}
```

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
        ModHelper.Console.WriteLine($"You changed your favorite food to: ${newFavorite}!");
    }
}
```
