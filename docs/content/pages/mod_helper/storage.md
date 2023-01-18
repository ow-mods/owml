---
Title: Storage
---

# ModHelper.Storage

Helps with storing data that persists through different runs using JSON.

## Load&lt;T&gt;

Loads an object (of type &lt;T&gt;) from the given file path. Loads the `default` of `T` if the file doesn't exist.

### Load Parameters

(*italicized* = optional)

- `string filename`: The file path, relative to your mod's directory, of the JSON file to load the object from.
- *`bool fixBackslashes`*: Replaces backslashes with forward-slashes, this can mess up escape characters; defaults to `true`
- *`JsonSerializerSettings settings`*: Use custom [JsonSerializerSettings](https://www.newtonsoft.com/json/help/html/T_Newtonsoft_Json_JsonSerializerSettings.htm){target="_blank"}.

## Save&lt;T&gt;

Saves an object (of type &lt;T&gt;) to the given file path. Creates a new file if one doesn't exist.

### Save Parameters

- `T obj`: The object to save.
- `string filename`: The file path, relative to your mod's directory, of the file to save the object to.

## Example

```csharp
public class MyCoolMod : ModBehaviour {

    public class MyModData {
        public DateTime lastRunDate = DateTime.now();
    }

    public void Start() {
        var myData = ModHelper.Storage.Load<MyModData>("save.json");
        ModHelper.Console.WriteLine($"You last used my mod on {myData.lastRunDate.ToShortDateString()}");
        myData.lastRunDate = DateTime.now();
        ModHelper.Storage.Save<MyModData>(myData, "save.json");
    }

}
```

!!! alert-warning "Warning"
    Please note that it's possible for your mod's save data to get removed when updating. To mitigate this set [pathsToPreserve]({{ "Manifest Schema"|route }}#pathsToPreserve){class="link-info"} in your manifest.
