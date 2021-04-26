/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 9, 2021</date>
/////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JeremySnyder.Shared.Data.Base.Models;
using Newtonsoft.Json;

namespace JeremySnyder.Security.Data.Models
{
    [Serializable]
    public class UserModel : BaseModelWithID
    {
        [JsonProperty("firstName", Order = 2)]
        public string FirstName { get; set; }

        [JsonProperty("lastName", Order = 3)]
        public string LastName { get; set; }

        [JsonProperty("emailAddress", Order = 4)]
        public string EmailAddress { get; set; }

        [Required]
        [JsonProperty("securityIdentifier", Order = 6)]
        public string SecurityIdentifier { get; set; }

        [JsonProperty("roles")]
        public IList<UserRoleModel> Roles { get; set; } = new List<UserRoleModel>();
    }
}