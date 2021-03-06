﻿using MCDSaveEdit.Save.Models.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
#nullable enable

namespace MCDSaveEdit
{
    public static partial class Constants {
        public readonly static StaticLevelData[] DEBUG_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData(".00", new Point(0.00, 0.00), LevelTypeEnum.mission),
            new StaticLevelData(".05", new Point(0.05, 0.05), LevelTypeEnum.mission),
            new StaticLevelData(".10", new Point(0.10, 0.10), LevelTypeEnum.mission),
            new StaticLevelData(".15", new Point(0.15, 0.15), LevelTypeEnum.mission),
            new StaticLevelData(".20", new Point(0.20, 0.20), LevelTypeEnum.mission),
            new StaticLevelData(".25", new Point(0.25, 0.25), LevelTypeEnum.mission),
            new StaticLevelData(".30", new Point(0.30, 0.30), LevelTypeEnum.mission),
            new StaticLevelData(".35", new Point(0.35, 0.35), LevelTypeEnum.mission),
            new StaticLevelData(".40", new Point(0.40, 0.40), LevelTypeEnum.mission),
            new StaticLevelData(".45", new Point(0.45, 0.45), LevelTypeEnum.mission),
            new StaticLevelData(".50", new Point(0.50, 0.50), LevelTypeEnum.mission),
            new StaticLevelData(".55", new Point(0.55, 0.55), LevelTypeEnum.mission),
            new StaticLevelData(".60", new Point(0.60, 0.60), LevelTypeEnum.mission),
            new StaticLevelData(".65", new Point(0.65, 0.65), LevelTypeEnum.mission),
            new StaticLevelData(".70", new Point(0.70, 0.70), LevelTypeEnum.mission),
            new StaticLevelData(".75", new Point(0.75, 0.75), LevelTypeEnum.mission),
            new StaticLevelData(".80", new Point(0.80, 0.80), LevelTypeEnum.mission),
            new StaticLevelData(".85", new Point(0.85, 0.85), LevelTypeEnum.mission),
            new StaticLevelData(".90", new Point(0.90, 0.90), LevelTypeEnum.mission),
            new StaticLevelData(".95", new Point(0.95, 0.95), LevelTypeEnum.mission),
            new StaticLevelData("1.0", new Point(1.00, 1.00), LevelTypeEnum.mission),
        };

