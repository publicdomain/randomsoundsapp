// <copyright file="SettingsData.cs" company="PublicDomain.com">
//     CC0 1.0 Universal (CC0 1.0) - Public Domain Dedication
//     https://creativecommons.org/publicdomain/zero/1.0/legalcode
// </copyright>

namespace RandomSoundsApp
{
    // Directives
    using System;

    /// <summary>
    /// Settings data.
    /// </summary>
    public class SettingsData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:RandomSoundsApp.SettingsData"/> class.
        /// </summary>
        public SettingsData()
        {
            // Parameterless constructor for serialization to work
        }

        /// <summary>
        /// Gets or sets the on state.
        /// </summary>
        /// <value>The current on state.</value>
        public int OnState { get; set; } = 0;

        /// <summary>
        /// Gets or sets the state of the option.
        /// </summary>
        /// <value>The state of the option.</value>
        public int OptionState { get; set; } = 1;

        /// <summary>
        /// Gets or sets the time interval option 1.
        /// </summary>
        /// <value>The time interval option 1.</value>
        public int TimeIntervalOption1 { get; set; } = 15;

        /// <summary>
        /// Gets or sets the time interval option 2.
        /// </summary>
        /// <value>The time interval option 2.</value>
        public int TimeIntervalOption2 { get; set; } = 15;

        /// <summary>
        /// Gets or sets the time interval option 3.
        /// </summary>
        /// <value>The time interval option 3.</value>
        public int TimeIntervalOption3 { get; set; } = 15;
    }
}
