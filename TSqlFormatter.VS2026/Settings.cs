using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell.Settings;
using System;
using Microsoft.VisualStudio.Shell;

namespace TSqlFormatter.VS2026
{
    public class Settings
    {
        private const string CollectionPath = "TSqlFormatter";
        private readonly WritableSettingsStore _settingsStore;

        // Setting properties with defaults
        public string IndentString { get; set; } = "\t";
        public int SpacesPerTab { get; set; } = 4;
        public int MaxLineWidth { get; set; } = 999;
        public bool ExpandCommaLists { get; set; } = true;
        public bool TrailingCommas { get; set; } = false;
        public bool ExpandBooleanExpressions { get; set; } = true;
        public bool ExpandCaseStatements { get; set; } = true;
        public bool ExpandBetweenConditions { get; set; } = true;
        public bool ExpandInLists { get; set; } = true;
        public bool BreakJoinOnSections { get; set; } = false;
        public bool UppercaseKeywords { get; set; } = true;
        public bool SpaceAfterExpandedComma { get; set; } = false;
        public bool KeywordStandardization { get; set; } = false;
        public int NewClauseLineBreaks { get; set; } = 1;
        public int NewStatementLineBreaks { get; set; } = 2;

        public Settings()
        {
            try
            {
                ThreadHelper.ThrowIfNotOnUIThread();
                var shellSettingsManager = new ShellSettingsManager(ServiceProvider.GlobalProvider);
                _settingsStore = shellSettingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);

                if (!_settingsStore.CollectionExists(CollectionPath))
                {
                    _settingsStore.CreateCollection(CollectionPath);
                }

                LoadSettings();
            }
            catch
            {
                // Use defaults if settings cannot be loaded
            }
        }

        public void LoadSettings()
        {
            try
            {
                if (_settingsStore == null) return;

                if (_settingsStore.PropertyExists(CollectionPath, nameof(IndentString)))
                    IndentString = _settingsStore.GetString(CollectionPath, nameof(IndentString));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(SpacesPerTab)))
                    SpacesPerTab = _settingsStore.GetInt32(CollectionPath, nameof(SpacesPerTab));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(MaxLineWidth)))
                    MaxLineWidth = _settingsStore.GetInt32(CollectionPath, nameof(MaxLineWidth));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(ExpandCommaLists)))
                    ExpandCommaLists = _settingsStore.GetBoolean(CollectionPath, nameof(ExpandCommaLists));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(TrailingCommas)))
                    TrailingCommas = _settingsStore.GetBoolean(CollectionPath, nameof(TrailingCommas));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(ExpandBooleanExpressions)))
                    ExpandBooleanExpressions = _settingsStore.GetBoolean(CollectionPath, nameof(ExpandBooleanExpressions));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(ExpandCaseStatements)))
                    ExpandCaseStatements = _settingsStore.GetBoolean(CollectionPath, nameof(ExpandCaseStatements));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(ExpandBetweenConditions)))
                    ExpandBetweenConditions = _settingsStore.GetBoolean(CollectionPath, nameof(ExpandBetweenConditions));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(ExpandInLists)))
                    ExpandInLists = _settingsStore.GetBoolean(CollectionPath, nameof(ExpandInLists));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(BreakJoinOnSections)))
                    BreakJoinOnSections = _settingsStore.GetBoolean(CollectionPath, nameof(BreakJoinOnSections));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(UppercaseKeywords)))
                    UppercaseKeywords = _settingsStore.GetBoolean(CollectionPath, nameof(UppercaseKeywords));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(SpaceAfterExpandedComma)))
                    SpaceAfterExpandedComma = _settingsStore.GetBoolean(CollectionPath, nameof(SpaceAfterExpandedComma));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(KeywordStandardization)))
                    KeywordStandardization = _settingsStore.GetBoolean(CollectionPath, nameof(KeywordStandardization));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(NewClauseLineBreaks)))
                    NewClauseLineBreaks = _settingsStore.GetInt32(CollectionPath, nameof(NewClauseLineBreaks));

                if (_settingsStore.PropertyExists(CollectionPath, nameof(NewStatementLineBreaks)))
                    NewStatementLineBreaks = _settingsStore.GetInt32(CollectionPath, nameof(NewStatementLineBreaks));
            }
            catch
            {
                // Use defaults if settings cannot be loaded
            }
        }

        public void SaveSettings()
        {
            try
            {
                if (_settingsStore == null) return;

                _settingsStore.SetString(CollectionPath, nameof(IndentString), IndentString);
                _settingsStore.SetInt32(CollectionPath, nameof(SpacesPerTab), SpacesPerTab);
                _settingsStore.SetInt32(CollectionPath, nameof(MaxLineWidth), MaxLineWidth);
                _settingsStore.SetBoolean(CollectionPath, nameof(ExpandCommaLists), ExpandCommaLists);
                _settingsStore.SetBoolean(CollectionPath, nameof(TrailingCommas), TrailingCommas);
                _settingsStore.SetBoolean(CollectionPath, nameof(ExpandBooleanExpressions), ExpandBooleanExpressions);
                _settingsStore.SetBoolean(CollectionPath, nameof(ExpandCaseStatements), ExpandCaseStatements);
                _settingsStore.SetBoolean(CollectionPath, nameof(ExpandBetweenConditions), ExpandBetweenConditions);
                _settingsStore.SetBoolean(CollectionPath, nameof(ExpandInLists), ExpandInLists);
                _settingsStore.SetBoolean(CollectionPath, nameof(BreakJoinOnSections), BreakJoinOnSections);
                _settingsStore.SetBoolean(CollectionPath, nameof(UppercaseKeywords), UppercaseKeywords);
                _settingsStore.SetBoolean(CollectionPath, nameof(SpaceAfterExpandedComma), SpaceAfterExpandedComma);
                _settingsStore.SetBoolean(CollectionPath, nameof(KeywordStandardization), KeywordStandardization);
                _settingsStore.SetInt32(CollectionPath, nameof(NewClauseLineBreaks), NewClauseLineBreaks);
                _settingsStore.SetInt32(CollectionPath, nameof(NewStatementLineBreaks), NewStatementLineBreaks);
            }
            catch
            {
                // Fail silently
            }
        }

        public TSqlFormatter.Formatters.TSqlStandardFormatterOptions GetFormatterOptions()
        {
            return new TSqlFormatter.Formatters.TSqlStandardFormatterOptions
            {
                IndentString = IndentString,
                SpacesPerTab = SpacesPerTab,
                MaxLineWidth = MaxLineWidth,
                ExpandCommaLists = ExpandCommaLists,
                TrailingCommas = TrailingCommas,
                ExpandBooleanExpressions = ExpandBooleanExpressions,
                ExpandCaseStatements = ExpandCaseStatements,
                ExpandBetweenConditions = ExpandBetweenConditions,
                ExpandInLists = ExpandInLists,
                BreakJoinOnSections = BreakJoinOnSections,
                UppercaseKeywords = UppercaseKeywords,
                SpaceAfterExpandedComma = SpaceAfterExpandedComma,
                KeywordStandardization = KeywordStandardization,
                NewClauseLineBreaks = NewClauseLineBreaks,
                NewStatementLineBreaks = NewStatementLineBreaks
            };
        }
    }
}