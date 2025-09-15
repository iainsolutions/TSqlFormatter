/*
Poor Man's T-SQL Formatter - a small free Transact-SQL formatting 
library for .Net 2.0 and JS, written in C#. 
Copyright (C) 2011-2025 Tao Klerks

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <http://www.gnu.org/licenses/>.

*/

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;
using PoorMansTSqlFormatterSSMSLib;
using System;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

//Please note, most of this code is duplicated across the SSMS Package, VS2015 extension, VS2019 extension, and SSMS 2021 extension. 
// Descriptions, GUIDs, Early SSMS support, and Async loading support differ.
// (it would make sense to improve this at some point)
namespace PoorMansTSqlFormatterSSMSPackage2021
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]  //General VSPackage hookup with async loading support
    [InstalledProductRegistration("#ProductName", "#ProductDescription", "1.6.16")]  //Package Medatada, references to VSPackage.resx resource keys
    [ProvideAutoLoad(VSConstants.UICONTEXT.NotBuildingAndNotDebugging_string, PackageAutoLoadFlags.BackgroundLoad)] // Auto-load for dynamic menu enabling/disabling with background loading
    [ProvideMenuResource("Menus.ctmenu", 1)]  //Hook to command definitions / to vsct stuff
    [Guid(guidPoorMansTSqlFormatterSSMSPackage2021PkgString)] //Arbitrarily/randomly defined guid for this extension
    public sealed class FormatterPackage : AsyncPackage
    {
        //These constants are duplicated in the vsct file
        public const string guidPoorMansTSqlFormatterSSMSPackage2021PkgString = "c47a9b21-2692-47d6-972a-976544685f0f";
        public const string guidPoorMansTSqlFormatterSSMSPackage2021CmdSetString = "6fa2e413-8351-4ca9-b0a0-34a9b241648c";
        public const uint cmdidPoorMansFormatSQL = 0x100;
        public const uint cmdidPoorMansSqlOptions = 0x101;

        public static readonly Guid guidPoorMansTSqlFormatterSSMSPackage2021CmdSet = new Guid(guidPoorMansTSqlFormatterSSMSPackage2021CmdSetString);

        //TODO: figure out how to deal with signing... where to keep the key, etc.

        private GenericVSHelper _SSMSHelper;
        private System.Timers.Timer _packageLoadingDisableTimer;

        public FormatterPackage()
        {
        }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            
            await base.InitializeAsync(cancellationToken, progress);

            _SSMSHelper = new GenericVSHelper(true, null, null, null);

            // Add our command handlers for the menu commands defined in the in the .vsct file, and enable them
            OleMenuCommandService mcs = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                CommandID menuCommandID;
                OleMenuCommand menuCommand;

                // Create the formatting command / menu item.
                menuCommandID = new CommandID(guidPoorMansTSqlFormatterSSMSPackage2021CmdSet, (int)cmdidPoorMansFormatSQL);
                menuCommand = new OleMenuCommand(FormatSqlCallback, menuCommandID);
                mcs.AddCommand(menuCommand);
                menuCommand.BeforeQueryStatus += new EventHandler(QueryFormatButtonStatus);

                // Create the options command / menu item.
                menuCommandID = new CommandID(guidPoorMansTSqlFormatterSSMSPackage2021CmdSet, (int)cmdidPoorMansSqlOptions);
                menuCommand = new OleMenuCommand(SqlOptionsCallback, menuCommandID);
                menuCommand.Enabled = true;
                mcs.AddCommand(menuCommand);
            }

            _packageLoadingDisableTimer = new System.Timers.Timer();
            _packageLoadingDisableTimer.Elapsed += new System.Timers.ElapsedEventHandler(PackageDisableLoadingCallback);
            _packageLoadingDisableTimer.Interval = 15000;
            _packageLoadingDisableTimer.Enabled = true;
        }

        private void FormatSqlCallback(object sender, EventArgs e)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            DTE2 dte = (DTE2)GetService(typeof(DTE));
            _SSMSHelper.FormatSqlInTextDoc(dte);
        }

        private void SqlOptionsCallback(object sender, EventArgs e)
        {
            _SSMSHelper.GetUpdatedFormattingOptionsFromUser();
        }

        private void QueryFormatButtonStatus(object sender, EventArgs e) 
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var queryingCommand = sender as OleMenuCommand;
            DTE2 dte = (DTE2)GetService(typeof(DTE));
            if (queryingCommand != null && dte.ActiveDocument != null && !dte.ActiveDocument.ReadOnly)
                queryingCommand.Enabled = true;
            else
                queryingCommand.Enabled = false;
        }


        private void PackageDisableLoadingCallback(object sender, System.Timers.ElapsedEventArgs e)
        {
            _packageLoadingDisableTimer.Enabled = false;
            SetPackageLoadingDisableKeyIfRequired();
        }

        protected override int QueryClose(out bool canClose)
        {
            SetPackageLoadingDisableKeyIfRequired();
            return base.QueryClose(out canClose);
        }

        /// <summary>
        /// For SSMS 2015 and earlier, this will set a registry key to disable the extension. Strangely, extension loading only works for disabled extensions...
        /// Note: SSMS 21 doesn't need this workaround, but keeping for compatibility detection
        /// </summary>
        private void SetPackageLoadingDisableKeyIfRequired()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            DTE2 dte = (DTE2)GetService(typeof(DTE));
            string fullName = dte.FullName.ToUpperInvariant();
            int majorVersion = int.Parse(dte.Version.Split('.')[0]);

            // SSMS 21 is based on VS 2022 (version 17) and doesn't need the SkipLoading workaround
            if ((fullName.Contains("SSMS") || fullName.Contains("MANAGEMENT STUDIO")) && majorVersion <= 2017)
                UserRegistryRoot.CreateSubKey(@"Packages\{" + guidPoorMansTSqlFormatterSSMSPackage2021PkgString + "}").SetValue("SkipLoading", 1);
        }
    }
}