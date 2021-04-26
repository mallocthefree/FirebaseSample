/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 22, 2021</date>
/////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NLog;
using NLog.Extensions.Logging;
using NLog.Targets;
using LogLevel = JeremySnyder.Common.Enums.LogLevel;

namespace JeremySnyder.Common
{
    public static class Logging
    {
        private static Logger _logger;
        
        private static readonly List<string> SensitiveWords = new List<string>
        {
            "password", 
            "pwd",
            "key",
            "ccNumber",
            "cvv",
            "apiKey",
            "ApiLoginID",
            "TransactionKey"
        };

        private static Logger Logger => _logger ??= LoadLogger();

        private static Logger LoadLogger()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            
            LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));

            ValidateRules();
            
            return LogManager.GetCurrentClassLogger();
        }

        private static void ValidateRules()
        {
            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                foreach (var target in rule.Targets.Where(t => t is FileTarget))
                {
                    var fileTarget = (FileTarget) target;
                    var fileName = fileTarget.FileName;
                    CreateLogDirectoryIfNotExists(fileName.ToString()?.Replace("'", string.Empty));
                }
            }
        }

        private static void CreateLogDirectoryIfNotExists(string path)
        {
            try
            {
                var dirs = path.Split("/").ToList();
                if (dirs.Any())
                {
                    dirs.RemoveAt(dirs.Count - 1);
                    path = string.Join("/", dirs);
                }
                DirectoryUtils.CreateDirectoryIfNotExists(path);
            }
            catch (Exception exception)
            {
                // TODO: Send to an alternate messaging system
                // Reasons to be here:
                // 1. No access to file location
                // 2. File is locked by another process
                // 3. Drive is full
            }
        }
        
        public static void Log(LogLevel logLevel, string message)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                    Critical(message);
                    break;
                case LogLevel.Error:
                    Error(message);
                    break;
                case LogLevel.Warning:
                    Warning(message);
                    break;
                case LogLevel.Info:
                    Info(message);
                    break;
                case LogLevel.Debug:
                    Debug(message);
                    break;
                case LogLevel.Trace:
                    Trace(message);
                    break;
                case LogLevel.Sensitive:
                    Sensitive(message);
                    break;
            }

            if (SystemState.LoggingLevel == LogLevel.Debug && logLevel != LogLevel.Sensitive)
            {
                Console.WriteLine(message);
            }
        }

        public static void Sensitive(string[] messageLines, LogLevel logLevel = LogLevel.Info)
        {
            var message = string.Empty;
            foreach (var line in messageLines)
            {
                var filteredLine = FilterKeyWords(line);
                message = $"{message}{Environment.NewLine}{filteredLine}";
            }
            
            message = $"{message}{Environment.NewLine}";

            if (logLevel == LogLevel.Sensitive)
            {
                logLevel = LogLevel.Info;
            }
            
            Log(logLevel, message);
        }
        
        public static void Sensitive(string message, LogLevel logLevel = LogLevel.Info)
        {
            message = FilterKeyWords(message);

            if (logLevel == LogLevel.Sensitive)
            {
                logLevel = LogLevel.Info;
            }
            
            Log(logLevel, message);
        }

        public static void Trace(string message)
        {
            Logger.Trace(message);
        }

        public static void Debug(string message)
        {
            Logger.Debug(message);
        }

        public static void Info(string message)
        {
            Logger.Info(message);
       }
        
        public static void Warning(string message)
        {
            Logger.Warn(message);
        }

        public static void Error(string message)
        {
            Logger.Error(message);
        }

        public static void Critical(string message)
        {
            Logger.Fatal(message);
        }
        
        #region Filter

        private static string FilterKeyWords(string message)
        {
            string result = "";

            var splitLine = message.Split(';');
            foreach (var word in splitLine)
            {
                string finalWord = word;
                foreach (var sensitiveWord in SensitiveWords)
                {
                    if (word.Contains(sensitiveWord, StringComparison.CurrentCultureIgnoreCase))
                    {
                        int wordLength = sensitiveWord.Length;
                        int messageLength = word.Length;
                        int firstPoint = word.IndexOf(sensitiveWord, StringComparison.InvariantCultureIgnoreCase);

                        string left = word.Substring(0, firstPoint + wordLength);
                        string pad = new string('*', messageLength - left.Length);
                        finalWord = left + pad;
                    }
                }

                result = $"{result} {finalWord}";
            }

            return result;
        }
        #endregion
    }
}
