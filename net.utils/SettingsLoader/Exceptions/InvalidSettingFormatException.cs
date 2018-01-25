using System;

namespace SettingsLoader.Exceptions
{
    /// <summary>
    /// Exception thrown when loading a setting with an invalid format into a SettingsLoader-derived class
    /// e.g. trying to load "abc" into an int property
    /// </summary>
    public class InvalidSettingFormatException : Exception
    {
        private const string MESSAGE_BASE = "Setting with name {0} has an invalid format for its expected type";

        /// <summary>
        /// Build a new InvalidSettingFormatException with a setting name and an inner exception explaining what happened
        /// </summary>
        /// <param name="settingName"></param>
        /// <param name="innerException"></param>
        public InvalidSettingFormatException(string settingName, Exception innerException)
            : base(string.Format(MESSAGE_BASE, settingName), innerException)
        {
        }
    }
}