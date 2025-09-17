using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using System;
using TSqlFormatter;
using TSqlFormatter.Formatters;

namespace TSqlFormatter.SSMS
{
    public class FormatCommand
    {
        private readonly AsyncPackage _package;
        private Settings _settings;

        public FormatCommand(AsyncPackage package)
        {
            _package = package ?? throw new ArgumentNullException(nameof(package));
            _settings = new Settings();
        }

        public void Execute()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                var dte = (DTE2)Package.GetGlobalService(typeof(DTE));
                if (dte?.ActiveDocument == null)
                    return;

                var textDocument = (TextDocument)dte.ActiveDocument.Object("TextDocument");
                if (textDocument == null)
                    return;

                var selection = textDocument.Selection;
                string sqlToFormat;
                bool selectionOnly = false;

                if (selection != null && !selection.IsEmpty)
                {
                    sqlToFormat = selection.Text;
                    selectionOnly = true;
                }
                else
                {
                    var startPoint = textDocument.StartPoint.CreateEditPoint();
                    var endPoint = textDocument.EndPoint.CreateEditPoint();
                    sqlToFormat = startPoint.GetText(endPoint);
                }

                if (string.IsNullOrWhiteSpace(sqlToFormat))
                    return;

                // Format the SQL with options
                var formatterOptions = _settings.GetFormatterOptions();
                var formatter = new SqlFormattingManager(new TSqlStandardFormatter(formatterOptions));
                string formattedSql = formatter.Format(sqlToFormat);

                // Replace the text
                if (selectionOnly)
                {
                    selection.Delete();
                    selection.Insert(formattedSql);
                }
                else
                {
                    var startPoint = textDocument.StartPoint.CreateEditPoint();
                    var endPoint = textDocument.EndPoint.CreateEditPoint();
                    startPoint.ReplaceText(endPoint, formattedSql, 0);
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Error formatting SQL: {ex.Message}",
                    "T-SQL Formatter",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        public void ShowOptions()
        {
            ThreadHelper.ThrowIfNotOnUIThread();

            try
            {
                // Show WPF dialog using VS 2022 DialogWindow
                var wpfDialog = new OptionsDialogWindow(_settings);
                wpfDialog.ShowModal();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(
                    $"Error showing options: {ex.Message}",
                    "T-SQL Formatter",
                    System.Windows.Forms.MessageBoxButtons.OK,
                    System.Windows.Forms.MessageBoxIcon.Error);
            }
        }
    }
}