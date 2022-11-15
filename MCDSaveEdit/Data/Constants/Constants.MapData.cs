using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Services; //TODO: remove this reference
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
#nullable enable

namespace MCDSaveEdit.Data
{
    public static partial class Constants {
        public readonly static StaticLevelData[] DEBUG_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData(".00", new Vector2(0.00f, 0.00f), LevelTypeEnum.mission),
            new StaticLevelData(".05", new Vector2(0.05f, 0.05f), LevelTypeEnum.mission),
            new StaticLevelData(".10", new Vector2(0.10f, 0.10f), LevelTypeEnum.mission),
            new StaticLevelData(".15", new Vector2(0.15f, 0.15f), LevelTypeEnum.mission),
            new StaticLevelData(".20", new Vector2(0.20f, 0.20f), LevelTypeEnum.mission),
            new StaticLevelData(".25", new Vector2(0.25f, 0.25f), LevelTypeEnum.mission),
            new StaticLevelData(".30", new Vector2(0.30f, 0.30f), LevelTypeEnum.mission),
            new StaticLevelData(".35", new Vector2(0.35f, 0.35f), LevelTypeEnum.mission),
            new StaticLevelData(".40", new Vector2(0.40f, 0.40f), LevelTypeEnum.mission),
            new StaticLevelData(".45", new Vector2(0.45f, 0.45f), LevelTypeEnum.mission),
            new StaticLevelData(".50", new Vector2(0.50f, 0.50f), LevelTypeEnum.mission),
            new StaticLevelData(".55", new Vector2(0.55f, 0.55f), LevelTypeEnum.mission),
            new StaticLevelData(".60", new Vector2(0.60f, 0.60f), LevelTypeEnum.mission),
            new StaticLevelData(".65", new Vector2(0.65f, 0.65f), LevelTypeEnum.mission),
            new StaticLevelData(".70", new Vector2(0.70f, 0.70f), LevelTypeEnum.mission),
            new StaticLevelData(".75", new Vector2(0.75f, 0.75f), LevelTypeEnum.mission),
            new StaticLevelData(".80", new Vector2(0.80f, 0.80f), LevelTypeEnum.mission),
            new StaticLevelData(".85", new Vector2(0.85f, 0.85f), LevelTypeEnum.mission),
            new StaticLevelData(".90", new Vector2(0.90f, 0.90f), LevelTypeEnum.mission),
            new StaticLevelData(".95", new Vector2(0.95f, 0.95f), LevelTypeEnum.mission),
            new StaticLevelData("1.0", new Vector2(1.00f, 1.00f), LevelTypeEnum.mission),
        };

