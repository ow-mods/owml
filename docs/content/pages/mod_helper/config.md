---
Title: Config
---

# ModHelper.Config

Access your mod's settings. You can check out how to use this module in [the creating mod settings tutorial]({{ "Creating Mod Settings"|route }}).

## Enabled

Whether your mod is enabled.

## Settings

A `Dictionary<string, object>` containing your mod's settings, it's recommended to use the below methods to access the settings instead of interfacing with this dict directly.

## GetSettingValue&lt;T&gt;

Gets the settings value from the mod's config with the given key. Deserialized into type `T`

### Get Parameters

- `string key`: The key to get

## SetSettingsValue

Sets the settings value in the mod's config with the given key to the given value.

### Set Parameters

- `string key`: The key to set
- `object value`: The value to set the key to, auto-serialized to a JSON string

## Copy

Copies the config of the mod
