/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>May 11, 2021</date>
/////////////////////////////////////////////////////////

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using JeremySnyder.Common.Web.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Logger = JeremySnyder.Common.Logging;

namespace JeremySnyder.Common.Web
{
    [ExcludeFromCodeCoverage]
    public abstract class CommonWebProgram
    {
        public static IConfiguration Configuration { get; set; }
        
        /// <summary>
        /// Set up logging levels and other logging configurations using the chosen logging system
        /// </summary>
        /// <param name="appSettingsPath">Full path to the appsettings.json file, including the file name</param>
        private static void LogSettings(string appSettingsPath)
        {
            if (!File.Exists(appSettingsPath))
            {
                Logger.Critical($"***** appsettings path does not exist: [{appSettingsPath}] *****");
            }
            else
            {
                string fileName = Path.GetFileName(appSettingsPath);
                string message = $"**** {fileName} content ****";
                string asteriskFrame = new string('*', message.Length);
                Logger.Info(asteriskFrame);
                Logger.Info(message);
                Logger.Info(asteriskFrame);
                Logger.Info($"Full path: {appSettingsPath}");

                var allLines = File.ReadAllLines(appSettingsPath);
                
                Logger.Sensitive(allLines);
                Logger.Info(asteriskFrame);
            }
        }

        /// <summary>
        /// Sets up configurations I like to apply to most of my web APIs, so why not write it only once?
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected static IWebHostBuilder CommonWebHostBuilder(string[] args)
        {
            SystemState.EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            Logging.Info($"Environment: [{SystemState.EnvironmentName}]");

            string appSettingsPath = $"{AppDomain.CurrentDomain.BaseDirectory}appsettings.json";
            LogSettings(appSettingsPath);

            Configuration = new ConfigurationBuilder()
                .AddJsonFile(appSettingsPath, optional: true, reloadOnChange: true)  
                .Build();

            var config = ConfigurationLoader.GetSection<HostModel>("Host");
            int port = config.PortValue;
            
            Logger.Debug($"Port defined as: [{port.ToString()}]");

            return WebHost.CreateDefaultBuilder(args)
                .UseContentRoot(AppDomain.CurrentDomain.BaseDirectory)
                .ConfigureServices(services => Console.WriteLine($"Service Count: [{services.Count.ToString()}]"))
                .UseConfiguration(Configuration)
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, port);
                });
        }
    }
}
