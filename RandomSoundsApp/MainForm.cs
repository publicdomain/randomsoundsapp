// <copyright file="MainForm.cs" company="PublicDomain.com">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

namespace RandomSoundsApp
{
    // Directives
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Windows.Forms;
    using Microsoft.Win32;

    /// <summary>
    /// Main form.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// The sound file list.
        /// </summary>
        private List<string> soundFileList = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RandomSoundsApp.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            this.InitializeComponent();

            // Set notify icon (utilize current one)
            this.mainNotifyIcon.Icon = this.Icon;

            // Open registry key
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
            {
                // Toggle check box by app value presence
                this.startCheckBox.Checked |= registryKey.GetValueNames().Contains("RandomSoundsApp");
            }

            // Match off radio button state by disabling controls on start
            this.EnableDisableControls(false);

            // Scan directory for sound files
            this.ScanDirectory();
        }

        /// <summary>
        /// Handles the main form load event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMainFormLoad(object sender, EventArgs e)
        {
            /* Autostart processing */

            // Set arguments via environment as list (instead of string[] args for convenience i.e. .Count, etc)
            List<string> argsList = new List<string>(Environment.GetCommandLineArgs());

            // Check for autostart argument
            if (argsList.Count > 1 && argsList[1].ToLowerInvariant() == "/autostart")
            {
                // Start as a system tray icon
                this.SendToSystemTray();
            }
        }

        /// <summary>
        /// Scans current directory for sound files.
        /// </summary>
        private void ScanDirectory()
        {
            // Populate sound file list
            this.soundFileList = new List<string>(Directory.GetFiles(Application.StartupPath, "*.wav", SearchOption.AllDirectories));
        }

        /// <summary>
        /// Handles the exact timer tick event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExactTimerTick(object sender, EventArgs e)
        {
            // TODO Add code.
        }

        /// <summary>
        /// Handles the random interval timer tick event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnRandomIntervalTimerTick(object sender, EventArgs e)
        {
            // TODO Add code.
        }

        /// <summary>
        /// Handles the start check box checked changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnStartCheckBoxCheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // Open registry key
                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    // Check if must write to registry
                    if (this.startCheckBox.Checked)
                    {
                        // Add app value
                        registryKey.SetValue("RandomSoundsApp", $"\"{Application.ExecutablePath}\" /autostart");
                    }
                    else
                    {
                        // Erase app value
                        registryKey.DeleteValue("RandomSoundsApp", false);
                    }
                }
            }
            catch
            {
                // Inform user
                MessageBox.Show("Error when interacting with the Windows registry.", "Registry error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Enables or disables relevant controls in the form.
        /// </summary>
        /// <param name="isEnabled">If set to <c>true</c> enables all controls. Disables them otherwise.</param>
        private void EnableDisableControls(bool isEnabled)
        {
            // Set enabled status for relevant controls
            this.everyIntervalRadioButton.Enabled = isEnabled;
            this.randomIntervalRadioButton.Enabled = isEnabled;
            this.inEveryLabel.Enabled = isEnabled;
            this.minutesLabel.Enabled = isEnabled;
            this.minuteIntervalLabel.Enabled = isEnabled;
            this.everyIntervalNumericUpDown.Enabled = isEnabled;
            this.randomIntervalNumericUpDown.Enabled = isEnabled;
        }

        /// <summary>
        /// Handles the on radio button checked changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOnRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // Toggle colors
            this.onRadioButton.ForeColor = Color.Red;
            this.offRadioButton.ForeColor = Color.Black;

            // Enable relevant form controls
            this.EnableDisableControls(true);
        }

        /// <summary>
        /// Handles the off radio button checked changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOffRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // Toggle colors
            this.offRadioButton.ForeColor = Color.Red;
            this.onRadioButton.ForeColor = Color.Black;

            // Disable relevant form controls
            this.EnableDisableControls(false);
        }

        /// <summary>
        /// Handles the scan directory tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnScanDirectoryToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code.
        }

        /// <summary>
        /// Handles the exit tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Close app
            this.Close();
        }

        /// <summary>
        /// Handles the headquarters patreon.com tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnHeadquartersPatreoncomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code.
        }

        /// <summary>
        /// Handles the source code github.com tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSourceCodeGithubcomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code.
        }

        /// <summary>
        /// Handles the original thread donationcoder.com tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOriginalThreadDonationCodercomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code.
        }

        /// <summary>
        /// Handles the about tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // TODO Add code.
        }

        /// <summary>
        /// Handles the show tool strip menu item click event. 
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnShowToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Restore window 
            this.RestoreFromSystemTray();
        }

        /// <summary>
        /// Handles the main notify icon mouse click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Mouse event arguments.</param>
        private void OnMainNotifyIconMouseClick(object sender, MouseEventArgs e)
        {
            // Check for left click
            if (e.Button == MouseButtons.Left)
            {
                // Restore window 
                this.RestoreFromSystemTray();
            }
        }

        /// <summary>
        /// Sends the program to the system tray.
        /// </summary>
        private void SendToSystemTray()
        {
            // Hide main form
            this.Hide();

            // Remove from task bar
            this.ShowInTaskbar = false;

            // Show notify icon 
            this.mainNotifyIcon.Visible = true;
        }

        /// <summary>
        /// Restores the window back from system tray to the foreground.
        /// </summary>
        private void RestoreFromSystemTray()
        {
            // Make form visible again
            this.Show();

            // Return window back to normal
            this.WindowState = FormWindowState.Normal;

            // Restore in task bar
            this.ShowInTaskbar = true;

            // Hide system tray icon
            this.mainNotifyIcon.Visible = false;
        }

        /// <summary>
        /// Handles the main form resize event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMainFormResize(object sender, EventArgs e)
        {
            // Check for minimized state
            if (this.WindowState == FormWindowState.Minimized)
            {
                // Send to the system tray
                this.SendToSystemTray();
            }
        }
    }
}