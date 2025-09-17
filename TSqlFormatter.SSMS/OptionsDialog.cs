using System;
using System.Drawing;
using System.Windows.Forms;

namespace TSqlFormatter.SSMS
{
    public partial class OptionsDialog : Form
    {
        private Settings _settings;

        // Controls
        private TabControl tabControl;
        private TabPage tabFormatting;
        private TabPage tabCase;

        // Formatting Tab Controls
        private RadioButton radioIndentTabs;
        private RadioButton radioIndentSpaces;
        private NumericUpDown numSpacesPerTab;
        private NumericUpDown numMaxLineWidth;
        private CheckBox chkExpandCommaLists;
        private CheckBox chkTrailingCommas;
        private CheckBox chkExpandBooleanExpressions;
        private CheckBox chkExpandCaseStatements;
        private CheckBox chkExpandBetweenConditions;
        private CheckBox chkExpandInLists;
        private CheckBox chkBreakJoinOnSections;
        private CheckBox chkSpaceAfterExpandedComma;
        private NumericUpDown numNewClauseLineBreaks;
        private NumericUpDown numNewStatementLineBreaks;

        // Case Tab Controls
        private CheckBox chkUppercaseKeywords;
        private CheckBox chkKeywordStandardization;

        // Dialog Buttons
        private Button btnOK;
        private Button btnCancel;
        private Button btnDefaults;

        public OptionsDialog(Settings settings)
        {
            _settings = settings;
            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form settings
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 600);
            this.Text = "T-SQL Formatter Options (v5 - Properly Sized)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Create tab control
            tabControl = new TabControl();
            tabControl.Location = new Point(12, 12);
            tabControl.Size = new Size(926, 530);
            tabControl.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Create tabs
            CreateFormattingTab();
            CreateCaseTab();

            tabControl.TabPages.Add(tabFormatting);
            tabControl.TabPages.Add(tabCase);
            this.Controls.Add(tabControl);

            // Create buttons at the bottom with proper spacing
            int buttonY = this.ClientSize.Height - 45;
            int buttonHeight = 28;

            btnDefaults = new Button();
            btnDefaults.Text = "Defaults";
            btnDefaults.Size = new Size(100, buttonHeight);
            btnDefaults.Location = new Point(12, buttonY);
            btnDefaults.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnDefaults.Click += BtnDefaults_Click;
            this.Controls.Add(btnDefaults);

            btnCancel = new Button();
            btnCancel.Text = "Cancel";
            btnCancel.Size = new Size(100, buttonHeight);
            btnCancel.Location = new Point(this.ClientSize.Width - 185, buttonY);
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            this.Controls.Add(btnCancel);

