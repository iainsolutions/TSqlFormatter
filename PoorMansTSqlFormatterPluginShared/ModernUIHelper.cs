using System;
using System.Drawing;
using System.Windows.Forms;

namespace PoorMansTSqlFormatterPluginShared
{
    /// <summary>
    /// Helper class to apply modern UI styling to Windows Forms
    /// </summary>
    public static class ModernUIHelper
    {
        // Modern color palette
        public static readonly Color PrimaryColor = Color.FromArgb(0, 122, 204);  // Modern blue
        public static readonly Color PrimaryHoverColor = Color.FromArgb(28, 151, 234);  // Lighter blue for hover
        public static readonly Color AccentColor = Color.FromArgb(0, 178, 148);  // Teal accent
        public static readonly Color BackgroundColor = Color.FromArgb(245, 245, 245);  // Light gray background
        public static readonly Color SurfaceColor = Color.White;
        public static readonly Color TextPrimaryColor = Color.FromArgb(33, 33, 33);  // Dark gray text
        public static readonly Color TextSecondaryColor = Color.FromArgb(117, 117, 117);  // Medium gray text
        public static readonly Color BorderColor = Color.FromArgb(229, 229, 229);  // Light border
        public static readonly Color SuccessColor = Color.FromArgb(67, 160, 71);  // Green
        public static readonly Color DangerColor = Color.FromArgb(239, 83, 80);  // Red

        // Font settings
        public static readonly string ModernFontFamily = "Segoe UI";
        public static readonly float DefaultFontSize = 9f;
        public static readonly float HeaderFontSize = 11f;
        public static readonly float ButtonFontSize = 9f;

        /// <summary>
        /// Apply modern styling to a form
        /// </summary>
        public static void StyleForm(Form form)
        {
            form.BackColor = BackgroundColor;
            form.Font = new Font(ModernFontFamily, DefaultFontSize, FontStyle.Regular);
            form.ForeColor = TextPrimaryColor;

            // Add padding around the form
            form.Padding = new Padding(20);

            // Style all controls recursively
            StyleControls(form.Controls);
        }

        /// <summary>
        /// Recursively style all controls in a collection
        /// </summary>
        private static void StyleControls(Control.ControlCollection controls)
        {
            foreach (Control control in controls)
            {
                // Style based on control type
                if (control is Button button)
                {
                    StyleButton(button);
                }
                else if (control is TextBox textBox)
                {
                    StyleTextBox(textBox);
                }
                else if (control is Label label)
                {
                    StyleLabel(label);
                }
                else if (control is LinkLabel linkLabel)
                {
                    StyleLinkLabel(linkLabel);
                }
                else if (control is CheckBox checkBox)
                {
                    StyleCheckBox(checkBox);
                }
                else if (control is GroupBox groupBox)
                {
                    StyleGroupBox(groupBox);
                }
                else if (control is Panel panel)
                {
                    StylePanel(panel);
                }
                else if (control is FlowLayoutPanel flowPanel)
                {
                    StyleFlowLayoutPanel(flowPanel);
                }

                // Recursively style child controls
                if (control.HasChildren)
                {
                    StyleControls(control.Controls);
                }
            }
        }

        /// <summary>
        /// Style a button with modern appearance
        /// </summary>
        public static void StyleButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.BackColor = PrimaryColor;
            button.ForeColor = Color.White;
            button.Font = new Font(ModernFontFamily, ButtonFontSize, FontStyle.Regular);
            button.Cursor = Cursors.Hand;
            button.Padding = new Padding(12, 8, 12, 8);

            // Set minimum size for better touch targets
            if (button.Height < 36)
                button.Height = 36;

            // Add hover effects
            button.MouseEnter += (s, e) => {
                button.BackColor = PrimaryHoverColor;
            };
            button.MouseLeave += (s, e) => {
                button.BackColor = PrimaryColor;
            };

            // Special styling for specific buttons
            if (button.Name == "btn_Cancel")
            {
                button.BackColor = Color.FromArgb(158, 158, 158);
                button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(117, 117, 117);
                button.MouseLeave += (s, e) => button.BackColor = Color.FromArgb(158, 158, 158);
            }
            else if (button.Name == "btn_Reset")
            {
                button.BackColor = DangerColor;
                button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(229, 57, 53);
                button.MouseLeave += (s, e) => button.BackColor = DangerColor;
            }
            else if (button.Name == "btn_About")
            {
                button.BackColor = AccentColor;
                button.MouseEnter += (s, e) => button.BackColor = Color.FromArgb(0, 150, 120);
                button.MouseLeave += (s, e) => button.BackColor = AccentColor;
            }
        }

