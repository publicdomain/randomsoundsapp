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
        /// Gets or sets the on state.
        /// </summary>
        /// <value>The current on state.</value>
        public int OnState { get; set; }

        /// <summary>
        /// Gets or sets the state of the option.
        /// </summary>
        /// <value>The state of the option.</value>
        public int OptionState { get; set; }

        /// <summary>
        /// Gets or sets the time interval option 1.
        /// </summary>
        /// <value>The time interval option 1.</value>
        public int TimeIntervalOption1 { get; set; }

        /// <summary>
        /// Gets or sets the time interval option 2.
        /// </summary>
        /// <value>The time interval option 2.</value>
        public int TimeIntervalOption2 { get; set; }

        /// <summary>
        /// Gets or sets the time interval option 3.
        /// </summary>
        /// <value>The time interval option 3.</value>
        public int TimeIntervalOption3 { get; set; }
    }
}