        public readonly static StaticLevelData[] MAINLAND_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("squidcoast", new Vector2(.15f, .64f), LevelTypeEnum.mission, alwaysAvailable: true),
            new StaticLevelData("creeperwoods", new Vector2(.28f, .40f), LevelTypeEnum.mission, alwaysAvailable: true),
            new StaticLevelData("creepycrypt", new Vector2(.19f, .37f), LevelTypeEnum.dungeon),
            new StaticLevelData("mooshroomisland", new Vector2(.26f, .24f), LevelTypeEnum.dungeon),
            new StaticLevelData("soggycave", new Vector2(.27f, .65f), LevelTypeEnum.dungeon),
            new StaticLevelData("soggyswamp", new Vector2(.29f, .74f), LevelTypeEnum.mission),
            new StaticLevelData("mooncorecaverns", new Vector2(.44f, .35f), LevelTypeEnum.mission),
            new StaticLevelData("cacticanyon", new Vector2(.42f, .56f), LevelTypeEnum.mission),
            new StaticLevelData("pumpkinpastures", new Vector2(.46f, .70f), LevelTypeEnum.mission),
            new StaticLevelData("archhaven", new Vector2(.57f, .73f), LevelTypeEnum.dungeon),
            new StaticLevelData("deserttemple", new Vector2(.59f, .45f), LevelTypeEnum.mission),
            new StaticLevelData("fieryforge", new Vector2(.63f, .20f), LevelTypeEnum.mission),
            new StaticLevelData("lowertemple", new Vector2(.67f, .59f), LevelTypeEnum.dungeon),
            new StaticLevelData("highblockhalls", new Vector2(.75f, .40f), LevelTypeEnum.mission),
            new StaticLevelData("obsidianpinnacle", new Vector2(.86f, .25f), LevelTypeEnum.mission),
            new StaticLevelData("underhalls", new Vector2(.87f, .49f), LevelTypeEnum.dungeon),
        };

        public readonly static MapImageData MAINLAND_MAP_IMAGE_DATA = new MapImageData {
            titleLookupString = "ArchIllagerRealm_name",
            titleBackupText = R.MAINLAND,
            mapImageSourcePath = "/Dungeons/Content/UI/Materials/MissionSelectMap/background/missionselect_map_center_xbox",
            backgroundColor = new ColorTuple(0xFF, 0xFF, 0xFF, 0xFF),
            cropToRect = new RectTuple(10, 83, 6136, 2975),
            levelData = MAINLAND_LEVEL_DATA!,
        };

        public readonly static StaticLevelData[] JUNGLE_AWAKENS_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("dingyjungle", new Vector2(.24f, .44f), LevelTypeEnum.mission, alwaysAvailable: true),
            new StaticLevelData("overgrowntemple", new Vector2(.62f, .15f), LevelTypeEnum.mission),
            new StaticLevelData("bamboobluff", new Vector2(.65f, .55f), LevelTypeEnum.dungeon),
        };

        public readonly static MapImageData JUNGLE_AWAKENS_MAP_IMAGE_DATA = new MapImageData {
            titleLookupString = "TheJungleAwakens_name",
            titleBackupText = R.JUNGLE_AWAKENS,
            mapImageSourcePath = "/Dungeons/Content/UI/Materials/MissionSelectMap/background/islands/DLC_Jungle_Island_low",
            backgroundColor = new ColorTuple(0xFF, 0xE0, 0xFF, 0xFF),
            cropToRect = null,
            levelData = JUNGLE_AWAKENS_LEVEL_DATA!,
        };

        public readonly static StaticLevelData[] CREEPING_WINTER_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("frozenfjord", new Vector2(.24f, .44f), LevelTypeEnum.mission, alwaysAvailable: true),
            new StaticLevelData("lonelyfortress", new Vector2(.62f, .15f), LevelTypeEnum.mission),
            new StaticLevelData("lostsettlement", new Vector2(.65f, .55f), LevelTypeEnum.dungeon),
        };

        public readonly static MapImageData CREEPING_WINTER_MAP_IMAGE_DATA = new MapImageData {
            titleLookupString = "TheCreepingWinter_name",
            titleBackupText = R.CREEPING_WINTER,
            mapImageSourcePath = "/Dungeons/Content/UI/Materials/MissionSelectMap/background/islands/DLC_Snowy_Island_low",
            backgroundColor = new ColorTuple(0xFF, 0xE0, 0xFF, 0xFF),
            cropToRect = null,
            levelData = CREEPING_WINTER_LEVEL_DATA!,
        };

        public readonly static StaticLevelData[] HOWLING_PEAKS_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("windsweptpeaks", new Vector2(.24f, .44f), LevelTypeEnum.mission, alwaysAvailable: true),
            new StaticLevelData("galesanctum", new Vector2(.62f, .15f), LevelTypeEnum.mission),
            new StaticLevelData("gauntletgales", new Vector2(.62f, .25f), LevelTypeEnum.mission),
            new StaticLevelData("endlessrampart", new Vector2(.65f, .55f), LevelTypeEnum.dungeon),
        };

        public readonly static MapImageData HOWLING_PEAKS_MAP_IMAGE_DATA = new MapImageData {
            titleLookupString = "TheHowlingPeaks_name",
            titleBackupText = R.HOWLING_PEAKS,
            mapImageSourcePath = "/Dungeons/Content/UI/Materials/MissionSelectMap/background/islands/Mountain_base_low",
            backgroundColor = new ColorTuple(0xFF, 0xE0, 0xFF, 0xFF),
            cropToRect = null,
            levelData = HOWLING_PEAKS_LEVEL_DATA!,
        };

        public readonly static StaticLevelData[] OCEANS_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("coralrise", new Vector2(.24f, .44f), LevelTypeEnum.mission, alwaysAvailable: true),
            new StaticLevelData("abyssalmonument", new Vector2(.62f, .15f), LevelTypeEnum.mission),
            new StaticLevelData("radiantravine", new Vector2(.65f, .55f), LevelTypeEnum.dungeon),
        };

        public readonly static MapImageData OCEANS_MAP_IMAGE_DATA = new MapImageData {
            titleLookupString = "Oceans_name",
            titleBackupText = "The Hidden Depths", //TODO: put in to resources
            mapImageSourcePath = "/Dungeons/Content/UI/Materials/MissionSelectMap/background/islands/DLC_Oceans_Island",
            backgroundColor = new ColorTuple(0xFF, 0xE0, 0xFF, 0xFF),
            cropToRect = new RectTuple(0, 0, 2900, 2061),
            levelData = OCEANS_LEVEL_DATA!,
        };

        public readonly static StaticLevelData[] NETHER_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("netherwastes", new Vector2(.62f, .15f), LevelTypeEnum.mission, alwaysAvailable: true),
            new StaticLevelData("warpedforest", new Vector2(.24f, .44f), LevelTypeEnum.mission),
            new StaticLevelData("basaltdeltas", new Vector2(.65f, .55f), LevelTypeEnum.mission),
            new StaticLevelData("crimsonforest", new Vector2(.65f, .65f), LevelTypeEnum.dungeon),
            new StaticLevelData("netherfortress", new Vector2(.65f, .75f), LevelTypeEnum.dungeon),
            new StaticLevelData("soulsandvalley", new Vector2(.65f, .85f), LevelTypeEnum.dungeon),
        };

        public readonly static MapImageData NETHER_MAP_IMAGE_DATA = new MapImageData {
            titleLookupString = "Nether_name",
            titleBackupText = "Nether", //TODO: put in to resources
            mapImageSourcePath = "/Dungeons/Content/Content_DLC4/UI/Materials/MissionSelectMap/DLC_NetherIsland",
            backgroundColor = new ColorTuple(0xFF, 0xC7, 0xB6, 0x99),
            cropToRect = new RectTuple(0, 0, 3016, 1646),
            levelData = NETHER_LEVEL_DATA!,
        };

        public readonly static StaticLevelData[] STRONGHOLD_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("thestronghold", new Vector2(.50f, .50f), LevelTypeEnum.mission, alwaysAvailable: true),
        };

        public readonly static MapImageData STRONGHOLD_MAP_IMAGE_DATA = new MapImageData {
            titleLookupString = "thestronghold_name",
            titleBackupText = "The Stronghold", //TODO: put in to resources
            mapImageSourcePath = "/Dungeons/Content/Content_DLC6/UI/Materials/MissionSelectMap/DLC_End_Stronghold_Island",
            backgroundColor = new ColorTuple(0xFF, 0xC7, 0xB6, 0x99),
            cropToRect = new RectTuple(0,0,1093,1200),
            levelData = STRONGHOLD_LEVEL_DATA!,
        };

        public readonly static StaticLevelData[] THE_END_LEVEL_DATA = new StaticLevelData[] {
            new StaticLevelData("blightedcitadel", new Vector2(.24f, .44f), LevelTypeEnum.mission),
            new StaticLevelData("enderwilds", new Vector2(.62f, .15f), LevelTypeEnum.mission),
        };

        public readonly static MapImageData THE_END_MAP_IMAGE_DATA = new MapImageData {
            titleLookupString = "End_name",
            titleBackupText = "Echoing Void", //TODO: put in to resources
            mapImageSourcePath = "/Dungeons/Content/Content_DLC6/UI/Materials/MissionSelectMap/DLC_End",
            backgroundColor = new ColorTuple(0xFF, 0xC7, 0xB6, 0x99),
            cropToRect = new RectTuple(0, 0, 2500, 1951),
            levelData = THE_END_LEVEL_DATA!,
        };

        public readonly static Dictionary<string, StaticLevelData> LEVEL_DATA_LOOKUP =
            MAINLAND_LEVEL_DATA
            .Concat(JUNGLE_AWAKENS_LEVEL_DATA)
            .Concat(CREEPING_WINTER_LEVEL_DATA)
            .Concat(HOWLING_PEAKS_LEVEL_DATA)
            .Concat(OCEANS_LEVEL_DATA)
            .Concat(NETHER_LEVEL_DATA)
            .Concat(STRONGHOLD_LEVEL_DATA)
            .Concat(THE_END_LEVEL_DATA)
            .ToDictionary(data=>data.key);

        public readonly static List<MapImageData> ALL_MAP_IMAGE_DATA = new List<MapImageData>() {
            MAINLAND_MAP_IMAGE_DATA,
            JUNGLE_AWAKENS_MAP_IMAGE_DATA,
            CREEPING_WINTER_MAP_IMAGE_DATA,
            HOWLING_PEAKS_MAP_IMAGE_DATA,
            OCEANS_MAP_IMAGE_DATA,
            NETHER_MAP_IMAGE_DATA,
            STRONGHOLD_MAP_IMAGE_DATA,
            THE_END_MAP_IMAGE_DATA,
        };
    }
}
