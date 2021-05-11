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
using JeremySnyder.Shared.Data.Base.Models;

namespace JeremySnyder.Common.Web.Models
{
    /// <summary>
    /// Generic message model to pass a non-error message back to the
    /// frontend in lieu of a return value
    /// </summary>
    [Serializable]
    public class ApiMessage : BaseModel
    {
        [Required]
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
