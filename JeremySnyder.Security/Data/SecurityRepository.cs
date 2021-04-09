/////////////////////////////////////////////////////////
// <copyright company="Jeremy Snyder Consulting">
// Copyright (c) 2021 All Rights Reserved
// </copyright>
// <author>Jeremy Snyder</author>
// <date>April 9, 2021</date>
/////////////////////////////////////////////////////////

using System.Collections.Generic;
using JeremySnyder.Security.Data.DTO;

namespace JeremySnyder.Security.Data
{
    internal static class SecurityRepository
    {
        public static IEnumerable<UserRoleDTO> GetUserRoles(object userId)
        {
            throw new System.NotImplementedException();
        }

        public static UserDTO FindByEmail(string emailAddress)
        {
            throw new System.NotImplementedException();
        }

        public static void Upsert(object userDTO)
        {
            throw new System.NotImplementedException();
        }

        public static UserDTO FindByExternalId(object firebase, string identifier)
        {
            throw new System.NotImplementedException();
        }
    }
}
