/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>March 16, 2021</date>
/////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace JeremySnyder.Common
{
    public static class ConfigurationLoader
    {
        private const string SettingsFileName = "appsettings.json";
        private static IConfiguration _configuration;

        static ConfigurationLoader()
        {
            LoadSettings();
        }
        
        /// <summary>
        /// Load appSettings.json and maintain <see cref="IConfiguration"/>
        /// for value references via <seealso cref="IConfigurationSection"/>
        /// </summary>
        private static void LoadSettings()
        {
            var path = Directory.GetParent(AppContext.BaseDirectory).FullName;
            _configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile(SettingsFileName, false, true)
                .Build();
        }
        
        /// <summary>
        /// Get a section of the configuration converted into a configuration model
        /// as defined by the template T
        /// </summary>
        /// <param name="sectionName">The name of the section to query</param>
        /// <typeparam name="T">Class representing the expected configuration section values</typeparam>
        /// <returns>An instantiation of a class instantiation of the T template</returns>
        /// <exception cref="Exception">Generic exception thrown if the section is not found</exception>
        public static T GetSection<T>(string sectionName)
        {
            if (_configuration == null)
            {
                LoadSettings();
            }
            
            if (_configuration == null)
            {
                throw new Exception($"{SettingsFileName} not found");
            }
            
            if (string.IsNullOrEmpty(sectionName))
            {
                throw new Exception($"Section [{sectionName}] not found");
            }

            return (T)_configuration.GetSection(sectionName).Convert<T>();
        }
        
        /// <summary>
        /// Using <see cref="JsonPropertyAttribute"/> on the template class, map the
        /// <seealso cref="IConfigurationSection"/> values to an instantiation of class
        /// defined by template T
        ///
        /// Note:
        /// Simple conversion that does not handle anything more complicated than a string value
        /// 
        /// </summary>
        /// <param name="config">The <see cref="IConfigurationSection"/> to interpret and convert</param>
        /// <typeparam name="T">The template stand-in for the class to be instantiated and converted into</typeparam>
        /// <returns>The instantiated object cast down from class template T</returns>
        private static object Convert<T>(this IConfiguration config)
        {
            var type = typeof(T);
            object result = Activator.CreateInstance(type);

            foreach (var propertyInfo in type.GetProperties())
            {
                if (!propertyInfo.CanWrite)
                {
                    continue;
                }
                
                var jsonPropertyAttribute = (JsonPropertyAttribute)
                    propertyInfo.GetCustomAttributes(typeof(JsonPropertyAttribute), true)
                                .FirstOrDefault();
                var propertyName = jsonPropertyAttribute?.PropertyName;

                if (propertyName != null)
                {
                    object jsonValue = config[propertyName];
                    if (jsonValue != null)
                    {
                        propertyInfo.SetValue(result, jsonValue);
                    }
                }
            }

            return result;
        }
        
        public static string GetValue(string section, string defaultValue = "")
        {
            return (string)_configuration.GetValue(typeof(string), section, defaultValue);
        }
    }
}
