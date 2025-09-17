using Microsoft.VisualStudio.PlatformUI;
using System.Windows;

namespace TSqlFormatter.SSMS
{
    public partial class OptionsDialogWindow : DialogWindow
    {
        private Settings _settings;

        public OptionsDialogWindow(Settings settings)
        {
            _settings = settings;
            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            // Load settings into controls

            // Indentation
            if (_settings.IndentString == "\t")
            {
                rbTabs.IsChecked = true;
            }
            else
            {
                rbSpaces.IsChecked = true;
                txtSpacesPerTab.Text = _settings.IndentString.Length.ToString();
            }

            txtMaxLineWidth.Text = _settings.MaxLineWidth.ToString();

            // Expansion options
            chkExpandCommaLists.IsChecked = _settings.ExpandCommaLists;
            chkTrailingCommas.IsChecked = _settings.TrailingCommas;
            chkExpandBooleanExpressions.IsChecked = _settings.ExpandBooleanExpressions;
            chkExpandCaseStatements.IsChecked = _settings.ExpandCaseStatements;
            chkExpandBetweenConditions.IsChecked = _settings.ExpandBetweenConditions;
            chkExpandInLists.IsChecked = _settings.ExpandInLists;
            chkBreakJoinOnSections.IsChecked = _settings.BreakJoinOnSections;
            chkSpaceAfterExpandedComma.IsChecked = _settings.SpaceAfterExpandedComma;

            // Line breaks
            txtNewClauseLineBreaks.Text = _settings.NewClauseLineBreaks.ToString();
            txtNewStatementLineBreaks.Text = _settings.NewStatementLineBreaks.ToString();

            // Case options
            chkUppercaseKeywords.IsChecked = _settings.UppercaseKeywords;
            chkStandardizeKeywords.IsChecked = _settings.KeywordStandardization;
        }

        private void SaveSettings()
        {

            // Indentation
            if (rbTabs.IsChecked == true)
            {
                _settings.IndentString = "\t";
            }
            else
            {
                int spaces = int.TryParse(txtSpacesPerTab.Text, out int s) ? s : 4;
                _settings.IndentString = new string(' ', spaces);
            }

            _settings.MaxLineWidth = int.TryParse(txtMaxLineWidth.Text, out int maxWidth) ? maxWidth : 999;

            // Expansion options
            _settings.ExpandCommaLists = chkExpandCommaLists.IsChecked == true;
            _settings.TrailingCommas = chkTrailingCommas.IsChecked == true;
            _settings.ExpandBooleanExpressions = chkExpandBooleanExpressions.IsChecked == true;
            _settings.ExpandCaseStatements = chkExpandCaseStatements.IsChecked == true;
            _settings.ExpandBetweenConditions = chkExpandBetweenConditions.IsChecked == true;
            _settings.ExpandInLists = chkExpandInLists.IsChecked == true;
            _settings.BreakJoinOnSections = chkBreakJoinOnSections.IsChecked == true;
            _settings.SpaceAfterExpandedComma = chkSpaceAfterExpandedComma.IsChecked == true;

            // Line breaks
            _settings.NewClauseLineBreaks = int.TryParse(txtNewClauseLineBreaks.Text, out int clauseBreaks) ? clauseBreaks : 1;
            _settings.NewStatementLineBreaks = int.TryParse(txtNewStatementLineBreaks.Text, out int statementBreaks) ? statementBreaks : 2;

            // Case options
            _settings.UppercaseKeywords = chkUppercaseKeywords.IsChecked == true;
            _settings.KeywordStandardization = chkStandardizeKeywords.IsChecked == true;

            _settings.SaveSettings();
        }

        private void OnDefaultsClick(object sender, RoutedEventArgs e)
        {
            // Reset to defaults
            rbSpaces.IsChecked = true;
            txtSpacesPerTab.Text = "4";
            txtMaxLineWidth.Text = "999";

            chkExpandCommaLists.IsChecked = true;
            chkTrailingCommas.IsChecked = false;
            chkExpandBooleanExpressions.IsChecked = true;
            chkExpandCaseStatements.IsChecked = true;
            chkExpandBetweenConditions.IsChecked = true;
            chkExpandInLists.IsChecked = true;
            chkBreakJoinOnSections.IsChecked = false;
            chkSpaceAfterExpandedComma.IsChecked = false;

            txtNewClauseLineBreaks.Text = "1";
            txtNewStatementLineBreaks.Text = "2";

            chkUppercaseKeywords.IsChecked = true;
            chkStandardizeKeywords.IsChecked = false;
        }

        private void OnOKClick(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            DialogResult = true;
            Close();
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}