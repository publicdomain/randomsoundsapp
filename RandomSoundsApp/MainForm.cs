// <copyright file="MainForm.cs" company="PublicDomain.com">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

namespace RandomSoundsApp
{
    // Directives
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Media;
    using System.Reflection;
    using System.Windows.Forms;
    using Microsoft.Win32;
    using PublicDomain;

    /// <summary>
    /// Main form.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// The sound player.
        /// </summary>
        private SoundPlayer soundPlayer = null;

        /// <summary>
        /// The sound file list.
        /// </summary>
        private List<string> soundFileList = null;

        /// <summary>
        /// The pseudo-random number generator.
        /// </summary>
        private Random random = new Random();

        /// <summary>
        /// The assembly version.
        /// </summary>
        private Version assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// The semantic version.
        /// </summary>
        private string semanticVersion = string.Empty;

        /// <summary>
        /// The associated icon.
        /// </summary>
        private Icon associatedIcon = null;

        /// <summary>
        /// The friendly name of the program.
        /// </summary>
        private string friendlyName = "Random Sounds App";

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RandomSoundsApp.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            this.InitializeComponent();

            // Set notify icon (utilize current one)
            this.mainNotifyIcon.Icon = this.Icon;

            // Set semantic version
            this.semanticVersion = this.assemblyVersion.Major + "." + this.assemblyVersion.Minor + "." + this.assemblyVersion.Build;

