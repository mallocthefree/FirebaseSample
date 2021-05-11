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

namespace JeremySnyder.Shared.Data.Base.Models
{
    [Serializable]
    public abstract class BaseModelWithDescriptionAndID : BaseModelWithID
    {
        [Required]
        [JsonProperty("description", Order = 2)]
        public string Description { get; set; }
        
        [JsonProperty("active")]
        public int Active { get; set; }
    }
}
