# &nbsp;[![icon](MCDSaveEdit/Properties/icon.ico)]() Minecraft: Dungeons Save File Editor
[![GitHub](https://img.shields.io/github/license/cutflame/mcdsaveedit)](https://github.com/CutFlame/MCDSaveEdit/blob/master/LICENSE)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/cutflame/mcdsaveedit?label=latest)](https://github.com/CutFlame/MCDSaveEdit/releases/latest)
[![GitHub Release Date](https://img.shields.io/github/release-date/cutflame/mcdsaveedit)](https://github.com/CutFlame/MCDSaveEdit/releases/latest)
[![GitHub all releases](https://img.shields.io/github/downloads/cutflame/mcdsaveedit/total)](https://github.com/CutFlame/MCDSaveEdit/releases)

Windows application for modifying [Minecraft: Dungeons](https://www.minecraft.net/en-us/about-dungeons/) save files.

#### DISCLAIMER: Please keep backups of your save files! This app does not guarantee your save file to be playable after editing!


<img src="screenshot.png"/>

---

### Installing and Running

1. Download and extract the latest release (MCDSaveEdit_*.zip) from [the releases section](https://github.com/CutFlame/MCDSaveEdit/releases)
1. If you have the Windows Store version of Minecraft: Dungeons -
   1. Download and run the [storepatcher.ps1](https://github.com/dungeonsworkshop/dungeonsworkshop.github.io/releases) powershell script to extract the required files from the Windows Store version of Minecraft: Dungeons
1. Run MCDSaveEdit.exe

For full features and functionality you need Minecraft: Dungeons installed and preferably in the default install location.

---

### Compiling
This application was developed entirely in Visual Studio 2019.

When cloning be sure to recurse through submodules because there are 2:
- [DungeonTools](https://github.com/HellPie/DungeonTools)
- [PakReader](https://github.com/WorkingRobot/PakReader)

There is one file deliberately removed from the repo that you will need to create and fill in.

`MCDSaveEdit\Data\Secrets.cs`
```csharp
namespace MCDSaveEdit
{
    public static class Secrets
    {
        // Fill in the value for this one
        public static string[] PAKS_AES_KEY_STRINGS = new string[] {
            "<AES key for unlocking the MCD .pak files>",
        };

        // You can leave these empty, they just need to exist
        public const string GAME_ANALYTICS_GAME_KEY = "";
        public const string GAME_ANALYTICS_SECRET_KEY = "";
    }
}
```


---

### Legal Disclaimer

This project is not affiliated with Mojang Studios, XBox Game Studios, Double 11 or the Minecraft brand.

"Minecraft" is a trademark of Mojang Synergies AB.

Other trademarks referenced herein are property of their respective owners.


### External Credits and Licenses

Images from the game are subject to copyright by Mojang. They are extracted at runtime from the .pak files that are installed as part of the Minecraft: Dungeons game files.


[DungeonTools](https://github.com/HellPie/DungeonTools) © Diego Russi ([AGPL 3.0](https://github.com/HellPie/DungeonTools/blob/master/LICENSE))

[Microsoft.Bcl.AsyncInterfaces](https://github.com/dotnet/corefx) © Microsoft ([MIT](https://licenses.nuget.org/MIT))

[Fody](https://github.com/Fody/Fody) © Simon Cropp ([MIT](https://github.com/Fody/Fody/blob/master/License.txt))

[Costura.Fody](https://github.com/Fody/Costura) © Simon Cropp and contributors ([MIT](https://github.com/Fody/Costura/blob/develop/LICENSE))

[FModel](https://github.com/iAmAsval/FModel) © Free Software Foundation, Inc. ([GPL 3.0](https://github.com/iAmAsval/FModel/blob/master/LICENSE))

[PakReader](https://github.com/WorkingRobot/PakReader) © Aleks Margarian ([MIT](https://github.com/WorkingRobot/PakReader/blob/master/LICENSE))

[Game-icons.net](https://game-icons.net/) © Lorc, Delapouite and contributors ([CC BY 3.0](http://creativecommons.org/licenses/by/3.0/))
