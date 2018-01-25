using System;

namespace SettingsLoader.Exceptions
{
    /// <summary>
    /// Exception thrown when loading a setting that does not exist into a SettingsLoader-derived class
    /// </summary>
    public class NonExistingSettingException : Exception
    {
        private const string MESSAGE_BASE = "Setting with name {0} does not exist";

        /// <summary>
        /// Build a new NonExistingSettingException with a setting name
        /// </summary>
        /// <param name="settingName">The name of the non-existing setting</param>
        public NonExistingSettingException(string settingName) : base(string.Format(MESSAGE_BASE, settingName))
        {
        }
    }
}