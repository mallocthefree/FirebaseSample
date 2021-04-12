/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 9, 2021</date>
/////////////////////////////////////////////////////////

using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace JeremySnyder.Common.Database
{
    public class SQLConfiguration
    {
        [JsonProperty("Comment")]
        public string Comment { get; set; }
        
        [JsonProperty("UseProxy")]
        public string Proxy { get; set; }
        
        [JsonProperty("CommandTimeout")]
        public string CommandTimeout { get; set; }
        
        [JsonProperty("ConnectionString")]
        public string ConnectionString { get; set; }

        public SQLConfiguration()
        {
            CommandTimeout = "90";
        }

        public string GetConnectionString()
        {
            string connectionString = ConnectionString;
            if (!string.IsNullOrWhiteSpace(connectionString) &&
                !connectionString.Contains("CommandTimeout", StringComparison.InvariantCultureIgnoreCase))
            {
                connectionString = $"{connectionString}; CommandTimeout={CommandTimeout}";
            }

            return connectionString;
        }
    }
}