        /// <summary>
        /// Style a textbox with modern appearance
        /// </summary>
        public static void StyleTextBox(TextBox textBox)
        {
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font(ModernFontFamily, DefaultFontSize);
            textBox.BackColor = SurfaceColor;
            textBox.ForeColor = TextPrimaryColor;
            textBox.Padding = new Padding(8, 4, 8, 4);

            // Set minimum height
            if (textBox.Multiline && textBox.Height < 24)
                textBox.Height = 24;
        }

        /// <summary>
        /// Style a label
        /// </summary>
        public static void StyleLabel(Label label)
        {
            label.Font = new Font(ModernFontFamily, DefaultFontSize);
            label.ForeColor = TextPrimaryColor;
            label.BackColor = Color.Transparent;

            // Style hint labels differently
            if (label.Name.Contains("Hint") || label.Name.Contains("Extra"))
            {
                label.Font = new Font(ModernFontFamily, 8.25f);
                label.ForeColor = TextSecondaryColor;
            }
        }

        /// <summary>
        /// Style a link label
        /// </summary>
        public static void StyleLinkLabel(LinkLabel linkLabel)
        {
            linkLabel.LinkColor = PrimaryColor;
            linkLabel.ActiveLinkColor = PrimaryHoverColor;
            linkLabel.VisitedLinkColor = PrimaryColor;
            linkLabel.Font = new Font(ModernFontFamily, DefaultFontSize);
            linkLabel.BackColor = Color.Transparent;
        }

        /// <summary>
        /// Style a checkbox
        /// </summary>
        public static void StyleCheckBox(CheckBox checkBox)
        {
            checkBox.Font = new Font(ModernFontFamily, DefaultFontSize);
            checkBox.ForeColor = TextPrimaryColor;
            checkBox.BackColor = Color.Transparent;
            checkBox.FlatStyle = FlatStyle.Flat;
            checkBox.Cursor = Cursors.Hand;
            checkBox.Padding = new Padding(0, 2, 0, 2);
        }

        /// <summary>
        /// Style a group box
        /// </summary>
        public static void StyleGroupBox(GroupBox groupBox)
        {
            groupBox.Font = new Font(ModernFontFamily, HeaderFontSize, FontStyle.Regular);
            groupBox.ForeColor = TextPrimaryColor;
            groupBox.BackColor = SurfaceColor;
            groupBox.FlatStyle = FlatStyle.Flat;
            groupBox.Padding = new Padding(12, 20, 12, 12);
        }

        /// <summary>
        /// Style a panel
        /// </summary>
        public static void StylePanel(Panel panel)
        {
            panel.BackColor = SurfaceColor;
            panel.BorderStyle = BorderStyle.None;
            panel.Padding = new Padding(12);
        }

        /// <summary>
        /// Style a flow layout panel
        /// </summary>
        public static void StyleFlowLayoutPanel(FlowLayoutPanel panel)
        {
            panel.BackColor = SurfaceColor;
            panel.BorderStyle = BorderStyle.None;
            panel.Padding = new Padding(12);
            panel.AutoScroll = true;
        }

        /// <summary>
        /// Create a modern styled separator line
        /// </summary>
        public static Panel CreateSeparator()
        {
            return new Panel
            {
                Height = 1,
                Dock = DockStyle.Top,
                BackColor = BorderColor,
                Margin = new Padding(0, 10, 0, 10)
            };
        }

        /// <summary>
        /// Apply card-style elevation to a control
        /// </summary>
        public static void ApplyCardStyle(Control control)
        {
            control.BackColor = SurfaceColor;
            control.Padding = new Padding(16);

            // Note: True shadows require custom painting or third-party controls
            // This is a simplified version using borders
            if (control is Panel || control is GroupBox)
            {
                control.Paint += (sender, e) =>
                {
                    ControlPaint.DrawBorder(e.Graphics, control.ClientRectangle,
                        BorderColor, 1, ButtonBorderStyle.Solid,
                        BorderColor, 1, ButtonBorderStyle.Solid,
                        BorderColor, 1, ButtonBorderStyle.Solid,
                        BorderColor, 1, ButtonBorderStyle.Solid);
                };
            }
        }
    }
}