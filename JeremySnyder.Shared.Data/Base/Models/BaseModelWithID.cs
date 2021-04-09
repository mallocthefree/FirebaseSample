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

namespace JeremySnyder.Shared.Data.Base.Models
{
    [Serializable]
    public abstract class BaseModelWithID : BaseModel
    {
        [Key]
        [JsonProperty("id", Order = 1)]
        public virtual long ID { get; set; }
    }

}
