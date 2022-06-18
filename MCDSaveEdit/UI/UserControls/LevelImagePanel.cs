using MCDSaveEdit.Save.Models.Enums;
using MCDSaveEdit.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
#nullable enable

namespace MCDSaveEdit.UI
{
    public class LevelImagePanel: Grid
    {
        public const double IMAGE_RADIUS = 18;

        private static readonly BitmapImage? _missionImageSource = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/mission_marker_front");
        private static readonly BitmapImage? _dungeonImageSource = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/shield_dungeon");

        private static readonly BitmapImage? _lockedDungeonImageSource = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/locked_dungeons");
        private static readonly BitmapImage? _incompleteMissionImageSource = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/mission_marker_iconSword_A");
        private static readonly BitmapImage? _unlockedDungeonImageSource = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/MissionSelectMap/marker/icon_dungeon");

        private static readonly BitmapImage? _difficulty1ImageSource = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level1");
        private static readonly BitmapImage? _difficulty2ImageSource = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level2");
        private static readonly BitmapImage? _difficulty3ImageSource = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level3");
        private static readonly BitmapImage? _difficulty4ImageSource = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level4");
        private static readonly BitmapImage? _difficulty5ImageSource = ImageResolver.instance.imageSource("/Dungeons/Content/UI/Materials/Difficulty/asset_mapnode_done_level5");

        public static void preload() { }

        private readonly Image _levelTypeImage = new Image();
        private readonly Image _levelDifficultyImage = new Image();

        private LevelTypeEnum _levelType;
        public LevelTypeEnum levelType { get => _levelType; set { _levelType = value; updateLevelTypeUI(); } }

        public LevelImagePanel()
        {
            this.Children.Add(_levelTypeImage);
            this.Children.Add(_levelDifficultyImage);

            _levelDifficultyImage.HorizontalAlignment = HorizontalAlignment.Center;
            _levelDifficultyImage.VerticalAlignment = VerticalAlignment.Center;
        }

        public void updateLockedUI(bool locked)
        {
            if (locked)
            {
                if (_levelType == LevelTypeEnum.dungeon)
                {
                    _levelDifficultyImage.Source = _lockedDungeonImageSource;
                }
                else
                {
                    _levelDifficultyImage.Source = _incompleteMissionImageSource;
                }
            }
            else
            {
                _levelDifficultyImage.Source = _unlockedDungeonImageSource;
            }
            _levelDifficultyImage.Width = IMAGE_RADIUS;
            _levelDifficultyImage.Height = IMAGE_RADIUS;
        }

        public void updateDifficultyLevelUI(uint difficulty)
        {
            _levelDifficultyImage.Source = imageForDifficulty(difficulty);
            _levelDifficultyImage.Width = IMAGE_RADIUS * .95;
            _levelDifficultyImage.Height = IMAGE_RADIUS * .95;
        }

        private BitmapImage? imageForDifficulty(uint difficulty)
        {
            switch (difficulty)
            {
                case 1: return _difficulty1ImageSource;
                case 2: return _difficulty2ImageSource;
                case 3: return _difficulty3ImageSource;
                case 4: return _difficulty4ImageSource;
                default: return _difficulty5ImageSource;
            }
        }

        private void updateLevelTypeUI()
        {
            _levelTypeImage.Source = imageForLevelType(_levelType);
        }

        private BitmapImage? imageForLevelType(LevelTypeEnum levelType)
        {
            switch (_levelType)
            {
                case LevelTypeEnum.mission: return _missionImageSource;
                case LevelTypeEnum.dungeon: return _dungeonImageSource;
                default: throw new NotImplementedException();
            }
        }
    }
}