            btnOK = new Button();
            btnOK.Text = "OK";
            btnOK.Size = new Size(100, buttonHeight);
            btnOK.Location = new Point(this.ClientSize.Width - 90, buttonY);
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Click += BtnOK_Click;
            this.Controls.Add(btnOK);

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void CreateFormattingTab()
        {
            tabFormatting = new TabPage("Formatting");
            tabFormatting.AutoScroll = false;

            // No panel needed - add controls directly to tab page

            int y = 10;
            int leftMargin = 10;

            // Indentation Section
            Label lblIndent = new Label();
            lblIndent.Text = "Indentation:";
            lblIndent.Font = new Font(lblIndent.Font, FontStyle.Bold);
            lblIndent.Location = new Point(leftMargin, y);
            lblIndent.AutoSize = true;
            tabFormatting.Controls.Add(lblIndent);
            y += 22;

            radioIndentTabs = new RadioButton();
            radioIndentTabs.Text = "Tabs";
            radioIndentTabs.Location = new Point(leftMargin + 15, y);
            radioIndentTabs.Size = new Size(60, 20);
            radioIndentTabs.Checked = true;
            tabFormatting.Controls.Add(radioIndentTabs);

            radioIndentSpaces = new RadioButton();
            radioIndentSpaces.Text = "Spaces";
            radioIndentSpaces.Location = new Point(leftMargin + 85, y);
            radioIndentSpaces.Size = new Size(70, 20);
            tabFormatting.Controls.Add(radioIndentSpaces);
            y += 22;

            Label lblSpacesPerTab = new Label();
            lblSpacesPerTab.Text = "Spaces per Tab:";
            lblSpacesPerTab.Location = new Point(leftMargin + 15, y + 2);
            lblSpacesPerTab.Size = new Size(95, 20);
            tabFormatting.Controls.Add(lblSpacesPerTab);

            numSpacesPerTab = new NumericUpDown();
            numSpacesPerTab.Location = new Point(leftMargin + 115, y);
            numSpacesPerTab.Size = new Size(50, 20);
            numSpacesPerTab.Minimum = 1;
            numSpacesPerTab.Maximum = 10;
            numSpacesPerTab.Value = 4;
            tabFormatting.Controls.Add(numSpacesPerTab);
            y += 22;

            Label lblMaxWidth = new Label();
            lblMaxWidth.Text = "Max Line Width:";
            lblMaxWidth.Location = new Point(leftMargin + 15, y + 2);
            lblMaxWidth.Size = new Size(95, 20);
            tabFormatting.Controls.Add(lblMaxWidth);

            numMaxLineWidth = new NumericUpDown();
            numMaxLineWidth.Location = new Point(leftMargin + 115, y);
            numMaxLineWidth.Size = new Size(60, 20);
            numMaxLineWidth.Minimum = 50;
            numMaxLineWidth.Maximum = 999;
            numMaxLineWidth.Value = 999;
            tabFormatting.Controls.Add(numMaxLineWidth);
            y += 30;

            // Expansion Options Section
            Label lblExpansion = new Label();
            lblExpansion.Text = "Expansion Options:";
            lblExpansion.Font = new Font(lblExpansion.Font, FontStyle.Bold);
            lblExpansion.Location = new Point(leftMargin, y);
            lblExpansion.AutoSize = true;
            tabFormatting.Controls.Add(lblExpansion);
            y += 22;

            // First row of checkboxes
            chkExpandCommaLists = new CheckBox();
            chkExpandCommaLists.Text = "Expand comma lists";
            chkExpandCommaLists.Location = new Point(leftMargin + 15, y);
            chkExpandCommaLists.Size = new Size(380, 24);
            chkExpandCommaLists.Checked = true;
            tabFormatting.Controls.Add(chkExpandCommaLists);

            chkTrailingCommas = new CheckBox();
            chkTrailingCommas.Text = "Use trailing commas";
            chkTrailingCommas.Location = new Point(leftMargin + 420, y);
            chkTrailingCommas.Size = new Size(380, 24);
            tabFormatting.Controls.Add(chkTrailingCommas);
            y += 22;

            // Second row
            chkExpandBooleanExpressions = new CheckBox();
            chkExpandBooleanExpressions.Text = "Expand boolean expressions";
            chkExpandBooleanExpressions.Location = new Point(leftMargin + 15, y);
            chkExpandBooleanExpressions.Size = new Size(380, 24);
            chkExpandBooleanExpressions.Checked = true;
            tabFormatting.Controls.Add(chkExpandBooleanExpressions);

            chkExpandCaseStatements = new CheckBox();
            chkExpandCaseStatements.Text = "Expand CASE statements";
            chkExpandCaseStatements.Location = new Point(leftMargin + 420, y);
            chkExpandCaseStatements.Size = new Size(380, 24);
            chkExpandCaseStatements.Checked = true;
            tabFormatting.Controls.Add(chkExpandCaseStatements);
            y += 22;

            // Third row
            chkExpandBetweenConditions = new CheckBox();
            chkExpandBetweenConditions.Text = "Expand BETWEEN conditions";
            chkExpandBetweenConditions.Location = new Point(leftMargin + 15, y);
            chkExpandBetweenConditions.Size = new Size(380, 24);
            chkExpandBetweenConditions.Checked = true;
            tabFormatting.Controls.Add(chkExpandBetweenConditions);

            chkExpandInLists = new CheckBox();
            chkExpandInLists.Text = "Expand IN lists";
            chkExpandInLists.Location = new Point(leftMargin + 420, y);
            chkExpandInLists.Size = new Size(380, 24);
            chkExpandInLists.Checked = true;
            tabFormatting.Controls.Add(chkExpandInLists);
            y += 22;

            // Fourth row
            chkBreakJoinOnSections = new CheckBox();
            chkBreakJoinOnSections.Text = "Break JOIN on sections";
            chkBreakJoinOnSections.Location = new Point(leftMargin + 15, y);
            chkBreakJoinOnSections.Size = new Size(380, 24);
            tabFormatting.Controls.Add(chkBreakJoinOnSections);

            chkSpaceAfterExpandedComma = new CheckBox();
            chkSpaceAfterExpandedComma.Text = "Space after expanded comma";
            chkSpaceAfterExpandedComma.Location = new Point(leftMargin + 420, y);
            chkSpaceAfterExpandedComma.Size = new Size(380, 24);
            tabFormatting.Controls.Add(chkSpaceAfterExpandedComma);
            y += 30;

            // Line Breaks Section
            Label lblLineBreaks = new Label();
            lblLineBreaks.Text = "Line Breaks:";
            lblLineBreaks.Font = new Font(lblLineBreaks.Font, FontStyle.Bold);
            lblLineBreaks.Location = new Point(leftMargin, y);
            lblLineBreaks.AutoSize = true;
            tabFormatting.Controls.Add(lblLineBreaks);
            y += 22;

            Label lblNewClause = new Label();
            lblNewClause.Text = "New clause breaks:";
            lblNewClause.Location = new Point(leftMargin + 15, y + 2);
            lblNewClause.Size = new Size(150, 20);
            tabFormatting.Controls.Add(lblNewClause);

            numNewClauseLineBreaks = new NumericUpDown();
            numNewClauseLineBreaks.Location = new Point(leftMargin + 170, y);
            numNewClauseLineBreaks.Size = new Size(50, 20);
            numNewClauseLineBreaks.Minimum = 0;
            numNewClauseLineBreaks.Maximum = 5;
            numNewClauseLineBreaks.Value = 1;
            tabFormatting.Controls.Add(numNewClauseLineBreaks);

            Label lblNewStatement = new Label();
            lblNewStatement.Text = "New statement breaks:";
            lblNewStatement.Location = new Point(leftMargin + 420, y + 2);
            lblNewStatement.Size = new Size(160, 20);
            tabFormatting.Controls.Add(lblNewStatement);

            numNewStatementLineBreaks = new NumericUpDown();
            numNewStatementLineBreaks.Location = new Point(leftMargin + 585, y);
            numNewStatementLineBreaks.Size = new Size(50, 20);
            numNewStatementLineBreaks.Minimum = 0;
            numNewStatementLineBreaks.Maximum = 5;
            numNewStatementLineBreaks.Value = 2;
            tabFormatting.Controls.Add(numNewStatementLineBreaks);
        }

