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
    using System.Timers;
    using System.Windows.Forms;
    using System.Xml.Serialization;
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
        /// The action timer.
        /// </summary>
        private System.Timers.Timer actionTimer = null;

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
        /// The scanned files.
        /// </summary>
        private string scannedFiles = string.Empty;

        /// <summary>
        /// The last selected radio button.
        /// </summary>
        private RadioButton lastSelectedRadioButton = null;

        /// <summary>
        /// The play sound date time.
        /// </summary>
        private DateTime playSoundDateTime;

        /// <summary>
        /// The random interval date time.
        /// </summary>
        private DateTime randomIntervalDateTime;

        /// <summary>
        /// The timer elapsed second.
        /// </summary>
        private int timerElapsedSecond = -1;

        /// <summary>
        /// The settings data.
        /// </summary>
        private SettingsData settingsData = new SettingsData();

        /// <summary>
        /// The restoring values flag.
        /// </summary>
        private bool RestoringValuesFlag = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:RandomSoundsApp.MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            // The InitializeComponent() call is required for Windows Forms designer support.
            this.InitializeComponent();

            // Set the action timer
            this.actionTimer = new System.Timers.Timer
            {
                Interval = 10, // Hundreth of a second
                AutoReset = true // Continuous loop
            };

            // Hook up the elapsed event
            this.actionTimer.Elapsed += this.OnActionTimerElapsed;

            // Set default last selected radio button
            this.lastSelectedRadioButton = this.fromTheHourRadioButton;

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

            /* Load settings */

            // Check for settings file
            if (File.Exists("SettingsData.txt"))
            {
                // Load from disk
                this.settingsData = this.LoadSettingsData();

                // Set numeric up down values
                this.fromTheHourNumericUpDown.Value = this.settingsData.TimeIntervalOption1;
                this.everyIntervalNumericUpDown.Value = this.settingsData.TimeIntervalOption2;
                this.randomIntervalNumericUpDown.Value = this.settingsData.TimeIntervalOption3;

                // Toggle restoring values flag
                this.RestoringValuesFlag = false;

                // Set option state
                switch (this.settingsData.OptionState)
                {
                    // From the hour
                    case 1:

                        // Set radio button
                        this.lastSelectedRadioButton = this.fromTheHourRadioButton;

                        // Halt flow
                        break;

                    // Every interval
                    case 2:

                        // Set radio button
                        this.lastSelectedRadioButton = this.everyIntervalRadioButton;

                        // Halt flow
                        break;

                    // Random interval
                    case 3:

                        // Set radio button
                        this.lastSelectedRadioButton = this.randomIntervalRadioButton;

                        // Halt flow
                        break;
                }

                // Set on state
                if (this.settingsData.OnState == 1)
                {
                    // Check on radio button
                    this.onRadioButton.Checked = true;
                }
            }
        }

        /// <summary>
        /// Handles the action timer elapsed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnActionTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                // Set current DateTime
                DateTime dateTime = DateTime.Now;

                // Only once per second
                if (this.timerElapsedSecond == dateTime.Second)
                {
                    // Halt flow
                    return;
                }

                // Set current second
                this.timerElapsedSecond = dateTime.Second;

                // Set time span to next play
                TimeSpan timeSpan = this.playSoundDateTime - dateTime;

                // Check if must play sound
                if (timeSpan.TotalSeconds < 1)
                {
                    // Get next play DateTime
                    this.playSoundDateTime = this.GetNextPlaySoundDateTime();

                    // Set time span to next play
                    timeSpan = this.playSoundDateTime - dateTime;

                    // Play random sound
                    this.PlayRandomSoundFile();
                }

                // Declare human-readable time string
                string friendlyTimeSpan = string.Empty;

                // Check for hours
                if (timeSpan.Hours > 0)
                {
                    // Set hours
                    friendlyTimeSpan = $"{timeSpan.Hours} hour{(timeSpan.Hours > 1 ? "s" : string.Empty)}, ";
                }

                // Check for minutes
                if (timeSpan.Minutes > 0)
                {
                    // Set minutes
                    friendlyTimeSpan += $"{timeSpan.Minutes} minute{(timeSpan.Minutes > 1 ? "s" : string.Empty)}, ";
                }

                // Check for minutes
                if (timeSpan.Seconds > 0)
                {
                    // Set seconds
                    friendlyTimeSpan += $"{timeSpan.Seconds} second{(timeSpan.Seconds > 1 ? "s" : string.Empty)}.";
                }

                // Fix dangling comma
                if (friendlyTimeSpan.EndsWith(", ", StringComparison.InvariantCulture))
                {
                    // Change to period
                    friendlyTimeSpan = $"{friendlyTimeSpan.Substring(0, friendlyTimeSpan.Length - 2)}.";
                }

                // Set status
                this.mainToolStripStatusLabel.Text = friendlyTimeSpan.Length > 0 ? $"Next play in {friendlyTimeSpan}" : "Now playing...";
            }
            catch (Exception ex)
            {
                // Advise user
                MessageBox.Show($"An error occurred:{Environment.NewLine}{Environment.NewLine}{ex.Message}{Environment.NewLine}{Environment.NewLine}Feel free to report it!", "Action timer error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Plays the passed sound file.
        /// </summary>
        /// <param name="filePath">File path.</param>
        private void PlaySoundFile(string filePath)
        {
            // Dispose of any previous player
            this.DisposeSoundPlayer();

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
        /// Disposes the sound player.
        /// </summary>
        private void DisposeSoundPlayer()
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
        }

        /// <summary>
        /// Scans current directory for sound files.
        /// </summary>
        private void ScanDirectory()
        {
            // Populate sound file list
            this.soundFileList = new List<string>(Directory.GetFiles(Application.StartupPath, "*.wav", SearchOption.AllDirectories));

            // Set scanned files
            this.scannedFiles = $"Found {this.soundFileList.Count} sound files to play";

            // Inform scanned file count
            this.mainToolStripStatusLabel.Text = this.scannedFiles;
        }

        /// <summary>
        /// Handles the checked changed event for both settings radio buttons.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnSettingsRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // Only flow when checked
            if (!((RadioButton)sender).Checked)
            {
                // Halt flow
                return;
            }

            // Set last selected 
            this.lastSelectedRadioButton = (RadioButton)sender;

            // Dispose of any sound player
            this.DisposeSoundPlayer();

            // Check for random interval
            if (this.lastSelectedRadioButton == this.randomIntervalRadioButton)
            {
                // Reset random interval DateTime
                this.randomIntervalDateTime = new DateTime(0001, 1, 1);
            }

            // Set play sound date time value
            this.playSoundDateTime = this.GetNextPlaySoundDateTime();

            // Inform
            this.mainToolStripStatusLabel.Text = "Initializing...";

            /* Save to settings file*/

            // Declare option state; default to zero to prevent use of unasigned local variable
            int optionState = 0;

            // Switch by first letter of name
            switch (this.lastSelectedRadioButton.Name.Substring(0, 1))
            {
                // From the hour
                case "f":

                    // Set to one
                    optionState = 1;

                    // Halt flow
                    break;

                // Every interval
                case "e":

                    // Set to two
                    optionState = 2;

                    // Halt flow
                    break;

                // Random interval
                case "r":

                    // Set to three
                    optionState = 3;

                    // Halt flow
                    break;
            }

            // Set option state
            this.settingsData.OptionState = optionState;

            // Save to settings file
            this.SaveSettingsData();
        }

        /// <summary>
        /// Gets the next play sound date time.
        /// </summary>
        /// <returns>The next play sound date time.</returns>
        private DateTime GetNextPlaySoundDateTime()
        {
            // Set current DateTime
            DateTime dateTime = DateTime.Now;

            // Act upon current checked radio button
            if (this.fromTheHourRadioButton.Checked)
            {
                // Add until modulo is zero
                do
                {
                    // Add one minute
                    dateTime = dateTime.AddMinutes(1);
                }
                while (dateTime.Minute % (int)this.fromTheHourNumericUpDown.Value != 0);

                // Remove seconds
                dateTime = dateTime.AddSeconds(-dateTime.Second);
            }
            else if (this.everyIntervalRadioButton.Checked)
            {
                // Add specific minutes to current date
                dateTime = dateTime.AddMinutes((int)this.everyIntervalNumericUpDown.Value);
            }
            else
            {
                // Set random value to subtract, by next
                int randomNext = this.random.Next(1, (int)this.randomIntervalNumericUpDown.Value * 60);

                // Compare random interval DateTime to current one
                if (this.randomIntervalDateTime.CompareTo(dateTime) < 1)
                {
                    // Set random interval Datetime by adding minutes to now
                    this.randomIntervalDateTime = dateTime.AddMinutes((double)this.randomIntervalNumericUpDown.Value);
                }
                else
                {
                    // Set random interval Datetime by adding minutes to itself
                    this.randomIntervalDateTime = this.randomIntervalDateTime.AddMinutes((double)this.randomIntervalNumericUpDown.Value);
                }

                // Set by subtracting random  seconds (within range)
                dateTime = this.randomIntervalDateTime.AddSeconds(-randomNext);
            }

            // Return processed DateTime
            return dateTime;
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
                this.everyIntervalRadioButton.Checked = false;
                this.randomIntervalRadioButton.Checked = false;
            }

            // Set enabled status for relevant controls
            this.fromTheHourRadioButton.Enabled = isEnabled;
            this.fromTheHourNumericUpDown.Enabled = isEnabled;
            this.fromTheHourMinutesLabel.Enabled = isEnabled;
            this.fromTheHourLabel.Enabled = isEnabled;
            this.everyIntervalRadioButton.Enabled = isEnabled;
            this.everyIntervalNumericUpDown.Enabled = isEnabled;
            this.everyIntervalMinutesLabel.Enabled = isEnabled;
            this.everyIntervalFromNowLabel.Enabled = isEnabled;
            this.randomIntervalRadioButton.Enabled = isEnabled;
            this.randomIntervalInEveryLabel.Enabled = isEnabled;
            this.randomIntervalNumericUpDown.Enabled = isEnabled;
            this.randomIntervalMinuteIntervalLabel.Enabled = isEnabled;
        }

        /// <summary>
        /// Handles the on radio button checked changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOnRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // Act only when checked
            if (!this.onRadioButton.Checked)
            {
                // Halt flow
                return;
            }

            // Guard against empty sounds list
            if (this.soundFileList.Count == 0)
            {
                // Advise user
                MessageBox.Show("Please add sound files (.wav) to current directory.", "Empty sounds list", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                // Focus off radio button
                this.offRadioButton.Checked = true;

                // Halt flow
                return;
            }

            // Toggle colors
            this.onRadioButton.ForeColor = Color.Red;
            this.offRadioButton.ForeColor = Color.Black;
            this.settingsLabel.ForeColor = Color.Red;

            // Enable relevant form controls
            this.EnableDisableControls(true);

            // Check last selected radio button
            this.lastSelectedRadioButton.Checked = true;

            // Set on state in settings
            this.settingsData.OnState = 1;

            // Save settings data to disk
            this.SaveSettingsData();

            // Start timer
            this.actionTimer.Start();
        }

        /// <summary>
        /// Handles the off radio button checked changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnOffRadioButtonCheckedChanged(object sender, EventArgs e)
        {
            // Stop timer
            this.actionTimer.Stop();

            // Dispose of any active sound player
            this.DisposeSoundPlayer();

            // Toggle colors
            this.offRadioButton.ForeColor = Color.Red;
            this.onRadioButton.ForeColor = Color.Black;
            this.settingsLabel.ForeColor = Color.Black;

            // Disable relevant form controls
            this.EnableDisableControls(false);

            // Set off state in settings
            this.settingsData.OnState = 0;

            // Save settings data to disk
            this.SaveSettingsData();

            // Inform scanned file count
            this.mainToolStripStatusLabel.Text = this.scannedFiles;
        }

        /// <summary>
        /// Unchecks the radio buttons.
        /// </summary>
        private void UncheckRadioButtons()
        {
            // Uncheck all radio buttons
            this.fromTheHourRadioButton.Checked = false;
            this.everyIntervalRadioButton.Checked = false;
            this.randomIntervalRadioButton.Checked = false;
        }

        /// <summary>
        /// Handles the numeric up down value changed event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnNumericUpDownValueChanged(object sender, EventArgs e)
        {
            // Check if restoring values
            if (this.RestoringValuesFlag)
            {
                // Halt flow
                return;
            }

            // Clear radio buttons' check state in order to guarantee below's check work
            this.UncheckRadioButtons();

            // Set matching sender, according to numeric up down name, to use switch(string)
            switch (((NumericUpDown)sender).Name)
            {
                // From the hour
                case "fromTheHourNumericUpDown":

                    // Check radio button
                    this.fromTheHourRadioButton.Checked = true;

                    // Time interval option one
                    this.settingsData.TimeIntervalOption1 = (int)this.fromTheHourNumericUpDown.Value;

                    // Halt flow
                    break;

                // Interval
                case "everyIntervalNumericUpDown":

                    // Check radio button
                    this.everyIntervalRadioButton.Checked = true;

                    // Time interval option two
                    this.settingsData.TimeIntervalOption2 = (int)this.everyIntervalNumericUpDown.Value;

                    // Halt flow
                    break;

                // Random
                case "randomIntervalNumericUpDown":

                    // Check radio button
                    this.randomIntervalRadioButton.Checked = true;

                    // Time interval option three
                    this.settingsData.TimeIntervalOption3 = (int)this.randomIntervalNumericUpDown.Value;

                    // Halt flow
                    break;
            }

            // Save new time interval
            this.SaveSettingsData();
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
                $"Minimize icon by made by Gregor Cresnar from www.flaticon.com{Environment.NewLine}" +
                $"https://www.flaticon.com/authors/gregor-cresnar{Environment.NewLine}{Environment.NewLine}" +
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
                $"Made for: JIMDUGGAN{Environment.NewLine}DonationCoder.com{Environment.NewLine}Week #40 @ September 2019",
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

        /// <summary>
        /// Handles the minimize tool strip menu item click event.
        /// </summary>
        /// <param name="sender">Sender object.</param>
        /// <param name="e">Event arguments.</param>
        private void OnMinimizeToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Minimize program window
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// Saves the settings data.
        /// </summary>
        private void SaveSettingsData()
        {
            // Use stream writer
            using (StreamWriter streamWriter = new StreamWriter("SettingsData.txt", false))
            {
                // Set xml serialzer
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsData));

                // Serialize settings data
                xmlSerializer.Serialize(streamWriter, this.settingsData);
            }
        }

        /// <summary>
        /// Loads the settings data.
        /// </summary>
        /// <returns>The settings data.</returns>
        private SettingsData LoadSettingsData()
        {
            // Use file stream
            using (FileStream fileStream = File.OpenRead("SettingsData.txt"))
            {
                // Set xml serialzer
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(SettingsData));

                // Return populated settings data
                return xmlSerializer.Deserialize(fileStream) as SettingsData;
            }
        }
    }
}