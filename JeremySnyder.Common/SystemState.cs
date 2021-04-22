/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 22, 2021</date>
/////////////////////////////////////////////////////////

using System;
using JeremySnyder.Common.Enums;

namespace JeremySnyder.Common
{
    public static class SystemState
    {
        private static string _environmentName;

        public static string EnvironmentName
        {
            get => _environmentName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _environmentName = value;
                }
                else if (value.Equals("Debug", StringComparison.CurrentCultureIgnoreCase))
                {
                    LoggingLevel = LogLevel.Debug;
                    _environmentName = "Development";
                    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", _environmentName);
                }
                else
                {
                    _environmentName = value;
                }
            }
        }
        
        public static LogLevel LoggingLevel { get; private set; }
    }
}
