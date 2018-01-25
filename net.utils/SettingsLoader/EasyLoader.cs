using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;

using SettingsLoader.Exceptions;

namespace SettingsLoader
{
    public abstract class EasyLoader
    {
        /// <inheritdoc />
        /// <summary>
        /// Build a settings class using the settings defined in the AppSettings config section.
        /// Use the property names to assign setting values.
        /// Call this constructor from a derived class
        /// </summary>
        protected EasyLoader() : this(ConfigurationManager.AppSettings)
        {
        }

        /// <summary>
        /// Build a settings class using any given NameValueCollection.
        /// Useful for testing or for other unforeseen scenarios.
        /// Use the property names to assign setting values.
        /// Call this constructor from a derived class
        /// </summary>
        /// <param name="settings">Collection of settings to import to this class</param>
        protected EasyLoader(NameValueCollection settings)
        {
            foreach (var propertyInfo in GetType().GetProperties())
            {
                var propertyType = propertyInfo.PropertyType;
                var value = settings[propertyInfo.Name];

                if (value == null)
                    throw new NonExistingSettingException(propertyInfo.Name);

                try
                {
                    object finalValue;

                    //Check if the property is an IEnumerable, ICollection, IEnumerable<T> or ICollection<T>
                    if (propertyType != typeof(string) && ( //Strings are IEnumerable<char>... and we don't want that.
                            typeof(IEnumerable).IsAssignableFrom(propertyType) ||
                            typeof(ICollection).IsAssignableFrom(propertyType)
                        ))
                    {
                        Type elementType = GetPropertyElementType(propertyType);

                        //Split the setting into elements
                        IEnumerable<string> listValues = SplitStringValues(value);

                        finalValue = CreateTypedList(listValues, elementType);
                    }
                    else
                        finalValue = Convert.ChangeType(value, propertyInfo.PropertyType, CultureInfo.InvariantCulture);

                    propertyInfo.SetValue(this, finalValue);
                }
                catch (Exception exception)
                {
                    throw new InvalidSettingFormatException(propertyInfo.Name, exception);
                }
            }

            Validate();
        }

        /// <summary>
        /// Validation method for settings. Called automatically in the constructor.
        /// </summary>
        /// <exception cref="InvalidSettingValueException">When a setting's value does not match what is expected</exception>
        protected abstract void Validate();

        protected void ValidateSetting(string name, string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new NonExistingSettingException(name);
        }

        protected void ValidateUri(string name, string value)
        {
            ValidateSetting(name, value);

            Uri csharp7Please;

            if (!Uri.TryCreate(value, UriKind.Absolute, out csharp7Please))
                throw new InvalidSettingValueException(name, value);
        }

        protected void ValidateDirectorySetting(string name, string value)
        {
            ValidateSetting(name, value);

            if (Directory.Exists(value) == false)
                throw new InvalidSettingValueException(name, value);
        }

        protected void ValidateNumericValue(string name, int value)
        {
            ValidateSetting(name, value.ToString());

            if (value <= 0)
                throw new InvalidSettingValueException(name, $"{nameof(value)} cannot be less or equal to 0");
        }

        protected void ValidateNumericValue(string name, decimal value)
        {
            ValidateSetting(name, value.ToString());

            if (value <= 0)
                throw new InvalidSettingValueException(name, $"{nameof(value)} cannot be less or equal to 0");
        }

        //Get the type of a (list, array) property's elements
        private Type GetPropertyElementType(Type propertyType)
        {
            Type elementType = typeof(object); //Default to object if the property is not generic
            if (propertyType.IsGenericType)
                elementType = propertyType.GetGenericArguments()[0];

            return elementType;
        }

        private IEnumerable<string> SplitStringValues(string value)
        {
            const char SETTING_LIST_SEPARATOR = ',';

            string[] listValues;

            if (value.Trim() == string.Empty)
                listValues = new string[] { }; //If it's empty, then there are no elements
            else
                listValues = value.Split(SETTING_LIST_SEPARATOR);

            return listValues;
        }

        private object CreateTypedList(IEnumerable<string> values, Type elementType)
        {
            //Create a list of the required type
            var listType = typeof(List<>);
            var constructedListType = listType.MakeGenericType(elementType);
            var list = (IList)(Activator.CreateInstance(constructedListType));

            foreach (var element in values)
                list.Add(Convert.ChangeType(element.Trim(), elementType, CultureInfo.InvariantCulture));

            return list;
        }
    }
}
