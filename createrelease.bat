rmdir /Q /S "Release"
mkdir "Release"

copy "OWML.Patcher\bin\Debug\OWML.Patcher.dll" "Release\OWML.Patcher.dll"
copy "OWML.ModLoader\bin\Debug\OWML.ModLoader.dll" "Release\OWML.ModLoader.dll"
copy "OWML.Launcher\bin\Debug\OWML.Launcher.exe" "Release\OWML.Launcher.exe"
copy "OWML.Launcher\bin\Debug\OWML.Config.json" "Release\OWML.Config.json"
copy "OWML.Patcher\bin\Debug\dnpatch.dll" "Release\dnpatch.dll"
copy "OWML.Patcher\bin\Debug\dnlib.dll" "Release\dnlib.dll"
copy "OWML.Launcher\bin\Debug\System.Runtime.Serialization.dll" "Release\System.Runtime.Serialization.dll"

copy "OWML.Nuget\bin\Debug\NAudio-Unity.dll" "Release\NAudio-Unity.dll"
copy "OWML.Nuget\bin\Debug\OWML.Common.dll" "Release\OWML.Common.dll"
copy "OWML.Nuget\bin\Debug\OWML.ModHelper.dll" "Release\OWML.ModHelper.dll"
copy "OWML.Nuget\bin\Debug\OWML.ModHelper.Events.dll" "Release\OWML.ModHelper.Events.dll"
copy "OWML.Nuget\bin\Debug\OWML.ModHelper.Assets.dll" "Release\OWML.ModHelper.Assets.dll"
copy "OWML.Nuget\bin\Debug\0Harmony.dll" "Release\0Harmony.dll"
copy "OWML.Nuget\bin\Debug\Newtonsoft.Json.dll" "Release\Newtonsoft.Json.dll"

mkdir "Release\Mods"

mkdir "Release\Mods\OWML.EnableDebugMode"
copy "OWML.SampleMods\OWML.EnableDebugMode\bin\Debug\OWML.EnableDebugMode.dll" "Release\Mods\OWML.EnableDebugMode\OWML.EnableDebugMode.dll"
copy "OWML.SampleMods\OWML.EnableDebugMode\bin\Debug\manifest.json" "Release\Mods\OWML.EnableDebugMode\manifest.json"

mkdir "Release\Mods\OWML.TestMod"
copy "OWML.SampleMods\OWML.TestMod\bin\Debug\OWML.TestMod.dll" "Release\Mods\OWML.TestMod\OWML.TestMod.dll"
copy "OWML.SampleMods\OWML.TestMod\bin\Debug\manifest.json" "Release\Mods\OWML.TestMod\manifest.json"

mkdir "Release\Mods\OWML.LoadCustomAssets"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\OWML.LoadCustomAssets.dll" "Release\Mods\OWML.LoadCustomAssets\OWML.LoadCustomAssets.dll"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\manifest.json" "Release\Mods\OWML.LoadCustomAssets\manifest.json"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\duck.obj" "Release\Mods\OWML.LoadCustomAssets\duck.obj"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\duck.png" "Release\Mods\OWML.LoadCustomAssets\duck.png"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\blaster-firing.wav" "Release\Mods\OWML.LoadCustomAssets\blaster-firing.wav"
copy "OWML.SampleMods\OWML.LoadCustomAssets\bin\Debug\spiral-mountain.mp3" "Release\Mods\OWML.LoadCustomAssets\spiral-mountain.mp3"

7z a "Release\OWML.zip" "Release"