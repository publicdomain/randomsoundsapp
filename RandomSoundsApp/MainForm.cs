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
    using System.Windows.Forms;
    using Microsoft.Win32;

    /// <summary>
    /// Main form.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:RandomSoundsApp.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            this.InitializeComponent();
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
                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true))
                {
                    // Check if must write to registry
                    if (this.startCheckBox.Checked)
                    {
                        // Add executable path
                        registryKey.SetValue("RandomSoundsApp", "\"" + Application.ExecutablePath + "\"");
                    }
                    else
                    {
                        // Remove app
                        registryKey.DeleteValue("RandomSoundsApp", false);
                    }
                }
            }
            catch
            {
                // Inform user
                MessageBox.Show("Error when writing to windows registry.", "Registry error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Handles the on radio button checked changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOnRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // TODO Add code.
        }

        /// <summary>
        /// Handles the off radio button checked changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOffRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // TODO Add code.
        }

        /// <summary>
        /// Ons the scan directory tool strip menu item click event.
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
            // TODO Add code.
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
        /// <param name="sender">Sender.</param>
        /// <param name="e">E.</param>
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
    }
}
