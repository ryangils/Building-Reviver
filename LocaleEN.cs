using System.Collections.Generic;
using Colossal;

namespace BuildingReviver
{
    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(
            IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Building Reviver" },

                { m_Setting.GetOptionTabLocaleID(Setting.kSection), "Main" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kMainGroup), "Options" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kTimingGroup), "Timing" },
                { m_Setting.GetOptionGroupLocaleID(Setting.kStatsGroup), "Statistics" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.EnableMod)), "Enable Building Reviver" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.EnableMod)), "Master switch. When off, no buildings are revived automatically." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ReviveAbandoned)), "Revive abandoned buildings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ReviveAbandoned)), "Clears the abandoned status, resets the building's condition, and puts the property back on the market so new occupants can move in. If the underlying problem (land value, services, rent) persists, the building can be abandoned again." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ReviveCondemned)), "Rescue condemned buildings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ReviveCondemned)), "Removes the condemned status from buildings marked for demolition. If the cause (e.g. invalid zoning) persists, the building can be condemned again." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.SweepsPerDay)), "Sweeps per in-game day" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.SweepsPerDay)), "How often the mod scans the city for buildings to revive. Higher values rescue buildings sooner; lower values batch the work up." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TotalRevived)), "Buildings revived this session" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TotalRevived)), "Number of buildings this mod has revived since the game was started." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TotalAbandoned)), "— abandoned" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TotalAbandoned)), "Abandoned buildings revived this session." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.TotalCondemned)), "— condemned" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.TotalCondemned)), "Condemned buildings rescued this session." },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetStatistics)), "Reset statistics" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetStatistics)), "Set all revival counters back to zero." },
                { m_Setting.GetOptionWarningLocaleID(nameof(Setting.ResetStatistics)), "Reset all revival counters to zero?" },
            };
        }

        public void Unload()
        {
        }
    }
}
