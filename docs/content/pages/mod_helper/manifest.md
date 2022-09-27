---
Title: Manifest
---

# ModHelper.Manifest

Get various metadata about your mod.

## Schema

Almost all properties in [the manifest schema]({{ "Manifest Schema"|route }}) are available in this module except for:

- `pathsToPreserve`
- `conflicts`
- `warning`

These properties are converted from camelCase to PascalCase (ex: `uniqueName` -> `ModHelper.Manifest.UniqueName`)

## ModFolderPath

The absolute file path to your mod's directory, most OWML methods auto-prepend this.
