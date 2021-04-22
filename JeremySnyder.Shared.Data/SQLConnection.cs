/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 22, 2021</date>
/////////////////////////////////////////////////////////

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using JeremySnyder.Common;

namespace JeremySnyder.Shared.Data
{
    public class SQLConnection : IDisposable
    {
        public readonly IDbConnection Connection;
        private const string DefaultConnectionLabel = "DefaultConnection";

        private static SQLConfiguration _defaultConnection;
        private static SQLConfiguration DefaultConnection
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_defaultConnection?.GetConnectionString()))
                {
                    var defaultConnectionName = ConfigurationLoader.GetValue(DefaultConnectionLabel);
                    _defaultConnection = ConfigurationLoader.GetSection<SQLConfiguration>(defaultConnectionName);
                }

                return _defaultConnection;
            }
        }

        /// <summary>
        /// Instantiate a SQLConnection object from a name correlating to
        /// named configuration in appsettings.json
        /// </summary>
        /// <param name="connectionName">Name of the connection configuration to pull the values from</param>
        /// <param name="commandTimeout">
        /// Time in seconds before we give up on a command run and call a time out. Default is 90 seconds
        /// </param>
        public SQLConnection(string connectionName = "", int commandTimeout = 90)
        {
            Logging.Trace($"Using database connection name [{connectionName}] with command timeout [{commandTimeout.ToString()}] ");
            var configuration =  GetConfiguration(connectionName);
            if (configuration == null)
            {
                const string error = "SQL Connection Configuration is invalid or missing";
                
                Logging.Critical(error);
                throw new Exception(error);
            }
            
            configuration.CommandTimeout = commandTimeout.ToString(CultureInfo.InvariantCulture);
            
            Connection = BuildConnection(configuration);
        }

        /// <summary>
        /// Create a <seealso cref="SQLConnection"/> object from a pre-created version of the
        /// <seealso cref="SQLConfiguration"/>, possibly by a Unit Test
        /// </summary>
        /// <param name="configuration"><see cref="SQLConfiguration"/> object</param>
        public SQLConnection(SQLConfiguration configuration)
        {
            Connection = BuildConnection(configuration);
        }

        /// <summary>
        /// Builds the connection object using the established connection information
        /// via configuration source (e.g. appsettings.json)
        ///
        /// </summary>
        /// <param name="configuration">The configuration established, likely from an appsettings.json</param>
        /// <returns>The connection object, which should be used within a using statement</returns>
        private static IDbConnection BuildConnection(SQLConfiguration configuration)
        {
            return new SqlConnection(configuration.GetConnectionString());
        }
        
        /// <summary>
        /// Get the <seealso cref="SQLConfiguration"/> object by name from
        /// the appsettings.json file
        /// 
        /// </summary>
        /// <param name="connectionName">String value associated to the configuration set by name</param>
        /// <returns>
        /// <see cref="SQLConfiguration"/> of the configuration
        /// setting to be used in creation of a <seealso cref="SQLConnection"/> object
        /// </returns>
        public static SQLConfiguration GetConfiguration(string connectionName = "")
        {
            // If a developer is running in Debug mode, then use the local dev connection configuration
            // This intent should help prevent, or at least reduce, accidental check-ins of developer configurations
            // that could impact QA & production environments
            connectionName = Debugger.IsAttached
                ? "LocalDevConnection"
                : !string.IsNullOrWhiteSpace(connectionName)
                    ? connectionName
                    : ConfigurationLoader.GetValue(DefaultConnectionLabel);

            Logging.Trace($"Final database connection name is [{connectionName}]");

            try
            {
                var connection = ConfigurationLoader.GetSection<SQLConfiguration>(connectionName);
                if (!string.IsNullOrWhiteSpace(connection.GetConnectionString()))
                {
                    return connection;
                }

                Logging.Trace($"[{connectionName}] is missing. Using [{DefaultConnection.GetConnectionString()}]");
                return DefaultConnection;
            }
            catch (Exception)
            {
                Logging.Trace($"[{connectionName}] is missing. Using [{DefaultConnection.GetConnectionString()}]");
                return DefaultConnection;
            }
        }

        /// <summary>
        /// Cleanup at the end of a scope of a "using" statement
        /// </summary>
        public void Dispose()
        {
            Connection?.Dispose();
        }
    }
}
