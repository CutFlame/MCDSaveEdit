# &nbsp;[![icon](https://raw.githubusercontent.com/CutFlame/MCDSaveEdit/master/MCDSaveEdit/Properties/icon.ico)]() Minecraft: Dungeons Save File Editor
[![GitHub](https://img.shields.io/github/license/cutflame/mcdsaveedit)](https://github.com/CutFlame/MCDSaveEdit/blob/master/LICENSE)
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/cutflame/mcdsaveedit?label=latest)](https://github.com/CutFlame/MCDSaveEdit/releases/latest)
[![GitHub Release Date](https://img.shields.io/github/release-date/cutflame/mcdsaveedit)](https://github.com/CutFlame/MCDSaveEdit/releases/latest)
[![GitHub all releases](https://img.shields.io/github/downloads/cutflame/mcdsaveedit/total)](https://github.com/CutFlame/MCDSaveEdit/releases)

Windows application for modifying [Minecraft: Dungeons](https://www.minecraft.net/en-us/about-dungeons/) save files.

#### DISCLAIMER: Please keep backups of your save files! This app could make your save file unplayable!


---

### Installing and Running

##### IMPORTANT! If you have the Windows Store version of Minecraft: Dungeons you need to download and run the [storepatcher.exe](https://docs.dungeonsworkshop.net/gettingstarted/) first (the .pak files need to be extracted)

1. Download the latest release (MCDSaveEdit_*.zip) from [here](https://github.com/CutFlame/MCDSaveEdit/releases/tag/release/latest)
1. Extract it (it's just a simple single portable executable)
1. Run it

For full features and functionality you need Minecraft: Dungeons installed and preferrably in the default install location.

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
        public const string PAKS_AES_KEY_STRING = "<AES key for unlocking the MCD .pak files>";

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

Images from the game are subject to copyright by Mojang. They are extracted at runtime from the .pak files installed as part of the Minecraft: Dungeons game files. 


[DungeonTools](https://github.com/HellPie/DungeonTools) © Diego Russi ([AGPL 3.0](https://github.com/HellPie/DungeonTools/blob/master/LICENSE))

[Microsoft.Bcl.AsyncInterfaces](https://github.com/dotnet/corefx) © Microsoft ([MIT](https://licenses.nuget.org/MIT))

[Fody](https://github.com/Fody/Fody) © Simon Cropp ([MIT](https://github.com/Fody/Fody/blob/master/License.txt))

[Costura.Fody](https://github.com/Fody/Costura) © Simon Cropp and contributors ([MIT](https://github.com/Fody/Costura/blob/develop/LICENSE))

[FModel](https://github.com/iAmAsval/FModel) © Free Software Foundation, Inc. ([GPL 3.0](https://github.com/iAmAsval/FModel/blob/master/LICENSE))

[PakReader](https://github.com/WorkingRobot/PakReader) © Aleks Margarian ([MIT](https://github.com/WorkingRobot/PakReader/blob/master/LICENSE))

[Game-icons.net](https://game-icons.net/) © Lorc, Delapouite and contributors ([CC BY 3.0](http://creativecommons.org/licenses/by/3.0/))
