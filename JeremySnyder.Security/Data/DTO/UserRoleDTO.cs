/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 9, 2021</date>
/////////////////////////////////////////////////////////

using JeremySnyder.Shared.Data.Base.DTO;

namespace JeremySnyder.Security.Data.DTO
{
    internal class UserRoleDTO : BaseDTO
    {
        public long UserID { get; set; }
        public long RoleID { get; set; }
        public string RoleName { get; set; }
    }
}