        public readonly static StaticLevelData[] MAINLAND_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("creepycrypt", new Point(.19, .37), LevelTypeEnum.dungeon),
            new StaticLevelData("mooshroomisland", new Point(.26, .24), LevelTypeEnum.dungeon),
            new StaticLevelData("creeperwoods", new Point(.28, .40), LevelTypeEnum.mission),
            new StaticLevelData("soggycave", new Point(.27, .65), LevelTypeEnum.dungeon),
            new StaticLevelData("soggyswamp", new Point(.29, .74), LevelTypeEnum.mission),
            new StaticLevelData("mooncorecaverns", new Point(.44, .35), LevelTypeEnum.mission),
            new StaticLevelData("cacticanyon", new Point(.42, .56), LevelTypeEnum.mission),
            new StaticLevelData("pumpkinpastures", new Point(.46, .70), LevelTypeEnum.mission),
            new StaticLevelData("archhaven", new Point(.57, .73), LevelTypeEnum.dungeon),
            new StaticLevelData("deserttemple", new Point(.59, .45), LevelTypeEnum.mission),
            new StaticLevelData("fieryforge", new Point(.63, .20), LevelTypeEnum.mission),
            new StaticLevelData("lowertemple", new Point(.67, .59), LevelTypeEnum.dungeon),
            new StaticLevelData("highblockhalls", new Point(.75, .40), LevelTypeEnum.mission),
            new StaticLevelData("obsidianpinnacle", new Point(.86, .25), LevelTypeEnum.mission),
            new StaticLevelData("underhalls", new Point(.87, .49), LevelTypeEnum.dungeon),
            new StaticLevelData("squidcoast", new Point(.15, .64), LevelTypeEnum.mission),
        };

        public readonly static MapImageData MAINLAND_MAP_IMAGE_DATA = new MapImageData {
            titleLookupString = "ArchIllagerRealm_name",
            titleBackupText = R.MAINLAND,
            mapImageSourcePath = "/Dungeons/Content/UI/Materials/MissionSelectMap/background/missionselect_map_center_xbox",
            backgroundColor = Colors.White,
            cropToRect = new Int32Rect(10, 83, 6136, 2975),
            levelData = MAINLAND_LEVEL_DATA!,
        };

        public readonly static StaticLevelData[] JUNGLE_AWAKENS_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("dingyjungle", new Point(.24, .44), LevelTypeEnum.mission),
            new StaticLevelData("overgrowntemple", new Point(.62, .15), LevelTypeEnum.mission),
            new StaticLevelData("bamboobluff", new Point(.65, .55), LevelTypeEnum.dungeon),
        };

        public readonly static MapImageData JUNGLE_AWAKENS_MAP_IMAGE_DATA = new MapImageData {
            titleLookupString = "TheJungleAwakens_name",
            titleBackupText = R.JUNGLE_AWAKENS,
            mapImageSourcePath = "/Dungeons/Content/UI/Materials/MissionSelectMap/background/islands/DLC_Jungle_Island",
            backgroundColor = Colors.LightCyan,
            cropToRect = new Int32Rect(0, 0, 2166, 1455),
            levelData = JUNGLE_AWAKENS_LEVEL_DATA!,
        };

        public readonly static StaticLevelData[] CREEPING_WINTER_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("frozenfjord", new Point(.24, .44), LevelTypeEnum.mission),
            new StaticLevelData("lonelyfortress", new Point(.62, .15), LevelTypeEnum.mission),
            new StaticLevelData("lostsettlement", new Point(.65, .55), LevelTypeEnum.dungeon),
        };

        public readonly static MapImageData CREEPING_WINTER_MAP_IMAGE_DATA = new MapImageData {
            titleLookupString = "TheCreepingWinter_name",
            titleBackupText = R.CREEPING_WINTER,
            mapImageSourcePath = "/Dungeons/Content/UI/Materials/MissionSelectMap/background/islands/DLC_Snowy_Island",
            backgroundColor = Colors.LightCyan,
            cropToRect = new Int32Rect(0, 0, 2211, 1437),
            levelData = CREEPING_WINTER_LEVEL_DATA!,
        };

        public readonly static StaticLevelData[] HOWLING_PEAKS_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("windsweptpeaks", new Point(.24, .44), LevelTypeEnum.mission),
            new StaticLevelData("galesanctum", new Point(.62, .15), LevelTypeEnum.mission),
            new StaticLevelData("endlessrampart", new Point(.65, .55), LevelTypeEnum.dungeon),
        };

        public readonly static MapImageData HOWLING_PEAKS_MAP_IMAGE_DATA = new MapImageData {
            titleLookupString = "TheHowlingPeaks_name",
            titleBackupText = R.HOWLING_PEAKS,
            mapImageSourcePath = "/Dungeons/Content/UI/Materials/MissionSelectMap/background/islands/Mountain_base_NOTPOTWO",
            backgroundColor = Colors.LightCyan,
            cropToRect = new Int32Rect(0, 0, 2466, 2414),
            levelData = HOWLING_PEAKS_LEVEL_DATA!,
        };

        public readonly static Dictionary<string, StaticLevelData> LEVEL_DATA_LOOKUP =
            MAINLAND_LEVEL_DATA
            .Concat(JUNGLE_AWAKENS_LEVEL_DATA)
            .Concat(CREEPING_WINTER_LEVEL_DATA)
            .Concat(HOWLING_PEAKS_LEVEL_DATA)
            .ToDictionary(data=>data.key);

        public readonly static List<MapImageData> ALL_MAP_IMAGE_DATA = new List<MapImageData>() {
            MAINLAND_MAP_IMAGE_DATA,
            JUNGLE_AWAKENS_MAP_IMAGE_DATA,
            CREEPING_WINTER_MAP_IMAGE_DATA,
            HOWLING_PEAKS_MAP_IMAGE_DATA,
        };
    }
}