        private void CreateCaseTab()
        {
            tabCase = new TabPage("Case");

            int y = 20;
            int leftMargin = 20;

            chkUppercaseKeywords = new CheckBox();
            chkUppercaseKeywords.Text = "Uppercase SQL keywords";
            chkUppercaseKeywords.Location = new Point(leftMargin, y);
            chkUppercaseKeywords.Size = new Size(380, 24);
            chkUppercaseKeywords.Checked = true;
            tabCase.Controls.Add(chkUppercaseKeywords);
            y += 25;

            chkKeywordStandardization = new CheckBox();
            chkKeywordStandardization.Text = "Standardize keywords";
            chkKeywordStandardization.Location = new Point(leftMargin, y);
            chkKeywordStandardization.Size = new Size(380, 24);
            tabCase.Controls.Add(chkKeywordStandardization);
        }

        private void LoadSettings()
        {
            radioIndentTabs.Checked = _settings.IndentString == "\t";
            radioIndentSpaces.Checked = _settings.IndentString != "\t";
            numSpacesPerTab.Value = _settings.SpacesPerTab;
            numMaxLineWidth.Value = _settings.MaxLineWidth;
            chkExpandCommaLists.Checked = _settings.ExpandCommaLists;
            chkTrailingCommas.Checked = _settings.TrailingCommas;
            chkExpandBooleanExpressions.Checked = _settings.ExpandBooleanExpressions;
            chkExpandCaseStatements.Checked = _settings.ExpandCaseStatements;
            chkExpandBetweenConditions.Checked = _settings.ExpandBetweenConditions;
            chkExpandInLists.Checked = _settings.ExpandInLists;
            chkBreakJoinOnSections.Checked = _settings.BreakJoinOnSections;
            chkSpaceAfterExpandedComma.Checked = _settings.SpaceAfterExpandedComma;
            numNewClauseLineBreaks.Value = _settings.NewClauseLineBreaks;
            numNewStatementLineBreaks.Value = _settings.NewStatementLineBreaks;
            chkUppercaseKeywords.Checked = _settings.UppercaseKeywords;
            chkKeywordStandardization.Checked = _settings.KeywordStandardization;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            // Save settings
            _settings.IndentString = radioIndentTabs.Checked ? "\t" : new string(' ', (int)numSpacesPerTab.Value);
            _settings.SpacesPerTab = (int)numSpacesPerTab.Value;
            _settings.MaxLineWidth = (int)numMaxLineWidth.Value;
            _settings.ExpandCommaLists = chkExpandCommaLists.Checked;
            _settings.TrailingCommas = chkTrailingCommas.Checked;
            _settings.ExpandBooleanExpressions = chkExpandBooleanExpressions.Checked;
            _settings.ExpandCaseStatements = chkExpandCaseStatements.Checked;
            _settings.ExpandBetweenConditions = chkExpandBetweenConditions.Checked;
            _settings.ExpandInLists = chkExpandInLists.Checked;
            _settings.BreakJoinOnSections = chkBreakJoinOnSections.Checked;
            _settings.SpaceAfterExpandedComma = chkSpaceAfterExpandedComma.Checked;
            _settings.NewClauseLineBreaks = (int)numNewClauseLineBreaks.Value;
            _settings.NewStatementLineBreaks = (int)numNewStatementLineBreaks.Value;
            _settings.UppercaseKeywords = chkUppercaseKeywords.Checked;
            _settings.KeywordStandardization = chkKeywordStandardization.Checked;

            _settings.SaveSettings();
        }

        private void BtnDefaults_Click(object sender, EventArgs e)
        {
            // Reset to defaults
            radioIndentTabs.Checked = true;
            radioIndentSpaces.Checked = false;
            numSpacesPerTab.Value = 4;
            numMaxLineWidth.Value = 999;
            chkExpandCommaLists.Checked = true;
            chkTrailingCommas.Checked = false;
            chkExpandBooleanExpressions.Checked = true;
            chkExpandCaseStatements.Checked = true;
            chkExpandBetweenConditions.Checked = true;
            chkExpandInLists.Checked = true;
            chkBreakJoinOnSections.Checked = false;
            chkSpaceAfterExpandedComma.Checked = false;
            numNewClauseLineBreaks.Value = 1;
            numNewStatementLineBreaks.Value = 2;
            chkUppercaseKeywords.Checked = true;
            chkKeywordStandardization.Checked = false;
        }
    }
}