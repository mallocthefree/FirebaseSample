/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>May 11, 2021</date>
/////////////////////////////////////////////////////////

using System;
using Newtonsoft.Json;
using JeremySnyder.Shared.Data.Base.Models;

namespace JeremySnyder.Common.Web.Models
{
    /// <summary>
    /// Return an error code to the front end
    /// </summary>
    [Serializable]
    public class ErrorViewModel : BaseModel
    {
        [JsonProperty("requestId")]
        public string RequestId { get; set; }

        [JsonProperty("showRequestId")]
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
