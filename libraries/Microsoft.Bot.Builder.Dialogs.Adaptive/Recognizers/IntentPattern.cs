﻿using System.Text.RegularExpressions;

namespace Microsoft.Bot.Builder.Dialogs.Adaptive.Recognizers
{
    /// <summary>
    /// Represents pattern for an intent.
    /// </summary>
    public class IntentPattern
    {
        private Regex regex;
        private string pattern;

        public IntentPattern()
        {
        }

        public IntentPattern(string intent, string pattern)
        {
            this.Intent = intent;
            this.Pattern = pattern;
        }

        /// <summary>
        /// Gets or sets the intent.
        /// </summary>
        /// <value>
        /// The intent.
        /// </value>
        public string Intent { get; set; }

        /// <summary>
        /// Gets or sets the regex pattern to match.
        /// </summary>
        /// <value>
        /// The regex pattern to match.
        /// </value>
        public string Pattern
        {
            get
            {
                return pattern;
            }

            set
            {
                this.pattern = value;
                this.regex = new Regex(pattern, RegexOptions.Compiled);
            }
        }

        public Regex Regex => this.regex;
    }
}
