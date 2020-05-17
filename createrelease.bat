rmdir /Q /S "Release"
mkdir "Release"
mkdir "Release\Mods"
mkdir "Release\VR"

copy "OWML.Patcher\bin\Debug\OWML.Patcher.dll" "Release\OWML.Patcher.dll"
copy "OWML.ModLoader\bin\Debug\OWML.ModLoader.dll" "Release\OWML.ModLoader.dll"
copy "OWML.Launcher\bin\Debug\OWML.Launcher.exe" "Release\OWML.Launcher.exe"
copy "OWML.Launcher\bin\Debug\OWML.Config.json" "Release\OWML.Config.json"
copy "OWML.Launcher\bin\Debug\OWML.Manifest.json" "Release\OWML.Manifest.json"
copy "OWML.Patcher\dnpatch\dnpatch.dll" "Release\dnpatch.dll"
copy "OWML.Patcher\dnpatch\dnlib.dll" "Release\dnlib.dll"
copy "OWML.Patcher\VR\*" "Release\VR\"
copy "OWML.Patcher\VR\BsPatch.dll" "Release\BsPatch.dll"
copy "OWML.Patcher\VR\ICSharpCode.SharpZipLib.dll" "Release\ICSharpCode.SharpZipLib.dll"
copy "OWML.Launcher\bin\Debug\System.Runtime.Serialization.dll" "Release\System.Runtime.Serialization.dll"

copy "OWML.Nuget\bin\Debug\NAudio-Unity.dll" "Release\NAudio-Unity.dll"
copy "OWML.Nuget\bin\Debug\OWML.Common.dll" "Release\OWML.Common.dll"
copy "OWML.Nuget\bin\Debug\OWML.ModHelper.dll" "Release\OWML.ModHelper.dll"
copy "OWML.Nuget\bin\Debug\OWML.ModHelper.Events.dll" "Release\OWML.ModHelper.Events.dll"
copy "OWML.Nuget\bin\Debug\OWML.ModHelper.Assets.dll" "Release\OWML.ModHelper.Assets.dll"
copy "OWML.Nuget\bin\Debug\OWML.ModHelper.Interaction.dll" "Release\OWML.ModHelper.Interaction.dll"
copy "OWML.Nuget\bin\Debug\OWML.ModHelper.Menus.dll" "Release\OWML.ModHelper.Menus.dll"
copy "OWML.Nuget\bin\Debug\OWML.ModHelper.Input.dll" "Release\OWML.ModHelper.Input.dll"
copy "OWML.Nuget\bin\Debug\0Harmony.dll" "Release\0Harmony.dll"
copy "OWML.Nuget\bin\Debug\Newtonsoft.Json.dll" "Release\Newtonsoft.Json.dll"

mkdir "Release\Mods\OWML.EnableDebugMode"
copy "OWML.SampleMods\OWML.EnableDebugMode\bin\Debug\OWML.EnableDebugMode.dll" "Release\Mods\OWML.EnableDebugMode\OWML.EnableDebugMode.dll"
copy "OWML.SampleMods\OWML.EnableDebugMode\bin\Debug\manifest.json" "Release\Mods\OWML.EnableDebugMode\manifest.json"
copy "OWML.SampleMods\OWML.EnableDebugMode\bin\Debug\default-config.json" "Release\Mods\OWML.EnableDebugMode\default-config.json"

mkdir "Release\Mods\TAIcheat"
copy "TAIcheat\bin\Debug\TAIcheat.dll" "Release\Mods\TAIcheat\TAIcheat.dll"
copy "TAIcheat\manifest.json" "Release\Mods\TAIcheat\manifest.json"
copy "TAIcheat\default-config.json" "Release\Mods\TAIcheat\default-config.json"

mkdir "Release\Mods\OWML.LoadCustomAssets"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\OWML.LoadCustomAssets.dll" "Release\Mods\OWML.LoadCustomAssets\OWML.LoadCustomAssets.dll"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\manifest.json" "Release\Mods\OWML.LoadCustomAssets\manifest.json"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\default-config.json" "Release\Mods\OWML.LoadCustomAssets\default-config.json"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\duck.obj" "Release\Mods\OWML.LoadCustomAssets\duck.obj"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\duck.png" "Release\Mods\OWML.LoadCustomAssets\duck.png"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\blaster-firing.wav" "Release\Mods\OWML.LoadCustomAssets\blaster-firing.wav"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\spiral-mountain.mp3" "Release\Mods\OWML.LoadCustomAssets\spiral-mountain.mp3"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\savefile.json" "Release\Mods\OWML.LoadCustomAssets\savefile.json"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\cubebundle" "Release\Mods\OWML.LoadCustomAssets\cubebundle"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\cubebundle.manifest" "Release\Mods\OWML.LoadCustomAssets\cubebundle.manifest"

7z a "Release\OWML.zip" "Release"
