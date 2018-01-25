using System;

namespace SettingsLoader.Exceptions
{
    /// <summary>
    /// Exception thrown when loading a setting with an invalid format into a SettingsLoader-derived class
    /// e.g. trying to load "abc" into an int property
    /// </summary>
    public class InvalidSettingValueException : Exception
    {
        private const string MESSAGE_BASE = "Setting with name {0} has an invalid value: {1}";

        /// <summary>
        /// Build a new InvalidSettingValueException with a setting name
        /// </summary>
        /// <param name="settingName">The name of the setting with the invalid value</param>
        /// <param name="reason">The reason why the value is invalid</param>
        public InvalidSettingValueException(string settingName, string reason)
            : base(string.Format(MESSAGE_BASE, settingName, reason))
        {
        }
    }
}