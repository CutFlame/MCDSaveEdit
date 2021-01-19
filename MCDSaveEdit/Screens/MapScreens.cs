#nullable enable

namespace MCDSaveEdit
{
    public class MainlandMapScreen : MapScreen
    {
        public MainlandMapScreen() : base(Constants.MAINLAND_LEVEL_DATA, Constants.MAINLAND_MAP_IMAGE_DATA)
        {
        }
    }

    public class JungleAwakensMapScreen : MapScreen
    {
        public JungleAwakensMapScreen() : base(Constants.JUNGLE_AWAKENS_LEVEL_DATA, Constants.JUNGLE_AWAKENS_MAP_IMAGE_DATA)
        {
        }
    }

    public class CreepingWinterMapScreen : MapScreen
    {
        public CreepingWinterMapScreen() : base(Constants.CREEPING_WINTER_LEVEL_DATA, Constants.CREEPING_WINTER_MAP_IMAGE_DATA)
        {
        }
    }
    public class HowlingPeaksMapScreen : MapScreen
    {
        public HowlingPeaksMapScreen() : base(Constants.HOWLING_PEAKS_LEVEL_DATA, Constants.HOWLING_PEAKS_MAP_IMAGE_DATA)
        {
        }
    }
}
