using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace TSqlFormatter.VS2026
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", "1.7.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideAutoLoad(VSConstants.UICONTEXT.NoSolution_string, PackageAutoLoadFlags.BackgroundLoad)]
    [Guid(PackageGuidString)]
    public sealed class FormatterPackage : AsyncPackage
    {
        public const string PackageGuidString = "c47a9b21-2692-47d6-972a-976544685f0f";
        public const string CommandSetGuidString = "6fa2e413-8351-4ca9-b0a0-34a9b241648c";

        public const uint FormatSqlCommandId = 0x0100;
        public const uint OptionsCommandId = 0x0101;

        public static readonly Guid CommandSetGuid = new Guid(CommandSetGuidString);

        private FormatCommand _formatCommand;

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            try
            {
                await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
                await base.InitializeAsync(cancellationToken, progress);

                var mcs = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
                if (mcs != null)
                {
                    // Format SQL command
                    var formatCommandID = new CommandID(CommandSetGuid, (int)FormatSqlCommandId);
                    var formatCommand = new OleMenuCommand(FormatSqlCallback, formatCommandID);
                    formatCommand.BeforeQueryStatus += QueryFormatButtonStatus;
                    mcs.AddCommand(formatCommand);

                    // Options command
                    var optionsCommandID = new CommandID(CommandSetGuid, (int)OptionsCommandId);
                    var optionsCommand = new OleMenuCommand(OptionsCallback, optionsCommandID);
                    mcs.AddCommand(optionsCommand);
                }

                _formatCommand = new FormatCommand(this);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing FormatterPackage: {ex}");
            }
        }

        private void QueryFormatButtonStatus(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var command = sender as OleMenuCommand;
            if (command != null)
            {
                var dte = (DTE2)GetService(typeof(DTE));
                command.Enabled = dte?.ActiveDocument != null;
            }
        }

        private void FormatSqlCallback(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_formatCommand != null)
                {
                    _formatCommand.Execute();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("FormatCommand is null!");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in FormatSqlCallback: {ex}");
                System.Windows.Forms.MessageBox.Show($"Error formatting SQL: {ex.Message}", "T-SQL Formatter Error");
            }
        }

        private void OptionsCallback(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (_formatCommand != null)
                {
                    _formatCommand.ShowOptions();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in OptionsCallback: {ex}");
                System.Windows.Forms.MessageBox.Show($"Error showing options: {ex.Message}", "T-SQL Formatter Error");
            }
        }
    }
}