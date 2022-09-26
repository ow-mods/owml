---
Title: Assets
---

<!-- TODO: AAAAAAAAAAAAAAAAAA WHY ARE THE SIGNATURES SO WEIRD -->
<!-- Awaiting context -->

# ModHelper.Assets

This module helps with loading assets from the filesystem.

## LoadBundle

Loads an asset bundle from your mod's folder.

### Parameters

- `string filename`: The file path, relative to your mod's directory, to load the bundle from.

### Example

```csharp
public class MyCoolMod : ModBehaviour {
    public void Start() {
        var bundle = ModHelper.Assets.LoadBundle("assets/my_bundle");
        LoadManager.OnCompleteSceneLoad += (scene, loadScene) =>
        {
           if (loadScene != OWScene.SolarSystem) return;
           var prefab = bundle.LoadAsset<GameObject>("Assets/Prefabs/myPrefab.prefab");
           GameObject.Instantiate(prefab, Vector3.zero);
        };
    }
}
```