            // Open registry key
            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false))
            {
                // Toggle check box by app value presence
                this.startCheckBox.Checked |= registryKey.GetValueNames().Contains("RandomSoundsApp");
            }

            // Match off radio button state by disabling controls on start
            this.EnableDisableControls(false);

            // Scan current directory for sound files
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
        /// Plays the passed sound file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        private void PlaySoundFile(string filePath)
        {
            // Check for previous player
            if (this.soundPlayer != null)
            {
                // Stop it
                this.soundPlayer.Stop();

                // Dispose of it
                this.soundPlayer.Dispose();

                // Reset instance variable
                this.soundPlayer = null;
            }

            // Set instance player afresh
            this.soundPlayer = new SoundPlayer(filePath);

            // Play the passed file
            this.soundPlayer.Play();
        }

        /// <summary>
        /// Plays a random sound in collected file list.
        /// </summary>
        private void PlayRandomSoundFile()
        {
            // Re-use instance's random to pick file to play
            this.PlaySoundFile(this.soundFileList[this.random.Next(this.soundFileList.Count)]);
        }

        /// <summary>
        /// Scans current directory for sound files.
        /// </summary>
        private void ScanDirectory()
        {
            // Populate sound file list
            this.soundFileList = new List<string>(Directory.GetFiles(Application.StartupPath, "*.wav", SearchOption.AllDirectories));

            // Assign count with message
            this.mainToolStripStatusLabel.Text = $"Found {this.soundFileList.Count} sound files to play";
        }

        /// <summary>
        /// Handles the checked changed event for both settings radio buttons.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSettingsRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // Check for no checked radio button
            if (!this.fromTheHourRadioButton.Checked && !this.randomIntervalRadioButton.Checked)
            {
                // Halt flow
                return;
            }

            /* Process */

            // Check if interval radio button is checked
            if (this.randomIntervalRadioButton.Checked)
            {
                // Trigger initial timer tick
                this.OnExactTimerTick(null, null);
            }

            // Set timer interval in milliseconds
            this.exactTimer.Interval = Convert.ToInt32((this.fromTheHourRadioButton.Checked ? this.fromTheHourNumericUpDown.Value : this.randomIntervalNumericUpDown.Value) * 60 * 1000);

            // Start exact timer
            this.exactTimer.Start();

            // Set settings label color
            this.settingsLabel.ForeColor = Color.Red;
        }

        /// <summary>
        /// Handles from the hour timer tick event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnFromTheHourTimerTick(object sender, EventArgs e)
        {
            // TODO Add code.
        }

        /// <summary>
        /// Handles the exact timer tick event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExactTimerTick(object sender, EventArgs e)
        {
            // Check for every interval
            if (this.fromTheHourRadioButton.Checked)
            {
                // Play random sound file
                this.PlayRandomSoundFile();
            }
            else
            {
                // Set random interval timer interval in milliseconds
                this.randomIntervalTimer.Interval = this.random.Next(Convert.ToInt32(this.randomIntervalNumericUpDown.Value * 60 * 1000));

                // Start random interval timer
                this.randomIntervalTimer.Start();
            }
        }

        /// <summary>
        /// Handles the random interval timer tick event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnRandomIntervalTimerTick(object sender, EventArgs e)
        {
            // Stop the timer
            this.randomIntervalTimer.Stop();

            // Play random sound file
            this.PlayRandomSoundFile();
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
            // Check if passed value is false
            if (!isEnabled)
            {
                // Unckeck radio buttons
                this.fromTheHourRadioButton.Checked = false;
                this.randomIntervalRadioButton.Checked = false;

                // Reset settings label color
                this.settingsLabel.ForeColor = Color.Black;
            }

            // Set enabled status for relevant controls
            this.fromTheHourRadioButton.Enabled = isEnabled;
            this.randomIntervalRadioButton.Enabled = isEnabled;
            this.inEveryLabel.Enabled = isEnabled;
            this.hourMinutesLabel.Enabled = isEnabled;
            this.minuteIntervalLabel.Enabled = isEnabled;
            this.fromTheHourNumericUpDown.Enabled = isEnabled;
            this.randomIntervalNumericUpDown.Enabled = isEnabled;

            // Stop timers
            this.exactTimer.Stop();
            this.randomIntervalTimer.Stop();
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
        /// Handles the numeric up down value changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnNumericUpDownValueChanged(object sender, EventArgs e)
        {
            // Trigger settings radio button check
            this.OnSettingsRadioButtonCheckedChanged(null, null);
        }

        /// <summary>
        /// Handles the scan directory tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnScanDirectoryToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Re-scan current directory
            this.ScanDirectory();
        }

        /// <summary>
        /// Handles the exit tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Close application
            this.Close();
        }

        /// <summary>
        /// Handles the headquarters patreon.com tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnHeadquartersPatreoncomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open Patreon headquarters
            Process.Start("https://www.patreon.com/publicdomain");
        }

        /// <summary>
        /// Handles the source code github.com tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSourceCodeGithubcomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open GitHub
            Process.Start("https://github.com/publicdomain");
        }

        /// <summary>
        /// Handles the original thread donationcoder.com tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOriginalThreadDonationCodercomToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Open original thread @ DonationCoder
            Process.Start("https://www.donationcoder.com/forum/index.php?topic=48061.0");
        }

        /// <summary>
        /// Handles the about tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnAboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Set license text
            var licenseText = $"CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication{Environment.NewLine}" +
                $"https://creativecommons.org/publicdomain/zero/1.0/legalcode{Environment.NewLine}{Environment.NewLine}" +
                $"Libraries and icons have separate licenses.{Environment.NewLine}{Environment.NewLine}" +
                $"Sound icon by Openclipart - Public Domain dedication{Environment.NewLine}" +
                $"https://publicdomainvectors.org/en/free-clipart/Sound-speaker/82806.html{Environment.NewLine}{Environment.NewLine}" +
                $"Patreon icon used according to published brand guidelines{Environment.NewLine}" +
                $"https://www.patreon.com/brand{Environment.NewLine}{Environment.NewLine}" +
                $"GitHub mark icon used according to published logos and usage guidelines{Environment.NewLine}" +
                $"https://github.com/logos{Environment.NewLine}{Environment.NewLine}" +
                $"DonationCoder icon used with permission{Environment.NewLine}" +
                $"https://www.donationcoder.com/forum/index.php?topic=48718{Environment.NewLine}{Environment.NewLine}" +
                $"PublicDomain icon is based on the following source images:{Environment.NewLine}{Environment.NewLine}" +
                $"Bitcoin by GDJ - Pixabay License{Environment.NewLine}" +
                $"https://pixabay.com/vectors/bitcoin-digital-currency-4130319/{Environment.NewLine}{Environment.NewLine}" +
                $"Letter P by ArtsyBee - Pixabay License{Environment.NewLine}" +
                $"https://pixabay.com/illustrations/p-glamour-gold-lights-2790632/{Environment.NewLine}{Environment.NewLine}" +
                $"Letter D by ArtsyBee - Pixabay License{Environment.NewLine}" +
                $"https://pixabay.com/illustrations/d-glamour-gold-lights-2790573/{Environment.NewLine}{Environment.NewLine}";


            // Set about form
            var aboutForm = new AboutForm(
                $"About {this.friendlyName}",
                $"{this.friendlyName} {this.semanticVersion}",
                "Week #40 @ September 2019",
                licenseText,
                this.Icon.ToBitmap());

            // Check for an associated icon
            if (this.associatedIcon == null)
            {
                // Set associated icon from exe file, once
                this.associatedIcon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            }

            // Set about form icon
            aboutForm.Icon = this.associatedIcon;

            // Show about form
            aboutForm.ShowDialog();
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