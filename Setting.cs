using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;

namespace BuildingReviver
{
    [FileLocation("ModsSettings/BuildingReviver/BuildingReviver")]
    [SettingsUIGroupOrder(kMainGroup, kTimingGroup, kStatsGroup)]
    [SettingsUIShowGroupName(kMainGroup, kTimingGroup, kStatsGroup)]
    public class Setting : ModSetting
    {
        public const string kSection = "Main";
        public const string kMainGroup = "Options";
        public const string kTimingGroup = "Timing";
        public const string kStatsGroup = "Statistics";

        public Setting(IMod mod) : base(mod)
        {
            SetDefaults();
        }

        [SettingsUISection(kSection, kMainGroup)]
        public bool EnableMod { get; set; }

        [SettingsUIDisableByCondition(typeof(Setting), nameof(IsModDisabled))]
        [SettingsUISection(kSection, kMainGroup)]
        public bool ReviveAbandoned { get; set; }

        [SettingsUIDisableByCondition(typeof(Setting), nameof(IsModDisabled))]
        [SettingsUISection(kSection, kMainGroup)]
        public bool ReviveCondemned { get; set; }

        [SettingsUIDisableByCondition(typeof(Setting), nameof(IsModDisabled))]
        [SettingsUISlider(min = 1, max = BuildingReviverSystem.kMaxSweepsPerDay, step = 1, scalarMultiplier = 1, unit = Unit.kInteger)]
        [SettingsUISection(kSection, kTimingGroup)]
        public int SweepsPerDay { get; set; }

        [SettingsUISection(kSection, kStatsGroup)]
        public string TotalRevived => BuildingReviverSystem.TotalRevived.ToString();

        [SettingsUISection(kSection, kStatsGroup)]
        public string TotalAbandoned => BuildingReviverSystem.TotalAbandoned.ToString();

        [SettingsUISection(kSection, kStatsGroup)]
        public string TotalCondemned => BuildingReviverSystem.TotalCondemned.ToString();

        [SettingsUIButton]
        [SettingsUIConfirmation]
        [SettingsUISection(kSection, kStatsGroup)]
        public bool ResetStatistics
        {
            set { BuildingReviverSystem.ResetStatistics(); }
        }

        public bool IsModDisabled() => !EnableMod;

        public sealed override void SetDefaults()
        {
            EnableMod = true;
            ReviveAbandoned = true;
            ReviveCondemned = false;
            SweepsPerDay = 16;
        }
    }
}
