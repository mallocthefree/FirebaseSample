/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 9, 2021</date>
/////////////////////////////////////////////////////////

using System;
using JeremySnyder.Shared.Data.Base.Models;
using Newtonsoft.Json;

namespace JeremySnyder.Security.Data.Models
{
    [Serializable]
    public class UserRoleModel : BaseModel
    {
        [JsonProperty("roleID")]
        public long RoleID { get; set; }
        
        [JsonProperty("roleName")]
        public string RoleName { get; set; }
    }
}