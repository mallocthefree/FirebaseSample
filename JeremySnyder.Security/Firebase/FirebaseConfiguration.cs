/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>March 16, 2021</date>
/////////////////////////////////////////////////////////

using System;
using Newtonsoft.Json;

namespace JeremySnyder.Security.Firebase
{
    [Serializable]
    [JsonObject("FirebaseConfiguration")]
    public class FirebaseConfiguration
    {
        [JsonProperty("comment")]
        public string Comment { get; set; }
        
        /// <summary>
        /// Intentionally vague externally,
        /// but this will only disable security on debug mode
        /// </summary>
        [JsonProperty("disableSecurity")]
        public string DisableSecurity { get; set; }
        
        [JsonProperty("authority")]
        public string Authority { get; set; }
        
        [JsonProperty("issuer")]
        public string Issuer { get; set; }
        
        [JsonProperty("apiKey")]
        public string ApiKey { get; set; }
        
        [JsonProperty("projectId")]
        public string ProjectId { get; set; }

        [JsonProperty("databaseURL")]
        public string DatabaseUrl { get; set; }
        
        [JsonProperty("storageBucket")]
        public string StorageBucket { get; set; }
        
        [JsonProperty("messagingSenderId")]
        public string MessagingSenderId { get; set; }
        
        [JsonProperty("appId")]
        public string AppId { get; set; }
        
        [JsonProperty("keyPath")]
        public string KeyPath { get; set; }
    }
}
