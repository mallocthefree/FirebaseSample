/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>May 11, 2021</date>
/////////////////////////////////////////////////////////

using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace JeremySnyder.Common.Web.Models
{
    [Serializable]
    public class HostModel
    {
        [Required]
        [JsonProperty("Port")]
        public string Port { get; set; }

        private int _portValue;

        [JsonIgnore]
        public int PortValue
        {
            get
            {
                if (_portValue == 0)
                {
                    int.TryParse(Port, out _portValue);
                }
                
                return _portValue;
            }
        }
    }
}
